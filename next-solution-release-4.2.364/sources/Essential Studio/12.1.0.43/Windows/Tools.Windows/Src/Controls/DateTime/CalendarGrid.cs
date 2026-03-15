#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Grid;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// This class is a wrapper for Grid, used in MonthCalendarAdv, for
    /// more extensibility.
    /// </summary>
    public class CalendarGrid
    {
        #region Class constants
        private const int DEF_OFFSET = 1;
        #endregion

        #region Class members
        private int m_offset = 0;
        private GridControlBase m_grid = null;
        private CalendarModel m_model = null;
        private CalendarCurrentCell m_currentCell = null;
        #endregion

        #region Class properties

        protected internal int ColumnOffset
        {
            get
            {
                return m_offset;
            }
            set
            {
                if (value != m_offset)
                {
                    m_offset = value;
                    m_model.ColumnOffset = value;
                    CurrentCell.ColumnOffset = value;
                }
            }
        }
        public GridControlBase GridControl
        {
            get
            {
                return m_grid;
            }
        }
        public DockStyle Dock
        {
            get
            {
                return m_grid.Dock;
            }
            set
            {
                m_grid.Dock = value;
            }
        }
        public bool ForceCurrentCellMoveTo
        {
            get
            {
                return m_grid.ForceCurrentCellMoveTo;
            }
            set
            {
                m_grid.ForceCurrentCellMoveTo = value;
            }
        }
        public MouseControllerDispatcher MouseControllerDispatcher
        {
            get
            {
                return m_grid.MouseControllerDispatcher;
            }
        }
        public GridScrollbarMode VScrollBehavior
        {
            get
            {
                return m_grid.VScrollBehavior;
            }
            set
            {
                m_grid.VScrollBehavior = value;
            }
        }
        public GridScrollbarMode HScrollBehavior
        {
            get
            {
                return m_grid.HScrollBehavior;
            }
            set
            {
                m_grid.HScrollBehavior = value;
            }
        }
        public bool ThemesEnabled
        {
            get
            {
                return m_grid.ThemesEnabled;
            }
            set
            {
                m_grid.ThemesEnabled = value;
            }
        }
        public CalendarModel Model
        {
            get
            {
                return m_model;
            }
            set
            {
                m_model = value;
                this.ColumnOffset = value.ColumnOffset;
                this.GridControl.Model = value.GridModel;
            }
        }
        public Color BackColor
        {
            get
            {
                return m_grid.BackColor;
            }
            set
            {
                m_grid.BackColor = value;
            }
        }
        public Image BackgroundImage
        {
            get
            {
                return m_grid.BackgroundImage;
            }
            set
            {
                m_grid.BackgroundImage = value;
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public ImageLayout BackgroundImageLayout
        {
            get
            {
                return m_grid.BackgroundImageLayout;
            }
            set
            {
                m_grid.BackgroundImageLayout = value;
            }
        }
#endif

        public ContextMenu ContextMenu
        {
            get
            {
                return m_grid.ContextMenu;
            }
            set
            {
                m_grid.ContextMenu = value;
            }
        }

        public CalendarCurrentCell CurrentCell
        {
            get
            {
                if (m_currentCell == null)
                {
                    m_currentCell = new CalendarCurrentCell(m_grid.CurrentCell);
                }

                return m_currentCell;
            }
        }

        public int DefaultColWidth
        {
            get
            {
                return m_grid.DefaultColWidth;
            }
            set
            {
                m_grid.DefaultColWidth = value;
            }
        }

        public int DefaultRowHeight
        {
            get
            {
                return m_grid.DefaultRowHeight;
            }
            set
            {
                m_grid.DefaultRowHeight = value;
            }
        }
        #endregion

        #region Class events
        public event MouseEventHandler MouseDown;
        public event EventHandler Click;
        public event GridDrawCellEventHandler CellDrawn;
        public event GridCellClickEventHandler CellClick;
        public event GridCellMouseEventHandler CellMouseDown;
        public event GridCurrentCellMovingEventHandler CurrentCellMoving;
        public event KeyEventHandler KeyUp;
        public event GridMoveCurrentCellDirectionEventHandler MoveCurrentCellDirection
        {
            add { m_grid.MoveCurrentCellDirection += value; }
            remove { m_grid.MoveCurrentCellDirection -= value; }
        }
        #endregion

        #region Class Initialize/Finalize methods
        public CalendarGrid()
        {
            InitializeGrid();
        }

        protected void InitializeGrid()
        {
            m_grid = new GridControl();

            m_grid.MouseDown += new MouseEventHandler(M_grid_MouseDown);
            m_grid.Click += new EventHandler(M_grid_Click);
            m_grid.CellDrawn += new GridDrawCellEventHandler(M_grid_CellDrawn);
            m_grid.CellClick += new GridCellClickEventHandler(M_grid_CellClick);
            m_grid.CellMouseDown += new GridCellMouseEventHandler(M_grid_CellMouseDown);
            m_grid.CurrentCellMoving += new GridCurrentCellMovingEventHandler(M_grid_CurrentCellMoving);
            m_grid.KeyUp += new KeyEventHandler(M_grid_KeyUp);
        }
        #endregion

        #region Class Public Methods
        
        public void BringToFront()
        {
            m_grid.BringToFront();
        }

        public void Invalidate()
        {
            m_grid.Invalidate();
        }

        public void Refresh()
        {
            m_grid.Refresh();
        }
        public void RefreshRange(GridRangeInfo range)
        {
            GridRangeInfo originalRange = range.OffsetRange(0, ColumnOffset);
            m_grid.RefreshRange(originalRange);
        }

        public void RefreshRange(GridRangeInfo range, bool forceRefreshCurrentCell)
        {
            GridRangeInfo originalRange = range.OffsetRange(0, ColumnOffset);
            m_grid.RefreshRange(originalRange, forceRefreshCurrentCell);
        }
        public void SetRowHeight(int from, int last, int value)
        {
            m_grid.SetRowHeight(from, last, value);
        }
        #endregion

        #region Class overrides
        protected virtual void OnOffset()
        {
            this.m_model.ColumnOffset = m_offset;
            this.CurrentCell.ColumnOffset = m_offset;
        }

        #endregion

        #region Class event handlers
        private void M_grid_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(sender, e);
            }
        }

        private void M_grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(sender, e);
            }
        }

        private void M_grid_CellDrawn(object sender, GridDrawCellEventArgs e)
        {
            if (this.CellDrawn != null)
            {
                GridDrawCellEventArgs args = new GridDrawCellEventArgs(e.Graphics, e.Renderer, e.Bounds, e.RowIndex, e.ColIndex - ColumnOffset, e.Style, e.IsBackgroundErased);     
                args.Cancel = e.Cancel;
                this.CellDrawn(sender, args);
                e.Cancel = args.Cancel;
            }
        }

        private void M_grid_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (this.CellClick != null)
            {
                GridCellClickEventArgs args = new GridCellClickEventArgs(e.RowIndex, e.ColIndex - ColumnOffset, e.MouseEventArgs, e.IsOverImage);
                args.Cancel = e.Cancel;
                this.CellClick(sender, args);
                e.Cancel = args.Cancel;
            }
        }

        private void M_grid_CellMouseDown(object sender, GridCellMouseEventArgs e)
        {
            if (this.CellMouseDown != null)
            {
                GridCellMouseEventArgs args = new GridCellMouseEventArgs(e.RowIndex, e.ColIndex - ColumnOffset, e.CellButton, e.MouseEventArgs);
                args.Cancel = e.Cancel;
                this.CellMouseDown(sender, args);
                e.Cancel = args.Cancel;
            }
        }

        private void M_grid_CurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
        {
            if (this.CurrentCellMoving != null)
            {
                GridCurrentCellMovingEventArgs args =
                    new GridCurrentCellMovingEventArgs(e.RowIndex, e.ColIndex, e.Options);

                args.Cancel = e.Cancel;
                this.CurrentCellMoving(sender, args);
                e.Cancel = args.Cancel;
            }
        }

        private void M_grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.KeyUp != null)
            {
                this.KeyUp(sender, e);
            }
        }
        #endregion
    }

    /// <summary>
    /// This class is a wrapper for GridModel, used in MonthCalendarAdv, for
    /// more extensibility.
    /// </summary>
    public class CalendarModel
    {
        #region Class members
        private GridModel m_gridModel = null;
        private int m_columnOffset = 0;
        #endregion

        #region Class properties

        internal GridModel GridModel
        {
            get { return m_gridModel; }
        }

        protected internal int ColumnOffset
        {
            get
            {
                return m_columnOffset;
            }
            set
            {
                if (value != m_columnOffset)
                {
                    m_columnOffset = value;
                }
            }
        }
        public GridModelOptions Options
        {
            get
            {
                return m_gridModel.Options;
            }
        }
        public GridStyleInfo TableStyle
        {
            get
            {
                return m_gridModel.TableStyle;
            }
        }
        public GridStyleInfo this[int rowIndex, int colIndex]
        {
            get
            {
                return m_gridModel[rowIndex, colIndex + ColumnOffset];
            }
            set
            {
                m_gridModel[rowIndex, colIndex + ColumnOffset] = value;
            }
        }

        public bool IgnoreReadOnly
        {
            get
            {
                return m_gridModel.IgnoreReadOnly;
            }
            set
            {
                m_gridModel.IgnoreReadOnly = value;
            }
        }
        public GridModelRowColSizeIndexer ColWidths
        {
            get
            {
                return m_gridModel.ColWidths;
            }
        }
        public GridModelRowColSizeIndexer RowHeights
        {
            get
            {
                return m_gridModel.RowHeights;
            }
        }
        public GridModelRowStylesIndexer RowStyles
        {
            get
            {
                return m_gridModel.RowStyles;
            }
        }

        public int ColCount
        {
            get
            {
                return m_gridModel.ColCount - ColumnOffset;
            }
            set
            {
                m_gridModel.ColCount = value + ColumnOffset;
            }
        }
        public int RowCount
        {
            get
            {
                return m_gridModel.Model.RowCount;
            }
            set
            {
                m_gridModel.Model.RowCount = value;
            }
        }
        public GridModelRowColOperations Cols
        {
            get
            {
                return m_gridModel.Cols;
            }
        }
        public GridModelRowColOperations Rows
        {
            get
            {
                return m_gridModel.Rows;
            }
        }
        public bool ReadOnly
        {
            get
            {
                return m_gridModel.ReadOnly;
            }
            set
            {
                m_gridModel.ReadOnly = value;
            }
        }
        #endregion

        #region Class events
        public event GridSelectionChangedEventHandler SelectionChanged;
        public event GridRowColCountEventHandler QueryRowCount;
        public event GridRowColCountEventHandler QueryColCount;
        public event GridQueryCellInfoEventHandler QueryCellInfo;
        #endregion

        #region Class Initialize/Finalize methods
        public CalendarModel(GridModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            m_gridModel = model;

            m_gridModel.SelectionChanged += new GridSelectionChangedEventHandler(M_gridModel_SelectionChanged);
            m_gridModel.QueryRowCount += new GridRowColCountEventHandler(M_gridModel_QueryRowCount);
            m_gridModel.QueryColCount += new GridRowColCountEventHandler(M_gridModel_QueryColCount);
            m_gridModel.QueryCellInfo += new GridQueryCellInfoEventHandler(M_gridModel_QueryCellInfo);
        }
        #endregion

        #region Class Public Methods
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo)
        {
            GridRangeInfo originalRange = range.OffsetRange(0, ColumnOffset);
            return m_gridModel.ChangeCells(originalRange, cellInfo);
        }
        #endregion

        #region Class event handlers
        private void M_gridModel_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(sender, e);
            }
        }

        private void M_gridModel_QueryColCount(object sender, GridRowColCountEventArgs e)
        {
            if (this.QueryColCount != null)
            {
                e.Count -= ColumnOffset;
                this.QueryColCount(sender, e);
                e.Count += ColumnOffset;
            }
        }

        private void M_gridModel_QueryRowCount(object sender, GridRowColCountEventArgs e)
        {
            if (this.QueryRowCount != null)
            {
                this.QueryRowCount(sender, e);
            }
        }

        private void M_gridModel_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (this.QueryCellInfo != null)
            {
                GridQueryCellInfoEventArgs args = new GridQueryCellInfoEventArgs(e.RowIndex, e.ColIndex - ColumnOffset, e.Style);
                args.Handled = e.Handled;
                this.QueryCellInfo(sender, args);
                e.Handled = args.Handled;
            }
        }

        #endregion
    }

    /// <summary>
    /// This class is a wrapper for CurrentCell, used in MonthCalendarAdv, for
    /// more extensibility.
    /// </summary>
    public class CalendarCurrentCell
    {
        #region Class members
        private int m_columnOffset = 0;
        private GridCurrentCell m_currentCell = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the horizontal distance.
        /// </summary>
        public int ColumnOffset
        {
            get
            {
                return m_columnOffset;
            }
            set
            {
                if (value != m_columnOffset)
                {
                    m_columnOffset = value;
                }
            }
        }

        /// <summary>
        /// Gets the Index of the cell's row. 
        /// </summary>
        public int RowIndex
        {
            get
            {
                return m_currentCell.RowIndex;
            }
        }

        /// <summary>
        /// Gets the Index of the cell's column. 
        /// </summary>
        public int ColIndex
        {
            get
            {
                return m_currentCell.ColIndex - ColumnOffset;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public CalendarCurrentCell(GridCurrentCell currentCell)
        {
            if (currentCell == null)
                throw new ArgumentNullException("currentCell");

            m_currentCell = currentCell;
        }

        #endregion

        #region Class Public Methods
        public bool MoveTo(int rowIndex, int colIndex)
        {
            return m_currentCell.MoveTo(rowIndex, colIndex + ColumnOffset);
        }

        public bool MoveTo(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
            return m_currentCell.MoveTo(rowIndex, colIndex + ColumnOffset, options);
        }

        /// <summary>
        /// Deactivates the current cell and confirms or rejects changes made to the current cell. 
        /// </summary>
        /// <param name="discardChanges">Bool value for discard chnages</param>
        /// <returns>return true if the current cell is deactive.</returns>
        public bool Deactivate(bool discardChanges)
        {
            return m_currentCell.Deactivate(discardChanges);
        }

        /// <summary>
        /// Notifies that the current cell  is active ( or not ).
        /// </summary>
        /// <param name="rowIndex">Index of the cell's row. </param>
        /// <param name="colIndex">Index of the cell's column</param>
        /// <param name="options">A <see cref="GridSetCurrentCellOptions"/> value that details options how to
        /// activate the current cell. You can specify if the associated control should get focus, if range
        /// selection should be ignored and more</param>
        /// <returns>Return true if the current cell is active.</returns>
        public bool Activate(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
            return m_currentCell.Activate(rowIndex, colIndex + ColumnOffset, options);
        }
        #endregion
    }

    internal class GridRangeInfoAdv :
        GridRangeInfo
    {
        public List<DateTime> Dates = new List<DateTime>();

        public GridRangeInfoAdv()
        {
        }

        protected GridRangeInfoAdv(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public static GridRangeInfoAdv FromGridRangeInfo(GridRangeInfo range)
        {
            SerializationInfo info = new SerializationInfo(typeof(GridRangeInfo), new FormatterConverter());
            StreamingContext context = new StreamingContext(StreamingContextStates.Clone);

            range.GetObjectData(info, context);

            return new GridRangeInfoAdv(info, context);
        }
    }
}
