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
using System.ComponentModel;

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// The UndoManager class observes and records model changes and supports
  /// undo and redo operations.
  /// </summary>
  public class UndoManager {
    /// <summary>
    /// Create a <see cref="UndoManager"/> that is ready to record model
    /// modifications, but that does not know about any models yet.
    /// </summary>
    public UndoManager() {
      this.ModelsList = new List<IDiagramModel>();
      this.CompoundEdits = new List<IUndoableEdit>();
      this.UndoEditIndex = -1;
      this.NestedTransactionNames = new List<String>();
      this.NestedTransactionStarts = new List<int>();
      this.MaximumEditCount = 999;
    }

    /// <summary>
    /// For debugging.
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      String s = "UndoManager (";
      foreach (IDiagramModel m in this.Models) s += (m.Name != null ? m.Name + " " : "");
      s += ") ";
      s += this.UndoEditIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "<";
      s += this.CompoundEdits.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) + "<=";
      s += this.MaximumEditCount.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
      s += "[";
      foreach (String n in this.NestedTransactionNames) s += n + " ";
      s += "]";
      return s;
    }

    /// <summary>
    /// Clear all of the <see cref="UndoManager.CompoundEdit"/>s and reset all other state.
    /// </summary>
    /// <remarks>
    /// However, this does not forget the models that this undo manager is managing.
    /// </remarks>
    public virtual void Clear() {
      for (int i = this.CompoundEdits.Count-1; i >= 0; i--) {
        this.CompoundEdits[i].Clear();
      }
      this.CompoundEdits.Clear();
      this.UndoEditIndex = -1;
      this.CurrentEdit = null;
      this.TransactionLevel = 0;
      this.NestedTransactionNames.Clear();
      this.NestedTransactionStarts.Clear();
      this.IsUndoingRedoing = false;
      // does not change this.ModelsList
    }

    /// <summary>
    /// Make sure this undo manager knows about a <see cref="IDiagramModel"/> for which
    /// it is receiving model Changed event notifications.
    /// </summary>
    /// <param name="model"></param>
    /// <remarks>
    /// This just adds <paramref name="model"/> to the list of <see cref="Models"/>.
    /// </remarks>
    /// <seealso cref="RemoveModel"/>
    public virtual void AddModel(IDiagramModel model) {
      if (!this.ModelsList.Contains(model))
        this.ModelsList.Add(model);
    }

    /// <summary>
    /// Call this method to inform this undo manager that it no longer will be
    /// notified of model Changed events.
    /// </summary>
    /// <param name="model"></param>
    /// <remarks>
    /// This just removes <paramref name="model"/> from the list of <see cref="Models"/>.
    /// </remarks>
    /// <seealso cref="AddModel"/>
    public virtual void RemoveModel(IDiagramModel model) {
      this.ModelsList.Remove(model);
    }

    /// <summary>
    /// Gets a list of models for which this UndoManager is recording undo/redo
    /// information.
    /// </summary>
    /// <remarks>
    /// You can manipulate this list explicitly by calling
    /// <see cref="AddModel"/> and <see cref="RemoveModel"/>.
    /// Setting <see cref="IDiagramModel.UndoManager"/> automatically calls these methods.
    /// <see cref="Undo"/> and <see cref="Redo"/> use this list to call
    /// <see cref="IDiagramModel.RaiseChanged"/> with notices about starting and
    /// ending undo and redo actions, and about starting/finishing/rollingback edits.
    /// </remarks>
    public IEnumerable<IDiagramModel> Models { get { return this.ModelsList; } }

    private List<IDiagramModel> ModelsList { get; set; }


    /// <summary>
    /// Gets the current <see cref="IUndoableEdit"/> to be undone, or null if there is none.
    /// </summary>
    /// <seealso cref="CanUndo"/>
    /// <seealso cref="Undo"/>
    /// <seealso cref="CompoundEdits"/>
    /// <seealso cref="UndoEditIndex"/>
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public IUndoableEdit EditToUndo {
      get {
        if (this.UndoEditIndex >= 0 &&
          this.UndoEditIndex <= this.CompoundEdits.Count-1)
          return this.CompoundEdits[this.UndoEditIndex];
        else
          return null;
      }
    }

    /// <summary>
    /// This predicate is true when one can call <see cref="Undo"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// In order to be able to perform an undo, a transaction must not
    /// be in progress, nor an undo or a redo.
    /// Furthermore there must be an <see cref="EditToUndo"/> that itself
    /// is ready to be undone, because its <see cref="IUndoableEdit.CanUndo"/>
    /// predicate is true.
    /// </remarks>
    /// <seealso cref="Undo"/>
    public virtual bool CanUndo() {
      if (this.TransactionLevel > 0) return false;
      if (this.IsUndoingRedoing) return false;
      IUndoableEdit curr = this.EditToUndo;
      return curr != null && curr.CanUndo();
    }

    /// <summary>
    /// Restore the state of some models to before the current <see cref="IUndoableEdit"/>.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="IUndoableEdit.Undo"/> on the current <see cref="EditToUndo"/>.
    /// This will raise a <see cref="IDiagramModel.Changed"/> event with a hint of
    /// <see cref="ModelChange.StartingUndo"/> before actually performing the undo, and will raise a
    /// Changed event with a hint of <see cref="ModelChange.FinishedUndo"/> afterwards.
    /// The <see cref="ModelChangedEventArgs.Data"/>
    /// is the <see cref="UndoManager.CompoundEdit"/> that was the value of
    /// <see cref="EditToUndo"/> before calling Undo.
    /// </remarks>
    /// <seealso cref="CanUndo"/>
    public virtual void Undo() {
      if (!CanUndo()) return;
      IUndoableEdit edit = this.EditToUndo;
      try {
        foreach (IDiagramModel model in this.ModelsList) {
          RaiseChanged(model, ModelChange.StartingUndo, edit, null, null);
        }
        this.IsUndoingRedoing = true;
        this.UndoEditIndex--;
        edit.Undo();
      } catch (Exception ex) {
        ModelHelper.Trace("Undo: " + ex.ToString());
        throw;
      } finally {
        this.IsUndoingRedoing = false;
        foreach (IDiagramModel model in this.ModelsList) {
          RaiseChanged(model, ModelChange.FinishedUndo, edit, null, null);
        }
      }
    }

    /// <summary>
    /// This property is true during a call to <see cref="Undo"/> or <see cref="Redo"/>.
    /// </summary>
    /// <remarks>
    /// When this property is true, <see cref="CanUndo"/> and
    /// <see cref="CanRedo"/> will be false.
    /// To avoid confusion, <see cref="HandleModelChanged"/> ignores all model change events
    /// when this property is true.
    /// </remarks>
    public bool IsUndoingRedoing { get; protected set; }
  

    /// <summary>
    /// Gets the current <see cref="IUndoableEdit"/> to be redone, or null if there is none.
    /// </summary>
    /// <seealso cref="CanRedo"/>
    /// <seealso cref="Redo"/>
    /// <seealso cref="CompoundEdits"/>
    /// <seealso cref="UndoEditIndex"/>
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public IUndoableEdit EditToRedo {
      get {
        if (this.UndoEditIndex < this.CompoundEdits.Count-1)
          return this.CompoundEdits[this.UndoEditIndex+1];
        else
          return null;
      }
    }

    /// <summary>
    /// This predicate is true when one can call <see cref="Redo"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// In order to be able to perform a redo, a transaction must not
    /// be in progress, nor an undo or a redo.
    /// Furthermore there must be an <see cref="EditToRedo"/> that itself
    /// is ready to be redone, because its <see cref="IUndoableEdit.CanRedo"/>
    /// predicate is true.
    /// </remarks>
    /// <seealso cref="Redo"/>
    public virtual bool CanRedo() {
      if (this.TransactionLevel > 0) return false;
      if (this.IsUndoingRedoing) return false;
      IUndoableEdit curr = this.EditToRedo;
      return curr != null && curr.CanRedo();
    }

    /// <summary>
    /// Restore the state of some models to after the current <see cref="IUndoableEdit"/>.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="IUndoableEdit.Redo"/> on the current <see cref="EditToRedo"/>.
    /// This will raise a <see cref="IDiagramModel.Changed"/> event with a hint of
    /// <see cref="ModelChange.StartingRedo"/> before actually performing the redo, and will raise a
    /// Changed event with a hint of <see cref="ModelChange.FinishedRedo"/> afterwards.
    /// The <see cref="ModelChangedEventArgs.Data"/>
    /// is the <see cref="UndoManager.CompoundEdit"/> that was the value of
    /// <see cref="EditToRedo"/> before calling Redo.
    /// </remarks>
    /// <seealso cref="CanRedo"/>
    public virtual void Redo() {
      if (!CanRedo()) return;
      IUndoableEdit edit = this.EditToRedo;
      try {
        foreach (IDiagramModel model in this.ModelsList) {
          RaiseChanged(model, ModelChange.StartingRedo, edit, null, null);
        }
        this.IsUndoingRedoing = true;
        this.UndoEditIndex++;
        edit.Redo();
      } catch (Exception ex) {
        ModelHelper.Trace("Redo: " + ex.ToString());
        throw;
      } finally {
        this.IsUndoingRedoing = false;
        foreach (IDiagramModel model in this.ModelsList) {
          RaiseChanged(model, ModelChange.FinishedRedo, edit, null, null);
        }
      }
    }


    /// <summary>
    /// Gets a list of all of the compound edits.
    /// </summary>
    /// <value>This will be a <c>List</c> of mostly <see cref="UndoManager.CompoundEdit"/>s</value>
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public IList<IUndoableEdit> CompoundEdits { get; private set; }

    /// <summary>
    /// Gets or sets the maximum number of compound edits that this undo manager will remember.
    /// </summary>
    /// <value>
    /// If the value is negative, no limit is assumed.
    /// A new value of zero is treated as if the new value were one.
    /// The initial value is 999.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is useful in helping limit the memory consumption of typical application usage.
    /// But note that this only limits the number of compound edits, not the size of any individual
    /// <see cref="UndoManager.CompoundEdit"/>, which may still have an unlimited number of
    /// <see cref="ModelChangedEventArgs"/>s.
    /// </para>
    /// <para>
    /// Decreasing this value will not necessarily remove any existing edits
    /// if there currently exist more edits in <see cref="CompoundEdits"/> than the new value would allow.
    /// </para>
    /// </remarks>
    public int MaximumEditCount { get; set; }

    /// <summary>
    /// Gets the index into <see cref="CompoundEdits"/> for the current undoable edit.
    /// </summary>
    /// <value>
    /// -1 if there's no undoable edit to be undone.
    /// </value>
    public int UndoEditIndex { get; protected set; }
    

    /// <summary>
    /// Create an <see cref="IUndoableEdit"/> for a <see cref="IDiagramModel.Changed"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    /// This calls <see cref="SkipEvent"/> if for some reason we should ignore
    /// the <paramref name="e"/>.
    /// This then creates a <see cref="ModelChangedEventArgs"/> and adds it to the
    /// <see cref="CurrentEdit"/>, a <see cref="UndoManager.CompoundEdit"/> which it allocates
    /// if needed.
    /// This method always ignores all Changed events while we are performing an
    /// <see cref="Undo"/> or <see cref="Redo"/>.
    /// </remarks>
    public virtual void HandleModelChanged(Object sender, ModelChangedEventArgs e) {
      // changes caused by performing an undo or redo should be ignored!
      if (this.IsUndoingRedoing) return;

      if (!SkipEvent(e)) {
        CompoundEdit cedit = this.CurrentEdit;
        //ModelHelper.Trace(this.TransactionLevel, e.ToString());
        if (cedit == null) {
          cedit = new CompoundEdit();
          this.CurrentEdit = cedit;
        }

        // make a copy of the event to save as an edit in the list
        ModelChangedEventArgs edit = new ModelChangedEventArgs(e);
        cedit.Edits.Add(edit);
        if (this.ChecksTransactionLevel && this.TransactionLevel <= 0) {
          ModelHelper.Trace("Change not within a transaction: " + edit.ToString());
        }
      }
    }

    /// <summary>
    /// This predicate is responsible for deciding if a <see cref="ModelChangedEventArgs"/>
    /// is not interesting enough to be recorded.
    /// </summary>
    /// <param name="evt"></param>
    /// <returns>normally false, which causes the given event to be remembered;
    /// but true for negative valued enumerations of <see cref="ModelChange"/>.</returns>
    /// <remarks>
    /// </remarks>
    protected virtual bool SkipEvent(ModelChangedEventArgs evt) {
      if (evt == null) return true;
      return (int)evt.Change < (int)ModelChange.Property;
    }

    /// <summary>
    /// Gets the current compound edit for recording additional model change events.
    /// </summary>
    /// <remarks>
    /// This is initialized and augmented by <see cref="HandleModelChanged"/>
    /// before it is added to <see cref="CompoundEdits"/> by a top-level call to <see cref="CommitTransaction"/>.
    /// </remarks>
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public CompoundEdit CurrentEdit { get; set; }


    /// <summary>
    /// Begin a transaction, where the changes are held by a <see cref="UndoManager.CompoundEdit"/>.
    /// </summary>
    /// <returns>true if starting a top-level transaction</returns>
    /// <remarks>
    /// Transactions can be nested:
    /// <list type="numbered">
    /// <item><c>StartTransaction</c> returns true</item>
    /// <item><c>StartTransaction</c> returns false</item>
    /// <item><c>CommitTransaction</c> returns false</item>
    /// <item><c>CommitTransaction</c> returns true</item>
    /// </list>
    /// Nested transactions will share the same compound edit as the top-level one.
    /// This will raise a <see cref="IDiagramModel.Changed"/> event for each of the <see cref="Models"/>,
    /// with a hint of <see cref="ModelChange.StartedTransaction"/>.
    /// </remarks>
    /// <seealso cref="EndTransaction"/>
    public virtual bool StartTransaction(String tname) {
      if (tname == null) tname = "";
      if (this.IsUndoingRedoing) return false;
      //ModelHelper.Trace(this.TransactionLevel, "start: " + tname);
      this.NestedTransactionNames.Add(tname);
      this.NestedTransactionStarts.Add(this.CurrentEdit != null ? this.CurrentEdit.Edits.Count : 0);
      this.TransactionLevel++;
      bool result = (this.TransactionLevel == 1);
      if (result) {
        foreach (IDiagramModel model in this.ModelsList) {
          RaiseChanged(model, ModelChange.StartedTransaction, tname, null, null);
        }
      }
      return result;
    }

    /// <summary>
    /// Just call <see cref="EndTransaction"/>, rolling back the current transaction.
    /// </summary>
    /// <returns>the value of the call to <see cref="EndTransaction"/></returns>
    public bool RollbackTransaction() {
      return EndTransaction(false, null);
    }

    /// <summary>
    /// Just call <see cref="EndTransaction"/>, committing the current transaction,
    /// with the presentation name for the transaction.
    /// </summary>
    /// <param name="tname">
    /// the transaction name
    /// </param>
    /// <returns>the value of the call to <see cref="EndTransaction"/></returns>
    public bool CommitTransaction(String tname) {
      return EndTransaction(true, tname);
    }

    /// <summary>
    /// Stop the current transaction, either rolling it back or committing it.
    /// </summary>
    /// <param name="commit">true to terminate the transaction normally;
    /// false to abort it and rollback the existing edits</param>
    /// <param name="tname">the internal locale-neutral name for the transaction</param>
    /// <returns>true for a committed top-level transaction</returns>
    /// <remarks>
    /// <para>
    /// If this call stops a top-level transaction, a value of false for
    /// <paramref name="commit"/> just undoes and discards the information in the
    /// <see cref="CurrentEdit"/>.
    /// If <paramref name="commit"/> is true for a top-level transaction,
    /// we mark the <see cref="CurrentEdit"/> complete,
    /// call <see cref="CommitCompoundEdit"/>,
    /// and add the resulting <see cref="UndoManager.CompoundEdit"/>
    /// to the list of compound edits that this undo manager is recording.
    /// </para>
    /// <para>
    /// Committing a transaction when there have been some undos without
    /// corresponding redos will throw away the compound edits holding
    /// changes that happened after the current state, before adding this
    /// new compound edit to the undo manager's list of edits.
    /// </para>
    /// <para>
    /// This method raises a <see cref="IDiagramModel.Changed"/> event
    /// for each of this undo manager's <see cref="Models"/>,
    /// with a hint of <see cref="ModelChange.CommittedTransaction"/>,
    /// and with a <see cref="ModelChangedEventArgs.OldValue"/>
    /// that is the <see cref="UndoManager.CompoundEdit"/>
    /// that has been added to the list of <see cref="CompoundEdits"/>.
    /// Similarly, if the transaction is aborted, either because <paramref name="commit"/>
    /// is false or because there is no <see cref="CurrentEdit"/> to commit,
    /// all of the <see cref="Models"/> get a <see cref="ModelChange.RolledBackTransaction"/>
    /// Changed event.  The values passed in the <see cref="ModelChangedEventArgs"/>
    /// may all be null, however.
    /// </para>
    /// </remarks>
    protected virtual bool EndTransaction(bool commit, String tname) {
      if (this.IsUndoingRedoing) return false;
      bool toplevel = (this.TransactionLevel == 1);
      int start = 0;
      // decrement the transaction level, but not below zero
      if (this.TransactionLevel > 0) {
        this.TransactionLevel--;
        int numnames = this.NestedTransactionNames.Count;
        if (numnames > 0) {
          if (tname == null) tname = this.NestedTransactionNames[0];
          this.NestedTransactionNames.RemoveAt(numnames-1);
        }
        int numstarts = this.NestedTransactionStarts.Count;
        if (numstarts > 0) {
          start = this.NestedTransactionStarts[numstarts-1];
          this.NestedTransactionStarts.RemoveAt(numstarts-1);
        }
      }
      if (tname == null) tname = "";
      CompoundEdit current = this.CurrentEdit;
      //Diagram.Debug(this.TransactionLevel, (commit ? "commit: " : "rollback: ") + (tname != "" ? tname : "(unknown)") + " " +
      //  (current != null ? current.Edits.Count.ToString() : "(no CEdit)") + " " + this.CompoundEdits.Count.ToString());
      if (toplevel) {
        if (commit) {
          // finish the current edit
          CompoundEdit cedit = CommitCompoundEdit(current);
          if (cedit != null) {
            cedit.IsComplete = true;
            cedit.Name = tname;
            // throw away any compound edits following the current index
            for (int i = this.CompoundEdits.Count-1; i > this.UndoEditIndex; i--) {
              this.CompoundEdits[i].Clear();
              this.CompoundEdits.RemoveAt(i);
            }
            // if there is a limit, just throw away the oldest edit
            int max = this.MaximumEditCount;
            if (max == 0) max = 1;
            if (max > 0) {
              if (this.CompoundEdits.Count >= max) {
                this.CompoundEdits[0].Clear();
                this.CompoundEdits.RemoveAt(0);
                this.UndoEditIndex--;
              }
            }
            // add to CompoundEdits list
            this.CompoundEdits.Add(cedit);
            this.UndoEditIndex++;
          }
          // notify all models
          foreach (IDiagramModel model in this.ModelsList) {
            RaiseChanged(model, ModelChange.CommittedTransaction, tname, cedit, null);
          }
        } else {  // !commit
          // rollback the current compound edit by undoing all of its edits
          if (current != null) {
            try {
              current.IsComplete = true;
              this.IsUndoingRedoing = true;
              current.Undo();
            } finally {
              this.IsUndoingRedoing = false;
            }
          }
          foreach (IDiagramModel model in this.ModelsList) {
            RaiseChanged(model, ModelChange.RolledBackTransaction, tname, current, null);
          }
          // now we can throw away all those undone edits
          if (current != null) current.Clear();
        }
        this.CurrentEdit = null;
        return true;
      } else {  // !toplevel
        // rollback the current edit by undoing all of its edits, but not of parent edits
        if (!commit && current != null) {
          current.RollbackTo(start);
        }
        return false;
      }
    }

    /// <summary>
    /// This method is called by <see cref="EndTransaction"/> when committing a
    /// compound edit. 
    /// </summary>
    /// <param name="cedit">
    /// the <see cref="CurrentEdit"/>;
    /// this may be null if there had been no changes at commit time
    /// </param>
    /// <returns>By default, the unmodified <paramref name="cedit"/>.</returns>
    /// <remarks>
    /// You may wish to override this method in order to perform optimizations,
    /// such as removing duplicate or unnecessary <see cref="ModelChangedEventArgs"/>s.
    /// </remarks>
    protected virtual CompoundEdit CommitCompoundEdit(CompoundEdit cedit) {
      return cedit;
    }


    /// <summary>
    /// Gets the current transaction level.
    /// </summary>
    /// <value>
    /// This value is zero when no transaction is in progress.
    /// The initial value is zero.
    /// <see cref="StartTransaction"/> will increment this value;
    /// <see cref="EndTransaction"/> will decrement it.
    /// When this value is greater than zero, <see cref="CanUndo"/>
    /// and <see cref="CanRedo"/> will be false, because
    /// additional logically related model change events may occur.
    /// </value>
    public int TransactionLevel { get; protected set; }
	
    /// <summary>
    ///  Gets or sets whether this undo manager will output warnings to Trace listeners
    ///  when model changes occur outside of a transaction.
    /// </summary>
    /// <value>
    /// This defaults to false.
    /// </value>
    public bool ChecksTransactionLevel { get; set; }

    /// <summary>
    /// Gets a stack of ongoing transaction names.
    /// </summary>
    /// <remarks>
    /// The outermost transaction name will be the first item in the list.
    /// The last one will be the name of the most recent (nested) call to <see cref="StartTransaction"/>.
    /// </remarks>
    public IList<String> NestedTransactionNames { get; private set; }

    /// <summary>
    /// Gets the current transaction name given by <see cref="StartTransaction"/>.
    /// </summary>
    /// <value>
    /// If no transaction is ongoing, this will be an empty string.
    /// </value>
    public String CurrentTransactionName {
      get {
        if (this.NestedTransactionNames.Count > 0)
          return this.NestedTransactionNames[this.NestedTransactionNames.Count-1];
        else
          return "";
      }
    }

    private List<int> NestedTransactionStarts { get; set; }

    private void RaiseChanged(IDiagramModel model, ModelChange change, Object data, Object oldval, Object newval) {
      ModelChangedEventArgs e = new ModelChangedEventArgs();
      e.Model = model;
      e.Change = change;
      e.Data = data;
      e.OldValue = oldval;
      e.NewValue = newval;
      model.RaiseChanged(e);
    }


    //internal ModelChangedEventArgs FindPreviousChange(ModelChangedEventArgs edit) {
    //  ModelChangedEventArgs found = null;
    //  if (this.CurrentEdit != null) found = this.CurrentEdit.FindPreviousChange(edit);
    //  if (found != null) return found;
    //  for (int i = this.CompoundEdits.Count-1; i >= 0; i--) {
    //    CompoundEdit ce = this.CompoundEdits[i] as CompoundEdit;
    //    if (ce == null) continue;
    //    found = ce.FindPreviousChange(edit);
    //    if (found != null) return found;
    //  }
    //  return null;
    //}

    //internal void ReplaceReference(Object oldref, Object newref) {
    //  foreach (IUndoableEdit edit in this.CompoundEdits) {
    //    CompoundEdit ce = edit as CompoundEdit;
    //    if (ce == null) continue;
    //    ce.ReplaceReference(oldref, newref);
    //  }
    //}

    internal void CoalesceLastTransaction(String type) {
      int num = this.CompoundEdits.Count;
      if (num < 2) return;
      CompoundEdit ult = this.CompoundEdits[num-1] as CompoundEdit;
      if (ult == null || ult.Name != type) return;
      CompoundEdit pen = this.CompoundEdits[num-2] as CompoundEdit;
      if (pen == null && pen != ult) return;
      //Diagram.Debug(" CoalesceLastTransaction " + ult.ToString() + " into " + pen.ToString());
      foreach (IUndoableEdit edit in ult.Edits) pen.Edits.Add(edit);
      this.CompoundEdits.RemoveAt(num-1);
      this.UndoEditIndex--;
    }


    /// <summary>
    /// Given an <see cref="IUndoableEdit"/> return an edited object
    /// that represents what was modified.
    /// </summary>
    /// <param name="edit">
    /// an <see cref="IUndoableEdit"/>,
    /// usually either a <see cref="ModelChangedEventArgs"/> or a <see cref="CompoundEdit"/>
    /// </param>
    /// <returns>
    /// typically a <see cref="Node"/> or a <see cref="Link"/>,
    /// but this may be null if there is no such object,
    /// perhaps because a model property was modified,
    /// or because there were no real edits in the argument <paramref name="edit"/>.
    /// </returns>
    public virtual Object FindPrimaryObject(IUndoableEdit edit) {
      ModelChangedEventArgs ea = edit as ModelChangedEventArgs;
      if (ea != null) return ea.Data;
      CompoundEdit ce = edit as CompoundEdit;
      if (ce != null) {
        foreach (IUndoableEdit e in ce.Edits) {
          Object data = FindPrimaryObject(e);
          if (data != null) return data;
        }
      }
      return null;
    }


    /// <summary>
    /// This class is used to hold a list of <see cref="ModelChangedEventArgs"/> that
    /// should be undone or redone all together because it represents the side-effects
    /// of a single logical operation, including user-driven events.
    /// </summary>
    public sealed class CompoundEdit : IUndoableEdit {  // nested class
      /// <summary>
      /// Construct an empty list of edits.
      /// </summary>
      public CompoundEdit() {
        this.Edits = new List<IUndoableEdit>();
      }

      /// <summary>
      /// For debugging.
      /// </summary>
      /// <returns></returns>
      public override string ToString() {
        String s = "CompoundEdit: " + (this.Name != null ? this.Name : "(noname)") + " " +
          this.Edits.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) + " edits" +
          (this.IsComplete ? "" : ", incomplete");
        //foreach (IUndoableEdit e in this.Edits) {
        //  s += "\n  " + e.ToString();
        //}
        return s;
      }

      /// <summary>
      /// Clear all of the <see cref="IUndoableEdit"/>s and forget all references to them.
      /// </summary>
      public void Clear() {
        for (int i = this.Edits.Count-1; i >= 0; i--) {
          IUndoableEdit edit = this.Edits[i];
          if (edit != null) edit.Clear();
        }
        this.Edits.Clear();
      }

      /// <summary>
      /// This predicate returns true if you can call <see cref="Undo"/>--
      /// namely when <see cref="IsComplete"/> is true.
      /// </summary>
      /// <returns></returns>
      public bool CanUndo() {
        return this.IsComplete;
      }

      /// <summary>
      /// Undo all of the <see cref="IUndoableEdit"/>s, in reverse order.
      /// </summary>
      public void Undo() {
        if (CanUndo()) {
          for (int i = this.Edits.Count-1; i >= 0; i--) {
            IUndoableEdit edit = this.Edits[i];
            if (edit != null) edit.Undo();
          }
        }
      }

      /// <summary>
      /// This predicate returns true if you can call <see cref="Redo"/>--
      /// namely when <see cref="IsComplete"/> is true.
      /// </summary>
      /// <returns></returns>
      public bool CanRedo() {
        return this.IsComplete;
      }

      /// <summary>
      /// Redo all of the <see cref="IUndoableEdit"/>s, in forwards order.
      /// </summary>
      public void Redo() {
        if (CanRedo()) {
          for (int i = 0; i <= this.Edits.Count-1; i++) {
            IUndoableEdit edit = this.Edits[i];
            if (edit != null) edit.Redo();
          }
        }
      }


      /// <summary>
      /// Gets a list of all the <see cref="IUndoableEdit"/>s in this compound edit.
      /// </summary>
      /// <value>A <c>List&lt;T&gt;</c> of <see cref="IUndoableEdit"/>s</value>
      /// <remarks>
      /// Each item is normally an instance of <see cref="ModelChangedEventArgs"/>.
      /// However, you may add your own <see cref="IUndoableEdit"/> objects.
      /// </remarks>
      //[TypeConverter(typeof(ExpandableObjectConverter))]
      public IList<IUndoableEdit> Edits { get; private set; }

      // partial undo, to implement rollback for nested RollbackTransaction() calls
      internal /*?? public */ void RollbackTo(int start) {
        for (int i = this.Edits.Count-1; i >= start; i--) {
          IUndoableEdit edit = this.Edits[i];
          if (edit != null) edit.Undo();
          this.Edits.RemoveAt(i);
        }
      }

      /// <summary>
      /// Gets or sets whether we can add more undoable edits to this compound edit.
      /// </summary>
      /// <value>
      /// This is initially false.  It can only be set to true,
      /// which is what <see cref="UndoManager.EndTransaction"/> does.
      /// </value>
      public bool IsComplete { get; set; }

      /// <summary>
      /// Gets or sets a name for this group of edits.
      /// </summary>
      /// <value>
      /// The default value is null.
      /// This is set by <see cref="EndTransaction"/> with the name passed to <see cref="CommitTransaction"/>.
      /// </value>
      public String Name { get; set; }


      //internal ModelChangedEventArgs FindPreviousChange(ModelChangedEventArgs edit) {
      //  for (int i = this.Edits.Count-1; i >= 0; i--) {
      //    ModelChangedEventArgs e = this.Edits[i] as ModelChangedEventArgs;
      //    if (e != null &&
      //        e.Change == edit.Change &&
      //        (e.Change != ModelChange.Property || e.PropertyName == edit.PropertyName) &&
      //        e.Model == edit.Model &&
      //        e.Data == edit.Data) {
      //      return e;
      //    }
      //  }
      //  return null;
      //}

      //internal void ReplaceReference(Object oldref, Object newref) {
      //  foreach (IUndoableEdit edit in this.Edits) {
      //    ModelChangedEventArgs e = edit as ModelChangedEventArgs;
      //    if (e == null) continue;
      //    if (e.Data == oldref) e.Data = newref;
      //  }
      //}

    }  // end of nested CompoundEdit class
  
  }  // end of UndoManager class
}
