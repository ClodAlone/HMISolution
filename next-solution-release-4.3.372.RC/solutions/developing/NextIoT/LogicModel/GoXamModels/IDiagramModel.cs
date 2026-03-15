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

namespace Northwoods.GoXam.Model {

  //??? XML data, relational data, DataProviders

  //?? problem notifying model when a data property (or other state) changes
  //??? how to force WPF rebinding when data does not implement INotifyPropertyChanged

  //??? support for DB transactions

  //?? problems with serialization

  //??? support for virtualizing

  //??? handle shared nested nodes and links: Sets

  /// <summary>
  /// All diagram models implement this interface or an interface that inherits from this interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is reponsible for defining relationships between nodes, involving links and grouping.
  /// It does not know about any <c>FrameworkElement</c>s
  /// (including <see cref="Diagram"/> or <see cref="DiagramPanel"/> or <see cref="Part"/>).
  /// It just knows about .NET CLR objects as data of type <see cref="Object"/> or about more specific types
  /// when instantiating one of the generic <see cref="DiagramModel"/> classes.
  /// It is vaguely analogous to a <c>CollectionView</c>, providing additional organization of the source data.
  /// It defines a graph from the data instead of imposing a total ordering on it and filtering/sorting/grouping it.
  /// It also supports two-pass copying of existing data and undo/redo support via an <see cref="UndoManager"/>.
  /// </para>
  /// <para>
  /// There are four categories of members in this interface: updating, navigation, modification, and miscellaneous.
  /// The updating methods need to be called when there has been a change to the data,
  /// so that the model can be kept up-to-date with the data.
  /// The navigation methods support examining the diagram and traversing the graph defined
  /// by the nodes and links in the diagram.
  /// The modification methods are used to alter the diagram.
  /// The miscellaneous methods and properties include support for model change notification,
  /// for undo/redo, for edits, and for data transfer.
  /// Additional categories of members exist in each of the model classes.
  /// </para>
  /// <para>
  /// The most important member of this interface is the <see cref="NodesSource"/> property,
  /// the collection of node data defining the graph.
  /// You will need to set it before anything can be seen in your <see cref="Diagram"/>.
  /// The model will look at each node data in that collection to discover the
  /// relationships that will form the graph.
  /// The methods and properties for discovering a graph from the data depend on
  /// the kind of model, so they are not part of this interface.
  /// </para>
  /// <para>
  /// As data is added or removed from the model, or as data is modified, the graph implied
  /// by the data changes.  Whenever the data is modified, the model must be notified.
  /// The model interfaces (both this interface and more specific interfaces inheriting
  /// from this interface) have a category of methods for being notified of data changes.
  /// This basic interface includes methods such as <see cref="DoNodeAdded"/>,
  /// <see cref="DoNodeRemoved"/>, and <see cref="DoNodeKeyChanged"/>.
  /// More specific kinds of models have additional methods, such as:
  /// <see cref="ITreeModel.DoParentNodeChanged"/>,
  /// <see cref="IConnectedModel.DoToNodeKeyRemoved"/>,
  /// <see cref="ILinksModel.DoLinkAdded"/>,
  /// <see cref="IGroupsModel.DoMemberNodeKeysChanged"/>, and
  /// <see cref="ISubGraphModel.DoGroupNodeChanged"/>.
  /// Updating methods have names that start with "Do".
  /// </para>
  /// <para>
  /// Of course if your data is unchanging, neither by application code nor by
  /// the user, you will not need to be concerned about keeping the model up-to-date.
  /// But if your data might change, .NET provides some standard interfaces for change
  /// notification that you should use if you want successful data binding.
  /// </para>
  /// <para>
  /// The <c>INotifyPropertyChanged</c> interface (in the
  /// <c>System.ComponentModel</c> namespace) is commonly used for
  /// notification of property changes.  The diagram model classes automatically
  /// register themselves as <c>INotifyPropertyChanged.PropertyChanged</c> event
  /// listeners for each of the data objects that are in the <see cref="NodesSource"/>
  /// collection so that they can be updated if a model-relevant property changes
  /// in a data object.
  /// We suggest that you have your node data class implement this interface,
  /// but that you use the <see cref="ModelChangedEventArgs"/> class for event arguments
  /// instead of <c>PropertyChangedEventArgs</c>.
  /// </para>
  /// <para>
  /// In addition, Microsoft recommends implementing the  <c>INotifyCollectionChanged</c>
  /// interface (in the <c>System.Collections.Specialized</c> namespace) for providing
  /// change notification of collection objects.
  /// .NET offers the <c>ObservableCollection&lt;T&gt;</c> class as a standard
  /// collection that implements <c>INotifyCollectionChanged</c>.
  /// So if the collection you supply as the value of <see cref="NodesSource"/> implements
  /// this interface, the model will automatically call the <see cref="DoNodeAdded"/>
  /// and <see cref="DoNodeRemoved"/> methods for you.
  /// Similarly, for the data properties that are expected to be collections,
  /// (such as the collection of tree children nodes for a data node in the tree model),
  /// we suggest you use <c>ObservableCollection&lt;T&gt;</c>.
  /// Note that the optional data classes that we provide for your use, if you do not already
  /// have your own data classes, implement <c>INotifyPropertyChanged</c> and make use
  /// of <c>ObservableCollection&lt;T&gt;</c>.
  /// An example is <see cref="Northwoods.GoXam.Model.TreeModelNodeData{NodeKey}"/>.
  /// </para>
  /// <para>
  /// The generic model classes have a parameterized type, <c>NodeType</c>,
  /// that denotes the type of the node data that it contains.
  /// That of course is the required type of each item in the <see cref="NodesSource"/> collection.
  /// Furthermore, all of the methods that operate on node data take and/or return that type.
  /// This design provides for better type checking at compile time and potentially
  /// better performance at run time.
  /// </para>
  /// <para>
  /// But <see cref="IDiagramModel"/> and its subinterfaces are not generic.
  /// Their methods all take and/or return data of type <c>Object</c> instead of a
  /// parameterized type.
  /// This "universality" is necessary because the diagram control is not a generic class.
  /// </para>
  /// <para>
  /// Unlike most controls that bind to a list of items, diagrams involve relationships
  /// between the items that are much more general than just the order in the list.
  /// This is achieved by interpreting some property values as "references" to other data.
  /// Such references might be implemented as .NET CLR references (i.e. pointers).
  /// However, it is also common to use other values, such as strings or numbers,
  /// as the references.  Such a scheme requires being able to identify each data
  /// object with a unique value.
  /// </para>
  /// <para>
  /// The generic model class have another parameterized type, <c>NodeKey</c>,
  /// that denotes the type of the values that refer to node data.
  /// When the abstract references are actually .NET CLR references, the <c>NodeKey</c>
  /// type will be the same as the <c>NodeType</c> type.
  /// But more often the <c>NodeKey</c> will be something like <c>String</c>
  /// or <c>int</c>.
  /// </para>
  /// <para>
  /// Normally the expectation is that the key value for each node data does not change.
  /// However, the models do support such a circumstance -- make sure the
  /// <see cref="DoNodeKeyChanged"/> method is called right after changing a node's key.
  /// </para>
  /// <para>
  /// The <see cref="FindNodeByKey"/> method is useful for finding a node data given its key.
  /// To find out if a node data is in the <see cref="NodesSource"/> collection, call the
  /// <see cref="IsNodeData"/> predicate.
  /// </para>
  /// <para>
  /// The models have a number of methods that are useful for examining or navigating.
  /// These method names tend to start with "Get" or "Is".
  /// For "link" relationships, the following methods apply to all models:
  /// <see cref="IsLinked"/>, <see cref="GetFromNodesForNode"/>,
  /// <see cref="GetToNodesForNode"/>, <see cref="GetConnectedNodesForNode"/>.
  /// Particular kinds of models have additional methods for traversing graphs, such as:
  /// <see cref="ITreeModel.GetChildrenForNode"/>,
  /// <see cref="ILinksModel.GetLinksBetweenNodes"/>,
  /// <see cref="ILinksModel.GetToNodeForLink"/>,
  /// <see cref="ILinksModel.GetLabelNodeForLink"/>,
  /// <see cref="IGroupsModel.GetMemberNodesForGroup"/>, and
  /// <see cref="ISubGraphModel.GetGroupForNode"/>.
  /// Note that the models will have similar protected (not public)
  /// methods that actually implement the corresponding behavior on the data.
  /// There will be virtual so that your model implementation can customize
  /// or optimize how the method performs the operation.
  /// </para>
  /// <para>
  /// The models also have methods for modifying.
  /// These method names start with "Modify", "Add", or "Remove".
  /// For nodes, there are <see cref="AddNode"/> and <see cref="RemoveNode"/>.
  /// For links, there are <see cref="AddLink"/> and <see cref="RemoveLink"/>.
  /// Furthermore, particular kinds of models have additional methods for
  /// changing relationships, such as:
  /// <see cref="ITreeModel.AddChildNodeKey"/>,
  /// <see cref="IConnectedModel.AddFromNodeKey"/>,
  /// <see cref="ILinksModel.SetLinkLabel"/>,
  /// <see cref="IGroupsModel.RemoveMemberNodeKey"/>, and
  /// <see cref="ISubGraphModel.SetGroupNode"/>.
  /// Note that the models will have similar protected (not public)
  /// methods that actually implement the corresponding behavior on the data.
  /// There will be virtual so that your model implementation can customize
  /// or optimize how the method performs the modification.
  /// </para>
  /// <para>
  /// Finally, there is the <see cref="Modifiable"/> property, that should
  /// disable user actions that would modify the model, and that causes
  /// errors when you call a model-modifying method anyway.
  /// </para>
  /// <para>
  /// Models support a two-pass copying process.
  /// A single pass copy is insufficient because some "references"
  /// cannot be made before all objects have been copied -- those references
  /// need to be fixed-up afterwards, in a second pass.
  /// Let's say that we have two objects to be copied, A and B, and that
  /// A has a string property that refers to B by its name, "B".
  /// After making the two copied objects, A2 and B2, A2's reference
  /// will typically still be "B".  The second copy pass will operate
  /// on A2 and give it a chance to look for the original "B" data,
  /// to find the copied B2 data, and to change A2's reference to be "B2".
  /// </para>
  /// <para>
  /// The principal copying method is <see cref="AddCollectionCopy"/>.
  /// This first makes copies of all the data to be copied, and then
  /// iterates over them again, fixing up the references.
  /// You may need to override protected virtual methods whose names
  /// start with "Copy" in order to correctly construct copies of your data
  /// in the first pass, and in order to correctly fix references to other
  /// data in the second pass.
  /// </para>
  /// <para>
  /// Even after the two-pass copying process has finished, there may still
  /// be unresolved references.
  /// <see cref="ClearUnresolvedReferences"/>
  /// </para>
  /// <para>
  /// For convenience in making a copy of a node and adding it to the model,
  /// there is also the <see cref="AddNodeCopy"/> method.
  /// </para>
  /// <para>
  /// Depending on the application, not all relationships should be possible.
  /// Models provide support for link relationships with the <see cref="IsLinkValid"/> method.
  /// There are more specific model predicates for <c>IsRelinkValid</c>:
  /// <see cref="ILinksModel.IsRelinkValid"/>, <see cref="IConnectedModel.IsRelinkValid"/>,
  /// and <see cref="ITreeModel.IsRelinkValid"/>.
  /// <see cref="IGroupsModel.IsMemberValid"/> offers similar support for the
  /// group-member relationship in <see cref="IGroupsModel"/>.
  /// The diagram control uses these methods for deciding which user actions
  /// are permissible.
  /// The model classes add support for customizing these methods.
  /// </para>
  /// <para>
  /// Each model is not only a consumer of events on its data but is also a generator
  /// <see cref="Changed"/> events, to allow model consumers such as the diagram control
  /// to keep themselves up-to-date with the model.
  /// <see cref="ModelChange"/> is an enumeration that lists all of the kinds of changes
  /// that can happen to the predefined model classes.
  /// Whenever there is a change to a model, the code calls <see cref="RaiseChanged"/>
  /// or a similar method to notify model consumers.
  /// The <see cref="IsModified"/> property is automatically set to true as
  /// <see cref="Changed"/> events are raised.
  /// </para>
  /// <para>
  /// Each model also supports undo and redo, using an
  /// <see cref="Northwoods.GoXam.Model.UndoManager"/> that records
  /// <see cref="Changed"/> events.
  /// Because <see cref="ModelChangedEventArgs"/> also implements <see cref="IUndoableEdit"/>,
  /// it is very easy for the undo manager to remember the changes in a manner that makes
  /// them easy to undo or redo.
  /// But note that the <see cref="UndoManager"/> property is initially null;
  /// you will need to set this property before any actions can be recorded
  /// and then undone.
  /// When you want to make transient changes that you do not want to be recorded
  /// by the undo manager, you can temporarily set <see cref="SkipsUndoManager"/>
  /// to true.
  /// </para>
  /// <para>
  /// As you add state to your data or to your model, you may need to add code
  /// that implements state changing as needed for undo and redo.  This can be done
  /// either by overriding a model method or a data method if the data implements
  /// <see cref="IChangeDataValue"/>.
  /// </para>
  /// <para>
  /// A single user gesture or command may result in many model/data changes.
  /// These all want to be treated as a single undo-able action.
  /// You can group such changes together by calling
  /// <see cref="StartTransaction"/> and <see cref="CommitTransaction"/>.
  /// If after a <see cref="StartTransaction"/> you don't want to commit the
  /// changes, you can call <see cref="RollbackTransaction"/>, which not only
  /// ends the transaction but also automatically "undoes" all of the changes
  /// since the call to <see cref="StartTransaction"/>.
  /// </para>
  /// </remarks>
  /// <seealso cref="ITreeModel"/>
  /// <seealso cref="IConnectedModel"/>
  /// <seealso cref="ILinksModel"/>
  /// <seealso cref="IGroupsModel"/>
  /// <seealso cref="DiagramModel"/>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  /// <seealso cref="GraphModel{NodeType, NodeKey}"/>
  /// <seealso cref="TreeModel{NodeType, NodeKey}"/>
  public interface IDiagramModel {
    /// <summary>
    /// Gets or sets the collection of node data items for the model.
    /// </summary>
    /// <value>
    /// Initially this value is null.  It must be set to a non-null, non-empty value for the model to have any "data".
    /// </value>
    System.Collections.IEnumerable NodesSource { get; set; }

