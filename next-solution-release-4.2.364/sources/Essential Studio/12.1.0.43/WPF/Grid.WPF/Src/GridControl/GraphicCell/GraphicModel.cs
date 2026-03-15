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
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using System.Xml.Serialization;
using System.ComponentModel;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.GridCommon;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicModel : Disposable, IGraphicVolatileCellStylesHost
    {
        GridControlBase grid = null;
        GraphicCellInfoCollection graphicCells;
        GraphicCellSpanInfoCollection<GraphicCellSpanInfo> selectedGraphicCells;
        GraphicVolatileCellStyles volatileCellStyles;
        GraphicCellRendererCollection graphicCellRenderers;
        internal ArrangedGraphicCellManager arrangedGraphicCellManager;
        internal IGraphicSelectionController SelectionController;
        IGraphicCellRenderer currentCellRenderers;
        GraphicCellData data = new GraphicCellData();
        GraphicCellSpanInfo currentgraphicCell = null;
        bool invalidateArrangeOnLoaded = false;
        
        public GraphicModel()
        {
            graphicCells = new GraphicCellInfoCollection();
            selectedGraphicCells = new GraphicCellInfoCollection();
            volatileCellStyles = new GraphicVolatileCellStyles(this);
            arrangedGraphicCellManager = new ArrangedGraphicCellManager();
            SelectionController = new GraphicSelectionController(this);
        }

        internal void SetGridControl(GridControlBase gridbase)
        {
            this.grid = gridbase;
            WireEvents();
        }

        public GridControlBase GridControl
        {
            get { return grid; }
        }

        public GraphicStyleInfo this[int index]
        {
            get
            {
                if (graphicCells != null)
                {
                    return volatileCellStyles[index];
                }
                return null;
            }
        }

        public GraphicStyleInfo this[int RowIndex, int ColumnIndex]
        {
            get
            {
                int index = graphicCells.FindIndex(RowIndex, ColumnIndex);
                if (index >= 0)
                {
                    return this[index];
                }
                return null;
            }
        }

        internal GraphicCellData Data
        {
            get { return data; }
        }

        internal GraphicVolatileCellStyles VolatileCellStyles
        {
            get { return volatileCellStyles; }
        }

        public GraphicCellInfoCollection GraphicCells
        {
            get
            {
                return graphicCells;
            }
        }

#if !SILVERLIGHT
        public GraphicCellSpanInfoCollection<GraphicCellSpanInfo> SelectedGraphicCells
#else
        internal GraphicCellSpanInfoCollection<GraphicCellSpanInfo> SelectedGraphicCells
#endif
        {
            get
            {
                return selectedGraphicCells;
            }
        }

#if !SILVERLIGHT
        public GraphicCellSpanInfo CurrentGraphicCell
        {
            get
            {
                return currentgraphicCell;
            }
            internal set
            {
                currentgraphicCell = value;
            }
        }
#else
        internal GraphicCellSpanInfo CurrentGraphicCell
        {
            get
            {
                return currentgraphicCell;
            }
            set
            {
                currentgraphicCell = value;
            }
        }
#endif
        

        protected virtual void WireEvents()
        {
            if (this.GridControl == null)
                return;
#if !SILVERLIGHT
            this.GridControl.ScrollControlPreviewMouseDown += new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlPreviewMouseUp += new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlPreviewMouseMove += new ScrollControlMouseEventHandler(OnScrollControlMouseMove);
            this.GridControl.ScrollControlMouseUp += new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlMouseDown += new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlMouseMove += new ScrollControlMouseEventHandler(OnScrollControlMouseMove);
#else
#endif
            this.GridControl.CellMouseDown += new GridCellMouseControllerEventHandler(GridControl_CellMouseDown);
            this.GridControl.Unloaded += new RoutedEventHandler(GridControl_Unloaded);
        }

        void GridControl_CellMouseDown(object sender, GridCellMouseControllerEventArgs args)
        {
            if (CurrentGraphicCell != null)
            {
                var spanInfo = CurrentGraphicCell;
                var style = this[spanInfo.CellSpanIndex];
                CurrentGraphicCell = null;
                this.RaiseCurrentGraphicCellDeactivated(spanInfo, style);
            }
            else if (selectedGraphicCells.Count > 0)
            {
                var spanInfo = SelectedGraphicCells[SelectedGraphicCells.Count - 1];
                var style = this[spanInfo.CellSpanIndex];
                this.RaiseCurrentGraphicCellDeactivated(spanInfo, style);
            }
                this.SelectionController.ClearGraphicCellSelections();
#if !SILVERLIGHT
                if (this.currentCellRenderers != null && this.currentCellRenderers.CurrentUIElement != null)
                {
#if SyncfusionFramework4_0
                    Keyboard.ClearFocus();
#else
                    var scope = FocusManager.GetFocusScope(this.CurrentCellRenderers.CurrentUIElement);
                    FocusManager.SetFocusedElement(scope, null);
#endif
                }
#endif
        }

        protected virtual void UnWireEvents()
        {
            if (this.GridControl == null)
                return;
#if !SILVERLIGHT
            this.GridControl.ScrollControlPreviewMouseDown -= new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlPreviewMouseUp -= new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlPreviewMouseMove -= new ScrollControlMouseEventHandler(OnScrollControlMouseMove);
            this.GridControl.ScrollControlMouseUp -= new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlMouseDown -= new ScrollControlMouseButtonEventHandler(OnSuspendScrollControlMouseUpDown);
            this.GridControl.ScrollControlMouseMove -= new ScrollControlMouseEventHandler(OnScrollControlMouseMove);
#else
#endif
            this.GridControl.CellMouseDown -= new GridCellMouseControllerEventHandler(GridControl_CellMouseDown);
            this.GridControl.Unloaded -= new RoutedEventHandler(GridControl_Unloaded);
        }

#if !SILVERLIGHT
        void OnScrollControlMouseMove(object sender, ScrollControlMouseEventArgs e)
        {
            if (GraphicCells == null || GraphicCells.Count <= 0)
                return;
            DependencyObject el = Mouse.DirectlyOver as DependencyObject;
            if (el != null && this.GridControl != null && el != this.GridControl)
            {
                var ctrl = (el as FrameworkElement).FindParentElementOfType<GraphicCellControl>();
                if (ctrl != null)
                    el = ctrl;
                if (GraphicCellHelper.GetHandleMouseInput(el, GridControl) == true)
                {
                    Mouse.OverrideCursor = null;
                    e.SkipListeners = true;
                    return;
                }
            }
        }

        void OnSuspendScrollControlMouseUpDown(object sender, ScrollControlMouseButtonEventArgs e)
        {
            if (GraphicCells == null || GraphicCells.Count <= 0)
                return;
            DependencyObject el = Mouse.DirectlyOver as DependencyObject;
            if (el != null && this.GridControl != null && el != this.GridControl)
            {
                var ctrl = (el as FrameworkElement).FindParentElementOfType<GraphicCellControl>();
                if (ctrl != null)
                    el = ctrl;
                if (GraphicCellHelper.GetHandleMouseInput(el, GridControl) == true)
                {
                    e.SkipListeners = true;
                    return;
                }
            }
            
        }
#else
#endif

        void GridControl_Unloaded(object sender, RoutedEventArgs e)
        {
            invalidateArrangeOnLoaded = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                UnWireEvents();
                if (this.GraphicCells != null)
                {
                    this.GraphicCells.Clear();
                    this.graphicCells = null;
                }
                if (this.data != null)
                {
                    this.data.Clear();
                    this.data = null;
                }
                if (this.volatileCellStyles != null)
                {
                    this.volatileCellStyles.Dispose();
                    this.volatileCellStyles = null;
                }
                if (this.arrangedGraphicCellManager != null)
                {
                    this.arrangedGraphicCellManager.Dispose();
                    this.arrangedGraphicCellManager = null;
                }
                if (this.cellModels != null)
                {
                    this.cellModels.Dispose();
                    this.cellModels = null;
                }
                if (this.graphicCellRenderers != null)
                {
                    this.graphicCellRenderers.Dispose();
                    this.graphicCellRenderers = null;
                }
                if (this.GridControl != null && this.GridControl.GraphicFrame != null)
                {
                    foreach (ScrollControlChildFrame canvas in this.GridControl.GraphicFrame.Children)
                    {
                        canvas.Children.Clear();
                    }
                }
                if (this.GridControl != null)
                    this.grid = null;
            }
        }

        #region Events

