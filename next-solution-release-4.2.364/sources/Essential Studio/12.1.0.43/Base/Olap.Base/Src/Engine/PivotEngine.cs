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
using System.ComponentModel;
using System.Collections;
using Syncfusion.Linq;
using System.Xml.Serialization;

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Data;
namespace Syncfusion.OlapSilverlight.Engine

#else
using System.Web.UI;
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Reports;
using System.Data;

namespace Syncfusion.Olap.Engine

#endif
{
    /// <summary>
    /// This class is used to represent OLAP table in virtual flexible way.
    /// Provides several useful methods to operate on OLAP table.
    /// </summary>
#if !SILVERLIGHT
    [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]

    public class PivotEngine
        : ICloneable<PivotEngine>
#else
    public class PivotEngine
    #endif

    {
        #region class constants
        private const string m_cSummaryText = "Total";
        private const string m_cMeasuresSuff = "Measure";
        private const string m_cNull = "Null";
        #endregion

        #region class members
        private List<PivotColumnDescriptor> m_tableColumns = null;
        private Dictionary<int, List<int>> m_levelsHash = null;
        private SummaryLayout m_summaryPos = SummaryLayout.Top;
        private Dictionary<PivotCellDescriptor, GridRangeInfo> m_spanCells = null;
        private List<int> m_rowsRendered = null;
        private bool m_bCrossLevelExpand = false;
        private GridRangeInfo m_headerSection = GridRangeInfo.Empty;
        private GridRangeInfo m_rowHeaderSection = GridRangeInfo.Empty;
        private List<GridRangeInfo> m_CoveredCellsRangeInfo = new List<GridRangeInfo>();
        private bool m_ApplyExType = false;
        private bool resetSpans = false;
        #endregion

        #region class properties
        /// <summary>
        /// Indicates if table is reverted. If true - column header cells are expandable.
        /// </summary>
        public bool Reverted
        {
            get
            {
                bool reverted = false;
                foreach (PivotCellDescriptor cellDescriptor in this.GetRowAt(0).Cells)
                {
                    if (cellDescriptor.HasChildren)
                    {
                        reverted = true;
                        break;
                    }
                }

                return reverted;
            }
        }
        /// <summary>
        /// gets number of rows in the table.
        /// </summary>
        public int RowsCount
        {
            get
            {
                if (this.TableColumns.Count > 0)
                    return this.TableColumns[0].Cells.Count;

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the Collection of Covered cells range info.
        /// </summary>
        /// <value>The covered cells range info.</value>
        public List<GridRangeInfo> CoveredCellsRangeInfo
        {
            get
            {
                return m_CoveredCellsRangeInfo;
            }
            set
            {
                m_CoveredCellsRangeInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent engine.
        /// </summary>
        /// <value>The parent engine.</value>
        public object ParentEngine
        {
            get;
            set;
        }

        internal bool isSortOrFilter
        {
            get ;
            set ;
        }

        internal bool isHorizontal
        {
            get;
            set;
        }
        internal bool isVertical
        {
            get;
            set;
        }
        /// <summary>
        /// Gets/sets table header range.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the row header section.
        /// </summary>
        /// <value>The row header section.</value>
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

        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Engine.PivotCellDescriptor"/> with the specified row index.
        /// </summary>
        /// <value></value>
        public PivotCellDescriptor this[int rowIndex, int columnIndex]
        {
            get
            {
                return this.TableColumns[columnIndex].Cells[rowIndex];
            }
        }
        /// <summary>
        /// Gets default pivot table.
        /// </summary>
        public static PivotEngine DefaultTable
        {
            get
            {
                PivotEngine table = new PivotEngine();

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
        /// <summary>
        /// Gets/sets if cross-level expand feature is enabled.
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue(typeof(bool), "false")]
        public bool CrossLevelExpand
        {
            get
            {
                return m_bCrossLevelExpand;
            }
            set
            {
                if (m_bCrossLevelExpand != value)
                {
                    m_bCrossLevelExpand = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets rendered rows.
        /// </summary>
        public List<int> RenderedRows
        {
            get
            {
                return m_rowsRendered;
            }
        }

        /// <summary>
        /// Gets or sets the span cells.
        /// </summary>
        /// <value>The span cells.</value>
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

        /// <summary>
        /// Gets/sets summary position in current table descriptor.
        /// </summary>
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
                    this.IndexCells();
                    //SetTotalsSigns();
                }
            }
        }

        /// <summary>
        /// Gets/sets Grid Layout
        /// </summary>
        internal GridLayout Layout
        {
            get;
            set;
        }

        /// <summary>
        /// Returns if this table descriptor contains OLAP expandable structured data.
        /// </summary>
        public bool IsOLAP
        {
            get
            {
                this.CreateRowsLevelsHash();
                if (ItemSource != null)
                {
                    return false;
                }
                if (this.LevelsHash != null)
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

#if !SILVERLIGHT
        [NonSerialized]
#endif
        /// <summary>
        /// Represents the Items source.
        /// </summary>
        public object m_ItemSource;
        /// <summary>
        /// Gets or sets the item source.
        /// </summary>
        /// <value>The item source.</value>
        [XmlIgnore()]
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

        /// <summary>
        /// Collection of child columns.
        /// </summary>
        /// <value>The table columns.</value>
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty), Browsable(true), NotifyParentProperty(true)]
#endif
        public List<PivotColumnDescriptor> TableColumns
        {
            get
            {
                return m_tableColumns;
            }
        }

        /// <summary>
        /// Gets levels hash for curent table.
        /// </summary>
        public Dictionary<int, List<int>> LevelsHash
        {
            get
            {
                if (m_levelsHash == null)
                    this.CreateRowsLevelsHash();

                return m_levelsHash;
            }
        }
        #endregion

        #region class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotEngine"/> class.
        /// </summary>
        public PivotEngine()
        {
            m_tableColumns = new List<PivotColumnDescriptor>();
            m_spanCells = new Dictionary<PivotCellDescriptor, GridRangeInfo>();
            m_rowsRendered = new List<int>();           
        }

        #endregion

        #region class public methods

        /// <summary>
        /// Gets details for specified cell.
        /// </summary>
        /// <param name="pivotCell"></param>
        /// <returns></returns>
        public virtual PivotValueCellData GetCellData(PivotCellDescriptor pivotCell)
        {
            GridRangeInfo location = GetUpdatedCellLocation(pivotCell);
            return GetCellData(location.Top, location.Left);
        }

        /// <summary>
        /// Gets details for a cell in specified location.
        /// </summary>
        /// <param name="row">Cell row.</param>
        /// <param name="column">Cell column</param>
        /// <returns>Data structure.</returns>
        public virtual PivotValueCellData GetCellData(int row, int column)
        {
            PivotValueCellData cellData = null;

            if (row > 0 && column > -1
                && this.TableColumns.Count > column && this.RowsCount > row)
            {
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
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                        {
                            cellData.Measure = cellDesc.CellValue;
                            cellData.MeasureInfo = new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName, Member = cellDesc.Tag as Member };
                        }
                        else
                        {
                            cellData.Rows.Add(cellDesc.CellValue);
                            cellData.RowInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName, Member = cellDesc.Tag as Member });
                        }
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.Value)
                        break;
                }

                for (int rowIndex = 0; rowIndex < row; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                        {
                            cellData.Measure = cellDesc.CellValue;
                            cellData.MeasureInfo = new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName, Member = cellDesc.Tag as Member };
                        }
                        else
                        {
                            cellData.Columns.Add(cellDesc.CellValue);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName, Member = cellDesc.Tag as Member });
                        }
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.Value)
                        break;
                }

                cellData.Value = originalCell.CellValue;
            }

            return cellData;
        }

        /// <summary>
        /// Gets the cell data value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public virtual PivotValueCellData GetCellDataValue(int row, int column)
        {
            PivotValueCellData cellData = null;

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
                        {
                            cellData.Rows.Add(cellDesc.CellValue);
                            cellData.RowInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
                        }
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
                        {
                            cellData.Columns.Add(cellDesc.CellValue);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
                        }
                    }
                    else
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                    }
                }
            }

            cellData.Value = originalCell.CellValue;

            return cellData;
        }

        /// <summary>
        /// Gets the cell data for excel layout.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        internal PivotValueCellData GetCellDataForExcelLayout(int row, int column)
        {
            PivotValueCellData cellData = null;

            cellData = new PivotValueCellData();
            PivotColumnDescriptor colDesc = null;
            colDesc = this.TableColumns[column];

            PivotRowDescriptor rowDesc = this.GetRowAt(row);
            PivotCellDescriptor originalCell = colDesc.Cells[row];

            //for (int colIndex = 0; colIndex < column; colIndex++)
            {
                PivotCellDescriptor cellDesc = this[row, 0];
                //if (cellDesc.ParentCellValues != null)
                //{
                //    foreach (var item in cellDesc.ParentCellValues)
                //    {
                //        cellData.Rows.Add(item);
                //    }
                //}

                cellData.Rows.Add(cellDesc.CellValue);
                cellData.RowInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
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
                        {
                            cellData.Columns.Add(cellDesc.CellValue);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
                        }
                    }
                    else
                    {
                        if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                            cellData.Measure = cellDesc.CellValue;
                    }
                }
            }

            cellData.Value = originalCell.CellValue;

            return cellData;
        }

        /// <summary>
        /// Gets the cell data value for IEnumerable source.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public virtual PivotValueCellData GetCellDataValueforIEnumerable(int row, int column)
        {
            PivotValueCellData cellData = null;

            if (row >= 0 && column >= 0
                && this.TableColumns.Count > column && this.RowsCount > row)
            {
                cellData = new PivotValueCellData();
                PivotColumnDescriptor colDesc = null;
                colDesc = this.TableColumns[column];

                PivotRowDescriptor rowDesc = this.GetRowAt(row);
                PivotCellDescriptor originalCell = colDesc.Cells[row];

                for (int colIndex = 0; colIndex < column; colIndex++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[colIndex];
                    if (cellDesc.SpanCell != null)
                    {
                        cellDesc = cellDesc.SpanCell;
                    }

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                        else
                        {
                            cellData.Rows.Add(cellDesc.CellValue);
                            cellData.RowInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
                        }
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
                    if (cellDesc.SpanCell != null)
                    {
                        cellDesc = cellDesc.SpanCell;
                    }
                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                        else
                        {
                            cellData.Columns.Add(cellDesc.CellValue);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = cellDesc.CellValue, UniqueName = cellDesc.UniqueName });
                        }
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        if (cellDesc.Tag is SummaryInfo)
                        {
                            cellData.Measure = ((SummaryInfo)cellDesc.Tag).Key;
                        }
                    }
                }

                cellData.Value = originalCell.CellValue;
            }

            return cellData;
        }

        /// <summary>
        /// Sets extended styles to the cells.
        /// </summary>
        public virtual void SetExtendedStyles(Member expandMember)
        {
            this.m_ApplyExType = true;
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
                            //IPivotCellHost cellCollection = null;

                            //if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                            //    cellCollection = this.GetRowAt(rowIndex);
                            //else if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                            //    cellCollection = colDescriptor;

                            //foreach (PivotCellDescriptor pcd in cellCollection.Cells)
                            //    pcd.CellExTypes.Add(PivotCellDescriptorExType.DrilledDown.ToString());
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

        /// <summary>
        /// Clears top-left area, should be called right before rendering the data.
        /// </summary>
        public void ClearLevelHeadersArea()
        {
            int rowHeaderDepth = 0, colHeaderDepth = 0;
            if (this.RowHeaderSection != GridRangeInfo.Empty)
                rowHeaderDepth = this.RowHeaderSection.Width;
            if (this.HeaderSection != GridRangeInfo.Empty)
                colHeaderDepth = this.HeaderSection.Height;

            PivotCellDescriptor cellDesc = null;
            int Row = 0;
            if (colHeaderDepth == 1)
            {
                if (!this.HeaderSection.ToString().Contains('-') && this.ItemSource != null)
                {
                    if (this[0, rowHeaderDepth].CellType == PivotCellDescriptorType.Any)
                        Row = 1;
                }
            }

            for (int colIndex = 0; colIndex < rowHeaderDepth + Row; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colHeaderDepth; rowIndex++)
                {
                    PivotCellDescriptor topLeftCell = colDesc.Cells[rowIndex];

                    if (rowIndex == 0 && colIndex == 0)
                    {
                        cellDesc = topLeftCell;
                        cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, colHeaderDepth, rowHeaderDepth + Row);
                        cellDesc.CellValue = string.Empty;
                        cellDesc.Value = string.Empty;
                        cellDesc.CellType = PivotCellDescriptorType.Any;
                        cellDesc.SpanCell = null;
                    }
                    else
                        topLeftCell.SpanCell = cellDesc;
                }
            }

            this.CoveredCellsRangeInfo.Add(this[0, 0].Range);
        }

        /// <summary>
        /// Removes the measures section.
        /// </summary>
        public void RemoveMeasuresSection()
        {
            PivotColumnDescriptor colDesc = this.TableColumns[this.TableColumns.Count - 1];
            bool removed = false;

            foreach (PivotCellDescriptor cellDesc in colDesc.Cells)
            {
                if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                {
                    this.RemoveRowAt(colDesc.Cells.IndexOf(cellDesc));
                    removed = true;
                    break;
                }
            }

            if (!removed)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(this.RowsCount - 1);

                foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                {
                    if (cellDesc.UniqueName.Contains(m_cMeasuresSuff))
                    {
                        this.TableColumns.RemoveAt(rowDesc.Cells.IndexOf(cellDesc));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Clears pivot table( removes empty rows )
        /// </summary>
        public void ClearTable()
        {
            for (int rowIndex = 0; rowIndex < this.RowsCount; rowIndex++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                bool emptyRow = true;

                foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                {
                    if (cellDesc.CellValue != string.Empty)
                    {
                        emptyRow = false;
                        break;
                    }
                }

                if (emptyRow)
                {
                    this.RemoveRowAt(rowIndex);
                    rowIndex--;
                }
            }
            this.IndexCells();
        }

        /// <summary>
        /// Transforms the table by changing rows with columns.
        /// </summary>
        public PivotEngine RevertTransform()
        {
            PivotEngine transformedEngine = new PivotEngine();
            transformedEngine.SummaryPosition = this.SummaryPosition;
            transformedEngine.HeaderSection = this.HeaderSection.Clone() as GridRangeInfo;

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
        /// <summary>
        /// Returns cell location.
        /// </summary>
        /// <param name="cellDesc"></param>
        /// <returns>Cell location.</returns>
        public GridRangeInfo GetUpdatedCellLocation(PivotCellDescriptor cellDesc)
        {
            GridRangeInfo location = GridRangeInfo.FromTlhw(-1, -1, 0, 0);
            
            if (cellDesc.PivotRowIndex == -1 || cellDesc.CellIndex == -1)
                this.IndexCells();

            if (cellDesc != null)
            {
                location = GridRangeInfo.FromTlhw(cellDesc.PivotRowIndex, cellDesc.CellIndex, 1, 1);
            }

            return location;
        }

        /// <summary>
        /// Returns cell location.
        /// </summary>
        /// <param name="cellDesc"></param>
        /// <returns>Cell location.</returns>
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

        /// <summary>
        /// Creates table filled with default cell objects.
        /// </summary>
        /// <param name="rowCount">Number of rows, should be contained in the table.</param>
        /// <param name="colCount">Number of columns, sould be contained in the table.</param>
        /// <returns>Created table.</returns>
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

        /// <summary>
        /// Checks if expand level is currect, corrects specified level if needed.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="state">State to apply.</param>
        /// <returns>Correct level.</returns>
        public int ValidateExpandLevel(int level, ExpandableState state)
        {
            int validLevel = level;
            List<int> levels = new List<int>(this.LevelsHash.Keys);

            if (!levels.Contains(level))
            {
                validLevel = 0;
                int maxLevel = 0;
                int minLevel = levels[0];

                foreach (int levelKey in levels)
                {
                    if (maxLevel < levelKey)
                        maxLevel = levelKey;
                    if (minLevel > levelKey)
                        minLevel = levelKey;
                }

                if (state == ExpandableState.Collapsed)
                {
                    while (level < maxLevel)
                    {
                        level++;

                        if (levels.Contains(level))
                        {
                            validLevel = level;
                            break;
                        }
                    }
                }
                else if (state == ExpandableState.Expanded)
                {
                    while (level > minLevel)
                    {
                        level--;

                        if (levels.Contains(level))
                        {
                            validLevel = level;
                            break;
                        }
                    }
                }
            }

            return validLevel;
        }

        /// <summary>
        /// Checks if data has OLAP structure, if not cell types will be changed.
        /// </summary>
        public void ValidateCellTypes(Dictionary<PivotCellDescriptor, GridRangeInfo> spannedCells)
        {
            if (!this.IsOLAP)
            {
                List<PivotCellDescriptor> spanHeaders = new List<PivotCellDescriptor>(spannedCells.Keys);

                for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
                {
                    PivotColumnDescriptor columnDesc = this.TableColumns[colIndex];

                    for (int rowIndex = 0; rowIndex < columnDesc.Cells.Count; rowIndex++)
                    {
                        PivotCellDescriptor cellDesc = columnDesc.Cells[rowIndex];

                        if (rowIndex == 0)
                            cellDesc.CellType = PivotCellDescriptorType.ColumnHeader;
                        else if (colIndex == 0)
                            cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                        else
                        {
                            cellDesc.CellType = PivotCellDescriptorType.Value;

                            if (spanHeaders.Contains(cellDesc))
                            {
                                spannedCells.Remove(cellDesc);
                                cellDesc.Range = GridRangeInfo.EmptyRange();
                            }

                            if (cellDesc.SpanCell != null)
                                cellDesc.SpanCell = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets inner summary layout. (table becomes grouped)
        /// </summary>
        public void SetInnerSummaryLayout()
        {
            Dictionary<string, GridRangeInfo> dict = new Dictionary<string, GridRangeInfo>();

            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];

                for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        List<string> names = new List<string>(dict.Keys);
                        string cellName = this.GetCellPath(colIndex, rowIndex);
                        int insertionPoint = rowIndex;

                        if (names.Contains(cellName))
                        {
                            GridRangeInfo location = dict[cellName];
                            insertionPoint = location.Top + 1;
                            this.MoveRow(rowIndex, insertionPoint);
                            //move previous records.
                            List<string> dictKeys = new List<string>(dict.Keys);

                            foreach (string key in dictKeys)
                            {
                                GridRangeInfo rangeInfo = dict[key];

                                if (rangeInfo.Top >= insertionPoint)
                                    dict[key] = GridRangeInfo.FromTlhw(rangeInfo.Top + 1, rangeInfo.Left
                                        , rangeInfo.Height, rangeInfo.Width);
                            }
                        }

                        dict[cellName] = GridRangeInfo.FromTlhw(insertionPoint, colIndex, 1, 1);
                    }
                }
            }

            this.CreateRowsLevelsHash();
        }

        /// <summary>
        /// Sets initially expanded rendering level.
        /// </summary>
        /// <param name="level">Last expanded level.</param>
        /// <remarks>Works if ExpandableModel is enabled.</remarks>
        public PivotEngine SetRenderingLevel(int level)
        {
            PivotEngine expandableDescriptor = new PivotEngine();
            expandableDescriptor.SummaryPosition = this.SummaryPosition;
            List<int> levels = new List<int>(LevelsHash.Keys);
            List<int> rows = new List<int>();
            int maxLevel = 0;
            int minLevel = levels[0];
            m_rowsRendered.Clear();

            foreach (int currLevel in levels)
            {
                if (maxLevel < currLevel)
                    maxLevel = currLevel;
                if (minLevel > currLevel)
                    minLevel = currLevel;
            }
            int expandedLevel = level + 1;

            expandedLevel = this.ValidateExpandLevel(expandedLevel, ExpandableState.Collapsed);
            if (!levels.Contains(expandedLevel))
                expandedLevel = minLevel;

            if (this.SummaryPosition == SummaryLayout.None)
            {
                rows.AddRange(LevelsHash[expandedLevel]);
            }
            else
            {
                foreach (int currLevel in levels)
                {
                    if (currLevel <= expandedLevel)
                        rows.AddRange(LevelsHash[currLevel]);
                }
            }

            // to-do: Add header
            if (m_headerSection != GridRangeInfo.Empty)
            {
                for (int rowIndex = m_headerSection.Top; rowIndex < m_headerSection.Height; rowIndex++)
                    rows.Insert(rowIndex, rowIndex);
            }
            else
                rows.Insert(0, 0);

            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor columnDescriptor = this.TableColumns[colIndex];
                PivotColumnDescriptor pcd = new PivotColumnDescriptor();
                bool emptyCol = true;

                for (int rowIndex = 0; rowIndex < columnDescriptor.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor currCell = columnDescriptor.Cells[rowIndex];

                    if (rows.Contains(rowIndex))
                    {
                        if (currCell.CellType != PivotCellDescriptorType.ColumnHeader
                            && currCell.CellValue != String.Empty)
                            emptyCol = false;

                        pcd.Cells.Add(columnDescriptor.Cells[rowIndex]);
                        if (colIndex == 0)
                            m_rowsRendered.Add(rowIndex);
                    }
                }

                if (!emptyCol)
                    expandableDescriptor.TableColumns.Add(pcd);
            }

            return expandableDescriptor;
        }

        /// <summary>
        /// Creates new table descriptor form array of rows numbers.
        /// </summary>
        /// <param name="rows">Array of rows numbers.</param>
        /// <returns>Created table.</returns>
        public PivotEngine CreateTableFromRows(List<int> rows)
        {
            PivotEngine table = new PivotEngine();

            foreach (PivotColumnDescriptor colDescriptor in this.TableColumns)
            {
                PivotColumnDescriptor newColumn = new PivotColumnDescriptor();
                bool emptyCol = true;

                foreach (int rowIndex in rows)
                {
                    PivotCellDescriptor pcd = colDescriptor.Cells[rowIndex];
                    if (this.CrossLevelExpand)
                        pcd = pcd.Clone();

                    newColumn.Cells.Add(pcd);

                    if (pcd.CellType != PivotCellDescriptorType.ColumnHeader
                        && pcd.CellValue != string.Empty)
                        emptyCol = false;
                }

                if (!emptyCol)
                    table.TableColumns.Add(newColumn);
            }

            return table;
        }

        /// <summary>
        /// Toggles expandable state of the specified cell.
        /// </summary>
        /// <param name="location">Cell location in the gird.</param>
        /// <param name="state">Current cell state.</param>
        /// <param name="renderedRows"></param>
        public List<int> ToggleExpandableState(GridRangeInfo location, ExpandableState state, List<int> renderedRows)
        {
            //m_renderingTable = this.m_tableWrapper.Clone();
            List<int> transformedRows = new List<int>(renderedRows);

            int cellRow = renderedRows[location.Top];
            PivotColumnDescriptor pcd = this.TableColumns[location.Right];
            StringBuilder cellPath = new StringBuilder();
            PivotCellDescriptor expandCell = pcd.Cells[cellRow];
            List<int> expandRows = new List<int>();

            for (int i = 0; i < location.Right; i++)
                cellPath.Append(this.TableColumns[i].Cells[cellRow].CellValue);

            if (state == ExpandableState.Collapsed)
            {
                int expandLevel = this.GetRowLevel(cellRow) + 1;
                expandLevel = this.ValidateExpandLevel(expandLevel, state);
                StringBuilder currPath = new StringBuilder();

                for (int rowIndex = 0; rowIndex < pcd.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDescriptor = pcd.Cells[rowIndex];

                    if (cellDescriptor.CellValue == expandCell.CellValue
                        && expandLevel == this.GetRowLevel(rowIndex))
                    {
                        currPath.Length = 0;
                        for (int i = 0; i < location.Right; i++)
                            currPath.Append(this.TableColumns[i].Cells[rowIndex].CellValue);

                        if (currPath.ToString() == cellPath.ToString() || this.CrossLevelExpand)
                            expandRows.Add(rowIndex);
                    }
                }

                if (this.SummaryPosition == SummaryLayout.Bottom)
                    transformedRows.InsertRange(location.Top, expandRows);
                else if (this.SummaryPosition == SummaryLayout.Top)
                    transformedRows.InsertRange(location.Top + 1, expandRows);
                else
                {
                    transformedRows.RemoveAt(location.Top);
                    transformedRows.InsertRange(location.Top, expandRows);
                }
            }
            else
            {
                PivotColumnDescriptor siblingColumn = this.TableColumns[location.Right + 1];
                StringBuilder currPath = new StringBuilder();

                for (int rowIndex = 0; rowIndex < pcd.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDescriptor = pcd.Cells[rowIndex];

                    if (cellDescriptor.CellValue == expandCell.CellValue)
                    {
                        PivotCellDescriptor siblingDescriptor = siblingColumn.Cells[rowIndex];
                        currPath.Length = 0;

                        for (int i = 0; i < location.Right; i++)
                            currPath.Append(this.TableColumns[i].Cells[rowIndex].CellValue);

                        if (currPath.ToString() == cellPath.ToString())
                        {
                            if (siblingDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                            {
                                if (transformedRows.Contains(rowIndex))
                                    transformedRows.Remove(rowIndex);
                            }
                            else
                            {
                                if (!transformedRows.Contains(rowIndex))
                                    transformedRows.Insert(location.Top, rowIndex);
                            }
                        }
                    }
                }
            }

            return transformedRows;
        }

        /// <summary>
        /// Sets expandable states to row header cells in expandable table.
        /// </summary>
        /// <param name="originalTable">Original non-expanded table.</param>
        public void SetExpandableState(PivotEngine originalTable)
        {
            List<int> levels = new List<int>(originalTable.LevelsHash.Keys);
            levels.Sort();
            int maxLevel = levels[levels.Count - 1];

            if (this.TableColumns.Count > 0)
            {
                PivotColumnDescriptor initColWrapper = this.TableColumns[0];

                for (int row = 0; row < initColWrapper.Cells.Count; row++)
                {
                    int currLevel = 0;

                    for (int col = 0; col < this.TableColumns.Count; col++)
                    {
                        PivotCellDescriptor cellWrapper = this.TableColumns[col].Cells[row];

                        if (cellWrapper.CellType == PivotCellDescriptorType.RowHeader
                            && cellWrapper.CellValue != null)
                        {
                            currLevel++;

                            if (currLevel < maxLevel && levels.Contains(currLevel))
                            {
                                PivotCellDescriptor nextCell = this.TableColumns[col + 1].Cells[row];
                                int cellHeight = cellWrapper.Range.Height;
                                if (nextCell.CellType != PivotCellDescriptorType.RowHeader && cellHeight > 1)
                                    nextCell = this.TableColumns[col + 1].Cells[row + cellHeight - 1];

                                if (nextCell.CellType == PivotCellDescriptorType.RowHeader)
                                    cellWrapper.ExpandableState = ExpandableState.Expanded;
                                else
                                {
                                    string path = this.GetCellPath(col, row);
                                    PivotColumnDescriptor originalColumn = originalTable.TableColumns[col];
                                    bool expandable = false;

                                    for (int oRowIndex = 0; oRowIndex < originalColumn.Cells.Count; oRowIndex++)
                                    {
                                        if (path == originalTable.GetCellPath(col, oRowIndex)
                                            && originalTable.GetRowLevel(oRowIndex) > currLevel)
                                            expandable = true;
                                    }

                                    if (expandable)
                                        cellWrapper.ExpandableState = ExpandableState.Collapsed;
                                    else
                                        cellWrapper.ExpandableState = ExpandableState.None;
                                }
                            }
                            else
                                cellWrapper.ExpandableState = ExpandableState.None;
                        }
                        else
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets [Summary] style to summary representing cells.
        /// </summary>
        /// <remarks>
        /// Summary cells are determined dynamically, depending on current max expand level.
        /// </remarks>
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
                            && cellD.CellValue == string.Empty)
                            cellD.CellType = PivotCellDescriptorType.SummaryRow;
                        else
                            cellD.CellType = PivotCellDescriptorType.Value;
                    }

                    if (!cellD.UniqueName.Contains("All") && cellD.Range.ToString().Contains('-') && cellD.SpanCell==null)
                    {
                        if(cellD != this[0,0])
                        {
                            this.CoveredCellsRangeInfo.Add(cellD.Range);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the rows levels hash.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, List<int>> CreateRowsLevelsHash()
        {
            //Dictionary<int, List<int>> levelsHash = null;

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
        /// Creates the columns levels hash.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the cell path.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        internal string GetCellPath(int colIndex, int rowIndex)
        {
            StringBuilder cellPath = new StringBuilder();

            for (int i = 0; i <= colIndex; i++)
            {
                PivotCellDescriptor cellDesc = this.TableColumns[i].Cells[rowIndex];
                if (cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                {
                    if (cellDesc.UniqueName.Length != 0)
                    {
                        cellPath.Append(cellDesc.UniqueName);
                    }
                    else
                    {
                        Member m_Member = cellDesc.Tag as Member;
                        if (m_Member != null)
                        {
                            cellPath.Append(m_Member.UniqueName);
                        }
                        else
                        {
                            cellPath.Append(cellDesc.CellValue);
                        }
                    }
                }
            }

            return cellPath.ToString();
        }

        /// <summary>
        /// Gets the cell vertical path.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        internal string GetCellVerticalPath(int colIndex, int rowIndex)
        {
            StringBuilder cellPath = new StringBuilder();

            for (int i = 0; i <= rowIndex; i++)
            {
                PivotCellDescriptor cellDesc = this.TableColumns[colIndex].Cells[i];
                if (cellDesc.CellType != PivotCellDescriptorType.SummaryColumn)
                {
                    Member m_Member = cellDesc.Tag as Member;
                    if (m_Member != null)
                    {
                        cellPath.Append(m_Member.UniqueName);
                    }
                    else
                    {
                        cellPath.Append(cellDesc.CellValue);
                    }
                }
            }

            return cellPath.ToString();
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

        /// <summary>
        /// Moves the row to current position to new position.
        /// </summary>
        /// <param name="currPosition">The curr position.</param>
        /// <param name="newPosition">The new position.</param>
        /// <param name="ignoreSpans">if set to <c>true</c> [ignore spans].</param>
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
                        colWrapper.Cells.RemoveAt(currPosition);
                        colWrapper.Cells.Insert(newPosition, cellWrapper);
                    }
                }
            }
        }

        /// <summary>
        /// Cleares descriptor.
        /// </summary>
        public void Reset()
        {
            foreach (PivotColumnDescriptor column in this.m_tableColumns)
                column.Cells.Clear();

            this.m_tableColumns.Clear();
        }

        /// <summary>
        /// Gets level of specified row.
        /// </summary>
        /// <param name="rowindex">Row index.</param>
        /// <returns>Level.</returns>
        public int GetRowLevel(int rowindex)
        {
            int level = -1;

            foreach (int levelKey in this.LevelsHash.Keys)
            {
                List<int> rows = this.LevelsHash[levelKey];

                if (rows.Contains(rowindex))
                {
                    level = levelKey;
                    break;
                }
            }

            return level;
        }

        /// <summary>
        /// Inserts row at specified index.
        /// </summary>
        /// <param name="row">Row to insert.</param>
        /// <param name="index">Insertion index(Zero-based)</param>
        public void InsertRow(PivotRowDescriptor row, int index)
        {
            if (index >= 0)
            {
                for (int i = 0; i < this.TableColumns.Count; i++)
                {
                    PivotColumnDescriptor column = this.TableColumns[i];

                    if (column.Cells.Count < index)
                        index = column.Cells.Count;

                    column.Cells.Insert(index, row.Cells[i]);
                }
            }
        }

        /// <summary>
        /// Inserts the additional rows if Summary type is set to String in case of IList/DataTable binding.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="col">The col.</param>
        /// <param name="row">The row.</param>
        /// <param name="summaryInfos">The summary infos.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="IListSource">if set to <c>true</c> [I list source].</param>
        internal void InsertAdditionalRows(IEnumerable iEnumerable, string columnName, int col, int row, SummaryInfo[] summaryInfos, GridLayout gridLayout,bool IListSource)
        {
            if (col - 1 >= 0)
            {
#if !SILVERLIGHT
                DataRow[] dataRowsCollection = null;
                if (IListSource)
                {
                    dataRowsCollection = iEnumerable.Cast<DataRow>().ToArray<DataRow>();
                }
#endif

                this.RowHeaderSection = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top, this.RowHeaderSection.Left, this.RowHeaderSection.Height + iEnumerable.AsQueryable().Count(), this.RowHeaderSection.Width);
                PivotCellDescriptor parentCell = this.TableColumns[col - 1].Cells[row];
                if(parentCell.CellType != PivotCellDescriptorType.Value)
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
                                                if (cell.Range.ToString().Contains('-') || cell.Range!= GridRangeInfo.Empty)
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
                                    if (IListSource)
                                    {
#if !SILVERLIGHT
                                        childCell.CellValue = dataRowsCollection.Select(i => i[summaryElement]).ElementAt(cellCount).ToString();
#endif
                                    }
                                    else
                                    {
                                        childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                    }
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
                                if (IListSource)
                                {
#if !SILVERLIGHT
                                    childCell.CellValue = dataRowsCollection.Select(i => i[summaryElement]).ElementAt(cellCount).ToString();
#endif
                                }
                                else
                                {
                                    childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                } 
                                childCell.CellType = PivotCellDescriptorType.Value;
                                childCell.UniqueName = summaryElement + "." + childCell.CellValue;
                            }
                        }

                        if (childCell.CellType == PivotCellDescriptorType.Any)
                        {                            
                            string summaryElement = GetSummaryElement(this.TableColumns[column]);
                            if (summaryElement != null)
                            {
                                if (IListSource)
                                {
#if !SILVERLIGHT
                                    childCell.CellValue = dataRowsCollection.Select(i => i[summaryElement]).ElementAt(cellCount).ToString();
#endif
                                }
                                else
                                {
                                    childCell.CellValue = iEnumerable.AsQueryable().Select(summaryElement).ElementAt(cellCount).ToString();
                                }
                                childCell.CellType = PivotCellDescriptorType.Value;
                                childCell.UniqueName = summaryElement + "." + childCell.CellValue;
                            }
                        }
                        //if (column >= fromCol && column <= toCol)
                        //{

                        //}
                        //else
                        //{
                        //    string uniqueName = childCell.UniqueName;
                        //    childCell = new PivotCellDescriptor();
                        //    childCell.CellType = PivotCellDescriptorType.Value;
                        //    //childCell.UniqueName = uniqueName;
                        //}
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

        /// <summary>
        /// Gets row at specified index.
        /// </summary>
        /// <param name="index">the row index.</param>
        /// <returns>Dynamically created row.</returns>
        /// <remarks>If index is incorrect empty row will be returned.</remarks>
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

        /// <summary>
        /// Removes row at specisied index.
        /// </summary>
        /// <param name="index">The row index.</param>
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

        #endregion

        #region class helper methods

        /// <summary>
        /// Removes the totals elements.
        /// </summary>
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
                        if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader || cellDesc.CellExTypes.Contains(PivotCellDescriptorType.ColumnHeader.ToString())) // .Range.Width >= cellDesc.Range.Height && !(cellDesc.Range.Width == 1 && cellDesc.Range.Height == 1))
                        {
                            SetCellDescriptorTagforRow(colInd, rowInd, olapMember);
                            this.RemoveRowAt(rowInd);
                            GridRangeInfo colHeaders = this.HeaderSection;
                            colHeaders = GridRangeInfo.FromTlhw(colHeaders.Top, colHeaders.Left
                                , colHeaders.Height - 1, colHeaders.Width);
                            this.HeaderSection = colHeaders;
                        }
                        else if (cellDesc.CellType == PivotCellDescriptorType.RowHeader || cellDesc.CellExTypes.Contains(PivotCellDescriptorType.RowHeader.ToString()))
                        {
                            SetCellDescriptorTagforColumn(colInd, rowInd, olapMember);
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

        void SetCellDescriptorTagforColumn(int colInd, int row, Member member)
        {
            colInd++;
            var nMember = this.CopyMember(member);
            if (this.TableColumns.Count <= colInd) return;
            foreach (PivotCellDescriptor cell in TableColumns[colInd].Cells)
            {
                if (cell.CellType == PivotCellDescriptorType.SummaryRow && cell.SpanCell == null && cell.Tag == null && cell.CellCaption == member.Caption)
                {
                    cell.Tag = member;
                }
            }
        }
        void SetCellDescriptorTagforRow(int column, int row, Member member)
        {
            row++;
            var nMember = this.CopyMember(member);
            if (this.RowsCount <= row) return;
            for (int i = 0; i < this.TableColumns.Count; i++)
            {
                var cell = this.TableColumns[i].Cells[row];
                if (cell.CellType == PivotCellDescriptorType.SummaryColumn && cell.SpanCell == null && cell.Tag == null && cell.CellCaption == member.Caption)
                {
                    cell.Tag = nMember;
                }
            }
        }

        private Member CopyMember(Member member)
        {
            var nMember = new Member();
            nMember.Caption = member.Caption;
            nMember.Description = member.Description;
            nMember.DrilledDown = member.DrilledDown;
            nMember.HasChildMembers = member.HasChildMembers;
            nMember.LevelDepth = member.LevelDepth;
            nMember.LevelUniqueName = member.LevelUniqueName;
            nMember.Name = member.Name;
            nMember.ParentCaption = member.ParentCaption;
            nMember.ParentHierarchy = member.ParentHierarchy;
            nMember.ParentUniqueName = member.ParentUniqueName;
            nMember.UniqueName = member.UniqueName;

            return nMember;
        }

        /// <summary>
        /// Inserts new column filled with cells, appropriate styles are being applied to the cells.
        /// </summary>
        /// <param name="index">Insertion position.</param>
        internal void InsertStyledColumn(int index)
        {
            PivotColumnDescriptor originalColumn = this.TableColumns[index];
            PivotColumnDescriptor insertionColumn = new PivotColumnDescriptor();

            foreach (PivotCellDescriptor cellDesc in originalColumn.Cells)
            {
                PivotCellDescriptor insertCell = new PivotCellDescriptor();
                insertCell.CellType = cellDesc.CellType;
                insertionColumn.Cells.Add(insertCell);
            }

            this.TableColumns.Insert(index, insertionColumn);
        }

        /// <summary>
        /// Span will be Recalculated if SummaryType is string with IList or DataTable binding
        /// </summary>
        /// <param name="summaryStringCount">Number of SummaryString Items</param>
        /// <param name="summaryElements">Number of SummaryElements</param>
        /// <param name="rowMeasure">Row Measure</param>
        /// <param name="gridLayout">Grid Layout</param>
        public void RecalculateSpans(int summaryStringCount,int summaryElements,bool rowMeasure,GridLayout gridLayout)
        {
            if (rowMeasure)
            {
                if (gridLayout != GridLayout.ExcelLikeLayout)
                {
                    int count = this.RowHeaderSection.Right - summaryStringCount;
                    for (int col = this.RowHeaderSection.Right - summaryStringCount; col < this.RowHeaderSection.Right; col++)
                    {
                        if (col == this.RowHeaderSection.Right - summaryStringCount)
                        {
                            PivotColumnDescriptor columnDesc = this.TableColumns[count];
                            for (int cellIndex = 0; cellIndex < columnDesc.Cells.Count; cellIndex++)
                            {
                                PivotCellDescriptor cellDesc = columnDesc.Cells[cellIndex];
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow && cellDesc.SpanCell == null)
                                {
                                    this.RemoveRowAt(cellIndex, count);
                                    cellIndex--;
                                }
                                else
                                {
                                    PivotCellDescriptor spanCell = cellDesc.SpanCell;
                                    if (spanCell != null)
                                    {
                                        if (spanCell.CellType == PivotCellDescriptorType.SummaryRow)
                                        {
                                            int top = GetTopIndex(columnDesc, spanCell);
                                            GridRangeInfo info = GridRangeInfo.FromTlhw(cellIndex, spanCell.Range.Left, spanCell.Range.Height, spanCell.Range.Width - 1);
                                            spanCell.Range = info;
                                        }
                                    }
                                }
                            }
                            this.TableColumns.RemoveAt(count);
                        }
                        else
                        {
                            PivotColumnDescriptor columnDesc = this.TableColumns[count];
                            for (int cellIndex = 0; cellIndex < columnDesc.Cells.Count; cellIndex++)
                            {
                                PivotCellDescriptor cellDesc = columnDesc.Cells[cellIndex];
                                if (cellDesc.SpanCell != null)
                                {
                                    PivotCellDescriptor spanCell = cellDesc.SpanCell;
                                    if (spanCell.CellType == PivotCellDescriptorType.SummaryRow)
                                    {
                                        GridRangeInfo info = GridRangeInfo.FromTlhw(cellIndex, spanCell.Range.Left, spanCell.Range.Height, spanCell.Range.Width - 1);
                                        spanCell.Range = info;
                                    }
                                }
                            }

                            this.TableColumns.RemoveAt(count);
                        }
                    }

                    GridRangeInfo rowHeaderinfo = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top, this.RowHeaderSection.Left, this.RowsCount, this.RowHeaderSection.Width - summaryStringCount);
                    this.RowHeaderSection = rowHeaderinfo;
                    GridRangeInfo Headerinfo = GridRangeInfo.FromTlhw(this.HeaderSection.Top, this.HeaderSection.Left - summaryStringCount, this.HeaderSection.Height, this.HeaderSection.Width);
                    this.HeaderSection = Headerinfo;

                    for (int top = 0; top < this.HeaderSection.Height; top++)
                    {
                        PivotRowDescriptor rowDesc = this.GetRowAt(top);
                        for (int left = this.HeaderSection.Left; left < this.TableColumns.Count; left++)
                        {
                            PivotCellDescriptor cellDesc = this[top, left];
                            if (cellDesc.Range != GridRangeInfo.Empty)
                            {
                                cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top, cellDesc.Range.Left - summaryStringCount, cellDesc.Range.Height, cellDesc.Range.Width);
                            }
                        }
                    }

                    this[0, 0].Range = GridRangeInfo.Empty;
                    this.ClearLevelHeadersArea();
                }
                else
                {
                    int level = int.Parse(this[summaryStringCount + 1, 0].Level.ToString());
                    for (int col = 0; col < this.TableColumns[0].Cells.Count; col++)
                    {
                        PivotCellDescriptor cellDesc = this.TableColumns[0].Cells[col];
                        if (cellDesc.Level >= level && cellDesc.Tag == null)
                        {
                            this.RemoveRowAt(col);
                            col--;
                        }                       
                        else if (cellDesc.Level >= level && cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            this.RemoveRowAt(col);
                            col--;
                        }
                        else if (cellDesc.Level >= level && cellDesc.Tag != null)
                        {
                            cellDesc.Level = cellDesc.Level - summaryStringCount + 1;
                        }

                        if (col > 1)
                        {
                            if (this.TableColumns.Count > 1)
                            {
                                PivotCellDescriptor spanCell = this.TableColumns[1].Cells[col];
                                if (spanCell.Range != GridRangeInfo.Empty)
                                {
                                    spanCell.Range = GridRangeInfo.FromTlhw(col, spanCell.Range.Left, spanCell.Range.Height, spanCell.Range.Width);
                                }
                            }
                        }
                    }                    
                }
            }
            else
            {
                int count = this.HeaderSection.Bottom - summaryStringCount;
                int temp = 0;
                for (int row = this.HeaderSection.Bottom - summaryStringCount; row < this.HeaderSection.Bottom; row++)
                {
                    if (row == this.HeaderSection.Bottom - summaryStringCount)
                    {
                        PivotRowDescriptor rowDesc = this.GetRowAt(row);
                        if (gridLayout != GridLayout.NoSummaries)
                        {
                            for (int cell = 0; cell < rowDesc.Cells.Count; cell++)
                            {
                                PivotCellDescriptor cellDesc = rowDesc.Cells[cell];

                                if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader && cellDesc.SpanCell == null)
                                {
                                    int col = cellDesc.Range.Left + summaryElements - temp;
                                    for (int left = cellDesc.Range.Left + summaryElements - temp; left < (cellDesc.Range.Left + summaryElements - temp + cellDesc.Range.Width); left++)
                                    {
                                        if (col < this.TableColumns.Count)
                                        {
                                            PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                            for (int top = 0; top < this.HeaderSection.Bottom - summaryStringCount; top++)
                                            {
                                                PivotCellDescriptor parentCell = columnDesc.Cells[top].SpanCell;
                                                if (parentCell != null)
                                                {
                                                    PivotRowDescriptor parentRows = this.GetRowAt(top);
                                                    int topIndex = GetTopIndex(parentRows, parentCell);
                                                    GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, topIndex, parentCell.Range.Height, parentCell.Range.Width - 1);
                                                    parentCell.Range = rangeInfo;
                                                }
                                            }
                                            this.TableColumns.RemoveAt(col);
                                            temp++;
                                        }
                                    }
                                }
                                else if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    this.TableColumns[cellDesc.Range.Left - temp].Cells[this.HeaderSection.Bottom] = cellDesc;
                                    this.TableColumns[cellDesc.Range.Left - temp].Cells[this.HeaderSection.Bottom].Range = GridRangeInfo.Empty;
                                }
                                else if (cellDesc.SpanCell != null && cellDesc.CellType != PivotCellDescriptorType.ColumnHeader)
                                {
                                    PivotCellDescriptor parentCell = cellDesc.SpanCell;
                                    GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left - temp, parentCell.Range.Height - summaryStringCount, parentCell.Range.Width);
                                    parentCell.Range = rangeInfo;
                                }
                            }
                        }
                        this.RemoveRowAt(count);
                    }
                    else
                    {
                        this.RemoveRowAt(count);
                    }
                }

                GridRangeInfo headerInfo = GridRangeInfo.FromTlhw(this.HeaderSection.Top, this.HeaderSection.Left, this.HeaderSection.Height - summaryStringCount, this.TableColumns.Count-summaryStringCount);
                this.HeaderSection = headerInfo;
                GridRangeInfo rowHeaderInfo = GridRangeInfo.Empty;
                if (this.RowHeaderSection.Top - summaryStringCount > 0)
                {
                    rowHeaderInfo = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top - summaryStringCount, this.RowHeaderSection.Left, this.RowHeaderSection.Height, this.RowHeaderSection.Width);
                    this.RowHeaderSection = rowHeaderInfo;
                }
                else
                    this.RowHeaderSection = rowHeaderInfo;

                if (this.RowHeaderSection != GridRangeInfo.Empty && gridLayout!= GridLayout.ExcelLikeLayout)
                {
                    for (int top = rowHeaderInfo.Top; top < rowHeaderInfo.Bottom + 1; top++)
                    {
                        PivotRowDescriptor rowDesc = this.GetRowAt(top);
                        for (int width = rowHeaderInfo.Left; width < rowHeaderInfo.Width; width++)
                        {
                            PivotCellDescriptor cellDesc = this[top, width];
                            if (cellDesc.Range != GridRangeInfo.Empty)
                            {
                                cellDesc.Range = GridRangeInfo.FromTlhw(cellDesc.Range.Top - summaryStringCount, cellDesc.Range.Left, cellDesc.Range.Height, cellDesc.Range.Width);
                            }
                        }
                    }
                    this[0, 0].Range = GridRangeInfo.Empty;
                    this.ClearLevelHeadersArea();
                }
            }
        }

        /// <summary>
        /// Gets the index of the top.
        /// </summary>
        /// <param name="pivotRowDescriptor">The pivot row descriptor.</param>
        /// <param name="parentCell">The parent cell.</param>
        /// <returns></returns>
        private int GetTopIndex(PivotRowDescriptor pivotRowDescriptor, PivotCellDescriptor parentCell)
        {
            for (int row = 0; row < pivotRowDescriptor.Cells.Count; row++)
            {
                if (pivotRowDescriptor.Cells[row] == parentCell)
                {
                    return row;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the index of CellDescriptor in the specified column.
        /// </summary>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="col">The col.</param>
        private void RemoveRowAt(int cellIndex, int col)
        {
            int temp = 0;
            if (cellIndex >= 0)
            {
                for (int i = 0; i < this.TableColumns.Count; i++)
                {
                    PivotColumnDescriptor column = this.TableColumns[i];

                    if (column.Cells.Count > cellIndex)
                    {
                        if (temp < col)
                        {
                            bool Status = false;
                            PivotCellDescriptor cellDesc = column.Cells[cellIndex].SpanCell;
                            if (cellDesc == null)
                            {
                                cellDesc = column.Cells[cellIndex];
                                Status = true;
                            }
                            if (cellDesc != null)
                            {
                                int top = GetTopIndex(column, column.Cells[cellIndex].SpanCell);
                                int height = ((cellDesc.Range.Height) + top);
                                GridRangeInfo info = GridRangeInfo.FromTlhw(top, cellDesc.Range.Left, (height - top) - 1, cellDesc.Range.Width);
                                cellDesc.Range = info;
                            }
                            if (Status == true)
                            {
                                this.TableColumns[i].Cells[cellIndex + 1] = cellDesc;
                            }
                        }
                        column.Cells.RemoveAt(cellIndex);
                        temp++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the index of CellDescriptor in the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="pivotCellDescriptor">The pivot cell descriptor.</param>
        /// <returns></returns>
        private int GetTopIndex(PivotColumnDescriptor column, PivotCellDescriptor pivotCellDescriptor)
        {
            for (int col = 0; col < column.Cells.Count; col++)
            {
                if (column.Cells[col] == pivotCellDescriptor)
                {
                    return col;
                }
            }
            return 0;
        }     

        /// <summary>
        /// Resets spans in the table.
        /// </summary>
        public void ResetSpans()
        {
            resetSpans = true;
            foreach( PivotColumnDescriptor colDesc in this.TableColumns)
            {
                foreach(PivotCellDescriptor cellDesc in colDesc.Cells)
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
                           // if (GetCellLocation(rangeCell).Top == GetCellLocation(cellDesc).Top)
                            if (rangeCell.Range.Width > 1)
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
                            //if (GetCellLocation(rangeCell).Left == GetCellLocation(cellDesc).Left)
                            if (rangeCell.Range.Height > 1)
                                cellDesc.SpanCell = null;  
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the span for excel layout.
        /// </summary>
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

        /// <summary>
        /// Recalculates spans in the table.
        /// </summary>
        public void RecalculateSpans()
        {
            if (this.ItemSource == null)
            {
                if (IsOLAP)
                    ClearTotalsSigns();
                if (!(isVertical || isHorizontal))
                     RecalculateSpans(true);
                if (IsOLAP)
                {
                      SetTotalsSigns(true);
                }
            }
            else
            {
                this.RecalculateRowHeaderSpans();
                this.RecalculateColumnHeaderSpans(true);
            }
        }

        /// <summary>
        /// Recalculates the row header spans.
        /// </summary>
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
                                string[] range = parent.Range.Info.Split('-');
                                if (range.Length != 2)
                                {
                                    parent.Range = GridRangeInfo.Empty;
                                }

                                if (parent != null && parent.Range.Right == 0)
                                {
                                    parent.Range = GridRangeInfo.FromTlhw(row, cell - previousCell.Keys.Max(), 1, previousCell.Keys.Max() + 1);
                                }
                                cellDesc.SpanCell = parent;
                                cellDesc.CellType = parent.CellType;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates the column header spans.
        /// </summary>
        /// <param name="ItemSource">if set to <c>true</c> [item source].</param>
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
                        if (col != 0 && cell != 0 || this.RowHeaderSection == GridRangeInfo.Empty && cell!=0)
                        {
                            Dictionary<int, PivotCellDescriptor> previousCell = GetPreviousCell(columnDesc, cell);
                            if (previousCell != null)
                            {
                                PivotCellDescriptor parent = previousCell[previousCell.Keys.Max()];
                                string[] range = parent.Range.Info.Split('-');
                                if (range.Length != 2)
                                {
                                    parent.Range = GridRangeInfo.Empty;
                                }

                                if (parent != null && parent.Range.Right == 0)
                                {
                                    parent.Range = GridRangeInfo.FromTlhw(cell - previousCell.Keys.Max(), col, previousCell.Keys.Max() + 1, 1);
                                }
                                cellDesc.SpanCell = parent;
                                cellDesc.CellType = parent.CellType;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous cell.
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the previous cell.
        /// </summary>
        /// <param name="rowDesc">The row desc.</param>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Indexes the cells of TableColumns and Rows.
        /// </summary>
        public void IndexCells()
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < this.RowsCount; rowIndex++)
                {
                    this.TableColumns[colIndex].Cells[rowIndex].PivotRowIndex = rowIndex;
                    this.TableColumns[colIndex].Cells[rowIndex].CellIndex = colIndex;
                }
            }
        }

        /// <summary>
        /// Recalculates spans in the table.
        /// </summary>
        /// <param name="resetHeader">If to reset table header spans</param>
        public void RecalculateSpans(bool resetHeader)
        {
            this.IndexCells();
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[colIndex];
                if (columnDesc.Cells[columnDesc.Cells.Count - 1].CellType == PivotCellDescriptorType.Value)
                    break;
                PivotCellDescriptor summaryCell = null;
                int colSpan = 1;
                bool isSummaryUpdated = false;
                string currPath = string.Empty;
                string summaryPath = string.Empty;

                for (int rowIndex = 0; rowIndex < columnDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = columnDesc.Cells[rowIndex];
                    GridRangeInfo location = GridRangeInfo.Empty;
                    int width = 1;

                    if (summaryCell != null)
                    {
                        if (isSummaryUpdated)
                        {
                            location = this.GetUpdatedCellLocation(summaryCell);
                            isSummaryUpdated = false;
                            summaryPath = this.GetCellPath(location.Left, location.Top);
                        }
                        if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                            width = summaryCell.Range.Width;
                        currPath = this.GetCellPath(colIndex, rowIndex);
                    }
                Label1:
                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader && IsParentElement(cellDesc, colIndex, true) || (cellDesc.CellType == PivotCellDescriptorType.SummaryRow && cellDesc.SpanCell == null)
                        )
                    {
                        if (summaryCell == null || currPath != summaryPath)
                        {
                            if (summaryCell != null)
                            {
                                summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);
                            }

                            summaryCell = cellDesc;
                            summaryCell.SpanCell = null;
                            colSpan = 1;
                            isSummaryUpdated = true;
                        }
                        else if (summaryCell.CellValue == cellDesc.CellValue && string.Equals(summaryPath, currPath))
                        {
                            cellDesc.SpanCell = summaryCell;
                            colSpan++;
                        }
                    }
                    else if (cellDesc.SpanCell != null && cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        if (cellDesc.SpanCell.Range.Width < 2 && cellDesc.SpanCell.Range.Height < 2)
                        {
                            cellDesc.SpanCell = null;
                            goto Label1;
                        }
                    }

                    else
                    {
                        if (summaryCell != null && !string.Equals(summaryCell.CellValue , cellDesc.CellValue))
                        {
                            if (colSpan > 1)
                            {
                                summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);
                            }

                            summaryCell = null;
                        }

                        if (cellDesc.SpanCell != null)
                        {
                            bool cellPresent = this.TableColumns.FirstOrDefault(col => col.Cells.FirstOrDefault(c => c == cellDesc.SpanCell) != null) != null;

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
                        else if (cellDesc.Range.RangeType != GridRangeInfoType.Empty
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
                    {
                        width = summaryCell.Range.Width;
                    }
                    summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, colSpan, width);
                }
            }

            if (resetHeader)
                RecalculateColumnHeaderSpans();
        }

        /// <summary>
        /// Recalculates the column header spans.
        /// </summary>
        public void RecalculateColumnHeaderSpans()
        {
            if (m_headerSection != GridRangeInfo.Empty)
            {
                for (int rowIndex = m_headerSection.Top; rowIndex < m_headerSection.Height; rowIndex++)
                {
                    PivotRowDescriptor rowDesc = this.GetRowAt(rowIndex);
                    PivotCellDescriptor summaryCell = null;
                    string spanHeaderPath = string.Empty;
                    int colSpan = 1;
                    bool isSummaryUpdated = false;

                    for (int cellIndex = m_headerSection.Left; cellIndex < rowDesc.Cells.Count; cellIndex++)
                    {
                        PivotCellDescriptor cellDesc = rowDesc.Cells[cellIndex];
                        string currentPath = GetCellVerticalPath(cellIndex, rowIndex);
                        GridRangeInfo location = GridRangeInfo.Empty;
                        string summaryPath = string.Empty;
                        int height = 1;

                        if (summaryCell != null)
                        {
                            if (isSummaryUpdated)
                            {
                                location = this.GetCellLocation(summaryCell);
                                summaryPath = this.GetCellPath(location.Left, location.Top);
                                isSummaryUpdated = false;
                            }
                            if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                                height = summaryCell.Range.Height;
                        }
                    Label1:
                        if (cellDesc.SpanCell == null || cellDesc.CellType != PivotCellDescriptorType.SummaryColumn)
                        {
                            if (summaryCell == null)
                            {
                                summaryCell = cellDesc;
                                spanHeaderPath = GetCellVerticalPath(cellIndex, rowIndex);
                                summaryCell.SpanCell = null;
                                isSummaryUpdated = true;
                                colSpan = 1;
                            }
                            else if ((string.Equals(summaryCell.CellValue, cellDesc.CellValue) && cellDesc.UniqueName.Contains("Measures"))
                                || currentPath == spanHeaderPath)
                            {
                                cellDesc.SpanCell = summaryCell;
                                colSpan++;
                            }

                            else
                            {
                                if (summaryCell != null && colSpan > 1)
                                {
                                    summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, height, colSpan);
                                }

                                summaryCell = cellDesc;
                                spanHeaderPath = GetCellVerticalPath(cellIndex, rowIndex);
                                colSpan = 1;
                            }
                        }
                        else if (cellDesc.SpanCell != null && cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                        {
                            if (cellDesc.SpanCell.Range.Width < 2 && cellDesc.SpanCell.Range.Height < 2)
                            {
                                cellDesc.SpanCell = null;
                                goto Label1;
                            }
                        }
                    }

                    if (summaryCell != null && colSpan > 1)
                    {
                        int height = 1;

                        if (summaryCell.Range != null && summaryCell.Range != GridRangeInfo.Empty)
                        {
                            height = summaryCell.Range.Height;
                        }

                        summaryCell.Range = GridRangeInfo.FromTlhw(0, 0, height, colSpan);
                    }
                }
            }
        }

        private bool IsParentElement(PivotCellDescriptor cellDesc,int col,bool IsRow)
        {
            //return true;
            Member m_Member = cellDesc.Tag as Member;
            if (m_Member != null)
            {
                if (IsRow)
                {
                    if (m_Member.Type != MemberTypeEnum.Measure && m_Member.Type != MemberTypeEnum.Formula)
                    {
                        if (m_Member.HasChildMembers || col < this.RowHeaderSection.Width - 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return true;
                }
                else
                {
                    if (m_Member.Type != MemberTypeEnum.Measure && m_Member.Type != MemberTypeEnum.Formula )
                    {
                        if (m_Member.HasChildMembers || col < this.HeaderSection.Height - 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return true;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Applies all changes in specified wrapper to currect wrapper.
        /// </summary>
        /// <param name="tableWrapper">Wrappaer with changes.</param>
        public void MergeWrapper(PivotEngine tableWrapper)
        {
            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor pcd = this.TableColumns[colIndex];
                PivotColumnDescriptor mergeColumn = null;

                for (int rowIndex = 0; rowIndex < pcd.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor tableCell = pcd.Cells[rowIndex];
                    PivotCellDescriptor mergeCell = null;

                    if (rowIndex == 0)
                        foreach (PivotColumnDescriptor columnDescriptor in tableWrapper.TableColumns)
                            if (columnDescriptor.MappingName == tableCell.CellValue)
                            {
                                mergeColumn = columnDescriptor;
                                break;
                            }

                    if (mergeColumn != null)
                        foreach (PivotCellDescriptor cellDescriptor in mergeColumn.Cells)
                            if (cellDescriptor.CellIndex == rowIndex)
                            {
                                mergeCell = cellDescriptor;
                                break;
                            }

                    foreach (PivotCellDescriptor cellDescriptor in m_spanCells.Keys)
                    {
                        GridRangeInfo range = m_spanCells[cellDescriptor];
                        if (range.Contains(GridRangeInfo.FromTlhw(rowIndex, colIndex, 1, 1)))
                        {
                            tableCell.SpanCell = cellDescriptor;
                            break;
                        }
                    }

                    if (mergeCell != null)
                    {
                        if (mergeCell.CellType != PivotCellDescriptorType.Any)
                            tableCell.CellType = mergeCell.CellType;
                        if (mergeCell.CellValue != string.Empty)
                            tableCell.CellValue = mergeCell.CellValue;

                        if (mergeCell.Range.RangeType != GridRangeInfoType.Empty
                            && tableCell.SpanCell == null)
                        {
                            GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(rowIndex, colIndex
                                , mergeCell.Range.Height, mergeCell.Range.Width);

                            tableCell.Range = mergeCell.Range;
                            m_spanCells.Add(tableCell, rangeInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Alters the table according to the Summary Position
        /// </summary>
        /// <param name="summaryPos">The summary pos.</param>
        internal void TransformTable(SummaryLayout summaryPos)
        {
            SetRowsSummaryPos(summaryPos);
            SetColumnsSummaryPos(summaryPos);

            this.ResetSpans();
            m_summaryPos = summaryPos;
            this.RecalculateSpans();
        }

        /// <summary>
        /// Clears the totals signs.
        /// </summary>
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
                        cellDesc.CellValue = string.Empty;
                }
            }
        }

        /// <summary>
        /// Clears the totals sign for excel layout.
        /// </summary>
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

        /// <summary>
        /// Sets the totals signs.
        /// </summary>
        public void SetTotalsSigns()
        {
            if (this.Layout == GridLayout.NormalTopSummary)
            {
                m_ApplyExType = true;
            }
            SetTotalsSigns(true);
        }

        /// <summary>
        /// Sets the totals signs.
        /// </summary>
        /// <param name="setTotalStyles">if set to <c>true</c> [set total styles].</param>
        public void SetTotalsSigns(bool setTotalStyles)
        {
            int colHeaderStart = 0;
            PivotCellDescriptor totalCell = null;
            PivotCellDescriptor cellToClear = null;
            PivotCellDescriptor parentCell = null;
            string type = PivotCellDescriptorExType.SummaryRow.ToString();

            for (int colIndex = 0; colIndex < this.TableColumns.Count; colIndex++)
            {
                PivotColumnDescriptor colDesc = this.TableColumns[colIndex];
                PivotCellDescriptor prevCell = null;

                if (colDesc.Cells[colDesc.Cells.Count - 1].CellType == PivotCellDescriptorType.Value && !isSortOrFilter)
                {
                    colHeaderStart = colIndex;
                    break;
                }

                for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                {
                    PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];
                    if (isVertical && prevCell != null && colDesc != null && rowIndex == colDesc.Cells.Count - 1 && prevCell.CellType == PivotCellDescriptorType.RowHeader)
                        cellDesc.SpanCell = null;
                    if (isHorizontal && colIndex == this.TableColumns.Count - 1)
                        cellDesc.SpanCell = null;
                    if (cellDesc.SpanCell == null)
                    {
                        totalCell = null;
                        cellToClear = null;

                        if (prevCell != null)
                        {
                            if (this.SummaryPosition == SummaryLayout.Bottom)
                            {
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow && cellDesc.CellExTypes.Count == 0)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.RowHeader || isSortOrFilter || prevCell.CellExTypes.Contains(PivotCellDescriptorType.RowHeader.ToString())
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (this.SummaryPosition == SummaryLayout.Top)
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryRow && prevCell.CellExTypes.Count == 0)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader || cellDesc.CellExTypes.Contains(PivotCellDescriptorType.RowHeader.ToString())
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
                                {
                                    maintenanceCell.CellValue = m_cSummaryText;

                                    if (maintenanceCell.Range.Height > 1 && maintenanceCell.SpanCell == null)
                                    {
                                        if (m_ApplyExType && this.SummaryPosition == SummaryLayout.Bottom)
                                        {
                                            for (int col = colIndex + 1; col < this.RowHeaderSection.Width; col++)
                                            {
                                                PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                                for (int row = rowIndex; row < rowIndex + maintenanceCell.Range.Height; row++)
                                                {
                                                    if (!columnDesc.Cells[row].UniqueName.Contains(MemberTypeEnum.All.ToString()) &&
                                                        !columnDesc.Cells[row].CellCaption.Contains(MemberTypeEnum.All.ToString()) &&
                                                        columnDesc.Cells[row].CellType != PivotCellDescriptorType.SummaryRow &&
                                                        !columnDesc.Cells[row].UniqueName.Contains(m_cMeasuresSuff))
                                                    {
                                                        columnDesc.Cells[row].CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());
                                                        columnDesc.Cells[row].CellType = PivotCellDescriptorType.SummaryRow;
                                                        columnDesc.Cells[row].ExpandableState = ExpandableState.None;
                                                        columnDesc.Cells[row].HasChildren = false;
                                                    }
                                                }
                                            }                                            
                                        }
                                    }
                                }
                                else
                                {
                                     maintenanceCell.CellValue = string.Empty;
                                }

                                int totalRowIndex = rowIndex;
                                if (prevCell == maintenanceCell)
                                {
                                    totalRowIndex -= prevCell.Range != null ? prevCell.Range.Height : 1;
                                }
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
                        if (m_ApplyExType && this.SummaryPosition == SummaryLayout.Top)
                        {
                            if (prevCell.Range.Height > 1 && prevCell.SpanCell == null && prevCell.CellValue == m_cSummaryText)
                            {
                                for (int col = colIndex + 1; col < this.RowHeaderSection.Width; col++)
                                {
                                    PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                    for (int row = rowIndex; row < rowIndex + prevCell.Range.Height; row++)
                                    {
                                        if (!columnDesc.Cells[row].UniqueName.Contains(MemberTypeEnum.All.ToString()) &&
                                            !columnDesc.Cells[row].CellCaption.Contains(MemberTypeEnum.All.ToString()) &&
                                            columnDesc.Cells[row].CellType != PivotCellDescriptorType.SummaryRow &&
                                            !columnDesc.Cells[row].UniqueName.Contains(m_cMeasuresSuff) 
                                            )
                                        {
                                            columnDesc.Cells[row].CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());
                                            columnDesc.Cells[row].CellType = PivotCellDescriptorType.SummaryRow;
                                            columnDesc.Cells[row].ExpandableState = ExpandableState.None;
                                            columnDesc.Cells[row].HasChildren = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (cellDesc.CellValue != m_cSummaryText && cellDesc.CellValue == string.Empty && cellDesc.Range.Width > 1
                         && resetSpans)
                    {
                        if (cellDesc.SpanCell != null && parentCell != GetParentCell(cellDesc.SpanCell))
                        {
                            parentCell = cellDesc.SpanCell;
                            GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, 1, parentCell.Range.Width);
                            GridRangeInfo parentRangeInfo = GridRangeInfo.FromTlhw(rangeInfo.Top, rangeInfo.Left, rangeInfo.Height+1, rangeInfo.Width);
                            parentCell.Range = parentRangeInfo;
                        }
                        else if (cellDesc.SpanCell != null && parentCell == GetParentCell(cellDesc.SpanCell))
                        {
                            GridRangeInfo parentRangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height+1, parentCell.Range.Width);
                            parentCell.Range = parentRangeInfo;
                        }
                    }
                }
            }

            int rowsCount = this.RowsCount;
            type = PivotCellDescriptorExType.SummaryColumn.ToString();

            parentCell = null;

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
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn && cellDesc.CellExTypes.Count ==0)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.ColumnHeader|| isSortOrFilter || prevCell.CellExTypes.Contains(PivotCellDescriptorType.ColumnHeader.ToString())
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (this.SummaryPosition == SummaryLayout.Top)
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryColumn && prevCell.CellExTypes.Count ==0)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader || cellDesc.CellExTypes.Contains(PivotCellDescriptorType.ColumnHeader.ToString()) 
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
                                {
                                    maintenanceCell.CellValue = m_cSummaryText;

                                    if (maintenanceCell.Range.Width > 1 && maintenanceCell.SpanCell == null)
                                    {
                                        if (m_ApplyExType && this.SummaryPosition == SummaryLayout.Bottom)
                                        {
                                            for (int col = colIndex; col < colIndex + maintenanceCell.Range.Width; col++)
                                            {
                                                PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                                for (int row = rowIndex+1; row < this.HeaderSection.Height; row++)
                                                {
                                                    if (!columnDesc.Cells[row].UniqueName.Contains(MemberTypeEnum.All.ToString()) &&
                                                        !columnDesc.Cells[row].CellCaption.Contains(MemberTypeEnum.All.ToString()) &&
                                                        columnDesc.Cells[row].CellType != PivotCellDescriptorType.SummaryColumn)
                                                    {
                                                        columnDesc.Cells[row].CellExTypes.Add(PivotCellDescriptorType.ColumnHeader.ToString());
                                                        columnDesc.Cells[row].CellType = PivotCellDescriptorType.SummaryColumn;
                                                        columnDesc.Cells[row].ExpandableState = ExpandableState.None;
                                                        columnDesc.Cells[row].HasChildren = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    maintenanceCell.CellValue = string.Empty;
                                }

                                int totalColIndex = colIndex;
                                if (maintenanceCell == prevCell)
                                {
                                    totalColIndex -= prevCell.Range != null ? prevCell.Range.Width : 1;
                                }
                                PivotCellDescriptor currentCell = null;

                                if (setTotalStyles)
                                    for (int colNumber = totalColIndex; colNumber < totalColIndex + maintenanceCell.Range.Width; colNumber++)
                                    {
                                        PivotColumnDescriptor colDesc = this.TableColumns[colNumber];

                                        for (int rowCellIndex = rowIndex; rowCellIndex < colDesc.Cells.Count; rowCellIndex++)
                                        {
                                            currentCell = colDesc.Cells[rowCellIndex];

                                            if (!currentCell.CellExTypes.Contains(type))
                                            {
                                                if (maintenanceCell == totalCell && currentCell.CellType == PivotCellDescriptorType.Value)
                                                    currentCell.CellExTypes.Add(type);

                                            }
                                            else
                                                if (maintenanceCell == cellToClear)
                                                    currentCell.CellExTypes.Remove(type);
                                        }
                                    }
                            }
                        }

                        prevCell = cellDesc;
                        if (m_ApplyExType && this.SummaryPosition == SummaryLayout.Top)
                        {
                            if (prevCell.Range.Width > 1 && prevCell.SpanCell == null && prevCell.CellValue == m_cSummaryText)
                            {
                                for (int col = colIndex; col < colIndex + prevCell.Range.Width; col++)
                                {
                                    PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                    for (int row = rowIndex + 1; row <  this.HeaderSection.Height; row++)
                                    {
                                        if (!columnDesc.Cells[row].UniqueName.Contains(MemberTypeEnum.All.ToString()) &&
                                            !columnDesc.Cells[row].CellCaption.Contains(MemberTypeEnum.All.ToString()) &&
                                            columnDesc.Cells[row].CellType != PivotCellDescriptorType.SummaryColumn &&
                                            !columnDesc.Cells[row].UniqueName.Contains(m_cMeasuresSuff) )
                                        {
                                            columnDesc.Cells[row].CellExTypes.Add(PivotCellDescriptorType.ColumnHeader.ToString());
                                            columnDesc.Cells[row].CellType = PivotCellDescriptorType.SummaryColumn;
                                            columnDesc.Cells[row].ExpandableState = ExpandableState.None;
                                            columnDesc.Cells[row].HasChildren = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (cellDesc.CellValue != m_cSummaryText && cellDesc.CellValue == string.Empty && cellDesc.Range.Height > 1
                        && resetSpans)
                    {

                        if (cellDesc.SpanCell != null && parentCell != GetParentCell(cellDesc.SpanCell))
                        {
                            parentCell = cellDesc.SpanCell;
                            GridRangeInfo rangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height, 1);
                            GridRangeInfo parentRangeInfo = GridRangeInfo.FromTlhw(rangeInfo.Top, rangeInfo.Left, rangeInfo.Height, rangeInfo.Width + 1);
                            parentCell.Range = parentRangeInfo;
                        }
                        else if (cellDesc.SpanCell !=null && parentCell == GetParentCell(cellDesc.SpanCell))
                        {
                            GridRangeInfo parentRangeInfo = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height, parentCell.Range.Width + 1);
                            parentCell.Range = parentRangeInfo;                            
                        }
                    }
                }
            }
        }

        private PivotCellDescriptor GetParentCell(PivotCellDescriptor cellDesc)
        {
            while (cellDesc.SpanCell != null)
            {
                cellDesc = cellDesc.SpanCell;                
            }
            return cellDesc;
        }

        /// <summary>
        /// Sets the totals signs.
        /// </summary>
        /// <param name="summaryPos">The summary pos.</param>
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
                                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn && cellDesc.CellExTypes.Count == 0)
                                {
                                    if (prevCell.CellType == PivotCellDescriptorType.ColumnHeader || isSortOrFilter || prevCell.CellExTypes.Contains(PivotCellDescriptorType.ColumnHeader.ToString())
                                        && prevCell.Tag is Member)
                                        totalCell = cellDesc;
                                    else
                                        cellToClear = cellDesc;
                                }
                            }
                            else if (summaryPos == SummaryLayout.Top.ToString())
                            {
                                if (prevCell.CellType == PivotCellDescriptorType.SummaryColumn && prevCell.CellExTypes.Count == 0)
                                {
                                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader || cellDesc.CellExTypes.Contains(PivotCellDescriptorType.ColumnHeader.ToString())
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
                                {
                                    maintenanceCell.CellValue = m_cSummaryText;

                                    if (maintenanceCell.Range.Width > 1 && maintenanceCell.SpanCell == null)
                                    {
                                        if (summaryPos == SummaryLayout.Bottom.ToString())
                                        {
                                            for (int col = colIndex; col < colIndex + maintenanceCell.Range.Width; col++)
                                            {
                                                PivotColumnDescriptor columnDesc = this.TableColumns[col];
                                                for (int row = rowIndex + 1; row < this.HeaderSection.Height; row++)
                                                {
                                                    if (!columnDesc.Cells[row].UniqueName.Contains(MemberTypeEnum.All.ToString()) &&
                                                        !columnDesc.Cells[row].CellCaption.Contains(MemberTypeEnum.All.ToString()) &&
                                                        columnDesc.Cells[row].CellType != PivotCellDescriptorType.SummaryColumn)
                                                    {
                                                        columnDesc.Cells[row].CellExTypes.Add(PivotCellDescriptorType.ColumnHeader.ToString());
                                                        columnDesc.Cells[row].CellType = PivotCellDescriptorType.SummaryColumn;
                                                        columnDesc.Cells[row].ExpandableState = ExpandableState.None;
                                                        columnDesc.Cells[row].HasChildren = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    maintenanceCell.CellValue = string.Empty;
                                }

                                int totalColIndex = colIndex;
                                if (maintenanceCell == prevCell)
                                    totalColIndex -= prevCell.Range != null ? prevCell.Range.Width : 1;
                                PivotCellDescriptor currentCell = null;

                                //if (setTotalStyles)
                                for (int colNumber = totalColIndex; colNumber < totalColIndex + maintenanceCell.Range.Width; colNumber++)
                                {
                                    if (colNumber < this.TableColumns.Count)
                                    {
                                        PivotColumnDescriptor colDesc = this.TableColumns[colNumber];

                                        for (int rowCellIndex = rowIndex; rowCellIndex < colDesc.Cells.Count; rowCellIndex++)
                                        {
                                            currentCell = colDesc.Cells[rowCellIndex];

                                            if (!currentCell.CellExTypes.Contains(type))
                                            {
                                                if (maintenanceCell == totalCell && currentCell.CellType == PivotCellDescriptorType.Value)
                                                    currentCell.CellExTypes.Add(type);
                                            }
                                            else
                                                if (maintenanceCell == cellToClear)
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

        /// <summary>
        /// Sets the rows summary position.
        /// </summary>
        /// <param name="summaryPos">The summary pos.</param>
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

        /// <summary>
        /// Sets the column summary position
        /// </summary>
        /// <param name="summaryPos">The summary pos.</param>
        public void SetColumnsSummaryPos(SummaryLayout summaryPos)
        {
            if (summaryPos == SummaryLayout.None)
            {
                summaryPos = SummaryLayout.Bottom;
            }

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

        /// <summary>
        /// Gets the valid kpis.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Merges the kpi rows with columns.
        /// </summary>
        /// <param name="kpiInfoCollection">The kpi info collection.</param>
        /// <returns></returns>
        public KpiInfoCollection MergeKpiRowsWithColumns(KpiInfoCollection kpiInfoCollection)
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

        /// <summary>
        /// Gets the kpis axis members.
        /// </summary>
        /// <returns></returns>
        public KpiInfoCollection GetKpisAxisMembers()
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
#if SILVERLIGHT
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
#else
                                if (member != null)
                                {
                                    Property propertyKpi_name = member.Properties.FindByName(PropertyConstants.KPI);
                                    if (propertyKpi_name == null)
                                    {
                                        propertyKpi_name = member.Properties.FindByName(PropertyConstants.VirtualKpiHeaderName);
                                    }
                                    if (propertyKpi_name != null)
                                    {
                                        if (propertyKpi_name.Value is Property)
                                        {
                                            Property property = (Property)propertyKpi_name.Value;
                                            tempKpiInfo.Kpi_Name = property.Value.ToString();
                                        }
                                        else
                                        {
                                            tempKpiInfo.Kpi_Name = propertyKpi_name.Value.ToString();
                                        }
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
#endif
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
#if !SILVERLIGHT
                                    Property propertyKpi_name = member.Properties.FindByName(PropertyConstants.KPI);
                                    if (propertyKpi_name != null)
                                    {
                                        Property property = (Property)propertyKpi_name.Value;
                                        tempKpiInfo.Kpi_Name = property.Value.ToString();
                                    }
#else
                                    if (member.Kpi_Name != null && member.Kpi_Name != string.Empty)
                                    {
                                        tempKpiInfo.Kpi_Name = member.Kpi_Name;

                                    }
#endif
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

        /// <summary>
        /// Gets the kpis.
        /// </summary>
        /// <returns></returns>
        public KpiInfoCollection GetKpis()
        {
            this.RemoveTotalsElements();
            this.ClearLevelHeadersArea();
            KpiInfoCollection kpiColumnCollection = new KpiInfoCollection();
            kpiColumnCollection = GetKpisAxisMembers();
            kpiColumnCollection.RemoveMeasures();
            KpiInfoCollection Kpis = new KpiInfoCollection();
            Kpis = MergeKpiRowsWithColumns(kpiColumnCollection);
            this.IndexCells();
            return (Kpis);
        }

        /// <summary>
        /// Gets the kpi axis.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Checks the range info.
        /// </summary>
        /// <param name="IsMdx">if set to <c>true</c> [is MDX].</param>
        internal void CheckRangeInfo(bool IsMdx)
        {
            if (this.m_summaryPos == SummaryLayout.None)
            {
                if (IsMdx)
                {
                    this.EnsureRangeInfo();
                }
            }
            else
            {
                for (int j = 0; j < this.TableColumns.Count; j++)
                {
                    PivotColumnDescriptor columnDesc = this.TableColumns[j];

                    for (int i = 0; i < columnDesc.Cells.Count; i++)
                    {
                        PivotCellDescriptor cellDesc = columnDesc.Cells[i];

                        if (cellDesc.CellValue == string.Empty && (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn))
                        {
                            if ((i - 1) >= 0)
                            {
                                PivotCellDescriptor parentCell = columnDesc.Cells[i - 1];
                                if (parentCell.CellValue != m_cSummaryText)
                                {
                                    if (parentCell.CellCaption == cellDesc.CellCaption && parentCell.CellCaption != null && cellDesc.CellCaption != null)
                                    {
                                        parentCell.Range = GridRangeInfo.FromTlhw(0, 0, GetRowColCount(columnDesc, parentCell, i - 1, 0, false), parentCell.Range.Right + 1);
                                        cellDesc.SpanCell = parentCell;
                                    }
                                }
                            }
                        }
                        else if (cellDesc.CellValue == string.Empty && (cellDesc.CellType == PivotCellDescriptorType.SummaryRow))
                        {
                            if ((j - 1) >= 0)
                            {
                                PivotCellDescriptor parentCell = this.TableColumns[j - 1].Cells[i];
                                if (parentCell.CellValue != m_cSummaryText)
                                {
                                    if (parentCell.CellCaption == cellDesc.CellCaption && parentCell.CellCaption != null && cellDesc.CellCaption != null)
                                    {
                                        parentCell.Range = GridRangeInfo.FromTlhw(0, 0, parentCell.Range.Right + 1, GetRowColCount(columnDesc, parentCell, j - 1, i, true));
                                        cellDesc.SpanCell = parentCell;

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ensures the range info.
        /// </summary>
        internal void EnsureRangeInfo()
        {
            for (int columnIndex = 0; columnIndex < this.TableColumns.Count; columnIndex++)
            {
                for (int cellIndex = 0; cellIndex < this.HeaderSection.Height; cellIndex++)
                {
                    PivotCellDescriptor childCell = this.TableColumns[columnIndex].Cells[cellIndex];
                    if (childCell.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        if ((cellIndex - 1) >= 0)
                        {
                            PivotCellDescriptor parentCell = this.TableColumns[columnIndex].Cells[cellIndex - 1];

                            if (EnsureMemberDrillDown(parentCell, false, columnIndex, cellIndex - 1))
                            {
                                if (childCell.CellCaption == parentCell.CellCaption)
                                {
                                    if (childCell.SpanCell == null)
                                    {
                                        childCell.CellValue = m_cSummaryText;
                                    }
                                    else
                                    {
                                        if (EnsureMemberDrillDown(childCell.SpanCell, true, columnIndex, cellIndex - 1))
                                            childCell.SpanCell.CellValue = m_cSummaryText;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int j = 0; j < this.TableColumns.Count; j++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[j];

                for (int i = 0; i < columnDesc.Cells.Count; i++)
                {
                    PivotCellDescriptor cellDesc = columnDesc.Cells[i];

                    if (cellDesc.CellValue == string.Empty && (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn))
                    {
                        if ((i - 1) >= 0)
                        {
                            PivotCellDescriptor parentCell = columnDesc.Cells[i - 1];
                            if (parentCell.CellValue != m_cSummaryText)
                            {
                                if (parentCell.CellCaption == cellDesc.CellCaption && parentCell.CellCaption != null && cellDesc.CellCaption != null)
                                {
                                    cellDesc.SpanCell = parentCell;
                                    parentCell.Range = GridRangeInfo.FromTlhw(i - 1, j, GetRowColCount(columnDesc, parentCell, i - 1, 0, false), parentCell.Range.Right + 1);
                                }
                            }
                        }
                    }
                    else if (cellDesc.CellValue == string.Empty && (cellDesc.CellType == PivotCellDescriptorType.SummaryRow))
                    {
                        if ((j - 1) >= 0)
                        {
                            PivotCellDescriptor parentCell = this.TableColumns[j - 1].Cells[i];
                            if (parentCell.CellValue != m_cSummaryText)
                            {
                                if (parentCell.CellCaption == cellDesc.CellCaption && parentCell.CellCaption != null && cellDesc.CellCaption != null)
                                {
                                    cellDesc.SpanCell = parentCell;
                                    parentCell.Range = GridRangeInfo.FromTlhw(i, j - 1, parentCell.Range.Right + 1, GetRowColCount(columnDesc, parentCell, j - 1, i, true));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ensures the member drill down.
        /// </summary>
        /// <param name="parentCell">The parent cell.</param>
        /// <param name="Option">if set to <c>true</c> [option].</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns></returns>
        private bool EnsureMemberDrillDown(PivotCellDescriptor parentCell, bool Option, int colIndex, int cellIndex)
        {
            Member m_member = null;
            if (!Option)
            {
                if (parentCell.SpanCell == null)
                {
                    PivotCellDescriptor cellDesc = this.TableColumns[colIndex + parentCell.Range.Right].Cells[cellIndex];
                    m_member = (Member)cellDesc.Tag;
                }
                else
                    m_member = (Member)parentCell.SpanCell.Tag;

                return m_member == null || m_member.DrilledDown;
            }
            else
            {
                while (cellIndex >= 0)
                {
                    PivotCellDescriptor cellDesc = this.TableColumns[colIndex].Cells[cellIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader && cellDesc.CellCaption == parentCell.CellCaption)
                    {
                        if (cellDesc.SpanCell == null)
                        {
                            m_member = (Member)cellDesc.Tag;
                        }
                        else
                            m_member = (Member)cellDesc.SpanCell.Tag;

                        return m_member == null || m_member.DrilledDown;
                    }
                    cellIndex--;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines the row/col count.
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        /// <param name="parentCell">The parent cell.</param>
        /// <param name="parentIndex">Index of the parent.</param>
        /// <param name="childIndex">Index of the child.</param>
        /// <param name="isRow">if set to <c>true</c> [is row].</param>
        /// <returns></returns>
        private int GetRowColCount(PivotColumnDescriptor columnDesc, PivotCellDescriptor parentCell, int parentIndex, int childIndex, bool isRow)
        {
            int rowCount = 0;
            if (!isRow)
            {
                if (parentCell.CellValue != m_cSummaryText)
                {
                    for (int i = parentIndex + 1; i < columnDesc.Cells.Count; i++)
                    {
                        if (columnDesc.Cells[i].CellType == PivotCellDescriptorType.SummaryColumn && columnDesc.Cells[i].CellCaption == parentCell.CellValue)
                        {
                            rowCount++;
                        }
                        else
                            break;
                    }

                    return rowCount + 1;
                }
                return 0;
            }
            else
            {
                if (parentCell.CellValue != m_cSummaryText)
                {
                    for (int i = parentIndex + 1; i < this.TableColumns.Count; i++)
                    {
                        if (this.TableColumns[i].Cells[childIndex].CellType == PivotCellDescriptorType.SummaryRow && this.TableColumns[i].Cells[childIndex].CellCaption == parentCell.CellValue)
                        {
                            rowCount++;
                        }
                        else
                            break;
                    }
                    return rowCount + 1;
                }
                return 0;
            }
        }

        /// <summary>
        /// Recalculates the span if summary position is set to Bottom
        /// </summary>
        internal void RecalculateNoSummaryLayoutSpan()
        {
            for (int row = 0; row < this.HeaderSection.Height; row++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(row);
                for (int cell = this.RowHeaderSection.Width; cell < rowDesc.Cells.Count; cell++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[cell];

                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn && cellDesc.SpanCell == null)
                    {
                        PivotColumnDescriptor columnDesc = this.TableColumns[cell];

                        for (int column = 0; column < cellDesc.Range.Width; column++)
                        {
                            for (int parent = 0; parent < this.HeaderSection.Height; parent++)
                            {
                                PivotCellDescriptor parentCell = columnDesc.Cells[parent];
                                if (parentCell.SpanCell != null)
                                {
                                    parentCell = parentCell.SpanCell;
                                }
                                if (parentCell.CellType == PivotCellDescriptorType.ColumnHeader)
                                {
                                    if (parentCell != null && parentCell.Range != GridRangeInfo.Empty)
                                    {
                                        parentCell.Range = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height, parentCell.Range.Width - 1);
                                    }
                                }
                            }
                            this.TableColumns.RemoveAt(cell);
                            this.HeaderSection = GridRangeInfo.FromTlhw(this.HeaderSection.Top, this.HeaderSection.Left, this.HeaderSection.Height, this.HeaderSection.Width - 1);
                        }
                        rowDesc = this.GetRowAt(row);
                    }
                }
            }
            for (int row = this.HeaderSection.Height; row < this.RowsCount; row++)
            {
                PivotRowDescriptor rowDesc = this.GetRowAt(row);
                for (int cell = 0; cell < this.RowHeaderSection.Width; cell++)
                {
                    PivotCellDescriptor cellDesc = rowDesc.Cells[cell];
                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow && cellDesc.SpanCell == null)
                    {
                        for (int previousCell = 0; previousCell < this.RowHeaderSection.Width; previousCell++)
                        {
                            PivotCellDescriptor parentCell = rowDesc.Cells[previousCell];
                            if (parentCell.SpanCell != null)
                            {
                                parentCell = parentCell.SpanCell;
                            }
                            if (parentCell.SpanCell == null)
                                parentCell.Range = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height - 1, parentCell.Range.Width);
                        }
                        this.RemoveRowAt(row);
                        this.RowHeaderSection = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top, this.RowHeaderSection.Left, this.RowHeaderSection.Height - 1, this.RowHeaderSection.Width);
                    }
                }
            }
        }

        #endregion

        #region ICloneable<PivotTableDescriptor> Members
        /// <summary>
        /// Clones table descriptor.
        /// </summary>
        /// <returns>Cloned descriptor.</returns>
        /// <remarks>
        /// Some public properties of this class are not getting cloned,
        /// they should be regenerated or recalculated in cloned PivotTableDescriptor.
        /// </remarks>
        public PivotEngine Clone()
        {
            PivotEngine cloneTable = new PivotEngine();
            cloneTable.SummaryPosition = this.SummaryPosition;
            cloneTable.HeaderSection = this.HeaderSection.Clone() as GridRangeInfo;

            foreach (PivotColumnDescriptor pcd in this.TableColumns)
                cloneTable.TableColumns.Add(pcd.Clone());

            cloneTable.CrossLevelExpand = this.CrossLevelExpand;

            return cloneTable;
        }
        #endregion

        #region Member Properties Implementation



        /// <summary>
        /// Clears the level header span.
        /// </summary>
        internal void ClearLevelHeaderSpan()
        {
            GridRangeInfo rangeInfo = this.TableColumns[0].Cells[0].Range;
            for (int col = 0; col < rangeInfo.Width; col++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[col];
                for (int cell = 0; cell < rangeInfo.Height; cell++)
                {
                    columnDesc.Cells[cell].Range = GridRangeInfo.Empty;
                    columnDesc.Cells[cell].SpanCell = null;
                }
            }
        }

        /// <summary>
        /// Processes the engine with member properties.
        /// </summary>
        /// <param name="rowAxis">The row axis.</param>
        /// <param name="headerRows">The header rows.</param>
        /// <param name="rowMembers">The row members.</param>
        internal void ProcessEngineWithMemberProperties(Axis rowAxis, int headerRows, int rowMembers)
        {
            bool MemberProperty = false;
            if (rowAxis != null)
            {
                if (rowAxis.TupleSet.Count > 0)
                {
                    foreach (var item in rowAxis.TupleSet[0].Members)
                    {
                        if (item.MemberProperties.Count > 0)
                        {
                            MemberProperty = true;
                            break;
                        }
                    }
                }
            }

            if (MemberProperty)
            {
                this.ClearLevelHeaderSpan();
                int additionalCols = 0;
                for (int column = 0; column < rowMembers; column++)
                {
                    PivotColumnDescriptor columnDesc = this.TableColumns[column];

                    for (int cell = headerRows; cell < columnDesc.Cells.Count; )
                    {
                        PivotCellDescriptor headerCell = columnDesc.Cells[cell];
                        if (headerCell.CellType != PivotCellDescriptorType.RowHeader)
                        {
                            headerCell = this.GetHeaderCell(columnDesc, cell + 1);
                        }
                        if (headerCell != null)
                        {
                            Member m_Member = headerCell.Tag as Member;
                            if (m_Member != null)
                            {
                                if (m_Member.Type != MemberTypeEnum.All)
                                {
                                    for (int memberProperties = 0; memberProperties < m_Member.MemberProperties.Count; memberProperties++)
                                    {
                                        PivotColumnDescriptor columnDescriptor = new PivotColumnDescriptor(this.TableColumns[0].Cells.Count);
                                        columnDescriptor.Cells[headerRows - 1].CellValue = m_Member.MemberProperties[memberProperties].Name;
                                        columnDescriptor.Cells[headerRows - 1].CellType = PivotCellDescriptorType.RowHeader;
                                        this.TableColumns[column].Cells[headerRows - 1].CellType = PivotCellDescriptorType.SummaryColumn;
                                        columnDescriptor = this.ProcessMemberValues(column, columnDescriptor, headerRows, m_Member.MemberProperties[memberProperties].Name);
                                        this.TableColumns.Insert(column + memberProperties + 1, columnDescriptor);
                                        additionalCols++;
                                    }
                                    PivotCellDescriptor headerEmptyCell = this.TableColumns[column].Cells[headerRows - 1];//.CellValue = string.Empty;
                                    headerEmptyCell.CellValue = string.Empty;
                                    headerEmptyCell.CellType = PivotCellDescriptorType.SummaryColumn;
                                    if (column > 0)
                                    {
                                        if (headerEmptyCell.CellValue == string.Empty)
                                        {
                                            if (this.TableColumns[column - 1].Cells[headerRows - 1].CellValue == string.Empty)
                                            {
                                                PivotCellDescriptor parentCell = this.TableColumns[column-1].Cells[headerRows-1];
                                                while (parentCell.SpanCell != null)
                                                {
                                                    parentCell = parentCell.SpanCell;
                                                }
                                                parentCell.Range = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height, parentCell.Range.Width + 1);
                                                headerEmptyCell.SpanCell = parentCell;
                                            }
                                        }
                                    }
                                    column += m_Member.MemberProperties.Count;
                                }
                            }
                        }
                        break;
                    }
                }

                GridRangeInfo rangeInfo = this.RowHeaderSection;
                this.RowHeaderSection = GridRangeInfo.FromTlhw(rangeInfo.Top, rangeInfo.Left, rangeInfo.Height, rangeInfo.Width + additionalCols);
                rangeInfo = this.HeaderSection;
                this.HeaderSection = GridRangeInfo.FromTlhw(rangeInfo.Top, rangeInfo.Left, rangeInfo.Height, rangeInfo.Width + additionalCols);
                this.ReCalculateLevelHeadersSpan();
            }
        }

        /// <summary>
        /// Gets the header cell.
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns></returns>
        private PivotCellDescriptor GetHeaderCell(PivotColumnDescriptor columnDesc, int cellIndex)
        {
            for (int cell = cellIndex; cell < columnDesc.Cells.Count; cell++)
            {
                if (columnDesc.Cells[cell].CellType == PivotCellDescriptorType.RowHeader)
                {
                    return columnDesc.Cells[cell];
                }
            }
            return null;
        }        

        /// <summary>
        /// Processes the member value cells.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="columnDescriptor">The column descriptor.</param>
        /// <param name="headerRows">The header rows.</param>
        /// <param name="memberName">The memberName.</param>
        /// <returns></returns>
        private PivotColumnDescriptor ProcessMemberValues(int column, PivotColumnDescriptor columnDescriptor, int headerRows, string memberName)
        {
            PivotColumnDescriptor memberColDescriptor = this.TableColumns[column];
            for (int cell = headerRows; cell < memberColDescriptor.Cells.Count; cell++)
            {
                PivotCellDescriptor memberCellDescriptor = memberColDescriptor.Cells[cell];
                Member m_Member = memberCellDescriptor.Tag as Member;
                if (m_Member != null)
                {
                    if (memberCellDescriptor.ExpandableState == ExpandableState.Expanded)
                    {
                        object value = m_Member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0).Value;
                        if (value != null)
                        {
                            columnDescriptor.Cells[cell].CellValue = value.ToString();
                        }
                        for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                        {
                            columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                        }
                        columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                        columnDescriptor.Cells[cell].Range = memberCellDescriptor.Range;
                    }
                    else if (memberCellDescriptor.ExpandableState == ExpandableState.Collapsed)
                    {
                        if (!IsExpandedColumn(memberColDescriptor, headerRows))
                        {
                            object value = m_Member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0).Value;
                            if (value != null)
                            {
                                columnDescriptor.Cells[cell].CellValue = value.ToString();
                            }
                            for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                            {
                                columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                            }
                            columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                            columnDescriptor.Cells[cell].Range = memberCellDescriptor.Range;
                        }
                        else
                        {
                            memberCellDescriptor.Range = GridRangeInfo.FromTlhw(memberCellDescriptor.Range.Top, memberCellDescriptor.Range.Left, memberCellDescriptor.Range.Height, memberCellDescriptor.Range.Width + 1);
                            columnDescriptor.Cells[cell].SpanCell = memberCellDescriptor;
                            columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                            for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                            {
                                columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                            }
                        }
                    }
                    else
                    {
                        if (memberCellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            object value = m_Member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0).Value;
                            if (value != null)
                            {
                                columnDescriptor.Cells[cell].CellValue = value.ToString();
                            }
                            for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                            {
                                columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                            }
                            columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                            columnDescriptor.Cells[cell].Range = memberCellDescriptor.Range;
                        }
                    }

                   cell += memberCellDescriptor.Range.Height - 1;                    
                }
                else if (memberCellDescriptor.SpanCell == null && memberCellDescriptor.CellType != PivotCellDescriptorType.RowHeader)
                {
                    memberCellDescriptor.Range = GridRangeInfo.FromTlhw(memberCellDescriptor.Range.Top, memberCellDescriptor.Range.Left, memberCellDescriptor.Range.Height, memberCellDescriptor.Range.Width+1);
                    columnDescriptor.Cells[cell].SpanCell = memberCellDescriptor;

                    for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                    {
                        columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                    }

                    cell += memberCellDescriptor.Range.Height - 1;
                }
                else if (memberCellDescriptor.SpanCell != null)
                {
                    PivotCellDescriptor parentCell = memberCellDescriptor.SpanCell;
                    if (parentCell.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        parentCell.Range = GridRangeInfo.FromTlhw(parentCell.Range.Top, parentCell.Range.Left, parentCell.Range.Height, parentCell.Range.Width + 1);
                        columnDescriptor.Cells[cell].SpanCell = parentCell;                        
                    }
                    else
                    {
                        while (parentCell.SpanCell != null)
                        {
                            parentCell = parentCell.SpanCell;
                        }

                        memberCellDescriptor = parentCell;
                        Member member = memberCellDescriptor.Tag as Member;
                        if (member != null)
                        {
                            if (memberCellDescriptor.ExpandableState == ExpandableState.Expanded)
                            {
                                object value = member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0).Value;
                                if (value != null)
                                {
                                    columnDescriptor.Cells[cell].CellValue = value.ToString();
                                }
                                for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                                {
                                    columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                                }
                                columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                                columnDescriptor.Cells[cell].Range = GridRangeInfo.FromTlhw(memberCellDescriptor.Range.Top, memberCellDescriptor.Range.Left, memberCellDescriptor.Range.Height, 1);
                               
                            }
                            else if (memberCellDescriptor.ExpandableState == ExpandableState.Collapsed)
                            {
                                if (!IsExpandedColumn(memberColDescriptor, headerRows))
                                {
                                    object value = member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0).Value;
                                    if (value != null)
                                    {
                                        columnDescriptor.Cells[cell].CellValue = value.ToString();
                                    }
                                    for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                                    {
                                        columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                                    }
                                    columnDescriptor.Cells[cell].CellType = PivotCellDescriptorType.SummaryRow;
                                    columnDescriptor.Cells[cell].Range = GridRangeInfo.FromTlhw(memberCellDescriptor.Range.Top, memberCellDescriptor.Range.Left, memberCellDescriptor.Range.Height,1);
                                   
                                }
                                else
                                {
                                    memberCellDescriptor.Range = GridRangeInfo.FromTlhw(memberCellDescriptor.Range.Top, memberCellDescriptor.Range.Left, memberCellDescriptor.Range.Height, memberCellDescriptor.Range.Width + 1);
                                    columnDescriptor.Cells[cell].SpanCell = memberCellDescriptor;
                                    for (int subCell = cell + 1; subCell < cell + memberCellDescriptor.Range.Height; subCell++)
                                    {
                                        columnDescriptor.Cells[subCell].SpanCell = columnDescriptor.Cells[cell];
                                    }
                                }
                            }
                            cell += memberCellDescriptor.Range.Height - 1;
                        }
                    }
                }
            }
            return columnDescriptor;
        }

        /// <summary>
        /// Determines whether column contains any expanded cells
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        /// <param name="headerRows">The header rows.</param>
        /// <returns>
        /// 	<c>true</c> if [is expanded column] [the specified column desc]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExpandedColumn(PivotColumnDescriptor columnDesc,int headerRows)
        {
            for (int cell = headerRows; cell < columnDesc.Cells.Count; cell++)
            {                
                PivotCellDescriptor cellDesc = columnDesc.Cells[cell];
                if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                {
                    if (cellDesc.ExpandableState == ExpandableState.Expanded)
                    {
                        return true;
                    }
                }
                cell += cellDesc.Range.Height - 1;
            }
            return false;
        }

        /// <summary>
        /// ReCalculates the Level headers Span
        /// </summary>
        internal void ReCalculateLevelHeadersSpan()
        {
            int height = this.HeaderSection.Height - 1;
            int width = this.RowHeaderSection.Width;
            PivotCellDescriptor topLeftCell = null;

            for (int col = 0; col < width; col++)
            {
                PivotColumnDescriptor columnDesc = this.TableColumns[col];
                for (int cell = 0; cell < height; cell++)
                {
                    PivotCellDescriptor cellDesc = columnDesc.Cells[cell];

                    if (col == 0 && cell == 0)
                    {
                        cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, height, width);
                        topLeftCell = cellDesc;
                    }
                    else
                    {
                        cellDesc.SpanCell = topLeftCell;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the engine with member properties for Excel Layout.
        /// </summary>
        /// <param name="rowAxis">The row axis.</param>
        internal void ProcessEngineWithMemberProperties(Axis rowAxis)
        {
            bool MemberProperty = false;
            if (rowAxis != null)
            {
                if (rowAxis.TupleSet.Count > 0)
                {
                    foreach (var item in rowAxis.TupleSet[0].Members)
                    {
                        if (item.MemberProperties.Count > 0)
                        {
                            MemberProperty = true;
                            break;
                        }
                    }
                }
            }

            if (MemberProperty)
            {
                this.ClearLevelHeaderSpan();
#if !SILVERLIGHT
                Syncfusion.Olap.Data.Tuple tuple = rowAxis.TupleSet[0];
#else
                Syncfusion.OlapSilverlight.Data.Tuple tuple = rowAxis.TupleSet[0];
#endif

                PivotRowDescriptor rowDesc = this.GetRowAt(this.HeaderSection.Bottom);
                PivotCellDescriptor headerCell = null;
                for (int cell = 0; cell < this.RowHeaderSection.Width; cell++)
                {
                    if (cell == 0)
                    {
                        rowDesc.Cells[cell].Range = GridRangeInfo.FromTlhw(0, 0, 1, this.RowHeaderSection.Width);
                        rowDesc.Cells[cell].CellType = PivotCellDescriptorType.RowHeader;
                        rowDesc.Cells[cell].CellValue = string.Empty;
                        headerCell = rowDesc.Cells[cell];
                    }
                    else
                    {
                        rowDesc.Cells[cell].SpanCell = headerCell;
                    }
                }

                for (int member = 0; member < tuple.Members.Count; member++)
                {
                    Member m_member = tuple.Members[member];
                    for (int count = 0; count < m_member.MemberProperties.Count; count++)
                    {
                        PivotColumnDescriptor columnDescriptor = new PivotColumnDescriptor(this.RowsCount);
                        columnDescriptor.Cells[this.HeaderSection.Bottom].CellValue = m_member.MemberProperties[count].Name;
                        columnDescriptor.Cells[this.HeaderSection.Bottom].CellType = PivotCellDescriptorType.RowHeader;
                        columnDescriptor.Cells[this.HeaderSection.Bottom].Level = 1;
                        columnDescriptor = this.ProcessMemberValues(columnDescriptor, m_member.MemberProperties[count].Name);
                        //this.TableColumns.Insert(GetColumnIndex(tuple, member) + count + this.RowHeaderSection.Width-2, columnDescriptor);
                        this.TableColumns.Insert(this.RowHeaderSection.Width + count, columnDescriptor);
                    }
                    this.RowHeaderSection = GridRangeInfo.FromTlhw(this.RowHeaderSection.Top, this.RowHeaderSection.Left, this.RowHeaderSection.Height, this.RowHeaderSection.Width + m_member.MemberProperties.Count);
                    this.HeaderSection = GridRangeInfo.FromTlhw(this.HeaderSection.Top, this.HeaderSection.Left, this.HeaderSection.Height, this.HeaderSection.Width + m_member.MemberProperties.Count);
                }
                this.ReCalculateLevelHeadersSpan();
            }
        }

        /// <summary>
        /// Processes the member values.
        /// </summary>
        /// <returns></returns>
        private PivotColumnDescriptor ProcessMemberValues(PivotColumnDescriptor columnDesc,string memberName)
        {
            for (int index = this.HeaderSection.Bottom + 1; index < columnDesc.Cells.Count; index++)
            {
                PivotCellDescriptor parentCell = this.TableColumns[0].Cells[index];
                Member m_Member = parentCell.Tag as Member;
                PivotCellDescriptor childCell = columnDesc.Cells[index];

                if (m_Member != null)
                {
                    Property val = m_Member.MemberProperties.Select(i => i).Where(j => j.Name == memberName).ElementAtOrDefault(0);
                    if (val != null)
                    {
                        if (val.Value != null)
                        {
                            childCell.CellValue = val.Value.ToString();
                        }
                        else
                        {
                            //childCell.CellValue = m_cNull;
                        }
                    }                 
                }
                childCell.CellType = PivotCellDescriptorType.SummaryRow;
            }
            return columnDesc;
        }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
#if !SILVERLIGHT
        private int GetColumnIndex(Syncfusion.Olap.Data.Tuple tuple, int member)
#else
        private int GetColumnIndex(Syncfusion.OlapSilverlight.Data.Tuple tuple, int member)
#endif
        {
            if (member > 0)
            {
                int memberCount = 0;
                for (int index = 0; index < member; index++)
                {
                    memberCount += tuple.Members[index].MemberProperties.Count;
                }
                return memberCount + 1;
            }
            else
                return 1;
        }
        #endregion        
    }
}