    // Updating the model when the data changes

    // When data changes occur that are not necessarily detectable by the model,
    // you must call the appropriate method to ensure the model is up-to-date.
    // However, in a load/store scenario, rather than in a frequently updated scenario,
    // such changes to the data will not happen independently of the diagram and application.

    /// <summary>
    /// This should be called when a node data object is added to the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <c>INotifyCollectionChanged</c>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been augmented.
    /// </para>
    /// </remarks>
    void DoNodeAdded(Object nodedata);

    /// <summary>
    /// This should be called when a node data object is removed from the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <c>INotifyCollectionChanged</c>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been diminished.
    /// </para>
    /// </remarks>
    void DoNodeRemoved(Object nodedata);

    /// <summary>
    /// This should be called when a node data's key value may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// <para>
    /// If a node data object implements <c>INotifyPropertyChanged</c>
    /// and if the key is a simple property on the data that the model recognizes,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of node's key has changed.
    /// </para>
    /// </remarks>
    void DoNodeKeyChanged(Object nodedata);

    /// <summary>
    /// Forget all unresolved delayed or forward references.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The model may learn about node data in any order, so references to
    /// nodes may be unresolvable until later, perhaps never.
    /// Call this method to clear the internal table that keeps track
    /// of node keys that are not defined by the presence of corresponding node data.
    /// </para>
    /// </remarks>
    void ClearUnresolvedReferences();

