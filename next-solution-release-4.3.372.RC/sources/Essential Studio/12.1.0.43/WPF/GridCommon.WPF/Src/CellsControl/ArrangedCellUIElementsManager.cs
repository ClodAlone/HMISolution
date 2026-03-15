#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#if SILVERLIGHT
using VirtualizingCellsControlChildFrame = Syncfusion.Windows.Controls.Scroll.ScrollControlChildFrame;
#endif

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Provides routines for managing cells visuals (aka UIElement children)
    /// of rendered cells that have been associated with one or more UIElement
    /// visuals. An instance of this class can be accessed with the 
    /// <see cref="VirtualizingCellsControl.ArrangedCellUIElements"/> property
    /// of a <see cref="VirtualizingCellsControl"/>.
    /// </summary>
    public class ArrangedCellUIElementsManager
    {
        VirtualizingCellsControl cellsControl;

        // Virtualized UIElements
        internal CellUIElementsDictionary aliveCellUIElements;
        internal CellUIElementsDictionary unloadCellUIElements;        
      

        internal ArrangedCellUIElementsManager(VirtualizingCellsControl cellsControl)
        {
            this.cellsControl = cellsControl;
            aliveCellUIElements = new CellUIElementsDictionary(cellsControl);            
        }

        internal void PreArrangeCell(ArrangeCellArgs aca, VirtualizingCellsControlChildFrame canvas)
        {
            RowColumnIndex cellRowColumnIndex = aca.CellRowColumnIndex;

            // Check if cell was visible before, in such case prevent visuals from being
            // unloaded. unloadVisualsDictionary is snapshot of aliveVisuals before
            // ArrangeCellUIElements call.
            CellUIElements cellUIElements;
            if (unloadCellUIElements != null && unloadCellUIElements.TryGetValue(cellRowColumnIndex, out cellUIElements))
            {
#if DEBUG
                if (aliveCellUIElements.ContainsKey(cellRowColumnIndex))
                    throw new InvalidOperationException(cellRowColumnIndex.ToString() + "has duplicate CellUIElemets.");
                // if above exception occurs double-check for cells inside covered cells and
                // if cell index was not adjusted with AdjustCoveredCellRowColumnIndex.
#endif

                bool sameCanvas = false;

                if (Object.ReferenceEquals(cellUIElements.Renderer, cellsControl.GetCellRenderer(aca.CellInfo)))
                {
                    sameCanvas = true;
                    foreach (UIElement el in cellUIElements.UIElements)
                    {
                        // In WPF, we simply removed and reinserted the UIElement in the
                        // new canvas, but this causes glitches with Silverlight. It is better
                        // to simple create a new UIElement, initialize it. The old element
                        // gets hidden and recycled.
                        VirtualizingCellsControlChildFrame oldCanvas = VisualTreeHelper.GetParent(el) as VirtualizingCellsControlChildFrame;
                        sameCanvas &= Object.ReferenceEquals(canvas, oldCanvas);
                    }
                }

                if (!sameCanvas)
                {
                    cellUIElements.Renderer.UnloadUIElements(cellsControl, aca.CellRowColumnIndex, cellUIElements);
                }
                else
                {
                    // retrieve visual from snapshot.
                    aca.SetCellUIElements(cellUIElements);

                    if (aca.VisibleCoveredCellInfo != null && aca.VisibleCoveredCellInfo.forceClipping)
                    {
                        CellSpanInfo span = aca.VisibleCoveredCellInfo.CellSpan;
                        if (span.ClipColumns || span.ClipRows)
                        {
                            foreach (UIElement el in cellUIElements.UIElements)
                            {
                                Rect r2 = aca.VisibleCoveredCellInfo.ClippedBounds;
                                Rect r1 = aca.VisibleCoveredCellInfo.ExactBounds;
                                if (span.ClipColumns)
                                    r2.X -= r1.X;
                                if (span.ClipRows)
                                    r2.Y -= r1.Y;
                                RectangleGeometry g = new RectangleGeometry();
                                g.Rect = r2;
#if !SILVERLIGHT
                                g.Freeze();
#endif
                                el.Clip = g;
                            }
                        }
                    }
                    if (aca.VisibleoverlappingCellInfo != null && aca.VisibleoverlappingCellInfo.forceClipping)
                    {
                        CellSpanInfo span = aca.VisibleoverlappingCellInfo.CellSpan;
                        if (span.ClipColumns || span.ClipRows)
                        {
                            foreach (UIElement el in cellUIElements.UIElements)
                            {
                                Rect r2 = aca.VisibleoverlappingCellInfo.ClippedBounds;
                                Rect r1 = aca.VisibleoverlappingCellInfo.ExactBounds;
                                if (span.ClipColumns)
                                    r2.X -= r1.X;
                                if (span.ClipRows)
                                    r2.Y -= r1.Y;
                                RectangleGeometry g = new RectangleGeometry();
                                g.Rect = r2;
#if !SILVERLIGHT
                                g.Freeze();
#endif
                                el.Clip = g;
                            }
                        }
                    }
                    // keep visuals alive - don't remove them when ArrangeCellUIElements ends - call Clear instead of Remove.
                    unloadCellUIElements.Clear(cellRowColumnIndex);

                    // CellRenderer.ArrangeCell should only rearrange UIElements stored in CellUIElements container.
                    aca.ShouldCreateVisuals = false;
                    return;
                }
            }

            // CellRenderer.ArrangeCell must create new UIElements and register them in the cells 
            // CellUIElements container.
            aca.ShouldCreateVisuals = true;
        }

        internal void PostArrangeCell(ArrangeCellArgs aca)
        {
            RowColumnIndex cellRowColumnIndex = aca.CellRowColumnIndex;

            // remember CellUIElements with UIElements.
            if (aca.HasVisuals && !aca.ShouldReinitializeContent)
            {
#if DEBUG
                if (aliveCellUIElements.ContainsKey(cellRowColumnIndex))
                    throw new InvalidOperationException(cellRowColumnIndex.ToString() + "has duplicate CellUIElements.");
                else
#endif
                    aliveCellUIElements.Add(cellRowColumnIndex, aca.CellUIElements);               
            }
        }


        internal void RefreshDirtyCellUIElementsContent()
        {

            foreach (KeyValuePair<RowColumnIndex, CellUIElements> entry in aliveCellUIElements)
            {
                if (entry.Value.IsDirty)
                {
                    entry.Value.Renderer.RefreshCellUIElementsContent(cellsControl, entry.Value, entry.Key);
                    entry.Value.IsDirty = false;
                }
            }

        }

        /// <summary>
        /// Clears all cell visuals and unloads them.
        /// </summary>
        public void UnloadAll()
        {
            aliveCellUIElements.RemoveAll();
        }

        /// <summary>
        /// Clears the visuals for a single cell only and unloads them.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void Unload(RowColumnIndex cellRowColumnIndex)
        {
            aliveCellUIElements.Remove(cellRowColumnIndex);
        }

        /// <summary>
        /// Clears the visuals for a range of cells and unloads them.
        /// </summary>
        /// <param name="cellSpan">The range of cells.</param>
        public void Unload(CellSpanInfoBase cellSpan)
        {
            aliveCellUIElements.Remove(cellSpan);
        }

        /// <summary>
        /// Determines whether the specified cell has UIElement children and if
        /// the children are alive.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns>
        /// 	<c>true</c> if  the specified cell has UIElement children and if
        /// the children are alive; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(RowColumnIndex cellRowColumnIndex)
        {
            return aliveCellUIElements.ContainsKey(cellRowColumnIndex);
        }


        /// <summary>
        /// Gets the cell visuals for a cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns></returns>
        public CellUIElements GetCellUIElements(RowColumnIndex cellRowColumnIndex)
        {
            return aliveCellUIElements[cellRowColumnIndex];
        }

        /// <summary>
        /// Gets the cell visuals for a cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public CellUIElements GetCellUIElements(int rowIndex, int columnIndex)
        {
            return GetCellUIElements(new RowColumnIndex(rowIndex, columnIndex));
        }

        /// <summary>
        /// Marks the visuals for a cell to be reinitialized with
        /// a call to <see cref="VirtualizingCellsControl.EraseRenderedCell"/> next time
        /// OnRender is called.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void Invalidate(RowColumnIndex cellRowColumnIndex)
        {
            CellUIElements cellVisual;
            if (aliveCellUIElements.TryGetValue(cellRowColumnIndex, out cellVisual))
                cellVisual.IsDirty = true;
        }

        /// <summary>
        /// Marks the visuals for a range of cells to be reinitialized with
        /// a call to <see cref="VirtualizingCellsControl.EraseRenderedCell"/> next time
        /// OnRender is called.
        /// </summary>
        /// <param name="span">The range of cells.</param>
        public void Invalidate(CellSpanInfoBase span)
        {
            aliveCellUIElements.Iterate(span,
                new RowColumnIndexValueDictionary<CellUIElements>.RowColumnIndexValueDelegate(setDirtyCallback)
                );
        }

        void setDirtyCallback(RowColumnIndex cell, CellUIElements value)
        {
            value.IsDirty = true;
        }

        /// <summary>
        /// Update visuals cell row and column index when rows were inserted.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index.</param>
        /// <param name="count">The number of inserted rows.</param>
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        /// <summary>
        /// Update visuals cell row and column index when rows were inserted.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index.</param>
        /// <param name="count">The number of inserted rows.</param>
        /// <param name="moveVisuals">A container with saved state from a preceeding <see cref="RemoveRows"/> call when rows should be moved.</param>
        public void InsertRows(int insertAtRowIndex, int count, CellUIElementsDictionary moveVisuals)
        {
            aliveCellUIElements.InsertRows(insertAtRowIndex, count, moveVisuals);
        }

        /// <summary>
        /// Update visuals cell row and column index when rows were removed.
        /// </summary>
        /// <param name="removeAtRowIndex">The row index.</param>
        /// <param name="count">The number of removed rows.</param>
        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        /// <summary>
        /// Update visuals cell row and column index when rows were removed.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The number of removed rows.</param>
        /// <param name="moveVisuals">A container to save state for a subsequent 
        /// <see cref="InsertRows"/> call when rows should be moved. When null, 
        /// cell visuals that belong to the removed rows are unloaded. 
        /// Otherwise these visuals will be saved into this container.</param>
        public void RemoveRows(int removeAtRowIndex, int count, CellUIElementsDictionary moveVisuals)
        {
            aliveCellUIElements.RemoveRows(removeAtRowIndex, count, moveVisuals);
        }


        /// <summary>
        /// Update visuals cell column and column index when columns were inserted.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        /// <summary>
        /// Update visuals cell column and column index when columns were inserted.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        /// <param name="moveVisuals">A container with saved state from a preceeding <see cref="RemoveColumns"/> call when columns should be moved.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, CellUIElementsDictionary moveVisuals)
        {
            aliveCellUIElements.InsertColumns(insertAtColumnIndex, count, moveVisuals);
        }

        /// <summary>
        /// Update visuals cell column and column index when columns were removed.
        /// </summary>
        /// <param name="removeAtColumnIndex">The column index.</param>
        /// <param name="count">The number of removed columns.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Update visuals cell column and column index when columns were removed.
        /// </summary>
        /// <param name="removeAtColumnIndex">Index of the remove at column.</param>
        /// <param name="count">The number of removed columns.</param>
        /// <param name="moveVisuals">A container to save state for a subsequent 
        /// <see cref="InsertColumns"/> call when columns should be moved. When null, 
        /// cell visuals that belong to the removed columns are unloaded. 
        /// Otherwise these visuals will be saved into this container.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, CellUIElementsDictionary moveVisuals)
        {
            aliveCellUIElements.RemoveColumns(removeAtColumnIndex, count, moveVisuals);
        }

        internal void PrepareArrange()
        {
            // Create new UIElements for cells scrolled into view or unload UIElements for cells scrolled out of view.
            unloadCellUIElements = this.aliveCellUIElements;
            aliveCellUIElements = new CellUIElementsDictionary(cellsControl);           
        }

        internal void ConcludeArrange()
        {
            foreach (KeyValuePair<RowColumnIndex, CellUIElements> entry in unloadCellUIElements)
            {
                // Do not unload cell of canceled mouse operation.
                bool isFocused = false;
#if !SILVERLIGHT
                isFocused = cellsControl.MouseControllerDispatcher.IsMouseOperationOrigin(entry.Key);
#endif
                foreach (UIElement el in entry.Value.UIElements)
                {
                    isFocused |= VirtualizingCellsControl.GetHasFocusWithin(el);
                }

                if (!isFocused && entry.Value.AllowUnloadVisuals && entry.Value.Renderer.UnloadUIElementsWhenScrolledOutOfView)
                    entry.Value.Renderer.UnloadUIElements(cellsControl, entry.Key, entry.Value);
                else
                {
                    foreach (UIElement e in entry.Value.UIElements)
                    {
                        // using Arrange has benefits that capture state of a textbox does not get affected
                        // instead of e.Visibility = Visibility.Hidden; 
                        // Could also add check for !IsMouseOperationOrigin(entry.Key) here if .Visibility would
                        // be more efficient for other cells.
                        entry.Value.Renderer.Hide(e);
                    }
                    aliveCellUIElements.Add(entry.Key, entry.Value);
                }
            }

            unloadCellUIElements.Clear();
            unloadCellUIElements = null;
        }
    }
}
