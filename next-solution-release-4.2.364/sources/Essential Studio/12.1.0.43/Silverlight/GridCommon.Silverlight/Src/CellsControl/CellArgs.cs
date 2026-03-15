#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;

#if !WinRT
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// Provides layout information about a cell such as the VisibleRow, VisibleColumb, the CellRect,
    /// Covered Cell Span and also cell style information.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellArgs
    {
        VirtualizingCellsControl cellsControl;
        VisibleLineInfo visibleColumn;
        VisibleLineInfo visibleRow;
        Rect cellRect;
        IRenderCellInfo ci;
        VisibleCoveredCellInfo coveredCellSpan;
        VisibleOverlappingCellInfo overlappingCellSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellArgs"/> class.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="visibleColumn">The visible column.</param>
        /// <param name="cellRect">The cell rect.</param>
        /// <param name="ci">The cell style.</param>
        public CellArgs(VirtualizingCellsControl cellsControl, VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn, Rect cellRect, IRenderCellInfo ci)
        {
            this.cellsControl = cellsControl;
            this.visibleRow = visibleRow;
            this.visibleColumn = visibleColumn;
            this.cellRect = cellRect;
            this.ci = ci;
        }

        /// <summary>
        /// Gets the cells control.
        /// </summary>
        /// <value>The cells control.</value>
        public VirtualizingCellsControl CellsControl
        {
            get { return cellsControl; }
        }

        /// <summary>
        /// Gets the absolute index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get
            {
                if (coveredCellSpan != null)
                    return coveredCellSpan.CoveredCell.Top;
                return visibleRow.LineIndex;
            }
        }

        /// <summary>
        /// Gets the absolute index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get
            {
                if (coveredCellSpan != null)
                    return coveredCellSpan.CoveredCell.Left;
                return visibleColumn.LineIndex;
            }
        }

        /// <summary>
        /// Gets or sets the cell rectangle.
        /// </summary>
        /// <value>The cell rectangle.</value>
        public Rect CellRect
        {
            get
            {
                return this.cellRect;
            }
            set
            {
                this.cellRect = value;
            }
        }

        /// <summary>
        /// Gets the visible covered cell info.
        /// </summary>
        /// <value>The visible covered cell info.</value>
        public VisibleCoveredCellInfo VisibleCoveredCellInfo
        {
            get { return coveredCellSpan; }
            internal set { coveredCellSpan = value; }
        }

        public VisibleOverlappingCellInfo VisibleOverlappingCellInfo
        {
            get { return overlappingCellSpan; }
            internal set { overlappingCellSpan = value; }
        }

        /// <summary>
        /// Gets the cell style information.
        /// </summary>
        /// <value>The cell style information.</value>
        public IRenderCellInfo CellInfo
        {
            get
            {
                return this.ci;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is row header at left side.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row header at left side; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowHeaderAtLeftSide
        {
            get
            {
                return visibleColumn.IsHeader;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is column header at top.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is column header at top; otherwise, <c>false</c>.
        /// </value>
        public bool IsColumnHeaderAtTop
        {
            get
            {
                return visibleRow.IsHeader;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is row footer at right side.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row footer at right side; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowFooterAtRightSide
        {
            get
            {
                return visibleColumn.IsFooter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is column footer at bottom.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is column footer at bottom; otherwise, <c>false</c>.
        /// </value>
        public bool IsColumnFooterAtBottom
        {
            get
            {
                return visibleRow.IsFooter;
            }
        }

        /// <summary>
        /// Gets the visible column.
        /// </summary>
        /// <value>The visible column.</value>
        public VisibleLineInfo VisibleColumn
        {
            get
            {
                return this.visibleColumn;
            }
        }


        /// <summary>
        /// Gets the visible row.
        /// </summary>
        /// <value>The visible row.</value>
        public VisibleLineInfo VisibleRow
        {
            get
            {
                return this.visibleRow;
            }
        }

        /// <summary>
        /// Gets the index of the cell row column.
        /// </summary>
        /// <value>The index of the cell row column.</value>
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                return new RowColumnIndex(RowIndex, ColumnIndex);
            }
        }

        /// <summary>
        /// Gets the cells clipping rectangle.
        /// </summary>
        /// <value>The cells clipping rectangle.</value>
        public Rect CellClipRect
        {
            get
            {
                if (coveredCellSpan != null)
                    return coveredCellSpan.ClippedBounds;

                if (overlappingCellSpan != null)
                {
                    return overlappingCellSpan.ClippedBounds;
                }

                return GridUtil.FromLTRB(visibleColumn.ClippedOrigin, visibleRow.ClippedOrigin, visibleColumn.ClippedCorner, visibleRow.ClippedCorner);
            }
        }

        /// <summary>
        /// Gets the original cell rectangle. This might differ from the <see cref="CellRect"/>
        /// if the <see cref="CellRect"/> was modified. For example OnArrangeCell and OnRenderCell
        /// in VirtualizingCellsControl substract border from the CellRect.
        /// </summary>
        /// <value>The original cell rectangle.</value>
        public Rect OriginalCellRect
        {
            get
            {
                if (coveredCellSpan != null)
                    return coveredCellSpan.ExactBounds;
#if!WinRT
                if (overlappingCellSpan != null)
                {
                    return overlappingCellSpan.ExactBounds;
                }
#endif

                return GridUtil.FromLTRB(visibleColumn.Origin, visibleRow.Origin, visibleColumn.Corner, visibleRow.Corner);
            }
        }

        /// <summary>
        /// Gets the cell visuals.
        /// </summary>
        /// <value>The cell visuals.</value>
        public virtual CellUIElements CellUIElements
        {
            get
            {
                return CellsControl.GetCellUIElements(RowIndex, ColumnIndex);
            }
        }

        /// <summary>
        /// Remove border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="mi"></param>
        /// <returns></returns>
        public Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
                return new Rect(0, 0, 0, 0);

            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;

            return cellRect;
        }
    }

    /// <summary>
    /// Provides layout information about a cell such as the VisibleRow, VisibleColumb, the CellRect,
    /// Covered Cell Span and also cell style information. RenderCellArgs is only used for "Render"
    /// methods in <see cref="VirtualizingCellsControl"/>.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class RenderCellArgs : CellArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderCellArgs"/> class.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="visibleColumn">The visible column.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="ci">The cell style information.</param>
        public RenderCellArgs(VirtualizingCellsControl cellsControl, VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn, Rect cellRect, IRenderCellInfo ci)
            : base(cellsControl, visibleRow, visibleColumn, cellRect, ci)
        {
        }
    }

    /// <summary>
    /// Provides layout information about a cell such as the VisibleRow, VisibleColumb, the CellRect,
    /// Covered Cell Span and also cell style information. ArrangeCellArgs is only used for "Arrange"
    /// methods in <see cref="VirtualizingCellsControl"/>. ArrangeCellArgs also has member for
    /// <see cref="CellUIElements"/> and <see cref="ShouldCreateVisuals"/>.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class ArrangeCellArgs : CellArgs
    {
        private CellUIElements cellUIElements;
        private bool shouldCreateVisuals;
        bool isProcessingArrangeCellUIElements;
        bool shouldInitializeContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrangeCellArgs"/> class.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="visibleColumn">The visible column.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="ci">The cell style information.</param>
        public ArrangeCellArgs(VirtualizingCellsControl cellsControl, VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn, Rect cellRect, IRenderCellInfo ci)
            : base(cellsControl, visibleRow, visibleColumn, cellRect, ci)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ArrangeCellArgs"/> class.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="visibleColumn">The visible column.</param>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="ci">The cell style information.</param>
        public ArrangeCellArgs(VirtualizingCellsControl cellsControl, VisibleLineInfo visibleRow, VisibleLineInfo visibleColumn, Rect cellRect, IRenderCellInfo ci, bool shouldCreateVisuals)
            : this(cellsControl, visibleRow, visibleColumn, cellRect, ci)
        {
            this.ShouldCreateVisuals = shouldCreateVisuals;
        }

        /// <summary>
        /// Gets the cells visuals.
        /// </summary>
        /// <value>The cells visuals.</value>
        public override CellUIElements CellUIElements
        {
            get
            {
                if (this.cellUIElements == null)
                {
                    cellUIElements = new CellUIElements();
                    cellUIElements.Renderer = CellsControl.GetCellRenderer(CellInfo);
                }
                return this.cellUIElements;
            }
        }

        internal void SetCellUIElements(CellUIElements value)
        {
            this.cellUIElements = value;
        }



        /// <summary>
        /// Gets a value indicating whether the cell has visuals.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the cell has visuals; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisuals
        {
            get
            {
                return cellUIElements != null && cellUIElements.UIElements.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether visuals should be create for the cell.
        /// </summary>
        /// <value><c>true</c> if should create visuals should be create for the cell; otherwise, <c>false</c>.</value>
        public bool ShouldCreateVisuals
        {
            get
            {
                return this.shouldCreateVisuals;
            }
            internal set
            {
                this.shouldCreateVisuals = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UIElements should be reinitialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if UIElements should be reinitialized; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldReinitializeContent
        {
            get { return shouldInitializeContent; }
            internal set { shouldInitializeContent = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the parent cells control is calling <see cref="VirtualizingCellsControl.OnArrangeCell"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if parent cells control is calling <see cref="VirtualizingCellsControl.OnArrangeCell"/>; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessingArrangeCellUIElements
        {
            get { return isProcessingArrangeCellUIElements; }
            internal set { isProcessingArrangeCellUIElements = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the UIElement.Measure method
        /// needs to be called.
        /// </summary>
        /// <value><c>true</c> if UIElement.Measure method
        /// needs to be called; otherwise, <c>false</c>.</value>
        public bool ForceMeasure
        {
            get
            {
                return this.ShouldCreateVisuals || this.ShouldReinitializeContent ||
                    this.HasVisuals && this.CellUIElements.IsDirty;
            }
        }
        public bool ShouldSwapUIElements
        {
            get;
            internal set;
        }

        public bool CloneUIElement
        {
            get;
            set;
        }
    }
}