    // Navigating the model

    /// <summary>
    /// Return the Type of the node data.
    /// </summary>
    /// <returns>a <see cref="Type"/>, not a node data object, nor a string</returns>
    /// <remarks>
    /// This is useful for data transfer.
    /// </remarks>
    Type GetNodeType();

    /// <summary>
    /// This predicate is true when the argument is an instance of the node data Type.
    /// </summary>
    /// <param name="nodedata">the arbitrary object to be checked for compatibility to be a node data</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> can be cast to the the node data Type;
    /// false otherwise
    /// </returns>
    bool IsNodeType(Object nodedata);

    /// <summary>
    /// This predicate is true if the argument is a node data in this model.
    /// </summary>
    /// <param name="nodedata">the object to be checked to see if it is a known node data in this model</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> is in the <see cref="NodesSource"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    bool IsNodeData(Object nodedata);

    /// <summary>
    /// Given a key, find the node data with that key.
    /// </summary>
    /// <param name="key">
    /// a value of null for this argument will result in the default value for the node data Type
    /// </param>
    /// <returns>
    /// a node data with that key, if it is present in the model;
    /// the value will be the default for the type if no such node data is known to be in this model
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    Object FindNodeByKey(Object key);

    /// <summary>
    /// To help distinguish between different kinds of nodes, each node has a "category"
    /// that is just a string.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the default category is the empty string</returns>
    String GetCategoryForNode(Object nodedata);

    /// <summary>
    /// This predicate is true if there is a link from one node data/port to another one.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns>true if there are any links</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    bool IsLinked(Object fromdata, Object fromparam, Object todata, Object toparam);

    /// <summary>
    /// Return a sequence of node data that are directly connected by links going into a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{Object}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetFromNodesForNode(Object nodedata);

    /// <summary>
    /// Return a sequence of node data that are directly connected by links coming out from a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{Object}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetToNodesForNode(Object nodedata);
    
    /// <summary>
    /// Return a sequence of node data that are directly connected to a given node, in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{Object}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetConnectedNodesForNode(Object nodedata);


    // Additional model services

    /// <summary>
    /// Create a copy of this model initialized with different data.
    /// </summary>
    /// <param name="init">this may be null, meaning no initial data</param>
    /// <returns>a model of the same type as <c>this</c></returns>
    /// <remarks>
    /// Most of the properties of the returned model should have the same value
    /// as this model, but the data depends on the argument <see cref="IDataCollection"/>.
    /// </remarks>
    IDiagramModel CreateInitializedCopy(IDataCollection init);

    /// <summary>
    /// Create an empty <see cref="IDataCollection"/> for this model.
    /// </summary>
    /// <returns></returns>
    IDataCollection CreateDataCollection();

    /// <summary>
    /// Create an <see cref="ICopyDictionary"/> initialized for this model.
    /// </summary>
    /// <returns>Normally this will be an empty dictionary.</returns>
    ICopyDictionary CreateCopyDictionary();

    /// <summary>
    /// A name for this model.
    /// </summary>
    /// <value>
    /// By default this is an empty string.
    /// </value>
    /// <remarks>
    /// This is mostly used to help distinguish between different models of the same type.
    /// </remarks>
    String Name { get; set; }

    /// <summary>
    /// Gets or sets the format of this model's data.
    /// </summary>
    /// <value>
    /// By default this is the fully qualified name of this model type.
    /// </value>
    /// <remarks>
    /// This string is used by clipboard and drag-and-drop operations to distinguish
    /// between different and presumably incompatible data sources.
    /// You may wish to provide different values in order to prevent data
    /// from being transferred to other applications that are using the same model class.
    /// </remarks>
    String DataFormat { get; set; }

    /// <summary>
    /// Gets or sets whether various model-changing methods are enabled.
    /// </summary>
    /// <value>
    /// By default this value is false.
    /// </value>
    /// <remarks>
    /// <para>
    /// When false, this property disables methods named "Add...", "Modify...", or "Remove...".
    /// </para>
    /// <para>
    /// But note that this property does not and cannot affect the "modifiability"
    /// or "read-only"-ness of model data, since the data classes may have no knowledge
    /// about this model class and this property.
    /// </para>
    /// </remarks>
    bool Modifiable { get; set; }

    /// <summary>
    /// Copy existing data and add to this model.
    /// </summary>
    /// <param name="coll">the collection of data to be copied</param>
    /// <param name="env">
    /// the <see cref="ICopyDictionary"/> used to keep track of copied objects;
    /// if null, the method will call <see cref="CreateCopyDictionary"/>, use it, and return it
    /// </param>
    /// <returns>the updated copy dictionary</returns>
    /// <remarks>
    /// <para>
    /// The primary purpose of this method is to perform a two-pass copy of a part of a diagram,
    /// and add the resulting data to this model.
    /// </para>
    /// <para>
    /// Of course you can add node data without copying them by calling <see cref="AddNode"/> or
    /// by just adding them directly to the <see cref="NodesSource"/>.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the collections are of type <see cref="IDataCollection"/>
    /// and <see cref="ICopyDictionary"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific collection type.
    /// </para>
    /// </remarks>
    ICopyDictionary AddCollectionCopy(IDataCollection coll, ICopyDictionary env);

    /// <summary>
    /// Add a copy of a node data to this model.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the copied node data</returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that just calls <see cref="AddCollectionCopy"/>.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object AddNodeCopy(Object nodedata);

    /// <summary>
    /// Add a node data to <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    void AddNode(Object nodedata);

    /// <summary>
    /// Add a link between one node/port and another node/port.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    Object AddLink(Object fromdata, Object fromparam, Object todata, Object toparam);

    /// <summary>
    /// Remove node data from <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This method can have potential side-effects, such as removing links that are connected to the <paramref name="nodedata"/>.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    void RemoveNode(Object nodedata);

    /// <summary>
    /// Remove all links connecting the two nodes/ports in the one direction.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <remarks>
    /// <para>
    /// This method can have potential side-effects, such as removing nodes that are labels on the link.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on a specific node data type.
    /// </para>
    /// </remarks>
    void RemoveLink(Object fromdata, Object fromparam, Object todata, Object toparam);

    /// <summary>
    /// This predicate is true if adding a link between two nodes/ports would result in a valid graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns></returns>
    bool IsLinkValid(Object fromdata, Object fromparam, Object todata, Object toparam);

    /// <summary>
    /// The Changed event is raised whenever the model is modified.
    /// </summary>
    /// <seealso cref="ModelChangedEventArgs"/>
    /// <seealso cref="ModelChange"/>
    event EventHandler<ModelChangedEventArgs> Changed;

