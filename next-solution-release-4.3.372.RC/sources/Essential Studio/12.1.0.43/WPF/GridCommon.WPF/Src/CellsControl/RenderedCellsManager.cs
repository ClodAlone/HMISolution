#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Manages the DrawingVisuals for visible cells rendered to the 
    /// grid canvas and provides routines for invalidating individual
    /// cells and move rendered cells after insert or remove rows or
    /// columns operations.
    /// </summary>
    public class RenderedCellsManager
    {
        IList<Visual> childFrames;

        #region Ctor
        internal RenderedCellsManager(IList<Visual> childFrames)
        {
            this.childFrames = childFrames;
        }
        #endregion

        #region #region Prepare and Conclude RenderCells
        internal void PrepareRenderCells()
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.PrepareRenderCells"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null && !frame.IsPrepareRenderCellIntialized)
                {
                    frame.RenderedCells.PrepareRenderCells();
                    frame.IsPrepareRenderCellIntialized = true;
                }
            }
#if MEASURETIME
            }
#endif
        }

        internal void ConcludeRenderCells()
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.ConcludeRenderCells"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    frame.RenderedCells.ConcludeRenderCells();
                    frame.IsPrepareRenderCellIntialized = false;
                }
            }
#if MEASURETIME
            }
#endif
        }
        #endregion

        #region Invalidate

        /// <summary>
        /// Invalidates the specified cell row column index. This will
        /// force the cells Render method to be called again the
        /// next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void Invalidate(RowColumnIndex cellRowColumnIndex)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.Invalidate(RowColumnIndex)"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                    frame.RenderedCells.Invalidate(cellRowColumnIndex);
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Invalidates the specified cell span. This will
        /// force the cells Render method to be called again the
        /// next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons.
        /// </summary>
        /// <param name="cellSpan">The cell span.</param>
        public void Invalidate(CellSpanInfoBase cellSpan)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.Invalidate(CellSpanInfoBase)"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                    frame.RenderedCells.Invalidate(cellSpan);
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Invalidates all cells. This will
        /// force the cells Render method to be called again the
        /// next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons.
        /// </summary>
        public void Invalidate()
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.Invalidate()"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                    frame.RenderedCells.Invalidate();
            }
#if MEASURETIME
            }
#endif
        }
        #endregion

        #region Insert and Remove Rows

        /// <summary>
        /// Updates the cache when rows were inserted. Only for cells that
        /// belong to rows that were inserted the cell renderer's Render
        /// method will be called next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons. 
        /// </summary>
        /// <param name="insertAtRowIndex">Index of the first row to insert.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State for moving cells. Use this 
        /// when moving rows; otherwise specify null.</param>
        public void InsertRows(int insertAtRowIndex, int count, RenderedCellsMoveState moveCellsState)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.InsertOrRemove"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    CellDrawingVisualsDictionary moveVisuals = null;
                    if (moveCellsState != null)
                        moveVisuals = moveCellsState.ChildFrameVisuals[frame];
                    frame.RenderedCells.InsertRows(insertAtRowIndex, count, moveVisuals);
                }
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Updates the cache when rows were removed. Only for cells that
        /// belong to rows that become scrolled into view the cell renderer's Render
        /// method will be called next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons. 
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the first row to remove.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State for moving cells. Use this 
        /// when moving rows; otherwise specify null.</param>
        public void RemoveRows(int removeAtRowIndex, int count, RenderedCellsMoveState moveCellsState)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.InsertOrRemove"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    CellDrawingVisualsDictionary moveVisuals = null;
                    if (moveCellsState != null)
                    {
                        moveVisuals = new CellDrawingVisualsDictionary();
                        moveCellsState.ChildFrameVisuals[frame] = moveVisuals;
                    }
                    frame.RenderedCells.RemoveRows(removeAtRowIndex, count, moveVisuals);
                }
            }
#if MEASURETIME
            }
