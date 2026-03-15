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
  /// This class represents a change to a model.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This inherits from <see cref="PropertyChangedEventArgs"/> so that it can be used as the
  /// <c>EventArgs</c> passed along for a <c>PropertyChanged</c> event
  /// for those data classes that implement <see cref="INotifyPropertyChanged"/>.
  /// </para>
  /// <para>
  /// This also implements the <see cref="IUndoableEdit"/> interface so that it
  /// can be used to remember changes in a <see cref="UndoManager"/>, as part of a
  /// <see cref="UndoManager.CompoundEdit"/>.
  /// </para>
  /// </remarks>
  public class ModelChangedEventArgs : PropertyChangedEventArgs, IUndoableEdit {
    /// <summary>
    /// The empty/default constructor produces an <see cref="EventArgs"/>
    /// whose properties still need to be initialized.
    /// </summary>
    /// <remarks>
    /// This is only used internally, for representing predefined <see cref="ModelChange"/> changes.
    /// </remarks>
    public ModelChangedEventArgs() : base("") { }

    /// <summary>
    /// This constructor initializes the mostly commonly used properties.
    /// </summary>
    /// <param name="pname">the <see cref="PropertyChangedEventArgs.PropertyName"/> property</param>
    /// <param name="data">the <see cref="Data"/> property</param>
    /// <param name="oldval">the <see cref="OldValue"/> property</param>
    /// <param name="newval">the <see cref="NewValue"/> property</param>
    /// <remarks>
    /// You may also need to initialize other properties, such as <see cref="OldParam"/> and <see cref="NewParam"/>.
    /// </remarks>
    public ModelChangedEventArgs(String pname, Object data, Object oldval, Object newval) : base(pname) {
      this.Data = data;
      this.OldValue = oldval;
      this.NewValue = newval;
    }

    /// <summary>
    /// This is basically a "copy constructor", making a copy of the given <see cref="ModelChangedEventArgs"/>.
    /// </summary>
    /// <param name="e"></param>
    public ModelChangedEventArgs(ModelChangedEventArgs e) : base(e.PropertyName) {  // "copy constructor"
      if (e != null) {
        this.Model = e.Model;
        this.Change = e.Change;
        this.Data = e.Data;
        this.OldValue = e.OldValue;
        this.OldParam = e.OldParam;
        this.NewValue = e.NewValue;
        this.NewParam = e.NewParam;
      }

      //if (this.Model != null) {
      //  this.Model.CopyOldValueForUndo(this);  //?? optimize memory usage
      //  this.Model.CopyNewValueForRedo(this);
      //}
    }

    /// <summary>
    /// Gets or sets the <see cref="IDiagramModel"/> that has been modified.
    /// </summary>
    public IDiagramModel Model { get; set; }

    /// <summary>
    /// Gets or sets the kind of change that this represents.
    /// </summary>
    /// <value>
    /// This is of type <see cref="ModelChange"/>.
    /// It defaults to <see cref="ModelChange.Property"/>.
    /// </value>
    public ModelChange Change { get; set; }

    /// <summary>
    /// Gets or sets the data object, part of the model, that was modified.
    /// </summary>
    /// <remarks>
    /// Typically this will be either data representing a node or
    /// data representing a link.
    /// </remarks>
    public Object Data { get; set; }

    /// <summary>
    /// Gets or sets the previous or old value that the property had.
    /// </summary>
    public Object OldValue { get; set; }

    /// <summary>
    /// Gets or sets an optional value associated with the old value.
    /// </summary>
    public Object OldParam { get; set; }

    /// <summary>
    /// Gets or sets the next or current value that the property has.
    /// </summary>
    public Object NewValue { get; set; }

    /// <summary>
    /// Gets or sets an optional value associated with the new value.
    /// </summary>
    public Object NewParam { get; set; }

    /// <summary>
    /// This is a convenient method to get the right value,
    /// depending on the value of <paramref name="undo"/>.
    /// </summary>
    /// <param name="undo"></param>
    /// <returns>either <see cref="OldValue"/> or <see cref="NewValue"/></returns>
    /// <remarks>
    /// This is useful
    /// in implementations of <see cref="IChangeDataValue.ChangeDataValue"/>
    /// or in overrides of <see cref="DiagramModel.ChangeDataValue"/>.
    /// </remarks>
    public Object GetValue(bool undo) {
      if (undo)
        return this.OldValue;
      else
        return this.NewValue;
    }

    /// <summary>
    /// This is a convenient method to get the right parameter value,
    /// depending on the value of <paramref name="undo"/>.
    /// </summary>
    /// <param name="undo"></param>
    /// <returns>either <see cref="OldParam"/> or <see cref="NewParam"/></returns>
    /// <remarks>
    /// This is useful
    /// in implementations of <see cref="IChangeDataValue.ChangeDataValue"/>
    /// or in overrides of <see cref="DiagramModel.ChangeDataValue"/>.
    /// </remarks>
    public Object GetParam(bool undo) {
      if (undo)
        return this.OldParam;
      else
        return this.NewParam;
    }

    /// <summary>
    /// Produce a human-readable description of the change to the model.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      String s = (this.Change < 0 ? "* " : "! ");
      if (this.Model != null && this.Model.Name != null) s += this.Model.Name + " ";
      s += this.Change.ToString();
      if (this.Change == ModelChange.Property) s += " " + this.PropertyName;
      s += ": ";
      if (this.Data != null) {
#if DATASET  // DataRowView
        System.Data.DataRowView row = this.Data as System.Data.DataRowView;
        if (row != null) {
          System.Data.DataRow r = row.Row;
          if (r.RowState != System.Data.DataRowState.Detached && r.RowState != System.Data.DataRowState.Deleted) {
            foreach (Object c in r.ItemArray) {
              if (r != null) s += c.ToString() + ",";
            }
          }
          s += " " + r.RowState.ToString();
          s += " " + row.RowVersion.ToString();
        } else
#endif
        s += this.Data.ToString();
      }
      if (this.OldValue != null) s += " old: " + this.OldValue.ToString();
      if (this.OldParam != null) s += " " + this.OldParam.ToString();
      if (this.NewValue != null) s += " new: " + this.NewValue.ToString();
      if (this.NewParam != null) s += " " + this.NewParam.ToString();
      return s;
    }

    // IUndoableEdit: Clear, CanUndo, Undo, CanRedo, Redo

    /// <summary>
    /// Forget any references that this object may have.
    /// </summary>
    public void Clear() {
      this.Model = null;
      this.Data = null;
      this.OldValue = null;
      this.OldParam = null;
      this.NewValue = null;
      this.NewParam = null;
    }

    /// <summary>
    /// This predicate returns true if you can call <see cref="Undo"/>.
    /// </summary>
    /// <returns></returns>
    public bool CanUndo() {
      return (this.Model != null);
    }

    /// <summary>
    /// Reverse the effects of this document change
    /// by calling <see cref="IDiagramModel.ChangeModel"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CanUndo"/> must be true for this method to call <see cref="IDiagramModel.ChangeModel"/>.
    /// </remarks>
    public void Undo() {
      if (CanUndo()) {
        this.Model.ChangeModel(this, true);
      }
    }

    /// <summary>
    /// This predicate returns true if you can call <see cref="Redo"/>.
    /// </summary>
    /// <returns></returns>
    public bool CanRedo() {
      return (this.Model != null);
    }

    /// <summary>
    /// Re-perform the document change after an <see cref="Undo"/>
    /// by calling <see cref="IDiagramModel.ChangeModel"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CanRedo"/> must be true for this method to call <see cref="IDiagramModel.ChangeModel"/>.
    /// </remarks>
    public void Redo() {
      if (CanRedo()) {
        this.Model.ChangeModel(this, false);
      }
    }
  }

}