    /// <summary>
    /// Raise a <see cref="Changed"/> event, given a <see cref="ModelChangedEventArgs"/>.
    /// </summary>
    /// <param name="e">a <see cref="ModelChangedEventArgs"/> that describes what changed and how</param>
    void RaiseChanged(ModelChangedEventArgs e);

    /// <summary>
    /// Gets or sets whether this model is considered changed from an earlier state.
    /// </summary>
    /// <value>
    /// true if this model has been marked as having been modified,
    /// if the <see cref="UndoManager"/> has recorded any changes, or
    /// if an undo has been performed without a corresponding redo.
    /// </value>
    bool IsModified { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="UndoManager"/> for this model.
    /// </summary>
    /// <value>
    /// When this value is null, there is no <see cref="UndoManager"/>,
    /// and thus no support for undo/redo.
    /// </value>
    UndoManager UndoManager { get; set; }

    /// <summary>
    /// Gets or sets a flag that controls whether the model notifies
    /// any <see cref="UndoManager"/> that a change has occurred.
    /// </summary>
    /// <value>
    /// This is normally false.
    /// You may want to temporarily set this to true in order to avoid
    /// recording temporary changes to the model.
    /// </value>
    bool SkipsUndoManager { get; set; }

    /// <summary>
    /// This is called during an Undo or a Redo to actually make state
    /// changes to this model or to this model's data.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    void ChangeModel(ModelChangedEventArgs e, bool undo);

    /// <summary>
    /// This property is true during a call to <see cref="ChangeModel"/>,
    /// indicating a change happening due to an undo or a redo.
    /// </summary>
    bool IsChangingModel { get; }

    /// <summary>
    /// Call the UndoManager's StartTransaction method.
    /// </summary>
    /// <param name="tname">a String describing the transaction</param>
    /// <returns>the value of the call to StartTransaction</returns>
    bool StartTransaction(String tname);

    /// <summary>
    /// Call the UndoManager's CommitTransaction method.
    /// </summary>
    /// <param name="tname">a String describing the transaction</param>
    bool CommitTransaction(String tname);

    /// <summary>
    /// Call the UndoManager's RollbackTransaction method.
    /// </summary>
    bool RollbackTransaction();

    /// <summary>
    /// True if there is an UndoManager and a transaction has been started.
    /// </summary>
    bool IsInTransaction { get; }
  }


  // Model variations

  /// <summary>
  /// This kind of diagram model only supports tree-structured relationships.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each node data may have a reference to a "parent" node.
  /// Each node data may have a collection of references to "children" nodes.
  /// All references are by key values.
  /// </para>
  /// <para>
  /// This kind of model assumes the links are implicit from the references in the lists.
  /// If you want to use separate data structures to explicitly represent link information,
  /// use the <see cref="ILinksModel"/>.
  /// </para>
  /// <para>
  /// This model does not support links that would form a cycle, including reflexive links.
  /// Nor does it support more than one link between any pair of nodes.
  /// If you need to model multiple links between nodes, use the <see cref="ILinksModel"/>.
  /// </para>
  /// <para>
  /// There are three categories of methods: updating, navigation, and modification.
  /// </para>
  /// <para>
  /// The updating methods need to be called when there has been a change to the data,
  /// so that the model can be kept up-to-date.
  /// These methods include changes to the "parent" node key:
  /// <see cref="DoParentNodeChanged"/>.
  /// These methods also include changes to the collection of "children" node keys:
  /// <see cref="DoChildNodeKeyAdded"/>, <see cref="DoChildNodeKeyRemoved"/>,
  /// <see cref="DoChildNodeKeysChanged"/>.
  /// </para>
  /// <para>
  /// The navigation methods support examining and traversing the graph.
  /// Some of these methods are actually defined in the base interface, <see cref="IDiagramModel"/>.
  /// <see cref="IDiagramModel.FindNodeByKey"/>, <see cref="IDiagramModel.IsLinked"/>,
  /// <see cref="IDiagramModel.GetConnectedNodesForNode"/>, and
  /// <see cref="IDiagramModel.IsLinkValid"/>.
  /// But this interface adds a few more methods:
  /// <see cref="GetParentForNode"/> and <see cref="GetChildrenForNode"/>.
  /// </para>
  /// <para>
  /// The modification methods are used to alter the graph.
  /// A number of these methods are defined in <see cref="IDiagramModel"/>, such as 
  /// <see cref="IDiagramModel.AddNode"/>, <see cref="IDiagramModel.RemoveNode"/>, 
  /// <see cref="IDiagramModel.AddLink"/>, <see cref="IDiagramModel.RemoveLink"/>, and
  /// <see cref="IDiagramModel.AddCollectionCopy"/>.
  /// But this interface adds several methods.
  /// For the "parent" node keys:
  /// <see cref="SetParentNodeKey"/>.
  /// For the collection of "children" node keys:
  /// <see cref="AddChildNodeKey"/>, <see cref="RemoveChildNodeKey"/>, <see cref="SetChildNodeKeys"/>.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="TreeModel{NodeType, NodeKey}"/>
  public interface ITreeModel : IDiagramModel {
    // Updating the model when the data changes

    /// <summary>
    /// This should be called when a node data's tree parent may have changed.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoParentNodeChanged(Object nodedata);

    /// <summary>
    /// This should be called when a child node data has been added to the collection of child nodes.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="childkey">the key for the added child node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoChildNodeKeyAdded(Object nodedata, Object childkey);

    /// <summary>
    /// This should be called when a child node data has been removed from the collection of child nodes.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="childkey">the key for the removed child node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoChildNodeKeyRemoved(Object nodedata, Object childkey);

    /// <summary>
    /// This should be called when a node data's list of children nodes has been replaced.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// </remarks>
    void DoChildNodeKeysChanged(Object nodedata);

    // Navigating the model
    
    /// <summary>
    /// Return a parent node data for a given node data, if there is one.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a node data, or null if there is no "parent"</returns>
    Object GetParentForNode(Object nodedata);  // simplified version of GetFromNodesForNode

    /// <summary>
    /// Return a sequence of node data that are immediate children of a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{Object}"/> of child node data; an empty sequence if there are none</returns>
    IEnumerable<Object> GetChildrenForNode(Object nodedata);  // same as GetToNodesForNode

    // Modification
    
    /// <summary>
    /// Change a node data so that it refers to a different parent node data, by node key.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="parentkey">the key value of the "parent" node data</param>
    void SetParentNodeKey(Object nodedata, Object parentkey);
    
    /// <summary>
    /// Add a "child" node data's key value to a node data's list of "children".
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkey">the key value of the "child" node data</param>
    void AddChildNodeKey(Object nodedata, Object childkey);

    /// <summary>
    /// Remove a child node data's key value from a node data's list of "children" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkey">the key value of the "child" node data</param>
    void RemoveChildNodeKey(Object nodedata, Object childkey);

    /// <summary>
    /// Replace a node data's list of "children" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkeys">a sequence of "child" node data key values</param>
    void SetChildNodeKeys(Object nodedata, System.Collections.IEnumerable childkeys);

    /// <summary>
    /// This predicate is true if changing an existing link between two nodes/ports would result in a valid graph
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the new link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the new link would go</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns></returns>
    bool IsRelinkValid(Object newfromdata, Object newtodata, Object oldfromdata, Object oldtodata);
  }


