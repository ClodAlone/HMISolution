#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Cells;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Controls.Scroll;
using System.Collections;
using System.Windows.Threading;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.GridCommon;
using Syncfusion.XlsIO.Implementation.Shapes;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class SpreadsheetGridModel : GridModel, IGridVolatileCellStylesHost
    {
        public SpreadsheetGridModel()
        {
            if (Options != null)
            {
                Options.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
#if !SILVERLIGHT
                this.TableStyle.TextWrapping = TextWrapping.NoWrap;
                this.TableStyle.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
                //this.TableStyle.EnableFloatingCell = true;
                this.Options.WrapCell = false;
#else
                this.TableStyle.TextWrapping = TextWrapping.NoWrap;
                this.TableStyle.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
                //this.TableStyle.EnableFloatCell = true;
#endif
                // Options.EnableFloatingCell = true;
                Options.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;
                Options.AllowExcelLikeResizing = true;
                Options.ExcelLikeFreezePane = true;
                Options.ExcelLikeSelectionFrame = true;
                Options.ExcelLikeCurrentCell = true;
                this.ColumnWidths[0] = 24;
                SetGridModelCommandManager(this);
                this.ColumnWidths.LineHiddenChanged += new HiddenRangeChangedEventHandler(OnColumnWidthsLineHiddenChanged);
                this.RowHeights.LineHiddenChanged += new HiddenRangeChangedEventHandler(OnRowHeightsLineHiddenChanged);                
#if !SILVERLIGHT
                this.GraphicModel.GraphicCellMoved += new GraphicCellMovedEventHandler(GraphicModel_GraphicCellMoved);
                this.GraphicModel.GraphicCellResized += new GraphicCellResizedEventHandler(GraphicModel_GraphicCellResized);
                this.GraphicModel.GraphicCellRemoved += new GraphicCellRemovedEventHandler(GraphicModel_GraphicCellRemoved);
                this.GraphicModel.CurrentGraphicCellActivated += new CurrrentGraphicCellActivatedEventHandler(GraphicModel_CurrentGraphicCellActivated);
                this.GraphicModel.CurrentGraphicCellDeactivated += new CurrrentGraphicCellDeactivatedEventHandler(GraphicModel_CurrentGraphicCellDeactivated);
#endif

            }
        }

#if !SILVERLIGHT
        void GraphicModel_CurrentGraphicCellDeactivated(object sender, CurrrentGraphicCellDeactivatedEventArgs e)
        {
            this.ExcelProperties.spreadControl.GridProperties.IsFocusedOnGraphicCells = false;
            this.ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();
            this.ActiveGridView.InvalidateRenderCell(GridRangeInfo.Row(0));
            this.ActiveGridView.InvalidateRenderCell(GridRangeInfo.Col(0));
        }

        void GraphicModel_CurrentGraphicCellActivated(object sender, CurrrentGraphicCellActivatedEventArgs e)
        {
            this.ExcelProperties.spreadControl.GridProperties.IsFocusedOnGraphicCells = true;
            this.ExcelProperties.spreadControl.FormulaBar.Text = string.Empty;
            if (this.GraphicModel.SelectedGraphicCells.Count == 1)
            {
                this.ExcelProperties.spreadControl.GridProperties.nameboxtext = e.CurrentSpanInfo.Name;
                this.ExcelProperties.spreadControl.GridProperties.CellName = e.CurrentSpanInfo.Name;
            }
            this.ActiveGridView.InvalidateRenderCell(GridRangeInfo.Cell(this.ActiveGridView.CurrentCell.RowIndex, this.ActiveGridView.CurrentCell.ColumnIndex));
            this.ActiveGridView.InvalidateRenderCell(GridRangeInfo.Row(0));
            this.ActiveGridView.InvalidateRenderCell(GridRangeInfo.Col(0));
        }

        void GraphicModel_GraphicCellRemoved(object sender, GraphicCellRemovedEventArgs e)
        {
            IWorksheet sheet = this.ExcelProperties.WorkBook.ActiveSheet;
            foreach (var spanInfo in e.RemovedGraphicCells)
            {
                ShapeImpl shape = sheet.Shapes[spanInfo.Name] as ShapeImpl;
                if (shape != null)
                    shape.Remove();
            }
        }

        void GraphicModel_GraphicCellResized(object sender, GraphicCellResizedEventArgs e)
        {
            IWorksheet sheet= this.ExcelProperties.WorkBook.ActiveSheet;
            foreach (ShapeImpl shape in sheet.Shapes)
            {
                if (shape.Name == e.Name)
                {
                    shape.TopRow = e.CellSpanInfo.RowIndex;
                    shape.LeftColumn = e.CellSpanInfo.ColumnIndex;
                    shape.Height = (int)e.CellSpanInfo.Height;
                    shape.Width = (int)e.CellSpanInfo.Width;

                    double xPos = 0, yPos = 0;
                    for (int col = 1; col < shape.LeftColumn; col++)
                        xPos += sheet.GetColumnWidthInPixels(col);
                    for (int row = 1; row < shape.TopRow; row++)
                        yPos += sheet.GetRowHeightInPixels(row);

                    shape.Left = (int)(e.CellSpanInfo.OffsetX + xPos);
                    shape.Top = (int)(e.CellSpanInfo.OffsetY + yPos);
                }
            }
        }

        void GraphicModel_GraphicCellMoved(object sender, GraphicCellMovedEventArgs e)
        {
            IWorksheet sheet = this.ExcelProperties.WorkBook.ActiveSheet;
            foreach (ShapeImpl shape in sheet.Shapes)
            {
                if (shape.Name == e.Name)
                {
                    shape.TopRow = e.CellSpanInfo.RowIndex;
                    shape.LeftColumn = e.CellSpanInfo.ColumnIndex;
                    shape.Height = (int)e.CellSpanInfo.Height;
                    shape.Width = (int)e.CellSpanInfo.Width;

                    double xPos = 0, yPos = 0;
                    for (int col = 1; col < shape.LeftColumn; col++)
                        xPos += sheet.GetColumnWidthInPixels(col);
                    for (int row = 1; row < shape.TopRow; row++)
                        yPos += sheet.GetRowHeightInPixels(row);
                   
                    shape.Left = (int)(e.CellSpanInfo.OffsetX + xPos);
                    shape.Top = (int)(e.CellSpanInfo.OffsetY + yPos);
                }
            }
        }

#else
        protected override GraphicModel OnGraphicModelCreated()
        {
            return new SpreadsheetGraphicModel(this);
        }

#endif

        void OnRowHeightsLineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            if (this.IsInGroup)
                return;

            if (e.To > 0 && !string.IsNullOrEmpty(SheetName))
            {
                int StartingRow = e.From;
                if(e.From <= 0)
                 StartingRow = 1;
                int EndingRow = e.To;
                IWorksheet worksheet = ExcelProperties.WorkBook.Worksheets[SheetName];
                if (worksheet == null || (this.SperadsheetGrid.Model.RowCount - 1 == EndingRow && StartingRow == 1))
                    return;
                
                if (!e.Hide)
                {
                    for (int row = StartingRow; row <= EndingRow; row++)
                        worksheet.ShowRow(row, true);
                }
                else
                {
                    for (int row = StartingRow; row <= EndingRow; row++)
                        worksheet.ShowRow(row, false);
                }
            }
        }

        void OnColumnWidthsLineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            if (e.To > 0 && !string.IsNullOrEmpty(SheetName))
            {
                int StartingColumn = e.From;
                if (e.From <= 0)
                    StartingColumn = 1;
                int EndingColumn = e.To;
                IWorksheet worksheet = ExcelProperties.WorkBook.Worksheets[SheetName];
                if (worksheet == null || (this.SperadsheetGrid.Model.ColumnCount - 1 == EndingColumn && StartingColumn == 1))
                    return;

                if(!e.Hide)
                {
                    for (int column = StartingColumn; column <= EndingColumn; column++)
                        worksheet.ShowColumn(column, true);
                }
                else
                {
                    for (int column = StartingColumn; column <= EndingColumn; column++)
                        worksheet.ShowColumn(column, false);
                }
            }
        }

        protected override void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            if (e.CellType == "FormulaCell")
            {
                e.CellModel = new SpreadsheetGridFormulaModel(this);
            }
            base.OnQueryCellModel(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
        protected override void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            //invalidate row/column header to reflect HighlightSelectionBackground
            if (e.Reason == GridSelectionReason.SelectRange || e.Reason== GridSelectionReason.MouseMove || e.Reason== GridSelectionReason.SetCurrentCell || e.Reason== GridSelectionReason.MouseDown)
            {
                this.InvalidateCell(GridRangeInfo.Row(0));
                this.InvalidateCell(GridRangeInfo.Col(0));
#if SILVERLIGHT
                this.InvalidateVisual();
#endif
            }
        }

        public  GridRenderStyleInfo GetCellStyle(RowColumnIndex RowColumnIndex)
        {
            return _spreadsheetGrid.RenderStyles.GetRenderStyleInfo(RowColumnIndex);
        }

        #region FormulaBar Editing Behavior
        
        private bool isInFormulabarEditing = false;
        /// <summary>
        /// Suspend Formaula parsing and calculation of the CurrentCell when we read the formatted from the GridStyleInfo
        /// </summary>
        internal bool IsInFormulabarEditing
        {
            get { return isInFormulabarEditing; }
            set { isInFormulabarEditing = value; }
        }

        internal void SuspendCurrentCellFormulaParsing()
        {
            this.isInFormulabarEditing = true;
        }

        internal void ResumeCurrentCellFormulaParsing()
        {
            this.isInFormulabarEditing = false;
        }

        protected override void SuspendFormattedTextCalculation()
        {
            base.SuspendFormattedTextCalculation();
        }

        protected override void ResumeFormattedTextCalculation()
        {
            base.ResumeFormattedTextCalculation();
        }
        #endregion

        public ExcelProperties ExcelProperties
        {
            get
            {
                if (this.SperadsheetGrid != null)
                    return this.SperadsheetGrid.ExcelProperties;
                return null;
            }
        }

        public string SheetName
        {
            get
            {
                if (this.SperadsheetGrid != null)
                    return this.SperadsheetGrid.SheetName;
                return null;
            }
        }


        private bool isInInsert = false;
        /// <summary>
        /// Returns True if inserting rows or columns.
        /// </summary>
        public bool IsInInsert
        {
            get
            {
                return isInInsert;
            }

            set
            {
                isInInsert = value;
            }
        }

        private bool isInGroup = false;
        /// <summary>
        /// Returns True if Grouping or Ungrouping.
        /// </summary>
        public bool IsInGroup
        {
            get { return isInGroup; }
            set { isInGroup = value; }
        }

        private SpreadsheetGrid _spreadsheetGrid;
        internal SpreadsheetGrid SperadsheetGrid
        {
            get { return _spreadsheetGrid; }
            set { _spreadsheetGrid = value; }
        }

        protected override GridVolatileCellStyles CreateVolatileCellStyles()
        {
            return new ExcelGridVolatileCellStyles(this);
        }

        protected override void OnCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            base.OnCellRequestNavigate(e);
            if (!e.Handled)
            {
                SperadsheetGrid.CellRequestNavigate(e);
            }
        }

        private ISpreadsheetGridVisualStyle _visualStyle;
        internal ISpreadsheetGridVisualStyle SpreadsheetGridVisualStyle
        {
            get
            {
                if (_visualStyle == null)
                    return new DefaultSpreadsheetGridVisualStyle();
                return _visualStyle;
            }
            set
            {
                _visualStyle = value;
            }


        }

        #region WrapText

        /// <summary>
        /// Checks the wrap row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="curCol">The cur col.</param>
        /// <returns></returns>
        public bool checkWrapRow(int row, int curCol)
        {
            double TempLength = 0.00;

            for (int col = 1; col < this.ColumnCount; col++)
            {
                var style = GetCellStyle(new RowColumnIndex(row, col));
                if ((style.TextWrapping == TextWrapping.Wrap) && (TempLength < style.Text.Length) && col != curCol)
                {
                    TempLength = calculateTextLengthPixals(row, col);
                    this.ResizeRowsToFit(GridRangeInfo.Cell(row, col), GridResizeToFitOptions.None);
                }
            }

            if (TempLength < calculateTextLengthPixals(row, curCol))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates the text length pixals.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        internal double calculateTextLengthPixals(int row, int col)
        {
            var style = GetCellStyle(new RowColumnIndex(row, col));
#if!SILVERLIGHT
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1));
            System.Drawing.SizeF textSize = graphics.MeasureString(this[row, col].Text, new System.Drawing.Font(this[row, col].Font.FontFamily.ToString(), (float)this[row, col].Font.FontSize));
            double textlength = textSize.Width;
