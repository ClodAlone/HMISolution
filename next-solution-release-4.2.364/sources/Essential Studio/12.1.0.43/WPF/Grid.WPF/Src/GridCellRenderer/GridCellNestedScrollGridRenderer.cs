#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the model part of a nested scroll grid cell.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellNestedScrollGridModel : GridCellModel<GridCellNestedScrollGridRenderer>
    {
    }

    /// <summary>
    /// Implements the renderer part of a nested scroll grid cell.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellNestedScrollGridRenderer : GridVirtualizingCellRenderer<GridCellNestedScrollGridEditor>
    {
        GridNestedAxisLayout rowLayout;
        GridNestedAxisLayout columnLayout;

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedScrollGridRenderer"/>.
        /// </summary>
        public GridCellNestedScrollGridRenderer()
            : this(GridNestedAxisLayout.Normal, GridNestedAxisLayout.Normal)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedScrollGridRenderer"/>.
        /// </summary>
        /// <param name="rowLayout">Row layout.</param>
        /// <param name="columnLayout">Column layout.</param>
        public GridCellNestedScrollGridRenderer(GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            AllowRecycle = true;
            AllowRecycleIfIsKeyboardFocusWithin = true;
            SupportsRenderOptimization = false;
            this.rowLayout = rowLayout;
            this.columnLayout = columnLayout;

            IsControlTextShown = false;
            IsFocusable = true;
            IsModifiable = false;
            IsEditable = true;
        }

        /// <summary>
        /// Returns true if the row layout is shared; false otherwise.
        /// </summary>
        public bool ShareRowLayout
        {
            get { return rowLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Returns true if the column layout is shared; false otherwise.
        /// </summary>
        public bool ShareColumnLayout
        {
            get { return columnLayout == GridNestedAxisLayout.Shared; }
        }

        /// <summary>
        /// Initializes the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="element">The cell control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(GridCellNestedScrollGridEditor element, GridRenderStyleInfo style)
        {
            element.InitializeNested(style.GridControl, style.CellValue as GridModel, rowLayout, columnLayout);

            // For some reason property inheritance does not work if parent is a ScrollViewer,
            // therefore the values are set here explicitly for the child grid.
            VirtualizingCellsControl.SetCellRowColumnIndex(element.Grid, style.CellRowColumnIndex);
            VirtualizingCellsControl.SetRenderCellInfo(element.Grid, style);
            VirtualizingCellsControl.SetCellRenderer(element.Grid, this);
            VirtualizingCellsControl.SetCellsControl(element.Grid, style.GridControl);
        }

        public override void CreateRendererElement(GridCellNestedScrollGridEditor uiElement, GridRenderStyleInfo style)
        {
            uiElement.InitializeNested(style.GridControl, style.CellValue as GridModel, rowLayout, columnLayout);
            base.CreateRendererElement(uiElement, style);
        }


        protected override string GetControlTextFromEditorCore(GridCellNestedScrollGridEditor uiElement)
        {
            return uiElement.ToString();
        }

        protected override void OnUnwireUIElement(GridCellNestedScrollGridEditor uiElement)
        {
            // When grid view goes out of view, unwire the view from the underlying Model so
            // that it can be garbage collected if needed. Model will be set again when
            // OnInitializeContent is called when cell is scrolled into view.
            uiElement.Grid.Model = null;
            //uiElement.Grid = null; - this would cause null ref exception later. 
            
            base.OnUnwireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridCellNestedScrollGridEditor uiElement, GridRenderStyleInfo style)
        {
            uiElement.ArrangeNested(aca, style);
            SetBounds(uiElement, aca.CellClipRect, aca.ForceMeasure, true);

            UIElement el = uiElement.Grid;
            Rect rect = aca.CellClipRect;
            // For some reason property inheritance does not work if parent is a ScrollViewer,
            // therefore the values are set here explicitly for the child grid.
            VisualContainer.SetRenderBounds(el, rect);
        }

        protected override void OnCancelMouseCapture(UIElement element)
        {
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
                cellsControl.MouseControllerDispatcher.CancelMode();
            base.OnCancelMouseCapture(element);
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
                cellsControl.MouseControllerDispatcher.RestoreMode();
            base.OnRecaptureMouse(element);
        }

        /// <summary>
        /// Refreshes the cell content.
        /// </summary>
        public override void RefreshContent()
        {
        }

        protected override void ScrollInView()
        {
        }

        protected override bool OnDeactivating()
        {
            if (CurrentCellUIElement != null && CurrentCellUIElement.Grid != null)
            {
                CurrentCellUIElement.Grid.CurrentCell.Deactivate();
                CurrentCellUIElement.Grid.Model.Selections.Clear();
            }
            return base.OnDeactivating();
        }


        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (CurrentCellUIElement == null)
                return true;

            // This does not work multiple-level nested grids! Only for the first level.
            if (!CurrentCellUIElement.IsKeyboardFocusWithin)
            {
                if (e.Key == Key.Down)
                {
                    GridRangeInfo r = CurrentCellUIElement.Grid.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.Grid.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Up)
                {
                    GridRangeInfo r = CurrentCellUIElement.Grid.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.Grid.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Bottom, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Left)
                {
                    GridRangeInfo r = CurrentCellUIElement.Grid.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.Grid.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Right);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Right)
                {
                    GridRangeInfo r = CurrentCellUIElement.Grid.NavigateWithArrowKeysCellsRange;
                    CurrentCellUIElement.Grid.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// Defines a nested scroll grid editor control that is placed inside a grid cell to form a nested scroll grid cell.
    /// It is based on <see cref="ScrollViewer"/> control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellNestedScrollGridEditor : ScrollViewer
    {
        GridCellNestedGridEditor grid = new GridCellNestedGridEditor();

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedScrollGridEditor"/> object.
        /// </summary>
        public GridCellNestedScrollGridEditor()
        {
            Focusable = true;
            CanContentScroll = true;
            Background = Brushes.Transparent;
            Content = grid;
            AddLogicalChild(grid);
            //VisualContainer.SetWantsMouseInput(this, true);
        }

        /*~NestedSharedLayoutScrollGrid()
        {
            Console.WriteLine("~NestedSharedLayoutScrollGrid");
        }*/

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Grid.Focus();
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellNestedScrollGridEditor"/>.
        /// </summary>
        /// <param name="simpleGridControl">The <see cref="GridControlBase"/> object.</param>
        /// <param name="model">The grid model.</param>
        /// <param name="rowLayout">Row layout.</param>
        /// <param name="columnLayout">Column layout.</param>
        public void InitializeNested(GridControlBase simpleGridControl, GridModel model, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            grid.InitializeNested(simpleGridControl, model, rowLayout, columnLayout);
            VerticalScrollBarVisibility = grid.ShareRowLayout ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = grid.ShareColumnLayout ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
        }

        /// <summary>
        /// Gets or sets the nested grid.
        /// </summary>
        public GridCellNestedGridEditor Grid
        {
            get { return grid; }
            set
            {
                if (grid != value)
                {
                    if (grid != null)
                        this.RemoveLogicalChild(grid);
                    grid = value;
                    Content = grid;
                    ScrollInfo = grid;
                    if (grid != null)
                        this.AddLogicalChild(grid);
                }
            }
        }

        /// <summary>
        /// Arranges the cell content inside the given area.
        /// </summary>
        /// <param name="aca">A reference to <see cref="ArrangeCellArgs"/>.</param>
        public void ArrangeNested(ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            grid.ArrangeNested(this, aca, style);
        }
    }

}