  /// <summary>
  /// A model that supports directed link relationships between nodes,
  /// with the relationship information stored on each node as collections of connected nodes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each node data has two collections of references to other nodes:
  /// one that lists the nodes from which there are links connecting to this node,
  /// and one that lists the nodes to which there are links connecting from this node.
  /// All node references are by node data key value.
  /// </para>
  /// <para>
  /// This kind of model assumes the links are implicit from the references in the lists.
  /// If you want to use separate data structures to explicitly represent link information,
  /// use the <see cref="ILinksModel"/>.
  /// </para>
  /// <para>
  /// Although this model supports links between any pair of nodes in either or both
  /// directions, and supports reflexive links from a node to itself, it does not support
  /// more than one link between any pair of nodes in the same direction.
  /// If you need to model multiple links between nodes, use the <see cref="ILinksModel"/>.
  /// </para>
  /// <para>
  /// There are three categories of methods: updating, navigation, and modification.
  /// </para>
  /// <para>
  /// The updating methods need to be called when there has been a change to the data,
  /// so that the model can be kept up-to-date.
  /// These methods include changes to the collection of "from" node keys:
  /// <see cref="DoFromNodeKeyAdded"/>, <see cref="DoFromNodeKeyRemoved"/>,
  /// <see cref="DoFromNodeKeysChanged"/>.
  /// These methods also include changes to the collection of "to" node keys:
  /// <see cref="DoToNodeKeyAdded"/>, <see cref="DoToNodeKeyRemoved"/>,
  /// <see cref="DoToNodeKeysChanged"/>.
  /// </para>
  /// <para>
  /// The navigation methods support examining and traversing the graph.
  /// These methods are actually defined in the base interface, <see cref="IDiagramModel"/>.
  /// <see cref="IDiagramModel.FindNodeByKey"/>, <see cref="IDiagramModel.IsLinked"/>,
  /// <see cref="IDiagramModel.GetFromNodesForNode"/>, <see cref="IDiagramModel.GetToNodesForNode"/>,
  /// <see cref="IDiagramModel.GetConnectedNodesForNode"/>, and
  /// <see cref="IDiagramModel.IsLinkValid"/>.
  /// </para>
  /// <para>
  /// The modification methods are used to alter the graph.
  /// A number of these methods are defined in <see cref="IDiagramModel"/>, such as 
  /// <see cref="IDiagramModel.AddNode"/>, <see cref="IDiagramModel.RemoveNode"/>, 
  /// <see cref="IDiagramModel.AddLink"/>, <see cref="IDiagramModel.RemoveLink"/>, and
  /// <see cref="IDiagramModel.AddCollectionCopy"/>.
  /// But this interface adds several methods.
  /// For the collection of "from" node keys:
  /// <see cref="AddFromNodeKey"/>, <see cref="RemoveFromNodeKey"/>, <see cref="SetFromNodeKeys"/>.
  /// For the collection of "to" node keys:
  /// <see cref="AddToNodeKey"/>, <see cref="RemoveToNodeKey"/>, <see cref="SetToNodeKeys"/>.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="GraphModel{NodeType, NodeKey}"/>
  public interface IConnectedModel : IDiagramModel {
    // Updating the model when the data changes

    /// <summary>
    /// This should be called when a "from" node data key value has been added to the collection of "from" nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromkey"></param>
    void DoFromNodeKeyAdded(Object nodedata, Object fromkey);

    /// <summary>
    /// This should be called when a "from" node data key value has been removed from the collection of "from" nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromkey"></param>
    void DoFromNodeKeyRemoved(Object nodedata, Object fromkey);

    /// <summary>
    /// This should be called when the collection of "from" node keys has been replaced on a node data.
    /// </summary>
    /// <param name="nodedata"></param>
    void DoFromNodeKeysChanged(Object nodedata);

    /// <summary>
    /// This should be called when a "to" node data key value has been added to the collection of "to" nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="tokey"></param>
    void DoToNodeKeyAdded(Object nodedata, Object tokey);

    /// <summary>
    /// This should be called when a "to" node data key value has been removed from the collection of "to" nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="tokey"></param>
    void DoToNodeKeyRemoved(Object nodedata, Object tokey);

    /// <summary>
    /// This should be called when the collection of "to" node keys has been replaced on a node data.
    /// </summary>
    /// <param name="nodedata"></param>
    void DoToNodeKeysChanged(Object nodedata);

    // Navigation methods are actually defined in IDiagramModel

    // Modification

    /// <summary>
    /// Modify the list of "from" node keys by adding another node data key value.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromkey"></param>
    void AddFromNodeKey(Object nodedata, Object fromkey);

    /// <summary>
    /// Modify the list of "from" node keys by removing a node data key value.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromkey"></param>
    void RemoveFromNodeKey(Object nodedata, Object fromkey);

    /// <summary>
    /// Replace the list of "from" node keys with another collection of key values.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromkeys"></param>
    void SetFromNodeKeys(Object nodedata, System.Collections.IEnumerable fromkeys);
    
    /// <summary>
    /// Modify the list of "to" node keys by adding another node data key value.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="tokey"></param>
    void AddToNodeKey(Object nodedata, Object tokey);

    /// <summary>
    /// Modify the list of "to" node keys by removing a node data key value.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="tokey"></param>
    void RemoveToNodeKey(Object nodedata, Object tokey);

    /// <summary>
    /// Replace the list of "to" node keys with another collection of key values.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="tokeys"></param>
    void SetToNodeKeys(Object nodedata, System.Collections.IEnumerable tokeys);

    /// <summary>
    /// This predicate is true if changing an existing link between two nodes/ports would result in a valid graph
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the new link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the new link would go</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns></returns>
    bool IsRelinkValid(Object newfromdata, Object newtodata, Object oldfromdata, Object oldtodata);

    /// <summary>
    /// This property controls the overall graph structure that may be drawn.
    /// </summary>
    ValidCycle ValidCycle { get; set; }
  }