#else
            TextBlock txtMeasure = new TextBlock();
            txtMeasure.FontSize = style.Font.FontSize;
            txtMeasure.Text = style.Text;
            txtMeasure.FontFamily = style.Font.FontFamily;
            double textlength = txtMeasure.ActualWidth;
#endif
            return textlength;
        }

    

        /// <summary>
        /// Sets the wrap text.
        /// </summary>
        internal void SetWrapText(bool isWrapText)
        {
            foreach (GridRangeInfo range in this.SelectedRanges)
            {
                int tempRow = 0, tempCol = 0;
                GridRangeInfo item = range;
                //whether the selection is single cell or section of cells
                if (item.RangeType == GridRangeInfoType.Cells)
                {
                    for (int i = item.Top; i <= item.Bottom; i++)
                    {
                        //temp length has been created to check whether the selected cells row already have enough height(if there is any other tex wrap cell available ) or not
                        double TempLength = 0.00;
                        for (int col = 1; col < this.ColumnCount; col++)
                        {
                            //check whether already text wrap cell in that row
                            if ((this[i, col].TextWrapping == TextWrapping.Wrap) && (TempLength < calculateTextLengthPixals(i, col)))
                            {
                                TempLength = calculateTextLengthPixals(i, col);
                                tempRow = i;
                                tempCol = col;
                            }
                        }
                        //set the textwrap in selected cells
                        for (int j = item.Left; j <= item.Right; j++)
                        {
                            if (isWrapText)
                            {
#if !SILVERLIGHT
                                this[i, j].TextTrimming = TextTrimming.None;
                                this[i, j].FloatCellMode = GridFloatCellsMode.None;
#else
                            this[i, j].FloatCellsMode = GridFloatCellsMode.None;
                            this[i, j].EnableFloatCell = false;
#endif
                                this[i, j].TextWrapping = TextWrapping.Wrap;
                                if (TempLength <= calculateTextLengthPixals(i, j))
                                {
                                    this.CommandStack.SuspendUndo = true;
                                    //ResizeRows to fit called whether the row of selected cell dont have enough space to fit the text                                 
                                    this.ResizeRowsToFit(GridRangeInfo.Cell(i, j), GridResizeToFitOptions.NoShrinkSize);
                                    this.CommandStack.SuspendUndo = false;
                                }
                            }
                            else
                            {
                                if (TempLength > this.ExcelProperties.DefaultRowHeight)
                                {
                                    this[tempRow, tempCol].TextTrimming = TextTrimming.None;
                                    this.ResizeRowsToFit(GridRangeInfo.Cell(tempRow, tempCol), GridResizeToFitOptions.None);
                                }
                                else
                                    this.RowHeights[i] = this.ExcelProperties.WorkBook.ActiveSheet.GetRowHeightInPixels(i);
                            }
                        }
                    }
                }
                //whether the selection is column
                else if (item.RangeType == GridRangeInfoType.Cols)
                {
                    // check all the wrapText cells (since in column selection we need to check rowheights for all row) in spreadsheetGrid whether they already have enough height or not using text
                    //pixel legth calculation of wrap cells text

                    for (int col = item.Left; col <= item.Right; col++)
                    {
                        for (int row = 1; row < this.RowCount; row++)
                        {
                            double TempLength = 0.00;
                            for (int i = 1; i < this.ColumnCount; i++)
                            {
                                if (this[row, i].TextWrapping == TextWrapping.Wrap && i != col)
                                {
                                    if (TempLength < calculateTextLengthPixals(row, i))
                                    {
                                        TempLength = calculateTextLengthPixals(row, i);
                                        tempRow = row;
                                        tempCol = i;
                                    }
                                }
                            }
                            if (isWrapText)
                            {
#if !SILVERLIGHT
                                this[row, col].TextTrimming = TextTrimming.None;
                                this[row, col].FloatCellMode = GridFloatCellsMode.None;
                                //this[row, item.Left].EnableFloatingCell = false;
#else
                        this[row, col].FloatCellsMode = GridFloatCellsMode.None;
                        this[row, col].EnableFloatCell = false;
#endif
                                this[row, col].TextWrapping = TextWrapping.Wrap;
                                if (TempLength < calculateTextLengthPixals(row, col))
                                {
                                    this.ResizeRowsToFit(GridRangeInfo.Cell(row, col), GridResizeToFitOptions.None);
                                }
                            }
                            else
                            {
                                if (TempLength > this.ExcelProperties.DefaultRowHeight)
                                {
                                    this[tempRow, tempCol].TextTrimming = TextTrimming.None;
                                    this.ResizeRowsToFit(GridRangeInfo.Cell(tempRow, tempCol), GridResizeToFitOptions.None);
                                }
                                else
                                    this.RowHeights[row] = this.ExcelProperties.WorkBook.ActiveSheet.GetRowHeightInPixels(row);
                            }
                        }
                    }
                }
                //whether the selection is Row
                else if (item.RangeType == GridRangeInfoType.Rows || item.RangeType == GridRangeInfoType.Table)
                {
                    //whether the selection is table
                    if (item.RangeType == GridRangeInfoType.Table)
                    {
                        item = GridRangeInfo.Cells(1, 1, this.RowCount - 1, this.ColumnCount - 1);
                    }

                    // set the text wrap in all cells in selected row
                    for (int row = item.Top; row <= item.Bottom; row++)
                    {
                        double TempLength = 0.00;
                        for (int col = 1; col < this.ColumnCount; col++)
                        {
                            if (isWrapText)
                            {
#if !SILVERLIGHT
                                this[row, col].TextTrimming = TextTrimming.None;
                                this[row, col].FloatCellMode = GridFloatCellsMode.None;
                                //this[item.Bottom, col].EnableFloatingCell = false;

#else
                        this[row, col].FloatCellsMode = GridFloatCellsMode.None;
                        this[row, col].EnableFloatCell = false;
#endif
                                this[row, col].TextWrapping = TextWrapping.Wrap;
                                //check whether selected row already contains the enough height or not
                                if (TempLength < calculateTextLengthPixals(row, col))
                                {
                                    TempLength = calculateTextLengthPixals(row, col);
                                    tempRow = row;
                                    tempCol = col;
                                    this.ResizeRowsToFit(GridRangeInfo.Cell(tempRow, tempCol), GridResizeToFitOptions.None);
                                }
                            }
                            else
                            {
                                this.RowHeights[row] = this.ExcelProperties.WorkBook.ActiveSheet.GetRowHeightInPixels(row);
                            }
                        }
                    }
                }
                this.InvalidateCell(item);
            }
        }

        #endregion

        #region Cut,Copy,Paste

        protected override void OnClipboardPasted(GridCutPasteEventArgs e)
        {
            base.OnClipboardPasted(e);
            if (e.RangeList != null)
            {
                foreach (GridRangeInfo gridRange in e.RangeList)
                {
                    for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
                    {
                        for (int col = gridRange.Left; col <= gridRange.Right; col++)
                        {
                            var Style = this[row, col];
                            this.SperadsheetGrid.CommittedCellRowColumnIndex = Style.CellRowColumnIndex;
                            if (Style.Description != null && Style.Description == "IsImported")
                            {
                                IWorksheet changedSheet = ExcelProperties.WorkBook.Worksheets[SheetName];
                                if (changedSheet != null)
                                {
                                    IRange range = changedSheet.Range[row, col];
                                    GridCommitCellInfoEventArgs arg = new GridCommitCellInfoEventArgs(Style.CellRowColumnIndex, Style, GridStyleInfoStore.CellValueProperty);
                                    ImportExportHelper.ExportCellToExcel(this, ExcelProperties.WorkBook, range, arg);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnClipboardCut(GridCutPasteEventArgs e)
        {
            base.OnClipboardCut(e);
            if (e.RangeList != null)
            {

                foreach (GridRangeInfo gridRange in e.RangeList)
                {
                    for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
                    {
                        for (int col = gridRange.Left; col <= gridRange.Right; col++)
                        {
                            var Style = this[row, col];
                            this.SperadsheetGrid.CommittedCellRowColumnIndex = Style.CellRowColumnIndex;
                        }
                    }
                }
            }
        }
        #endregion

        #region Insert/Remove Row/Column

        public override void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRowsCore(insertAtRowIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtRowIndex, count);
            OnRowsInserted(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new SpreadsheetModelRemoveRowsCommand(this, insertAtRowIndex, count));
            }
        }

        public override void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumnsCore(insertAtColumnIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtColumnIndex, count);
            OnColumnsInserted(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new SpreadsheetModelRemoveColumnsCommand(this, insertAtColumnIndex, count));
            }
        }

        protected override void InsertColumnsCore(int insertAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if(moveCellsState == null)
            {
                moveCellsState = GridMoveCellsState.Empty;
                moveCellsState.SuspendSelections = true;
            }
            base.InsertColumnsCore(insertAtColumnIndex, count, moveCellsState);
        }

        protected override void InsertRowsCore(int insertAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null)
            {
                moveCellsState = GridMoveCellsState.Empty;
                moveCellsState.SuspendSelections = true;
            }
            base.InsertRowsCore(insertAtRowIndex, count, moveCellsState);
        }

        public override void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRowsCore(removeAtRowIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtRowIndex, count);
            OnRowsRemoved(e);
            //if (this.CommandStack.ShouldGenerateUndoInfo)
            //{
            //    this.CommandStack.Push(new SpreadsheetModelInsertRowsCommand(this, removeAtRowIndex, count));
            //}
        }

        public override void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumnsCore(removeAtColumnIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtColumnIndex, count);
            OnColumnsRemoved(e);
            //if (CommandStack.ShouldGenerateUndoInfo)
            //{
            //    CommandStack.Push(new SpreadsheetModelInsertColumnsCommand(this, removeAtColumnIndex, count));
            //}
        }

        protected override void RemoveColumnsCore(int removeAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null)
            {
                moveCellsState = GridMoveCellsState.Empty;
                moveCellsState.SuspendSelections = true;
            }
            base.RemoveColumnsCore(removeAtColumnIndex, count, moveCellsState);
        }

        protected override void RemoveRowsCore(int removeAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null)
            {
                moveCellsState = GridMoveCellsState.Empty;
                moveCellsState.SuspendSelections = true;
            }
            base.RemoveRowsCore(removeAtRowIndex, count, moveCellsState);
        }

        #endregion

        #region UndoRedo

        protected override void SetGridModelCommandManager(GridModel gridModel)
        {
            base.SetGridModelCommandManager(gridModel);
        }

        public override bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
        {
            // return base.ChangeCells(range, cellsInfo, modifyType);
            bool success = false;
            GridStyleInfo[] savedCellsInfo = null;    // will be filled with style setting
            //// start op, generate undo info
            OperationFeedback op = new OperationFeedback(this);
            op.Name = "ChangeCells";
            // op.Description = Syncfusion.Windows.Controls.Grid.Resources.SR.GetString("DescriptionChangeCells", range);
            op.AllowCancel = CommandStack.IsRecording || !CommandStack.Enabled;
            op.AllowRollback = CommandStack.IsRecording;            

            try
            {
                //// bool bIsMouseAction = GetHitState() > 0;  // op has a AllowProgress setting
                GridRangeInfo intRange = range.ExpandRange(-1, -1, -1, -1);
                int dwSize = intRange.Width * intRange.Height;

               if (CommandStack.ShouldGenerateUndoInfo)
                {
                    try
                    {
                        savedCellsInfo = GetCellsInfo(intRange);
                    }
                    catch (Exception ex)
                    {
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        return false;
                    }
                }

                GridRangeInfo restoreRange = GridRangeInfo.Empty;
                //ResetVolatileData();
                try
                {
                    int cellIndex = 0;
                    int counter = 0;
                    for (int rowIndex = intRange.Top; rowIndex <= intRange.Bottom; rowIndex++)
                    {
                        for (int colIndex = intRange.Left; colIndex <= intRange.Right; colIndex++)
                        {
                            if (cellsInfo.Length > 1)
                            {
                                cellIndex = ((rowIndex - intRange.Top) * intRange.Width) + (colIndex - intRange.Left);
                            }

                            if (cellIndex >= cellsInfo.Length)
                            {
                                break;
                            }

                            GridStyleInfo cellInfo = cellsInfo[cellIndex];
                            if (cellInfo != null)
                            {
                                success |= SetCellInfo(rowIndex, colIndex, cellInfo, modifyType);
                            }
                            else
                            {
                                success |= SetCellInfo(rowIndex, colIndex, null, StyleModifyType.Remove);
                            }
                            op.PercentComplete = (int)((++counter) * 100 / dwSize);
                            if (op.ShouldCancel)
                            {
                                if (rowIndex == range.Top)
                                {
                                    restoreRange = GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, colIndex);
                                }
                                else
                                {
                                    restoreRange = GridRangeInfo.Cells(range.Top, range.Left, rowIndex, range.Right);
                                }

                                throw new ArgumentException();
                            }
                        }
                    }
                }
                catch (ArgumentException ucex)
                {
                    if (!ExceptionManager.RaiseExceptionCatched(this, ucex))
                    {
                        throw;
                    }

                    if (success && savedCellsInfo != null)
                    {
                        if (CommandStack.IsRecording)
                        {
                            CommandStack.Mode = GridCommandMode.Rollback;
                            ChangeCells(restoreRange, savedCellsInfo, StyleModifyType.Copy);
                            CommandStack.Mode = GridCommandMode.Recording;
                            savedCellsInfo = null;
                        }

                        success = false;
                    }
                }
                if (CommandStack.ShouldGenerateUndoInfo && savedCellsInfo != null)
                {
                   CommandStack.Push(new SpreadsheetGridCommand(this, range, savedCellsInfo, StyleModifyType.Copy, GridStyleInfoStore.CellValueProperty));
                }                
            }
            finally
            {
                op.Close();
            }

            return success;
        }
        
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ColumnWidths.LineHiddenChanged -= new HiddenRangeChangedEventHandler(OnColumnWidthsLineHiddenChanged);
                this.RowHeights.LineHiddenChanged -= new HiddenRangeChangedEventHandler(OnRowHeightsLineHiddenChanged);
