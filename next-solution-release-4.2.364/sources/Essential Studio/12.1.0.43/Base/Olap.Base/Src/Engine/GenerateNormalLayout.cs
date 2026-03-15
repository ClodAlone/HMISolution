#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Linq;
using Syncfusion.Olap.Engine.Extension;

#if !SILVERLIGHT
using System.Data;
using Syncfusion.Olap.Engine.CalculationColumn;
using Syncfusion.Olap.Reports;
using System.Globalization;
namespace Syncfusion.Olap.Engine
#endif

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.OlapSilverlight.Reports;
using System.Globalization;

namespace Syncfusion.OlapSilverlight.Engine
#endif
{
    /// <summary>
    /// NormalLayout - Pivot Engine Generation for IListSource and IEnumerable Source
    /// </summary>
    public class GenerateNormalLayout
    {
        /*
         *  ==================================================================================================
         * 
         *  IEnumerable Source
         * 
         *  ===================================================================================================
         */

        #region IEnumerable Source

        internal static PivotEngine ProcessRowMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IQueryable queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged)
        {
            int count = 0,
                rowCount = 0,
                columnCount = 0,
                rowLevels = 0,
                columnLevels = 0,
                subGroup = 0;
                //temp_subGroup = 1;

            #region Row/Column count calculation

            //// Calculating row count
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
            rowData.ForEach(i => countLoopin(i));
            //// updating count to the row count variable
            rowCount = count;
            //// if not grouping defined in row axis then displaying the default summary
            if (rowCount == 0)
            {
                rowCount++;
            }

            //// Calculating column count
            //// re-setting the count variable
            count = 0;
            //// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;
            //// if no grouping are defined in the column axis then summary of column values 
            //// will be displayed as default value for column axis, hece to display the values 
            //// incremeanting the count by 1
            if (columnCount == 0)
            {
                columnCount++;
            }

            #endregion

            #region Levels Calculation

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;
            List<int> Index = new List<int>();

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            //// resetting the subGroup for row processing
            subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            if (Index.Count > 0)
                subGroup = Index.Max() + 1;
            rowLevels = subGroup;

            subGroup = 0;
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin(i));
                if (isLayoutChanged)
                {
                    columnLevels = Index.Max() + 1;                    
                }
                else
                    columnLevels = Index.Max() + 1;
            }
            else
            {
                columnLevels = 1;
            }

            if (!isLayoutChanged)
            {
                subGroup = columnLevels;
                if (rowLevels == 0)
                {
                    rowLevels++;
                }              
            }
            else
            {
                subGroup = columnLevels;
                rowLevels++;
            }


            #endregion

            #region Engine initilization

            /// No of rows = column levels + row count
            /// No of columns = row levels + column count
            PivotEngine engineRow = PivotEngine.CreateEngine(columnLevels + (rowCount * summaryInfos.Length), rowLevels + columnCount);

            //// calculating row header section
            engineRow.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, rowCount * summaryInfos.Length, rowLevels);

            //// calculating column header section
            engineRow.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, columnCount);

            //// preserving the datasource
            engineRow.ItemSource = queryableSource;

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing row headers
             * 
             * ===================================================================================================
             */

            #region Processing row headers

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            int cRow = columnLevels - 1;
            int cColumn = 0;
            List<string> parent = new List<string>();

            rowHeaderLoop = (g) =>
            {
                cRow++;
                PivotCellDescriptor cel = engineRow[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Level = g.Level + 1;
                cel.UniqueName = rowGroup[g.Level];
                cel.Tag = g.Key;

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cel, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cel.HasChildren = true;
                    cel.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cel.UniqueName + "." + cel.CellValue);
                }

                if (summaryInfos.Length == 1)
                {
                    if (g.ElementsCount == 0)
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount) * summaryInfos.Length, 1);
                    }
                    else
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount) + summaryInfos.Length - summaryInfos.Length, 1);
                }
                else
                {
                    if (g.ElementsCount == 0)
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount + 1) * summaryInfos.Length, 1);
                    }
                    else
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount + 1) * summaryInfos.Length - summaryInfos.Length, 1);
                    }
                }

                if (g.ElementsCount == 0)
                {
                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            PivotCellDescriptor measureCell = engineRow[cRow + summaryCount, cColumn + 1];
                            measureCell.CellValue = summaryInfos[summaryCount].Key;
                            measureCell.CellCaption = summaryInfos[summaryCount].Key;
                            measureCell.Value = summaryInfos[summaryCount].Key;
                            measureCell.CellType = PivotCellDescriptorType.RowHeader;
                            measureCell.Tag = summaryInfos[summaryCount];
                            measureCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, g.ElementsCount, 1);
                        }
                    }
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineRow[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Tag = g.Key;
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        cRow++;
                    }
                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineRow[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        if (g.SubGroups != null)
                        {
                            spanCell.HasChildren = true;
                        }
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;

                        g.SubGroups.ForEach(i => rowHeaderLoop(i));

                        //// Re-setting the position
                        cRow++; cColumn--;

                        if (!isLayoutChanged)
                        {
                            PivotCellDescriptor summaryCell = engineRow[cRow, cColumn];
                            summaryCell.CellValue = g.Key.ToString() + " Total";
                            summaryCell.Value = g.Key.ToString();
                            summaryCell.CellType = PivotCellDescriptorType.SummaryRow;
                            summaryCell.CellCaption = summaryInfos[0].Key;
                            summaryCell.Tag = summaryInfos[0];
                            summaryCell.ParentCellValues = new List<string>();
                            summaryCell.ParentCellValues.Add(g.Key.ToString());
                            summaryCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (rowLevels - 1) - g.Level + 1);

                            for (int row = summaryCell.Range.Height; row < summaryCell.Range.Width; row++)
                            {
                                PivotCellDescriptor SpanCell = engineRow[cRow, cColumn + row];
                                SpanCell.CellType = PivotCellDescriptorType.SummaryRow;
                                SpanCell.SpanCell = summaryCell;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < summaryInfos.Length; i++)
                            {
                                PivotCellDescriptor summaryCell = engineRow[cRow, cColumn];
                                summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                                summaryCell.Value = summaryInfos[i].Key;
                                summaryCell.CellType = PivotCellDescriptorType.SummaryRow;
                                summaryCell.CellCaption = summaryInfos[i].Key;
                                summaryCell.Tag = summaryInfos[i];
                                summaryCell.ParentCellValues = new List<string>();
                                summaryCell.ParentCellValues.Add(g.Key.ToString());

                                summaryCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (rowLevels - 1) - g.Level + 1);

                                for (int row = summaryCell.Range.Height; row < summaryCell.Range.Width; row++)
                                {
                                    PivotCellDescriptor SpanCell = engineRow[cRow, cColumn + row];
                                    SpanCell.CellType = PivotCellDescriptorType.SummaryRow;
                                    SpanCell.SpanCell = summaryCell;
                                }

                                cRow++;
                            }

                            cRow--;
                        }
                    }
                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }
            else
            {
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineRow[columnLevels + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.RowHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing column headers
             * 
             * ===================================================================================================
             */

            #region Processing column headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoop = null;
            cColumn = rowLevels - 1;
            cRow = 0;
            colHeaderLoop = (g) =>
            {
                cColumn++;
                PivotCellDescriptor colCell = engineRow[cRow, cColumn];
                colCell.CellValue = g.Key.ToString();
                colCell.CellCaption = g.Key.ToString();
                colCell.Value = g.Key.ToString();
                colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                colCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (g.ElementsCount));
                colCell.Level = g.Level + 1;
                colCell.Tag = g.Key;
                colCell.UniqueName = columnGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(colCell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    colCell.HasChildren = true;
                    colCell.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(colCell.UniqueName + "." + colCell.CellValue);
                }

                for (int i = cColumn; i < colCell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineRow[cRow, i + 1];
                    spanCell.SpanCell = colCell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.Tag = g.Key;
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                }

                if (g.ElementsCount > 0)
                {
                    cColumn--;
                    //// Moving the child group to next row
                    cRow++;
                    g.SubGroups.ForEach(i => colHeaderLoop(i));
                    //// Re-setting the position
                    cRow--; cColumn++;

                    if (g.SubGroups != null)
                    {
                        PivotCellDescriptor cellDesc = engineRow[cRow, cColumn];
                        cellDesc.CellValue = g.Key.ToString() + " Total";
                        cellDesc.CellCaption = g.Key.ToString();
                        cellDesc.Value = g.Key.ToString();
                        cellDesc.CellType = PivotCellDescriptorType.SummaryColumn;

                        if (!isLayoutChanged)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRow, cColumn, columnLevels - g.Level, 1);
                        }
                        else
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRow, cColumn, ((columnLevels) - g.Level), 1);
                        }

                        for (int row = cellDesc.Range.Width; row < cellDesc.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRow[cRow + row, cColumn];
                            SpanCell.CellType =PivotCellDescriptorType.SummaryColumn;
                            SpanCell.SpanCell = cellDesc;

                        }
                    }
                }
            };

            /// Ignoring the column when its not supplied
            if (columnData != null)
            {
                columnData.ForEach(i => colHeaderLoop(i));
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing value cells
             * 
             * ===================================================================================================
             */


            #region Processing value cells

#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif
            for (int row = rowLevels; row < engineRow.TableColumns.Count; row++)
            {
                for (int column = subGroup; column < engineRow.TableColumns[row].Cells.Count; column++)
                {
                    PivotCellDescriptor cellVal = engineRow[column, row];
                    cellVal.CellType = PivotCellDescriptorType.Value;
                    HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineRow, column, row, false);
                    string result = string.Empty;
                    Syncfusion.Olap.Engine.Extension.GroupResult grpResult = null;
                    grpResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                    if (grpResult == null)
                    {
                        headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineRow, column, row, true);
                        grpResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        if (TableBuilderHelper.CheckIsSummaryRow(engineRow, row, column))
                        {
                            cellVal.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());                           
                        }
                    }

                    if (grpResult != null)
                    {
                        if (TableBuilderHelper.CheckIsSummary(engineRow, column, row))
                        {
                            cellVal.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }
                    }

                    if (grpResult == null)
                    {
                        List<string> summaryString = new List<string>();

                        //// populating the column summary string
                        for (int i = 0; i < headerInfo.ColumnHeaderCaptions.Count; i++)
                        {
                            summaryString.Add(columnGroup[i]);
                        }

                        //// populating the row summary strings
                        for (int i = 0; i < headerInfo.RowHeaderCaptions.Count; i++)
                        {
                            summaryString.Add(rowGroup[i]);
                        }

                        //// grouping the values with header information
                        var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();

                        grpResult = TableBuilderHelper.GetValue(summary, headerInfo);
                    }

                    if (grpResult != null)
                    {
                        SummaryInfo sumInfo = null;
                        if (isLayoutChanged)
                        {
                            sumInfo = headerInfo.SummaryInfo;
                        }
                        else
                        {
                            if (summaryInfos.Length > 0)
                                sumInfo = summaryInfos[0];
                        }

                        if (sumInfo != null)
                        {
                            switch (sumInfo.Type)
                            {
                                case SummaryType.Average:
                                    {
                                        result = grpResult.Items.AsQueryable().Average(sumInfo.Column).ToString();
                                        break;
                                    }
                                case SummaryType.Count:
                                    {
                                        result = grpResult.Items.AsQueryable().Count().ToString();
                                        break;
                                    };
                                case SummaryType.Sum:
                                    {
                                        result = grpResult.Items.AsQueryable().Sum(sumInfo.Column).ToString();
                                        break;
                                    };
                                case SummaryType.Max:
                                    {
                                        result = grpResult.Items.AsQueryable().Max(sumInfo.Column).ToString();
                                        break;
                                    }
                                case SummaryType.Min:
                                    {
                                        result = grpResult.Items.AsQueryable().Min(sumInfo.Column).ToString();
                                        break;
                                    }
#if !SILVERLIGHT
                                case SummaryType.Expression:
                                    {
                                        object o = helper.ComputeSummary(sumInfo.Expression, grpResult.Items);
                                        if (null != o)
                                            result = o.ToString();
                                        break;
                                    };
#endif
                                case SummaryType.Last:
                                    {
                                        result = grpResult.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(grpResult.Items.AsQueryable().Count()-1).ToString();
                                        break;
                                    }
                                case SummaryType.String:
                                case SummaryType.First:
                                    {
                                        result = grpResult.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(0).ToString();
                                        break;
                                    };
                                                                      
                            }
                            if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                            {
                                double val = 0;
                                if (double.TryParse(result, out val))
                                    result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, val);
                            }
                            cellVal.CellValue = result;
                            cellVal.CellCaption = result;
                            cellVal.CellType = PivotCellDescriptorType.Value;
                        }
                    }
                    cellVal.CellData = engineRow.GetCellDataValueforIEnumerable(column, row);
                }
            }

            #endregion

            engineRow.ClearLevelHeadersArea();
            engineRow.RecalculateSpans();
            return engineRow;
        }

        internal static PivotEngine ProcessColumnMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IQueryable queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged)
        {
            int count = 0,
            rowCount = 0,
            columnCount = 0,
            rowLevels = 0,
            columnLevels = 0,
            subGroup = 0;
            //temp_subGroup = 1;

            #region Row/Column count calculation

            //// Calculating row count
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
            rowData.ForEach(i => countLoopin(i));
            //// updating count to the row count variable
            rowCount = count;
            if (rowCount == 0)
            {
                rowCount++;
            }

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

            if (columnCount == 0)
            {
                columnCount++;
            }

            #endregion

            #region Levels Calculation

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;
            List<int> Index = new List<int>();

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            //// resetting the subGroup for row processing
            subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            if (Index.Count > 0)
            {
                subGroup = Index.Max() + 1;
            }
            rowLevels = subGroup;

            subGroup = 0;
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin(i));
                if (isLayoutChanged)
                {
                    columnLevels = Index.Max() + 1;
                    columnLevels++;
                }
                else
                    columnLevels = Index.Max() + 1;
            }
            else
            {
                columnLevels = 1;
            }

            subGroup = columnLevels - 1;

            #endregion

            #region Engine initilization

            /// No of rows = column levels + row count
            /// No of columns = row levels + column count
            PivotEngine engine = PivotEngine.CreateEngine(columnLevels + rowCount, rowLevels + (columnCount * summaryInfos.Length));

            //// calculating row header section
            engine.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, rowCount, rowLevels);

            //// calculating column header section
            engine.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, columnCount * summaryInfos.Length);

            //// preserving the datasource
            engine.ItemSource = queryableSource;

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing row headers
             * 
             * ===================================================================================================
             */

            #region Processing row headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopin = null;
            List<string> parent = new List<string>();
            int currentRow = columnLevels - 1;
            int currentColumn = 0;
            rowHeaderLoopin = (g) =>
            {
                currentRow++;
                PivotCellDescriptor cell = engine[currentRow, currentColumn];
                cell.CellValue = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.RowHeader;
                cell.Range = GridRangeInfo.FromTlhw(currentRow, currentColumn, g.ElementsCount, 1);
                cell.Level = g.Level + 1;
                cell.Tag = g.Key;

                cell.UniqueName = rowGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cell.HasChildren = true;
                    cell.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cell.UniqueName + "." + cell.CellValue);
                }

                #region Spanned cell

                //// Inserting row spanned cells
                for (int i = currentRow; i < cell.Range.Bottom; i++)
                {
                    PivotCellDescriptor spanCell = engine[i + 1, currentColumn];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.RowHeader;
                    spanCell.Tag = g.Key;
                }

                #endregion

                if (g.ElementsCount > 0)
                {
                    //// Summary parent cells
                    PivotCellDescriptor summaryCell = engine[currentRow + g.ElementsCount, currentColumn];
                    summaryCell.CellValue = g.Key.ToString();
                    summaryCell.Value = g.Key.ToString();
                    summaryCell.CellCaption = g.Key.ToString();
                    summaryCell.CellType = PivotCellDescriptorType.RowHeader;
                  
                    int tempcount = 0;
                    for (int i = currentColumn; i < rowLevels - 1; i++)
                    {
                        // Summary children cells
                        PivotCellDescriptor summarySpanCell = engine[currentRow + g.ElementsCount, i];
                        summarySpanCell.CellValue = g.Key.ToString() + " Total";
                        summarySpanCell.CellCaption = g.Key.ToString();
                        summarySpanCell.CellType = PivotCellDescriptorType.SummaryRow;

                        if (i < summaryInfos.Length)
                        {
                            summarySpanCell.Tag = summaryInfos[i];
                        }

                        summarySpanCell.CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());

                        if (tempcount == 0)
                        {
                            summarySpanCell.Range = GridRangeInfo.FromTlhw(currentRow + g.ElementsCount, currentColumn, 1, (rowLevels - g.Level));
                        }

                        else
                        {
                            summarySpanCell.SpanCell = summaryCell;
                            summarySpanCell.CellType = summaryCell.CellType;
                        }

                        for (int counter = 0; counter < summarySpanCell.Range.Width-1; counter++)
                        {
                            PivotCellDescriptor childCell = engine[currentRow + g.ElementsCount, i + counter + 1];
                            childCell.SpanCell = summarySpanCell;
                        }

                        tempcount++;
                    }
                }
                if (g.SubGroups != null)
                {
                    //// Child group element insert starts from same row
                    currentRow--;
                    //// Moving the child group to next column
                    currentColumn++;
                    g.SubGroups.ForEach(i => rowHeaderLoopin(i));
                    //// Re-setting the position
                    currentColumn--; currentRow++;

                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopin(i));
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing column headers
             * 
             * ==================================================================================================
             */

            #region Processing column headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopin = null;
            currentColumn = rowLevels - 1;
            currentRow = 0;
            columnHeaderLoopin = (g) =>
            {
                currentColumn++;
                PivotCellDescriptor cell = engine[currentRow, currentColumn];
                cell.CellValue = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.ColumnHeader;
                cell.Range = GridRangeInfo.FromTlhw(currentRow, currentColumn, 1, (g.ElementsCount + 1) * summaryInfos.Length);
                cell.Level = g.Level + 1;
                cell.Tag = g.Key;

                cell.UniqueName = columnGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cell.HasChildren = true;
                    cell.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cell.UniqueName + "." + cell.CellValue);
                }

                #region Inserting spanned cells

                //// Inserting column spanned cells
                for (int i = currentColumn; i < cell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engine[currentRow, i + 1];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    spanCell.Tag = g.Key;                  
                }
                #endregion

                if (!isLayoutChanged)
                {
                    if (g.ElementsCount > 0)
                    {
                        PivotCellDescriptor spanCell = engine[currentRow + 1, currentColumn + g.ElementsCount];
                        spanCell.CellValue = g.Key.ToString() + " Total";
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        if (summaryInfos.Length > 0)
                        {
                            spanCell.Tag = summaryInfos[0];
                        }
                        spanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        spanCell.Range = GridRangeInfo.FromTlhw(currentRow + 1, currentColumn + g.ElementsCount, ((columnLevels - 1) - g.Level), 1);

                        for (int row = spanCell.Range.Width; row < spanCell.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engine[currentRow + row + 1, currentColumn + g.ElementsCount];
                            SpanCell.SpanCell = spanCell;
                            SpanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        }

                    }
                }
                if (g.SubGroups != null)
                {
                    //// Child group insert starts form the sample column
                    currentColumn--;
                    //// Moving the child group to next row
                    currentRow++;
                    g.SubGroups.ForEach(i => columnHeaderLoopin(i));
                    //// Re-setting the position
                    currentRow--; currentColumn++;

                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engine[currentRow + 1, currentColumn++];
                            summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                            summaryCell.Range = GridRangeInfo.FromTlhw(currentRow + 1, currentColumn - 1, (columnLevels - 1) - g.Level, 1);

                            for (int row = summaryCell.Range.Width; row < summaryCell.Range.Height; row++)
                            {
                                PivotCellDescriptor SpanCell = engine[currentRow + row + 1, currentColumn - 1];
                                SpanCell.SpanCell = summaryCell;
                                SpanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            }
                        }
                        currentColumn--;
                    }

                }
                else
                {
                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engine[currentRow + 1, currentColumn++];
                            summaryCell.CellValue = summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                        }
                        currentColumn--;
                    }
                }
            };

            if (columnData.Count > 0)
            {
                columnData.ForEach(i => columnHeaderLoopin(i));
            }
            else
            {
                // Include the summaries in the column headers
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engine[0, rowLevels + i];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing value cells
             * 
             * ==================================================================================================
             */


            #region Processing value cells

#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif
            int baseCol = -1;
            
            for (int row = columnLevels; row < engine.RowsCount; row++)
            {
                for (int column = rowLevels; column < engine.TableColumns.Count; column++)
                {
                    PivotCellDescriptor cell = engine[row, column];
                    if (cell.UniqueName == string.Empty)
                    {
                        cell.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptions(engine, row, column, false);
                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                        groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (groupResult == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptions(engine, row, column, true);
                            groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                            //cellVal.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }

                        if (TableBuilderHelper.CheckIsSummaryRow(engine, column, row))
                        {
                            cell.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }

                        if (groupResult == null)
                        {
                            List<string> summaryString = new List<string>();

                            //// populating the column summary string
                            for (int i = 0; i < headerInfo.ColumnHeaderCaptions.Count; i++)
                            {
                                summaryString.Add(columnGroup[i]);
                            }

                            //// populating the row summary strings
                            for (int i = 0; i < headerInfo.RowHeaderCaptions.Count; i++)
                            {
                                summaryString.Add(rowGroup[i]);
                            }

                            if (summaryString.Count > 0)
                            {
                                //// grouping the values with header information
                                var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();

                                groupResult = TableBuilderHelper.GetValue(summary, headerInfo);
                            }                           
                        }
                        if (groupResult != null)
                        {
                            SummaryInfo summaryInfo = null;
                            if (isLayoutChanged)
                            {
                                summaryInfo = headerInfo.SummaryInfo;
                            }
                            else
                            {
                                if (summaryInfos.Length > 0)
                                    summaryInfo = summaryInfos[0];
                            }

                            if (summaryInfo != null)
                            {
                                switch (summaryInfo.Type)
                                {
                                    case SummaryType.Average:
                                        {
                                            result = groupResult.Items.AsQueryable().Average(summaryInfo.Column).ToString();
                                            break;
                                        }
                                    case SummaryType.Count:
                                        {
                                            result = groupResult.Items.AsQueryable().Count().ToString();
                                            break;
                                        };
                                    case SummaryType.Sum:
                                        {
                                            result = groupResult.Items.AsQueryable().Sum(summaryInfo.Column).ToString();
                                            break;
                                        };
                                    case SummaryType.Max:
                                        {
                                            result = groupResult.Items.AsQueryable().Max(summaryInfo.Column).ToString();
                                            break;
                                        }
                                    case SummaryType.Min:
                                        {
                                            result = groupResult.Items.AsQueryable().Min(summaryInfo.Column).ToString();
                                            break;
                                        }
#if !SILVERLIGHT
                                    case SummaryType.Expression:
                                        {
                                            object val = helper.ComputeSummary(summaryInfo.Expression, groupResult.Items);//.AsQueryable());
                                            if (val != null)
                                                result = val.ToString();
                                            break;
                                        };
#endif
                                    case SummaryType.First:
                                        {
                                            result = groupResult.Items.AsQueryable().Select(summaryInfo.Column).ElementAtOrDefault(0).ToString();
                                            break;
                                        };
                                    case SummaryType.Last:
                                        {
                                            result = groupResult.Items.AsQueryable().Select(summaryInfo.Column).ElementAtOrDefault(groupResult.Items.AsQueryable().Count() - 1).ToString();
                                            break;
                                        };
                                    case SummaryType.String:
                                        {
                                            result = groupResult.Items.AsQueryable().Select(summaryInfo.Column).ElementAtOrDefault(0).ToString();
                                            if (columnData.Count == 0)
                                            {
                                                if (baseCol != row)
                                                {
                                                    if (groupResult.SubGroups == null)
                                                    {
                                                        if (groupResult.Items.AsQueryable().Count() > 1)
                                                        {
                                                            engine.InsertAdditionalRows(groupResult.Items, summaryInfo.Column, column, row, summaryInfos, GridLayout.Normal, false);
                                                            baseCol = row;
                                                        }
                                                    }
                                                }
                                            }
                                            if (groupResult.Items.AsQueryable().Count() > 1)
                                            {
                                                result = null;
                                            }

                                            break;
                                        };
                                }

                            }
                            if (summaryInfo.FormatString != null && summaryInfo.FormatString != string.Empty)
                            {
                                double val = 0;
                                if (double.TryParse(result, out val))
                                    result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, val);
                            }
                        }
                        cell.CellType = PivotCellDescriptorType.Value;

                        cell.CellValue = result;
                        cell.CellCaption = result;
                    }
                    cell.CellData = engine.GetCellDataValueforIEnumerable(row, column);
                }

            }

            #endregion

            engine.ClearLevelHeadersArea();
            engine.RecalculateSpans();
            return engine;
        }

        #endregion

        /*
         *  ==================================================================================================
         * 
         *  IListSource 
         * 
         *  ===================================================================================================
         */


        #region IListSource
