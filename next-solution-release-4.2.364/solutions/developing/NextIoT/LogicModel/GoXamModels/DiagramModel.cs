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
using System.ComponentModel;

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// The common base class for all predefined model classes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is not a generic class, unlike the predefined model classes
  /// (<see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>,
  /// <see cref="GraphModel{NodeType, NodeKey}"/>,
  /// <see cref="TreeModel{NodeType, NodeKey}"/>).
  /// </para>
  /// <para>
  /// This class defines several kinds of members:
  /// <list type="bullet">
  /// <item>
  /// a few properties:
  /// <see cref="Name"/>, <see cref="DataFormat"/>, <see cref="Modifiable"/>
  /// </item>
  /// <item>
  /// the <see cref="Changed"/> event and some methods for raising that event
  /// </item>
  /// <item>
  /// the <see cref="DiagramModel.UndoManager"/> property and some methods
  /// for executing state changes due to an undo or redo
  /// </item>
  /// <item>
  /// some methods for starting and finishing edits (i.e. groups of state changes)
  /// </item>
  /// <item>
  /// the <see cref="IsModified"/> property
  /// </item>
  /// </list>
  /// </para>
  /// </remarks>
  /// <seealso cref="IDiagramModel"/>
  /// <seealso cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  /// <seealso cref="GraphModel{NodeType, NodeKey}"/>
  /// <seealso cref="TreeModel{NodeType, NodeKey}"/>
  public abstract class DiagramModel {
    // common model state
    private String _Name = "";
    private String _DataFormat;
    private bool _Modifiable;

    // transient state
    [NonSerialized]
    private EventHandler<ModelChangedEventArgs> _ChangedEvent;

    private UndoManager _UndoManager;
    private int _UndoEditIndex = -2;
    private bool _HasUndoManager;

    private bool _IsModified;

    /// <summary>
    /// The constructor is protected because this class is abstract.
    /// </summary>
    protected DiagramModel() {}

    /// <summary>
    /// Gets or sets whether the model is being constructed or re-constructed.
    /// </summary>
    protected bool Initializing { get; set; }

    /// <summary>
    /// Reset the fields that should not be shared from a copy created by <see cref="Object.MemberwiseClone"/>.
    /// </summary>
    /// <remarks>
    /// If you override this method because you have added some fields,
    /// be sure to call the base method too.
    /// </remarks>
    protected virtual void Reinitialize() {
      _ChangedEvent = null;

      _UndoManager = null;
      _UndoEditIndex = -2;

      _IsModified = false;
    }

    
    /// <summary>
    /// A name for this model.
    /// </summary>
    /// <value>
    /// By default this is an empty string.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// This is mostly used to help distinguish between different models of the same type.
    /// </remarks>
    public String Name {
      get { return _Name; }
      set {
        String old = _Name;
        if (old != value && value != null) {
          _Name = value;
          RaisePropertyChanged("Name", this, old, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the format of this model's data.
    /// </summary>
    /// <value>
    /// By default this is the fully qualified name of this model type.
    /// This property cannot be set to null.
    /// </value>
    /// <remarks>
    /// This string is used by clipboard and drag-and-drop operations to distinguish
    /// between different and presumably incompatible data sources.
    /// You may wish to provide different values in order to prevent data
    /// from being transferred to other applications that are using the same model class.
    /// </remarks>
    public String DataFormat {
      get {
        if (_DataFormat == null) {
          _DataFormat = GetType().FullName;
          // can't allow some characters -- would result in Win32Exception
          int idx = _DataFormat.IndexOf('[');
          if (idx > 0) _DataFormat = _DataFormat.Substring(0, idx);
        }
        return _DataFormat;
      }
      set {
        String old = _DataFormat;
        if (old != value && value != null) {
          _DataFormat = value;
          RaisePropertyChanged("DataFormat", this, old, value);
        }
      }
    }


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
    public bool Modifiable {
      get { return _Modifiable; }
      set {
        bool old = _Modifiable;
        if (old != value) {
          _Modifiable = value;
          RaisePropertyChanged("Modifiable", this, old, value);
        }
      }
    }


    /// <summary>
    /// The Changed event is raised whenever the model is modified.
    /// </summary>
    /// <seealso cref="ModelChangedEventArgs"/>
    /// <seealso cref="ModelChange"/>
    public event EventHandler<ModelChangedEventArgs> Changed {
      add { _ChangedEvent = _ChangedEvent + value; }
      remove { _ChangedEvent = _ChangedEvent - value; }
    }


    /// <summary>
    /// Raises the <see cref="Changed"/> event.
    /// </summary>
    /// <param name="e">an edit describing the change that just happened</param>
    /// <remarks>
    /// If you override this method, be sure to call the base method first.
    /// </remarks>
    protected virtual void OnChanged(ModelChangedEventArgs e) {
      if (e == null) return;
      if (_ChangedEvent != null) _ChangedEvent(this, e);

      if (!this.SkipsUndoManager && !this.Initializing) {
        UndoManager um = this.UndoManager;
        if (um != null) um.HandleModelChanged(this, e);
        if (((int)e.Change) >= (int)ModelChange.Property) this.IsModified = true;
      }
    }

    /// <summary>
    /// Raise a <see cref="Changed"/> event, given a <see cref="ModelChangedEventArgs"/>.
    /// </summary>
    /// <param name="e">an edit describing the change that just happened</param>
    /// <remarks>
    /// This just calls <see cref="OnChanged"/>.
    /// This method is public because it is part of the implementation of <see cref="IDiagramModel"/>.
    /// </remarks>
    public void RaiseChanged(ModelChangedEventArgs e) {
      OnChanged(e);
    }

    /// <summary>
    /// Raise a <see cref="Changed"/> event, given before and after values for a particular property.
    /// </summary>
    /// <param name="propname">a property name</param>
    /// <param name="data">the object whose property has just changed</param>
    /// <param name="oldval">the previous value for the property</param>
    /// <param name="newval">the new value for the property</param>
    /// <remarks>
    /// This is the mostly commonly used way to notify about changes to the model or to the model's data.
    /// </remarks>
    /// <seealso cref="RaiseChanged(ModelChangedEventArgs)"/>
    /// <seealso cref="RaisePropertyChanged(String, Object, Object, Object, Object, Object)"/>
    public void RaisePropertyChanged(String propname, Object data, Object oldval, Object newval) {
      ModelChangedEventArgs e = new ModelChangedEventArgs(propname, data, oldval, newval);
      e.Model = (IDiagramModel)this;
      e.Change = ModelChange.Property;
      OnChanged(e);
    }

    /// <summary>
    /// Raise a <see cref="Changed"/> event for a property change.
    /// </summary>
    /// <param name="propname">a property name</param>
    /// <param name="data">the object whose property has just changed</param>
    /// <param name="oldval">the previous value for the property</param>
    /// <param name="oldparam">additional information for the old value</param>
    /// <param name="newval">the new value for the property</param>
    /// <param name="newparam">additional information for the new value</param>
    /// <remarks>
    /// This is not used as frequently as <see cref="RaisePropertyChanged(String, Object, Object, Object)"/>.
    /// Typically the parameter values are used as indexes into the <paramref name="data"/>,
    /// to identify the particular value that was changed.
    /// </remarks>
    /// <seealso cref="RaiseChanged(ModelChangedEventArgs)"/>
    /// <seealso cref="RaisePropertyChanged(String, Object, Object, Object)"/>
    public void RaisePropertyChanged(String propname, Object data, Object oldval, Object oldparam, Object newval, Object newparam) {
      ModelChangedEventArgs e = new ModelChangedEventArgs(propname, data, oldval, newval);
      e.Model = (IDiagramModel)this;
      e.Change = ModelChange.Property;
      e.OldParam = oldparam;
      e.NewParam = newparam;
      OnChanged(e);
    }

    internal void RaiseModelChanged(ModelChange change, Object data, Object oldval, Object newval) {
      ModelChangedEventArgs e = new ModelChangedEventArgs();
      e.Model = (IDiagramModel)this;
      e.Change = change;
      e.Data = data;
      e.OldValue = oldval;
      e.NewValue = newval;
      OnChanged(e);
    }

    internal void RaiseModelChanged(ModelChange change, Object data, Object oldval, Object oldparam, Object newval, Object newparam) {
      ModelChangedEventArgs e = new ModelChangedEventArgs();
      e.Model = (IDiagramModel)this;
      e.Change = change;
      e.Data = data;
      e.OldValue = oldval;
      e.OldParam = oldparam;
      e.NewValue = newval;
      e.NewParam = newparam;
      OnChanged(e);
    }

    /// <summary>
    /// Gets or sets the <see cref="UndoManager"/> for this model.
    /// </summary>
    /// <value>
    /// This value may be null, when there is no <see cref="UndoManager"/>.
    /// By default this value is null.
    /// If this value is null, it is automatically set to a new <see cref="UndoManager"/>
    /// when <see cref="HasUndoManager"/> is changed to be true.
    /// </value>
    /// <remarks>
    /// Of course, when there is no UndoManager, the user cannot perform an Undo or a Redo.
    /// Setting this property does not raise a <see cref="Changed"/> event.
    /// Setting this property may also change the value of <see cref="HasUndoManager"/>.
    /// </remarks>
    /// <seealso cref="HasUndoManager"/>
    /// <seealso cref="SkipsUndoManager"/>
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public UndoManager UndoManager {
      get { return _UndoManager; }
      set {
        UndoManager old = _UndoManager;
        if (old != value) {
          if (old != null) old.RemoveModel((IDiagramModel)this);
          _UndoManager = value;
          _IsModified = false;
          _UndoEditIndex = -2;
          if (value != null) value.AddModel((IDiagramModel)this);
          _HasUndoManager = (value != null);
        }
      }
    }

    /// <summary>
    /// Gets or sets a flag that enables or disables support for undo and redo.
    /// </summary>
    /// <value>
    /// Initially this property is false.
    /// Changing it to true will create a new undo/redo manager and set the
    /// <see cref="DiagramModel.UndoManager"/> property,
    /// if there is not already a value for that property.
    /// Changing it to false will remove any undo/redo manager.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is just a more convenient way to enable undo/redo support,
    /// instead of creating an <see cref="UndoManager"/> and setting the
    /// property in XAML using property element syntax.
    /// </para>
    /// <para>
    /// Setting this property does not raise a <see cref="Changed"/> event.
    /// Setting this property may also change the value of <see cref="UndoManager"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="UndoManager"/>
    public bool HasUndoManager {
      get { return _HasUndoManager; }
      set {
        bool old = _HasUndoManager;
        if (old != value) {
          _HasUndoManager = value;
          if (value && this.UndoManager == null) {
            this.UndoManager = new UndoManager();
          } else if (!value && this.UndoManager != null) {
            this.UndoManager = null;
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets a flag that tells <see cref="OnChanged"/> whether it
    /// should notify any <see cref="UndoManager"/> that a change has occurred.
    /// </summary>
    /// <value>
    /// This is normally false and should be set to true only transiently.
    /// Typically you may want to temporarily set this to true in order to avoid
    /// recording temporary changes to the model.
    /// </value>
    /// <remarks>
    /// Setting this property does not raise a <see cref="Changed"/> event.
    /// Do not use this property to disable the <see cref="UndoManager"/> --
    /// instead remove the <see cref="UndoManager"/>.
    /// </remarks>
    public bool SkipsUndoManager { get; set; }

    /// <summary>
    /// This property is true during a call to <see cref="ChangeModel"/>,
    /// indicating a change happening due to an undo or a redo.
    /// </summary>
    /// <remarks>
    /// Setting this property does not raise a <see cref="Changed"/> event.
    /// </remarks>
    public bool IsChangingModel { get; protected set; }


    /// <summary>
    /// This is called during an Undo or a Redo to actually make state
    /// changes to this model or to this model's data.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// <para>
    /// When <paramref name="e"/>'s <see cref="ModelChangedEventArgs.Change"/>
    /// value is <see cref="ModelChange.Property"/>,
    /// this calls <see cref="ChangeModelValue"/>
    /// if the <see cref="ModelChangedEventArgs.Data"/> is this model,
    /// or else it calls <see cref="ChangeDataValue"/>.
    /// </para>
    /// <para>
    /// This method handles all other <see cref="ModelChange"/> cases,
    /// since they are all predefined.
    /// </para>
    /// </remarks>
    public void ChangeModel(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      bool old = this.IsChangingModel;
      //ModelHelper.Trace((undo ? "undo: " : "redo: ") + e.ToString());
      try {
        this.IsChangingModel = true;
        if (e.Change == ModelChange.Property) {
          Object data = e.Data;
          if (data == null) data = this;
          if (data == this) {  // changes to programmer defined properties on the model
            ChangeModelValue(e, undo);
          } else {  // changes to some data inside the model
            ChangeDataValue(e, undo);
          }
        } else {
          ChangeModelState(e, undo);
        }
      } finally {
        this.IsChangingModel = old;
      }
    }

    /// <summary>
    /// This is called during undo or redo to effect state changes to this model.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// <para>
    /// This is called by <see cref="ChangeModel"/>.
    /// You will want to override this method to handle properties that you
    /// have added to your derived model class.
    /// </para>
    /// <para>
    /// By default this uses reflection to set the <see cref="PropertyChangedEventArgs.PropertyName"/>
    /// to the <see cref="ModelChangedEventArgs.OldValue"/> or the
    /// <see cref="ModelChangedEventArgs.NewValue"/>, depending on the value of <paramref name="undo"/>.
    /// </para>
    /// <para>
    /// If you override this method, remember to call the base method for all
    /// cases that your override method does not handle.
    /// </para>
    /// </remarks>
    protected virtual void ChangeModelValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.PropertyName == "Name") {
        this.Name = (String)e.GetValue(undo);
      } else if (e.PropertyName == "DataFormat") {
        this.DataFormat = (String)e.GetValue(undo);
      } else if (e.PropertyName == "Modifiable") {
        this.Modifiable = (bool)e.GetValue(undo);
      } else if (ModelHelper.SetProperty(e.PropertyName, this, e.GetValue(undo))) {
        return;  // successful set of model property
      } else {
        ModelHelper.Error((IDiagramModel)this, "Override ChangeModelValue to handle ModelChangedEventArgs of a model property: " + e.ToString());
      }
    }

    /// <summary>
    /// This is called during undo or redo to effect state changes to model data.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// <para>
    /// This is called by <see cref="ChangeModel"/>.
    /// You will want to override this method to handle properties that you
    /// have added to your model data classes.
    /// Or you can have your data classes implement <see cref="IChangeDataValue"/>
    /// to achieve the same effect.
    /// </para>
    /// <para>
    /// By default this just calls <see cref="IChangeDataValue.ChangeDataValue"/>
    /// if the <see cref="ModelChangedEventArgs.Data"/> implements <see cref="IChangeDataValue"/>.
    /// Otherwise this uses reflection to set the <see cref="PropertyChangedEventArgs.PropertyName"/>
    /// to the <see cref="ModelChangedEventArgs.OldValue"/> or the
    /// <see cref="ModelChangedEventArgs.NewValue"/>, depending on the value of <paramref name="undo"/>.
    /// </para>
    /// <para>
    /// If you override this method, remember to call the base method for all
    /// cases that your override method does not handle.
    /// </para>
    /// </remarks>
    protected virtual void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      Object data = e.Data;
      IChangeDataValue changeable = data as IChangeDataValue;
      if (changeable != null) {
        changeable.ChangeDataValue(e, undo);
      } else if (ModelHelper.SetProperty(e.PropertyName, data, e.GetValue(undo))) {
        return;  // successful set of data property
      } else {
        ModelHelper.Error((IDiagramModel)this, "Override ChangeDataValue to handle ModelChangedEventArgs.Data, or have data implement IChangeDataValue: " + data.ToString());
      }
    }

    internal /*?? protected */ virtual void ChangeModelState(ModelChangedEventArgs e, bool undo) {
      if ((int)e.Change > (int)ModelChange.Property) {
        ModelHelper.Error((IDiagramModel)this, "ChangeModelState did not handle ModelChange." + e.Change.ToString());
      }
    }


    internal int NoUndoManagerTransactionLevel { get; set; }
    
    /// <summary>
    /// Call the UndoManager's StartTransaction method.
    /// </summary>
    /// <param name="tname">a String describing the transaction</param>
    /// <returns>the value of the call to StartTransaction</returns>
    /// <remarks>
    /// Transactions may be nested, e.g. Start, Start, Commit, Commit.
    /// <see cref="Northwoods.GoXam.Model.UndoManager.StartTransaction"/>
    /// will raise a Changed event with a hint of <see cref="ModelChange.StartedTransaction"/>.
    /// </remarks>
    /// <seealso cref="CommitTransaction"/>
    public bool StartTransaction(String tname) {
      UndoManager m = this.UndoManager;
      if (m != null) {
        return m.StartTransaction(tname);
      } else {
        if (this.NoUndoManagerTransactionLevel == 0) RaiseModelChanged(ModelChange.StartedTransaction, tname, null, null);
        this.NoUndoManagerTransactionLevel++;
        return false;
      }
    }

    /// <summary>
    /// Call the UndoManager's CommitTransaction method.
    /// </summary>
    /// <param name="tname">a String describing the transaction</param>
    /// <returns>the value of the call to CommitTransaction</returns>
    /// <remarks>
    /// <see cref="Northwoods.GoXam.Model.UndoManager.CommitTransaction"/>
    /// will raise a Changed event with a hint of <see cref="ModelChange.CommittedTransaction"/>,
    /// and with a <see cref="ModelChangedEventArgs.Data"/>
    /// that is the <see cref="Northwoods.GoXam.Model.UndoManager.CompoundEdit"/>
    /// that was the value of <see cref="Northwoods.GoXam.Model.UndoManager.CurrentEdit"/>
    /// before calling <see cref="Northwoods.GoXam.Model.UndoManager.CommitTransaction"/>.
    /// </remarks>
    /// <seealso cref="StartTransaction"/>
    /// <seealso cref="RollbackTransaction"/>
    public bool CommitTransaction(String tname) {
      UndoManager m = this.UndoManager;
      if (m != null) {
        return m.CommitTransaction(tname);
      } else if (this.NoUndoManagerTransactionLevel > 0) {
        this.NoUndoManagerTransactionLevel--;
        if (this.NoUndoManagerTransactionLevel == 0) RaiseModelChanged(ModelChange.CommittedTransaction, tname, null, null);
      }
      return false;
    }

    /// <summary>
    /// Call the UndoManager's RollbackTransaction method.
    /// </summary>
    /// <returns>the value of the call to RollbackTransaction</returns>
    /// <remarks>
    /// After calling <see cref="Northwoods.GoXam.Model.UndoManager.RollbackTransaction"/>,
    /// if that call returned true, this raises a Changed event with a hint of
    /// <see cref="ModelChange.RolledBackTransaction"/>.
    /// </remarks>
    /// <seealso cref="CommitTransaction"/>
    public bool RollbackTransaction() {
      UndoManager m = this.UndoManager;
      if (m != null) {
        return m.RollbackTransaction();
      } else if (this.NoUndoManagerTransactionLevel > 0) {
        this.NoUndoManagerTransactionLevel--;
        if (this.NoUndoManagerTransactionLevel == 0) RaiseModelChanged(ModelChange.RolledBackTransaction, "", null, null);
      }
      return false;
    }

    /// <summary>
    /// This property is true after a <see cref="StartTransaction"/>
    /// and before a corresponding <see cref="CommitTransaction"/> or <see cref="RollbackTransaction"/>.
    /// </summary>
    public bool IsInTransaction {
      get {
        UndoManager m = this.UndoManager;
        if (m != null) {
          return m.TransactionLevel > 0;
        } else {
          return this.NoUndoManagerTransactionLevel > 0;
        }
      }
    }


    /// <summary>
    /// Gets or sets whether this model is considered changed from an earlier state.
    /// </summary>
    /// <value>
    /// true if this model has been marked as having been modified,
    /// if the <see cref="UndoManager"/> has recorded any changes, or
    /// if an undo has been performed without a corresponding redo.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is set to true in <see cref="OnChanged"/>.
    /// Setting this property does not raise a <see cref="Changed"/> event.
    /// </para>
    /// <para>
    /// Although you can set this property at any time, the value of
    /// <see cref="IsModified"/> will continue to be true as long as
    /// there have been changes made to the model and you are using an
    /// <see cref="Northwoods.GoXam.Model.UndoManager"/>.
    /// Any modifications to a model or one of its parts will result
    /// in setting <see cref="IsModified"/> to true and in adding fromport
    /// <see cref="ModelChangedEventArgs"/> to the
    /// <see cref="Northwoods.GoXam.Model.UndoManager.CurrentEdit"/> list,
    /// which holds all of the changes for undo/redo.
    /// </para>
    /// <para>
    /// When using an UndoManager, you should be making all changes within a transaction.
    /// After finishing or rolling back a transaction, you can set <see cref="IsModified"/>
    /// to false, and then it will remain false until another change is made to the model.
    /// </para>
    /// <para>
    /// When there is no <see cref="UndoManager"/>, this property is
    /// implemented as only a simple boolean state variable.
    /// </para>
    /// </remarks>
    public bool IsModified {
      get {
        UndoManager m = this.UndoManager;
        if (m == null) return _IsModified;
        if (m.CurrentEdit != null) return true;
        return _IsModified && _UndoEditIndex != m.UndoEditIndex;
      }
      set {
        bool old = _IsModified;
        _IsModified = value;
        if (!value && this.UndoManager != null) {
          _UndoEditIndex = this.UndoManager.UndoEditIndex;
        }
      }
    }
  }


  /// <summary>
  /// Specifies what kinds of cycles may be made by a valid link from a port.
  /// </summary>
  /// <remarks>
  /// This provides values for the properties <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.ValidCycle"/>
  /// and <see cref="GraphModel{NodeType, NodeKey}.ValidCycle"/>.
  /// </remarks>
  public enum ValidCycle {
    /// <summary>
    /// No restrictions on cycles.
    /// </summary>
    All=0,
    /// <summary>
    /// A valid link from a port will not produce a directed cycle in the graph.
    /// </summary>
    NotDirected=1,
    /// <summary>
    /// A valid link from a port will not produce a directed cycle in the graph,
    /// assuming there are no directed cycles anywhere accessible from either port.
    /// </summary>
    NotDirectedFast=2,
    /// <summary>
    /// A valid link from a port will not produce an undirected cycle in the graph.
    /// </summary>
    NotUndirected=3,
    /// <summary>
    /// Any number of destination links may go out of a port, but at most one
    /// source link may come into a port, and there are no directed cycles.
    /// </summary>
    DestinationTree=4,
    /// <summary>
    /// Any number of source links may come into a port, but at most one
    /// destination link may go out of a port, and there are no directed cycles.
    /// </summary>
    SourceTree=5
  }


  // multi-argument predicates:

  /// <summary>
  /// A generic delegate describing a two argument function returning a boolean value.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <returns>a boolean</returns>
  internal /*?? public */ delegate bool Predicate<T1, T2>(T1 arg1, T2 arg2);

  /// <summary>
  /// A generic delegate describing a three argument function returning a boolean value.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <returns>a boolean</returns>
  internal /*?? public */ delegate bool Predicate<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

  /// <summary>
  /// A generic delegate describing a four argument function returning a boolean value.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <returns>a boolean</returns>
  internal /*?? public */ delegate bool Predicate<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

  /// <summary>
  /// A generic delegate describing a five argument function returning a boolean value.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <returns>a boolean</returns>
  internal /*?? public */ delegate bool Predicate<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

  /// <summary>
  /// A generic delegate describing a six argument function returning a boolean value.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <typeparam name="T6"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <param name="arg6"></param>
  /// <returns>a boolean</returns>
  internal /*?? public */ delegate bool Predicate<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

#if !DOTNET4
  // five and six argument functions:

  /// <summary>
  /// A generic delegate describing a five argument function returning value of type <typeparamref name="TResult"/>.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <returns></returns>
  internal /*?? public */ delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

  /// <summary>
  /// A generic delegate describing a six argument function returning value of type <typeparamref name="TResult"/>.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <typeparam name="T6"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <param name="arg6"></param>
  /// <returns></returns>
  internal /*?? public */ delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);


  // five, six, and seven argument procedures:

  /// <summary>
  /// A generic delegate describing a five argument procedure.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  internal /*?? public */ delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

  /// <summary>
  /// A generic delegate describing a six argument procedure.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <typeparam name="T6"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <param name="arg6"></param>
  internal /*?? public */ delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

  /// <summary>
  /// A generic delegate describing a seven argument procedure.
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <typeparam name="T3"></typeparam>
  /// <typeparam name="T4"></typeparam>
  /// <typeparam name="T5"></typeparam>
  /// <typeparam name="T6"></typeparam>
  /// <typeparam name="T7"></typeparam>
  /// <param name="arg1"></param>
  /// <param name="arg2"></param>
  /// <param name="arg3"></param>
  /// <param name="arg4"></param>
  /// <param name="arg5"></param>
  /// <param name="arg6"></param>
  /// <param name="arg7"></param>
  internal /*?? public */ delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
#endif
}