  /// <summary>
  /// A model that supports directed link relationships between nodes,
  /// with the relationship information stored in separate link data structures.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Link relationship information is found in link data held in the
  /// <see cref="LinksSource"/> collection property.
  /// If you want a simpler model where the link relationship information is
  /// stored in a list on each node data, use the <see cref="IConnectedModel"/>.
  /// </para>
  /// <para>
  /// This model can support any number of links between any pair of nodes.
  /// Furthermore, this model supports additional information at each end of
  /// the link, to distinguish between different "ports" on each node.
  /// </para>
  /// <para>
  /// Links are nominally directional, each "coming from a node" and "going to a node".
  /// However, if you want to model undirected relationships, you can use the methods that
  /// ignore the direction of the links.
  /// </para>
  /// <para>
  /// There are three categories of methods: updating, navigation, and modification.
  /// </para>
  /// <para>
  /// The updating methods need to be called when there has been a change to the data,
  /// so that the model can be kept up-to-date.
  /// These methods include changes to the collection of link data:
  /// <see cref="DoLinkAdded"/> and <see cref="DoLinkRemoved"/>.
  /// They also include methods involving changes to the state of the link data:
  /// <see cref="DoLinkPortsChanged"/> and <see cref="DoLinkLabelChanged"/>.
  /// </para>
  /// <para>
  /// The navigation methods support examining and traversing the graph.
  /// Some methods working on node data are actually defined in the base interface, <see cref="IDiagramModel"/>.
  /// <see cref="IDiagramModel.FindNodeByKey"/>, <see cref="IDiagramModel.IsLinked"/>,
  /// <see cref="IDiagramModel.GetFromNodesForNode"/>, <see cref="IDiagramModel.GetToNodesForNode"/>,
  /// <see cref="IDiagramModel.GetConnectedNodesForNode"/>, and
  /// <see cref="IDiagramModel.IsLinkValid"/>.
  /// But this interface adds a number of methods that also work on node data:
  /// <see cref="IsLinkData"/>, <see cref="GetLinksForNode(Object)"/>,
  /// <see cref="GetLinksForNode(Object, Predicate{Object})"/>,
  /// <see cref="GetFromLinksForNode"/>, <see cref="GetToLinksForNode"/>,
  /// <see cref="GetLinksBetweenNodes"/>.
  /// And there are methods that work on link data:
  /// <see cref="GetFromNodeForLink"/>, <see cref="GetFromParameterForLink"/>,
  /// <see cref="GetToNodeForLink"/>, <see cref="GetToParameterForLink"/>.
  /// And there are methods that involve a link data that has a node data as a "label":
  /// <see cref="GetHasLabelNodeForLink"/>, <see cref="GetHasLabeledLinkForNode"/>,
  /// <see cref="GetLabelNodeForLink"/>, and <see cref="GetLabeledLinkForNode"/>.
  /// </para>
  /// <para>
  /// The modification methods are used to alter the graph.
  /// A number of these methods working on node data are defined in <see cref="IDiagramModel"/>, such as 
  /// <see cref="IDiagramModel.AddNode"/>, <see cref="IDiagramModel.RemoveNode"/>, 
  /// <see cref="IDiagramModel.AddLink"/>, <see cref="IDiagramModel.RemoveLink"/>, and
  /// <see cref="IDiagramModel.AddCollectionCopy"/>.
  /// But this interface adds several methods working on link data:
  /// <see cref="AddLink"/>, <see cref="RemoveLink"/>, <see cref="AddLinkCopy"/>,
  /// <see cref="SetLinkFromPort"/>, <see cref="SetLinkToPort"/>, and
  /// <see cref="SetLinkLabel"/>.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  public interface ILinksModel : IDiagramModel {
    /// <summary>
    /// The collection of link data objects
    /// </summary>
    System.Collections.IEnumerable LinksSource { get; set; }

    // Updating the model when the data changes

    /// <summary>
    /// This method should be called when a link data has been added to the <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoLinkAdded(Object linkdata);

    /// <summary>
    /// This method should be called when a link data has been removed from the <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoLinkRemoved(Object linkdata);

    /// <summary>
    /// This method must be called when either the "from" or the "to" node (or port) has changed.
    /// </summary>
    /// <param name="linkdata">a link data</param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoLinkPortsChanged(Object linkdata);

    /// <summary>
    /// This method must be called when any "label" node is added, removed, or replaced.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void DoLinkLabelChanged(Object linkdata);

    
    // Navigating the model

    /// <summary>
    /// This predicate is true if the object is non-null and is of a type that the model accepts for link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    bool IsLinkType(Object linkdata);

    /// <summary>
    /// This predicate is true if the linkdata is in the <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool IsLinkData(Object linkdata);

    /// <summary>
    /// Find all links connected to a node in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a sequence of link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetLinksForNode(Object nodedata);

    /// <summary>
    /// Find all links connected to a node that satisfy a given predicate.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="predicate">the predicate takes a single argument, the link data</param>
    /// <returns>a sequence of link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetLinksForNode(Object nodedata, Predicate<Object> predicate);

    // additional convenience methods

    /// <summary>
    /// Find all links coming into a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a sequence of link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetFromLinksForNode(Object nodedata);

    /// <summary>
    /// Find all links going out of a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a sequence of link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetToLinksForNode(Object nodedata);

    /// <summary>
    /// Find all links connecting two nodes/ports in one direction.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns>a sequence of link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> GetLinksBetweenNodes(Object fromdata, Object fromparam, Object todata, Object toparam);

    /// <summary>
    /// Returns the node from which a link comes.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a node data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetFromNodeForLink(Object linkdata);

    /// <summary>
    /// Returns any "from" port parameter information.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>the port parameter data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetFromParameterForLink(Object linkdata);

    /// <summary>
    /// Returns the node to which a link goes.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a node data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetToNodeForLink(Object linkdata);

    /// <summary>
    /// Returns any "to" port parameter information.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>the port parameter data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetToParameterForLink(Object linkdata);

    /// <summary>
    /// This predicate is true if the given node data acts as a label for a link.
    /// </summary>
    /// <param name="nodedata">a node data</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool GetIsLinkLabelForNode(Object nodedata);

    /// <summary>
    /// This predicate is true if the given link data has a node label.
    /// </summary>
    /// <param name="linkdata">a link data</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool GetHasLabelNodeForLink(Object linkdata);

    /// <summary>
    /// Find the node data that is the label for a link, if there is one.
    /// </summary>
    /// <param name="linkdata">a link data</param>
    /// <returns>a node data, or null if there is none</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetLabelNodeForLink(Object linkdata);

    /// <summary>
    /// To help distinguish between different kinds of links, each link has a "category"
    /// that is just a string.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>the default category is the empty string</returns>
    String GetCategoryForLink(Object linkdata);

    /// <summary>
    /// This predicate is true if the given node data is associated with a link data as a label.
    /// </summary>
    /// <param name="nodedata">a node data that is a "label"</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool GetHasLabeledLinkForNode(Object nodedata);

    /// <summary>
    /// Find the link data with which a label node is associated, if any.
    /// </summary>
    /// <param name="nodedata">a node data that is a "label"</param>
    /// <returns>a link data, or null if there is none</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object GetLabeledLinkForNode(Object nodedata);


    // Additional model services

    /// <summary>
    /// Add a copy of a link data to this model.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>the newly copied link data</returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object AddLinkCopy(Object linkdata);

    /// <summary>
    /// Add a link data to <see cref="LinksSource"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void AddLink(Object linkdata);

    /// <summary>
    /// Remove a link data from <see cref="LinksSource"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void RemoveLink(Object linkdata);

    /// <summary>
    /// Set the "from" node and port information for a link data.
    /// </summary>
    /// <param name="linkdata">the link data being modified</param>
    /// <param name="nodedata">a node data</param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void SetLinkFromPort(Object linkdata, Object nodedata, Object portparam);

    /// <summary>
    /// Set the "to" node and port information for a link data.
    /// </summary>
    /// <param name="linkdata">the link data being modified</param>
    /// <param name="nodedata">a node data</param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void SetLinkToPort(Object linkdata, Object nodedata, Object portparam);

    /// <summary>
    /// Change the "label" node data for a link data.
    /// </summary>
    /// <param name="linkdata">the link data being modified</param>
    /// <param name="labelnodedata">the new node data that is a "label"; may be null to remove the label</param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void SetLinkLabel(Object linkdata, Object labelnodedata);

    /// <summary>
    /// This predicate is true if changing an existing link between two nodes/ports would result in a valid graph
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the new link would come</param>
    /// <param name="newfromparam">an optional value identifying which port on the "from" node the link would be connected to</param>
    /// <param name="newtodata">a node key identify the node data to which the new link would go</param>
    /// <param name="newtoparam">an optional value identifying which port on the "to" node the link would be connected to</param>
    /// <param name="oldlinkdata">an existing link that would be deleted</param>
    /// <returns></returns>
    bool IsRelinkValid(Object newfromdata, Object newfromparam, Object newtodata, Object newtoparam, Object oldlinkdata);

