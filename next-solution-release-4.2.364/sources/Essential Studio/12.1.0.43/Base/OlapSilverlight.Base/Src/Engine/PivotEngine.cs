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
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using System.Collections;
using Syncfusion.Linq;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Engine
{
    public class PivotEngine
    {
        #region class constants
        private const string m_cSummaryText = "Total";
        private const string m_cMeasuresSuff = "Measure";
        #endregion

        public PivotEngine()
        {
            TableColumns = new List<PivotColumnDescriptor>();
            this.HeaderSection = new GridRangeInfo();
            this.RowHeaderSection = new GridRangeInfo();
            m_spanCells = new Dictionary<PivotCellDescriptor, GridRangeInfo>();

        }
        //public int RowsCount { get; set; }
        private SummaryLayout m_summaryPos = SummaryLayout.Top;
        private Dictionary<int, List<int>> m_levelsHash = null;
        private Dictionary<PivotCellDescriptor, GridRangeInfo> m_spanCells = null;

        public GridRangeInfo HeaderSection
        {
            get
            {
                return m_headerSection;
            }
            set
            {
                if (m_headerSection != value)
                {
                    m_headerSection = value;
                }
            }
        }
        public GridRangeInfo RowHeaderSection
        {
            get
            {
                return m_rowHeaderSection;
            }
            set
            {
                if (m_rowHeaderSection != value)
                {
                    m_rowHeaderSection = value;
                }
            }
        }
        private GridRangeInfo m_headerSection = GridRangeInfo.Empty;
        private GridRangeInfo m_rowHeaderSection = GridRangeInfo.Empty;

        internal Dictionary<PivotCellDescriptor, GridRangeInfo> SpanCells
        {
            get
            {
                return m_spanCells;
            }
            set
            {
                if (m_spanCells != value)
                {
                    m_spanCells = value;
                }
            }
        }


        public SummaryLayout SummaryPosition
        {
            get
            {
                return m_summaryPos;
            }
            set
            {
                if (m_summaryPos != value)
                {
                    this.TransformTable(value);
                    m_summaryPos = value;
                    //SetTotalsSigns();
                }
            }
        }

        public Dictionary<int, List<int>> LevelsHash
        {
            get
            {
                if (m_levelsHash == null)
                    this.CreateRowsLevelsHash();

                return m_levelsHash;
            }
        }

        public bool IsOLAP
        {
            get
            {
                this.CreateRowsLevelsHash();
                if (ItemSource != null)
                {
                    return false;
                }
                if (this.TableColumns.Count > 0)
                {
                    List<int> keys = new List<int>(this.LevelsHash.Keys);
                    bool isOlap = true;

                    if (keys.Count == 0)
                        isOlap = false;
                    else if (keys.Count == 1 && keys[0] == this.TableColumns.Count)
                        isOlap = false;

                    return isOlap;
                }
                return false;
            }
        }

        public List<PivotColumnDescriptor> TableColumns { get; set; }

        public object m_ItemSource;
        
        public object ItemSource
        {
            get
            {
                return m_ItemSource;
            }
            set
            {
                m_ItemSource = value;
            }
        }

        public int RowsCount
        {
            get
            {
                int count = 0;

                if (this.TableColumns.Count > 0)
                    count = this.TableColumns[0].Cells.Count;

                return count;
            }
            set
            {
            }
        }       

        internal Dictionary<int, List<int>> CreateColumnsLevelsHash()
        {
            Dictionary<int, List<int>> levelsHash = null;

            if (this.RowsCount > 0)
            {
                levelsHash = new Dictionary<int, List<int>>();
                List<int> levelCols = new List<int>();
                int summaryLevel = 0;

                for (int col = 0; col < this.TableColumns.Count; col++)
                {
                    PivotColumnDescriptor initColWrapper = this.TableColumns[0];
                    int currLevel = 0;

                    for (int row = 0; row < initColWrapper.Cells.Count; row++)
                    {
                        PivotCellDescriptor cellWrapper = this.TableColumns[col].Cells[row];

                        if (cellWrapper.CellType == PivotCellDescriptorType.ColumnHeader
                            && cellWrapper.CellValue != null && cellWrapper.CellValue != "")
                            currLevel++;
                        else
                            break;
                    }

                    if (summaryLevel == 0 && currLevel != 0)
                        summaryLevel = currLevel;

                    if (summaryLevel != 0 && currLevel != 0)
                    {
                        if (currLevel == summaryLevel)
                            levelCols.Add(col);
                        else
                        {
                            List<int> keys = new List<int>(levelsHash.Keys);

                            if (keys.Contains(summaryLevel))
                            {
                                List<int> rows = levelsHash[summaryLevel];
                                rows.AddRange(levelCols);
                            }
                            else
                                levelsHash.Add(summaryLevel, levelCols);

                            levelCols = new List<int>();
                            levelCols.Add(col);
                            summaryLevel = currLevel;
                        }
                    }
                }

                if (summaryLevel != 0)
                {
                    List<int> keys = new List<int>(levelsHash.Keys);

                    if (keys.Contains(summaryLevel))
                    {
                        List<int> rows = levelsHash[summaryLevel];
                        rows.AddRange(levelCols);
                    }
                    else
                        levelsHash.Add(summaryLevel, levelCols);
                }
            }
            else
                levelsHash = null;

            return levelsHash;
        }

        private void RecalculateRowHeaderSpans()
        {
            for (int row = 0; row < this.RowsCount; row++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(row);

                for (int cell = m_headerSection.Left; cell >= 0; cell--)
                {
                    if (cell < rowDesc.Cells.Count)
                    {
                        PivotCellDescriptor cellDesc = rowDesc.Cells[cell];

                        if (cellDesc.CellType == PivotCellDescriptorType.Any && cellDesc.SpanCell == null)
                        {
                            Dictionary<int, PivotCellDescriptor> previousCell = GetPreviousCell(rowDesc, cell);
                            if (previousCell != null)
                            {
                                PivotCellDescriptor parent = previousCell[previousCell.Keys.Max()];
                                string[] range = parent.Range.ToString().Split('-');
                                if (range.Length != 2)
                                {
                                    parent.Range = GridRangeInfo.Empty;
                                }

                                if (parent != null && parent.Range.Right == 0)
                                {
                                    parent.Range = GridRangeInfo.FromTlhw(row, cell - previousCell.Keys.Max(), 1, previousCell.Keys.Max() + 1);
                                }
                                cellDesc.SpanCell = parent;
                            }
                        }
                    }
                }
            }
        }

        public void RecalculateColumnHeaderSpans(bool ItemSource)
        {
            for (int col = 0; col < this.TableColumns.Count; col++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[col];

                for (int cell = m_headerSection.Height; cell >= 0; cell--)
                {
                    PivotCellDescriptor cellDesc = columnDesc.Cells[cell];

                    if (cellDesc.CellType == PivotCellDescriptorType.Any && cellDesc.SpanCell == null && cellDesc.CellCaption != "Span")
                    {
                        if (col != 0 && cell != 0)
                        {
                            Dictionary<int, PivotCellDescriptor> previousCell = GetPreviousCell(columnDesc, cell);
                            if (previousCell != null)
                            {
                                PivotCellDescriptor parent = previousCell[previousCell.Keys.Max()];
                                string[] range = parent.Range.ToString().Split('-');
                                if (range.Length != 2)
                                {
                                    parent.Range = GridRangeInfo.Empty;
                                }

                                if (parent != null && parent.Range.Right == 0)
                                {
                                    parent.Range = GridRangeInfo.FromTlhw(cell - previousCell.Keys.Max(), col, previousCell.Keys.Max() + 1, 1);
                                }
                                cellDesc.SpanCell = parent;
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<int, PivotCellDescriptor> GetPreviousCell(PivotColumnDescriptor columnDesc, int cell)
        {
            Dictionary<int, PivotCellDescriptor> previousCells = new Dictionary<int, PivotCellDescriptor>();
            if (cell > 0)
            {
                int count = 0;
                for (int index = cell - 1; index >= 0; index--)
                {
                    count++;
                    PivotCellDescriptor cellDesc = columnDesc.Cells[index];
                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        previousCells.Add(count, cellDesc);
                        return previousCells;
                    }
                }
                return null;
            }
            else
                return null;
        }

        private Dictionary<int, PivotCellDescriptor> GetPreviousCell(PivotRowDescriptor rowDesc, int cell)
        {
            Dictionary<int, PivotCellDescriptor> previousCells = new Dictionary<int, PivotCellDescriptor>();
            if (cell > 0)
            {
                int count = 0;
                for (int index = cell - 1; index >= 0; index--)
                {
                    count++;
                    PivotCellDescriptor cellDesc = rowDesc.Cells[index];
                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        previousCells.Add(count, cellDesc);
                        return previousCells;
                    }
                }
                return null;
            }
            else
                return null;
        }


         public void RemoveTotalsElements()
        {
            for (int colInd = 0; colInd < this.TableColumns.Count; colInd++)
            {
                PivotColumnDescriptor pcd = this.TableColumns[colInd];

                for (int rowInd = 0; rowInd < pcd.Cells.Count; rowInd++)
                {
                    PivotCellDescriptor cellDesc = pcd.Cells[rowInd];
                    Member olapMember = cellDesc.Tag as Member;

                    if (olapMember != null && olapMember.Type == MemberTypeEnum.All)
                    {
                        if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader) // .Range.Width >= cellDesc.Range.Height && !(cellDesc.Range.Width == 1 && cellDesc.Range.Height == 1))
                        {
                            this.RemoveRowAt(rowInd);
                            GridRangeInfo colHeaders = this.HeaderSection;
                            colHeaders = GridRangeInfo.FromTlhw(colHeaders.Top, colHeaders.Left
                                , colHeaders.Height - 1, colHeaders.Width);
                            this.HeaderSection = colHeaders;
                        }
                        else if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            this.TableColumns.RemoveAt(colInd);
                            GridRangeInfo rowHeaders = this.RowHeaderSection;
                            rowHeaders = GridRangeInfo.FromTlhw(rowHeaders.Top, rowHeaders.Left
                                , rowHeaders.Height, rowHeaders.Width - 1);
                            this.RowHeaderSection = rowHeaders;
                            break;
                        }
                    }
                }
            }
        }

         public void ResetSpansforExcelLayout()
         {
             foreach (PivotColumnDescriptor colDesc in this.TableColumns)
             {
                 foreach (PivotCellDescriptor cellDesc in colDesc.Cells)
                 {
                     if (cellDesc.CellType != PivotCellDescriptorType.RowHeader && cellDesc.CellType != PivotCellDescriptorType.SummaryRow && cellDesc.CellType != PivotCellDescriptorType.Any)
                     {
                         PivotCellDescriptor lastCell = colDesc.Cells[colDesc.Cells.Count - 1];
                         if (lastCell.CellType == PivotCellDescriptorType.Value)
                         {
                             if (cellDesc.Range != null && cellDesc.Range != GridRangeInfo.Empty)
                             {
                                 cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top, cellDesc.Range.Left
                                     , cellDesc.Range.Height, 1);
                             }
                             PivotCellDescriptor rangeCell = cellDesc.SpanCell;

                             if (rangeCell != null)
                             {
                                 if (GetCellLocation(rangeCell).Top == GetCellLocation(cellDesc).Top)
                                     cellDesc.SpanCell = null;
                             }
                         }
                         else
                         {
                             if (cellDesc.Range != null && cellDesc.Range != GridRangeInfo.Empty)
                             {
                                 cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top, cellDesc.Range.Left
                                     , 1, cellDesc.Range.Width);
                             }
                             PivotCellDescriptor rangeCell = cellDesc.SpanCell;

                             if (rangeCell != null)
                             {
                                 if (GetCellLocation(rangeCell).Left == GetCellLocation(cellDesc).Left)
                                     cellDesc.SpanCell = null;
                             }
                         }
                     }
                 }
             }
         }

        internal string GetCellPath(int colIndex, int rowIndex)
        {
            StringBuilder cellPath = new StringBuilder();

            for (int i = 0; i <= colIndex; i++)
            {
                PivotCellDescriptor cellDesc = this.TableColumns[i].Cells[rowIndex];
                if (cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                    cellPath.Append(cellDesc.CellValue);
            }

            return cellPath.ToString();
        }

        internal string GetCellVerticalPath(int colIndex, int rowIndex)
        {
            StringBuilder cellPath = new StringBuilder();

            for (int i = 0; i <= rowIndex; i++)
            {
                PivotCellDescriptor cellDesc = this.TableColumns[colIndex].Cells[i];
                if (cellDesc.CellType != PivotCellDescriptorType.SummaryColumn)
                    cellPath.Append(cellDesc.CellValue);
            }

            return cellPath.ToString();
        }

        public void SetSummaryRows()
        {
            this.CreateRowsLevelsHash();

            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDescriptor = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colDescriptor.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellD = colDescriptor.Cells[rowIndex];
                    if (cellD.CellType == PivotCellDescriptorType.Any)
                    {
                        if (this.HeaderSection.Contains(GridRangeInfo.FromTlhw(rowIndex, colIndex, 1, 1))
                            && cellD.CellValue == string.Empty || cellD.CellValue =="")
                            cellD.CellType = PivotCellDescriptorType.SummaryRow;
                        else
                            cellD.CellType = PivotCellDescriptorType.Value;
                    }
                }
            }
        }

        internal Dictionary<int, List<int>> CreateRowsLevelsHash()
        {
            Dictionary<int, List<int>> levelsHash = null;

            if (this.TableColumns.Count > 0)
            {
                m_levelsHash = new Dictionary<int, List<int>>();
                List<int> levelRows = new List<int>();
                PivotColumnDescriptor initColWrapper = this.TableColumns[0];
                int summaryLevel = 0;

                for (int row = 0; row < initColWrapper.Cells.Count; row++)
                {
                    int currLevel = 0;

                    for (int col = 0; col < this.TableColumns.Count; col++)
                    {
                        PivotCellDescriptor cellWrapper = this.TableColumns[col].Cells[row];

                        if (cellWrapper.CellType == PivotCellDescriptorType.RowHeader
                            && cellWrapper.CellValue != null && cellWrapper.CellValue != "")
                            currLevel++;
                        else
                            break;
                    }

                    if (summaryLevel == 0 && currLevel != 0)
                        summaryLevel = currLevel;

                    if (summaryLevel != 0 && currLevel != 0)
                    {
                        if (currLevel == summaryLevel)
                            levelRows.Add(row);
                        else
                        {
                            List<int> keys = new List<int>(m_levelsHash.Keys);

                            if (keys.Contains(summaryLevel))
                            {
                                List<int> rows = m_levelsHash[summaryLevel];
                                rows.AddRange(levelRows);
                            }
                            else
                                m_levelsHash.Add(summaryLevel, levelRows);

                            levelRows = new List<int>();
                            levelRows.Add(row);
                            summaryLevel = currLevel;
                        }
                    }
                }

                if (summaryLevel != 0)
                {
                    List<int> keys = new List<int>(m_levelsHash.Keys);

                    if (keys.Contains(summaryLevel))
                    {
                        List<int> rows = m_levelsHash[summaryLevel];
                        rows.AddRange(levelRows);
                    }
                    else
                        m_levelsHash.Add(summaryLevel, levelRows);
                }
            }
            else
                m_levelsHash = null;

            return m_levelsHash;
        }

        /// <summary>
        /// Moves row.
        /// </summary>
        /// <param name="currPosition">Current row position.</param>
        /// <param name="newPosition">New row position.</param>
        public void MoveRow(int currPosition, int newPosition)
        {
            MoveRow(currPosition, newPosition, false);
        }

        public void MoveRow(int currPosition, int newPosition, bool ignoreSpans)
        {
            if (currPosition != newPosition)
            {
                foreach (PivotColumnDescriptor colWrapper in this.TableColumns)
                {
                    PivotCellDescriptor cellWrapper = colWrapper.Cells[currPosition];
                    PivotCellDescriptor insertionCellWrapper = colWrapper.Cells[newPosition];

                    if (cellWrapper.SpanCell == null || ignoreSpans
                        || (cellWrapper.SpanCell != insertionCellWrapper && cellWrapper.SpanCell != insertionCellWrapper.SpanCell))
                    {
                        colWrapper.Cells.Remove(cellWrapper);
                        colWrapper.Cells.Insert(newPosition, cellWrapper);
                    }
                }
            }
        }


        internal void TransformTable(SummaryLayout summaryPos)
        {
            SetRowsSummaryPos(summaryPos);
            SetColumnsSummaryPos(summaryPos);

            this.ResetSpans();
            m_summaryPos = summaryPos;
            this.RecalculateSpans();
        }

        public void RemoveRowAt(int index)
        {
            if (index >= 0)
            {
                for (int i = 0; i < this.TableColumns.Count; i++)
                {
                    PivotColumnDescriptor column = this.TableColumns[i];

                    if (column.Cells.Count > index)
                        column.Cells.RemoveAt(index);
                }
            }
        }

        private void SetRowsSummaryPos(SummaryLayout summaryPos)
        {
            if (summaryPos != m_summaryPos && m_levelsHash != null)
            {
                for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
                {
                    PivotColumnDescriptor colDesc = this.TableColumns[colIndex];
                    bool rowHeaderPresent = false;

                    for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                    {
                        PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                        if (cellDesc.CellType == PivotCellDescriptorType.RowHeader
                            && cellDesc.Range.Height > 1)
                        {
                            rowHeaderPresent = true;

                            if (this.SummaryPosition != SummaryLayout.None)
                            {
                                int summaryInd = 0;
                                int endInd = 0;
                                int summaryLength = 0;
                                PivotColumnDescriptor nextColumn = this.TableColumns[colIndex + 1];

                                if (this.SummaryPosition == SummaryLayout.Top)
                                {
                                    summaryInd = rowIndex;
                                    endInd = rowIndex + cellDesc.Range.Height - 1;

                                    while (summaryLength < cellDesc.Range.Height)
                                    {
                                        PivotCellDescriptor summaryCell = nextColumn.Cells[summaryInd + summaryLength];

                                        if (summaryCell.CellType == PivotCellDescriptorType.SummaryRow)
                                            summaryLength++;
                                        else
                                            break;
                                    }
                                }
                                else if (this.SummaryPosition == SummaryLayout.Bottom)
                                {
                                    summaryInd = rowIndex + cellDesc.Range.Height - 1;
                                    endInd = rowIndex;

                                    while (summaryLength < cellDesc.Range.Height)
                                    {
                                        PivotCellDescriptor summaryCell = nextColumn.Cells[summaryInd - summaryLength];

                                        if (summaryCell.CellType == PivotCellDescriptorType.SummaryRow)
                                            summaryLength++;
                                        else
                                            break;
                                    }

                                    summaryInd = summaryInd - summaryLength;
                                }

                                if (summaryPos == SummaryLayout.None)
                                {
                                    if (summaryLength > 0 && cellDesc.Range.Height != summaryLength)
                                    {
                                        for (int i = 0; i < summaryLength; i++)
                                        {
                                            this.RemoveRowAt(summaryInd);
                                            rowIndex--;
                                        }
                                    }
                                }
                                else
                                {
                                    if (summaryLength > 0 && cellDesc.Range.Height != summaryLength)
                                        for (int i = 0; i < summaryLength; i++)
                                            this.MoveRow(summaryInd, endInd, true);
                                }

                                rowIndex += cellDesc.Range.Height - 1;
                            }
                        }
                    }

                    if (!rowHeaderPresent)
                        break;
                }
            }
        }

        public void SetColumnsSummaryPos(SummaryLayout summaryPos)
        {
            if (summaryPos != m_summaryPos)// && m_levelsHash != null)
            {
                int rowsCount = this.RowsCount;

                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                    bool colHeaderPresent = false;

                    for (int colIndex = 0; colIndex < rowDesc.Cells.Count; colIndex++)
                    {
                        PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];
                        Member olapMember = cellDesc.Tag as Member;
                        bool measures = olapMember == null ? false : olapMember.LevelUniqueName.Contains("Measure");

                        if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            colHeaderPresent = true;

                            if (cellDesc.Range.Width > 1 && !measures)
                            {
                                if (this.SummaryPosition != SummaryLayout.None)
                                {
                                    int summaryInd = 0;
                                    int endInd = 0;
                                    int summaryLength = 0;

                                    PivotRowDescriptor nextRow = this.GetRowAt(rowIndex + 1);

                                    if (this.SummaryPosition == SummaryLayout.Top)
                                    {
                                        summaryInd = colIndex;
                                        endInd = colIndex + cellDesc.Range.Width - 1;
                                        PivotCellDescriptor endCell = null;
                                        if (rowDesc.Cells.Count > endInd)
                                            endCell = rowDesc.Cells[endInd];
                                        if (endCell != null && endCell.SpanCell == cellDesc)
                                        {
                                            while (summaryLength < cellDesc.Range.Width)
                                            {
                                                PivotCellDescriptor summaryCell = nextRow.Cells[summaryInd + summaryLength];

                                                if (summaryCell.CellType == PivotCellDescriptorType.SummaryColumn)
                                                    summaryLength++;
                                                else
                                                    break;
                                            }
                                        }
                                        else
                                            summaryLength = -1;
                                    }
                                    else if (this.SummaryPosition == SummaryLayout.Bottom)
                                    {
                                        summaryInd = colIndex + cellDesc.Range.Width - 1;
                                        PivotCellDescriptor summaryCell = nextRow.Cells[summaryInd];
                                        endInd = rowIndex;
                                        if (summaryCell.CellType == PivotCellDescriptorType.SummaryColumn)
                                            summaryLength = 1;

                                        while (summaryLength < cellDesc.Range.Width)
                                        {
                                            if (summaryCell.CellType != PivotCellDescriptorType.SummaryColumn)
                                            {
                                                summaryLength--;
                                                break;
                                            }

                                            summaryCell = nextRow.Cells[summaryInd - summaryLength];
                                            summaryLength++;
                                        }

                                        summaryInd = summaryInd - summaryLength;
                                    }

                                    if (summaryPos == SummaryLayout.None)
                                    {
                                        if (summaryLength > 0 && cellDesc.Range.Width != summaryLength)
                                        {
                                            // Include to fix the Exapnded White cell issue
                                            for (int i = this.HeaderSection.Height - 1; i >= 0; i--)
                                            {
                                                int currentRowIndex = i;
                                                PivotRowDescriptor updateRowDesc = this.GetRowAt(currentRowIndex);
                                                PivotCellDescriptor summaryCell = updateRowDesc.Cells[summaryInd];
                                                if (summaryCell.SpanCell == null)
                                                {
                                                    if (updateRowDesc.Cells.Count > summaryInd)
                                                    {
                                                        PivotCellDescriptor nextCell = updateRowDesc.Cells[summaryInd + summaryLength];
                                                        if (nextCell.SpanCell == summaryCell)
                                                        {
                                                            nextCell.Range = new GridRangeInfo(summaryCell.Range.Top, summaryCell.Range.Left, summaryCell.Range.Bottom, (summaryCell.Range.Right == 0) ? 0 : summaryCell.Range.Right - 1);
                                                            nextCell.SpanCell = null;
                                                            for (int j = 2; j < nextCell.Range.Right; j++)
                                                            {
                                                                if (updateRowDesc.Cells.Count > summaryInd + j)
                                                                {
                                                                    updateRowDesc.Cells[summaryInd + j].SpanCell = nextCell;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            // -----

                                            for (int i = 0; i < summaryLength; i++)
                                            {
                                                this.TableColumns.RemoveAt(summaryInd);
                                                rowIndex--;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (summaryLength > 0 && cellDesc.Range.Width != summaryLength)
                                            for (int i = 0; i < summaryLength; i++)
                                            {
                                                PivotColumnDescriptor movingColumn = this.TableColumns[summaryInd];
                                                this.TableColumns.RemoveAt(summaryInd);
                                                this.TableColumns.Insert(endInd, movingColumn);
                                            }
                                    }

                                    colIndex += cellDesc.Range.Width - 1;
                                }
                            }
                        }
                    }

                    if (!colHeaderPresent)
                        break;
                }
            }
        }

        public virtual PivotValueCellData GetCellDataValue(int row, int column)
        {
            PivotValueCellData cellData = null;

            List<string> rowsData = new List<string>();
            List<string> columnsData = new List<string>();
            cellData = new PivotValueCellData();
            PivotColumnDescriptor colDesc = null;
            colDesc = this.TableColumns[column];

            PivotRowDescriptor rowDesc = this.GetRowAt(row);
            PivotCellDescriptor originalCell = colDesc.Cells[row];

            for (int colIndex = 0; colIndex < column; colIndex++)
            {
                PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];

                if (cellDesc.CellType == PivotCellDescriptorType.RowHeader && cellDesc.Tag != null)
                {
                    Member _member = cellDesc.Tag as Member;
                    if (_member.Type != MemberTypeEnum.All)
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                        else
                            rowsData.Add(cellDesc.CellValue);
                    }
                    else
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                    }
                }
            }

            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader && cellDesc.Tag != null)
                {
                    Member _member = cellDesc.Tag as Member;
                    if (_member.Type != MemberTypeEnum.All)
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                        else
                            columnsData.Add(cellDesc.CellValue);
                    }
                    else
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                    }
                }
            }

            cellData.Rows = rowsData;
            cellData.Columns = columnsData;
            cellData.Value = originalCell.CellValue;

            return cellData;
        }

        /// <summary>
        /// Gets the cell data valuefor I enumerable.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public virtual PivotValueCellData GetCellDataValueforIEnumerable(int row, int column)
        {
            PivotValueCellData cellData = null;

            if (row > 0 && column > 0
                && this.TableColumns.Count > column && this.RowsCount > row)
            {
                List<string> rowsData = new List<string>();
                List<string> columnsData = new List<string>();
                cellData = new PivotValueCellData();
                PivotColumnDescriptor colDesc = null;
                colDesc = this.TableColumns[column];

                PivotRowDescriptor rowDesc = this.GetRowAt(row);
                PivotCellDescriptor originalCell = colDesc.Cells[row];

                for (int colIndex = 0; colIndex < column; colIndex++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                        else
                            rowsData.Add(cellDesc.CellValue);
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                    }
                }

                for (int rowIndex = 0; rowIndex < row; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                        else
                            columnsData.Add(cellDesc.CellValue);
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                    }
                }

                cellData.Rows = rowsData;
                cellData.Columns = columnsData;
                cellData.Value = originalCell.CellValue;
            }

            return cellData;
        }

        public void ResetSpans()
        {
            foreach (PivotColumnDescriptor colDesc in this.TableColumns)
            {
                foreach (PivotCellDescriptor cellDesc in colDesc.Cells)
                {
                    PivotCellDescriptor lastCell = colDesc.Cells[colDesc.Cells.Count - 1];
                    if (lastCell.CellType == PivotCellDescriptorType.Value)
                    {
                        if (cellDesc.Range != null && cellDesc.Range != GridRangeInfo.Empty)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top, cellDesc.Range.Left
                                , cellDesc.Range.Height, 1);
                        }
                        PivotCellDescriptor rangeCell = cellDesc.SpanCell;

                        if (rangeCell != null)
                        {
                            if (GetCellLocation(rangeCell).Top == GetCellLocation(cellDesc).Top)
                                cellDesc.SpanCell = null;
                        }
                    }
                    else
                    {
                        if (cellDesc.Range != null && cellDesc.Range != GridRangeInfo.Empty)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top, cellDesc.Range.Left
                                , 1, cellDesc.Range.Width);
                        }
                        PivotCellDescriptor rangeCell = cellDesc.SpanCell;

                        if (rangeCell != null)
                        {
                            if (GetCellLocation(rangeCell).Left == GetCellLocation(cellDesc).Left)
                                cellDesc.SpanCell = null;
                        }
                    }
                }
            }
        }

        public GridRangeInfo GetCellLocation(PivotCellDescriptor cellDesc)
        {
            GridRangeInfo location = GridRangeInfo.FromTlhw(-1, -1, 0, 0);

            if (cellDesc != null)
            {
                for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
                {
                    PivotColumnDescriptor columnDesc = this.TableColumns[colIndex];
                    int rowIndex = columnDesc.Cells.IndexOf(cellDesc);

                    if (rowIndex != -1)
                    {
                        location = GridRangeInfo.FromTlhw(rowIndex, colIndex, 1, 1);
                        break;
                    }
                }
            }

            return location;
        }

        public void RecalculateSpans()
        {
            if (this.ItemSource == null)
            {
                if (IsOLAP)
                    ClearTotalsSigns();

                RecalculateSpans(true);
                if (IsOLAP)
                    SetTotalsSigns();
            }
            else
            {
                this.RecalculateRowHeaderSpans();
                this.RecalculateColumnHeaderSpans(true);
            }
        }

        public void RecalculateSpans(bool resetHeader)
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[colIndex];
                if (columnDesc.Cells[columnDesc.Cells.Count - 1].CellType == PivotCellDescriptorType.Value)
                    break;
                PivotCellDescriptor summaryCell = null;
                int colSpan = 1;

                for (int rowIndex = 0; rowIndex < columnDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = columnDesc.Cells[rowIndex];
                    string summaryPath = string.Empty;
                    GridRangeInfo location = GridRangeInfo.Empty;
                    int width = 1;

                    if (summaryCell != null)
                    {
                        location = this.GetCellLocation(summaryCell);
                        summaryPath = this.GetCellPath(location.Left, location.Top);

                        if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                            width = summaryCell.Range.Width;
                    }
                    string currPath = this.GetCellPath(colIndex, rowIndex);

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader
                        || (cellDesc.CellType == PivotCellDescriptorType.SummaryRow && cellDesc.SpanCell == null))
                    {
                        if (summaryCell == null || currPath != summaryPath)
                        {
                            if (summaryCell != null)
                                summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);

                            summaryCell = cellDesc;
                            summaryCell.SpanCell = null;
                            colSpan = 1;
                        }
                        else
                        {
                            if (summaryCell.CellValue == cellDesc.CellValue && summaryPath == currPath)
                            {
                                cellDesc.SpanCell = summaryCell;
                                colSpan++;
                            }
                        }
                    }
                    else
                    {
                        if (summaryCell != null && summaryCell.CellValue != cellDesc.CellValue)
                        {
                            if (colSpan > 1)
                                summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);

                            summaryCell = null;
                        }

                        if (cellDesc.SpanCell != null)
                        {
                            bool cellPresent = false;

                            foreach (PivotColumnDescriptor columns in this.TableColumns)
                                foreach (PivotCellDescriptor cell in columns.Cells)
                                    if (cell == cellDesc.SpanCell)
                                        cellPresent = true;

                            if (!cellPresent && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                            {
                                cellDesc.SpanCell = null;

                                foreach (PivotCellDescriptor cellDescriptor in m_spanCells.Keys)
                                {
                                    GridRangeInfo range = m_spanCells[cellDescriptor];
                                    if (range.Contains(GridRangeInfo.FromTlhw(rowIndex, colIndex, 1, 1)))
                                    {
                                        cellDesc.SpanCell = cellDescriptor;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (cellDesc.Range.RangeType   != GridRangeInfoType.Empty
                            && cellDesc.SpanCell == null && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                        {
                            GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(rowIndex, colIndex
                                , cellDesc.Range.Height, cellDesc.Range.Width);
                            if (!m_spanCells.Keys.Contains(cellDesc))
                                m_spanCells.Add(cellDesc, rangeInfo);
                        }
                    }
                }

                if (summaryCell != null && colSpan > 1)
                {
                    int width = 1;
                    if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                        width = summaryCell.Range.Width;

                    summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);
                }
            }

            if (resetHeader)
                RecalculateColumnHeaderSpans();
        }

        public void RecalculateColumnHeaderSpans()
        {
            if (m_headerSection != GridRangeInfo.Empty && !m_headerSection.IsEmpty)
            {
                for (int rowIndex = m_headerSection.Top; rowIndex < m_headerSection.Height; rowIndex++)
                {
                    PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                    PivotCellDescriptor summaryCell = null;
                    string spanHeaderPath = string.Empty;
                    int colSpan = 1;

                    for (int cellIndex = m_headerSection.Left; cellIndex < rowDesc.Cells.Count; cellIndex++)
                    {
                        PivotCellDescriptor cellDesc = rowDesc.Cells[cellIndex];
                        string currentPath = GetCellVerticalPath(cellIndex, rowIndex);
                        GridRangeInfo location = GridRangeInfo.Empty;
                        string summaryPath = string.Empty;
                        int height = 1;

                        if (summaryCell != null)
                        {
                            location = this.GetCellLocation(summaryCell);
                            summaryPath = this.GetCellPath(location.Left, location.Top);

                            if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                                height = summaryCell.Range.Height;
                        }
                        if (cellDesc.SpanCell == null || cellDesc.CellType != PivotCellDescriptorType.SummaryColumn)
                        {
                            if (summaryCell == null)
                            {
                                summaryCell = cellDesc;
                                spanHeaderPath = GetCellVerticalPath(cellIndex, rowIndex);
                                summaryCell.SpanCell = null;
                                colSpan = 1;
                            }
                            else if (((summaryCell.CellValue == cellDesc.CellValue && cellDesc.UniqueName.Contains("Measures"))
                                || currentPath == spanHeaderPath))
                            {
                                cellDesc.SpanCell = summaryCell;
                                colSpan++;
                            }
                            else
                            {
                                if (summaryCell != null && colSpan > 1)
                                    summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, height, colSpan);

                                summaryCell = cellDesc;
                                spanHeaderPath = GetCellVerticalPath(cellIndex, rowIndex);
                                colSpan = 1;
                            }
                        }
                    }

                    if (summaryCell != null && colSpan > 1)
                    {
                        int height = 1;
                        if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                            height = summaryCell.Range.Height;

                        summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, height, colSpan);
                    }
                }
            }
        }

        internal void ClearTotalsSigns()
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow
                        || cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                        cellDesc.CellValue = string.Empty;

                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        //cellDesc.CellValue = string.Empty;
                    }
                        
                }
            }
        }

        public void SetTotalsSigns()
        {
            SetTotalsSigns(true);
        }

        public void SetTotalsSigns(bool setTotalStyles)
        {
            int colHeaderStart = 0;
            PivotCellDescriptor totalCell = null;
            PivotCellDescriptor cellToClear = null;
            string type = PivotCellDescriptorExType.SummaryRow.ToString();

            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];
                PivotCellDescriptor prevCell = null;

                if (colDesc.Cells[colDesc.Cells.Count - 1].CellType == PivotCellDescriptorType.Value)
                {
                    colHeaderStart = colIndex;
                    break;
                }

                for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                    if (cellDesc.SpanCell == null)
                    {
                        totalCell = null;
                        cellToClear = null;

                        if (prevCell != null)
                        {
                            if (this.SummaryPosition == SummaryLayout.Bottom)
                            {
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.RowHeader
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (this.SummaryPosition == SummaryLayout.Top)
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryRow)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader
                                        && cellDesc.Tag is Member)
                                        totalCell = prevCell;
                                    else
                                        cellToClear = prevCell;
                                }
                            }

                            PivotCellDescriptor maintenanceCell = totalCell == null ? cellToClear : totalCell;

                            if (maintenanceCell != null)
                            {
                                if (maintenanceCell == totalCell)
                                    maintenanceCell.CellValue = m_cSummaryText;
                                else
                                    maintenanceCell.CellValue = string.Empty;

                                int totalRowIndex = rowIndex;
                                if (prevCell == maintenanceCell)
                                    totalRowIndex -= prevCell.Range != null ? prevCell.Range.Height : 1;
                                PivotCellDescriptor currentCell = null;

                                if (setTotalStyles)
                                    for (int rowNumber = totalRowIndex; rowNumber < totalRowIndex + maintenanceCell.Range.Height; rowNumber++)
                                    {
                                        PivotRowDescriptor rowDesc = this.GetRowAt(rowNumber);

                                        for (int colCellIndex = colIndex; colCellIndex < rowDesc.Cells.Count; colCellIndex++)
                                        {
                                            currentCell = rowDesc.Cells[colCellIndex];

                                            if (!currentCell.CellExTypes.Contains(type))
                                            {
                                                if (maintenanceCell == totalCell && currentCell.CellType == PivotCellDescriptorType.Value)
                                                    currentCell.CellExTypes.Add(type);
                                            }
                                            else
                                                if (maintenanceCell == cellToClear)
                                                    maintenanceCell.CellExTypes.Remove(type);
                                        }
                                    }
                            }
                        }

                        prevCell = cellDesc;
                    }
                }
            }

            int rowsCount = this.RowsCount;
            type = PivotCellDescriptorExType.SummaryColumn.ToString();

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                PivotCellDescriptor prevCell = null;

                for (int colIndex = colHeaderStart; colIndex < rowDesc.Cells.Count; colIndex++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];
                    if (cellDesc.SpanCell == null)
                    {
                        totalCell = null;
                        cellToClear = null;

                        if (prevCell != null)
                        {
                            if (this.SummaryPosition == SummaryLayout.Bottom)
                            {
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.ColumnHeader
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (this.SummaryPosition == SummaryLayout.Top)
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader
                                        && cellDesc.Tag is Member)
                                        totalCell = prevCell;
                                    else
                                        cellToClear = prevCell;
                                }
                            }

                            PivotCellDescriptor maintnanceCell = totalCell == null ? cellToClear : totalCell;

                            if (maintnanceCell != null)
                            {
                                if (maintnanceCell == totalCell)
                                    maintnanceCell.CellValue = m_cSummaryText;
                                else
                                    maintnanceCell.CellValue = string.Empty;

                                int totalColIndex = colIndex;
                                if (maintnanceCell == prevCell)
                                    totalColIndex -= prevCell.Range != null ? prevCell.Range.Width : 1;
                                PivotCellDescriptor currentCell = null;

                                if (setTotalStyles)
                                    for (int colNumber = totalColIndex; colNumber < totalColIndex + maintnanceCell.Range.Width; colNumber++)
                                    {
                                        PivotColumnDescriptor colDesc = this.TableColumns[colNumber];

                                        for (int rowCellIndex = rowIndex; rowCellIndex < colDesc.Cells.Count; rowCellIndex++)
                                        {
                                            currentCell = colDesc.Cells[rowCellIndex];

                                            if (!currentCell.CellExTypes.Contains(type))
                                            {
                                                if (maintnanceCell == totalCell && currentCell.CellType == PivotCellDescriptorType.Value)
                                                    currentCell.CellExTypes.Add(type);
                                            }
                                            else
                                                if (maintnanceCell == cellToClear)
                                                    currentCell.CellExTypes.Remove(type);
                                        }
                                    }
                            }
                        }

                        prevCell = cellDesc;
                    }
                }
            }
        }

        public void SetTotalsSigns(string summaryPos)
        {
            int colHeaderStart = 0;
            PivotCellDescriptor totalCell = null;
            PivotCellDescriptor cellToClear = null;
            string type = PivotCellDescriptorExType.SummaryRow.ToString();
            int rowsCount = this.RowsCount;
            type = PivotCellDescriptorExType.SummaryColumn.ToString();

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                PivotCellDescriptor prevCell = null;

                for (int colIndex = colHeaderStart; colIndex < rowDesc.Cells.Count; colIndex++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];
                    if (cellDesc.SpanCell == null)
                    {
                        totalCell = null;
                        cellToClear = null;

                        if (prevCell != null)
                        {
                            if (summaryPos == SummaryLayout.Bottom.ToString())
                            {
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.ColumnHeader
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (summaryPos == SummaryLayout.Top.ToString())
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader
                                        && cellDesc.Tag is Member)
                                        totalCell = prevCell;
                                    else
                                        cellToClear = prevCell;
                                }
                            }

                            PivotCellDescriptor maintnanceCell = totalCell == null ? cellToClear : totalCell;

                            if (maintnanceCell != null)
                            {
                                if (maintnanceCell == totalCell)
                                    maintnanceCell.CellValue = m_cSummaryText;
                                else
                                    maintnanceCell.CellValue = string.Empty;

                                int totalColIndex = colIndex;
                                if (maintnanceCell == prevCell)
                                    totalColIndex -= prevCell.Range != null ? prevCell.Range.Width : 1;
                                PivotCellDescriptor currentCell = null;

                                //if (setTotalStyles)
                                for (int colNumber = totalColIndex; colNumber < totalColIndex + maintnanceCell.Range.Width; colNumber++)
                                {
                                    if (colNumber < this.TableColumns.Count)
                                    {
                                        PivotColumnDescriptor colDesc = this.TableColumns[colNumber];

                                        for (int rowCellIndex = rowIndex; rowCellIndex < colDesc.Cells.Count; rowCellIndex++)
                                        {
                                            currentCell = colDesc.Cells[rowCellIndex];

                                            if (!currentCell.CellExTypes.Contains(type))
                                            {
                                                if (maintnanceCell == totalCell && currentCell.CellType == PivotCellDescriptorType.Value)
                                                    currentCell.CellExTypes.Add(type);
                                            }
                                            else
                                                if (maintnanceCell == cellToClear)
                                                    currentCell.CellExTypes.Remove(type);
                                        }
                                    }
                                }
                            }
                        }

                        prevCell = cellDesc;
                    }
                }
            }
        }

        internal void InsertAdditionalRows(IEnumerable iEnumerable, string columnName, int col, int row, SummaryInfo[] summaryInfos, GridLayout gridLayout, bool IListSource)
        {
            if (col - 1 >= 0)
            {

                this.RowHeaderSection = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top, this.RowHeaderSection.Left, this.RowHeaderSection.Height + iEnumerable.AsQueryable().Count(), this.RowHeaderSection.Width);
                PivotCellDescriptor parentCell = this.TableColumns[col - 1].Cells[row];
                if (parentCell.CellType != PivotCellDescriptorType.Value)
                    parentCell.Range = GridRangeInfo.FromTlhw(parentCell.Range.Top + row, parentCell.Range.Left + col - 1, parentCell.Range.Height + iEnumerable.AsQueryable().Count(), parentCell.Range.Width);

                for (int column = 0; column < this.TableColumns.Count; column++)
                {
                    PivotCellDescriptor childCell = null;
                    for (int cellCount = 0; cellCount < iEnumerable.AsQueryable().Count(); cellCount++)
                    {
                        if (gridLayout != GridLayout.ExcelLikeLayout)
                        {
                            if (column < col)
                            {
                                childCell = new PivotCellDescriptor();
                                if (this.TableColumns[column].Cells[row].SpanCell == null)
                                {
                                    PivotCellDescriptor baseCell = this.TableColumns[column].Cells[row];

                                    if (baseCell.CellType != PivotCellDescriptorType.Value)
                                    {
                                        childCell.SpanCell = this.TableColumns[column].Cells[row];

                                        if (column + 1 != col)
                                        {
                                            baseCell.Range = GridRangeInfo.FromTlhw(row, column, baseCell.Range.Height + 1, baseCell.Range.Width);

                                            for (int c = row + 1; c < this.TableColumns[column].Cells.Count; c++)
                                            {
                                                PivotCellDescriptor cell = this.TableColumns[column].Cells[c];
                                                if (cell.Range.ToString().Contains('-'))
                                                {

                                                    cell.Range = GridRangeInfo.FromTlhw(cell.Range.Top + 1, cell.Range.Left, cell.Range.Height, cell.Range.Width);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    PivotCellDescriptor baseCell = this.TableColumns[column].Cells[row].SpanCell;

                                    if (baseCell.CellType != PivotCellDescriptorType.Value)
                                    {
                                        childCell.SpanCell = this.TableColumns[column].Cells[row].SpanCell;

                                        if (column + 1 != col)
                                        {
                                            baseCell.Range = GridRangeInfo.FromTlhw(baseCell.Range.Top, column, baseCell.Range.Height + 1, baseCell.Range.Width);

                                            for (int c = row + 1; c < this.TableColumns[column].Cells.Count; c++)
                                            {
                                                PivotCellDescriptor cell = this.TableColumns[column].Cells[c];
                                                if (cell.Range.ToString().Contains('-'))
                                                {
                                                    cell.Range = GridRangeInfo.FromTlhw(cell.Range.Top + 1, cell.Range.Left, cell.Range.Height, cell.Range.Width);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                childCell = new PivotCellDescriptor();
                                string summaryElement = GetSummaryElement(this.TableColumns[column]);
                                if (summaryElement != null)
                                {
                                    childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                    childCell.CellType = PivotCellDescriptorType.Value;
                                    childCell.UniqueName = summaryElement + "." + childCell.CellValue;
                                }
                            }
                        }
                        if (gridLayout == GridLayout.ExcelLikeLayout)
                        {
                            if (column < col)
                            {
                                childCell = new PivotCellDescriptor();
                            }
                            else
                            {
                                childCell = new PivotCellDescriptor();
                                string summaryElement = GetSummaryElement(this.TableColumns[column]);

                                childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                childCell.CellType = PivotCellDescriptorType.Value;
                                childCell.UniqueName = summaryElement + "." + childCell.CellValue;
                            }
                        }

                        if (childCell.CellType == PivotCellDescriptorType.Any)
                        {

                            string summaryElement = GetSummaryElement(this.TableColumns[column]);
                            if (summaryElement != null)
                            {
                                childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                childCell.CellType = PivotCellDescriptorType.Value;
                                childCell.UniqueName = summaryElement + "." + childCell.CellValue;
                            }
                        }
                        this.TableColumns[column].Cells.Insert(row + cellCount + 1, childCell);

                    }
                }

                if (gridLayout != GridLayout.NoSummaries && gridLayout != GridLayout.ExcelLikeLayout)
                {
                    PivotRowDescriptor rowDesc = this.GetRowAt(row);
                    foreach (PivotCellDescriptor item in rowDesc.Cells)
                    {
                        item.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                    }
                }
            }
        }

        private static string GetSummaryElement(PivotColumnDescriptor pivotColumnDescriptor)
        {
            foreach (PivotCellDescriptor item in pivotColumnDescriptor.Cells)
            {
                if (item.Tag != null)
                {
                    SummaryInfo sumInfo = item.Tag as SummaryInfo;
                    if (sumInfo != null)
                    {
                        return sumInfo.Key;
                    }
                }
            }
            return null;
        }

        internal void ClearTotalsSignsforExcelLayout()
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];
                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                        cellDesc.CellValue = string.Empty;
                }
            }
        }

        public void ClearLevelHeadersArea()
        {
            PivotColumnDescriptor lastColumn = this.TableColumns[this.TableColumns.Count - 1];
            PivotRowDescriptor lastRow = this.GetRowAt(this.RowsCount - 1);

            int rowHeaderDepth = 0, colHeaderDepth = 0;
            if (this.RowHeaderSection != GridRangeInfo.Empty)
                rowHeaderDepth = this.RowHeaderSection.Width;
            if (this.HeaderSection != GridRangeInfo.Empty)
                colHeaderDepth = this.HeaderSection.Height;

            PivotCellDescriptor cellDesc = null;
            //PivotCellDescriptor cellDesc = lastColumn.Cells[colHeaderDepth];
            //while (cellDesc.CellType != PivotCellDescriptorType.Value)
            //    cellDesc = lastColumn.Cells[++colHeaderDepth];

            //cellDesc = lastRow.Cells[rowHeaderDepth];
            //while (cellDesc.CellType != PivotCellDescriptorType.Value)
            //    cellDesc = lastRow.Cells[++rowHeaderDepth];

            for (int colIndex = 0; colIndex < rowHeaderDepth; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colHeaderDepth; rowIndex++)
                {
                    PivotCellDescriptor topLeftCell = colDesc.Cells[rowIndex];

                    if (rowIndex == 0 && colIndex == 0)
                    {
                        cellDesc = topLeftCell;
                        cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, colHeaderDepth, rowHeaderDepth);
                        cellDesc.CellValue = string.Empty;
                        cellDesc.Value = string.Empty;
                        cellDesc.CellType = PivotCellDescriptorType.Any;
                        cellDesc.SpanCell = null;
                    }
                    else
                        topLeftCell.SpanCell = cellDesc;
                }
            }
        }

        public static PivotEngine CreateEngine(int rowCount, int colCount)
        {
            PivotEngine engine = new PivotEngine();

            for (int col = 0; col < colCount; col++)
            {
                PivotColumnDescriptor column = new PivotColumnDescriptor();

                for (int row = 0; row < rowCount; row++)
                    column.Cells.Add(new PivotCellDescriptor());

                engine.TableColumns.Add(column);
            }

            return engine;
        }

        public PivotCellDescriptor this[int rowIndex, int columnIndex]
        {
            get
            {
                return this.TableColumns[columnIndex].Cells[rowIndex];
            }
        }

        public static PivotEngine DefaultTable
        {
            get
            {
                PivotEngine table = new PivotEngine();
                table.RowsCount = 4;
                for (int col = 0; col < 3; col++)
                {
                    PivotColumnDescriptor colDesc = new PivotColumnDescriptor();

                    for (int row = 0; row < 4; row++)
                    {
                        PivotCellDescriptor cellDesc = new PivotCellDescriptor();

                        if (row == 0)
                        {
                            cellDesc.CellValue = col == 2 ? "Measures" : "Level" + ((int)(col + 1)).ToString();
                            cellDesc.CellType = PivotCellDescriptorType.ColumnHeader;
                        }
                        else if (col == 0)
                        {
                            cellDesc.CellValue = "Level1 Member1";
                            cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (col == 1)
                        {
                            if (row != 3)
                            {
                                cellDesc.CellValue = "Level2 Member" + row.ToString();
                                cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                            }
                        }
                        else
                        {
                            if (row != 3)
                                cellDesc.CellValue = table.TableColumns[1].Cells[row].CellValue + " Measure";
                            else
                                cellDesc.CellValue = table.TableColumns[0].Cells[1].CellValue + " Measure";

                            cellDesc.CellType = PivotCellDescriptorType.Value;
                        }
                        colDesc.Cells.Add(cellDesc);
                    }

                    table.TableColumns.Add(colDesc);
                }

                return table;
            }
        }

        public PivotEngine RevertTransform()
        {
            PivotEngine transformedEngine = new PivotEngine();
            transformedEngine.SummaryPosition = this.SummaryPosition;
            transformedEngine.HeaderSection = this.HeaderSection;

            for (int rowIndex = 0; rowIndex < this.RowsCount; rowIndex++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                PivotColumnDescriptor colDesc = new PivotColumnDescriptor();

                foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                {
                    if (cellDesc.Range.Height > 1 || cellDesc.Range.Width > 1)
                        cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Left, cellDesc.Range.Top
                            , cellDesc.Range.Width, cellDesc.Range.Height);

                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                        cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                    else if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                        cellDesc.CellType = PivotCellDescriptorType.ColumnHeader;
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                        cellDesc.CellType = PivotCellDescriptorType.SummaryColumn;
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                        cellDesc.CellType = PivotCellDescriptorType.SummaryRow;

                    colDesc.Cells.Add(cellDesc);
                }

                transformedEngine.TableColumns.Add(colDesc);
            }

            return transformedEngine;
        }

        public KpiInfoCollection GetValidKpis()
        {
            KpiInfoCollection Kpis = new KpiInfoCollection();
            Kpis = this.GetKpis();
            int validKpiCount = Kpis.Count;
            for (int i = (validKpiCount - 1); i >= 0; i--)
            {
                KpiInfo kpiInfo = Kpis[i];
                if (!kpiInfo.IsValidKpi)
                {
                    Kpis.RemoveAt(i);
                }
            }
            return Kpis;
        }

        private KpiInfoCollection MergeKpiRowsWithColumns(KpiInfoCollection kpiInfoCollection)
        {
            KpiInfoCollection kpis = new KpiInfoCollection();
            KpiAxisType kpiAxis = KpiAxisType.None;
            int rowsCount = 0;
            int columnsCount = 0;
            bool breakloop = false;
            kpiAxis = this.GetKpiAxis();
            if (kpiAxis != KpiAxisType.None)
            {
                if (kpiAxis == KpiAxisType.Column)
                {
                    rowsCount = this.RowsCount;
                    columnsCount = this.TableColumns.Count;
                }
                else if (kpiAxis == KpiAxisType.Row)
                {
                    rowsCount = this.TableColumns.Count;
                    columnsCount = this.RowsCount;
                }
            }
            else
            {
                return kpis;
            }
            for (int i = 0; i < rowsCount; i++)
            {
                int j;
                KpiInfo tempKpiInfo = new KpiInfo();
                for (j = 0; j < columnsCount; j++)
                {
                    PivotCellDescriptor cellDescriptor;
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        cellDescriptor = this.TableColumns[j].Cells[i];
                    }
                    else
                    {
                        cellDescriptor = this.TableColumns[i].Cells[j];
                    }

                    #region Traversing Rows
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader && cellDescriptor.Level > 0)
                        {
                            if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                            {
                                tempKpiInfo.MemberName = this.TableColumns[j].Cells[i].CellCaption;
                            }
                            else
                            {
                                tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + this.TableColumns[j].Cells[i].CellCaption;
                            }
                            tempKpiInfo.MemberRowIndex = i;
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any ||
                                cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                        {
                            break;
                        }
                    }
                    #endregion

                    #region Traversing Rows
                    if (kpiAxis == KpiAxisType.Row)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && cellDescriptor.Level > 0)
                        {
                            if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                            {
                                tempKpiInfo.MemberName = this.TableColumns[i].Cells[j].CellCaption;
                            }
                            else
                            {
                                tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + this.TableColumns[i].Cells[j].CellCaption;
                            }
                            tempKpiInfo.MemberRowIndex = i;
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any ||
                                cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                        {
                            break;
                        }
                    }
                    #endregion

                }
                if (!breakloop)
                {
                    for (int k = 0; k < kpiInfoCollection.Count; k++)
                    {
                        if (kpiAxis == KpiAxisType.Column)
                        {
                            KpiInfo kpiInfo = new KpiInfo(kpiInfoCollection[k]);
                            if (kpiInfo.GoalIndex >= 0)
                            {
                                kpiInfo.ActualGoalValue = this.TableColumns[kpiInfo.GoalIndex].Cells[i].CellValue;
                                kpiInfo.GoalValue = this.TableColumns[kpiInfo.GoalIndex].Cells[i].Value;
                            }
                            if (kpiInfo.MemberName == null || kpiInfo.MemberName == string.Empty)
                            {
                                kpiInfo.MemberName = tempKpiInfo.MemberName;
                            }
                            else
                            {
                                kpiInfo.MemberName = kpiInfo.MemberName + " - " + tempKpiInfo.MemberName;
                            }
                            if (kpiInfo.ValueIndex >= 0)
                            {
                                kpiInfo.MeasureValue = this.TableColumns[kpiInfo.ValueIndex].Cells[i].Value;
                                kpiInfo.ActualMeasureValue = this.TableColumns[kpiInfo.ValueIndex].Cells[i].CellValue;
                            }
                            if (kpiInfo.StatusIndex >= 0)
                            {
                                kpiInfo.StatusValue = this.TableColumns[kpiInfo.StatusIndex].Cells[i].CellValue != string.Empty ? int.Parse(this.TableColumns[kpiInfo.StatusIndex].Cells[i].CellValue) : kpiInfo.StatusValue;
                            }
                            if (kpiInfo.TrendIndex >= 0)
                            {
                                kpiInfo.TrendValue = this.TableColumns[kpiInfo.TrendIndex].Cells[i].CellValue != string.Empty ? int.Parse(this.TableColumns[kpiInfo.TrendIndex].Cells[i].CellValue) : kpiInfo.TrendValue;
                            }
                            kpiInfo.MemberRowIndex = tempKpiInfo.MemberRowIndex;
                            if (!kpis.Contains(kpiInfo) && kpiInfo.Kpi_Name != string.Empty)
                            {
                                kpis.Add(kpiInfo);
                            }
                        }
                        else if (kpiAxis == KpiAxisType.Row)
                        {
                            KpiInfo kpiInfo = new KpiInfo(kpiInfoCollection[k]);
                            if (kpiInfo.GoalIndex >= 0)
                            {
                                kpiInfo.ActualGoalValue = this.TableColumns[i].Cells[kpiInfo.GoalIndex].CellValue;
                                kpiInfo.GoalValue = this.TableColumns[i].Cells[kpiInfo.GoalIndex].Value;
                            }
                            if (kpiInfo.MemberName == null || kpiInfo.MemberName == string.Empty)
                            {
                                kpiInfo.MemberName = tempKpiInfo.MemberName;
                            }
                            else
                            {
                                kpiInfo.MemberName = kpiInfo.MemberName + " - " + tempKpiInfo.MemberName;
                            }
                            if (kpiInfo.ValueIndex >= 0)
                            {
                                kpiInfo.MeasureValue = this.TableColumns[i].Cells[kpiInfo.ValueIndex].Value;
                                kpiInfo.ActualMeasureValue = this.TableColumns[i].Cells[kpiInfo.ValueIndex].CellValue;
                            }
                            if (kpiInfo.StatusIndex >= 0)
                            {
                                kpiInfo.StatusValue = this.TableColumns[i].Cells[kpiInfo.StatusIndex].CellValue != string.Empty ? int.Parse(this.TableColumns[i].Cells[kpiInfo.StatusIndex].CellValue) : kpiInfo.StatusValue;
                            }
                            if (kpiInfo.TrendIndex >= 0)
                            {
                                kpiInfo.TrendValue = this.TableColumns[i].Cells[kpiInfo.TrendIndex].CellValue != string.Empty ? int.Parse(this.TableColumns[i].Cells[kpiInfo.TrendIndex].CellValue) : kpiInfo.TrendValue;
                            }
                            kpiInfo.MemberRowIndex = tempKpiInfo.MemberRowIndex;
                            if (!kpis.Contains(kpiInfo) && kpiInfo.Kpi_Name != string.Empty)
                            {
                                kpis.Add(kpiInfo);
                            }
                        }
                    }
                }
                else
                {
                    breakloop = false;
                }
            }
            return kpis;
        }

        private KpiInfoCollection GetKpisAxisMembers()
        {
            KpiInfoCollection kpis = new KpiInfoCollection();
            KpiAxisType kpiAxis = KpiAxisType.None;
            int rowsCount = 0;
            int columnsCount = 0;
            bool breakloop = false;
            kpiAxis = this.GetKpiAxis();
            if (kpiAxis != KpiAxisType.None)
            {
                if (kpiAxis == KpiAxisType.Column)
                {
                    rowsCount = this.RowsCount;
                    columnsCount = this.TableColumns.Count;
                }
                else if (kpiAxis == KpiAxisType.Row)
                {
                    rowsCount = this.TableColumns.Count;
                    columnsCount = this.RowsCount;
                }
            }
            else
            {
                return kpis;
            }

            for (int i = 0; i < columnsCount; i++)
            {
                KpiInfo tempKpiInfo = new KpiInfo();
                for (int j = 0; j < rowsCount; j++)
                {
                    PivotCellDescriptor cellDescriptor;
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        cellDescriptor = this.TableColumns[i].Cells[j];
                    }
                    else
                    {
                        cellDescriptor = this.TableColumns[j].Cells[i];
                    }

                    #region Traversing Columns
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            if (cellDescriptor.Tag is Member)
                            {
                                Member member = (Member)cellDescriptor.Tag;

                                if (member != null)
                                {
                                    if (member.Kpi_Name != null && member.Kpi_Name != string.Empty)
                                    {
                                        tempKpiInfo.Kpi_Name = member.Kpi_Name;
                                    }
                                    else
                                    {
                                        if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                                        {
                                            tempKpiInfo.MemberName = member.Caption;
                                        }
                                        else
                                        {
                                            tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + member.Caption;
                                        }
                                        tempKpiInfo.MemberColumnIndex = i;
                                    }
                                }
                            }
                            switch (cellDescriptor.KpiType)
                            {
                                case KpiTypeEnum.Kpi_Goal:
                                    {
                                        tempKpiInfo.GoalCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.GoalIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Value:
                                    {
                                        tempKpiInfo.MeasureCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.ValueIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Status:
                                    {
                                        tempKpiInfo.StatusGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.StatusIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Trend:
                                    {
                                        tempKpiInfo.TrendGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.TrendIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_None:
                                    {
                                        break;
                                    }
                            }
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value ||
                                cellDescriptor.CellType == PivotCellDescriptorType.RowHeader ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any)
                        {
                            break;
                        }
                    }
                    #endregion

                    #region Traversing Rows
                    else if (kpiAxis == KpiAxisType.Row)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            if (cellDescriptor.Tag is Member)
                            {
                                Member member = (Member)cellDescriptor.Tag;
                                if (member != null)
                                {
                                    if (member.Kpi_Name != null && member.Kpi_Name != string.Empty)
                                    {
                                        tempKpiInfo.Kpi_Name = member.Kpi_Name;
                                    }
                                    else
                                    {
                                        if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                                        {
                                            tempKpiInfo.MemberName = member.Caption;
                                        }
                                        else
                                        {
                                            tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + member.Caption;
                                        }
                                        tempKpiInfo.MemberColumnIndex = i;
                                    }
                                }
                            }
                            switch (cellDescriptor.KpiType)
                            {
                                case KpiTypeEnum.Kpi_Goal:
                                    {
                                        tempKpiInfo.GoalCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.GoalIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Value:
                                    {
                                        tempKpiInfo.MeasureCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.ValueIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Status:
                                    {
                                        tempKpiInfo.StatusGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.StatusIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Trend:
                                    {
                                        tempKpiInfo.TrendGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.TrendIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_None:
                                    {
                                        break;
                                    }
                            }
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value ||
                                cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any)
                        {
                            break;
                        }
                    }
                    #endregion

                }
                if (!kpis.Contains(tempKpiInfo) && tempKpiInfo.Kpi_Name != string.Empty && !breakloop)
                {
                    kpis.Add(tempKpiInfo);
                }
                else
                {
                    int k = kpis.FindMemberIndexByName(tempKpiInfo.Kpi_Name, tempKpiInfo.MemberName);
                    if (k >= 0)
                    {
                        kpis[k] = kpis[k].Copy(tempKpiInfo, kpis[k]);
                    }
                    breakloop = false;
                }
            }
            return kpis;
        }

        public KpiInfoCollection GetKpis()
        {
            this.RemoveTotalsElements();
            this.ClearLevelHeadersArea();
            KpiInfoCollection kpiColumnCollection = new KpiInfoCollection();
            kpiColumnCollection = GetKpisAxisMembers();
            kpiColumnCollection.RemoveMeasures();
            KpiInfoCollection Kpis = new KpiInfoCollection();
            Kpis = MergeKpiRowsWithColumns(kpiColumnCollection);
            return (Kpis);
        }

        private KpiAxisType GetKpiAxis()
        {
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.TableColumns.Count; j++)
                {
                    PivotCellDescriptor cellDescriptor = this.TableColumns[j].Cells[i];
                    if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        if (cellDescriptor.KpiType != KpiTypeEnum.Kpi_None)
                        {
                            return KpiAxisType.Column;
                        }
                    }
                    if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        if (cellDescriptor.KpiType != KpiTypeEnum.Kpi_None)
                        {
                            return KpiAxisType.Row;
                        }
                    }
                }
            }
            return KpiAxisType.None;
        }

        public PivotRowDescriptor GetRowAt(int index)
        {
            PivotRowDescriptor row = new PivotRowDescriptor();

            if (index >= 0)
            {
                for (int i = 0; i < this.TableColumns.Count; i++)
                {
                    PivotColumnDescriptor column = this.TableColumns[i];

                    if (column.Cells.Count > index)
                        row.Cells.Add(column.Cells[index]);
                }
            }

            return row;
        }


        public virtual void SetExtendedStyles(Member expandMember)
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDescriptor = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colDescriptor.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDescriptor.Cells[rowIndex];

                    if (expandMember != null)//Setting expanded ex style.
                    {
                        Member olapMember = cellDesc.Tag as Member;

                        if (olapMember != null && olapMember.ParentMember != null
                            && olapMember.ParentMember.UniqueName == expandMember.UniqueName)
                        {
                            IPivotCellHost cellCollection = null;

                            if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                                cellCollection = this.GetRowAt(rowIndex);
                            else if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                                cellCollection = colDescriptor;

                            foreach (PivotCellDescriptor pcd in cellCollection.Cells)
                                pcd.CellExTypes.Add(PivotCellDescriptorExType.DrilledDown.ToString());
                        }
                    }
                }
            }

            Dictionary<int, List<int>> colLevels = this.CreateColumnsLevelsHash();
            Dictionary<int, List<int>> rowLevels = this.CreateRowsLevelsHash();
            List<int> overTotalCols = new List<int>() { this.TableColumns.Count - 1 };
            if (colLevels.Keys.Count > 0)
                overTotalCols = colLevels[colLevels.Keys.Min()];
            List<int> overTotalRows = new List<int>() { this.RowsCount - 1 };
            if (rowLevels.Keys.Count > 0)
                overTotalRows = rowLevels[rowLevels.Keys.Min()];

            foreach (int colIndex in overTotalCols)
                foreach (int rowIndex in overTotalRows)
                {
                    PivotCellDescriptor cellDesc = this.TableColumns[colIndex].Cells[rowIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.Value)
                        cellDesc.CellExTypes.Add(PivotCellDescriptorExType.OverallTotal.ToString());
                }
        }
    }

    public class PivotValueCellData
    {
        #region members
        private string m_measure = string.Empty;
        private string m_value = string.Empty;
        private List<string> m_rows = null;
        private List<string> m_columns = null;
        #endregion

        #region properties
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value != value)
                {
                    m_value = value;
                }
            }
        }

        public List<string> Rows
        {
            get
            {
                return m_rows;
            }
            set
            {
                if (m_rows != value)
                {
                    m_rows = value;
                }
            }
        }

        public List<string> Columns
        {
            get
            {
                return m_columns;
            }
            set
            {
                if (m_columns != value)
                {
                    m_columns = value;
                }
            }
        }

        public string Measure
        {
            get
            {
                return m_measure;
            }
            set
            {
                if (m_measure != value)
                {
                    m_measure = value;
                }
            }
        }
        #endregion

        #region initialize/finalize methods
        public PivotValueCellData()
        {
            m_rows = new List<string>();
            m_columns = new List<string>();
        }

        public PivotValueCellData(string measureName, string valueName, List<string> rowsName, List<string> columnsName)
        {
            this.Measure = measureName;
            this.Value = valueName;
            this.Rows = rowsName;
            this.Columns = columnsName;
        }
        #endregion
    }
}