#if !SILVERLIGHT
        public event CurrrentGraphicCellActivatingEventHandler CurrentGraphicCellActivating;
#else
        internal event CurrrentGraphicCellActivatingEventHandler CurrentGraphicCellActivating;
#endif

        protected virtual void OnCurrrentGraphicCellActivating(CurrrentGraphicCellActivatingEventArgs e)
        {
            if (CurrentGraphicCellActivating != null)
                CurrentGraphicCellActivating(this, e);
        }

        internal bool RaiseCurrentGraphicCellActivating(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style, GraphicCellSpanInfo newSpan)
        {
            CurrrentGraphicCellActivatingEventArgs e = new CurrrentGraphicCellActivatingEventArgs(spanInfo, style, newSpan);
            OnCurrrentGraphicCellActivating(e);
            return !e.Cancel;
        }

#if !SILVERLIGHT
        public event CurrrentGraphicCellActivatedEventHandler CurrentGraphicCellActivated;
#else
        internal event CurrrentGraphicCellActivatedEventHandler CurrentGraphicCellActivated;
#endif

        protected virtual void OnCurrrentGraphicCellActivated(CurrrentGraphicCellActivatedEventArgs e)
        {
            if (CurrentGraphicCellActivated != null)
                CurrentGraphicCellActivated(this, e);
        }

        internal void RaiseCurrentGraphicCellActivated(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style)
        {
            CurrrentGraphicCellActivatedEventArgs e = new CurrrentGraphicCellActivatedEventArgs(spanInfo, style);
            OnCurrrentGraphicCellActivated(e);
        }

