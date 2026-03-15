#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
{
    public abstract class GraphicSelectionControllerBase : IGraphicSelectionController
    {
        private GraphicModel graphicModel;

        public GraphicSelectionControllerBase(GraphicModel graphicModel)
        {
            this.graphicModel = graphicModel;
        }

        public GraphicModel GraphicModel
        {
            get { return graphicModel; }
        }
        
        void IGraphicSelectionController.OnMouseDown(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnMouseDown(args, spanInfo);
        }

        void IGraphicSelectionController.OnMouseUp(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnMouseUp(args, spanInfo);
        }

        void IGraphicSelectionController.OnMouseLeave(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnMouseLeave(args, spanInfo);
        }

        void IGraphicSelectionController.OnMouseEnter(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnMouseEnter(args, spanInfo);
        }

        void IGraphicSelectionController.OnMouseMove(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnMouseMove(args, spanInfo);
        }

        void IGraphicSelectionController.OnKeyDown(KeyEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnKeyDown(args, spanInfo);
        }

        void IGraphicSelectionController.OnKeyUp(KeyEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnKeyUp(args, spanInfo);
        }

        void IGraphicSelectionController.OnGotFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnGotFocus(args, spanInfo);
        }

        void IGraphicSelectionController.OnLostFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            OnLostFocus(args, spanInfo);
        }

        public void SelectGraphicCell(GraphicCellSpanInfo spanInfo)
        {
            if (!graphicModel.SelectedGraphicCells.Contains(spanInfo))
            {
                var control = graphicModel[spanInfo.CellSpanIndex].GraphicCellControl;
                if (control != null)
                    control.IsSelected = true;
                graphicModel.SelectedGraphicCells.Add(spanInfo);
                graphicModel.CurrentGraphicCell = spanInfo;
            }
        }

        public void UnSelectGraphicCell(GraphicCellSpanInfo spanInfo)
        {
            if (graphicModel.SelectedGraphicCells.Contains(spanInfo))
            {
                var control = graphicModel[spanInfo.CellSpanIndex].GraphicCellControl;
                if (control != null)
                    control.IsSelected = false;
                graphicModel.SelectedGraphicCells.Remove(spanInfo);
                if (graphicModel.CurrentGraphicCell == spanInfo)
                    graphicModel.CurrentGraphicCell = null;
            }
        }

        public void SelectAllGraphicCells()
        {
            foreach (var span in GraphicModel.GraphicCells)
            {
                if (!graphicModel.SelectedGraphicCells.Contains(span))
                {
                    var control = GraphicModel[span.CellSpanIndex].GraphicCellControl;
#if !SILVERLIGHT
                    if (control != null)
                        control.IsSelected = true;
#endif
                    graphicModel.SelectedGraphicCells.Add(span);
                    graphicModel.CurrentGraphicCell = span;
                }
            }
        }

        public void ClearGraphicCellSelections()
        {
            if (GraphicModel.SelectedGraphicCells.Count > 0)
            {
                foreach (var span in GraphicModel.SelectedGraphicCells)
                {
                    GraphicModel[span.CellSpanIndex].GraphicCellControl.IsSelected = false;
                }
                GraphicModel.SelectedGraphicCells.Clear();
                GraphicModel.CurrentGraphicCell = null;
            }
        }

        protected abstract void OnMouseDown(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnMouseUp(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnMouseLeave(MouseEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnMouseEnter(MouseEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnMouseMove(MouseEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnKeyDown(KeyEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnKeyUp(KeyEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnGotFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo);
        
        protected abstract void OnLostFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo);

    }


    public class GraphicSelectionController : GraphicSelectionControllerBase
    {
        public GraphicSelectionController(GraphicModel graphicModel)
            : base(graphicModel)
        {

        }

        #region Mouse events
        protected override void OnMouseDown(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            GraphicCellControl graphicCellControl = this.GraphicModel[spanInfo.CellSpanIndex].GraphicCellControl;
            GridControlBase grid = GraphicModel.GridControl;
            bool isControlKeyPressed = Keyboard.Modifiers == ModifierKeys.Control;
            GraphicStyleInfo style = GraphicModel[spanInfo.CellSpanIndex];

            if (!style.Enabled)
                return;

            if (GraphicModel.SelectedGraphicCells.Count <= 0)
            {
                if (GraphicModel.RaiseCurrentGraphicCellActivating(GraphicModel.CurrentGraphicCell, null, spanInfo))
                {
                    grid.Model.Selections.Clear();
                    if (GraphicModel.GridControl.CurrentCell.IsEditing)
                        grid.CurrentCell.EndEdit();
                }
                else
                {
                    grid.Focus();
                    args.Handled = true;
                    return;
                }
                
            }
            else if (GraphicModel.CurrentGraphicCell != null)
            {
                var oldstyle = GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex];
                if (!GraphicModel.RaiseCurrentGraphicCellActivating(GraphicModel.CurrentGraphicCell, oldstyle, spanInfo))
                {
                    graphicCellControl = this.GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex].GraphicCellControl;
                    graphicCellControl.Focus();
                    args.Handled = true;
                    return;
                }
            }

            if (!isControlKeyPressed)
            {
                ClearGraphicCellSelections();
                GraphicModel.SelectedGraphicCells.Add(spanInfo);
#if !SILVERLIGHT
                graphicCellControl.IsSelected = true;
#endif
                GraphicModel.CurrentGraphicCell = spanInfo;
                GraphicModel.RaiseCurrentGraphicCellActivated(spanInfo, style);
                graphicCellControl.Focus();
            }
            else
            {
                graphicCellControl.Focus();
                args.Handled = true;
                if (GraphicModel.SelectedGraphicCells.Contains(spanInfo))
                {
                    GraphicModel.SelectedGraphicCells.Remove(spanInfo);
                    graphicCellControl.IsSelected = false;
                    if (GraphicModel.SelectedGraphicCells.Count > 0)
                    {
                        var span = GraphicModel.SelectedGraphicCells[0];
                        var styl = GraphicModel[span.CellSpanIndex];
                        if (GraphicModel.CurrentGraphicCell != null)
                            GraphicModel.RaiseCurrentGraphicCellDeactivated(GraphicModel.CurrentGraphicCell, GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex]);
                        GraphicModel.CurrentGraphicCell = span;
                        GraphicModel.RaiseCurrentGraphicCellActivated(span, styl);
                    }
                    else
                    {
                        if (GraphicModel.CurrentGraphicCell != null)
                            GraphicModel.RaiseCurrentGraphicCellDeactivated(GraphicModel.CurrentGraphicCell, GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex]);
                        GraphicModel.CurrentGraphicCell = null;
                        grid.Model.Selections.Add(GridRangeInfo.Cell(grid.CurrentCell.RowIndex, grid.CurrentCell.ColumnIndex));
                        grid.Focus();
                    }
                }
                else
                {
                    if (GraphicModel.CurrentGraphicCell != null)
                        GraphicModel.RaiseCurrentGraphicCellDeactivated(GraphicModel.CurrentGraphicCell, GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex]);
                    GraphicModel.CurrentGraphicCell = spanInfo;
                    GraphicModel.SelectedGraphicCells.Add(spanInfo);
#if !SILVERLIGHT
                    graphicCellControl.IsSelected = true;
#endif
                    GraphicModel.RaiseCurrentGraphicCellActivated(spanInfo, style);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }

        protected override void OnMouseEnter(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }

        protected override void OnMouseLeave(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }

        protected override void OnMouseMove(MouseEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }
        #endregion

        #region Keyboard events
        protected override void OnKeyDown(KeyEventArgs args, GraphicCellSpanInfo spanInfo)
        {
            var graphiccellControl = GraphicModel[spanInfo.CellSpanIndex].GraphicCellControl;
            GridControlBase grid = GraphicModel.GridControl;

            switch (args.Key)
            {

                case Key.Escape:
                    #region Escape
                    ClearGraphicCellSelections();
                    GraphicModel.CurrentGraphicCell = null;
                    GraphicModel.RaiseCurrentGraphicCellDeactivated(spanInfo, GraphicModel[spanInfo.CellSpanIndex]);
                    grid.Focus();
                    grid.Model.Selections.Add(GridRangeInfo.Cell(grid.CurrentCell.RowIndex, grid.CurrentCell.ColumnIndex));
                    break;
                    #endregion

                case Key.Tab:
                    #region Tab
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        GraphicStyleInfo style = null;
                        GraphicCellSpanInfo spaninfo = null;
                        int count = this.GraphicModel.SelectedGraphicCells.Count;
                        var cellspan = this.GraphicModel.SelectedGraphicCells[count - 1];
                        int index = this.GraphicModel.GraphicCells.IndexOf(cellspan);
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            do
                            {
                                int newindex = index > 0 ? index - 1 : this.GraphicModel.GraphicCells.Count - 1;
                                spaninfo = this.GraphicModel.GraphicCells[newindex];
                                style = this.GraphicModel[spaninfo.CellSpanIndex];
                            } while (!style.Enabled);
                        }
                        else
                        {
                            do
                            {
                                int newindex = index < this.GraphicModel.GraphicCells.Count - 1 ? index + 1 : 0;
                                spaninfo = this.GraphicModel.GraphicCells[newindex];
                                style = this.GraphicModel[spaninfo.CellSpanIndex];
                            } while (!style.Enabled);
                        }
                        if (!GraphicModel.RaiseCurrentGraphicCellActivating(GraphicModel.CurrentGraphicCell, GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex], spaninfo))
                            return;

                        ClearGraphicCellSelections();
                        grid.ScrollInView(new RowColumnIndex(spaninfo.RowIndex, spaninfo.ColumnIndex));
                        GraphicModel.SelectedGraphicCells.Add(spaninfo);
                        if (GraphicModel.CurrentGraphicCell != null)
                            GraphicModel.RaiseCurrentGraphicCellDeactivated(GraphicModel.CurrentGraphicCell, GraphicModel[GraphicModel.CurrentGraphicCell.CellSpanIndex]);
                        GraphicModel.CurrentGraphicCell = spaninfo;
                        GraphicModel.RaiseCurrentGraphicCellActivated(spaninfo, style);
                        if (style.GraphicCellControl != null)
                        {
#if !SILVERLIGHT
                            style.GraphicCellControl.IsSelected = true;
#endif
                            style.GraphicCellControl.Focus();
                        }
                    }
                    break;
                    #endregion

                case Key.Delete:
                    #region Delete
                        if (!GraphicModel.RaiseGraphicCellRemoving(GraphicModel.SelectedGraphicCells))
                            return;

                        foreach (var span in GraphicModel.SelectedGraphicCells)
                        {
                            int index = this.GraphicModel.GraphicCells.IndexOf(span);
                            var style = GraphicModel[span.CellSpanIndex];
                            var control = style.GraphicCellControl;
                            GraphicModel.GraphicCells.Remove(span);
                            GraphicModel.VolatileCellStyles.Clear(span.CellSpanIndex);
                            GraphicModel.Data.RemoveAt(index);
                            ScrollControlChildFrame canvas = VisualTreeHelper.GetParent(control) as ScrollControlChildFrame;
                            if (canvas != null)
                                canvas.Children.Remove(control);
                        }
                        GraphicModel.RaiseGraphicCellRemoved(GraphicModel.SelectedGraphicCells);
                        GraphicModel.CurrentGraphicCell = null;
                        var cellstyle = GraphicModel[GraphicModel.SelectedGraphicCells[0].CellSpanIndex];
                        GraphicModel.RaiseCurrentGraphicCellDeactivated(GraphicModel.SelectedGraphicCells[0], cellstyle);
                        GraphicModel.SelectedGraphicCells.Clear();
                        grid.Focus();
                        grid.Model.Selections.Add(GridRangeInfo.Cell(this.GraphicModel.GridControl.CurrentCell.RowIndex, this.GraphicModel.GridControl.CurrentCell.ColumnIndex));
                        grid.InvalidateCells();
                        grid.InvalidateVisual();
                        args.Handled = true;
                    break;
                    #endregion

                case Key.A:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        this.SelectAllGraphicCells();
                    break;

                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    args.Handled = true;
                    break;

                default:
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }
        #endregion

        protected override void OnGotFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }

        protected override void OnLostFocus(RoutedEventArgs args, GraphicCellSpanInfo spanInfo)
        {

        }

    }

}