#endif
        }
        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Updates the cache when columns were inserted. Only for cells that
        /// belong to columns that were inserted the cell renderer's Render
        /// method will be called next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons. 
        /// </summary>
        /// <param name="insertAtColumnIndex">Index of the first column to insert.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State for moving cells. Use this 
        /// when moving columns; otherwise specify null.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, RenderedCellsMoveState moveCellsState)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.InsertOrRemove"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    CellDrawingVisualsDictionary moveVisuals = null;
                    if (moveCellsState != null)
                        moveVisuals = moveCellsState.ChildFrameVisuals[frame];
                    frame.RenderedCells.InsertColumns(insertAtColumnIndex, count, moveVisuals);
                }
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Updates the cache when columns were removed. Only for cells that
        /// belong to columns that become scrolled into view the cell renderer's Render
        /// method will be called next time you call InvalidateVisual on the 
        /// VirtualizingCellsControl or if the control is forced
        /// to render for other reasons. 
        /// </summary>
        /// <param name="removeAtColumnIndex">Index of the first column to remove.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State for moving cells. Use this 
        /// when moving columns; otherwise specify null.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, RenderedCellsMoveState moveCellsState)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.InsertOrRemove"))
            {
#endif
            foreach (Visual visual in childFrames)
            {
                VirtualizingCellsControlChildFrame frame = visual as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    CellDrawingVisualsDictionary moveVisuals = null;
                    if (moveCellsState != null)
                    {
                        moveVisuals = new CellDrawingVisualsDictionary();
                        moveCellsState.ChildFrameVisuals[frame] = moveVisuals;
                    }
                    frame.RenderedCells.RemoveColumns(removeAtColumnIndex, count, moveVisuals);
                }
            }
#if MEASURETIME
            }