#if !SILVERLIGHT
        public event CurrrentGraphicCellDeactivatedEventHandler CurrentGraphicCellDeactivated;
#else
        internal event CurrrentGraphicCellDeactivatedEventHandler CurrentGraphicCellDeactivated;
#endif
        protected virtual void OnCurrrentGraphicCellDeactivated(CurrrentGraphicCellDeactivatedEventArgs e)
        {
            if (CurrentGraphicCellDeactivated != null)
                CurrentGraphicCellDeactivated(this, e);
        }

        internal void RaiseCurrentGraphicCellDeactivated(GraphicCellSpanInfo spanInfo, GraphicStyleInfo style)
        {
            CurrrentGraphicCellDeactivatedEventArgs e = new CurrrentGraphicCellDeactivatedEventArgs(spanInfo, style);
            OnCurrrentGraphicCellDeactivated(e);
        }

#if !SILVERLIGHT
        public event GraphicCellMovingEventHandler GraphicCellMoving;
#else
        internal event GraphicCellMovingEventHandler GraphicCellMoving;
#endif

        protected virtual void OnGraphicCellMoving(GraphicCellMovingEventArgs e)
        {
            if (GraphicCellMoving != null)
                GraphicCellMoving(this, e);
        }

        internal bool RaiseGraphicCellMoving(GraphicCellSpanInfo spanInfo, string name)
        {
            GraphicCellMovingEventArgs e = new GraphicCellMovingEventArgs(spanInfo, name);
            OnGraphicCellMoving(e);
            return !e.Cancel;
        }

#if !SILVERLIGHT
        public event GraphicCellMovedEventHandler GraphicCellMoved;
#else
        internal event GraphicCellMovedEventHandler GraphicCellMoved;
#endif

        protected virtual void OnGraphicCellMoved(GraphicCellMovedEventArgs e)
        {
            if (GraphicCellMoved != null)
                GraphicCellMoved(this, e);
        }

        internal void RaiseGraphicCellMoved(GraphicCellSpanInfo spanInfo, string name)
        {
            GraphicCellMovedEventArgs e = new GraphicCellMovedEventArgs(spanInfo, name);
            OnGraphicCellMoved(e);
        }

#if !SILVERLIGHT
        public event GraphicCellResizingEventHandler GraphicCellResizing;
#else
        internal event GraphicCellResizingEventHandler GraphicCellResizing;
