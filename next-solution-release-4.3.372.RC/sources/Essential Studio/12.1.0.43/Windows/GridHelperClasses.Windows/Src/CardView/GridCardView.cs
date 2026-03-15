#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
using Syncfusion.ComponentModel;
using System.ComponentModel;
using Syncfusion.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Provides the information to manipulate the functionalites of CardView.
    /// </summary>
    public class GridCardView
    {
        #region [ Variable Declaration ]

        private GridDataBoundGrid grid = null;
        private int cardSpacingWidth = 5;
        private int cardSpacingHeight = 5;
        private int captionHeight = 35;
        private bool allowResizing = true;
        private int maxCardCols = 0;
        private int maxCardRows = 1;
        private string captionField = string.Empty;
        private bool showCaption = true;
        private CardVisualStyles visualStyle = CardVisualStyles.Office2010Blue;
        private CardStyle cardStyle = CardStyle.StandardLabels;
        private bool showCardCellBorders = false;
        private bool applyRoundedCorner = false;
        private bool highlightActiveCard = false;
        private Color cardBackColor = Color.White;

        #endregion

        /// <summary>
        /// constructor of card view.
        /// </summary>
        public GridCardView() { }

        # region [ Properties ]
        /// <summary>
        /// Gets/Sets the field to be displayed in the Caption.
        /// </summary>
        public string CaptionField
        {
            get { return captionField; }
            set { captionField = value; }
        }

        /// <summary>
        /// Gets/Sets the width between each cards.
        /// </summary>
        public int CardSpacingWidth
        {
            get { return cardSpacingWidth; }
            set { cardSpacingWidth = value; }
        }

        /// <summary>
        /// Gets/Sets the height between each cards.
        /// </summary>
        public int CardSpacingHeight
        {
            get { return cardSpacingHeight; }
            set { cardSpacingHeight = value; }
        }

        /// <summary>
        /// Gets/Sets the height of the caption cell.
        /// </summary>
        public int CaptionHeight
        {
            get { return captionHeight; }
            set { captionHeight = value; }
        }

        /// <summary>
        /// Gets the grid model of the card view.
        /// </summary>
        public GridModel Model
        {
            get { return this.grid != null ? this.grid.Model : null; }
        }

        /// <summary>
        /// Gets/Sets the maximum columns of cards.
        /// </summary>
        public int MaxCardCols
        {
            get { return maxCardCols; }
            set { maxCardCols = value; }
        }

        /// <summary>
        /// Gets/Sets the maximum rows of cards.
        /// </summary>
        public int MaxCardRows
        {
            get { return maxCardRows; }
            set { maxCardRows = value; }
        }

        /// <summary>
        /// Gets/Sets to enable resizing.
        /// </summary>
        public bool AllowResizing
        {
            get { return allowResizing; }
            set
            {
                allowResizing = value;
                if (this.grid != null)
                    SetResizing(value);
            }
        }

        /// <summary>
        /// Gets/Sets the visual themes of the card view.
        /// </summary>
        public CardVisualStyles VisualStyle
        {
            get { return visualStyle; }
            set
            {
                visualStyle = value;
                if (this.grid != null)
                    SetVisualStyle(value);
            }
        }

        /// <summary>
        /// Gets/Sets the card style.
        /// </summary>
        public CardStyle CardStyle
        {
            get { return cardStyle; }
            set
            {
                cardStyle = value;
                if (this.grid != null)
                    SetCardStyle(value);
            }
        }

        /// <summary>
        /// To hide/show the caption button.
        /// </summary>
        public bool ShowCaption
        {
            get { return showCaption; }
            set { showCaption = value; }
        }

        /// <summary>
        /// Gets/Sets the property to handle cell borders.
        /// </summary>
        public bool ShowCardCellBorders
        {
            get { return showCardCellBorders; }
            set { showCardCellBorders = value; }
        }

        /// <summary>
        /// Gets/Sets the card to be rounded.
        /// </summary>
        public bool ApplyRoundedCorner
        {
            get { return applyRoundedCorner; }
            set { applyRoundedCorner = value; }
        }

        /// <summary>
        /// Gets the column information.
        /// </summary>
        public GridBoundColumnsCollection Columns
        {
            get
            {
                return this.grid.Binder.InternalColumns;
            }
        }

        /// <summary>
        /// Gets/Sets to highlight the active card.
        /// </summary>
        public bool HighlightActiveCard
        {
            get { return highlightActiveCard; }
            set { highlightActiveCard = value; }
        }

        /// <summary>
        /// Specifies current cell activation behavior when moving the current cell or clicking inside a cell.
        /// </summary>
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get { return this.grid.ActivateCurrentCellBehavior; }
            set { this.grid.ActivateCurrentCellBehavior = value; }
        }

        /// <summary>
        /// Gets/Sets the backcolor of the outer area.
        /// </summary>
        public Color CardBackColor
        {
            get { return cardBackColor; }
            set { cardBackColor = value; }
        }

        /// <summary>
        /// Gets/Sets the grid to browseOnly state.
        /// </summary>
        public bool BrowseOnly
        {
            get { return this.grid.BrowseOnly; }
            set
            {
                if (this.grid != null)
                    this.grid.BrowseOnly = value;
            }
        }
        /// <summary>
        ///  Gets Syncfusion.Windows.Forms.Grid.GridCurrentCell object that provides storage
        ///     for current cell information and manages all current cell operation such
        ///     as activating, deactivating, saving, editing, and moving the current cell.
        /// </summary>
        public GridCurrentCell CurrentCell
        {
            get { return this.grid.CurrentCell; }
        }
        #endregion

        # region [ Card Events ]

        /// <summary>
        /// Is triggered when the borders are drawn for the current cell.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridDrawCurrentCellBorderEventArgs</param>
        void grid_DrawCurrentCellBorder(object sender, GridDrawCurrentCellBorderEventArgs e)
        {
            if (DrawCurrentCellBorder != null)
                DrawCurrentCellBorder(this, e);
        }

        /// <summary>
        /// Is triggered when the texts are entered in cell
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridDrawCellDisplayTextEventArgs</param>
        void grid_DrawCellDisplayText(object sender, GridDrawCellDisplayTextEventArgs e)
        {
            if (DrawCellDisplayText != null)
                DrawCellDisplayText(this, e);
        }

        /// <summary>
        /// Is triggered when the changes are rejected in current cell
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        void grid_CurrentCellRejectedChanges(object sender, EventArgs e)
        {
            if (CurrentCellRejectedChanges != null)
                CurrentCellRejectedChanges(this, e);
        }

        /// <summary>
        /// Is triggered when the current cell is being validated
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCurrentCellValidateStringEventArgs</param>
        void grid_CurrentCellValidateString(object sender, GridCurrentCellValidateStringEventArgs e)
        {
            if (CurrentCellValidateString != null)
                CurrentCellValidateString(this, e);
        }

        /// <summary>
        /// Is triggered when the drop-down is clicked in the cell.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColCountEventArgs</param>
        void grid_CurrentCellShowingDropDown(object sender, GridCurrentCellShowingDropDownEventArgs e)
        {
            if (CurrentCellShowingDropDown != null)
                CurrentCellShowingDropDown(this, e);
        }

        /// <summary>
        /// When any key is pressed.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCurrentCellErrorMessageEventArgs</param>
        void grid_CurrentCellErrorMessage(object sender, GridCurrentCellErrorMessageEventArgs e)
        {
            if (CurrentCellErrorMessage != null)
                CurrentCellErrorMessage(this, e);
        }

        /// <summary>
        /// When any key is released in keyboard.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyEventArgs</param>
        void grid_CurrentCellKeyUp(object sender, KeyEventArgs e)
        {
            if (CurrentCellKeyUp != null)
                CurrentCellKeyUp(this, e);
        }


        /// <summary>
        /// When any key is pressed.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyPressEventArgs</param>
        void grid_CurrentCellKeyPress(object sender, KeyPressEventArgs e)
        {
            if (CurrentCellKeyPress != null)
                CurrentCellKeyPress(this, e);
        }


        /// <summary>
        /// when the key is pressed down in keyboard
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyEventArgs</param>
        void grid_CurrentCellKeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentCellKeyDown != null)
                CurrentCellKeyDown(this, e);
        }


        /// <summary>
        /// Is trigered when the drop-down is closed.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">PopupClosedEventArgs</param>
        void grid_CurrentCellCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (CurrentCellCloseDropDown != null)
                CurrentCellCloseDropDown(this, e);
        }


        /// <summary>
        /// Is triggered after Validating the current cell.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        void grid_CurrentCellValidated(object sender, EventArgs e)
        {
            if (CurrentCellValidated != null)
                CurrentCellValidated(this, e);
        }

        /// <summary>
        /// Validates the current cell.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CancelEventArgs</param>
        void grid_CurrentCellValidating(object sender, CancelEventArgs e)
        {
            if (CurrentCellValidating != null)
                CurrentCellValidating(this, e);
        }

        /// <summary>
        /// Is triggered after the changes are accepted in current cell.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CancelEventArgs</param>
        void grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            if (CurrentCellAcceptedChanges != null)
                CurrentCellAcceptedChanges(this, e);
        }

        /// <summary>
        /// Is triggered when editing occurs in current cell
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CancelEventArgs</param>
        void grid_CurrentCellStartEditing(object sender, CancelEventArgs e)
        {
            if (CurrentCellStartEditing != null)
                CurrentCellStartEditing(this, e);
        }
        #endregion

        #region [ Inner Methods ]

        #region [..Public Methods..]
        /// <summary>
        /// Wires the bounded grid to design the card view layout.
        /// </summary>
        /// <param name="boundGrid">The GridDataBoundGrid.</param>
        public void WireGrid(GridDataBoundGrid boundGrid)
        {
            this.grid = boundGrid;
            UnwireGrid();
            this.grid.BeginUpdate();
            this.grid.ShowColumnHeaders = false;
            this.grid.Binder.EnableAddNew = false;
            GridControlBase.UseOldHiddenScrollLogic = true;
            HookCardEvents();
            if (!this.grid.Model.CellModels.ContainsKey("CardCaptionCell"))
                this.grid.Model.CellModels.Add("CardCaptionCell", new GridCardCaptionCellModel(grid.Model, this));
            SetResizing(this.AllowResizing);
            this.grid.DefaultRowHeight = 21;
            bool maxCol = false;
            if (MaxCardRows > 1)
            {
                MaxCardCols = this.grid.Binder.RecordCount / MaxCardRows;
                if (this.grid.Binder.RecordCount % MaxCardRows != 0)
                    MaxCardCols += 1;
                maxCol = true;
            }
            if (!maxCol && MaxCardCols > 0)
            {
                MaxCardRows = this.grid.Binder.RecordCount / MaxCardCols;
                if (this.grid.Binder.RecordCount % MaxCardCols != 0)
                    MaxCardRows += 1;
            }
            SetCardStyle(this.CardStyle);
            SetVisualStyle(this.VisualStyle);
            if (this.CardStyle != CardStyle.MergedLabels)
                this.grid.Model.ColWidths[2] = 18;
            this.grid.Model.ColWidths.ResizeToFit(GridRangeInfo.Table());
            this.grid.EndUpdate();
            this.grid.Refresh();
        }

        /// <summary>
        /// Indicates the state of the card if active.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The boolean value.</returns>
        public bool IsActiveCard(int rowIndex, int colIndex)
        {
            int colCount = this.Columns.Count;
            GridCurrentCell cc = this.grid.CurrentCell;
            if (cc.IsActive && cc.RowIndex > rowIndex && colCount >= cc.RowIndex - rowIndex
                && (cc.ColIndex == colIndex || cc.ColIndex == colIndex + 1))
                return true;
            return false;
        }

        /// <summary>
        /// Indicates if the cell is header column cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The boolean value.</returns>
        public bool IsHeaderCell(int rowIndex, int colIndex)
        {
            int colCount = this.Columns.Count;
            int reminder = rowIndex % (colCount + 2);
            switch (CardStyle)
            {
                case CardStyle.StandardLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + ((colIndex + 1) / 3 - 1);
                        int colReminder = (colIndex - 1) % 3;
                        if (colIndex > 0 && colReminder != 0 && recordIndex < this.grid.Binder.RecordCount
                            && (reminder == 0 || reminder > 2) && colReminder == 1)
                        {
                            return true;
                        }
                        return false;
                    }
                case CardStyle.MergedLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + (colIndex / 2 - 1);
                        if (colIndex == 0 && recordIndex < this.grid.Binder.RecordCount && (reminder == 0 || reminder > 2))
                        {
                            return true;
                        }
                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// Indicates if the cell is any record cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The boolean value.</returns>
        public bool IsRecordCell(int rowIndex, int colIndex)
        {
            int colCount = this.Columns.Count;
            int reminder = rowIndex % (colCount + 2);
            switch (CardStyle)
            {
                case CardStyle.StandardLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + ((colIndex + 1) / 3 - 1);
                        int colReminder = (colIndex - 1) % 3;
                        if (colIndex > 0 && colReminder != 0 && recordIndex < this.grid.Binder.RecordCount
                            && (reminder == 0 || reminder > 2) && (colReminder == 2 || colReminder == 1))
                        {
                            return true;
                        }
                        return false;
                    }
                case CardStyle.MergedLabels:
                    return IsValueCell(rowIndex, colIndex);
            }
            return false;
        }

        /// <summary>
        /// Indicates if the cell is a value cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The boolean value.</returns>
        public bool IsValueCell(int rowIndex, int colIndex)
        {
            int colCount = this.Columns.Count;
            int reminder = rowIndex % (colCount + 2);
            switch (CardStyle)
            {
                case CardStyle.StandardLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + ((colIndex + 1) / 3 - 1);
                        int colReminder = (colIndex - 1) % 3;
                        if (colIndex > 0 && colReminder != 0 && recordIndex < this.grid.Binder.RecordCount
                            && (reminder == 0 || reminder > 2) && colReminder == 2)
                        {
                            return true;
                        }
                        return false;
                    }
                case CardStyle.MergedLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + (colIndex / 2 - 1);
                        if (colIndex > 0 && colIndex % 2 == 0 && recordIndex < this.grid.Binder.RecordCount && (reminder == 0 || reminder > 2))
                        {
                            return true;
                        }
                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// Indicates if the cell is a caption cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The boolean value.</returns>
        public bool IsCardCaption(int rowIndex, int colIndex)
        {
            int colCount = this.Columns.Count;
            int reminder = rowIndex % (colCount + 2);
            switch (CardStyle)
            {
                case CardStyle.StandardLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + ((colIndex + 1) / 3 - 1);
                        int colReminder = (colIndex - 1) % 3;
                        if (colIndex > 0 && colReminder != 0 && recordIndex < this.grid.Binder.RecordCount && reminder == 2)
                        {
                            return true;
                        }
                        return false;
                    }
                case CardStyle.MergedLabels:
                    {
                        int recordIndex = ((rowIndex - 2) / (colCount + 2)) * MaxCardCols + (colIndex / 2 - 1);
                        if (colIndex > 0 && colIndex % 2 == 0 && recordIndex < this.grid.Binder.RecordCount && reminder == 2)
                        {
                            return true;
                        }
                        return false;
                    }
                default:
                    return false;

            }
        }

        /// <summary>
        /// Specifies the type of the card cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The card cell type.</returns>
        public CardCellType GetCardCellType(int rowIndex, int colIndex)
        {
            if (IsCardCaption(rowIndex, colIndex))
                return CardCellType.CaptionCell;
            else if (IsHeaderCell(rowIndex, colIndex))
                return CardCellType.HeaderCell;
            else if (IsValueCell(rowIndex, colIndex))
                return CardCellType.ValueCell;
            else if (IsRecordCell(rowIndex, colIndex))
                return CardCellType.RecordCell;
            else
                return CardCellType.None;
        }

        /// <summary>
        /// Unwires the grid and its components.
        /// </summary>
        public void UnwireGrid()
        {
            if (this.grid != null)
            {
                UnHookCardEvents();
                this.grid.Model.CellModels.Remove("CardCaptionCell");
            }
        }
        #endregion

        #region [..Private Methods..]

        /// <summary>
        /// Hooking events while wiring the grid.
        /// </summary>
        private void HookCardEvents()
        {
            this.grid.Model.QueryRowCount += new GridRowColCountEventHandler(Model_QueryRowCount);
            this.grid.Model.QueryColCount += new GridRowColCountEventHandler(Model_QueryColCount);
            this.grid.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            this.grid.Model.QueryColWidth += new GridRowColSizeEventHandler(Model_QueryColWidth);
            this.grid.Model.QueryRowHeight += new GridRowColSizeEventHandler(Model_QueryRowHeight);
            this.grid.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(grid_CurrentCellActivating);
            this.grid.Model.SaveCellInfo += new GridSaveCellInfoEventHandler(Model_SaveCellInfo);
            this.grid.Model.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(Model_QueryCoveredRange);
            this.grid.CellClick += new GridCellClickEventHandler(grid_CellClick);
            this.grid.CellDrawn += new GridDrawCellEventHandler(grid_CellDrawn);
            this.grid.PushButtonClick += new GridCellPushButtonClickEventHandler(grid_PushButtonClick);
            this.grid.QueryNextCurrentCellPosition += new GridQueryNextCurrentCellPositionEventHandler(grid_QueryNextCurrentCellPosition);
            this.grid.CurrentCellStartEditing += new System.ComponentModel.CancelEventHandler(grid_CurrentCellStartEditing);
            this.grid.CurrentCellAcceptedChanges += new CancelEventHandler(grid_CurrentCellAcceptedChanges);
            this.grid.CurrentCellValidating += new CancelEventHandler(grid_CurrentCellValidating);
            this.grid.CurrentCellValidated += new EventHandler(grid_CurrentCellValidated);
            this.grid.CurrentCellCloseDropDown += new Syncfusion.Windows.Forms.PopupClosedEventHandler(grid_CurrentCellCloseDropDown);
            this.grid.CurrentCellKeyDown += new KeyEventHandler(grid_CurrentCellKeyDown);
            this.grid.CurrentCellKeyPress += new KeyPressEventHandler(grid_CurrentCellKeyPress);
            this.grid.CurrentCellKeyUp += new KeyEventHandler(grid_CurrentCellKeyUp);
            this.grid.CurrentCellErrorMessage += new GridCurrentCellErrorMessageEventHandler(grid_CurrentCellErrorMessage);
            this.grid.CurrentCellShowingDropDown += new GridCurrentCellShowingDropDownEventHandler(grid_CurrentCellShowingDropDown);
            this.grid.CurrentCellValidateString += new GridCurrentCellValidateStringEventHandler(grid_CurrentCellValidateString);
            this.grid.CurrentCellRejectedChanges += new EventHandler(grid_CurrentCellRejectedChanges);
            this.grid.DrawCellDisplayText += new GridDrawCellDisplayTextEventHandler(grid_DrawCellDisplayText);
            this.grid.DrawCurrentCellBorder += new GridDrawCurrentCellBorderEventHandler(grid_DrawCurrentCellBorder);
            this.grid.ResizingColumns += new GridResizingColumnsEventHandler(grid_ResizingColumns);
            this.grid.ResizingRows += new GridResizingRowsEventHandler(grid_ResizingRows);
            this.grid.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
        }

        /// <summary>
        /// Unhooking events while unwiring the grid.
        /// </summary>
        private void UnHookCardEvents()
        {
            this.grid.Model.QueryRowCount -= new GridRowColCountEventHandler(Model_QueryRowCount);
            this.grid.Model.QueryColCount -= new GridRowColCountEventHandler(Model_QueryColCount);
            this.grid.Model.QueryCellInfo -= new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            this.grid.Model.QueryColWidth -= new GridRowColSizeEventHandler(Model_QueryColWidth);
            this.grid.Model.QueryRowHeight -= new GridRowColSizeEventHandler(Model_QueryRowHeight);
            this.grid.CurrentCellActivating -= new GridCurrentCellActivatingEventHandler(grid_CurrentCellActivating);
            this.grid.Model.SaveCellInfo -= new GridSaveCellInfoEventHandler(Model_SaveCellInfo);
            this.grid.Model.QueryCoveredRange -= new GridQueryCoveredRangeEventHandler(Model_QueryCoveredRange);
            this.grid.CellClick -= new GridCellClickEventHandler(grid_CellClick);
            this.grid.CellDrawn -= new GridDrawCellEventHandler(grid_CellDrawn);
            this.grid.PushButtonClick -= new GridCellPushButtonClickEventHandler(grid_PushButtonClick);
            this.grid.QueryNextCurrentCellPosition -= new GridQueryNextCurrentCellPositionEventHandler(grid_QueryNextCurrentCellPosition);
            this.grid.CurrentCellStartEditing -= new System.ComponentModel.CancelEventHandler(grid_CurrentCellStartEditing);
            this.grid.CurrentCellAcceptedChanges -= new CancelEventHandler(grid_CurrentCellAcceptedChanges);
            this.grid.CurrentCellValidating -= new CancelEventHandler(grid_CurrentCellValidating);
            this.grid.CurrentCellValidated -= new EventHandler(grid_CurrentCellValidated);
            this.grid.CurrentCellCloseDropDown -= new Syncfusion.Windows.Forms.PopupClosedEventHandler(grid_CurrentCellCloseDropDown);
            this.grid.CurrentCellKeyDown -= new KeyEventHandler(grid_CurrentCellKeyDown);
            this.grid.CurrentCellKeyPress -= new KeyPressEventHandler(grid_CurrentCellKeyPress);
            this.grid.CurrentCellKeyUp -= new KeyEventHandler(grid_CurrentCellKeyUp);
            this.grid.CurrentCellErrorMessage -= new GridCurrentCellErrorMessageEventHandler(grid_CurrentCellErrorMessage);
            this.grid.CurrentCellShowingDropDown -= new GridCurrentCellShowingDropDownEventHandler(grid_CurrentCellShowingDropDown);
            this.grid.CurrentCellValidateString -= new GridCurrentCellValidateStringEventHandler(grid_CurrentCellValidateString);
            this.grid.CurrentCellRejectedChanges -= new EventHandler(grid_CurrentCellRejectedChanges);
            this.grid.DrawCellDisplayText -= new GridDrawCellDisplayTextEventHandler(grid_DrawCellDisplayText);
            this.grid.DrawCurrentCellBorder -= new GridDrawCurrentCellBorderEventHandler(grid_DrawCurrentCellBorder);
            this.grid.ResizingColumns -= new GridResizingColumnsEventHandler(grid_ResizingColumns);
            this.grid.ResizingRows -= new GridResizingRowsEventHandler(grid_ResizingRows);
            this.grid.Model.SelectionChanged -= new GridSelectionChangedEventHandler(Model_SelectionChanged);
        }

        /// <summary>
        /// Sets the selection of current card.
        /// </summary>
        /// <param name="rowIndex">Caption row index.</param>
        /// <param name="colIndex">Caption col index.</param>
        private void SetCardSelected(int rowIndex, int colIndex)
        {
            if (Control.ModifierKeys != Keys.Control)
                this.grid.Model.Selections.Clear();
            switch (CardStyle)
            {
                case CardStyle.MergedLabels:
                    this.grid.Model.Selections.Add(GridRangeInfo.Cells(rowIndex + 1, colIndex, rowIndex + this.Columns.Count, colIndex));
                    break;
                case CardStyle.StandardLabels:
                    this.grid.Model.Selections.Add(GridRangeInfo.Cells(rowIndex + 1, colIndex, rowIndex + this.Columns.Count, colIndex + 1));
                    break;
            }
        }

        /// <summary>
        /// Sets the current cell direction.
        /// </summary>
        /// <param name="e">event data.</param>
        private void SetDirection(GridQueryNextCurrentCellPositionEventArgs e)
        {
            switch (e.Direction)
            {
                case GridDirectionType.Right:
                    e.ColIndex += 1;
                    break;
                case GridDirectionType.Left:
                    e.ColIndex -= 1;
                    break;
                case GridDirectionType.Down:
                    e.RowIndex += 2;
                    break;
                case GridDirectionType.Up:
                    e.RowIndex -= 2;
                    break;
            }
            e.Result = true;
            e.Handled = true;
        }

        /// <summary>
        /// Sets the card style dynamically.
        /// </summary>
        /// <param name="style">style to be set</param>
        private void SetCardStyle(CardStyle style)
        {
            switch (style)
            {
                case CardStyle.MergedLabels:
                    this.grid.ShowRowHeaders = true;
                    break;
                case CardStyle.StandardLabels:
                    this.grid.ShowRowHeaders = false;
                    break;
            }
        }

        /// <summary>
        /// Sets the behavior for resizing.
        /// </summary>
        /// <param name="value">value to allow resizing.</param>
        private void SetResizing(bool value)
        {
            if (value)
            {
                this.grid.Model.Options.ResizeColsBehavior |= GridResizeCellsBehavior.InsideGrid;
                this.grid.Model.Options.ResizeRowsBehavior |= GridResizeCellsBehavior.InsideGrid;
            }
            else
            {
                this.grid.Model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
                this.grid.Model.Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
            }
        }

        /// <summary>
        /// Sets the visual styles of the card view dynamically.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        private void SetVisualStyle(CardVisualStyles style)
        {
            this.grid.Model.EnableLegacyStyle = false;
            this.grid.ThemesEnabled = true;
            switch (style)
            {
                case CardVisualStyles.Office2007Black:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2007Black;
                    break;
                case CardVisualStyles.Office2007Blue:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2007Blue;
                    break;
                case CardVisualStyles.Office2007Silver:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2007Silver;
                    break;
                case CardVisualStyles.Office2010Black:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2010Black;
                    break;
                case CardVisualStyles.Office2010Blue:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2010Blue;
                    break;
                case CardVisualStyles.Office2010Silver:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.Office2010Silver;
                    break;
                case CardVisualStyles.Metro:
                    this.grid.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Metro;
                    break;
                case CardVisualStyles.System:
                    this.grid.ColorStyles = Syncfusion.Windows.Forms.ColorStyles.SystemTheme;
                    this.grid.GridVisualStyles = GridVisualStyles.SystemTheme;
                    this.grid.GridOfficeScrollBars = OfficeScrollBars.None;
                    break;
                case CardVisualStyles.None:
                    this.grid.ThemesEnabled = false;
                    this.grid.GridOfficeScrollBars = OfficeScrollBars.None;
                    break;
            }
        }

        /// <summary>
        /// Sets the specified value into the data source.
        /// </summary>
        /// <param name="field">The column index.</param>
        /// <param name="recordIndex">The index of the record.</param>
        /// <param name="value">Value to be set.</param>
        private void SetValue(int field, int recordIndex, object value)
        {
            DataView dv = null;
            if (this.grid.DataSource is DataTable)
            {
                dv = (this.grid.DataSource as DataTable).DefaultView;
            }
            else if (this.grid.DataSource is DataSet)
            {
                CurrencyManager cm = grid.BindingContext[grid.DataSource, grid.DataMember] as CurrencyManager;
                dv = (cm != null) ? cm.List as DataView : null;
            }
            if (dv != null)
                dv[recordIndex][field] = value;
        }

        /// <summary>
        /// Gets the value from the data source.
        /// </summary>
        /// <param name="field">The column index.</param>
        /// <param name="recordIndex">The index of the record.</param>
        /// <returns>The value.</returns>
        private object GetText(int field, int recordIndex)
        {
            if (this.grid.DataSource == null || this.Columns.Count == 0)
                return null;
            string colName = string.Empty;
            if (this.Columns.Count > field)
                colName = this.Columns[field].MappingName;
            return GetText(colName, recordIndex);
        }

        /// <summary>
        /// Gets the value from the data source.
        /// </summary>
        /// <param name="field">The column name.</param>
        /// <param name="recordIndex">The index of the record.</param>
        /// <returns>The value.</returns>
        private object GetText(string field, int recordIndex)
        {
            DataView dv = null;
            if (!this.Columns.Contains(field))
            {
                //MessageBox.Show("Caption Field is not a valid column name");
                return "";
            }
            if (recordIndex >= this.grid.Binder.RecordCount)
                return string.Empty;
            if (this.grid.DataSource is DataTable)
            {
                dv = (this.grid.DataSource as DataTable).DefaultView;
            }
            else if (this.grid.DataSource is DataSet)
            {
                CurrencyManager cm = grid.BindingContext[grid.DataSource, field] as CurrencyManager;
                dv = (cm != null) ? cm.List as DataView : null;
            }
            return dv != null ? dv[recordIndex][field] : null;
        }

        /// <summary>
        /// Draws the 2-sided rounded corner rectangle path.
        /// </summary>
        /// <param name="rect">The bounds.</param>
        /// <param name="g">The graphics context.</param>
        private void DrawRoundedCorners(Rectangle rect, Graphics g)
        {
            Pen pen = new Pen(new SolidBrush(Color.DimGray));
            Pen pen1 = new Pen(new SolidBrush(CardBackColor));

            g.SmoothingMode = SmoothingMode.HighQuality;

            for (int i = 1; i < 50; i++)
            {
                GraphicsPath path1 = CreateRoundRectangle(rect, i);
                g.DrawPath(pen1, path1);
                path1.Dispose();
            }

            GraphicsPath path = CreateRoundRectangle(rect, 60);
            g.DrawPath(pen, path);
            g.DrawLine(pen, new PointF(rect.X, rect.Y + rect.Height / 2), new PointF(rect.X, rect.Y + rect.Height));
            g.DrawLine(pen, new PointF(rect.X + rect.Width - 1, rect.Y + rect.Height / 2), new PointF(rect.X + rect.Width - 1, rect.Y + rect.Height));
            path.Dispose();
        }

        /// <summary>
        /// Creates/returns the graphics path of the 2-sided rounded corner rectangle with specified angle.
        /// </summary>
        /// <param name="rect">The bounds.</param>
        /// <param name="fCurveRadius">The angle.</param>
        /// <returns>The graphics path.</returns>
        private GraphicsPath CreateRoundRectangle(RectangleF rect, float fCurveRadius)
        {
            // Mimimal dimension
            float fMinDim = (rect.Width > rect.Height) ? rect.Height : rect.Width;
            fCurveRadius = (fCurveRadius > fMinDim / 2) ? fMinDim / 2 : fCurveRadius;

            // If width or height lower zero, get its abs
            if (rect.Width < 0)
            {
                rect.Width = Math.Abs(rect.Width);
                rect.X -= rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Height = Math.Abs(rect.Height);
                rect.Y -= rect.Height;
            }

            // Create offsets for curve
            float offsetX = fCurveRadius * 2;
            float offsetY = offsetX;

            GraphicsPath pathToReturn = new GraphicsPath();
            // Calculate arc arounds.
            RectangleF rcLeftUp = new RectangleF(rect.X, rect.Y, offsetX, offsetY);
            RectangleF rcRigthUp = new RectangleF(rect.Right - offsetX - 1, rect.Y, offsetX, offsetY);

            // Create RoundRect's shape GraphicsPath.
            pathToReturn.AddArc(rcLeftUp, 180, 90);
            pathToReturn.AddArc(rcRigthUp, 270, 90);

            return pathToReturn;
        }
        #endregion

        #endregion

        #region [ Events handling ]

        /// <summary>
        /// Is used when the selection is changed.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridSelectionChangedEventArgs</param>
        void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            
            if (e.Reason == GridSelectionReason.MouseMove && e.Range.RangeType == GridRangeInfoType.Rows)
            {
                this.grid.Invalidate();
            }
        }

        /// <summary>
        /// Is called when the grid's row is resized
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridResizingColumnsEventArgs</param>
        
        void grid_ResizingRows(object sender, GridResizingRowsEventArgs e)
        {
            int colCount = this.Columns.Count;
            if (e.Rows.Bottom == 0 || e.Rows.Bottom % (colCount + 2) == 2)
            {
                e.Cancel = true;
            }

            int reminder = e.Rows.Bottom % (colCount + 2);
            if (reminder == 1)
            {
                e.Cancel = true;
            }

        }

        /// <summary>
        /// Is called when the grid is resized
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridResizingColumnsEventArgs</param>
        void grid_ResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
            switch (this.CardStyle)
            {
                case CardStyle.MergedLabels:
                    if (e.Columns.Left % 2 != 0)
                    {
                        e.Cancel = true;
                    }
                    break;
                case CardStyle.StandardLabels:
                    if ((e.Columns.Left - 1) % 3 == 0)
                    {
                        e.Cancel = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// IS triggered after the cell is drawn
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">GridDrawCellEventArgs</param>
        void grid_CellDrawn(object sender, GridDrawCellEventArgs e)
        {
            if (CellDrawn != null)
                CellDrawn(this, e);
            if (this.ApplyRoundedCorner && this.ShowCaption && IsCardCaption(e.RowIndex, e.ColIndex))
            {
                DrawRoundedCorners(e.Bounds, e.Graphics);
            }
        }

        /// <summary>
        /// Obtains the next current cell's position
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridQueryNextCurrentCellPositionEventArgs</param>
        void grid_QueryNextCurrentCellPosition(object sender, GridQueryNextCurrentCellPositionEventArgs e)
        {
            int colCount = this.Columns.Count;
            int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + (e.ColIndex / 2 - 1);

            int colReminder = (e.ColIndex - 1) % 3;
            switch (this.CardStyle)
            {
                case CardStyle.MergedLabels:
                    if (e.ColIndex % 2 != 0 || e.RowIndex % (colCount + 2) == 1 || recordIndex >= this.grid.Binder.RecordCount)
                    {
                        SetDirection(e);
                    }
                    break;
                case CardStyle.StandardLabels:
                    if (e.Direction == GridDirectionType.Up && e.RowIndex % (colCount + 2) == 2
                        || (colReminder == 0 || e.RowIndex % (colCount + 2) == 1 || recordIndex >= this.grid.Binder.RecordCount))
                    {
                        SetDirection(e);
                    }
                    break;
            }
        }

        /// <summary>
        /// Is triggered when the pushbutton is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCellPushButtonClickEventArgs</param>

        void grid_PushButtonClick(object sender, GridCellPushButtonClickEventArgs e)
        {
            if (PushButtonClick != null)
                PushButtonClick(this, new CardCellPushButtonClickEventArgs(e, this));
            if (IsCardCaption(e.RowIndex, e.ColIndex))
            {
                SetCardSelected(e.RowIndex, e.ColIndex);
            }
        }

        /// <summary>
        /// Is triggered when the cell is clicked
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCellClickEventArgs</param>
        void grid_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (CellClick != null)
                CellClick(this, new CardCellClickEventArgs(e, this));
            //refresh card after highlight
            if (IsRecordCell(e.RowIndex, e.ColIndex) && HighlightActiveCard)
            {
                this.grid.Refresh();
            }
        }

        /// <summary>
        /// Could be used for setting the Covered Range 
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridQueryCoveredRangeEventArgs</param>
        void Model_QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            int colCount = this.Columns.Count;
            int reminder = e.RowIndex % (colCount + 2);
            switch (this.CardStyle)
            {
                case CardStyle.StandardLabels:
                    if (e.ColIndex > 0 && reminder == 2)
                    {
                        int colReminder = (e.ColIndex - 1) % 3;
                        if (colReminder == 2)
                            e.Range = GridRangeInfo.Cells(e.RowIndex, e.ColIndex - 1, e.RowIndex, e.ColIndex);
                        if (colReminder == 1)
                            e.Range = GridRangeInfo.Cells(e.RowIndex, e.ColIndex, e.RowIndex, e.ColIndex + 1);
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Is triggered for saving the changes amde to grid
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridSaveCellInfoEventArgs</param>
        void Model_SaveCellInfo(object sender, GridSaveCellInfoEventArgs e)
        {
            if (SaveCardCellInfo != null)
                SaveCardCellInfo(this, new SaveCardCellInfoEventArgs(e, this));

            int colCount = this.Columns.Count;
            int reminder = e.RowIndex % (colCount + 2);
            int field = reminder == 0 ? colCount - 1 : reminder - 3;
            switch (this.CardStyle)
            {
                case CardStyle.MergedLabels:
                    {
                        int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + (e.ColIndex / 2 - 1);
                        if (e.ColIndex > 0 && e.ColIndex % 2 == 0 && recordIndex < this.grid.Binder.RecordCount)
                        {
                            SetValue(field, recordIndex, e.Style.CellValue);
                        }
                    }
                    break;
                case CardStyle.StandardLabels:
                    {
                        int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + ((e.ColIndex + 1) / 3 - 1);
                        if (e.ColIndex > 0 && (e.ColIndex - 1) % 3 > 1 && recordIndex < this.grid.Binder.RecordCount)
                        {
                            SetValue(field, recordIndex, e.Style.CellValue);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Is triggered after the current cell isactivated
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCurrentCellActivatingEventArgs</param>
        void grid_CurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs e)
        {
            int colCount = this.Columns.Count;
            int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + (e.ColIndex / 2 - 1);
            int colReminder = (e.ColIndex - 1) % 3;
            switch (this.CardStyle)
            {
                case CardStyle.MergedLabels:
                    if (e.ColIndex % 2 != 0 || e.RowIndex % (colCount + 2) == 1 || recordIndex >= this.grid.Binder.RecordCount)
                        e.Cancel = true;
                    break;
                case CardStyle.StandardLabels:
                    if (colReminder == 0 || e.RowIndex % (colCount + 2) == 1 || recordIndex >= this.grid.Binder.RecordCount)
                        e.Cancel = true;
                    break;
            }
            this.grid.Refresh();
        }

        /// <summary>
        /// Sets width for columns during runtime
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColSizeEventArgs</param>
        void Model_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            switch (this.CardStyle)
            {
                case CardStyle.MergedLabels:
                    if (e.Index % 2 != 0)
                    {
                        if (CardSpacingWidth < 0) CardSpacingWidth = 0;
                        e.Size = CardSpacingWidth;
                        e.Handled = true;
                    }
                    break;
                case CardStyle.StandardLabels:
                    if ((e.Index - 1) % 3 == 0)
                    {
                        if (CardSpacingWidth < 0) CardSpacingWidth = 0;
                        e.Size = CardSpacingWidth;
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Sets the Rowheight on runtime
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColSizeEventArgs</param>
        void Model_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            int colCount = this.Columns.Count;
            if (e.Index % (colCount + 2) == 2)
            {
                e.Size = ShowCaption ? CaptionHeight : 0;
                e.Handled = true;
            }

            int reminder = e.Index % (colCount + 2);
            if (reminder == 1)
            {
                if (CardSpacingHeight < 0) CardSpacingHeight = 0;
                e.Size = CardSpacingHeight;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Counts the number of columns in grid.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColCountEventArgs</param>
        void Model_QueryColCount(object sender, GridRowColCountEventArgs e)
        {
            switch (this.CardStyle)
            {
                case CardStyle.StandardLabels:
                    if (MaxCardCols <= 0 || MaxCardCols == this.grid.Binder.RecordCount)
                        e.Count = this.grid.Binder.RecordCount * 3;
                    else
                        e.Count = MaxCardCols * 3;
                    break;
                case CardStyle.MergedLabels:
                    if (MaxCardCols <= 0 || MaxCardCols == this.grid.Binder.RecordCount)
                        e.Count = this.grid.Binder.RecordCount * 2;
                    else
                        e.Count = MaxCardCols * 2;
                    break;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Counts the number of rows in grid
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridRowColCountEventArgs</param>
        void Model_QueryRowCount(object sender, GridRowColCountEventArgs e)
        {
            int colCount = this.Columns.Count;
            e.Count = MaxCardRows * (colCount + 2) + 1;
            e.Handled = true;
        }

        /// <summary>
        /// Sets the style for all the cells
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridQueryCellInfoEventArgs</param>
        void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (QueryCardCellInfo != null)
                QueryCardCellInfo(this, new QueryCardCellInfoEventArgs(e, this));

            switch (this.CardStyle)
            {
                case CardStyle.StandardLabels:
                    StandardStyle(e);
                    break;
                case CardStyle.MergedLabels:
                    MergedStyle(e);
                    break;
            }
        }
        #endregion

        #region [ Standard Style ]
        /// <summary>
        /// Provides the data about standard style.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void StandardStyle(GridQueryCellInfoEventArgs e)
        {
            int colCount = this.Columns.Count;
            int reminder = e.RowIndex % (colCount + 2);

            e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
            if (!ShowCardCellBorders)
                e.Style.Borders.All = new GridBorder(GridBorderStyle.None);
            if (reminder == 1 || reminder == 2) //outside header of cards
            {
                e.Style.CellType = GridCellTypeName.Static;
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
                if (reminder == 1)
                    e.Style.BackColor = CardBackColor;
                e.Style.CellValue = "";
            }
            int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + ((e.ColIndex + 1) / 3 - 1);
            int colReminder = (e.ColIndex - 1) % 3;
            if (e.ColIndex > 0 && colReminder != 0 && recordIndex < this.grid.Binder.RecordCount)
            {
                if (reminder == 0 || reminder > 2)//value cells
                {
                    if (colReminder > 1)
                    {
                        if (ShowCardCellBorders)
                            e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                        e.Style.Enabled = true;
                        int field = reminder == 0 ? colCount - 1 : reminder - 3;
                        e.Style.CellValue = GetText(field, recordIndex);
                    }
                    if (colReminder == 1)
                    {
                        e.Style.CellType = GridCellTypeName.Static;
                        e.Style.Enabled = true;
                        e.Style.BackColor = Color.WhiteSmoke;
                        int col = reminder == 0 ? colCount - 1 : reminder - 3;
                        e.Style.CellValueType = typeof(string);
                        e.Style.Text = this.Columns[col].HeaderText;
                    }
                }
                if (reminder == 3 && !ShowCaption)
                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);

                //card cells except card space cols
                if (e.RowIndex > 0 && reminder != 1)
                {
                    if (colReminder == 2)
                    {
                        if (this.ShowCardCellBorders)
                        {
                            e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, Color.LightGray, GridBorderWeight.Thin);
                        }
                        if (ApplyRoundedCorner && reminder == 2) { }
                        else
                            e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                    }
                    else
                    {
                        if (ApplyRoundedCorner && reminder == 2) { }
                        else
                            e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                        e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
                    }
                }
                if (e.RowIndex > 1 && reminder == 1)//card bottom
                {
                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                }
                //card caption
                if (reminder == 2 && this.ShowCaption)
                {
                    e.Style.CellType = "CardCaptionCell";
                    if (CaptionField == string.Empty) CaptionField = this.Columns[0].MappingName;
                    if (colReminder == 1)
                        e.Style.Description = GetText(CaptionField, recordIndex).ToString();
                    e.Style.Font.Bold = true;
                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                    e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                    e.Style.Enabled = true;
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                    if (!ApplyRoundedCorner)
                    {
                        e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                        e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                    }
                    else
                    {
                        e.Style.Borders.Top = e.Style.Borders.Right = e.Style.Borders.Left = GridBorder.Empty;
                    }
                }
            }
            else
            {
                if (recordIndex >= this.grid.Binder.RecordCount)
                    e.Style.BackColor = CardBackColor;
                e.Style.Borders.All = new GridBorder(GridBorderStyle.None);
            }
            if (colReminder == 0)
            {
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
                e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.None);
                e.Style.BackColor = CardBackColor;
                e.Style.CellValue = "";
            }
        }
        #endregion

        #region [ Merged Style ]
        /// <summary>
        /// Provides the data about merged card style.
        /// </summary>
        /// <param name="e">The event data.</param>
        private void MergedStyle(GridQueryCellInfoEventArgs e)
        {
            int colCount = this.Columns.Count;
            int reminder = e.RowIndex % (colCount + 2);
            e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
            if (e.RowIndex > 2 && e.ColIndex == 0)
            {
                if (reminder == 0 || reminder > 2)//header labels
                {
                    e.Style.CellType = GridCellTypeName.Header;
                    int col = reminder == 0 ? colCount - 1 : reminder - 3;
                    e.Style.Text = this.Columns[col].HeaderText;
                }
            }
            if (reminder == 1 || reminder == 2) //outside header of cards
            {
                e.Style.CellType = GridCellTypeName.Static;
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
                e.Style.BackColor = CardBackColor;
                e.Style.CellValue = "";
            }
            int recordIndex = ((e.RowIndex - 2) / (colCount + 2)) * MaxCardCols + (e.ColIndex / 2 - 1);
            if (e.ColIndex > 0 && e.ColIndex % 2 == 0 && recordIndex < this.grid.Binder.RecordCount)
            {
                if (reminder == 0 || reminder > 2)//value cells
                {
                    if (ShowCardCellBorders)
                        e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                    e.Style.Enabled = true;
                    int field = reminder == 0 ? colCount - 1 : reminder - 3;
                    e.Style.CellValue = GetText(field, recordIndex);
                }
                if (reminder == 3 && !ShowCaption)
                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);

                //card cells except card space cols
                if (e.RowIndex > 0 && reminder != 1)
                {
                    if (ApplyRoundedCorner && reminder == 2) { }
                    else
                    {
                        e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                        e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                    }
                }
                if (e.RowIndex > 1 && reminder == 1)//card bottom
                {
                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                }
                //card caption
                if (reminder == 2 && this.ShowCaption)
                {
                    e.Style.CellType = "CardCaptionCell";
                    if (CaptionField == string.Empty) CaptionField = this.Columns[0].MappingName;
                    e.Style.Description = GetText(CaptionField, recordIndex).ToString();
                    e.Style.Font.Bold = true;
                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                    e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                    e.Style.Enabled = true;
                    if (!ApplyRoundedCorner)
                    {
                        e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                        e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                        e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.DimGray, GridBorderWeight.Thin);
                    }
                }
            }
            else if (e.ColIndex > 0 && recordIndex >= this.grid.Binder.RecordCount)
                e.Style.BackColor = CardBackColor;

            if (e.ColIndex % 2 != 0)
            {
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
                e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.None);
                e.Style.CellValue = "";
                if (e.ColIndex > 0)
                    e.Style.BackColor = CardBackColor;
            }
        }
        #endregion

        # region [ Event Declaration ]
        /// <summary>
        /// Occurs when the card model queries for style information about a specific cell.
        /// </summary>
        public event QueryCardCellInfoEventHandler QueryCardCellInfo;
        /// <summary>
        /// Occurs when the user clicks inside a cell.
        /// </summary>
        public event CardCellClickEventHandler CellClick;
        /// <summary>
        /// Occurs when the user clicks a push button.
        /// </summary>
        public event CardCellPushButtonClickEventHandler PushButtonClick;
        /// <summary>
        /// Occurs when the card model is about to save style information about at specific cell.
        /// </summary>
        public event SaveCardCellInfoEventHandler SaveCardCellInfo;
        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        public event CancelEventHandler CurrentCellStartEditing;
        /// <summary>
        /// Occurs when the grid accepts changes made to the active current cell.
        /// </summary>
        public event CancelEventHandler CurrentCellAcceptedChanges;
        /// <summary>
        /// Occurs when the grid validates contents of the active current cell.
        /// </summary>
        public event CancelEventHandler CurrentCellValidating;
        /// <summary>
        /// Occurs when the grid has successfully validated the contents of the active current cell.
        /// </summary>
        public event EventHandler CurrentCellValidated;
        /// <summary>
        /// Occurs when the grid rejects changes made to the active current cell.
        /// </summary>
        public event EventHandler CurrentCellRejectedChanges;
        /// <summary>
        /// Occurs when the drop-down part of the current cell is closed.
        /// </summary>
        public event PopupClosedEventHandler CurrentCellCloseDropDown;
        /// <summary>
        /// Occurs when the current cell state tends to change on key down.
        /// </summary>
        public event KeyEventHandler CurrentCellKeyDown;
        /// <summary>
        /// Occurs when the current cell state tends to change on key up.
        /// </summary>
        public event KeyEventHandler CurrentCellKeyUp;
        /// <summary>
        /// Occurs when the current cell state tends to change on key press.
        /// </summary>
        public event KeyPressEventHandler CurrentCellKeyPress;
        /// <summary>
        /// The CurrentCellErrorMessage notifies you that the current cell validation
        /// failed and a message is displayed. You can cancel the event and display your
        /// own custom messagebox.
        /// </summary>
        public event GridCurrentCellErrorMessageEventHandler CurrentCellErrorMessage;
        /// <summary>
        /// Occurs when the drop-down part is about to be shown.
        /// </summary>
        public event GridCurrentCellShowingDropDownEventHandler CurrentCellShowingDropDown;
        /// <summary>
        /// Occurs after the user presses a key in the current cell and before it is
        /// accepted. Allows you to limit the keys that are accepted for the current
        /// cell while the user is typing text.
        /// </summary>
        public event GridCurrentCellValidateStringEventHandler CurrentCellValidateString;
        /// <summary>
        /// Occurs for every cell before the grid draws the display text for the specified cell.
        /// </summary>
        public event GridDrawCellDisplayTextEventHandler DrawCellDisplayText;
        /// <summary>
        /// Occurs when the grid draws a border around the current cell.
        /// </summary>
        public event GridDrawCurrentCellBorderEventHandler DrawCurrentCellBorder;
        /// <summary>
        /// Occurs for every cell after the grid has drawn the specified cell.
        /// </summary>
        public event GridDrawCellEventHandler CellDrawn;

        #endregion
    }

    #region [ Event Handlers ]
    /// <summary>
    /// Represents the method that handles a QueryCardCellInfo
    /// event which can be marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void QueryCardCellInfoEventHandler(object sender, QueryCardCellInfoEventArgs e);
    /// <summary>
    /// Represents a method that handles CheckBoxClick, CellClick and CellDoubleClick events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void CardCellClickEventHandler(object sender, CardCellClickEventArgs e);
    /// <summary>
    /// Represents a method that handles a PushButtonClick event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void CardCellPushButtonClickEventHandler(object sender, CardCellPushButtonClickEventArgs e);
    /// <summary>
    /// Represents a method that handles a SaveCardCellInfo event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void SaveCardCellInfoEventHandler(object sender, SaveCardCellInfoEventArgs e);

    #endregion

    #region [ Enum ]

    /// <summary>
    /// Specifies the visual styles of the card view.
    /// </summary>
    public enum CardVisualStyles
    {
        /// <summary>
        /// Applying Office2010Blue visual style 
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Applying Office2010Black visual style 
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Applying Office2010Silver visual style 
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Applying Office2007Blue visual style
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Applying Office2007Black visual style
        /// </summary>
        Office2007Black,
        /// <summary>
        /// Applying Office2007Silver visual style
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Applying Metro visual style
        /// </summary>
        Metro,
        /// <summary>
        /// Applying System visual style
        /// </summary>
        System,
        /// <summary>
        /// Applying No visual style
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies the style of each cards.
    /// </summary>
    public enum CardStyle
    {
        /// <summary>
        /// Provide standard label for cardstyle
        /// </summary>
        StandardLabels,
        /// <summary>
        /// Provide merged label for cardstyle
        /// </summary>
        MergedLabels,
    }

    /// <summary>
    /// Specifies the type of the cell in card view.
    /// </summary>
    public enum CardCellType
    {
        /// <summary>
        /// No cell type is assigned for the card view
        /// </summary>
        None,
        /// <summary>
        /// The cell is a caption cell
        /// </summary>
        CaptionCell,
        /// <summary>
        /// The cell is a header cell
        /// </summary>
        HeaderCell,
        /// <summary>
        /// The cell is record cell
        /// </summary>
        RecordCell,
        /// <summary>
        /// Cell is value cell
        /// </summary>
        ValueCell
    }
    #endregion

    #region [ EventArgs ]

    /// <summary>
    /// Provides the data about QueryCardCellInfo event.
    /// </summary>
    public class QueryCardCellInfoEventArgs : SyncfusionEventArgs
    {
        private GridQueryCellInfoEventArgs inner;
        private GridCardView cardView;

        /// <summary>
        /// constructor for QueryCardCellInfoEventArgs.
        /// </summary>
        /// <param name="e">The base event data</param>
        /// <param name="cardView">The card view object.</param>
        public QueryCardCellInfoEventArgs(GridQueryCellInfoEventArgs e, GridCardView cardView)
        {
            this.inner = e;
            this.cardView = cardView;
        }

        /// <summary>
        /// Gets the cell information.
        /// </summary>
        public GridQueryCellInfoEventArgs Inner
        {
            get { return inner; }
        }

        /// <summary>
        /// Gets the information of cards.
        /// </summary>
        public GridCardView CardView
        {
            get { return cardView; }
        }

        /// <summary>
        /// Gets the type of the card cell.
        /// </summary>
        public CardCellType CardCellType
        {
            get
            {
                return CardView.GetCardCellType(Inner.RowIndex, Inner.ColIndex);
            }
        }
    }

    /// <summary>
    /// Provides the data about CellClick event.
    /// </summary>
    public class CardCellClickEventArgs : SyncfusionEventArgs
    {
        private GridCellClickEventArgs inner;
        private GridCardView cardView;

        /// <summary>
        /// constructor for CardCellClickEventArgs.
        /// </summary>
        /// <param name="e">The base event data</param>
        /// <param name="cardView">The card view object.</param>
        public CardCellClickEventArgs(GridCellClickEventArgs e, GridCardView cardView)
        {
            this.inner = e;
            this.cardView = cardView;
        }
          
        /// <summary>
        /// Gets the cell information.
        /// </summary>
        public GridCellClickEventArgs Inner
        {
            get { return inner; }
        }

        /// <summary>
        /// Gets the information of cards.
        /// </summary>
        public GridCardView CardView
        {
            get { return cardView; }
        }

        /// <summary>
        /// Gets the type of the card cell.
        /// </summary>
        public CardCellType CardCellType
        {
            get
            {
                return CardView.GetCardCellType(Inner.RowIndex, Inner.ColIndex);
            }
        }
    }

    /// <summary>
    /// Provides the data about PushButtonClick event.
    /// </summary>
    public class CardCellPushButtonClickEventArgs : SyncfusionEventArgs
    {
        private GridCellPushButtonClickEventArgs inner;
        private GridCardView cardView;

        /// <summary>
        /// constructor for CardCellPushButtonClickEventArgs.
        /// </summary>
        /// <param name="e">The base event data</param>
        /// <param name="cardView">The card view object.</param>
        public CardCellPushButtonClickEventArgs(GridCellPushButtonClickEventArgs e, GridCardView cardView)
        {
            this.inner = e;
            this.cardView = cardView;
        }

        /// <summary>
        /// Gets the cell information.
        /// </summary>
        public GridCellPushButtonClickEventArgs Inner
        {
            get { return inner; }
        }

        /// <summary>
        /// Gets the information of cards.
        /// </summary>
        public GridCardView CardView
        {
            get { return cardView; }
        }

        /// <summary>
        /// Gets the type of the card cell.
        /// </summary>
        public CardCellType CardCellType
        {
            get
            {
                return CardView.GetCardCellType(Inner.RowIndex, Inner.ColIndex);
            }
        }
    }

    /// <summary>
    /// Provides the data about SaveCardCellInfo event.
    /// </summary>
    public class SaveCardCellInfoEventArgs : SyncfusionEventArgs
    {
        private GridSaveCellInfoEventArgs inner;
        private GridCardView cardView;

        /// <summary>
        /// constructor for SaveCardCellInfoEventArgs.
        /// </summary>
        /// <param name="e">The base event data</param>
        /// <param name="cardView">The card view object.</param>
        public SaveCardCellInfoEventArgs(GridSaveCellInfoEventArgs e, GridCardView cardView)
        {
            this.inner = e;
            this.cardView = cardView;
        }

        /// <summary>
        /// Gets the cell information.
        /// </summary>
        public GridSaveCellInfoEventArgs Inner
        {
            get { return inner; }
        }

        /// <summary>
        /// Gets the information of cards.
        /// </summary>
        public GridCardView CardView
        {
            get { return cardView; }
        }

        /// <summary>
        /// Gets the type of the card cell.
        /// </summary>
        public CardCellType CardCellType
        {
            get
            {
                return CardView.GetCardCellType(Inner.RowIndex, Inner.ColIndex);
            }
        }
    }

    #endregion
} 