#if !SILVERLIGHT
                this.GraphicModel.GraphicCellMoved -= new GraphicCellMovedEventHandler(GraphicModel_GraphicCellMoved);
                this.GraphicModel.GraphicCellResized -= new GraphicCellResizedEventHandler(GraphicModel_GraphicCellResized);
                this.GraphicModel.GraphicCellRemoved -= new GraphicCellRemovedEventHandler(GraphicModel_GraphicCellRemoved);
                this.GraphicModel.CurrentGraphicCellActivated -= new CurrrentGraphicCellActivatedEventHandler(GraphicModel_CurrentGraphicCellActivated);
                this.GraphicModel.CurrentGraphicCellDeactivated -= new CurrrentGraphicCellDeactivatedEventHandler(GraphicModel_CurrentGraphicCellDeactivated);
#endif
                _spreadsheetGrid = null;
                (this as GridModel).ActiveGridView = null;
                if (this.GridCopyPaste != null)
                    (this.GridCopyPaste as SpreadsheetGridCopyPaste).Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class SpreadsheetGraphicModel : GraphicModel
    {
        private SpreadsheetGridModel spreadsheetGridModel;
        public SpreadsheetGraphicModel(SpreadsheetGridModel spreadsheetGridModel)
            : base()
        {
            this.spreadsheetGridModel = spreadsheetGridModel;
        }

        protected override void OnGraphicCellRemoved(GraphicCellRemovedEventArgs e)
        {
            IWorksheet sheet = spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet;
            foreach (var spanInfo in e.RemovedGraphicCells)
            {
                ShapeImpl shape = sheet.Shapes[spanInfo.Name] as ShapeImpl;
                if (shape != null)
                    shape.Remove();
            }
            base.OnGraphicCellRemoved(e);
        }

        protected override void OnCurrrentGraphicCellActivated(CurrrentGraphicCellActivatedEventArgs e)
        {
            spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.IsFocusedOnGraphicCells = true;
            spreadsheetGridModel.ExcelProperties.spreadControl.FormulaBar.Text = string.Empty;
            spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.nameboxtext = e.CurrentSpanInfo.Name;
            spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.CellName = e.CurrentSpanInfo.Name;
            spreadsheetGridModel.ActiveGridView.InvalidateCell(GridRangeInfo.Cell(spreadsheetGridModel.ActiveGridView.CurrentCell.RowIndex,
                                   spreadsheetGridModel.ActiveGridView.CurrentCell.ColumnIndex));
            spreadsheetGridModel.ActiveGridView.InvalidateCell(GridRangeInfo.Row(0));
            spreadsheetGridModel.ActiveGridView.InvalidateCell(GridRangeInfo.Col(0));
            spreadsheetGridModel.ActiveGridView.InvalidateVisual();
            base.OnCurrrentGraphicCellActivated(e);
        }

        protected override void OnCurrrentGraphicCellDeactivated(CurrrentGraphicCellDeactivatedEventArgs e)
        {
            spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.IsFocusedOnGraphicCells = false;
            spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();
            spreadsheetGridModel.ActiveGridView.InvalidateCell(GridRangeInfo.Row(0));
            spreadsheetGridModel.ActiveGridView.InvalidateCell(GridRangeInfo.Col(0));
            spreadsheetGridModel.ActiveGridView.InvalidateVisual();
            base.OnCurrrentGraphicCellDeactivated(e);
        }
    }



    public class ExcelGridVolatileCellStyles : GridVolatileCellStyles
    {
        public ExcelGridVolatileCellStyles(IGridVolatileCellStylesHost host)
            : base(host)
        {

        }

        protected override GridStyleInfo CreateStyle(Cells.RowColumnIndex cell)
        {
            if (this.ExcelGridModel != null && this.ExcelGridModel.SperadsheetGrid != null)
            {
                SpreadsheetGridStyleInfo styleInfo = new SpreadsheetGridStyleInfo(new ExcelGridStyleInfoIdentity(this, cell));
                styleInfo.BeginInit();
                IWorksheet sheet = ExcelGridModel.ExcelProperties.WorkBook.Worksheets[this.ExcelGridModel.SheetName];
                object description = null;

                if (ExcelGridModel.Data.ContainsKey(cell) && ExcelGridModel.Data[cell] != null)
                    description = ExcelGridModel.Data[cell].GetValue(GridStyleInfoStore.DescriptionProperty);

                if (description != null && description.ToString() == "IsFormulaOnlyImported")
                    styleInfo.Description = "NeedToImport";

                if (sheet != null && cell.RowIndex > 0 && cell.ColumnIndex > 0)
                {
                    IRange range = sheet.Range;
                    styleInfo.Borders.Bottom = null;
                    styleInfo.Borders.Right = null;
                    IRange rangeToConvert = sheet.Range[cell.RowIndex, cell.ColumnIndex];
                    if ((styleInfo.Description == "NeedToImport" || ExcelGridModel.Data[cell] == null) && ExcelGridModel.ExcelProperties.spreadControl.OptimizeFormulaCalculation) 
                    {
                        ExcelGridModelImportExtensions.ConvertExcelRangeToVirtualGrid(styleInfo, sheet, rangeToConvert, null);
                        if (rangeToConvert.HorizontalAlignment == ExcelHAlign.HAlignCenterAcrossSelection)
                            ExcelGridModelImportExtensions.CopyCenterAcrossSelection(ExcelGridModel, sheet, styleInfo);

                        string s;
                        if (styleInfo.FormulaTag == null && ExcelGridModel.ExcelProperties.spreadControl.OptimizeFormulaCalculation)
                        {
                            string text = styleInfo.Text;

                            if (text != null && text.Length > 0 && text[0] == '=')
                            {
                                s = text.Substring(1);
                                try
                                {
                                    ExcelGridModel.FormulaEngine.FormulaContextCell = GridRangeInfo.GetAlphaLabel(cell.ColumnIndex) + cell.RowIndex.ToString();
                                    styleInfo.FormulaTag = new GridFormulaTag(ExcelGridModel.FormulaEngine.Parse(s), null, cell.RowIndex, cell.ColumnIndex);
                                    string cellvalue = rangeToConvert.DisplayText;
                                    if (!rangeToConvert.NumberFormat.Equals("General"))
                                    {
                                        var format = ExcelGridModelImportExtensions.GetFormat(rangeToConvert.NumberFormat);
                                        var zeroFormatedText = 0.ToString(format);
                                        string dateFormatedText = string.Empty;
                                        //if (format.ToString() != "0")
                                        //    dateFormatedText = DateTime.Now.ToString(format);
                                        if (cellvalue.Trim() == zeroFormatedText.Trim())
                                            cellvalue = "0";
                                        //if (cellvalue.Trim() == dateFormatedText.Trim())
                                        //    cellvalue = null;
                                    }
                                    GridSheetFamilyItem family = GridFormulaEngine.GetSheetFamilyItem(ExcelGridModel);
                                    var token = family.GetTokenForGridModel(ExcelGridModel);
                                    string sheettoken = token != null ? token.ToString() : string.Empty;
                                    string s1 = sheettoken + ExcelGridModel.FormulaEngine.FormulaContextCell;
                                    if (cellvalue != null && this.ExcelGridModel.ExcelProperties.ChangedCellList.Count > 0 && ExcelGridModel.FormulaEngine.DependentFormulaCells.Contains(s1))
                                    {                                        
                                        if (CheckIsDependentCellsChanged(s1))
                                            cellvalue = null;                                                                                   
                                    }
                                    styleInfo.FormulaTag.Text = cellvalue;
                                }
                                catch (Exception ex)
                                {
                                    styleInfo.FormulaTag = new GridFormulaTag(ex.Message, ex.Message, cell.RowIndex, cell.ColumnIndex);
                                }
                            }
                        }
                        styleInfo.Description = "IsImported";
                        if (ExcelGridModel.Data.ContainsKey(cell) && ExcelGridModel.Data[cell] != null)
                        {
                            object celltype = ExcelGridModel.Data[cell].GetValue(GridStyleInfoStore.CellTypeProperty);
                            if (celltype != null && (celltype.ToString() == "ImageCell" || celltype.ToString() == "Hyperlink"
                                || celltype.ToString() == "RichText" || celltype.ToString() == "SparkLineCell"))
                            {
                            }
                            else
                                ExcelGridModel.Data[cell] = styleInfo.Store;
                        }
                    }
                    else
                    {
                        ExcelGridModelImportExtensions.ConvertExcelRangeToVirtualGrid(styleInfo, sheet, rangeToConvert, null);
                        if (rangeToConvert.HorizontalAlignment == ExcelHAlign.HAlignCenterAcrossSelection)
                            ExcelGridModelImportExtensions.CopyCenterAcrossSelection(ExcelGridModel, sheet, styleInfo);

                        styleInfo.Description = "IsImported";
                        if (ExcelGridModel.Data.ContainsKey(cell) && ExcelGridModel.Data[cell] != null)
                        {
                            object celltype = ExcelGridModel.Data[cell].GetValue(GridStyleInfoStore.CellTypeProperty);
                            if (celltype != null && (celltype.ToString() == "ImageCell" || celltype.ToString() == "Hyperlink"
                                || celltype.ToString() == "RichText" || celltype.ToString() == "SparkLineCell"))
                            {
                            }
                            else
                                ExcelGridModel.Data[cell] = styleInfo.Store;

                        }
                    }
                }
                styleInfo.EndInit();
                return styleInfo;
            }
            else
                return base.CreateStyle(cell);
        }

        internal bool CheckIsDependentCellsChanged(string cell)
        {            
            Hashtable ht = (Hashtable)ExcelGridModel.FormulaEngine.DependentFormulaCells[cell];
            foreach (object o1 in ht.Keys)
            {
                string cell2 = o1 as string;
                string Sheettoken = ExcelGridModel.SperadsheetGrid.SheetToken(cell2);

                if (this.ExcelGridModel.ExcelProperties.ChangedCellList.Contains(cell2))                                    
                    return true;                
                else if (this.ExcelGridModel.FormulaEngine.DependentFormulaCells.Contains(cell2))
                {
                    if (CheckIsDependentCellsChanged(cell2))
                        return true;
                }
            }
            return false;            
        }

        public SpreadsheetGridModel ExcelGridModel
        {
            get { return this.Host as SpreadsheetGridModel; }
        }

        public int RowIndex(string s)
        {
            int i = 0;
            if (i < s.Length && s[i] == '!')
            {
                i++;
                while (i < s.Length && s[i] != '!')
                    i++;
                i++;
            }

            while (i < s.Length && char.IsLetter(s[i]))
                i++;
            if (i < s.Length)
            {
                i = int.Parse(s.Substring(i));
                return i;
            }
            return int.MinValue;
        }

        public int ColIndex(string s)
        {
            int i = 0;
            int k = 0;
            s = s.ToUpper();

            if (i < s.Length && s[i] == '!')
            {
                i++;
                while (i < s.Length && s[i] != '!')
                    i++;
                i++;
            }

            while (i < s.Length && char.IsLetter(s[i]))
            {
                k = (int)(k * 26 + (int)s[i] - (int)('A') + 1);
                i++;
            }

            return k;
        }

    }
}
