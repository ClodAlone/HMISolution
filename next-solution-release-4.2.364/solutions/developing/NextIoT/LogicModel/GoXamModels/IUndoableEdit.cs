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

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// This interface specifies how a document change (an edit) can be
  /// managed by the <see cref="UndoManager"/>.
  /// </summary>
  /// <seealso cref="ModelChangedEventArgs"/>
  /// <seealso cref="UndoManager.CompoundEdit"/>
  public interface IUndoableEdit {
    /// <summary>
    /// Forget about any state remembered in this edit.
    /// </summary>
    void Clear();

    /// <summary>
    /// Determine if this edit is ready to be and can be undone.
    /// </summary>
    /// <returns></returns>
    bool CanUndo();

    /// <summary>
    /// Restore the previous state of this edit.
    /// </summary>
    void Undo();

    /// <summary>
    /// Determine if this edit is ready to be and can be redone.
    /// </summary>
    /// <returns></returns>
    bool CanRedo();

    /// <summary>
    /// Restore the new state of this edit after having been undone.
    /// </summary>
    void Redo();
  }

}