    /// <summary>
    /// This property controls the overall graph structure that may be drawn.
    /// </summary>
    ValidCycle ValidCycle { get; set; }

    /// <summary>
    /// This property controls whether link data must always connect to node data at both ends of the link.
    /// </summary>
    ValidUnconnectedLinks ValidUnconnectedLinks { get; set; }
  }


  /// <summary>
  /// A model that supports a grouping relationship between nodes,
  /// with the relationship information stored on each node as collections of related nodes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each node data can be a group (i.e. a node "container") or not (i.e. an "atomic" node).
  /// It is assumed that a node data cannot dynamically switch between being a group or not.
  /// Each node data can have a list of references to "member" nodes.
  /// All node references are by node data key value.
  /// </para>
  /// <para>
  /// This kind of model assumes the grouping is implicit from these references.
  /// At this time we do not support a model where the grouping information is
  /// explicitly provided by data structures different from the node data.
  /// </para>
  /// <para>
  /// There are three categories of methods: updating, navigation, and modification.
  /// </para>
  /// <para>
  /// The updating methods need to be called when there has been a change to the data,
  /// so that the model can be kept up-to-date.
  /// These methods include changes to the collection of "member" node keys:
  /// <see cref="DoMemberNodeKeyAdded"/>, <see cref="DoMemberNodeKeyRemoved"/>,
  /// <see cref="DoMemberNodeKeysChanged"/>.
  /// </para>
  /// <para>
  /// The navigation methods support examining and traversing the graph.
  /// One such method is actually defined in the base interface,
  /// <see cref="IDiagramModel.FindNodeByKey"/>.
  /// This interface adds the following methods:
  /// <see cref="GetIsGroupForNode"/>, <see cref="GetMemberNodesForGroup"/>,
  /// <see cref="IsMember"/>, and <see cref="IsMemberValid"/>.
  /// </para>
  /// <para>
  /// The modification methods are used to alter the graph.
  /// A number of these methods are defined in <see cref="IDiagramModel"/>, such as 
  /// <see cref="IDiagramModel.AddNode"/> and <see cref="IDiagramModel.RemoveNode"/>.
  /// But this interface adds several methods:
  /// <see cref="AddMemberNodeKey"/>, <see cref="RemoveMemberNodeKey"/>, <see cref="SetMemberNodeKeys"/>.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="ISubGraphModel"/>
  public interface IGroupsModel : IDiagramModel {
    // Updating the model when the data changes

    // Assume data cannot dynamically change from representing a group to a regular node, or vice-versa
    //void DoNodeIsGroupChanged(Object nodedata);

    // The list of a node's member node keys has been changed
    /// <summary>
    /// This method should be called when a "member" node data key has been added to the list of members.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="memberkey"></param>
    void DoMemberNodeKeyAdded(Object nodedata, Object memberkey);

    /// <summary>
    /// This method should be called when a "member" node data key has been removed from the list of members.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="memberkey"></param>
    void DoMemberNodeKeyRemoved(Object nodedata, Object memberkey);

    /// <summary>
    /// This method should be called when the list of member key values has been replaced.
    /// </summary>
    /// <param name="nodedata"></param>
    void DoMemberNodeKeysChanged(Object nodedata);


    // Navigation

    /// <summary>
    /// This predicate is true when the given node data represents a group of nodes
    /// instead of an "atomic" node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    bool GetIsGroupForNode(Object nodedata);

    /// <summary>
    /// Return a collection of member node datas.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    IEnumerable<Object> GetMemberNodesForGroup(Object nodedata);

    /// <summary>
    /// This predicate is true if the <paramref name="membernodedata"/> is an
    /// immediate member of the <paramref name="groupnodedata"/>.
    /// </summary>
    /// <param name="groupnodedata"></param>
    /// <param name="membernodedata"></param>
    /// <returns></returns>
    bool IsMember(Object groupnodedata, Object membernodedata);

    /// <summary>
    /// This predicate is true when it is valid to add the <paramref name="membernodedata"/>
    /// to the <paramref name="groupnodedata"/>.
    /// </summary>
    /// <param name="groupnodedata"></param>
    /// <param name="membernodedata"></param>
    /// <returns>
    /// false if the <paramref name="membernodedata"/> cannot be added to <paramref name="groupnodedata"/>
    /// </returns>
    /// <remarks>
    /// This assumes that if the member node is already part of a group, it will be removed from that group first.
    /// </remarks>
    bool IsMemberValid(Object groupnodedata, Object membernodedata);


    // Modification

    /// <summary>
    /// Add a node data key to the list of "member" node data keys.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="membernodekey"></param>
    void AddMemberNodeKey(Object nodedata, Object membernodekey);

    /// <summary>
    /// Remove a node data key from the list of "member" node data keys.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="membernodekey"></param>
    void RemoveMemberNodeKey(Object nodedata, Object membernodekey);

    /// <summary>
    /// Replace the list of "member" node data keys with a new collection of keys.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="membernodekeys"></param>
    void SetMemberNodeKeys(Object nodedata, System.Collections.IEnumerable membernodekeys);
  }


  // Subgraphs (groups that don't share nodes)

  /// <summary>
  /// The subgraph model is a kind of grouping model that limits each node
  /// (including subgraphs) to be a member of at most one group.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The restriction that each node can have at most one container node
  /// means that there is basically a property for each node data that is a
  /// reference to its container node, if any.
  /// </para>
  /// <para>
  /// When that property is changed, <see cref="DoGroupNodeChanged"/> must be called.
  /// You can get the container group by calling <see cref="GetGroupForNode"/>.
  /// You can set the container group by calling <see cref="SetGroupNode"/>.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="GraphModel{NodeType, NodeKey}"/>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  public interface ISubGraphModel : IGroupsModel {
    // Updating the model when the data changes

    /// <summary>
    /// This method should be called when the containing group node data for a node data has been changed.
    /// </summary>
    /// <param name="nodedata"></param>
    void DoGroupNodeChanged(Object nodedata);

    // Navigation

    /// <summary>
    /// Get the "container" node data for a given node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a node data</returns>
    Object GetGroupForNode(Object nodedata);

    // Modification

    /// <summary>
    /// Change the container group node data of a node data.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="groupnodedata">the new "container" group node data, or null to make the node uncontained</param>
    void SetGroupNode(Object nodedata, Object groupnodedata);
  }

  /// <summary>
  /// This is a <see cref="ISubGraphModel"/> that also supports link data (<see cref="ILinksModel"/>).
  /// </summary>
  /// <remarks>
  /// <para>
  /// This model is basically a <see cref="ISubGraphModel"/> that automatically assigns
  /// links to belong to groups.  There are two methods, one to get the containing group
  /// for a link (if any), <see cref="GetGroupForLink"/>, and one to get all of the
  /// link data that are members of a group, <see cref="GetMemberLinksForGroup"/>.
  /// </para>
  /// <para>
  /// Because membership of links in groups is automatically computed,
  /// there are no modification or updating methods in this interface.
  /// </para>
  /// <para>
  /// Note that this interface is universal, because it can only assume the node data is of type <see cref="Object"/>.
  /// The corresponding methods in the generic model classes operate on and return a specific node data type.
  /// </para>
  /// </remarks>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  public interface ISubGraphLinksModel : ISubGraphModel, ILinksModel {
    // link membership in a group is automatically computed, so this method is unnecessary
    //void DoLinkGroupChanged(Object linkdata);

    // Navigation
    /// <summary>
    /// Returns the container node data for a link data, if any.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a node data, or null if there is no container for a link</returns>
    Object GetGroupForLink(Object linkdata);

    /// <summary>
    /// Returns a collection of link data that are assumed to belong to the given group data.
    /// </summary>
    /// <param name="nodedata">a node data that is a group (a container)</param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// If there are no links in a subgraph, this will be an empty sequence.
    /// </remarks>
    IEnumerable<Object> GetMemberLinksForGroup(Object nodedata);

    // Modification, also not needed:
    //void ModifyLinkGroup(Object linkdata, Object groupnodedata);
  }