#endif

        protected virtual void OnGraphicCellResizing(GraphicCellResizingEventArgs e)
        {
            if (GraphicCellResizing != null)
                GraphicCellResizing(this, e);
        }

        internal bool RaiseGraphicCellResizing(GraphicCellSpanInfo spanInfo, string name)
        {
            GraphicCellResizingEventArgs e = new GraphicCellResizingEventArgs(spanInfo, name);
            OnGraphicCellResizing(e);
            return !e.Cancel;
        }

#if !SILVERLIGHT
        public event GraphicCellResizedEventHandler GraphicCellResized;
#else
        internal event GraphicCellResizedEventHandler GraphicCellResized;
#endif

        protected virtual void OnGraphicCellResized(GraphicCellResizedEventArgs e)
        {
            if (GraphicCellResized != null)
                GraphicCellResized(this, e);
        }

        internal void RaiseGraphicCellResized(GraphicCellSpanInfo spanInfo, string name)
        {
            GraphicCellResizedEventArgs e = new GraphicCellResizedEventArgs(spanInfo, name);
            OnGraphicCellResized(e);
        }

#if !SILVERLIGHT
        public event GraphicCellRemovingEventHandler GraphicCellRemoving;
#else
        internal event GraphicCellRemovingEventHandler GraphicCellRemoving;
#endif

        protected virtual void OnGraphicCellRemoving(GraphicCellRemovingEventArgs e)
        {
            if (GraphicCellRemoving != null)
                GraphicCellRemoving(this, e);
        }

        internal bool RaiseGraphicCellRemoving(GraphicCellSpanInfoCollection<GraphicCellSpanInfo> cells)
        {
            GraphicCellRemovingEventArgs e = new GraphicCellRemovingEventArgs(cells);
            OnGraphicCellRemoving(e);
            return !e.Cancel;
        }

#if !SILVERLIGHT
        public event GraphicCellRemovedEventHandler GraphicCellRemoved;
#else
        internal event GraphicCellRemovedEventHandler GraphicCellRemoved;