#endif
        }

        #endregion

        public void TryGetVisual(RowColumnIndex rowColIndex, out DrawingVisual visual)
        {
            visual = null;
            foreach (var visualFrame in this.childFrames)
            {
                var frame = visualFrame as VirtualizingCellsControlChildFrame;
                if (frame != null)
                {
                    frame.RenderedCells.aliveCellDrawingVisuals.TryGetValue(rowColIndex, out visual);
                    if (visual != null)
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// An object which holds state when moving rows or columns for
    /// <see cref="RenderedCellsManager"/> InsertRows, InsertColumns,
    /// RemoveRows and RemoveColumns methods. It will be initialized
    /// by the Remove operation and later applied by the subsequent
    /// Insert operation.
    /// </summary>
    public class RenderedCellsMoveState
    {
        Dictionary<VirtualizingCellsControlChildFrame, CellDrawingVisualsDictionary> childFrameVisuals = new Dictionary<VirtualizingCellsControlChildFrame, CellDrawingVisualsDictionary>();

        internal Dictionary<VirtualizingCellsControlChildFrame, CellDrawingVisualsDictionary> ChildFrameVisuals
        {
            get { return childFrameVisuals; }
        }

        internal CellDrawingVisualsDictionary this[VirtualizingCellsControlChildFrame frame]
        {
            get
            {
                return childFrameVisuals[frame];
            }
            set
            {
                childFrameVisuals[frame] = value;
            }
        }
    }

    internal class InternalChildFrameRenderedCellsManager : IRowColumnIndexValueDictionaryCallbacks<DrawingVisual>
    {
        IList<Visual> visuals; // VisualContainer.Children
        internal CellDrawingVisualsDictionary aliveCellDrawingVisuals;
        internal CellDrawingVisualsDictionary unloadCellDrawingVisuals;

        #region Ctor
        public InternalChildFrameRenderedCellsManager(IList<Visual> visuals)
        {
            this.visuals = visuals;
            aliveCellDrawingVisuals = new CellDrawingVisualsDictionary(this);
        }
        #endregion

        #region Prepare and Conclude RenderCells
        public void PrepareRenderCells()
        {
            unloadCellDrawingVisuals = aliveCellDrawingVisuals;
            aliveCellDrawingVisuals = new CellDrawingVisualsDictionary(this);
        }

        public void ConcludeRenderCells()
        {
            unloadCellDrawingVisuals.RemoveAll();
            unloadCellDrawingVisuals = null;
        }
        #endregion

        #region DrawingVisual attached properties

        public static void SetDrawingVisualContext(DrawingVisual dv, RenderCellArgs rca)
        {
            VirtualizingCellsControl.SetCellsControl(dv, rca.CellsControl);

            // SH 4/6/10: 
            // Note: Because NotifyMovedRow is set false to improve performance of insert
            // and remove operations I need to comment out call to VirtualizingCellsControl.SetCellRowColumnIndex.
            // It is not safe anymore that VirtualizingCellsControl.GetCellRowColumnIndex
            // will return correct row index. I did however find no call to VirtualizingCellsControl.GetCellRowColumnIndex
            // for drawing visuals in existing code base so this should be safe to remove this property
            // from drawing visuals.
            //VirtualizingCellsControl.SetCellRowColumnIndex(dv, rca.CellRowColumnIndex);

            VirtualizingCellsControl.SetRenderCellInfo(dv, rca.CellInfo);
            VirtualizingCellsControl.SetCellRenderer(dv, rca.CellsControl.GetCellRenderer(rca.CellInfo));
        }

        public static void SetVisualBounds(Visual el, Rect rect)
        {
            VisualContainer.SetRenderBounds(el, rect);
        }

        public static Rect GetVisualBounds(Visual el)
        {
            return VisualContainer.GetRenderBounds(el);
        }

        #endregion

        #region TransformOrCreateDrawingVisual

        public bool TransformOrCreateDrawingVisual(RenderCellArgs rca, out DrawingVisual dv)
        {
            RowColumnIndex cell = new RowColumnIndex(rca.VisibleRow.LineIndex, rca.VisibleColumn.LineIndex);
            bool isNewDrawingVisual;
            bool needRender;
            if (unloadCellDrawingVisuals != null)
            {
                // Called during OnRender
                TransformHelper(unloadCellDrawingVisuals, rca, cell, out dv, out isNewDrawingVisual, out needRender);
                if (isNewDrawingVisual)
                {
#if MEASURETIME
                    //using (MeasureTime.Measure("RenderedCellsManager.children.Add"))
#endif
                    visuals.Add(dv);
                    aliveCellDrawingVisuals.Add(cell, dv);
                }
                else
                {
                    unloadCellDrawingVisuals.Clear(cell);
                    aliveCellDrawingVisuals.Add(cell, dv);
                }
            }
            else
            {
                // Called during UndoRenderCell
                TransformHelper(aliveCellDrawingVisuals, rca, cell, out dv, out isNewDrawingVisual, out needRender);
                if (isNewDrawingVisual)
                {
#if MEASURETIME
                    //using (MeasureTime.Measure("RenderedCellsManager.children.Add"))
#endif
                    visuals.Add(dv);
                    aliveCellDrawingVisuals.Add(cell, dv);
                }
            }

            return needRender;
        }

        private void TransformHelper(CellDrawingVisualsDictionary renderedCells, RenderCellArgs rca, RowColumnIndex cell, out DrawingVisual dv, out bool isNewDrawingVisual, out bool needRender)
        {
            if (renderedCells.TryGetValue(cell, out dv))
            {
                isNewDrawingVisual = false;
                Rect rect = GetVisualBounds(dv);
                bool b = false;
                if (rca.VisibleCoveredCellInfo != null && rca.VisibleCoveredCellInfo.forceClipping)
                {
                    CellSpanInfo span = rca.VisibleCoveredCellInfo.CellSpan;
                    b = span.ClipColumns || span.ClipRows;
                }

                if (rect == rca.OriginalCellRect && !b)
                {
                    if (dv.Transform != null)
                        dv.Transform = null;
                    needRender = false;
                }
                else
                {
                    if (rect.Size == rca.OriginalCellRect.Size && !b)
                    {
                        // relocate from original position, but check first if already relocated
                        // to the correct position.
                        TranslateTransform translate = new TranslateTransform(
                            rca.OriginalCellRect.X - rect.X,
                            rca.OriginalCellRect.Y - rect.Y);

                        TranslateTransform translate2 = dv.Transform as TranslateTransform;
                        if (translate2 == null || translate2.X != translate.X || translate2.Y != translate.Y)
                        {
                            translate.Freeze();
                            dv.Transform = translate;
                        }

                        needRender = false;
                    }
                    else
                    {
                        if (dv.Transform != null)
                            dv.Transform = null;
                        SetDrawingVisualContext(dv, rca);
                        SetVisualBounds(dv, rca.OriginalCellRect);
                        needRender = true;
                    }
                }
            }
            else
            {
                dv = new NoHitTestDrawingVisual();
                //? VirtualizingCellsControl.SetWantsMouseInput(dv, true);
                isNewDrawingVisual = true;
                SetDrawingVisualContext(dv, rca);
                needRender = true;
            }
        }

        #endregion

        #region Invalidate

        public bool Invalidate(RowColumnIndex cellRowColumnIndex)
        {
            DrawingVisual dv;
            if (aliveCellDrawingVisuals.TryGetValue(cellRowColumnIndex, out dv))
            {
                SetVisualBounds(dv, Rect.Empty);
                return true;
            }
            return false;
        }

        public void Invalidate(CellSpanInfoBase span)
        {
            foreach (KeyValuePair<int, IntegerValueCellsDictionary<DrawingVisual>> rowEntry in aliveCellDrawingVisuals.Rows)
            {
                if (span.ContainsRow(rowEntry.Key))
                {
                    foreach (KeyValuePair<int, DrawingVisual> cellEntry in rowEntry.Value.Cells)
                    {
                        if (span.ContainsColumn(cellEntry.Key))
                            SetVisualBounds(cellEntry.Value, Rect.Empty);
                    }
                }
            }
        }

        public void Invalidate()
        {
            //foreach (KeyValuePair<RowColumnIndex, DrawingVisual> cellEntry in aliveCellDrawingVisuals)
            //    SetVisualBounds(cellEntry.Value, Rect.Empty);

            for (int n = visuals.Count - 1; n >= 0; n--)
                visuals.RemoveAt(n);
            visuals.Clear();
            aliveCellDrawingVisuals.Clear();
        }

        #endregion

        #region Insert and Remove Rows
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
        public void InsertRows(int insertAtRowIndex, int count, CellDrawingVisualsDictionary moveVisuals)
        {
            this.aliveCellDrawingVisuals.InsertRows(insertAtRowIndex, count, moveVisuals);
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
        public void RemoveRows(int removeAtRowIndex, int count, CellDrawingVisualsDictionary moveVisuals)
        {
            aliveCellDrawingVisuals.RemoveRows(removeAtRowIndex, count, moveVisuals);
        }
        #endregion

        #region Insert and Remove Columns

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
        public void InsertColumns(int insertAtColumnIndex, int count, CellDrawingVisualsDictionary moveVisuals)
        {
            aliveCellDrawingVisuals.InsertColumns(insertAtColumnIndex, count, moveVisuals);
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
        public void RemoveColumns(int removeAtColumnIndex, int count, CellDrawingVisualsDictionary moveVisuals)
        {
            aliveCellDrawingVisuals.RemoveColumns(removeAtColumnIndex, count, moveVisuals);
        }
        #endregion

        #region IRowColumnIndexValueCallbacks<T> Members

        public void OnMovedCell(RowColumnIndex cellRowColumnIndex, DrawingVisual value)
        {
            // SH 4/6/10: OnMovedCell will not be called anymore since NotifyMovedRow is false.
            //VirtualizingCellsControl.SetCellRowColumnIndex(value, cellRowColumnIndex);
        }

        public void OnRemoveCell(RowColumnIndex cellRowColumnIndex, DrawingVisual value)
        {
#if MEASURETIME
            //using (MeasureTime.Measure("RenderedCellsManager.canvas.Children.Remove"))
#endif
            VisualContainer parent = VisualTreeHelper.GetParent(value) as VisualContainer;
            parent.Children.Remove(value);
        }

        #endregion

    }

    internal class CellDrawingVisualsDictionary : RowColumnIndexValueDictionary<DrawingVisual>
    {
        public CellDrawingVisualsDictionary()
        {
            // SH 4/6/10: 
            // Set NotifyMovedRow  to speed
            // up insert and remove operations for drawing visuals. Only visuals that were removed
            // need to be looped through then and OnRemoveCell be called to give parent host
            // a chance to remove visual from Visuals collection.
            NotifyMovedRow = false;

            // Note: Because visuals are not notified about position change it is also necessary
            // to comment out call to VirtualizingCellsControl.SetCellRowColumnIndex
            // in InternalChildFrameRenderedCellsManager.SetDrawingVisualContext since
            // it is not safe anymore that VirtualizingCellsControl.GetCellRowColumnIndex
            // will return correct row index.
        }

        public CellDrawingVisualsDictionary(IRowColumnIndexValueDictionaryCallbacks<DrawingVisual> callback)
            : this()
        {
            SetCallback(callback);
        }
    }

}