#if GROUPSETS
  // Sets (groups that may share nodes)

  public interface ISetsModel : IGroupsModel {
    // Updating the model when the data changes
    void DoGroupNodesChanged(Object nodedata);

    // Navigation
    IEnumerable<Object> GetGroupsForNode(Object nodedata);

    // Modification
    void AddGroupNodeKey(Object nodedata, Object groupkey);
    void RemoveGroupNodeKey(Object nodedata, Object groupkey);
    void ModifyGroupNodeKeys(Object nodedata, System.Collections.IEnumerable groupkeys);
}

  public interface ISetsLinksModel : ISetsModel, ILinksModel {
    // link membership in group is automatically computed, so this method is unnecessary
    //void DoGroupLinksChanged(Object linkdata);

    // Navigation
    IEnumerable<Object> GetGroupsForLink(Object linkdata);
    IEnumerable<Object> GetMemberLinksForGroup(Object nodedata);

    // Modification, also not needed:
    //void AddLinkGroupNodeKey(Object linkdata, Object groupkey);
    //void RemoveLinkGroupNodeKey(Object linkdata, Object groupkey);
    //void ModifyLinkGroupNodeKeys(Object linkdata, System.Collections.IEnumerable groupkeys);
  }
#endif


  // For auxiliary model classes:

  /// <summary>
  /// A serializable collection of data representing a set of nodes and links.
  /// </summary>
  /// <seealso cref="TreeModel{NodeType, NodeKey}.DataCollection"/>
  /// <seealso cref="GraphModel{NodeType, NodeKey}.DataCollection"/>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.DataCollection"/>
  public interface IDataCollection {
    /// <summary>
    /// Gets or sets the model holds this collection's data.
    /// </summary>
    IDiagramModel Model { get; set; }

    /// <summary>
    /// Gets or sets the collection of node data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> Nodes { get; set; }

    /// <summary>
    /// This predicate is true if the given node data is in the collection of <see cref="Nodes"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool ContainsNode(Object nodedata);

    /// <summary>
    /// Add a node data to this collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void AddNode(Object nodedata);

    /// <summary>
    /// Remove a node data from this collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void RemoveNode(Object nodedata);

    /// <summary>
    /// Gets or sets the collection of link data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will always be an empty sequence for those models that do not support separate link data.
    /// Setting this property is an error for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    IEnumerable<Object> Links { get; set; }

    /// <summary>
    /// This predicate is true if the given link data is in the collection of <see cref="Links"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This will always be false for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    bool ContainsLink(Object linkdata);

    /// <summary>
    /// Add a link data to this collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is an error for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    void AddLink(Object linkdata);

    /// <summary>
    /// Remove a link data from this collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is an error for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    void RemoveLink(Object linkdata);
  }


  /// <summary>
  /// A dictionary of data representing a set of nodes and links and their corresponding copies,
  /// used during the copying process.
  /// </summary>
  /// <seealso cref="TreeModel{NodeType, NodeKey}.CopyDictionary"/>
  /// <seealso cref="GraphModel{NodeType, NodeKey}.CopyDictionary"/>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.CopyDictionary"/>
  public interface ICopyDictionary {
    /// <summary>
    /// Gets or sets the source model for the copying operation.
    /// </summary>
    IDiagramModel SourceModel { get; set; }

    /// <summary>
    /// Gets or sets the destination model for the copying operation.
    /// </summary>
    IDiagramModel DestinationModel { get; set; }

    /// <summary>
    /// Gets or sets the source collection of data to be copied for the copying operation.
    /// </summary>
    IDataCollection SourceCollection { get; set; }

    /// <summary>
    /// Gets the collection of copied nodes and copied links as an <see cref="IDataCollection"/>.
    /// </summary>
    IDataCollection Copies { get; }

    /// <summary>
    /// This predicate is true if the given node data is in the source collection.
    /// </summary>
    /// <param name="srcnodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    bool ContainsSourceNode(Object srcnodedata);

    /// <summary>
    /// Look up the copied node for a given source node.
    /// </summary>
    /// <param name="srcnodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    Object FindCopiedNode(Object srcnodedata);

    /// <summary>
    /// Declare the mapping of a source node data to a copied node data.
    /// </summary>
    /// <param name="srcnodedata"></param>
    /// <param name="dstnodedata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void AddCopiedNode(Object srcnodedata, Object dstnodedata);

    /// <summary>
    /// Remove any association between a source node data and any copied node data.
    /// </summary>
    /// <param name="srcnodedata"></param>
    /// <remarks>
    /// <para>
    /// Note that this method is universal, because it can only assume the node data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific node data type.
    /// </para>
    /// </remarks>
    void RemoveSourceNode(Object srcnodedata);

    /// <summary>
    /// This predicate is true if the given link data is in the source collection.
    /// </summary>
    /// <param name="srclinkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This will always be false for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    bool ContainsSourceLink(Object srclinkdata);

    /// <summary>
    /// Look up the copied link for a given source link.
    /// </summary>
    /// <param name="srclinkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This will always return null for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    Object FindCopiedLink(Object srclinkdata);

    /// <summary>
    /// Declare the mapping of a source link data to a copied link data.
    /// </summary>
    /// <param name="srclinkdata"></param>
    /// <param name="dstlinkdata"></param>
    /// <remarks>
    /// <para>
    /// This is an error for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    void AddCopiedLink(Object srclinkdata, Object dstlinkdata);

    /// <summary>
    /// Remove any association between a source link data and any copied link data.
    /// </summary>
    /// <param name="srclinkdata"></param>
    /// <remarks>
    /// <para>
    /// This is an error for those models that do not support separate link data.
    /// </para>
    /// <para>
    /// Note that this method is universal, because it can only assume the link data is of type <see cref="Object"/>.
    /// The corresponding methods in the generic model classes operate on and return a specific link data type.
    /// </para>
    /// </remarks>
    void RemoveSourceLink(Object srclinkdata);
  }


  /// <summary>
  /// Implement this interface on your node data or link data classes
  /// to support undo and redo functionality.
  /// </summary>
  /// <seealso cref="Northwoods.GoXam.Model.TreeModelNodeData{NodeKey}"/>
  /// <seealso cref="Northwoods.GoXam.Model.GraphModelNodeData{NodeKey}"/>
  /// <seealso cref="Northwoods.GoXam.Model.GraphLinksModelNodeData{NodeKey}"/>
  /// <seealso cref="Northwoods.GoXam.Model.GraphLinksModelLinkData{NodeKey, PortKey}"/>
  public interface IChangeDataValue {
    /// <summary>
    /// This method is called during an undo or a redo to modify the state
    /// of the data object implementing the <see cref="IChangeDataValue"/> interface.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    void ChangeDataValue(ModelChangedEventArgs e, bool undo);
  }


#if SILVERLIGHT  // ICloneable, for data
  /// <summary>
  /// This interface supports the cloning (copying) of data.
  /// </summary>
  public interface ICloneable {
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>a new object of the same type with the same data</returns>
    object Clone();
  }
#endif

}
