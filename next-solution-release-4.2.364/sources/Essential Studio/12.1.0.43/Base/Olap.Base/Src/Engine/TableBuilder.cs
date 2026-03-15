//-------------------------------------------------------------------------------------------------
// <copyright file="TableBuilder.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#if !SILVERLIGHT
using System.Data;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Engine.Extension;
using System.Text;
using Syncfusion.Olap.Data;
using Syncfusion.Linq;
using Syncfusion.Olap.DataProvider;
namespace Syncfusion.Olap.Engine
#endif

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.OlapSilverlight.Reports;

using Syncfusion.Olap.Engine.Extension;
using Syncfusion.Linq;
using System.Text;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Manager;
namespace Syncfusion.OlapSilverlight.Engine
#endif

{
    /// <summary>
    /// Table Builder Class used to generate Pivot Engine either from CellSet or RelationalData
    /// </summary>
    public class TableBuilder
    {
        #region Private Variables
        private static Syncfusion.Olap.Engine.Extension.GroupResult childGroupResult
        {
            get;
            set;
        }
        #endregion
        private static bool isSortOrFilterApplied 
        {
            get;
            set;
        }
#if !SILVERLIGHT
        #region Public Methods

        /// <summary>
        /// Builds the PivotEngine from cell set.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineTableFromCellSet(CellSet cellSet)
        {
            return BuildEngineFromCellSet(cellSet, null, SummaryLayout.Bottom, GridLayout.Normal, false, false);
        }

        #endregion
#endif

        #region Private Methods

        /// <summary>
        /// Returns the Maximum Level of the TupleSet
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns></returns>
        public static int GetMaxLevel(Axis cubeAxis, int dimension)
        {
#if !SILVERLIGHT
            return cubeAxis.TupleSet.MaxLevel[dimension];
#else
            return cubeAxis.MaxLevel[dimension];
#endif
        }

        /// <summary>
        /// Returns the Minimum Level of the TupleSet
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns></returns>
        public static int GetMinLevel(Axis cubeAxis, int dimension)
        {
#if !SILVERLIGHT
            return cubeAxis.TupleSet.MinLevel[dimension];
#else
            return cubeAxis.MinLevel[dimension];
#endif
        }

        /// <summary>
        /// Customizes the cell based on the value present in Olap Cell.
        /// </summary>
        /// <param name="cellDesc">The PivotCellDescriptor.</param>
        /// <param name="olapCell">The olap cell.</param>
        private static void CustomizeCell(PivotCellDescriptor cellDesc, Cell olapCell)
        {
#if !SILVERLIGHT
            cellDesc.CellValue = olapCell.FormattedValue;
            cellDesc.FormatString = olapCell.FormatString;
#else
            cellDesc.FormatString = olapCell.FormatString;
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentUICulture;
            if (olapCell.FormatString == "Currency")
            {
                if(olapCell.Value!=null)
                    cellDesc.CellValue = double.Parse(olapCell.Value.ToString()).ToString("C", culture);
            }
            else
                cellDesc.CellValue = olapCell.FormattedValue;
#endif

            double result;
            if (olapCell.Value != null)
            {
                if (Double.TryParse(olapCell.Value.ToString(), out result))
                {
                    cellDesc.Value = result.ToString();
                }
            }
            else
            {
                cellDesc.Value = string.Empty;
            }
            if (cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
            {
                cellDesc.CellType = PivotCellDescriptorType.Value;
            }
            else
            {
                cellDesc.CellType = PivotCellDescriptorType.Value;
                cellDesc.CellExTypes.Add("SummaryRow");
            }
            cellDesc.Tag = olapCell;
        }

        /// <summary>
        /// Returns the current level of Cube Axis
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="memberInd">The member index.</param>
        /// <returns></returns>
        private static int GetCurrentLevel(Axis cubeAxis, int memberInd)
        {
            int currentLevel = 0;

            if (memberInd > 0)
            {
                //List<int> levelsArr = new List<int>();
#if !SILVERLIGHT
                Syncfusion.Olap.Data.Tuple tupleRows = cubeAxis.TupleSet[0];
#else
                Syncfusion.OlapSilverlight.Data.Tuple tupleRows = cubeAxis.TupleSet[0];
#endif

                for (int j = 0; j < memberInd && j < tupleRows.Members.Count; j++)
                {
                    if (!isSortOrFilterApplied)
                        currentLevel += (GetMaxLevel(cubeAxis, j) - GetMinLevel(cubeAxis, j) + 1);
                    else
                    {
                        currentLevel += 2;
                    }
                }
            }

            return currentLevel;
        }

        /// <summary>
        /// Gets the expandable buffer rows.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns></returns>
        private static List<PivotRowDescriptor> GetExpandableBufferRows(Axis cubeAxis, OlapReport olapReport)
        {
            List<PivotRowDescriptor> bufferRows = new List<PivotRowDescriptor>();

            if (cubeAxis != null)
            {
                
               
                {
                    for (int i = 0; i < cubeAxis.TupleSet.Count; )
                    {
#if SILVERLIGHT
                    Syncfusion.OlapSilverlight.Data.Tuple tupleRows = cubeAxis.TupleSet[i];
#else
                        Syncfusion.Olap.Data.Tuple tupleRows = cubeAxis.TupleSet[i];
#endif


                        for (int j = 0; j < tupleRows.Members.Count; j++)
                        {
                            int levelsCount =0;
                            if (olapReport!=null && ValidatePreserveHierarchy(olapReport, cubeAxis.Name))
                                levelsCount += 2;
                            else
                                levelsCount = GetMaxLevel(cubeAxis, j) - GetMinLevel(cubeAxis, j) + 1;
                              
                            PivotRowDescriptor bufferRow = new PivotRowDescriptor();

                            for (int levelInd = 0; levelInd < levelsCount; levelInd++)
                            {
                                PivotCellDescriptor cellDesc = new PivotCellDescriptor();
                                cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                                bufferRow.Cells.Add(cellDesc);
                            }

                            if (levelsCount == 0)
                            {
                                levelsCount++;
                            }

                            bufferRows.Insert(bufferRows.Count, bufferRow);

                        }

                        break;
                    }
                }
            }

            return bufferRows;
        }

        /// <summary>
        /// Gets the level depth.
        /// </summary>
        /// <param name="bufferRows">The buffer rows.</param>
        /// <returns></returns>
        private static int GetLevelDepth(List<PivotRowDescriptor> bufferRows)
        {
            int depth = 0;

            foreach (PivotRowDescriptor rowDesc in bufferRows)
            {
                int cells = rowDesc.Cells.Count;
                if (cells == 0)
                {
                    cells++;
                }

                depth += cells;
            }

            return depth;
        }

        private static bool ValidatePreserveHierarchy(OlapReport olapreport, string Axis)
        {
            int sortElementIndex = -1;
            if (Axis == "Axis0")
                sortElementIndex = CheckSortElements(olapreport.CategoricalElements);
            else if (Axis == "Axis1")
                sortElementIndex = CheckSortElements(olapreport.SeriesElements);
            if (sortElementIndex >= 0)
            {
                SortElement sortElement = null;
                if (Axis == "Axis0")
                    sortElement = olapreport.CategoricalElements[sortElementIndex].ElementValue as SortElement;
                else if (Axis == "Axis1")
                    sortElement = olapreport.SeriesElements[sortElementIndex].ElementValue as SortElement;
#if !SILVERLIGHT
                if (sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BDESC || sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC)
                    return true;
#else
                if (sortElement.SortOrder == Syncfusion.OlapSilverlight.Reports.SortOrder.BDESC || sortElement.SortOrder == Syncfusion.OlapSilverlight.Reports.SortOrder.BASC)
                    return true;
#endif

            }
            if (olapreport.FilterElements.Count > 0)
            {
                foreach (Item filterElement in olapreport.FilterElements)
                {
                    if ((filterElement.Axis == AxisPosition.Categorical && Axis == "Axis0") || (filterElement.Axis == AxisPosition.Series && Axis == "Axis1"))
                        return true;
                }
            }
            return false;
        }

        
        private static int CheckSortElements(Items items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Element sortElement = items[i].ElementValue;
                if (sortElement is SortElement)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Merges the column header.
        /// </summary>
        /// <param name="mainTable">The main table.</param>
        /// <param name="colSection">The col section.</param>
        /// <param name="rowHeaderDepth">The row header depth.</param>
        /// <param name="isExcelLayout">if set to <c>true</c> [is excel layout].</param>
        private static void MergeColumnHeader(PivotEngine mainTable, PivotEngine colSection, int rowHeaderDepth, bool isExcelLayout)
        {
            if (!isExcelLayout)
            {
                for (int colIndex = 0; colIndex < colSection.TableColumns.Count; colIndex++)
                {
                    PivotColumnDescriptor colDesc = colSection.TableColumns[colIndex];

                    for (int rowIndex = 0; rowIndex < colDesc.Cells.Count; rowIndex++)
                    {
                        PivotCellDescriptor cellDesc = colDesc.Cells[rowIndex];
                        mainTable.TableColumns[rowHeaderDepth + colIndex].Cells[rowIndex] = cellDesc;
                    }
                }
            }
            else
            {
                int mainTableColIndex = 0;
                if (rowHeaderDepth != 0)
                {
                    mainTableColIndex = 1;
                }

                for (int colIndex = rowHeaderDepth; colIndex < colSection.TableColumns.Count; colIndex++)
                {
                    PivotColumnDescriptor columnDesc = colSection.TableColumns[colIndex];

                    for (int rowIndex = 0; rowIndex < columnDesc.Cells.Count; rowIndex++)
                    {
                        PivotCellDescriptor cellDesc = columnDesc.Cells[rowIndex];
                        mainTable.TableColumns[mainTableColIndex].Cells[rowIndex] = cellDesc;
                    }

                    mainTableColIndex++;
                }
            }
        }

        /// <summary>
        /// Processes the axis.
        /// </summary>
        /// <param name="pEngine">The p engine.</param>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="_headerRows">The _header rows.</param>
        /// <param name="bufferRows">The buffer rows.</param>
        /// <param name="rowMembers">The row members.</param>
        /// <returns></returns>
        private static List<PivotCellDescriptor> ProcessAxis(PivotEngine pEngine, Axis cubeAxis, int _headerRows, List<PivotRowDescriptor> bufferRows, int rowMembers, OlapReport currentReport)
        {
            List<PivotCellDescriptor> kpiVector = null;

            if (pEngine != null && cubeAxis != null)
            {
                kpiVector = new List<PivotCellDescriptor>(cubeAxis.TupleSet.Count);

                cubeAxis.NormalizeExpandedState();

                for (int i = 0; i < kpiVector.Capacity; i++)
                {
                    kpiVector.Add(null);
                }

                for (int i = 0; i < cubeAxis.TupleSet.Count; i++)
                {
#if SILVERLIGHT
                    Syncfusion.OlapSilverlight.Data.Tuple tupleRows = cubeAxis.TupleSet[i];

#else
                    Syncfusion.Olap.Data.Tuple tupleRows = cubeAxis.TupleSet[i];
#endif
                    for (int j = 0; j < tupleRows.Members.Count; j++)
                    {
                            int currentLevel, initCurrentLevel = 0;
                            isSortOrFilterApplied = false;
                            Member member = tupleRows.Members[j];
                            cubeAxis.UpdateMemberDrillState(member, i, j, tupleRows);
                            PivotRowDescriptor bufferRow = bufferRows[j];
#if !SILVERLIGHT
                            if(ValidatePreserveHierarchy(currentReport, cubeAxis.Name))
                                isSortOrFilterApplied = true;
#endif
                            currentLevel = (int)member.LevelDepth - GetMinLevel(cubeAxis, j);
                            if (isSortOrFilterApplied && !member.UniqueName.Contains("Measures") && !(currentLevel == 0))
                                currentLevel = (int)member.LevelDepth - GetMinLevel(cubeAxis, j) - (int)member.LevelDepth + 1;
                            int dimensionLevel = currentLevel;
                            PivotColumnDescriptor column = pEngine.TableColumns[j];
                            PivotRowDescriptor rowDescriptor = pEngine.GetRowAt(i + _headerRows);
                            PivotCellDescriptor cellDesc = null;
                            initCurrentLevel = GetCurrentLevel(cubeAxis, j);
                            currentLevel += initCurrentLevel;
                            if (bufferRow.Cells.Count > 0)
                            {
                                for (int lev = dimensionLevel; lev < bufferRow.Cells.Count; lev++)
                                {
                                    PivotCellDescriptor cell = new PivotCellDescriptor();
                                    cell.CellType = PivotCellDescriptorType.SummaryRow;
                                    bufferRow.Cells[lev] = cell;
                                }
                                for (int rowIndex = initCurrentLevel; rowIndex < bufferRow.Cells.Count + initCurrentLevel; rowIndex++)
                                {
                                    if (rowIndex != currentLevel)
                                    {
                                        cellDesc = rowDescriptor.Cells[rowIndex];
                                        int tempIndex = rowIndex - initCurrentLevel;
                                        cellDesc.CellType = PivotCellDescriptorType.SummaryRow;

                                        if (bufferRow.Cells.Count > tempIndex)
                                        {
                                            PivotCellDescriptor bufferCell = bufferRow.Cells[tempIndex];
                                            cellDesc.CellValue = bufferCell.CellValue;
                                            cellDesc.Value = bufferCell.Value;
                                            cellDesc.ExpandableState = bufferCell.ExpandableState;
                                            cellDesc.HasChildren = bufferCell.HasChildren;
                                            cellDesc.Tag = bufferCell.Tag;
                                            cellDesc.UniqueName = bufferCell.UniqueName;
                                            cellDesc.CellCaption = member.Caption;
                                            cellDesc.Level = bufferCell.Level;
                                            cellDesc.KpiType = bufferCell.KpiType;
                                            cellDesc.CellType = bufferCell.CellType;
                                            cellDesc.KpiGraphicsStyle = bufferCell.KpiGraphicsStyle;
                                        }
                                    }
                                }
                        }
                        if (rowDescriptor.Cells.Count > currentLevel)
                        {
                            cellDesc = rowDescriptor.Cells[currentLevel];
                            cellDesc.CellCaption = member.Caption;
                            cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                            SetCellStyle(member, cellDesc);
                            if (_headerRows != 0)
                            {
                                PivotCellDescriptor colHeader = pEngine.TableColumns[currentLevel].Cells[_headerRows - 1];
                                SetMembersLevelHeader(member, colHeader);
                            }

                            SetSummaryStructure(rowDescriptor);

                            if (cellDesc.KpiType != KpiTypeEnum.Kpi_None)
                            {
                                kpiVector[i] = cellDesc;
                            }

                            if (!member.UniqueName.Contains("Measures"))
                            {
                                if (bufferRow.Cells.Count - 1 >= dimensionLevel)
                                {
                                    bufferRow.Cells.RemoveAt(dimensionLevel);
                                }

                                bufferRow.Cells.Insert(dimensionLevel, cellDesc.Clone());
                                bufferRow.Cells[dimensionLevel].Tag = cellDesc.Tag;
                            }
                        }
                    }
                }
            }

            return kpiVector;
        }

        /// <summary>
        /// Sets the members level header.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="colHeader">The col header.</param>
        private static void SetMembersLevelHeader(Member member, PivotCellDescriptor colHeader)
        {
            if (colHeader.CellValue == string.Empty)
            {
                colHeader.CellType = PivotCellDescriptorType.ColumnHeader;
                colHeader.UniqueName = member.LevelUniqueName;
                string[] elements = colHeader.UniqueName.Split('.');
                string friendlyName = elements[elements.Length - 1];
                colHeader.CellValue = friendlyName.Trim(new char[] { '[', ']' });
            }
        }

        /// <summary>
        /// Determines the summary structure.
        /// </summary>
        /// <param name="rowDescriptor">The row descriptor.</param>
        private static void SetSummaryStructure(PivotRowDescriptor rowDescriptor)
        {
            if (rowDescriptor != null)
            {
                PivotCellDescriptor spanCell = null;
                int spanLength = 0;

                for (int cellIndex = 0; cellIndex < rowDescriptor.Cells.Count; cellIndex++)
                {
                    PivotCellDescriptor cellDesc = rowDescriptor.Cells[cellIndex];

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader
                        && (cellDesc.ExpandableState == ExpandableState.Collapsed || cellDesc.CellType == PivotCellDescriptorType.RowHeader && cellDesc.ExpandableState == ExpandableState.None && !cellDesc.UniqueName.Contains("All") && (EnsureMemberDrillDown(cellDesc) || isSortOrFilterApplied)))
                    {
                        if (spanCell != null && spanLength > 1)
                        {
                            spanCell.Range = GridRangeInfo.FromTlhw(0, 0, 1, spanLength);
                        }

                        spanCell = cellDesc;
                        spanLength = 1;
                    }

                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow
                        && cellDesc.CellValue == string.Empty)
                    {
                        if (spanCell == null)
                        {
                            spanCell = cellDesc;
                        }
                        else
                        {
                            cellDesc.SpanCell = spanCell;
                        }

                        spanLength++;
                    }
                    else
                    {
                        if (spanCell != null && spanLength > 1)
                        {
                            spanCell.Range = GridRangeInfo.FromTlhw(0, 0, 1, spanLength);
                        }

                        spanCell = null;
                        spanLength = 0;
                    }
                }

                if (spanCell != null && spanLength > 1)
                {
                    spanCell.Range = GridRangeInfo.FromTlhw(0, 0, 1, spanLength);
                    spanCell = null;
                    spanLength = 0;
                }
            }
        }

        /// <summary>
        /// Checks whether the given member is drilled down and the type 
        /// </summary>
        /// <param name="cellDesc">The cell desc.</param>
        /// <returns></returns>
        private static bool EnsureMemberDrillDown(PivotCellDescriptor cellDesc)
        {
            if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
            {
                Member m_member = (Member)cellDesc.Tag;

                if (m_member != null)
                {
                    if (!m_member.DrilledDown && m_member.Type != MemberTypeEnum.All)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the cell style.
        /// </summary>
        /// <param name="olapCell">The olap cell.</param>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        private static void SetCellStyle(object olapCell, PivotCellDescriptor cellDescriptor)
        {
            Member headerMember = olapCell as Member;

            if (headerMember != null)
            {
                cellDescriptor.CellValue = headerMember.Caption;
                cellDescriptor.UniqueName = headerMember.UniqueName;
                cellDescriptor.HasChildren = headerMember.HasChildMembers;
                cellDescriptor.Level = headerMember.LevelDepth;
                cellDescriptor.KpiType = headerMember.KPIType;

                if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Trend)
                {
                    cellDescriptor.KpiGraphicsStyle = headerMember.KPITrendGraphic;
                }
                else if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
                {
                    cellDescriptor.KpiGraphicsStyle = headerMember.KPIStatusGraphic;
                }

                cellDescriptor.Tag = headerMember;

                if (headerMember.HasChildMembers)
                {
                    if (headerMember.DrilledDown)
                    {
                        cellDescriptor.ExpandableState = ExpandableState.Expanded;
                    }
                    else
                    {
                        cellDescriptor.ExpandableState = ExpandableState.Collapsed;
                    }
                }
                else
                {
                    cellDescriptor.ExpandableState = ExpandableState.None;
                }
            }
        }


        #endregion


        /// <summary>
        /// Builds the Engine from cell set.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="summaryLayout">The summary layout.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="IsMDX">if set to <c>true</c> [is MDX].</param>
        /// <returns></returns>
        public static PivotEngine BuildTableFromCellSet(CellSet cellSet, SummaryLayout summaryLayout, GridLayout gridLayout, bool IsMDX)
        {
            return BuildEngineFromCellSet(cellSet, null, summaryLayout, gridLayout, false, IsMDX);
        }

#if !SILVERLIGHT

        private static int GetPreviousMemberPropertiesCount(Syncfusion.Olap.Data.Tuple tuple, int m_Member)
        {
            if (m_Member > 0)
            {
                return tuple.Members[m_Member - 1].MemberProperties.Count;
            }
            else
                return 0;
        }


        private static Axis GetMemberRowAxis(Axis rowAxis)
        {
            for (int i = 0; i < rowAxis.TupleSet.Count; i++)
            {
                for (int j = 0; j < rowAxis.TupleSet[i].Members.Count; j++)
                {
                    if (rowAxis.TupleSet[i].Members[j].MemberProperties.Count > 0)
                    {
                        for (int memberCount = 0; memberCount < rowAxis.TupleSet[i].Members[j].MemberProperties.Count; memberCount++)
                        {
                            Member m_Member = new Member();
                            m_Member.Caption = rowAxis.TupleSet[i].Members[j].MemberProperties[memberCount].Name;
                            Property m_Property = new Property(rowAxis.TupleSet[i].Members[j].MemberProperties[memberCount].Name, rowAxis.TupleSet[i].Members[j].MemberProperties[memberCount].Value);
                            m_Member.Properties.Add(m_Property);
                            m_Member.LevelDepth = 0;
                            rowAxis.TupleSet[i].Members.Insert(j + memberCount + 1, m_Member);

                            if (rowAxis.TupleSet[0].Members.Count != rowAxis.TupleSet.MaxLevel.Count)
                            {
                                rowAxis.TupleSet.MaxLevel.Insert(j + memberCount + 1, 0);
                                rowAxis.TupleSet.MinLevel.Insert(j + memberCount + 1, 0);
                            }
                        }
                    }
                }
            }
            return rowAxis;
        }

        private static void GetTupleMembers(Syncfusion.Olap.Data.Tuple tuple, Axis cubeAxis)
        {
            Syncfusion.Olap.Data.Tuple memberTuple = new Syncfusion.Olap.Data.Tuple();
            for (int m_Member = 0; m_Member < tuple.Members.Count; m_Member++)
            {
                memberTuple.Members.Add(tuple.Members[m_Member]);
                for (int property = 0; property < tuple.Members[m_Member].MemberProperties.Count; property++)
                {
                    //memberTuple.Members.Add(new Member { Caption = tuple.Members[m_Member].MemberProperties[property].Name, LevelDepth = 0 });
                    cubeAxis.TupleSet.MaxLevel.Insert(m_Member + GetPreviousMemberPropertiesCount(tuple, m_Member) + property + 1, 0);
                    cubeAxis.TupleSet.MinLevel.Insert(m_Member + GetPreviousMemberPropertiesCount(tuple, m_Member) + property + 1, 0);

                }
            }
        }

        /// <summary>
        /// Builds the engine with member properties.
        /// </summary>
        /// <param name="expandCell">The expand cell.</param>
        /// <returns></returns>
        internal static PivotEngine BuildEngineWithMemberProperties(PivotCellDescriptor expandCell)
        {
            int memberName = 0;
            int memberValue = 1;
            int headerCount = memberName + memberValue + 1;
            Member m_Member = expandCell.Tag as Member;
            PivotEngine memberEngine = null;

            if (expandCell.CellType == PivotCellDescriptorType.ColumnHeader)
            {
                memberEngine = PivotEngine.CreateEngine(headerCount, m_Member.MemberProperties.Count);

                for (int member = 0; member < m_Member.MemberProperties.Count; member++)
                {
                    memberEngine.TableColumns[member].Cells[memberName].CellValue = m_Member.MemberProperties[member].Name;
                    memberEngine.TableColumns[member].Cells[memberName].CellType = PivotCellDescriptorType.ColumnHeader;
                    memberEngine.TableColumns[member].Cells[memberName].ExpandableState = ExpandableState.Collapsed;
                    memberEngine.TableColumns[member].Cells[memberName].Tag = m_Member.MemberProperties[member];
                    if (m_Member.MemberProperties[member].Value != null)
                    {
                        memberEngine.TableColumns[member].Cells[memberValue].CellValue = m_Member.MemberProperties[member].Value.ToString();
                    }
                    else
                    {
                        memberEngine.TableColumns[member].Cells[memberValue].CellValue = "Null";
                    }
                    memberEngine.TableColumns[member].Cells[memberValue].CellType = PivotCellDescriptorType.Value;
                    memberEngine.TableColumns[member].Cells[memberValue].ExpandableState = ExpandableState.None;
                    memberEngine.TableColumns[member].Cells[memberValue].Tag = m_Member.MemberProperties[member];
                }
            }
            else
            {
                memberEngine = PivotEngine.CreateEngine(m_Member.MemberProperties.Count, headerCount);
                for (int member = 0; member < m_Member.MemberProperties.Count; member++)
                {
                    memberEngine.TableColumns[memberName].Cells[member].CellValue = m_Member.MemberProperties[member].Name;
                    memberEngine.TableColumns[memberName].Cells[member].CellType = PivotCellDescriptorType.RowHeader;
                    memberEngine.TableColumns[memberName].Cells[member].ExpandableState = ExpandableState.Collapsed;
                    memberEngine.TableColumns[memberName].Cells[member].Tag = m_Member.MemberProperties[member];
                    if (m_Member.MemberProperties[memberName].Value != null)
                    {
                        memberEngine.TableColumns[memberValue].Cells[member].CellValue = m_Member.MemberProperties[member].Value.ToString();
                    }
                    else
                    {
                        memberEngine.TableColumns[memberValue].Cells[member].CellValue = "Null";
                    }
                    memberEngine.TableColumns[memberValue].Cells[member].CellType = PivotCellDescriptorType.Value;
                    memberEngine.TableColumns[memberValue].Cells[member].ExpandableState = ExpandableState.None;
                    memberEngine.TableColumns[memberValue].Cells[member].Tag = m_Member.MemberProperties[member];
                }
            }

            return memberEngine;
        }
#endif
        /// <summary>
        /// Builds the Engine from cell set.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="expandedCell">The expanded cell.</param>
        /// <param name="summaryLayout">The summary layout.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="IncludeCellCaption">if set to <c>true</c> [include cell caption].</param>
        /// <param name="IsMdx">if set to <c>true</c> [is MDX].</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromCellSet(CellSet cellSet, Member expandedCell, SummaryLayout summaryLayout, GridLayout gridLayout, bool IncludeCellCaption, bool IsMdx)
        {
            return BuildEngineFromCellSet(cellSet, expandedCell, summaryLayout, gridLayout, IncludeCellCaption, IsMdx, null);
        }

        /// <summary>
        /// Builds the engine from cell set.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="expandedCell">The expanded cell.</param>
        /// <param name="summaryLayout">The summary layout.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="IncludeCellCaption">if set to <c>true</c> [include cell caption].</param>
        /// <param name="IsMdx">if set to <c>true</c> [is MDX].</param>
        /// <param name="dataManager">The data manager.</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromCellSet(CellSet cellSet, Member expandedCell, SummaryLayout summaryLayout, GridLayout gridLayout, bool IncludeCellCaption, bool IsMdx, object dataManager)
        {
            PivotEngine pEngine = null;
            int cellIndex = 0;
            List<PivotCellDescriptor> kpiColVector = null;
            List<PivotCellDescriptor> kpiRowVector = null;

#if !SILVERLIGHT
            OlapDataManager olapDataManager = dataManager as OlapDataManager;
#else
            Syncfusion.OlapSilverlight.Manager.OlapDataManager olapDataManager = dataManager as Syncfusion.OlapSilverlight.Manager.OlapDataManager;
#endif

            if (cellSet != null && cellSet.Axes.Count > 0)// && cellSet.HasValidCells())
            {
#if !SILVERLIGHT
                Axis columnAxis = null;
                Axis rowAxis = null;
#else
                Syncfusion.OlapSilverlight.Data.Axis columnAxis = null;
                Syncfusion.OlapSilverlight.Data.Axis rowAxis = null;
#endif
                if (cellSet.Axes.Count > 0)
                {
                    columnAxis = cellSet.Axes[0];
                }
                if (cellSet.Axes.Count > 1)
                {
                    rowAxis = cellSet.Axes[1];
                }
                GridRangeInfo columnsHeaderArea = GridRangeInfo.Empty;
                GridRangeInfo rowsHeaderArea = GridRangeInfo.Empty;
#if !SILVERLIGHT
                List<PivotRowDescriptor> rowBufferRows = GetExpandableBufferRows(rowAxis, olapDataManager.CurrentReport);
                List<PivotRowDescriptor> colBufferRows = GetExpandableBufferRows(columnAxis, olapDataManager.CurrentReport);
#else
                List<PivotRowDescriptor> rowBufferRows = GetExpandableBufferRows(rowAxis, null);
                List<PivotRowDescriptor> colBufferRows = GetExpandableBufferRows(columnAxis, null);
#endif

                int rowHeaderLength = rowAxis == null || rowAxis.TupleSet.Count == 0 ? 1 : rowAxis.TupleSet.Count;
                int colHeaderLength = columnAxis == null || columnAxis.TupleSet.Count == 0 ? 1 : columnAxis.TupleSet.Count;

                int rowHeaderDepth = GetLevelDepth(rowBufferRows);
                int colHeaderDepth = GetLevelDepth(colBufferRows);
#if !SILVERLIGHT
                int memberProperiesCount = GetMemberPropertiesCount(rowAxis);
#endif

                rowsHeaderArea = GridRangeInfo.FromTlhw(colHeaderDepth, 0, rowHeaderLength, rowHeaderDepth);
                columnsHeaderArea = GridRangeInfo.FromTlhw(0, rowHeaderDepth, colHeaderDepth, colHeaderLength);

                pEngine = PivotEngine.CreateEngine(
                  (colHeaderDepth + rowHeaderLength), (rowHeaderDepth + colHeaderLength)
                  );

                pEngine.SummaryPosition = SummaryLayout.Top;

                PivotEngine colTableBuffer = PivotEngine.CreateEngine((colHeaderLength + rowHeaderDepth), colHeaderDepth);

#if SILVERLIGHT
                 kpiColVector = ProcessAxis(colTableBuffer, columnAxis, rowHeaderDepth, colBufferRows, 0, null);
#else
                pEngine.isSortOrFilter = ((rowAxis!= null && ValidatePreserveHierarchy(olapDataManager.CurrentReport, rowAxis.Name)) || (columnAxis!=null && ValidatePreserveHierarchy(olapDataManager.CurrentReport, columnAxis.Name))); 
                kpiColVector = ProcessAxis(colTableBuffer, columnAxis, rowHeaderDepth, colBufferRows, 0, olapDataManager.CurrentReport);
#endif

                colTableBuffer = colTableBuffer.RevertTransform();

                colTableBuffer.RecalculateSpans();

                MergeColumnHeader(pEngine, colTableBuffer, 0, false);
#if SILVERLIGHT
                kpiRowVector = ProcessAxis(pEngine, rowAxis, colHeaderDepth, rowBufferRows, 0, null);
#else
                kpiRowVector = ProcessAxis(pEngine, rowAxis, colHeaderDepth, rowBufferRows, 0, olapDataManager.CurrentReport);
#endif
                #region ValueCells

                for (int j = 0; j < colHeaderLength; j++)
                {
                    List<int> intList = new List<int>();
                    intList.Add(j);

                    PivotColumnDescriptor column = pEngine.TableColumns[(j + rowHeaderDepth)];
                    bool filled = false;

                    if (rowAxis != null)
                    {
                        for (int i = 0; i < rowHeaderLength; i++)
                        {
                            intList.Add(i);
                            PivotCellDescriptor cellDesc = column.Cells[(i + colHeaderDepth)];
                            cellDesc.Engine = pEngine;
                            filled = true;

#if !SILVERLIGHT
                            Cell olapCell = cellSet.GetCell(intList.ToArray());
#else
                            Cell olapCell = cellSet.CellCollection[intList[0]][intList[1]];// cellIndex];
#endif
                            if (olapCell != null)
                            {
                                CustomizeCell(cellDesc, olapCell);
                                //cellDesc.CellData = pEngine.GetCellDataValue(i + colHeaderDepth, j + rowHeaderDepth);
#if !SILVERLIGHT
                                OverrideFormatString(olapDataManager, olapCell, cellDesc);
#endif
                                PivotCellDescriptor kpiCell = kpiColVector[j];
                                if (kpiCell != null && kpiCell.KpiType != KpiTypeEnum.Kpi_None)
                                {
                                    cellDesc.KpiType = kpiCell.KpiType;
                                }
                                else
                                {
                                    if (kpiRowVector.Count > 0)
                                    {
                                        kpiCell = kpiRowVector[i];

                                        if (kpiCell != null && kpiCell.KpiType != KpiTypeEnum.Kpi_None)
                                        {
                                            cellDesc.KpiType = kpiCell.KpiType;
                                            if (cellDesc.KpiType == KpiTypeEnum.Kpi_Trend || cellDesc.KpiType == KpiTypeEnum.Kpi_Status)
                                            {
                                                if (kpiCell.KpiGraphicsStyle != string.Empty)
                                                {
                                                    cellDesc.KpiGraphicsStyle = kpiCell.KpiGraphicsStyle;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (cellDesc.KpiType == KpiTypeEnum.Kpi_Trend || cellDesc.KpiType == KpiTypeEnum.Kpi_Status)
                                {
                                    kpiCell = kpiColVector[j];

                                    if (kpiCell != null)
                                    {
                                        cellDesc.KpiGraphicsStyle = kpiColVector[j].KpiGraphicsStyle;
                                    }
                                }

                                intList.RemoveAt(1);
                            }

                            if ((i + 1) < rowHeaderLength)
                                cellIndex++;
                        }
                    }

                    if ((j + 1) < colHeaderLength)
                        cellIndex++;

                    if (!filled)
                    {

                        PivotCellDescriptor cellDesc = column.Cells[colHeaderDepth];
                        cellDesc.Engine = pEngine;
#if !SILVERLIGHT
                        Cell olapColCell = cellSet.GetCell(intList.ToArray());
                        CustomizeCell(cellDesc, olapColCell);
                        OverrideFormatString(olapDataManager, olapColCell, cellDesc);
#else
                        if (cellSet.CellCollection.Count > 0)
                        {
                            Cell olapColCell = cellSet.CellCollection[intList[0]][0];
                            CustomizeCell(cellDesc, olapColCell);

                        }
#endif

                    }
                }

                #endregion

                GridRangeInfo zeroZone = GridRangeInfo.FromTlhw(0, 0, colHeaderDepth, rowHeaderDepth);

                pEngine.HeaderSection = GridRangeInfo.UnionRange(zeroZone, columnsHeaderArea);

                pEngine.RowHeaderSection = rowsHeaderArea;

                pEngine.Layout = gridLayout;
#if !SILVERLIGHT
                if (((olapDataManager.CurrentReport.CategoricalElements.Count == 1 && olapDataManager.CurrentReport.CategoricalElements[0].ElementValue is MeasureElements) || (olapDataManager.CurrentReport.CategoricalElements.Count == 2 && 
                    ((olapDataManager.CurrentReport.CategoricalElements[0].ElementValue is MeasureElements && olapDataManager.CurrentReport.CategoricalElements[1].ElementValue is SortElement) ||
                    (olapDataManager.CurrentReport.CategoricalElements[1].ElementValue is MeasureElements && olapDataManager.CurrentReport.CategoricalElements[0].ElementValue is SortElement)))) && pEngine.isSortOrFilter)
                    pEngine.isVertical = true;
                if (((olapDataManager.CurrentReport.SeriesElements.Count == 1 && olapDataManager.CurrentReport.SeriesElements[0].ElementValue is MeasureElements) || (olapDataManager.CurrentReport.SeriesElements.Count == 2 &&
                    ((olapDataManager.CurrentReport.SeriesElements[0].ElementValue is MeasureElements && olapDataManager.CurrentReport.SeriesElements[1].ElementValue is SortElement) || 
                    (olapDataManager.CurrentReport.SeriesElements[1].ElementValue is MeasureElements && olapDataManager.CurrentReport.SeriesElements[0].ElementValue is SortElement)))) && pEngine.isSortOrFilter)
                    pEngine.isHorizontal = true;
#endif
                    pEngine.RecalculateSpans();

                pEngine.SetSummaryRows();

                pEngine.SetTotalsSigns();

                if (gridLayout == GridLayout.NoSummaries)
                {
                    pEngine.SummaryPosition = SummaryLayout.None;
                }
                else if (gridLayout == GridLayout.NormalTopSummary)
                {
                    pEngine.SummaryPosition = SummaryLayout.Top;
                }
                else
                    pEngine.SummaryPosition = summaryLayout;

                if (!IncludeCellCaption)
                {
                    pEngine.RemoveTotalsElements();
                }

                pEngine.ClearLevelHeadersArea();

                if (IsMdx || pEngine.SummaryPosition == SummaryLayout.None)
                {
#if !SILVERLIGHT
                    pEngine.CheckRangeInfo(IsMdx);
#endif
                }

                if (gridLayout == GridLayout.NoSummaries)
                {
                    pEngine.RecalculateNoSummaryLayoutSpan();
                }
                //pEngine.ProcessEngineWithMemberProperties(rowAxis, pEngine.HeaderSection.Height, memberProperiesCount + rowHeaderDepth);
            }
            if (pEngine != null)
                pEngine.IndexCells();
            return pEngine;
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets the member properties count.
        /// </summary>
        /// <param name="m_Axis">The m_ axis.</param>
        /// <returns></returns>
        private static int GetMemberPropertiesCount(Axis m_Axis)
        {
            int memberPropertiesCount = 0;
            if (m_Axis != null)
            {
                if (m_Axis.TupleSet.Count > 0)
                {
                    for (int j = 0; j < m_Axis.TupleSet[0].Members.Count; j++)
                    {
                        memberPropertiesCount += m_Axis.TupleSet[0].Members[j].MemberProperties.Count;
                        memberPropertiesCount *= m_Axis.TupleSet.MaxLevel[j];
                    }
                }
            }
            return memberPropertiesCount;
        }

        private static void OverrideFormatString(OlapDataManager dataManager, Cell olapCell, PivotCellDescriptor cellDesc)
        {
            if (dataManager != null && dataManager.OverrideDefaultFormatStrings)
            {
                if (olapCell.FormatString == "Currency" && olapCell.Value is decimal)
                    cellDesc.CellValue = ((decimal)olapCell.Value).ToString("c", dataManager.Culture.NumberFormat);
                else
                    cellDesc.CellValue = Convert.ToString(olapCell.Value, dataManager.Culture.NumberFormat);
            }
        }

#endif

        #region PivotEngine Generation for Excel Layout

        /// <summary>
        /// Builds the engine from cell setfor excel layout.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="expandedCell">The expanded cell.</param>
        /// <param name="isMdx">if set to <c>true</c> [is MDX].</param>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromCellSetforExcelLayout(CellSet cellSet, Member expandedCell, bool isMdx, GridLayout layout)
        {
            return BuildEngineFromCellSetforExcelLayout(cellSet, expandedCell, isMdx, layout, false, null);
        }

        /// <summary>
        /// Builds the engine from cell setfor excel layout.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="expandedCell">The expanded cell.</param>
        /// <param name="isMdx">if set to <c>true</c> [is MDX].</param>
        /// <param name="layout">The layout.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromCellSetforExcelLayout(CellSet cellSet, Member expandedCell, bool isMdx, GridLayout layout, bool showLevelTypeAll)
        {
            return BuildEngineFromCellSetforExcelLayout(cellSet, expandedCell, isMdx, layout, showLevelTypeAll, null);
        }

        /// <summary>
        /// Builds the engine from cell setfor excel layout.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="expandedCell">The expanded cell.</param>
        /// <param name="isMdx">if set to <c>true</c> [is MDX].</param>
        /// <param name="layout">The layout.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <param name="dataManager">The data manager.</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromCellSetforExcelLayout(CellSet cellSet, Member expandedCell, bool isMdx, GridLayout layout, bool showLevelTypeAll, object dataManager) 
        {
            PivotEngine pEngine = null;
            int cellIndex = 0;
            List<PivotCellDescriptor> kpiColVector = null;
            List<PivotCellDescriptor> kpiRowVector = null;

            if (cellSet != null && cellSet.Axes.Count > 0)// && cellSet.HasValidCells())
            {
                int header = 0;
#if !SILVERLIGHT
                Axis columnAxis = null;
                Axis rowAxis = null;
                var olapDataManager = dataManager as OlapDataManager;
#else
                Syncfusion.OlapSilverlight.Data.Axis columnAxis = null;
                Syncfusion.OlapSilverlight.Data.Axis rowAxis = null;
                var olapDataManager = dataManager as Syncfusion.OlapSilverlight.Manager.OlapDataManager;
#endif
                List<string> Measures = new List<string>();

                if (cellSet.Axes.Count > 0)
                {
                    columnAxis = cellSet.Axes[0];
                }

                if (cellSet.Axes.Count > 1)
                {
                    rowAxis = cellSet.Axes[1];
                }
                List<PivotRowDescriptor> rowBufferRows = GetExpandableBufferRows(rowAxis, olapDataManager.CurrentReport);
                List<PivotRowDescriptor> colBufferRows = GetExpandableBufferRows(columnAxis, olapDataManager.CurrentReport);
                int rowHeaderLength = rowAxis == null || rowAxis.TupleSet.Count == 0 ? 1 : rowAxis.TupleSet.Count;
                int colHeaderLength = columnAxis == null || columnAxis.TupleSet.Count == 0 ? 1 : columnAxis.TupleSet.Count;

                int rowHeaderDepth = GetLevelDepth(rowBufferRows);
                int colHeaderDepth = GetLevelDepth(colBufferRows);
                GridRangeInfo columnsHeaderArea = GridRangeInfo.Empty;

                if (rowAxis != null)
                {
                    Measures = GetMeasures(rowAxis);
                }

                if (columnAxis != null)
                {
                    header = colHeaderDepth;// - GetSummaries(columnAxis);
                }

                if (rowAxis != null)
                {
                    if (IsMeasure(rowAxis))
                    {
#if !SILVERLIGHT
                        pEngine = PivotEngine.CreateEngine((GetRowCount(rowAxis, showLevelTypeAll, olapDataManager.DataProvider.ProviderName) + header + 1 + Measures.Count) - 1, colHeaderLength + 1); 
#else
                        pEngine = PivotEngine.CreateEngine((GetRowCount(rowAxis, showLevelTypeAll, (dataManager as OlapDataManager).ProviderName) + header + 1 + Measures.Count) - 1, colHeaderLength + 1); 
#endif
                        pEngine.RowHeaderSection = GridRangeInfo.FromTlhw(header + 1, 0, pEngine.RowsCount - (header + 1), 1);
#if !SILVERLIGHT
                        pEngine = ProcessEngine(pEngine, rowAxis, (header + 1) - 1, Measures, showLevelTypeAll, olapDataManager.DataProvider.ProviderName); 
#else
                        pEngine = ProcessEngine(pEngine, rowAxis, (header + 1) - 1, Measures, showLevelTypeAll, (dataManager as OlapDataManager).ProviderName);
#endif
                    }
                    else
                    {
#if !SILVERLIGHT
                        pEngine = PivotEngine.CreateEngine(GetRowCount(rowAxis, showLevelTypeAll, olapDataManager.DataProvider.ProviderName) + header + (olapDataManager.CurrentReport.PagerOptions.SeriesCurrentPage == 1 ? 1 : 0), colHeaderLength + 1);
#else
                        pEngine = PivotEngine.CreateEngine(GetRowCount(rowAxis, showLevelTypeAll, (dataManager as OlapDataManager).ProviderName) + header + 1, colHeaderLength + 1);
#endif
                        pEngine.RowHeaderSection = GridRangeInfo.FromTlhw((header + 1) - 1, 0, pEngine.RowsCount - ((header + 1) - 1), 1);
#if !SILVERLIGHT
                        pEngine = ProcessEngine(pEngine, rowAxis, (header + 1) - 1, null, showLevelTypeAll, olapDataManager.DataProvider.ProviderName); 
#else
                        pEngine = ProcessEngine(pEngine, rowAxis, (header + 1) - 1, null, showLevelTypeAll, (dataManager as OlapDataManager).ProviderName);
#endif
                    }
                }
                else
                {
                    pEngine = PivotEngine.CreateEngine(0 + header + 1, colHeaderLength);
                }

                PivotEngine colTableBuffer = null;

                colTableBuffer = PivotEngine.CreateEngine((colHeaderLength + rowHeaderDepth), colHeaderDepth);

                kpiColVector = ProcessAxis(colTableBuffer, columnAxis, rowHeaderDepth, colBufferRows, 0, olapDataManager.CurrentReport);

                colTableBuffer = colTableBuffer.RevertTransform();

                colTableBuffer.RecalculateSpans();

                MergeColumnHeader(pEngine, colTableBuffer, rowHeaderDepth, true);

                kpiRowVector = GetKpiValues(pEngine);

                if (isMdx)
                {
                    PivotEngine tempEngine = BuildEngineFromCellSet(cellSet, expandedCell, SummaryLayout.Bottom, GridLayout.Normal, showLevelTypeAll, true);

                    int colIndex = 1;
                    bool isUpdate = false;

                    for (int colCount = 0; colCount < tempEngine.TableColumns.Count; colCount++)
                    {
                        for (int cell = 0; cell < tempEngine.TableColumns[colCount].Cells.Count; cell++)
                        {
                            PivotCellDescriptor cellDesc = tempEngine.TableColumns[colCount].Cells[cell];

                            if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader || cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                            {
                                if (cellDesc.Level >= 0 || cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    pEngine.TableColumns[colIndex].Cells[cell] = cellDesc;
                                    isUpdate = true;
                                }
                            }
                        }

                        if (isUpdate)
                        {
                            colIndex++;
                        }
                    }
                }

                // Processing Value Cells //
                int temp = 0;
                for (int j = 0; j < colHeaderLength; j++)
                {
                    List<int> intList = new List<int>();
                    intList.Add(j);

                    if (rowAxis != null)
                    {
                        PivotColumnDescriptor column = pEngine.TableColumns[(j + 1)];
                        {
                            cellIndex += temp;
                            for (int i = 0; i < pEngine.RowsCount - pEngine.HeaderSection.Height; i++)
                            {
                                PivotCellDescriptor cellDesc = column.Cells[i + pEngine.HeaderSection.Height];
                                cellDesc.Engine = pEngine;
                                if (cellDesc.RowIndex != -2 && !cellDesc.IsEmpty && cellDesc.RowIndex != -1)
                                {
                                    intList.Add(cellDesc.RowIndex);

                                    //Cell olapCell = cellSet.GetCell(intList.ToArray());

#if !SILVERLIGHT
                                    Cell olapCell = cellSet.GetCell(intList.ToArray());
#else
                                    Cell olapCell =  cellSet.CellCollection[intList[0]][intList[1]];// [cellDesc.RowIndex + cellIndex];
#endif
                                    CustomizeCell(cellDesc, olapCell);
#if !SILVERLIGHT
                                    OverrideFormatString(olapDataManager, olapCell, cellDesc);
#endif
                                    //cellDesc.CellData = pEngine.GetCellDataForExcelLayout(i + pEngine.HeaderSection.Height, j + 1); 

                                    PivotCellDescriptor kpiCell = kpiColVector[j];
                                    if (kpiCell != null && kpiCell.KpiType != KpiTypeEnum.Kpi_None)
                                    {
                                        cellDesc.KpiType = kpiCell.KpiType;
                                    }
                                    else
                                    {
                                        if (kpiRowVector != null && kpiRowVector.Count == pEngine.RowsCount - pEngine.HeaderSection.Height)
                                        {
                                            kpiCell = kpiRowVector[i];

                                            if (kpiCell != null && kpiCell.KpiType != KpiTypeEnum.Kpi_None)
                                            {
                                                cellDesc.KpiType = kpiCell.KpiType;
                                                if (cellDesc.KpiType == KpiTypeEnum.Kpi_Trend || cellDesc.KpiType == KpiTypeEnum.Kpi_Status)
                                                {
                                                    if (kpiCell.KpiGraphicsStyle != string.Empty)
                                                    {
                                                        cellDesc.KpiGraphicsStyle = kpiCell.KpiGraphicsStyle;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (cellDesc.KpiType == KpiTypeEnum.Kpi_Trend || cellDesc.KpiType == KpiTypeEnum.Kpi_Status)
                                    {
                                        kpiCell = kpiColVector[j];

                                        if (kpiCell != null)
                                        {
                                            cellDesc.KpiGraphicsStyle = kpiColVector[j].KpiGraphicsStyle;
                                        }
                                    }

                                    intList.RemoveAt(1);

                                    //if ((i + 1) < rowHeaderLength)
                                    temp++;
                                }
                            }
                        }
                        cellIndex = 0;
                    }
                }

                pEngine.HeaderSection = GridRangeInfo.FromTlhw(0, 0, header, pEngine.TableColumns.Count + 1);

                if (!isMdx)
                {
                    pEngine.ClearTotalsSignsforExcelLayout();
                    pEngine.RecalculateColumnHeaderSpans();
                    pEngine.SetTotalsSigns(SummaryLayout.Top.ToString());
                    pEngine.SetColumnsSummaryPos(SummaryLayout.Bottom);
                    pEngine.ResetSpansforExcelLayout();
                    pEngine.RecalculateColumnHeaderSpans();
                    pEngine.SetTotalsSigns(SummaryLayout.Bottom.ToString());
                    if (!showLevelTypeAll)
                        pEngine.RemoveTotalsElements();
                    pEngine.ClearLevelHeadersArea();

                    if (layout == GridLayout.ExcelLikeLayoutWithMemberProperties)
                    {
                        pEngine.ProcessEngineWithMemberProperties(rowAxis);
                    }
                }
            }
            pEngine.IndexCells();

            return pEngine;
        }

        /// <summary>
        /// Returns the measures from Cube Axis
        /// </summary>
        /// <param name="rowAxis">The row axis.</param>
        /// <returns></returns>
        private static List<string> GetMeasures(Axis rowAxis)
        {
            List<string> Measure = new List<string>();

#if SILVERLIGHT
            foreach (Syncfusion.OlapSilverlight.Data.Tuple tuple in rowAxis.TupleSet)

#else
            foreach (Syncfusion.Olap.Data.Tuple tuple in rowAxis.TupleSet)
#endif
            {
                foreach (Member member in tuple.Members)
                {
                    if (member.UniqueName.Contains("Measures") && !Measure.Contains(member.Caption))
                    {
                        Measure.Add(member.Caption);
                    }
                }
            }

            return Measure;
        }

        /// <summary>
        /// Returns the Summary Elements from the specified Axis
        /// </summary>
        /// <param name="columnAxis">The column axis.</param>
        /// <returns></returns>
        private static int GetSummaries(Axis columnAxis)
        {
            int count = 0;
            if (columnAxis.TupleSet.Count > 0)
            {
                for (int i = 0; i < columnAxis.TupleSet[0].Members.Count; i++)
                {
                    if (columnAxis.TupleSet[0].Members[i].Type == MemberTypeEnum.All)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Determines whether the specified row axis has measure.
        /// </summary>
        /// <param name="rowAxis">The row axis.</param>
        /// <returns>
        /// 	<c>true</c> if the specified row axis has measure; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMeasure(Axis rowAxis)
        {
#if SILVERLIGHT
            foreach (Syncfusion.OlapSilverlight.Data.Tuple tuple in rowAxis.TupleSet) 
#else
            foreach (Syncfusion.Olap.Data.Tuple tuple in rowAxis.TupleSet)
#endif
            {
                foreach (Member member in tuple.Members)
                {
                    if (member.UniqueName.Contains("Measures"))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the number of rows for Creating Engine
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns></returns>
        private static int GetRowCount(Axis cubeAxis, bool showLevelTypeAll, Providers providerName)
        {
            int rowsCount = 0;
            string member = null;
            List<string> Measurelist = null;
            bool checkMeasureMiddle = IsMeasureMiddle(cubeAxis);
            bool checkMeasureLast = IsMeasureEnd(cubeAxis);

            if (IsMeasure(cubeAxis))
            {
                Measurelist = GetMeasures(cubeAxis);
            }

            for (int i = 0; i < cubeAxis.TupleSet.Count; i++)
            {
#if SILVERLIGHT
                Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[i];
#else
                Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[i];
#endif

                if (!showLevelTypeAll && IsSummaryTuple(tuple))
                {
                    continue;
                }

                for (int j = 0; j < tuple.Members.Count; j++)
                {
#if SILVERLIGHT
                    Syncfusion.OlapSilverlight.Data.Tuple previousTuple = GetPreviousTuple(cubeAxis.TupleSet, i, showLevelTypeAll);
#else
                    Syncfusion.Olap.Data.Tuple previousTuple = GetPreviousTuple(cubeAxis.TupleSet, i, showLevelTypeAll);

#endif

                    if (previousTuple != null)
                    {
                        if (providerName == Providers.ActivePivot)
                        {
                            if (previousTuple.Members[j] != null && previousTuple.Members[j].Caption == tuple.Members[j].Caption)
                            {
                                if (!PreviousMeasure(previousTuple, tuple, providerName))
                                {
                                    continue;
                                }
                            }
                        }
                        else 
                        {
                            if (previousTuple.Members[j].Name == tuple.Members[j].Name)
                            {
                                if (!PreviousMeasure(previousTuple, tuple, providerName))
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (checkMeasureMiddle || checkMeasureLast)
                    {
                        member = GetMemberCaption(tuple, tuple.Members[j], j, checkMeasureLast);
                        if (member != null)
                        {
                            if (Measurelist != null)
                            {
                                foreach (var item in Measurelist)
                                {
                                    rowsCount++;
                                }
                            }
                        }
                    }

                    rowsCount++;
                }
            }
            return rowsCount;
        }

        /// <summary>
        /// Determines whether the measure is in between members
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns>
        /// 	<c>true</c> if measure is in between members; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMeasureMiddle(Axis cubeAxis)
        {
            if (cubeAxis.TupleSet.Count > 0)
            {
                for (int i = 0; i < cubeAxis.TupleSet[0].Members.Count; i++)
                {
                    if (cubeAxis.TupleSet[0].Members[i].UniqueName.Contains("Measure"))
                    {
                        if ((i - 1) >= 0 && (i + 1) != cubeAxis.TupleSet[0].Members.Count)
                        {
                            if (!cubeAxis.TupleSet[0].Members[i - 1].UniqueName.Contains("Measure"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether measure is not in between two members and placed at the end in Collection
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns>
        /// 	<c>true</c> if not in between two members ; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMeasureEnd(Axis cubeAxis)
        {
            if (cubeAxis.TupleSet.Count > 0)
            {
                if (cubeAxis.TupleSet[0].Members.Count > 1)
                {
                    if (cubeAxis.TupleSet[0].Members[cubeAxis.TupleSet[0].Members.Count - 1].UniqueName.Contains("Measure"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Determines whether the specified tuple object is SummaryTuple
        /// </summary>
        /// <param name="tupleObj">The tuple obj.</param>
        /// <returns>
        /// 	<c>true</c> if summary tuple; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSummaryTuple(Syncfusion.Olap.Data.Tuple tupleObj)
#else
        /// <summary>
        /// Determines whether the specified tuple object is SummaryTuple
        /// </summary>
        /// <param name="tupleObj">The tuple obj.</param>
        /// <returns>
        /// 	<c>true</c> if summary tuple; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSummaryTuple(Syncfusion.OlapSilverlight.Data.Tuple tupleObj)
#endif

        {
            foreach (var item in tupleObj.Members)
            {
                if (item.Type == MemberTypeEnum.All)
                    return true;
            }

            return false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the previous tuple.
        /// </summary>
        /// <param name="tupleSet">The tuple set.</param>
        /// <param name="currentIndex">Index of the current.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns></returns>
        private static Syncfusion.Olap.Data.Tuple GetPreviousTuple(TupleCollection tupleSet, int currentIndex, bool showLevelTypeAll)
        {
            if (currentIndex == 0)
                return null;
            else
            {
                Syncfusion.Olap.Data.Tuple previousTuple = tupleSet[currentIndex - 1];
                if (!showLevelTypeAll && IsSummaryTuple(previousTuple))
                    return GetPreviousTuple(tupleSet, currentIndex - 1, false);
                return previousTuple;
            }
        }
#else
        /// <summary>
        /// Gets the previous tuple.
        /// </summary>
        /// <param name="tupleSet">The tuple set.</param>
        /// <param name="currentIndex">Index of the current.</param>
        /// <returns></returns>
        private static Syncfusion.OlapSilverlight.Data.Tuple GetPreviousTuple(TupleCollection tupleSet, int currentIndex, bool showLevelTypeAll)
        {
            if (currentIndex == 0)
                return null;
            else
            {
                Syncfusion.OlapSilverlight.Data.Tuple previousTuple = tupleSet[currentIndex - 1];
                if (!showLevelTypeAll && IsSummaryTuple(previousTuple))
                    return GetPreviousTuple(tupleSet, currentIndex - 1, false);
                return previousTuple;
            }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Checks whether the previous member is measure.
        /// </summary>
        /// <param name="previousTuple">The previous tuple.</param>
        /// <param name="tuple">The tuple.</param>
        /// <returns></returns>
        private static bool PreviousMeasure(Syncfusion.Olap.Data.Tuple previousTuple, Syncfusion.Olap.Data.Tuple tuple, Syncfusion.Olap.DataProvider.Providers providerName)
#else
        /// <summary>
        /// Checks whether the previous member is measure.
        /// </summary>
        /// <param name="previousTuple">The previous tuple.</param>
        /// <param name="tuple">The tuple.</param>
        /// <returns></returns>
        private static bool PreviousMeasure(Syncfusion.OlapSilverlight.Data.Tuple previousTuple, Syncfusion.OlapSilverlight.Data.Tuple tuple, Providers providerName)
#endif
        {
            if (providerName == Providers.ActivePivot)
            {
                if (previousTuple.Members[0].Caption != tuple.Members[0].Caption)
                {
                    return true;
                }
            }
            else
            {
                if (previousTuple.Members[0].Name != tuple.Members[0].Name)
                {
                    return true;
                }
            }
            return false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Returns the member caption.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <param name="isMeasureLast">if set to <c>true</c> [is measure last].</param>
        /// <returns></returns>
        private static string GetMemberCaption(Syncfusion.Olap.Data.Tuple tuple, Member member, int memberIndex, bool isMeasureLast)
#else
        /// <summary>
        /// Returns the member caption.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <param name="isMeasureLast">if set to <c>true</c> [is measure last].</param>
        /// <returns></returns>
        private static string GetMemberCaption(Syncfusion.OlapSilverlight.Data.Tuple tuple, Member member, int memberIndex, bool isMeasureLast)
#endif
        {
            int measureIndex = 0;

            if (tuple.Members.Count > 0)
            {
                for (int i = 0; i < tuple.Members.Count; i++)
                {
                    if (tuple.Members[i].UniqueName.Contains("Measure"))
                    {
                        measureIndex = i;
                    }
                }

                if (isMeasureLast)
                {
                    if (memberIndex < measureIndex - 1)
                    {
                        return member.Caption;
                    }
                }
                else
                {
                    if (memberIndex < measureIndex)
                    {
                        return member.Caption;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Processes the engine and fills the row headers.
        /// </summary>
        /// <param name="pEngine">The Pivot engine.</param>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="cRow">The current row.</param>
        /// <param name="Measures">The measures.</param>
        /// <returns></returns>
        private static PivotEngine ProcessEngine(PivotEngine pEngine, Axis cubeAxis, int cRow, List<string> Measures, bool showLevelTypeAll, Providers providerName)
        {
            int count = 0;
            int measureCount = 0;
            int currentColumn = 0;
            int currentRow = cRow;
            bool drillDown = false;
            bool LayoutChange = CheckMeasure(cubeAxis);
            bool isMeasureMiddle = IsMeasureMiddle(cubeAxis);
            bool isMeasureEnd = IsMeasureEnd(cubeAxis);
            List<string> memberList = new List<string>();  //cubeAxis = InterChange(cubeAxis);            

            for (int i = 0; i < cubeAxis.TupleSet.Count; i++)
            {
#if !SILVERLIGHT
                Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[i];
#else
                Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[i];
#endif

                if (!showLevelTypeAll && IsSummaryTuple(tuple))
                {
                    continue;
                }

                for (int j = 0; j < tuple.Members.Count; j++)
                {
#if !SILVERLIGHT
                    Syncfusion.Olap.Data.Tuple previousTuple = GetPreviousTuple(cubeAxis.TupleSet, i, showLevelTypeAll);
#else
                    Syncfusion.OlapSilverlight.Data.Tuple previousTuple = GetPreviousTuple(cubeAxis.TupleSet, i, showLevelTypeAll);
#endif

                    if (previousTuple != null)
                    {
                        if (providerName == Providers.ActivePivot)
                        {
                            if (previousTuple.Members[j] != null && previousTuple.Members[j].Caption == tuple.Members[j].Caption)
                            {
                                if (!PreviousMeasure(previousTuple, tuple, providerName))
                                {
                                    continue;
                                }
                            }
                        }
                        else 
                        {
                            if (previousTuple.Members[j].Name == tuple.Members[j].Name)
                            {
                                if (!PreviousMeasure(previousTuple, tuple, providerName))
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (LayoutChange)
                    {
                        if (count < Measures.Count)
                        {
                            if (tuple.Members[j].UniqueName.Contains(Measures[count]))
                            {
                                count++;
                                if (count > 1)
                                {
                                    pEngine[currentRow, currentColumn].CellCaption = Measures[measureCount];
                                    pEngine[currentRow, currentColumn].CellValue = Measures[measureCount] + " Total";
                                    pEngine[currentRow, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                                    pEngine[currentRow, currentColumn].CellExTypes.Add("RowHeader");
                                    pEngine[currentRow, currentColumn].UniqueName = Measures[measureCount];
                                    MarkSummary(pEngine, currentRow);
                                    int tupleOrdinalPosition = GetTupleOrdinalPosition(cubeAxis, 0, Measures, 0, Measures[measureCount], true);
                                    MarkRowIndex(pEngine, currentRow, tupleOrdinalPosition);
                                    FillKpiElements(pEngine[currentRow, currentColumn], cubeAxis, Measures[measureCount]);
                                    measureCount++;
                                    currentRow++;
                                }
                            }
                        }
                    }

                    if (isMeasureMiddle || isMeasureEnd)
                    {
                        string member = GetMemberCaption(tuple, tuple.Members[j], j, isMeasureEnd);

                        if (memberList != null && memberList.Count > 0)
                        {
                            if (memberList[memberList.Count - 1] != tuple.Members[memberList.Count - 1].Caption)// || tuple.OrdinalPosition == cubeAxis.TupleSet.Count-1)
                            {
                                foreach (string item in Measures)
                                {
                                    pEngine[currentRow, currentColumn].CellValue = memberList[memberList.Count - 1] + " " + item;
                                    pEngine[currentRow, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                                    pEngine[currentRow, currentColumn].CellExTypes.Add("RowHeader");
                                    pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, false) + tuple.Members[j].LevelDepth + 1;
                                    pEngine[currentRow, currentColumn].UniqueName = Measures[measureCount];
                                    MarkSummary(pEngine, currentRow);
                                    int tupleOrdinalPosition = GetTupleOrdinalPosition(cubeAxis, i, memberList, memberList.Count - 1, item, false);
                                    MarkRowIndex(pEngine, currentRow, tupleOrdinalPosition);
                                    FillKpiElements(pEngine[currentRow, currentColumn], cubeAxis, Measures[measureCount]);
                                    currentRow++;
                                }
                                memberList.RemoveAt(memberList.Count - 1);
                            }
                        }

                        if (memberList != null && memberList.Count > 0)
                        {
                            if (memberList[memberList.Count - 1] != tuple.Members[memberList.Count - 1].Caption)// || tuple.OrdinalPosition == cubeAxis.TupleSet.Count)
                            {
                                foreach (string item in Measures)
                                {
                                    pEngine[currentRow, currentColumn].CellValue = memberList[memberList.Count - 1] + " " + item;
                                    pEngine[currentRow, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                                    pEngine[currentRow, currentColumn].CellExTypes.Add("RowHeader");
                                    pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, false) + tuple.Members[j].LevelDepth;
                                    pEngine[currentRow, currentColumn].UniqueName = Measures[measureCount];
                                    MarkSummary(pEngine, currentRow);
                                    int tupleOrdinalPosition = GetTupleOrdinalPosition(cubeAxis, i, memberList, memberList.Count - 1, item, false);
                                    MarkRowIndex(pEngine, currentRow, tupleOrdinalPosition);

                                    FillKpiElements(pEngine[currentRow, currentColumn], cubeAxis, Measures[measureCount]);

                                    currentRow++;
                                }
                                memberList.RemoveAt(memberList.Count - 1);
                            }
                        }
                        if (member != null)
                        {
                            memberList.Add(member);
                        }
                    }

                    pEngine[currentRow, currentColumn].CellValue = tuple.Members[j].Caption;
                    pEngine[currentRow, currentColumn].CellCaption = tuple.Members[j].Caption;
                    drillDown = CheckLastMemberDrillDown(cubeAxis, tuple, i, j);
                    //pEngine[currentRow, currentColumn].ParentCellValues = GetParentCellValues(tuple, j);

                    if (tuple.Members[j].LevelDepth == 0)
                    {
                        pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, showLevelTypeAll) + tuple.Members[j].LevelDepth + 1;
                    }
                    else
                    {
                        if (showLevelTypeAll)
                            pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, showLevelTypeAll) + tuple.Members[j].LevelDepth + 1;
                        else
                            pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, showLevelTypeAll) + tuple.Members[j].LevelDepth;
                    }

                    pEngine[currentRow, currentColumn].UniqueName = tuple.Members[j].UniqueName;
                    pEngine[currentRow, currentColumn].Tag = tuple.Members[j];
                    pEngine[currentRow, currentColumn].KpiType = tuple.Members[j].KPIType;
                    pEngine[currentRow, currentColumn].KpiGraphicsStyle = tuple.Members[j].KPIStatusGraphic;
                    pEngine[currentRow, currentColumn].CellType = PivotCellDescriptorType.RowHeader;

                    if (tuple.Members[j].DrilledDown || drillDown)
                    {
                        if (tuple.Members[j].HasChildMembers)
                        {
                            pEngine[currentRow, currentColumn].HasChildren = true;
                            pEngine[currentRow, currentColumn].ExpandableState = ExpandableState.Expanded;
                            pEngine.GetRowAt(currentRow).Cells.ForEach<PivotCellDescriptor>(cell => cell.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString()));
                        }
                    }
                    else
                    {
                        if (tuple.Members[j].HasChildMembers)
                        {
                            pEngine[currentRow, currentColumn].HasChildren = true;
                            pEngine[currentRow, currentColumn].ExpandableState = ExpandableState.Collapsed;
                        }

                        else
                            pEngine[currentRow, currentColumn].ExpandableState = ExpandableState.None;
                    }

                    if (tuple.Members[j].UniqueName.Contains("Measure"))
                    {
                        //Mark IsEmpty
                        MarkCellEmpty(pEngine, tuple, currentRow, tuple.Members[j], isMeasureMiddle, j, Measures);
                    }

                    int ordinalPosition = GetTupleOrdinalPosition(cubeAxis, tuple.Members[j].Caption, i, j);
                    MarkRowIndex(pEngine, currentRow, ordinalPosition);
                    currentRow++;

                    if (tuple.OrdinalPosition == cubeAxis.TupleSet.Count - 1)
                    {
                        if (isMeasureMiddle || isMeasureEnd)
                        {
                            if (memberList != null && memberList.Count > 0)
                            {
                                for (int k = memberList.Count - 1; k >= 0; k--)
                                {
                                    foreach (string item in Measures)
                                    {
                                        pEngine[currentRow, currentColumn].CellValue = memberList[k] + " " + item;
                                        pEngine[currentRow, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                                        pEngine[currentRow, currentColumn].CellExTypes.Add("RowHeader");
                                        if (k != memberList.Count - 1)
                                        {
                                            pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, false) + tuple.Members[j].LevelDepth;
                                        }
                                        else
                                            pEngine[currentRow, currentColumn].Level = GetLevelDepth(tuple, j, showLevelTypeAll) + tuple.Members[j].LevelDepth + 1;
                                        pEngine[currentRow, currentColumn].UniqueName = Measures[measureCount];
                                        MarkSummary(pEngine, currentRow);
                                        int tupleOridnalPosition = GetTupleOrdinalPosition(cubeAxis, i, memberList, memberList.Count - 1, item, false);
                                        MarkRowIndex(pEngine, currentRow, tupleOridnalPosition);
                                        FillKpiElements(pEngine[currentRow, currentColumn], cubeAxis, Measures[measureCount]);

                                        currentRow++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Measures != null)
            {
                if (Measures.Count > 1)
                {
                    if (!LayoutChange)
                    {
                        for (int i = 0; i < Measures.Count; i++)
                        {
                            pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn].CellCaption = Measures[i];
                            pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn].CellValue = Measures[i] + " Total";
                            pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                            pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn].CellExTypes.Add("RowHeader");
                            pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn].UniqueName = Measures[i];// "[Measure]" + "." + "[" + Measures[i] + "]";
                            MarkSummary(pEngine, pEngine.RowsCount - (Measures.Count - i));
                            int tupleOrdinalPosition = GetTupleOrdinalPosition(cubeAxis, 0, Measures, 0, Measures[i], true);
                            MarkRowIndex(pEngine, pEngine.RowsCount - (Measures.Count - i), tupleOrdinalPosition);
                            FillKpiElements(pEngine[pEngine.RowsCount - (Measures.Count - i), currentColumn], cubeAxis, Measures[i]);
                        }
                    }
                    else
                    {
                        pEngine[pEngine.RowsCount - (1), currentColumn].CellCaption = Measures[Measures.Count - 1];
                        pEngine[pEngine.RowsCount - (1), currentColumn].CellValue = Measures[Measures.Count - 1] + " Total";
                        pEngine[pEngine.RowsCount - (1), currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                        pEngine[pEngine.RowsCount - (1), currentColumn].CellExTypes.Add("RowHeader");
                        pEngine[pEngine.RowsCount - (1), currentColumn].UniqueName = "[Measure]" + "." + "[" + Measures[Measures.Count - 1] + "]";// Measures[Measures.Count - 1];
                        MarkSummary(pEngine, pEngine.RowsCount - (1));
                        int tupleOrdinalPosition = GetTupleOrdinalPosition(cubeAxis, 0, Measures, 0, Measures[Measures.Count - 1], true);
                        MarkRowIndex(pEngine, pEngine.RowsCount - (1), tupleOrdinalPosition);
                        FillKpiElements(pEngine[pEngine.RowsCount - (1), currentColumn], cubeAxis, Measures[Measures.Count - 1]);

                    }
                }
                else
                {
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellCaption = Measures[0];
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellValue = "Grand Total";
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellExTypes.Add("RowHeader");
                    pEngine[pEngine.RowsCount - 1, currentColumn].UniqueName = "[Measure]" + "." + "[" + Measures[0] + "]";// Measures[0];
                    MarkSummary(pEngine, pEngine.RowsCount - 1);
                    FillKpiElements(pEngine[pEngine.RowsCount - 1, currentColumn], cubeAxis, Measures[0]);

                    bool IsUpdate = true;
                    if (cubeAxis.TupleSet.Count > 0)
                    {
                        foreach (Member item in cubeAxis.TupleSet[0].Members)
                        {
                            if (item.Type != MemberTypeEnum.All && !item.UniqueName.Contains("Measure"))
                            {
                                IsUpdate = false;
                                break;
                            }
                        }
                    }
                    if (IsUpdate)
                        MarkRowIndex(pEngine, pEngine.RowsCount - 1, 0);
                }
            }
            else
            {
                if (pEngine[pEngine.RowsCount - 1, currentColumn].CellType == PivotCellDescriptorType.Any)
                {
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellCaption = "Total";
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellValue = "Grand Total";
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellType = PivotCellDescriptorType.SummaryRow;
                    pEngine[pEngine.RowsCount - 1, currentColumn].CellExTypes.Add("RowHeader");
                    MarkSummary(pEngine, pEngine.RowsCount - 1);
                    bool IsUpdate = true;
                    if (cubeAxis.TupleSet.Count > 0)
                    {
                        foreach (Member item in cubeAxis.TupleSet[0].Members)
                        {
                            if (item.Type != MemberTypeEnum.All && !item.UniqueName.Contains("Measure"))
                            {
                                IsUpdate = false;
                                break;
                            }
                        }
                    }
                    if (IsUpdate)
                        MarkRowIndex(pEngine, pEngine.RowsCount - 1, 0);
                }
            }

            return pEngine;
        }


#if !SILVERLIGHT
        /// <summary>
        /// Gets the parent cell values.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <returns></returns>
        private static List<string> GetParentCellValues(Syncfusion.Olap.Data.Tuple tuple, int memberIndex)
        {
            List<string> parentCells = new List<string>();
            for (int i = 0; i <= memberIndex; i++)
            {
                Member olapMember = tuple.Members[i];
                if (tuple.Members[i].Properties.Count > 0)
                {
                    Microsoft.AnalysisServices.AdomdClient.Member adomdMember = (Microsoft.AnalysisServices.AdomdClient.Member)olapMember.Properties[0].Value;
                    int index = 0;
                    if (parentCells.Count > 0)
                    {
                        index = parentCells.Count;
                    }
                    parentCells.Add(adomdMember.Caption);
                    int count = 0;
                    while (adomdMember.Parent != null)
                    {
                        if (adomdMember.Parent.Type != Microsoft.AnalysisServices.AdomdClient.MemberTypeEnum.All)
                        {
                            parentCells.Add(adomdMember.Parent.Caption);
                        }
                        adomdMember = adomdMember.Parent;
                        count++;
                    }
                    parentCells.Reverse(index, count);
                }
            }
            return parentCells;
        }
#endif

        /// <summary>
        /// Fills the kpi elements.
        /// </summary>
        /// <param name="pivotCellDescriptor">The pivot cell descriptor.</param>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="measureValue">The measure value.</param>
        private static void FillKpiElements(PivotCellDescriptor pivotCellDescriptor, Axis cubeAxis, string measureValue)
        {
            for (int i = 0; i < cubeAxis.TupleSet.Count; i++)
            {
#if !SILVERLIGHT
                Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[i];
#else
                Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[i];
#endif

                for (int j = 0; j < tuple.Members.Count; j++)
                {
#if !SILVERLIGHT
                    Syncfusion.Olap.Data.Member member = tuple.Members[j];
#else
                    Syncfusion.OlapSilverlight.Data.Member member = tuple.Members[j];
#endif
                    if (member.UniqueName.Contains("Measure") && member.Caption == measureValue)
                    {
                        if (!CheckMemberType(tuple, j))
                        {
                            pivotCellDescriptor.KpiType = member.KPIType;
                            pivotCellDescriptor.KpiGraphicsStyle = member.KPIStatusGraphic;
                        }
                    }
                }
            }

        }

#if SILVERLIGHT
        /// <summary>
        /// Checks the type of the member.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="measureIndex">Index of the measure.</param>
        /// <returns></returns>
        private static bool CheckMemberType(Syncfusion.OlapSilverlight.Data.Tuple tuple, int measureIndex)
            
#else

        /// <summary>
        /// Checks the type of the member.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        /// <param name="measureIndex">Index of the measure.</param>
        /// <returns></returns>
        private static bool CheckMemberType(Syncfusion.Olap.Data.Tuple tuple, int measureIndex)
#endif
        {
            for (int i = 0; i < tuple.Members.Count; i++)
            {
                if (i != measureIndex)
                {
#if SILVERLIGHT
                    if (tuple.Members[i].Type != MemberTypeEnum.All)
#else
                    if (tuple.Members[i].Type != Syncfusion.Olap.Data.MemberTypeEnum.All)
#endif
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the current Axis has Measure.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns></returns>
        private static bool CheckMeasure(Axis cubeAxis)
        {
            if (cubeAxis.TupleSet.Count > 0 && cubeAxis.TupleSet[0].Members.Count > 0)
            {
                if (cubeAxis.TupleSet[0].Members[0].UniqueName.Contains("Measure"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the summary.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <param name="rowIndex">Index of the row.</param>
        private static void MarkSummary(PivotEngine pEngine, int rowIndex)
        {
            PivotRowDescriptor rowDesc = pEngine.GetRowAt(rowIndex);

            foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
            {
                if (cellDesc.CellExTypes.Count > 0)
                {
                    if (!cellDesc.CellExTypes[0].Contains("RowHeader"))
                    {
                        cellDesc.CellType = PivotCellDescriptorType.Value;
                        cellDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                    }
                }

                else
                    cellDesc.CellType = PivotCellDescriptorType.Value;
                cellDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
            }
        }

        /// <summary>
        /// Gets the tuple's ordinal position .
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="tupleIndex">Index of the tuple.</param>
        /// <param name="memberList">The member list.</param>
        /// <param name="memIndex">Index of the member.</param>
        /// <param name="measureName">Name of the measure.</param>
        /// <param name="measureTotal">if set to <c>true</c> [measure total].</param>
        /// <returns></returns>
        private static int GetTupleOrdinalPosition(Axis cubeAxis, int tupleIndex, List<string> memberList, int memIndex, string measureName, bool measureTotal)
        {
            if (!measureTotal)
            {
                for (int tupleCount = tupleIndex - 1; tupleCount > 0; tupleCount--)
                {
#if SILVERLIGHT
                    Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[tupleCount];

#else
                    Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[tupleCount];
#endif
                    StringBuilder str1 = new StringBuilder();
                    StringBuilder str2 = new StringBuilder();

                    for (int memberIndex = 0; memberIndex < memberList.Count; memberIndex++)
                    {
                        str1.Append(tuple.Members[memberIndex].Caption);
                        str2.Append(memberList[memberIndex]);
                    }

                    if (str1.ToString() == str2.ToString())
                    {
#if SILVERLIGHT
                        Syncfusion.OlapSilverlight.Data.Tuple child = cubeAxis.TupleSet[tupleCount];

#else
                        Syncfusion.Olap.Data.Tuple child = cubeAxis.TupleSet[tupleCount];
#endif
                        for (int membercount = memberList.Count - 1; membercount < child.Members.Count; membercount++)
                        {
                            if (!child.Members[membercount].UniqueName.Contains("Measure") && child.Members[membercount].Type == MemberTypeEnum.All && child.ToString().Contains(measureName))
                            {
                                return tupleCount;
                            }
                        }
                    }
                }
            }
            else
            {
                int measureIndex = -1;

                if (cubeAxis.TupleSet.Count > 0)
                {
                    if (cubeAxis.TupleSet[0].Members.Count > 0)
                    {
                        for (int i = 0; i < cubeAxis.TupleSet[0].Members.Count; i++)
                        {
                            if (cubeAxis.TupleSet[0].Members[i].UniqueName.Contains("Measure"))
                            {
                                measureIndex = i;
                                break;
                            }
                        }
                    }
                }

                if (measureIndex != -1)
                {
                    for (int i = 0; i < cubeAxis.TupleSet.Count; i++)
                    {
                        bool IsUpdate = false;
#if SILVERLIGHT
                        Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[i];
#else
                        Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[i];
#endif
                        if (measureIndex < tuple.Members.Count)
                        {
                            if (tuple.Members[measureIndex].Caption.Contains(measureName))
                            {
                                for (int j = 0; j < tuple.Members.Count; j++)
                                {
                                    if (tuple.Members[j].Type == MemberTypeEnum.All && !tuple.Members[j].UniqueName.Contains("Measure"))
                                    {
                                        IsUpdate = true;
                                    }
                                    else if (!tuple.Members[j].UniqueName.Contains("Measure") && tuple.Members[j].Type != MemberTypeEnum.All)
                                    {
                                        IsUpdate = false;
                                        break;
                                    }
                                    else if (tuple.Members[j].UniqueName.Contains("Measure"))
                                    {
                                        IsUpdate = true;
                                        break;
                                    }
                                }
                            }
                            else
                                continue;
                        }
                        if (IsUpdate)
                            return tuple.OrdinalPosition;
                        else
                            return -2;

                    }
                }

            }
            return -2;
        }

        /// <summary>
        /// Gets the tuple ordinal position.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="memberCaption">The member caption.</param>
        /// <param name="tupleIndex">Index of the tuple.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <returns></returns>
        private static int GetTupleOrdinalPosition(Axis cubeAxis, String memberCaption, int tupleIndex, int memberIndex)
        {
            bool isUpdate = false;
            if (!cubeAxis.TupleSet[tupleIndex].Members[memberIndex].UniqueName.Contains("Measure"))
            {
                for (int i = tupleIndex; i >= 0; i--)
                {
#if SILVERLIGHT
                    Syncfusion.OlapSilverlight.Data.Tuple tuple = cubeAxis.TupleSet[i];

#else
                    Syncfusion.Olap.Data.Tuple tuple = cubeAxis.TupleSet[i];
#endif
                    isUpdate = false;
                    for (int member = memberIndex + 1; member < tuple.Members.Count; member++)
                    {
                        if (cubeAxis.TupleSet[i].Members[member].LevelDepth != 0)
                        {
                            isUpdate = true;
                            break;
                        }
                    }
                    if (!isUpdate)
                        return i;
                }
            }

            return tupleIndex;
        }

        /// <summary>
        /// Marks the index of the row.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="OrdinalPosition">The ordinal position.</param>
        private static void MarkRowIndex(PivotEngine pEngine, int currentRow, int OrdinalPosition)
        {
            pEngine.GetRowAt(currentRow).Cells.ForEach<PivotCellDescriptor>(cell =>
                {
                    if (cell.CellType != PivotCellDescriptorType.RowHeader)// && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                    {
                        if (!cell.IsEmpty || cell.RowIndex != -2)
                        {
                            cell.RowIndex = OrdinalPosition;
                        }
                    }
                }
            );
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the level depth.
        /// </summary>
        /// <param name="tupleObj">The tuple obj.</param>
        /// <param name="currentIndex">Index of the current.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns></returns>
        private static int GetLevelDepth(Syncfusion.Olap.Data.Tuple tupleObj, int currentIndex, bool showLevelTypeAll)
#else
        /// <summary>
        /// Gets the level depth.
        /// </summary>
        /// <param name="tupleObj">The tuple obj.</param>
        /// <param name="currentIndex">Index of the current.</param>
        /// <returns></returns>
        private static int GetLevelDepth(Syncfusion.OlapSilverlight.Data.Tuple tupleObj, int currentIndex, bool showLevelTypeAll)
#endif
        {
            int levelDepth = 0;
            for (int i = 0; i < currentIndex; i++)
            {
                levelDepth += tupleObj.Members[i].LevelDepth;
            }
            if (showLevelTypeAll)
                return levelDepth + currentIndex;
            else
                return levelDepth;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Checks the last member drill down.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="tuple">The tuple.</param>
        /// <param name="tupleIndex">Index of the tuple.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <returns></returns>
        private static bool CheckLastMemberDrillDown(Axis cubeAxis, Syncfusion.Olap.Data.Tuple tuple, int tupleIndex, int memberIndex)
#else
        /// <summary>
        /// Checks the last member drill down.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="tuple">The tuple.</param>
        /// <param name="tupleIndex">Index of the tuple.</param>
        /// <param name="memberIndex">Index of the member.</param>
        /// <returns></returns>
        private static bool CheckLastMemberDrillDown(Axis cubeAxis, Syncfusion.OlapSilverlight.Data.Tuple tuple, int tupleIndex, int memberIndex)
#endif
        {
            for (int i = tupleIndex; i < cubeAxis.TupleSet.Count; i++)
            {
                if (cubeAxis.TupleSet[i].Members[memberIndex].UniqueName == tuple.Members[memberIndex].UniqueName)
                {
                    if (cubeAxis.TupleSet[i].Members[memberIndex].DrilledDown)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Marks the cell empty.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <param name="tuple">The tuple.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="tupleMember">The tuple member.</param>
        /// <param name="ismeasureMiddle">if set to <c>true</c> [ismeasure middle].</param>
        /// <param name="mIndex">Index of the measure.</param>
        /// <param name="Measures">The measures.</param>
        private static void MarkCellEmpty(PivotEngine pEngine, Syncfusion.Olap.Data.Tuple tuple, int currentRow, Member tupleMember, bool ismeasureMiddle, int mIndex, List<string> Measures)
#else
        /// <summary>
        /// Marks the cell empty.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <param name="tuple">The tuple.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="tupleMember">The tuple member.</param>
        /// <param name="ismeasureMiddle">if set to <c>true</c> [ismeasure middle].</param>
        /// <param name="mIndex">Index of the measure.</param>
        /// <param name="Measures">The measures.</param>
        private static void MarkCellEmpty(PivotEngine pEngine, Syncfusion.OlapSilverlight.Data.Tuple tuple, int currentRow, Member tupleMember, bool ismeasureMiddle, int mIndex, List<string> Measures)
#endif
        {
            if (tuple.Members.Count > 0)
            {
                if (tuple.Members[0].UniqueName.Contains("Measure"))
                {
                    PivotRowDescriptor rowDesc = pEngine.GetRowAt(currentRow);

                    foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                    {
                        if (cellDesc.CellType != PivotCellDescriptorType.RowHeader && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                        {
                            cellDesc.IsEmpty = true;
                            cellDesc.CellValue = " ";
                            cellDesc.RowIndex = -2;
                        }
                    }
                }

                else
                {
                    if (!ismeasureMiddle)
                    {
                        for (int memb = tuple.Members.Count - 1; memb >= 0; memb--)
                        {
                            if (tuple.Members[memb] != tupleMember)
                            {
                                PivotRowDescriptor rowDesc = pEngine.GetRowAt(currentRow);

                                if (rowDesc.Cells.Count > 0)
                                {
                                    if (rowDesc.Cells[0].UniqueName.Contains("Measure"))
                                    {
                                        break;
                                    }
                                }

                                foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                                {
                                    if (cellDesc.CellType != PivotCellDescriptorType.RowHeader && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                                    {
                                        cellDesc.IsEmpty = true;
                                        cellDesc.CellValue = " ";
                                        cellDesc.RowIndex = -2;
                                    }
                                }

                            }
                            currentRow--;
                        }
                    }
                    else if (tupleMember.UniqueName.Contains(Measures[0]))
                    {
                        for (int memb = mIndex; memb >= 0; memb--)
                        {
                            PivotRowDescriptor rowDesc = pEngine.GetRowAt(currentRow);

                            foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
                            {
                                if (cellDesc.CellType != PivotCellDescriptorType.RowHeader && cellDesc.CellType != PivotCellDescriptorType.SummaryRow)
                                {
                                    cellDesc.IsEmpty = true;
                                    cellDesc.CellValue = " ";
                                    cellDesc.RowIndex = -2;
                                }
                            }
                            currentRow--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the kpi values.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <returns></returns>
        private static List<PivotCellDescriptor> GetKpiValues(PivotEngine pEngine)
        {
            List<PivotCellDescriptor> KpiVector = new List<PivotCellDescriptor>();

            if (pEngine.RowHeaderSection != GridRangeInfo.Empty)
            {
                for (int i = pEngine.RowHeaderSection._top - 1; i < pEngine.RowsCount; i++)
                {
                    if (pEngine.TableColumns[0].Cells[i].UniqueName.Contains("Measure"))
                    {
                        PivotCellDescriptor kpiCell = pEngine.TableColumns[0].Cells[i];
                        //int count = GetChildren(pEngine.TableColumns[0], i) + 1;
                        //for (int j = 0; j < count; j++)
                        //{
                        KpiVector.Add(kpiCell);
                        //}
                    }
                    else
                    {
                        if (pEngine.TableColumns[0].Cells[i].CellType != PivotCellDescriptorType.Any)
                        {
                            KpiVector.Add(pEngine.TableColumns[0].Cells[i]);
                        }
                    }
                }

                int c = 0;
                for (int i = 0; i < pEngine.RowHeaderSection.Top; i++)
                {
                    PivotCellDescriptor memberCell = pEngine.TableColumns[1].Cells[i];
#if SILVERLIGHT
                    Member member = memberCell.Tag as Member;
                    if (member != null && member.Type == MemberTypeEnum.All) 
                    {
                        KpiVector.Insert(c, memberCell);
                        c++;
                    }      
#else
                    Syncfusion.Olap.Data.Member member = memberCell.Tag as Syncfusion.Olap.Data.Member;
                    if (member != null && member.Type == Syncfusion.Olap.Data.MemberTypeEnum.All)
                    {
                        KpiVector.Insert(c, memberCell);
                        c++;
                    }
#endif

                }

                return KpiVector;
            }
            return null;
        }

        /// <summary>
        /// Returns the number of child elements
        /// </summary>
        /// <param name="pivotColumnDescriptor">The pivot column descriptor.</param>
        /// <param name="i">The index.</param>
        /// <returns></returns>
        private static int GetChildren(PivotColumnDescriptor pivotColumnDescriptor, int i)
        {
            int count = 0;
            int index = 0;
            for (index = i + 1; index < pivotColumnDescriptor.Cells.Count; index++)
            {
                if (pivotColumnDescriptor.Cells[index].UniqueName.Contains("Measure"))// || index==pivotColumnDescriptor.Cells.Count)
                {
                    return count;
                }
                count++;
            }
            if (index == pivotColumnDescriptor.Cells.Count)
            {
                return count;
            }
            return -1;
        }

        /// <summary>
        /// Sets the summary for excel layout.
        /// </summary>
        /// <param name="pEngine">The p engine.</param>
        /// <param name="maxLevel">The max level.</param>
        /// <param name="isMdx">if set to <c>true</c> [is MDX].</param>
        private static void SetSummaryforExcelLayout(PivotEngine pEngine, int maxLevel, bool isMdx)
        {
            for (int row = 0; row < maxLevel; row++)
            {
                PivotRowDescriptor rowDescriptor = pEngine.GetRowAt(row);

                for (int cell = 0; cell < rowDescriptor.Cells.Count; cell++)
                {
                    PivotCellDescriptor cellDesc = rowDescriptor.Cells[cell];

                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn && cellDesc.SpanCell == null)
                    {
                        //if (CheckMemberDrilledDown(pEngine,row,cell))
                        {
                            cellDesc.CellValue = "Total";
                            int colSpan = GetChildCount(pEngine, row, cell, cellDesc);

                            if (colSpan > 0)
                            {
                                if (cellDesc.Range != GridRangeInfo.Empty)
                                {
                                    cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, cellDesc.Range.Bottom + 1, colSpan + 1);
                                }
                                else
                                    cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, 1, colSpan + 1);

                                for (int cel = cell + 1; cel < (cell + cellDesc.Range.Right + 1); cel++)
                                {
                                    PivotCellDescriptor childCell = rowDescriptor.Cells[cel];
                                    childCell.SpanCell = cellDesc;
                                    childCell.Range = GridRangeInfo.Empty;
                                }

                            }

                            SetSummary(pEngine.TableColumns[cell]);
                        }
                    }

                    if (cellDesc.CellValue == cellDesc.CellCaption && cellDesc.SpanCell == null)
                    {
                        int colSpan = GetChildCount(pEngine, row, cell, cellDesc);
                        if (colSpan > 0)
                        {
                            if (cellDesc.Range != GridRangeInfo.Empty)
                            {
                                cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, cellDesc.Range.Bottom + 1, colSpan + 1);
                            }
                            else
                                cellDesc.Range = GridRangeInfo.FromTlhw(0, 0, 1, colSpan + 1);

                            for (int cel = cell + 1; cel < (cell + cellDesc.Range.Right + 1); cel++)
                            {
                                PivotCellDescriptor childCell = rowDescriptor.Cells[cel];
                                childCell.SpanCell = cellDesc;
                                childCell.Range = GridRangeInfo.Empty;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the member drilled down.
        /// </summary>
        /// <param name="pEngine">The pivot engine.</param>
        /// <param name="col">The column.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private static bool CheckMemberDrilledDown(PivotEngine pEngine, int col, int row)
        {
            if (col - 1 < 0)
            {
                return true;
            }
            else
            {
                PivotCellDescriptor cellDesc = pEngine[col - 1, row];
                if (cellDesc.Range != GridRangeInfo.Empty)
                {
                    PivotCellDescriptor memberCell = pEngine[col - 1, (pEngine[col - 1, row].Range.Right + row)];
                    Member m_Member = (Member)memberCell.Tag;
                    if (m_Member != null)
                    {
                        if (m_Member.DrilledDown && m_Member.Type != MemberTypeEnum.All)
                        {
                            return true;
                        }
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        /// <param name="parentCell">The parent cell.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private static int GetChild(PivotColumnDescriptor columnDesc, PivotCellDescriptor parentCell, int row)
        {
            int count = 0;
            for (int cellIndex = row; cellIndex < columnDesc.Cells.Count; cellIndex++)
            {
                if (columnDesc.Cells[cellIndex].CellCaption == parentCell.CellCaption)
                {
                    count++;
                }
                else
                    break;
            }
            return count + 1;
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <param name="pEngine">The p engine.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="headerCell">The header cell.</param>
        /// <returns></returns>
        private static int GetChildCount(PivotEngine pEngine, int colIndex, int cellIndex, PivotCellDescriptor headerCell)
        {
            int childCount = 0;
            int temp1 = cellIndex;
            int temp2 = cellIndex;
            PivotRowDescriptor rowDesc = pEngine.GetRowAt(colIndex);

            for (int cell = cellIndex + 1; cell < rowDesc.Cells.Count; cell++)
            {
                if (headerCell.Tag != null && rowDesc.Cells[cell].Tag != null)
                {
                    if (((Member)headerCell.Tag).UniqueName == ((Member)rowDesc.Cells[cell].Tag).UniqueName)
                    {
                        if (temp1 != 0)
                        {
                            if ((temp1 + 1) == cell)
                            {
                                childCount++;
                                temp1 = cell;
                            }
                        }

                    }
                    else if (childCount == 0)
                        return 0;
                }
                else if (headerCell.CellType == PivotCellDescriptorType.SummaryColumn)
                {
                    if (headerCell.CellCaption == rowDesc.Cells[cell].CellCaption)
                    {
                        if (temp2 != 0)
                        {
                            if ((temp2 + 1) == cell)
                            {
                                childCount++;
                                temp2 = cell;
                            }
                        }
                    }
                    else if (childCount == 0)
                        return 0;
                }
            }

            return childCount;
        }

        /// <summary>
        /// Sets the summary for categories.
        /// </summary>
        /// <param name="columnDesc">The column desc.</param>
        private static void SetSummary(PivotColumnDescriptor columnDesc)
        {
            foreach (PivotCellDescriptor item in columnDesc.Cells)
            {
                if (item.CellType != PivotCellDescriptorType.ColumnHeader)
                {
                    item.CellExTypes.Add(PivotCellDescriptorType.SummaryColumn.ToString());
                }
            }
        }

        #endregion

        #region Engine Generation for Relational Data Source

#if !SILVERLIGHT

        /// <summary>
        /// Pivot Engine Generation for Ilistsource.
        /// </summary>
        /// <param name="listSource">The list source.</param>
        /// <param name="CurrentReport">The current report.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="columnGroup">The column group.</param>
        /// <param name="rowGroup">The row group.</param>
        /// <param name="summaryInfos">The summary infos.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <param name="layout">The layout.</param>
        /// <param name="ExpandAll">if set to <c>true</c> [expand all].</param>
        /// <param name="summaryStringCount">The summary string count.</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <param name="drillDown">if set to <c>true</c> [drill down].</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromIListSource(IListSource listSource, OlapReport CurrentReport, SortType sortOrder, string[] columnGroup, string[] rowGroup, SummaryInfo[] summaryInfos, bool isRowSummary, GridLayout layout, bool ExpandAll, int summaryStringCount, PivotCellDescriptor drilledCell, bool drillDown)
        {
            //// Will the number of summaries, cause the grid layout to be changes.??
            bool isLayoutChanged = summaryInfos.Length > 1 ? true : false;

            //// Generate summary string
            string[] summaryGroup = new string[rowGroup.Length + columnGroup.Length];
            //// For calculating the column summaries
            string[] columnSummaryGroup = new string[rowGroup.Length + columnGroup.Length];

            //// Adding row values to the summary
            for (int i = 0; i < rowGroup.Length; i++)
            {
                summaryGroup[i] = rowGroup[i];
            }

            //// Adding column values to the summary
            for (int i = 0; i < columnGroup.Length; i++)
            {
                summaryGroup[rowGroup.Length + i] = columnGroup[i];
            }

            /*
             * For calculating the column summary first appending the 
             * column headers and the appending the row header, this cannot be calculated by 
             * the summaryGroup which is already defined since the summaries are out of order
             */

            //// adding the column values to the summary group
            for (int i = 0; i < columnGroup.Length; i++)
            {
                columnSummaryGroup[i] = columnGroup[i];
            }

            //// adding the row values to the summary group
            for (int i = 0; i < rowGroup.Length; i++)
            {
                columnSummaryGroup[columnGroup.Length + i] = rowGroup[i];
            }

            //// Grouping the source based on column group
            var temp_columnData = TableBuilderHelper.GetSortedData(listSource, sortOrder, columnGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (temp_columnData != null)
            {
                if (columnGroup.Length > 0)
                {
                    columnData = (temp_columnData as DataTable).GroupBy(columnGroup).ToList();
                }
            }

            //// Grouping the source based on row group
            var temp_rowData = TableBuilderHelper.GetSortedData(listSource, sortOrder, rowGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (temp_rowData != null)
            {
                if (rowGroup.Length > 0)
                {
                    rowData = (temp_rowData as DataTable).GroupBy(rowGroup).ToList();
                }
            }

            /// Grouping the source for value and row summary
            IListSource temp_listSource = TableBuilderHelper.GetSortedData(listSource, sortOrder, summaryGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (temp_listSource != null)
            {
                summaryData = (temp_listSource as DataTable).GroupBy(summaryGroup).ToList();
            }

            //// Grouping the source for column summary
            temp_listSource = TableBuilderHelper.GetSortedData(listSource, sortOrder, columnSummaryGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (temp_listSource != null)
            {
                columnSummaryData = (temp_listSource as DataTable).GroupBy(columnSummaryGroup).ToList();
            }

            List<Syncfusion.Olap.Engine.Extension.GroupResult> rowDataValues = null;
            List<Syncfusion.Olap.Engine.Extension.GroupResult> columnDataValues = null;

            if (drilledCell == null && !ExpandAll)
            {
                rowDataValues = GetParentData(rowData);
                columnDataValues = GetParentData(columnData);
            }
            else if (drilledCell == null && ExpandAll)
            {
                FillOlapReport(CurrentReport.SeriesElements, rowData, rowGroup);
                FillOlapReport(CurrentReport.CategoricalElements, columnData, columnGroup);

                foreach (var item in rowData)
                {
                    if (item.SubGroups != null)
                    {
                        item.ExpandableState = ExpandableState.Expanded;
                        item.SubNodes = new List<Extension.GroupResult>();
                        AddChildSubGroups(item, item.SubGroups, item.SubNodes);
                        item.SubGroups = item.SubNodes;
                    }
                }
                foreach (var item in columnData)
                {
                    if (item.SubGroups != null)
                    {
                        item.ExpandableState = ExpandableState.Expanded;
                        item.SubNodes = new List<Extension.GroupResult>();
                        AddChildSubGroups(item, item.SubGroups, item.SubNodes);
                        item.SubGroups = item.SubNodes;
                    }
                }

                rowDataValues = rowData;
                columnDataValues = columnData;
            }
            else if (drilledCell != null)
            {
                rowDataValues = SeriesGroupResult(rowData, CurrentReport, drilledCell);
                columnDataValues = CategoriesGroupResult(columnData, CurrentReport, drilledCell);

                if (rowDataValues == null)
                {
                    rowDataValues = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
                }
                else if (columnDataValues == null)
                {
                    columnDataValues = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
                }
            }
            else
            {
                rowDataValues = rowData;
                columnDataValues = columnData;
            }

            ///Engine Initialization and Processing
            if (isRowSummary)
            {
                if (layout == GridLayout.ExcelLikeLayout)
                {
                    if (rowData.Count > 0)
                        return GenerateExcelLikeLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                    else
                    {
                        if (summaryInfos.Length > 1)

                            return GenerateNormalLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                        else
                            return GenerateExcelLikeLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                    }
                }
                else if (layout == GridLayout.NoSummaries)
                {
                    if (summaryInfos.Length > 1)
                    {
                        return GenerateNoSummariesLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                    }
                    else
                        return GenerateNoSummariesLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                }
                else if (layout == GridLayout.Normal)
                {
                    return GenerateNormalLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                }
                else
                    return GenerateNormalTopSummary.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
            }
            else
            {
                if (layout == GridLayout.ExcelLikeLayout)
                {
                    if (rowData.Count > 0)
                    {
                        return GenerateExcelLikeLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                    }
                    else
                    {
                        return GenerateNormalLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                    }
                }
                else if (layout == GridLayout.NoSummaries)
                {
                    return GenerateNoSummariesLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                }
                else if (layout == GridLayout.Normal)
                {
                    return GenerateNormalLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
                }
                else
                    return GenerateNormalTopSummary.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, listSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged, summaryStringCount);
            }
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Builds the engine from IEnumerable source
        /// </summary>
        /// <param name="queryableSource">The IEnumerable source.</param>
        /// <param name="CurrentReport">The current report.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="columnGroup">The column group.</param>
        /// <param name="rowGroup">The row group.</param>
        /// <param name="summaryInfos">The summary infos.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <param name="layout">The layout.</param>
        /// <param name="ExpandAll">if set to <c>true</c> [expand all].</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <param name="drillDown">if set to <c>true</c> [drill down].</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromIQueryable(IQueryable queryableSource,Syncfusion.OlapSilverlight.Reports.OlapReport CurrentReport, SortType sortOrder, string[] columnGroup, string[] rowGroup, SummaryInfo[] summaryInfos, bool isRowSummary, GridLayout layout,bool ExpandAll, PivotCellDescriptor drilledCell, bool drillDown)

#else
        /// <summary>
        /// Builds the engine from IEnumerable source
        /// </summary>
        /// <param name="queryableSource">The IEnumerable source.</param>
        /// <param name="CurrentReport">The current report.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="columnGroup">The column group.</param>
        /// <param name="rowGroup">The row group.</param>
        /// <param name="summaryInfos">The summary infos.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <param name="layout">The layout.</param>
        /// <param name="ExpandAll">if set to <c>true</c> [expand all].</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <param name="drillDown">if set to <c>true</c> [drill down].</param>
        /// <returns></returns>
        public static PivotEngine BuildEngineFromIQueryable(IQueryable queryableSource, OlapReport CurrentReport, SortType sortOrder, string[] columnGroup, string[] rowGroup, SummaryInfo[] summaryInfos, bool isRowSummary, GridLayout layout, bool ExpandAll, PivotCellDescriptor drilledCell, bool drillDown)

#endif
        {

            //// Will the number of summaries, cause the grid layout to be changes.??
            bool isLayoutChanged = summaryInfos.Length > 1 ? true : false;

            //// Generate summary string
            string[] summaryGroup = new string[rowGroup.Length + columnGroup.Length];
            //// For calculating the column summaries
            string[] columnSummaryGroup = new string[rowGroup.Length + columnGroup.Length];

            //// Adding row values to the summary
            for (int i = 0; i < rowGroup.Length; i++)
            {
                summaryGroup[i] = rowGroup[i];
            }

            //// Adding column values to the summary
            for (int i = 0; i < columnGroup.Length; i++)
            {
                summaryGroup[rowGroup.Length + i] = columnGroup[i];
            }

            /*
             * For calculating the column summary first appending the 
             * column headers and the appending the row header, this cannot be calculated by 
             * the summaryGroup which is already defined since the summaries are out of order
             */

            //// adding the column values to the summary group
            for (int i = 0; i < columnGroup.Length; i++)
            {
                columnSummaryGroup[i] = columnGroup[i];
            }

            //// adding the row values to the summary group
            for (int i = 0; i < rowGroup.Length; i++)
            {
                columnSummaryGroup[columnGroup.Length + i] = rowGroup[i];
            }

            //// Grouping the source based on column group
            var temp_columnData = TableBuilderHelper.GetSortedData(queryableSource, sortOrder, columnGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (columnGroup.Length > 0)
            {
                columnData = temp_columnData.GroupBy(columnGroup).ToList();
            }

            //// Grouping the source based on row group
            var temp_rowData = TableBuilderHelper.GetSortedData(queryableSource, sortOrder, rowGroup);
            List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            if (rowGroup.Length > 0)
            {
                rowData = temp_rowData.GroupBy(rowGroup).ToList();
            }

            //// Grouping the source for value and row summary
            var summaryData = TableBuilderHelper.GetSortedData(queryableSource, sortOrder, summaryGroup).GroupBy(summaryGroup).ToList();
            //// Grouping the source for column summary
            var columnSummaryData = TableBuilderHelper.GetSortedData(queryableSource, sortOrder, columnSummaryGroup).GroupBy(columnSummaryGroup).ToList();

            ///Engine Initialization and Processing

            List<Syncfusion.Olap.Engine.Extension.GroupResult> rowDataValues = null;
            List<Syncfusion.Olap.Engine.Extension.GroupResult> columnDataValues = null;

            if (drilledCell == null && !ExpandAll)
            {
                rowDataValues = GetParentData(rowData);
                columnDataValues = GetParentData(columnData);
            }
            else if (drilledCell == null && ExpandAll)
            {
                FillOlapReport(CurrentReport.SeriesElements, rowData, rowGroup);
                FillOlapReport(CurrentReport.CategoricalElements, columnData, columnGroup);

                foreach (var item in rowData)
                {
                    if (item.SubGroups != null)
                    {
                        item.ExpandableState = ExpandableState.Expanded;
#if SILVERLIGHT
                        item.SubNodes = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
#else 
                        item.SubNodes = new List<Extension.GroupResult>();
#endif
                        AddChildSubGroups(item, item.SubGroups, item.SubNodes);
                        item.SubGroups = item.SubNodes;
                    }
                }
                foreach (var item in columnData)
                {
                    if (item.SubGroups != null)
                    {
                        item.ExpandableState = ExpandableState.Expanded;
#if SILVERLIGHT
                        item.SubNodes = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
#else 
                        item.SubNodes = new List<Extension.GroupResult>();
#endif
                        AddChildSubGroups(item, item.SubGroups, item.SubNodes);
                        item.SubGroups = item.SubNodes;
                    }
                }

                rowDataValues = rowData;
                columnDataValues = columnData;
            }
            else if (drilledCell != null)
            {
                rowDataValues = SeriesGroupResult(rowData, CurrentReport, drilledCell);
                columnDataValues = CategoriesGroupResult(columnData, CurrentReport, drilledCell);

                if (rowDataValues == null)
                {
                    rowDataValues = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
                }
                else if (columnDataValues == null)
                {
                    columnDataValues = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
                }
            }
            else
            {
                rowDataValues = rowData;
                columnDataValues = columnData;
            }



            if (isRowSummary)
            {

                if (layout == GridLayout.ExcelLikeLayout)
                {
                    //Excel Layout - Row Measure
                    if (rowData.Count == 0)
                    {
                        return GenerateNormalLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                    }
                    else
                    {
                        if (summaryInfos.Length > 1)
                            return GenerateExcelLikeLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                        else
                            return GenerateExcelLikeLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                    }
                }

                else if (layout == GridLayout.NoSummaries)
                {
                    //No Summaries Layout - Row measure
                    if (summaryInfos.Length > 1)
                    {
                        return GenerateNoSummariesLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                    }
                    else
                        return GenerateNoSummariesLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                }

                else if (layout == GridLayout.Normal)
                {
                    //Normal Layout - Row Measure
                    return GenerateNormalLayout.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                }

                else
                    //Normal Top summary Layout - Row Measure
                    return GenerateNormalTopSummary.ProcessRowMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);

            }

            else
            {
                if (layout == GridLayout.ExcelLikeLayout)
                {
                    //Excel Layout - Column Measure
                    if (rowData.Count == 0)
                    {
                        return GenerateNormalLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                    }
                    else
                        return GenerateExcelLikeLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                }

                else if (layout == GridLayout.NoSummaries)
                {
                    //No Summaries Layout - Column Measure
                    return GenerateNoSummariesLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);

                }

                else if (layout == GridLayout.Normal)
                {
                    //Normal Layout - Column Measure
                    return GenerateNormalLayout.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);
                }

                else
                    //Normal Top Summary Layout - Column Measure
                    return GenerateNormalTopSummary.ProcessColumnMeasure(rowDataValues, columnDataValues, summaryInfos, summaryData, queryableSource, rowGroup, columnGroup, columnSummaryData, isLayoutChanged);

            }


        }

        /// <summary>
        /// Fills the Olap Report when "Expand All" option is set.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="rowColumnData">The row column data.</param>
        /// <param name="rowColumnGroup">The row column group.</param>
#if SILVERLIGHT
        private static void FillOlapReport(Items items, List<Syncfusion.Olap.Engine.Extension.GroupResult> rowColumnData, string[] rowColumnGroup)
#else
        private static void FillOlapReport(Items items, List<Extension.GroupResult> rowColumnData, string[] rowColumnGroup)
#endif
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is DimensionElement)
                {
                    LevelElement _levelElement = (item.ElementValue as DimensionElement).Hierarchy.LevelElements[0];
                    foreach (var _rowColumnDataValue in rowColumnData)
                    {
                        MemberElement _memberElement = new MemberElement(_levelElement) { Name = _rowColumnDataValue.Key.ToString(), UniqueName = rowColumnGroup[0] + "." + _rowColumnDataValue.Key.ToString(), Level = 0 };
                        if (_rowColumnDataValue.SubGroups != null && _rowColumnDataValue.SubGroups.Count() > 0)
                            AddChildMemberElement(_memberElement, _rowColumnDataValue.SubGroups, rowColumnGroup, 1);
                        _levelElement.MemberElements.Add(_memberElement);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Adds the child sub-groups.
        /// </summary>
        /// <param name="mainGroup">The main group.</param>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="subNodes">The sub nodes.</param>
#if SILVERLIGHT
        private static void AddChildSubGroups(Syncfusion.Olap.Engine.Extension.GroupResult mainGroup, IEnumerable<Syncfusion.Olap.Engine.Extension.GroupResult> iEnumerable, List<Syncfusion.Olap.Engine.Extension.GroupResult> subNodes)
#else 
        private static void AddChildSubGroups(Extension.GroupResult mainGroup, IEnumerable<Extension.GroupResult> iEnumerable, List<Extension.GroupResult> subNodes)
#endif
        {
            foreach (var item in iEnumerable)
            {
                if (item.SubGroups != null)
                {
                    item.ExpandableState = ExpandableState.Expanded;
#if SILVERLIGHT
                    item.SubNodes = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
#else 
                    item.SubNodes = new List<Extension.GroupResult>();
#endif
                    AddChildSubGroups(item, item.SubGroups, item.SubNodes);
                    mainGroup.SubNodes.Add(item);
                    mainGroup.SubGroups = mainGroup.SubNodes;
                }
                else
                {
                    mainGroup.SubNodes.Add(item);
                    mainGroup.SubGroups = mainGroup.SubNodes;
                }
            }
        }

        /// <summary>
        /// Adds the child member element.
        /// </summary>
        /// <param name="_memberElement">The _member element.</param>
        /// <param name="rowData">The row data.</param>
        /// <param name="rowGroup">The row group.</param>
        /// <param name="index">The index.</param>
#if SILVERLIGHT
        private static void AddChildMemberElement(MemberElement _memberElement, IEnumerable<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, string[] rowGroup, int index)
#else 
        private static void AddChildMemberElement(MemberElement _memberElement, IEnumerable<Extension.GroupResult> rowData, string[] rowGroup, int index)
#endif
        {
            foreach (var _rowDataValue in rowData)
            {
                MemberElement childMemberElement = new MemberElement(_memberElement) { Name = _rowDataValue.Key.ToString(), UniqueName = rowGroup[index] + "." + _rowDataValue.Key.ToString(), Level = index };
                if (_rowDataValue.SubGroups != null && _rowDataValue.SubGroups.Count() > 0)
                    AddChildMemberElement(childMemberElement, _rowDataValue.SubGroups, rowGroup, index + 1);
                _memberElement.ChildMemberElements.Add(childMemberElement);
            }
        }

        /// <summary>
        /// Generates the GroupResult for Series
        /// </summary>
        /// <param name="parentData">The parent data.</param>
        /// <param name="CurrentReport">The current report.</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <returns></returns>
        private static List<Syncfusion.Olap.Engine.Extension.GroupResult> SeriesGroupResult(List<Syncfusion.Olap.Engine.Extension.GroupResult> parentData, OlapReport CurrentReport, PivotCellDescriptor drilledCell)
        {

            List<Syncfusion.Olap.Engine.Extension.GroupResult> grpResult = GetParentData(parentData);

            for (int item = 0; item < CurrentReport.SeriesElements.Count; item++)
            {
                DimensionElement dimensionElement = CurrentReport.SeriesElements[item].ElementValue as DimensionElement;
                if (dimensionElement != null)
                {
                    if (dimensionElement.Hierarchy != null)
                    {
                        if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            grpResult = FillGroupResult(grpResult, parentData, dimensionElement.Hierarchy.LevelElements[0], drilledCell);
                            return grpResult;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Generates the GroupResult for Categories
        /// </summary>
        /// <param name="parentData">The parent data.</param>
        /// <param name="CurrentReport">The current report.</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <returns></returns>
        private static List<Syncfusion.Olap.Engine.Extension.GroupResult> CategoriesGroupResult(List<Syncfusion.Olap.Engine.Extension.GroupResult> parentData, OlapReport CurrentReport, PivotCellDescriptor drilledCell)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> grpResult = GetParentData(parentData);
            for (int item = 0; item < CurrentReport.CategoricalElements.Count; item++)
            {
                DimensionElement dimensionElement = CurrentReport.CategoricalElements[item].ElementValue as DimensionElement;
                if (dimensionElement != null)
                {
                    if (dimensionElement.Hierarchy != null)
                    {
                        if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            grpResult = FillGroupResult(grpResult, parentData, dimensionElement.Hierarchy.LevelElements[0], drilledCell);
                            return grpResult;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Fills the group result.
        /// </summary>
        /// <param name="childData">The child data.</param>
        /// <param name="parentData">The parent data.</param>
        /// <param name="levelElement">The level element.</param>
        /// <param name="drilledCell">The drilled cell.</param>
        /// <returns></returns>
        private static List<Syncfusion.Olap.Engine.Extension.GroupResult> FillGroupResult(List<Syncfusion.Olap.Engine.Extension.GroupResult> childData, List<Syncfusion.Olap.Engine.Extension.GroupResult> parentData, LevelElement levelElement, PivotCellDescriptor drilledCell)
        {
            foreach (MemberElement element in levelElement.MemberElements)
            {
                List<MemberElement> parentMemberElements = new List<MemberElement>();
                childGroupResult = GetChildGroupResult(childData, element.Name);
                if (childGroupResult != null)
                {
                    bool memberCollection = MemberCollection(element, drilledCell.UniqueName, drilledCell, GetChildGroupResult(parentData, element.Name), childGroupResult, parentMemberElements);
                }                
                childGroupResult = ChildMemberElementsCount(childGroupResult);
                childData = WrapChildData(childData, element.Name, childGroupResult);
                parentMemberElements = null;
            }
            return childData;
        }

        /// <summary>
        /// Wraps the child data.
        /// </summary>
        /// <param name="childData">The child data.</param>
        /// <param name="Name">The name.</param>
        /// <param name="childGroupResult">The child group result.</param>
        /// <returns></returns>
        private static List<Syncfusion.Olap.Engine.Extension.GroupResult> WrapChildData(List<Syncfusion.Olap.Engine.Extension.GroupResult> childData, string Name, Syncfusion.Olap.Engine.Extension.GroupResult childGroupResult)
        {
            for (int i = 0; i < childData.Count; i++)
            {
                if (childData[i].Key.ToString() == Name)
                {
                    childData[i] = childGroupResult;
                    return childData;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the child group result.
        /// </summary>
        /// <param name="childData">The child data.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        private static Syncfusion.Olap.Engine.Extension.GroupResult GetChildGroupResult(List<Syncfusion.Olap.Engine.Extension.GroupResult> childData, string Name)
        {
            foreach (var item in childData)
            {
                if (item.Key.ToString() == Name)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Iterates the parent groupresult based on the member element and then generates a child group result
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <param name="cellDesc">The cell desc.</param>
        /// <param name="parentData">The parent data.</param>
        /// <param name="childData">The child data.</param>
        /// <returns></returns>
        private static bool MemberCollection(MemberElement memberElement, string uniqueName, PivotCellDescriptor cellDesc, Syncfusion.Olap.Engine.Extension.GroupResult parentData, Syncfusion.Olap.Engine.Extension.GroupResult childData, List<MemberElement> parentMemberElements)
        {
            bool isDrillUpDown = false;
            {
                if (memberElement.UniqueName == uniqueName)
                {
                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        memberElement.ChildMemberElements.Clear();
                    }
                }
                else
                {
                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        if (!IsMemberExists(parentMemberElements, memberElement))
                        {
                            parentMemberElements.Add(memberElement);
                            childGroupResult = GetExpandedChilds(parentData, childData, memberElement);
                        }

                        for (int _member = 0; _member < memberElement.ChildMemberElements.Count; _member++)
                        {
                            MemberElement _memberElement = memberElement.ChildMemberElements[_member];
                            if (!IsMemberExists(parentMemberElements, _memberElement))
                            {
                                parentMemberElements.Add(_memberElement);
                                childGroupResult = GetExpandedChilds(GetChildGroupResult(parentData, _memberElement), GetChildGroupResult(childData, _memberElement), _memberElement);
                            }

                            if (MemberCollection(_memberElement, uniqueName, cellDesc, parentData, childData, parentMemberElements))
                            {
                                isDrillUpDown = true;
                                break;
                            }
                        }
                    }

                    if (!IsMemberExists(parentMemberElements, memberElement))
                    {
                        parentMemberElements.Add(memberElement);
                        childGroupResult = GetExpandedChilds(parentData, childData, memberElement);
                        //parentData = parentData.SubGroups;                        
                    }
                }
            }
            return isDrillUpDown;
        }

        /// <summary>
        /// Checks the parent members.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static bool CheckParentMembers(MemberElement memberElement, List<string> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (memberElement != null)
                {
                    if (memberElement.UniqueName != list[i])
                    {
                        return false;
                    }
                    memberElement = memberElement.ParentMemberElement;
                }
            }

            return true;
        }

        /// <summary>
        /// Fills the child group.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="childGroup">The child group.</param>
        /// <param name="headerKey">The header key.</param>
        /// <param name="level">The level.</param>
        /// <param name="_memberElement">The _member element.</param>
        private static void FillChildGroup(IEnumerable<Syncfusion.Olap.Engine.Extension.GroupResult> iEnumerable, List<Syncfusion.Olap.Engine.Extension.GroupResult> childGroup, string headerKey, int level, MemberElement _memberElement)
        {
            Action<Syncfusion.Olap.Engine.Extension.GroupResult> countLoopin = null;
            countLoopin = (n) =>
            {
                //count++;
                if (n.Key.ToString() == headerKey && n.Level == level)
                {
                    if (n.SubGroups == null)
                    {
                        n.SubGroups = childGroup;
                        n.ExpandableState = ExpandableState.Expanded;
                    }
                    //n.ElementsCount = GetElementsCount(n.SubGroups)+1;
                }
                if (n.SubGroups != null)
                {
                    n.SubGroups.ForEach(n1 => countLoopin(n1));
                }
            };
            //// Iterating row data
            iEnumerable.ForEach(i => countLoopin(i));
        }

        /// <summary>
        /// Gets the child group result.
        /// </summary>
        /// <param name="parentData">The parent data.</param>
        /// <param name="_memberElement">The _member element.</param>
        /// <returns></returns>
        private static Syncfusion.Olap.Engine.Extension.GroupResult GetChildGroupResult(Syncfusion.Olap.Engine.Extension.GroupResult parentData, MemberElement _memberElement)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> list = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            List<Syncfusion.Olap.Engine.Extension.GroupResult> childGroupResult = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            childGroupResult.Add(parentData);

            List<string> parentElements = GetParentElements(_memberElement);

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> countLoopin = null;
            countLoopin = (n) =>
            {
                if (n.Key.ToString() == _memberElement.Name && n.Level == _memberElement.Level)// && n.Key.ToString() == parentElements[0])
                {
                    if (parentElements.Count > 0)
                    {
                        if (n.Key.ToString() == parentElements[0])
                        {
                            list.Add(n);
                        }
                    }
                }
                if (parentElements.Count > 0)
                {
                    if (n.Key.ToString() == parentElements[0])
                    {
                        parentElements.RemoveAt(0);
                    }
                }
                if (n.SubGroups != null)
                {
                    if (list.Count == 0)
                    {
                        n.SubGroups.ForEach(n1 => countLoopin(n1));
                    }
                }
            };
            //parentElements = clone;
            //// Iterating row data
            childGroupResult.ForEach(i => countLoopin(i));
            return list[0];
        }

        /// <summary>
        /// Gets the parent elements.
        /// </summary>
        /// <param name="_memberElement">The _member element.</param>
        /// <returns></returns>
        private static List<string> GetParentElements(MemberElement _memberElement)
        {
            List<string> parentElements = new List<string>();
        label:
            parentElements.Add(_memberElement.Name);
            if (_memberElement.ParentMemberElement != null)
            {
                _memberElement = _memberElement.ParentMemberElement;
                goto label;
            }
            parentElements.Reverse();
            return parentElements;
        }

        /// <summary>
        /// Returns the child member elements count.
        /// </summary>
        /// <param name="childGroupResult">The child group result.</param>
        /// <returns></returns>
        private static Syncfusion.Olap.Engine.Extension.GroupResult ChildMemberElementsCount(Syncfusion.Olap.Engine.Extension.GroupResult childGroupResult)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> childGroup = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            childGroup.Add(childGroupResult);

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> countLoopin = null;
            countLoopin = (n) =>
            {
                if (n.SubGroups != null)
                {
                    //if (n.SubGroups.Contains(groupResult))
                    {
                        //n.SubGroups.ElementsCount = GetElementsCount(groupResult);
                        n.ElementsCount = GetElementsCount(n);
                        //return;
                    }
                    n.SubGroups.ForEach(n1 => countLoopin(n1));
                }
            };
            //// Iterating row data
            childGroup.ForEach(i => countLoopin(i));

            return childGroup[0];
        }

        /// <summary>
        /// Gets the expanded childs.
        /// </summary>
        /// <param name="parentData">The parent data.</param>
        /// <param name="childData">The child data.</param>
        /// <param name="memberElement">The member element.</param>
        /// <returns></returns>
        private static Syncfusion.Olap.Engine.Extension.GroupResult GetExpandedChilds(Syncfusion.Olap.Engine.Extension.GroupResult parentData, Syncfusion.Olap.Engine.Extension.GroupResult childData, MemberElement memberElement)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> childGroup = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();

            //if (parentData != null)
            {
                if (parentData.SubGroups != null)
                {
                    foreach (var item in parentData.SubGroups)
                    {
                        if (item.SubGroups != null)
                        {
                            item.HasChildren = true;
                        }
                        item.SubGroups = null;
                        item.ElementsCount = 0;
                        item.ExpandableState = ExpandableState.Collapsed;
                        childGroup.Add(item);
                    }

                    if (childGroupResult.SubGroups == null)
                    {
                        childGroupResult.ExpandableState = ExpandableState.Expanded;
                        childGroupResult.SubGroups = childGroup;
                        childGroupResult.ElementsCount = childGroupResult.SubGroups.Count();
                    }
                    else
                    {
                        FillChildGroup(childGroupResult.SubGroups, childGroup, parentData.Key.ToString(), parentData.Level, memberElement);
                    }

                    //childData.ElementsCount = GetElementsCount(childData);                   
                    //childGroupResult.ElementsCount = GetElementsCount(childGroupResult);
                }
            }
            return childGroupResult;
        }

        /// <summary>
        /// Gets the elements count.
        /// </summary>
        /// <param name="childGroup">The child group.</param>
        /// <returns></returns>
        private static int GetElementsCount(Syncfusion.Olap.Engine.Extension.GroupResult childGroup)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> childGroupResult = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();
            childGroupResult.Add(childGroup);

            int count = 0;
            Action<Syncfusion.Olap.Engine.Extension.GroupResult> countLoopin = null;
            countLoopin = (n) =>
            {
                count++;
                if (n.SubGroups != null)
                {
                    n.SubGroups.ForEach(n1 => countLoopin(n1));
                }
            };
            //// Iterating row data
            childGroupResult.ForEach(i => countLoopin(i));
            return count - 1;
        }


        /// <summary>
        /// Determines whether [ member exists] [the specified parent cells].
        /// </summary>
        /// <param name="parentCells">The parent cells.</param>
        /// <param name="member">The member.</param>
        /// <returns>
        /// 	<c>true</c> if [ member exists] [the specified parent cells]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMemberExists(List<MemberElement> parentCells, MemberElement member)
        {
            foreach (MemberElement item in parentCells)
            {
                if (item == member)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the parent data.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <returns></returns>
        private static List<Syncfusion.Olap.Engine.Extension.GroupResult> GetParentData(List<Syncfusion.Olap.Engine.Extension.GroupResult> Data)
        {
            List<Syncfusion.Olap.Engine.Extension.GroupResult> grpResult = new List<Syncfusion.Olap.Engine.Extension.GroupResult>();

            foreach (var item in Data)
            {
                Syncfusion.Olap.Engine.Extension.GroupResult grp = new Syncfusion.Olap.Engine.Extension.GroupResult();
                // grpResult.Add(new Syncfusion.Olap.Engine.Extension.GroupResult
                // {
                grp.ElementsCount = 0;
                if (item.SubGroups != null)
                {
                    grp.HasChildren = true;
                    grp.ExpandableState = ExpandableState.Collapsed;
                }
                grp.GroupKey = item.GroupKey;
                grp.Items = item.Items;
                grp.Key = item.Key;
                grp.Level = item.Level;
                grp.SubGroups = null;
                grp.Summary = item.Summary;
                //});
                grpResult.Add(grp);
            }

            return grpResult;
        }

        #endregion
    }
}