#endif

        protected virtual void OnGraphicCellRemoved(GraphicCellRemovedEventArgs e)
        {
            if (GraphicCellRemoved != null)
                GraphicCellRemoved(this, e);
        }

        internal void RaiseGraphicCellRemoved(GraphicCellSpanInfoCollection<GraphicCellSpanInfo> cells)
        {
            GraphicCellRemovedEventArgs e = new GraphicCellRemovedEventArgs(cells);
            OnGraphicCellRemoved(e);
        }

        public event GraphicQueryCellInfoEventHandler GraphicQueryCellInfo;

        protected virtual void OnQueryCellInfo(GraphicQueryCellInfoEventArgs e)
        {
            if (GraphicQueryCellInfo != null)
                GraphicQueryCellInfo(this, e);
        }

        public void QueryGraphicCellInfo(int index, GraphicStyleInfo style)
        {
            GraphicStyleInfoStore store = data[index];
            if (store != null)
                style.ModifyStyle(store, StyleModifyType.Override);

            GraphicQueryCellInfoEventArgs e = new GraphicQueryCellInfoEventArgs(index, style);
            OnQueryCellInfo(e);
        }

        public event GraphicCommitCellInfoEventHandler GraphicCommittedCellInfo;

        protected virtual void OnCommittedCellInfo(GraphicCommitCellInfoEventArgs e)
        {
            if (GraphicCommittedCellInfo != null)
                GraphicCommittedCellInfo(this, e);
        }

        public void CommitGraphicCellInfo(int index, GraphicStyleInfo style, Styles.StyleInfoProperty sip)
        {
            GraphicStyleInfoStore store = data[index];
            if (store == null)
            {
                store = new GraphicStyleInfoStore();
                if (index > -1)
                    data[index] = store;
            }
            if (sip != null)
                store.SetValue(sip, style.Store.GetValue(sip));
            else
                store.ModifyStyle(style.Store, StyleModifyType.Changes);

            GraphicCommitCellInfoEventArgs e = new GraphicCommitCellInfoEventArgs(index, style, sip);
            OnCommittedCellInfo(e);
        }

        #endregion

        public IStyleInfo[] QueryBaseGraphicStyles(int index, GraphicStyleInfo style)
        {
            return null;
        }

        internal void RaiseGraphicQueryCellModel(GraphicQueryCellModelEventArgs e)
        {
            if (e.CellModel == null)
            {
                e.CellModel = CreateCellModel(e.CellType, this);
            }
        }

        private GraphicCellModelBase CreateCellModel(string cellTypeName, GraphicModel graphicModel)
        {
            switch (cellTypeName)
            {
                case "RichTextBox":
                    return new GraphicRichTextBoxCellModel();
                case "ImageCell":
                    return new GraphicImageCellModel();
                case "CheckBox":
                    return new GraphicCheckBoxCellModel();
                default:
                    return new GraphicRichTextBoxCellModel();
            }
        }

        GraphicCellModelCollection cellModels = null;

        /// <summary>
        /// Manages cell types for the grid.
        /// </summary>
        [XmlIgnore]
        public GraphicCellModelCollection CellModels
        {
            get
            {
                if (cellModels == null)
                    cellModels = new GraphicCellModelCollection(this);
                return cellModels;
            }
        }

        public GraphicCellModelBase LookupGraphicCellModel(string id)
        {
            return CellModels[id];
        }

        #region CellRenderers
        
        public GraphicCellRendererCollection CellRenderers
        {
            get
            {
                if (graphicCellRenderers == null)
                    graphicCellRenderers = new GraphicCellRendererCollection(this);
                return graphicCellRenderers;
            }
        }

        public IGraphicCellRenderer CurrentCellRenderers
        {
            get
            {
                return currentCellRenderers;
            }
        }

        internal void SetCurrentCellRenderers(IGraphicCellRenderer Renderer)
        {
            this.currentCellRenderers = Renderer;
        }

        #endregion

        #region Invalidate

        public void Invalidate(int RowIndex, int ColumnIndex)
        {
            int index = graphicCells.FindIndex(RowIndex, ColumnIndex);
            if (index > 0)
            {
                arrangedGraphicCellManager.Invalidate(index);
                volatileCellStyles.Clear(index);
            }
        }

        public void Invalidate(RowColumnIndex cellRowColumnIndex)
        {
            Invalidate(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        public void InvalidateVisual()
        {
            arrangedGraphicCellManager.InvalidateVisual();
            this.GridControl.InvalidateVisual();
        }

        public void InvalidateGraphicCells()
        {
            volatileCellStyles.Clear();
            this.InvalidateVisual();
        }

        #endregion

        #region ArrangeCells

        internal void ArrangeGraphicCells()
        {
            if (this.arrangedGraphicCellManager == null && this.GridControl == null)
                return;
            VisibleLinesCollection visibleRows = this.GridControl.ScrollRows.GetVisibleLines();
            VisibleLinesCollection visibleColumns = this.GridControl.ScrollColumns.GetVisibleLines();
            int top = visibleRows[visibleRows.FirstBodyVisibleIndex].LineIndex;
            int left = visibleColumns[visibleColumns.FirstBodyVisibleIndex].LineIndex;
            int bottom = visibleRows[visibleRows.LastBodyVisibleIndex].LineIndex;
            int right = visibleColumns[visibleColumns.LastBodyVisibleIndex].LineIndex;
            this.arrangedGraphicCellManager.PrepareArrangeGraphicCell();
            foreach (GraphicCellSpanInfo cellspan in this.GraphicCells)
            {
                if (cellspan.RowIndex <= bottom && cellspan.ColumnIndex <= right)
                {
                    VisibleLineInfo visibleRow = visibleRows.GetVisibleLineAtLineIndex(cellspan.RowIndex);
                    VisibleLineInfo visibleColumn = visibleColumns.GetVisibleLineAtLineIndex(cellspan.ColumnIndex);
                    Rect cellRect = Rect.Empty;
                    if (visibleRow != null && visibleColumn != null)
                    {
                        cellRect = new Rect(visibleColumn.Origin, visibleRow.Origin, cellspan.Width, cellspan.Height);
                    }
                    else
                    {
                        cellRect = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(cellspan.RowIndex, cellspan.ColumnIndex), true, true);
                        cellRect.Height = cellspan.Height;
                        cellRect.Width = cellspan.Width;
                    }
                    if (!cellRect.IsEmpty)
                    {
                        cellRect.X += cellspan.OffsetX;
                        cellRect.Y += cellspan.OffsetY;
                        RowColumnIndex CellBottomRowColIndex = this.GridControl.PointToCellRowColumnIndex(new Point(cellRect.X + cellRect.Width, cellRect.Y + cellRect.Height));
                        if (!CellBottomRowColIndex.IsEmpty && ((CellBottomRowColIndex.RowIndex >= top && CellBottomRowColIndex.ColumnIndex >= left) ||
                            (cellspan.RowIndex <= this.GridControl.Model.FrozenRows || cellspan.ColumnIndex <= this.GridControl.Model.FrozenColumns)))
                        {
                            bool isAtTop = false;
                            bool isAtLeftSide = false;
                            bool isAtBottom = false;
                            bool isAtRightSide = false;
                            if (cellspan.RowIndex < grid.Model.FrozenRows)
                                isAtTop = true;
                            if (cellspan.ColumnIndex < grid.Model.FrozenColumns)
                                isAtLeftSide = true;
                            if (CellBottomRowColIndex.RowIndex >= grid.Model.RowCount - grid.Model.FooterRows)
                                isAtBottom = true;
                            if (CellBottomRowColIndex.ColumnIndex >= grid.Model.ColumnCount - grid.Model.FooterColumns)
                                isAtRightSide = true;

                            GraphicStyleInfo style = this[cellspan.CellSpanIndex];
                            GraphicCellUIElement graphicCellUIElement = this.arrangedGraphicCellManager.PreArrangeGraphicCell(cellspan.CellSpanIndex);
                            try
                            {
                                if (graphicCellUIElement == null)
                                    graphicCellUIElement = PrepareGraphicCellUIElements(style, cellspan);
                                if (graphicCellUIElement != null && graphicCellUIElement.UIElement != null)
                                {
                                    bool? isintop = GraphicCellHelper.GetIsInTop(graphicCellUIElement.UIElement);
                                    if (SelectedGraphicCells.Contains(cellspan))
                                    {
#if !SILVERLIGHT
                                        (graphicCellUIElement.UIElement as GraphicCellControl).IsSelected = true;

                                        grid.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            graphicCellUIElement.UIElement.Focus();

                                        }), DispatcherPriority.ApplicationIdle);
#else
                                        grid.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            (graphicCellUIElement.UIElement as Control).Focus();

                                        }));
#endif
                                    }
                                    else
                                        (graphicCellUIElement.UIElement as GraphicCellControl).IsSelected = false;

                                    ScrollControlChildFrame canvas;
                                    canvas = this.GridControl.GetChildFrame(isAtLeftSide, isAtTop, isAtRightSide, isAtBottom, this.GridControl.GraphicFrame);
                                    ScrollControlChildFrame oldCanvas = VisualTreeHelper.GetParent(graphicCellUIElement.UIElement) as ScrollControlChildFrame;
                                    if (oldCanvas != null && oldCanvas != canvas)
                                    {
                                        oldCanvas.Children.Remove(graphicCellUIElement.UIElement);
                                        oldCanvas = null;
                                    }
                                    if(oldCanvas == null)
                                        canvas.Children.Add(graphicCellUIElement.UIElement);
                                    OnArrangeGraphicCell(graphicCellUIElement.UIElement, cellRect, style);
                                    this.arrangedGraphicCellManager.PostArrangeGraphicCell(style.CellIndex, graphicCellUIElement);
                                }
                            }
                            finally
                            {

                            }
                        }
                    }
                }
            }
            this.arrangedGraphicCellManager.ConcludeArrange();
            if (invalidateArrangeOnLoaded)
                invalidateArrangeOnLoaded = false;
        }

        protected void OnArrangeGraphicCell(UIElement uiElements, Rect cellRect, GraphicStyleInfo style)
        {
            IGraphicCellRenderer graphicRenderer = GetGraphicCellRenderer(style);
            if (graphicRenderer != null)
            {
                if (!invalidateArrangeOnLoaded)
                    graphicRenderer.Arrange(uiElements, cellRect, style);
                else
                    graphicRenderer.SetBounds(uiElements, cellRect, true, false);
            }
        }

        protected GraphicCellUIElement PrepareGraphicCellUIElements(GraphicStyleInfo style, GraphicCellSpanInfo cellSpanInfo)
        {
            IGraphicCellRenderer graphicRenderer = GetGraphicCellRenderer(style);
            if (graphicRenderer != null)
            {
                UIElement element = graphicRenderer.PrepareUIElements(style, cellSpanInfo);
                GraphicCellUIElement graphicCellUIElement = new GraphicCellUIElement(element, graphicRenderer);
                return graphicCellUIElement;
            }
            return null;
        }

        protected IGraphicCellRenderer GetGraphicCellRenderer(GraphicStyleInfo style)
        {
            IGraphicCellRenderer graphicRenderer = this.CellRenderers[style.CellType];
            return graphicRenderer;
        }

        internal void SwapGraphicCellUIElement(UIElement element)
        {
            ScrollControlChildFrame bottomChildFrame = this.GridControl.GetChildFrame(false, false, false, false, this.GridControl.GraphicFrame);
            bottomChildFrame.Children.Remove(element);
            ScrollControlChildFrame topChildFrame = this.GridControl.GetChildFrame(false, true, false, false, this.GridControl.GraphicFrame);
            if (topChildFrame.Children.Count > 0)
            {
                UIElement oldelement = topChildFrame.Children[0] as UIElement;
                topChildFrame.Children.Remove(oldelement);
                bottomChildFrame.Children.Add(oldelement);
                Rect cellrect = VisualContainer.GetRenderBounds(oldelement);
                if (!cellrect.IsEmpty)
                    oldelement.Arrange(cellrect);
                GraphicCellHelper.SetIsInTop(oldelement, false);
            }
            topChildFrame.Children.Add(element);
            Rect rect = VisualContainer.GetRenderBounds(element);
            if (!rect.IsEmpty)
                element.Arrange(rect);
        }

        #endregion
    }

    public class GraphicCellInfoCollection : GraphicCellSpanInfoCollection<GraphicCellSpanInfo>
    {
        private int cellspanIndex = 0;
        public GraphicCellInfoCollection()
        {

        }

        public new void Add(GraphicCellSpanInfo span)
        {
            if (span != null)
            {
                span.CellSpanIndex = cellspanIndex;
                base.Add(span);
                cellspanIndex++;
            }
        }

        public int FindIndex(int rowIndex,int columnIndex)
        {
            foreach (GraphicCellSpanInfo item in this)
            {
                if (item.RowIndex == rowIndex && item.ColumnIndex == columnIndex)
                {
                    return IndexOf(item);
                }
            }
            return -1;
        }
    }

    public class GraphicCellData : IList<GraphicStyleInfoStore>
    {
        List<GraphicStyleInfoStore> inner = new List<GraphicStyleInfoStore>();
        public int IndexOf(GraphicStyleInfoStore item)
        {
            return inner.IndexOf(item);
        }

        public void Insert(int index, GraphicStyleInfoStore item)
        {
            inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            //inner.RemoveAt(index);
            inner[index] = null;
        }

        public GraphicStyleInfoStore this[int index]
        {
            get
            {
                GraphicStyleInfoStore value;
                TryGetValue(index, out value);
                if(value == null)
                {

                }
                return value;
            }
            set
            {
                EnsureCount(index + 1);
                inner[index] = value;
            }
        }

        public bool TryGetValue(int index, out GraphicStyleInfoStore value)
        {
            if (inner.Count > index)
            {
                value = inner[index];
                return true;
            }
            value = default(GraphicStyleInfoStore);
            return false;
        }

        void EnsureCount(int count)
        {
            if (inner.Count <= count)
                inner.AddRange(new GraphicStyleInfoStore[count - inner.Count]);
        }

        public void Add(GraphicStyleInfoStore item)
        {
            inner.Add(item);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(GraphicStyleInfoStore item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(GraphicStyleInfoStore[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(GraphicStyleInfoStore item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<GraphicStyleInfoStore> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