#if !SILVERLIGHT
        internal static PivotEngine ProcessRowMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IListSource queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged, int summaryStringCount)
        {
            int count = 0,
                rowCount = 0,
                columnCount = 0,
                rowLevels = 0,
                columnLevels = 0,
                subGroup = 0;
                //temp_subGroup = 1;

            #region Row/Column count calculation

            //// Calculating row count
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
            rowData.ForEach(i => countLoopin(i));
            //// updating count to the row count variable
            rowCount = count;
            //// if not grouping defined in row axis then displaying the default summary
            if (rowCount == 0)
            {
                rowCount++;
            }

            //// Calculating column count
            //// re-setting the count variable
            count = 0;
            //// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;
            //// if no grouping are defined in the column axis then summary of column values 
            //// will be displayed as default value for column axis, hece to display the values 
            //// incremeanting the count by 1
            if (columnCount == 0)
            {
                columnCount++;
            }

            #endregion

            #region Levels Calculation

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;
            List<int> Index = new List<int>();

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            //// resetting the subGroup for row processing
            subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            if (Index.Count > 0)
                subGroup = Index.Max() + 1;
            rowLevels = subGroup;

            subGroup = 0;
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin(i));
                if (isLayoutChanged)
                {
                    columnLevels = Index.Max() + 1;
                    //columnLevels++;
                }
                else
                    columnLevels = Index.Max() + 1;
            }
            else
            {
                columnLevels = 1;
            }

            if (!isLayoutChanged)
            {
                subGroup = columnLevels;
                if (rowLevels == 0)
                {
                    rowLevels++;
                }
                //if(columnData.Count !=0)
                //rowLevels++;
            }
            else
            {
                subGroup = columnLevels;
                rowLevels++;
            }


            #endregion

            #region Engine initilization

            /// No of rows = column levels + row count
            /// No of columns = row levels + column count
            PivotEngine engineRow = PivotEngine.CreateEngine(columnLevels + (rowCount * summaryInfos.Length), rowLevels + columnCount);

            //// calculating row header section
            engineRow.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, rowCount * summaryInfos.Length, rowLevels);

            //// calculating column header section
            engineRow.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, columnCount);

            //// preserving the datasource
            engineRow.ItemSource = queryableSource;

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing row headers
             * 
             * ===================================================================================================
             */

            #region Processing row headers

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            int cRow = columnLevels - 1;
            int cColumn = 0;
            List<string> parent = new List<string>();

            rowHeaderLoop = (g) =>
            {
                cRow++;
                PivotCellDescriptor cel = engineRow[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Level = g.Level + 1;
                cel.UniqueName = rowGroup[g.Level];
                cel.Tag = g.Key;

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cel, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cel.HasChildren = true;
                    cel.ExpandableState = g.ExpandableState;// TableBuilderHelper.GetExpandableState(g, rowGroup);
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cel.UniqueName + "." + cel.CellValue);
                }

                if (summaryInfos.Length == 1)
                {
                    if (g.ElementsCount == 0)
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount) * summaryInfos.Length, 1);
                    }
                    else
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount) + summaryInfos.Length - summaryInfos.Length, 1);
                }
                else
                {
                    if (g.ElementsCount == 0)
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount + 1) * summaryInfos.Length, 1);
                    }
                    else
                    {
                        cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (g.ElementsCount + 1) * summaryInfos.Length - summaryInfos.Length, 1);
                    }
                }

                if (g.ElementsCount == 0)
                {
                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            //cRow++;
                            PivotCellDescriptor measureCell = engineRow[cRow + summaryCount, cColumn + 1];
                            measureCell.CellValue = summaryInfos[summaryCount].Key;
                            measureCell.CellCaption = summaryInfos[summaryCount].Key;
                            measureCell.Value = summaryInfos[summaryCount].Key;
                            measureCell.CellType = PivotCellDescriptorType.RowHeader;
                            measureCell.Tag = summaryInfos[summaryCount];
                            measureCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, g.ElementsCount, 1);
                        }
                    }
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineRow[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Tag = g.Key;
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        cRow++;
                    }
                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineRow[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        if (g.SubGroups != null)
                        {
                            spanCell.HasChildren = true;
                        }
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;

                        g.SubGroups.ForEach(i => rowHeaderLoop(i));

                        //// Re-setting the position
                        cRow++; cColumn--;

                        if (!isLayoutChanged)
                        {
                            PivotCellDescriptor summaryCell = engineRow[cRow, cColumn];
                            summaryCell.CellValue = g.Key.ToString() + " Total";// summaryInfos[0].Key;
                            summaryCell.Value = g.Key.ToString();// summaryInfos[0].Key;
                            summaryCell.CellType = PivotCellDescriptorType.SummaryRow;
                            summaryCell.CellCaption = summaryInfos[0].Key;
                            summaryCell.Tag = summaryInfos[0];
                            summaryCell.ParentCellValues = new List<string>();
                            summaryCell.ParentCellValues.Add(g.Key.ToString());
                            summaryCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (rowLevels - 1) - g.Level + 1);

                            for (int row = summaryCell.Range.Height; row < summaryCell.Range.Width; row++)
                            {
                                PivotCellDescriptor SpanCell = engineRow[cRow, cColumn + row];
                                SpanCell.SpanCell = summaryCell;
                                SpanCell.CellType = PivotCellDescriptorType.SummaryRow;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < summaryInfos.Length; i++)
                            {
                                PivotCellDescriptor summaryCell = engineRow[cRow, cColumn];
                                summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                                summaryCell.Value = summaryInfos[i].Key;
                                summaryCell.CellType = PivotCellDescriptorType.SummaryRow;
                                summaryCell.CellCaption = summaryInfos[i].Key;
                                summaryCell.Tag = summaryInfos[i];
                                summaryCell.ParentCellValues = new List<string>();
                                summaryCell.ParentCellValues.Add(g.Key.ToString());

                                summaryCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (rowLevels - 1) - g.Level + 1);

                                for (int row = summaryCell.Range.Height; row < summaryCell.Range.Width; row++)
                                {
                                    PivotCellDescriptor SpanCell = engineRow[cRow, cColumn + row];
                                    SpanCell.SpanCell = summaryCell;
                                    SpanCell.CellType = PivotCellDescriptorType.SummaryRow;
                                }

                                cRow++;
                            }

                            cRow--;
                        }
                    }
                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }
            else
            {
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineRow[columnLevels + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.RowHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing column headers
             * 
             * ===================================================================================================
             */

            #region Processing column headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoop = null;
            cColumn = rowLevels - 1;
            cRow = 0;
            colHeaderLoop = (g) =>
            {
                cColumn++;
                PivotCellDescriptor colCell = engineRow[cRow, cColumn];
                colCell.CellValue = g.Key.ToString();
                colCell.CellCaption = g.Key.ToString();
                colCell.Value = g.Key.ToString();
                colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                colCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, (g.ElementsCount));
                colCell.Level = g.Level + 1;
                colCell.Tag = g.Key;
                colCell.UniqueName = columnGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(colCell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    colCell.HasChildren = true;
                    colCell.ExpandableState = g.ExpandableState;// TableBuilderHelper.GetExpandableState(g, rowGroup);
                }

                if (g.SubGroups != null)
                {
                    parent.Add(colCell.UniqueName + "." + colCell.CellValue);
                }

                for (int i = cColumn; i < colCell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineRow[cRow, i + 1];
                    spanCell.SpanCell = colCell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.Tag = g.Key;
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                }

                if (g.ElementsCount > 0)
                {
                    cColumn--;
                    //// Moving the child group to next row
                    cRow++;
                    g.SubGroups.ForEach(i => colHeaderLoop(i));
                    //// Re-setting the position
                    cRow--; cColumn++;

                    if (g.SubGroups != null)
                    {
                        PivotCellDescriptor cellDesc = engineRow[cRow, cColumn];
                        cellDesc.CellValue = g.Key.ToString() + " Total";
                        cellDesc.CellCaption = g.Key.ToString();
                        cellDesc.Value = g.Key.ToString();
                        cellDesc.CellType = PivotCellDescriptorType.SummaryColumn;

                        if (!isLayoutChanged)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRow, cColumn, columnLevels - g.Level, 1);
                        }
                        else
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRow, cColumn, ((columnLevels) - g.Level), 1);
                        }

                        for (int row = cellDesc.Range.Width; row < cellDesc.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRow[cRow + row, cColumn];
                            SpanCell.SpanCell = cellDesc;
                            SpanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        }
                    }
                }
            };

            /// Ignoring the column when its not supplied
            if (columnData != null)
            {
                columnData.ForEach(i => colHeaderLoop(i));
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing value cells
             * 
             * ===================================================================================================
             */

            #region Processing value cells
            int baseCol = subGroup;
            ExpressionHelper helper = new ExpressionHelper();
            for (int row = rowLevels; row < engineRow.TableColumns.Count; row++)
            {
                for (int column = subGroup; column < engineRow.TableColumns[row].Cells.Count; column++)
                {
                    PivotCellDescriptor cellVal = engineRow[column, row];
                    cellVal.CellType = PivotCellDescriptorType.Value;
                    HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineRow, column, row, false);
                    string result = string.Empty;
                    Syncfusion.Olap.Engine.Extension.GroupResult grpResult = null;
                    grpResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                    if (grpResult == null)
                    {
                        headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineRow, column, row, true);
                        grpResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                    }

                    if (TableBuilderHelper.CheckIsSummaryRow(engineRow, row, column))
                    {
                        cellVal.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                    }

                    if (grpResult == null)
                    {
                        List<string> summaryString = new List<string>();

                        //// populating the column summary string
                        for (int i = 0; i < headerInfo.ColumnHeaderCaptions.Count; i++)
                        {
                            summaryString.Add(columnGroup[i]);
                        }

                        //// populating the row summary strings
                        for (int i = 0; i < headerInfo.RowHeaderCaptions.Count; i++)
                        {
                            summaryString.Add(rowGroup[i]);
                        }

                        //// grouping the values with header information

                        if (summaryString.Count > 0)
                        {
                            var summary = (queryableSource as DataTable).GroupBy(summaryString.ToArray()).ToList();
                            grpResult = TableBuilderHelper.GetValue(summary, headerInfo);
                        }

                    }

                    if (grpResult != null)
                    {
                        SummaryInfo sumInfo = null;
                        if (isLayoutChanged)
                        {
                            sumInfo = headerInfo.SummaryInfo;
                        }
                        else
                        {
                            if (summaryInfos.Length > 0)
                                sumInfo = summaryInfos[0];
                        }

                        if (sumInfo != null)
                        {
                            switch (sumInfo.Type)
                            {
                                case SummaryType.Average:
                                    {
                                        result = DataRowHelper.ComputeAvg(grpResult.Items, sumInfo.Column);
                                        break;
                                    }
                                case SummaryType.Count:
                                    {
                                        result = DataRowHelper.ComputeCount(grpResult.Items, sumInfo.Column);
                                        break;
                                    };
                                case SummaryType.Sum:
                                    {
                                        result = DataRowHelper.ComputeSum(grpResult.Items, sumInfo.Column);
                                        // result = datarowhe

                                        break;
                                    };
                                case SummaryType.Max:
                                    {
                                        result = DataRowHelper.ComputeMax(grpResult.Items, sumInfo.Column);
                                        break;
                                    };
                                case SummaryType.Min:
                                    {
                                        result = DataRowHelper.ComputeMin(grpResult.Items, sumInfo.Column);
                                        break;
                                    };
                                case SummaryType.Expression:
                                    {
                                        result = "Expression Not spported";
                                        //object o = helper.ComputeSummary(sumInfo.Expression, grpResult.Items);
                                        //if (null != o)
                                        //    result = o.ToString();
                                        break;
                                    };
                                case SummaryType.First:
                                    {
                                        result = DataRowHelper.ComputeFirst(grpResult.Items,sumInfo.Column);
                                        break;
                                    };
                               
                                case SummaryType.Last:
                                    {
                                        result = DataRowHelper.ComputeLast(grpResult.Items, sumInfo.Column);
                                        break;
                                    }
                                case SummaryType.String:
                                    {
                                        result = DataRowHelper.Select(grpResult.Items, sumInfo.Column);

                                        break;
                                    };
                                   
                            }
                            if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                            {
                                string[] formatString = new string[] { "d", "t", "m", "y", "hh", "ss" };
                                double val = 0, dtVal=0;
                                DateTime dateTime;
                                if (double.TryParse(result, out val))
                                    result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, val);
                                if (formatString.Any(m => sumInfo.FormatString.ToLower().Contains(m)))
                                {
                                    DateTime dTime = new DateTime();
                                    if (DateTime.TryParse(result, out dTime))
                                    {
                                        dateTime = Convert.ToDateTime(result);
                                        dtVal = dateTime.ToOADate();
                                        if (dtVal != 0)
                                            result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, DateTime.FromOADate(dtVal));
                                    }
                                    else if (val != 0)
                                        result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, DateTime.FromOADate(val));
                                }
                            }
                            cellVal.CellValue = result;
                            cellVal.CellCaption = result;
                            cellVal.CellType = PivotCellDescriptorType.Value;

                        }
                    }
                    cellVal.CellData = engineRow.GetCellDataValueforIEnumerable(column, row);

                }
            }

            #endregion

            engineRow.ClearLevelHeadersArea();
            engineRow.RecalculateSpans();

            return engineRow;
        }

        internal static PivotEngine ProcessColumnMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IListSource queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged, int summaryStringCount)
        {
            int count = 0,
             rowCount = 0,
             columnCount = 0,
             rowLevels = 0,
             columnLevels = 0,
             subGroup = 0;
             //temp_subGroup = 1;

            #region Row/Column count calculation

            //// Calculating row count
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
            rowData.ForEach(i => countLoopin(i));
            //// updating count to the row count variable
            rowCount = count;
            if (rowCount == 0)
            {
                rowCount++;
            }

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

            if (columnCount == 0)
            {
                columnCount++;
            }

            #endregion

            #region Levels Calculation

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;
            List<int> Index = new List<int>();

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            //// resetting the subGroup for row processing
            subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            if (Index.Count > 0)
            {
                subGroup = Index.Max() + 1;
            }
            rowLevels = subGroup;

            subGroup = 0;
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin(i));
                if (isLayoutChanged)
                {
                    columnLevels = Index.Max() + 1;
                    columnLevels++;
                }
                else
                    columnLevels = Index.Max() + 1;
            }
            else
            {
                columnLevels = 1;
            }

            subGroup = columnLevels - 1;

            #endregion

            #region Engine initilization

            /// No of rows = column levels + row count
            /// No of columns = row levels + column count
            PivotEngine engine = PivotEngine.CreateEngine(columnLevels + rowCount, rowLevels + (columnCount * summaryInfos.Length));

            //// calculating row header section
            engine.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, rowCount, rowLevels);

            //// calculating column header section
            engine.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, columnCount * summaryInfos.Length);

            //// preserving the datasource
            engine.ItemSource = queryableSource;

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing row headers
             * 
             * ===================================================================================================
             */

            #region Processing row headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopin = null;
            List<string> parent = new List<string>();
            int currentRow = columnLevels - 1;
            int currentColumn = 0;
            rowHeaderLoopin = (g) =>
            {
                currentRow++;
                PivotCellDescriptor cell = engine[currentRow, currentColumn];
                cell.CellValue = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.RowHeader;
                cell.Range = GridRangeInfo.FromTlhw(currentRow, currentColumn, g.ElementsCount, 1);
                cell.Level = g.Level + 1;
                cell.Tag = g.Key;

                cell.UniqueName = rowGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cell.HasChildren = true;
                    cell.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cell.UniqueName + "." + cell.CellValue);
                }

                #region Spanned cell

                //// Inserting row spanned cells
                for (int i = currentRow; i < cell.Range.Bottom; i++)
                {
                    PivotCellDescriptor spanCell = engine[i + 1, currentColumn];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.RowHeader;
                    spanCell.Tag = g.Key;
                }

                #endregion

                if (g.ElementsCount > 0)
                {
                    //// Summary parent cells
                    PivotCellDescriptor summaryCell = engine[currentRow + g.ElementsCount, currentColumn];
                    summaryCell.CellValue = g.Key.ToString();
                    summaryCell.Value = g.Key.ToString();
                    summaryCell.CellCaption = g.Key.ToString();
                    summaryCell.CellType = PivotCellDescriptorType.RowHeader;
                 
                    int tempcount = 0;
                    for (int i = currentColumn; i < rowLevels - 1; i++)
                    {
                        // Summary children cells
                        PivotCellDescriptor summarySpanCell = engine[currentRow + g.ElementsCount, i];
                        summarySpanCell.CellValue = g.Key.ToString() + " Total";
                        summarySpanCell.CellCaption = g.Key.ToString();
                        //summarySpanCell.Value = g.Key.ToString();
                        summarySpanCell.CellType = PivotCellDescriptorType.SummaryRow;

                        if (i < summaryInfos.Length)
                        {
                            summarySpanCell.Tag = summaryInfos[i];
                        }

                        summarySpanCell.CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());

                        if (tempcount == 0)
                        {
                            summarySpanCell.Range = GridRangeInfo.FromTlhw(currentRow + g.ElementsCount, currentColumn, 1, (rowLevels - g.Level));
                        }

                        else
                        {
                            summarySpanCell.SpanCell = summaryCell;
                            summarySpanCell.CellType = summaryCell.CellType;
                        }

                        for (int counter = 0; counter < summarySpanCell.Range.Width-1; counter++)
                        {
                            PivotCellDescriptor childCell = engine[currentRow + g.ElementsCount, i + counter + 1];
                            childCell.SpanCell = summarySpanCell;
                        }

                        tempcount++;
                    }
                }


                if (g.SubGroups != null)
                {
                    //// Child group element insert starts from same row
                    currentRow--;
                    //// Moving the child group to next column
                    currentColumn++;
                    g.SubGroups.ForEach(i => rowHeaderLoopin(i));
                    //// Re-setting the position
                    currentColumn--; currentRow++;

                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopin(i));
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing column headers
             * 
             * ==================================================================================================
             */

            #region Processing column headres

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopin = null;
            currentColumn = rowLevels - 1;
            currentRow = 0;
            columnHeaderLoopin = (g) =>
            {
                currentColumn++;
                PivotCellDescriptor cell = engine[currentRow, currentColumn];
                cell.CellValue = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.ColumnHeader;
                cell.Range = GridRangeInfo.FromTlhw(currentRow, currentColumn, 1, (g.ElementsCount + 1) * summaryInfos.Length);
                cell.Level = g.Level + 1;
                cell.Tag = g.Key;

                cell.UniqueName = columnGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(cell, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    cell.HasChildren = true;
                    cell.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(cell.UniqueName + "." + cell.CellValue);
                }

                #region Inserting spanned cells

                //// Inserting column spanned cells
                for (int i = currentColumn; i < cell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engine[currentRow, i + 1];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    spanCell.Tag = g.Key;                    
                }
                #endregion

                if (!isLayoutChanged)
                {
                    if (g.ElementsCount > 0)
                    {
                        PivotCellDescriptor spanCell = engine[currentRow + 1, currentColumn + g.ElementsCount];
                        spanCell.CellValue = g.Key.ToString() + " Total";
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        if (summaryInfos.Length > 0)
                        {
                            spanCell.Tag = summaryInfos[0];
                        }
                        spanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        spanCell.Range = GridRangeInfo.FromTlhw(currentRow + 1, currentColumn + g.ElementsCount, ((columnLevels - 1) - g.Level), 1);

                        for (int row = spanCell.Range.Width; row < spanCell.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engine[currentRow + row + 1, currentColumn + g.ElementsCount];
                            SpanCell.SpanCell = spanCell;
                            SpanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        }

                    }
                }
                if (g.SubGroups != null)
                {
                    //// Child group insert starts form the sample column
                    currentColumn--;
                    //// Moving the child group to next row
                    currentRow++;
                    g.SubGroups.ForEach(i => columnHeaderLoopin(i));
                    //// Re-setting the position
                    currentRow--; currentColumn++;

                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engine[currentRow + 1, currentColumn++];
                            summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                            summaryCell.Range = GridRangeInfo.FromTlhw(currentRow + 1, currentColumn - 1, (columnLevels - 1) - g.Level, 1);

                            for (int row = summaryCell.Range.Width; row < summaryCell.Range.Height; row++)
                            {
                                PivotCellDescriptor SpanCell = engine[currentRow + row + 1, currentColumn - 1];
                                SpanCell.SpanCell = summaryCell;
                                SpanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            }
                        }
                        currentColumn--;
                    }

                }
                else
                {
                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engine[currentRow + 1, currentColumn++];
                            summaryCell.CellValue = summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                        }
                        currentColumn--;
                    }
                }
            };

            if (columnData.Count > 0)
            {
                columnData.ForEach(i => columnHeaderLoopin(i));
            }
            else
            {
                // Include the summaries in the column headers
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engine[0, rowLevels + i];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            #endregion

            /*
             * ==================================================================================================
             * 
             * Processing value cells
             * 
             * ==================================================================================================
             */

            #region Processing value cells

            int baseCol = -1;
            ExpressionHelper helper = new ExpressionHelper();
            for (int row = columnLevels; row < engine.RowsCount; row++)
            {
                for (int column = rowLevels; column < engine.TableColumns.Count; column++)
                {
                    PivotCellDescriptor cell = engine[row, column];
                    if (cell.UniqueName == string.Empty)
                    {
                        cell.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptions(engine, row, column, false);
                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                        groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (groupResult == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptions(engine, row, column, true);
                            groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        }

                        if (TableBuilderHelper.CheckIsSummaryRow(engine, column, row))
                        {
                            cell.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }

                        if (groupResult == null)
                        {
                            List<string> summaryString = new List<string>();

                            //// populating the column summary string
                            for (int i = 0; i < headerInfo.ColumnHeaderCaptions.Count; i++)
                            {
                                summaryString.Add(columnGroup[i]);
                            }

                            //// populating the row summary strings
                            for (int i = 0; i < headerInfo.RowHeaderCaptions.Count; i++)
                            {
                                //if(i<rowGroup.Count())
                                summaryString.Add(rowGroup[i]);
                            }

                            if (summaryString.Count > 0)
                            {
                                //// grouping the values with header information
                                var summary = (queryableSource as DataTable).GroupBy(summaryString.ToArray()).ToList();

                                groupResult = TableBuilderHelper.GetValue(summary, headerInfo);
                            }
                        }
                        if (groupResult != null)
                        {
                            SummaryInfo summaryInfo = null;
                            if (isLayoutChanged)
                            {
                                summaryInfo = headerInfo.SummaryInfo;
                            }
                            else
                            {
                                if (summaryInfos.Length > 0)
                                    summaryInfo = summaryInfos[0];
                            }

                            if (summaryInfo != null)
                            {
                                switch (summaryInfo.Type)
                                {
                                    case SummaryType.Average:
                                        {
                                            result = DataRowHelper.ComputeAvg(groupResult.Items, summaryInfo.Column);
                                            break;
                                        }
                                    case SummaryType.Count:
                                        {
                                            result = DataRowHelper.ComputeCount(groupResult.Items, summaryInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Sum:
                                        {
                                            result = DataRowHelper.ComputeSum(groupResult.Items, summaryInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Max:
                                        {
                                            result = DataRowHelper.ComputeMax(groupResult.Items, summaryInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Min:
                                        {
                                            result = DataRowHelper.ComputeMin(groupResult.Items, summaryInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Expression:
                                        {
                                            result = "Expression Not spported";
                                            break;
                                        };
                                    case SummaryType.First:
                                        {
                                            result = DataRowHelper.ComputeFirst(groupResult.Items,summaryInfo.Column);//  .AsQueryable().Select(summaryInfo.Column).ElementAtOrDefault(0).ToString();
                                            break;
                                        };
                                    case SummaryType.Last:
                                        {
                                            result = DataRowHelper.ComputeLast(groupResult.Items, summaryInfo.Column);// groupResult.Items.AsQueryable().Select(summaryInfo.Column).ElementAtOrDefault(groupResult.Items.AsQueryable().Count() - 1).ToString();
                                            break;
                                        };
                                    case SummaryType.String:
                                        {
                                            result = DataRowHelper.Select(groupResult.Items, summaryInfo.Column);
                                            if (columnData.Count == 0)
                                            {
                                                if (baseCol != row)
                                                {
                                                    if (groupResult.SubGroups == null)
                                                    {
                                                        result = DataRowHelper.StringType(groupResult.Items, summaryInfo.Column, engine, column, row, summaryInfos, GridLayout.Normal, true);
                                                        baseCol = row;
                                                    }
                                                }
                                            }
                                            if (groupResult.Items.AsQueryable().Count() > 1)
                                            {
                                                result = null;
                                            }
                                            break;
                                        };
                                      
                                }
                            }
                            if (summaryInfo.FormatString != null && summaryInfo.FormatString != string.Empty)
                            {
                                string[] formatString = new string[] { "d", "t", "m", "y", "hh", "ss" };
                                double val = 0, dtVal = 0;
                                DateTime dateTime;
                                if (double.TryParse(result, out val))
                                    result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, val);
                                if (formatString.Any(m => summaryInfo.FormatString.ToLower().Contains(m)))
                                {
                                    DateTime dTime = new DateTime();
                                    if (DateTime.TryParse(result, out dTime))
                                    {
                                        dateTime = Convert.ToDateTime(result);
                                        dtVal = dateTime.ToOADate();
                                        if (dtVal != 0)
                                            result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, DateTime.FromOADate(dtVal));
                                    }
                                    else if(val!=0)
                                        result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, DateTime.FromOADate(val));
                                }
                            }
                        }
                        cell.CellType = PivotCellDescriptorType.Value;
                        cell.CellValue = result;
                        cell.CellCaption = result;

                    }
                    cell.CellData = engine.GetCellDataValueforIEnumerable(row, column);
                  
                }
            }

            #endregion

            engine.ClearLevelHeadersArea();
            engine.RecalculateSpans();
            return engine;
        }
#endif
        #endregion
    }
}