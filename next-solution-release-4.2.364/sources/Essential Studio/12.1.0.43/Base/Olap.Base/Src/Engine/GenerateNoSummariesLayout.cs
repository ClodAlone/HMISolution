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
    /// No SummariesLayout - Pivot Engine Generation for IListSource and IEnumerable Source
    /// </summary>
    public class GenerateNoSummariesLayout
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

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

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

            if (!isLayoutChanged)
            {
                subGroup = columnLevels;
            }
            else
            {
                subGroup = columnLevels - 1;
                if (subGroup == 0)
                    subGroup++;
            }

            #endregion

            int rowscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCount = null;

            lastLevelCount = (l) =>
            {
                if (l.SubGroups == null)
                {
                    rowscount++;
                }
                else
                {
                    l.SubGroups.ForEach(i => lastLevelCount(i));
                }
            };

            rowData.ForEach(i => lastLevelCount(i));

            int columnscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCountCol = null;

            lastLevelCountCol = (c) =>
            {
                if (c.SubGroups == null)
                {
                    columnscount++;
                }
                else
                {
                    c.SubGroups.ForEach(i => lastLevelCountCol(i));
                }
            };

            columnData.ForEach(i => lastLevelCountCol(i));

            int temp = 0;
            if (rowData.Count == 0)
            {
                temp = summaryInfos.Length;
            }
            else if (columnData.Count == 0)
            {
                columnscount = 1;
            }

            PivotEngine engineSummaries = null;
            if (!isLayoutChanged)
            {
                //Engine Creation
                engineSummaries = PivotEngine.CreateEngine((subGroup + ((rowscount * summaryInfos.Length)) + temp), columnscount + rowLevels + temp);
                //Specifying the Row Header Section
                // Before it was rowLevels+1

                if (columnData.Count != 0)
                {
                    if (rowData.Count == 0)
                    {
                        //Specifying the Row Header Section
                        engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - subGroup), rowLevels + 1);
                    }
                    else
                    {
                        //Specifying the Column Header Section
                        engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - subGroup), rowLevels);
                    }
                }
                else
                {
                    //Specifying the Row Header Section
                    engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - rowLevels) + 1, rowLevels);
                    //Specifying the Column Header Section
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, subGroup, engineSummaries.TableColumns.Count - rowLevels);
                }
            }
            else
            {
                //Engine Creation
                engineSummaries = PivotEngine.CreateEngine((subGroup + ((rowscount * summaryInfos.Length)) + temp), columnscount + rowLevels + 1);

                //Specifying the Row Header Section
                engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - rowLevels) - 1, rowLevels + 1);

                //Specifying the Column Header Section
                if (columnData.Count == 0)
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels + subGroup, columnLevels, columnCount + 1);
                else
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels + subGroup, columnLevels - 1, columnCount);
            }
            //Preserving the DataSource
            engineSummaries.ItemSource = queryableSource;

            /*
             * Processing Row Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            int cRow = subGroup - 1;
            int cColumn = 0;
            List<string> parent = new List<string>();

            rowHeaderLoop = (g) =>
            {
                cRow++;

                int tempcount = 0;

                Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                LevelCount = (l) =>
                {
                    if (l.SubGroups == null)
                    {
                        tempcount++;
                    }
                    else
                    {
                        l.SubGroups.ForEach(i => LevelCount(i));
                    }
                };

                if (g.SubGroups != null)
                {
                    g.SubGroups.ForEach(i => LevelCount(i));

                }
                else
                    tempcount = 1;

                PivotCellDescriptor cel = engineSummaries[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (tempcount * summaryInfos.Length), 1);
#if !SILVERLIGHT
                if (!cel.Range.ToString().Contains("-"))
                {
                    cel.Range = GridRangeInfo.Empty;
                }
#endif
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

                if (g.ElementsCount == 0)
                {
                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            PivotCellDescriptor measureCell = engineSummaries[cRow + summaryCount, cColumn + 1];
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
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        //spanCell.Tag = g.Key;
                        cRow++;
                    }

                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;
                        g.SubGroups.ForEach(i => rowHeaderLoop(i));
                        //// Re-setting the position
                        cColumn--;
                    }
                }

            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }
            else
            {
                //int tempCount = 0;
                //if (!isLayoutChanged)
                //    tempCount = 1;
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineSummaries[(subGroup) + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoop = null;

            if (!isLayoutChanged && rowData.Count != 0)
            {
                cColumn = rowLevels - 1;
            }
            else
                cColumn = rowLevels;
            cRow = 0;
            colHeaderLoop = (g) =>
            {
                cColumn++;

                if (cColumn < engineSummaries.TableColumns.Count)
                {

                    PivotCellDescriptor colCell = engineSummaries[cRow, cColumn];
                    colCell.CellValue = g.Key.ToString();
                    colCell.CellCaption = g.Key.ToString();
                    colCell.Value = g.Key.ToString();
                    colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    colCell.Level = g.Level + 1;
                    colCell.UniqueName = columnGroup[g.Level];
                    colCell.Tag = g.Key;

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

                    int tempCount = 0;
                    Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                    LevelCount = (l) =>
                    {
                        if (l.SubGroups == null)
                        {
                            tempCount++;
                        }
                        else
                        {
                            l.SubGroups.ForEach(i => LevelCount(i));
                        }
                    };

                    if (g.SubGroups != null)
                    {
                        g.SubGroups.ForEach(i => LevelCount(i));

                    }

                    colCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, tempCount);//(g.ElementsCount * subGroup) - columnLevels);

                    for (int i = cColumn; i < colCell.Range.Right; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[cRow, i + 1];
                        spanCell.SpanCell = colCell;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                        spanCell.Tag = g.Key;
                    }

                    if (g.ElementsCount > 0)
                    {
                        cColumn--;
                        //// Moving the child group to next row
                        cRow++;
                        g.SubGroups.ForEach(i => colHeaderLoop(i));
                        //// Re-setting the position
                        cRow--;
                    }
                }

            };

            columnData.ForEach(i => colHeaderLoop(i));

            /*
             * Processing Value Cells
             */


#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif
            if (!isLayoutChanged)
            {
                rowLevels--;
            }

            for (int row = rowLevels + 1; row < engineSummaries.TableColumns.Count; row++)
            {
                for (int column = subGroup; column < engineSummaries.TableColumns[row].Cells.Count; column++)
                {
                    PivotCellDescriptor cellVal = engineSummaries[column, row];
                    cellVal.CellType = PivotCellDescriptorType.Value;
                    HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineSummaries, column, row, false);
                    string result = string.Empty;
                    Syncfusion.Olap.Engine.Extension.GroupResult grpResult = null;
                    grpResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                    if (grpResult == null)
                    {
                        headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineSummaries, column, row, true);
                        grpResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);                        
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
                            if (i < rowGroup.Count())
                            {
                                summaryString.Add(rowGroup[i]);
                            }
                        }

                        //// grouping the values with header information
                        if (summaryString.Count > 0)
                        {
                            var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();

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
#if !SILVERLIGHT
                                case SummaryType.Expression:
                                    {
                                        object val = helper.ComputeSummary(sumInfo.Expression, grpResult.Items);//.AsQueryable());
                                        if (val != null)
                                            result = val.ToString();
                                        break;
                                    };                             
#endif
                                case SummaryType.Max:
                                    {
                                        result = grpResult.Items.AsQueryable().Max(sumInfo.Column).ToString();
                                        break;
                                    };
                                case SummaryType.Min:
                                    {
                                        result = grpResult.Items.AsQueryable().Min(sumInfo.Column).ToString();
                                        break;
                                    };
                                case SummaryType.String:
                                case SummaryType.First:
                                    {
                                        result = grpResult.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(0).ToString();
                                        break;
                                    };
                                case SummaryType.Last:
                                    {
                                        result = grpResult.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(grpResult.Items.AsQueryable().Count() - 1).ToString();
                                        break;
                                    };
                                  
                            }
                            if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                            {
                                double val = 0;
                                if (double.TryParse(result, out val))
                                    result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, val);
                            }
                            cellVal.Value = result;
                            cellVal.CellValue = result;
                            cellVal.CellCaption = result;
                            cellVal.CellType = PivotCellDescriptorType.Value;                         

                        }
                    }
                   
                    cellVal.CellData = engineSummaries.GetCellDataValueforIEnumerable(column, row);
                }
            }

            engineSummaries.ClearLevelHeadersArea();
            engineSummaries.RecalculateSpans();
            return engineSummaries;
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

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

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

            if (isLayoutChanged)
                subGroup = columnLevels - 1;
            else
                subGroup = columnLevels;

            #endregion

            int rowscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCount = null;

            lastLevelCount = (l) =>
            {
                if (l.SubGroups == null)
                {
                    rowscount++;
                }
                else
                {
                    l.SubGroups.ForEach(i => lastLevelCount(i));
                }
            };

            rowData.ForEach(i => lastLevelCount(i));

            int columnscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCountCol = null;

            lastLevelCountCol = (c) =>
            {
                if (c.SubGroups == null)
                {
                    columnscount++;
                }
                else
                {
                    c.SubGroups.ForEach(i => lastLevelCountCol(i));
                }
            };

            columnData.ForEach(i => lastLevelCountCol(i));

            int temp = 0;
            //if (columnData.Count != null)
            {
                if (columnData.Count == 0)
                {
                    temp = summaryInfos.Length;
                }
            }
            if (rowData.Count == 0)
            {
                rowscount++;
            }

            //Engine Creation
            PivotEngine engineSummaries = PivotEngine.CreateEngine((columnLevels + ((rowscount))), rowLevels + (columnscount * summaryInfos.Length) + temp);

            engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, engineSummaries.RowsCount - columnLevels, rowLevels);

            //Specifying the ColumnHeaderSection
            if (!isLayoutChanged && columnData.Count == 0)
                engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, (engineSummaries.TableColumns.Count) - rowLevels);
            else
                engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, (engineSummaries.TableColumns.Count) - rowLevels);

            //Preserving the DataSource
            engineSummaries.ItemSource = queryableSource;

            /*
             * Processing Row Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            List<string> parent = new List<string>();

            int cRow = 0;
            if (summaryInfos.Length > 1)
            {
                cRow = subGroup;
            }

            else if (columnData.Count != 0)
                cRow = subGroup - 1;
            
            int cColumn = 0;
            rowHeaderLoop = (g) =>
            {
                cRow++;

                int tempcount = 0;

                Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                LevelCount = (l) =>
                {
                    if (l.SubGroups == null)
                    {
                        tempcount++;
                    }
                    else
                    {
                        l.SubGroups.ForEach(i => LevelCount(i));
                    }
                };

                if (g.SubGroups != null)
                {
                    g.SubGroups.ForEach(i => LevelCount(i));

                }
                else
                    tempcount = 0;


                PivotCellDescriptor cel = engineSummaries[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Level = g.Level + 1;
                cel.Tag = g.Key;
                cel.UniqueName = rowGroup[g.Level];

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

                if (tempcount == 1)
                {
                    tempcount = 0;
                }

                cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, tempcount, 1);

                if (g.ElementsCount == 0)
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        spanCell.Tag = g.Key;
                        cRow++;
                    }

                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        spanCell.Tag = g.Key;
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;
                        g.SubGroups.ForEach(i => rowHeaderLoop(i));
                        //// Re-setting the position                        
                        cColumn--;

                    }
                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopin = null;
            cColumn = rowLevels - 1;
            cRow = 0;
            columnHeaderLoopin = (g) =>
            {
                cColumn++;
                    PivotCellDescriptor cell = engineSummaries[cRow, cColumn];
                    cell.CellValue = g.Key.ToString();
                    cell.Value = g.Key.ToString();
                    cell.CellCaption = g.Key.ToString();
                    cell.CellType = PivotCellDescriptorType.ColumnHeader;
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

                    if (g.ElementsCount == 1 || g.ElementsCount == 0)
                    {
                        if (summaryInfos.Length == 1)
                        {
                            cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, 0);
                        }
                        else
                            cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, summaryInfos.Length);
                    }
                    else
                    {

                        int tempCount = 0;
                        Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;
                        LevelCount = (l) =>
                        {
                            if (l.SubGroups == null)
                            {
                                tempCount++;
                            }
                            else
                            {
                                l.SubGroups.ForEach(i => LevelCount(i));
                            }
                        };

                        if (g.SubGroups != null)
                        {
                            g.SubGroups.ForEach(i => LevelCount(i));
                        }
                        cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, summaryInfos.Length * tempCount);
                    }

                    #region Inserting spanned cells

                    //// Inserting column spanned cells
                    for (int i = cColumn; i < cell.Range.Right; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[cRow, i + 1];
                        spanCell.SpanCell = cell;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                        spanCell.Tag = g.Key;                       
                    }
                    #endregion

                    if (g.SubGroups != null)
                    {
                        //// Child group insert starts form the sample column
                        cColumn--;
                        //// Moving the child group to next row
                        cRow++;
                        g.SubGroups.ForEach(i => columnHeaderLoopin(i));
                        //// Re-setting the position
                        cRow--;
                    }
                    else
                    {
                        if (isLayoutChanged)
                        {
                            for (int i = 0; i < summaryInfos.Length; i++)
                            {

                                PivotCellDescriptor summaryCell = engineSummaries[cRow + 1, cColumn++];
                                summaryCell.CellValue = summaryInfos[i].Key;
                                summaryCell.Value = summaryInfos[i].Key;
                                summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                                summaryCell.CellCaption = summaryInfos[i].Key;
                                summaryCell.Tag = summaryInfos[i];
                            }
                            cColumn--;
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
                    PivotCellDescriptor summaryCell = engineSummaries[0, rowLevels + i];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            /*
             * Processing Value Cells
             */

            if (rowData.Count == 0)
            {
                engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(0, 0, engineSummaries.RowsCount, engineSummaries.TableColumns.Count);
            }

            int baseCol = -1;
#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif
            for (int row = columnLevels; row < engineSummaries.RowHeaderSection.Bottom + 1; row++)
            {
                for (int column = rowLevels; column < engineSummaries.HeaderSection.Right + 1; column++)
                {
                    if (row < engineSummaries.RowsCount)
                    {
                        PivotCellDescriptor cell = engineSummaries[row, column];
                        if (cell.UniqueName == string.Empty)
                        {
                            if (cell.CellType != PivotCellDescriptorType.ColumnHeader)
                            {
                                cell.CellType = PivotCellDescriptorType.Value;
                                HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptions(engineSummaries, row, column, false);
                                string result = string.Empty;
                                Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                                groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                                if (groupResult == null)
                                {
                                    headerInfo = TableBuilderHelper.GetHeaderCaptions(engineSummaries, row, column, true);
                                    groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                                    cell.CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());
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
                                        if (i<rowGroup.Count())
                                        {
                                            summaryString.Add(rowGroup[i]);
                                        }
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
                                                };
                                            case SummaryType.Min:
                                                {
                                                    result = groupResult.Items.AsQueryable().Min(summaryInfo.Column).ToString();
                                                    break;
                                                };
#if !SILVERLIGHT
                                            case SummaryType.Expression:
                                                {
                                                    object val = helper.ComputeSummary(summaryInfo.Expression, groupResult.Items);//.AsQueryable());
                                                    if (val != null)
                                                        result = val.ToString();
                                                    break;
                                                };
#endif
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
                                                                    engineSummaries.InsertAdditionalRows(groupResult.Items, summaryInfo.Column, column, row, summaryInfos, GridLayout.Normal, false);
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
                                cell.Value = result;
                                cell.CellValue = result;
                                cell.CellCaption = result;

                            }
                        }
                        cell.CellData = engineSummaries.GetCellDataValueforIEnumerable(row, column);
                    }
                }
            }

            if (rowData.Count == 0)
            {
                engineSummaries.RowHeaderSection = GridRangeInfo.Empty;
            }

            engineSummaries.ClearLevelHeadersArea();
            engineSummaries.RecalculateSpans();
            return engineSummaries;
        }

        #endregion

        /*
         *  ==================================================================================================
         * 
         *  IListSource 
         * 
         *  ===================================================================================================
         */

#if !SILVERLIGHT
        #region IListSource

        internal static PivotEngine ProcessRowMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, System.ComponentModel.IListSource listSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged,int summaryStringCount)
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

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

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
            if(Index.Count > 0) 
            subGroup = Index.Max() + 1;
            rowLevels = subGroup;

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

            if (!isLayoutChanged)
            {
                subGroup = columnLevels;
            }
            else
            {
                subGroup = columnLevels - 1;
                if (subGroup == 0)
                    subGroup++;
            }

            #endregion

            int rowscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCount = null;

            lastLevelCount = (l) =>
            {
                if (l.SubGroups == null)
                {
                    rowscount++;
                }
                else
                {
                    l.SubGroups.ForEach(i => lastLevelCount(i));
                }
            };

            rowData.ForEach(i => lastLevelCount(i));

            int columnscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCountCol = null;

            lastLevelCountCol = (c) =>
            {
                if (c.SubGroups == null)
                {
                    columnscount++;
                }
                else
                {
                    c.SubGroups.ForEach(i => lastLevelCountCol(i));
                }
            };

            columnData.ForEach(i => lastLevelCountCol(i));

            int temp = 0;
            if (rowData.Count == 0)
            {
                temp = summaryInfos.Length;
            }
            else if (columnData.Count == 0)
            {
                columnscount = 1;
            }

            PivotEngine engineSummaries = null;
            if (!isLayoutChanged)
            {
                //Engine Creation
                engineSummaries = PivotEngine.CreateEngine((subGroup + ((rowscount * summaryInfos.Length)) + temp), columnscount + rowLevels + temp);
                //Specifying the Row Header Section
                // Before it was rowLevels+1

                if (columnData.Count != 0)
                {
                    if (rowData.Count == 0)
                    {
                        //Specifying the Row Header Section
                        engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - subGroup), rowLevels + 1);
                    }
                    else
                    {
                        //Specifying the Column Header Section
                        engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - subGroup), rowLevels);
                    }
                }
                else
                {
                    //Specifying the Row Header Section
                    engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - rowLevels) + 1, rowLevels);
                    //Specifying the Column Header Section
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, subGroup, engineSummaries.TableColumns.Count - rowLevels);
                }               
            }
            else
            {
                //Engine Creation
                engineSummaries = PivotEngine.CreateEngine((subGroup + ((rowscount * summaryInfos.Length)) + temp), columnscount + rowLevels + 1);

                //Specifying the Row Header Section
                engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(subGroup, 0, (engineSummaries.RowsCount - rowLevels) - 1, rowLevels + 1);

                //Specifying the Column Header Section
                if(columnData.Count==0)
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels+subGroup, columnLevels, columnCount+1);
                else
                    engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels + subGroup, columnLevels - 1, columnCount);
            }
            //Preserving the DataSource
            engineSummaries.ItemSource = listSource;

            /*
             * Processing Row Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            int cRow = subGroup - 1;
            int cColumn = 0;
            List<string> parent = new List<string>();

            rowHeaderLoop = (g) =>
            {
                cRow++;
                int tempcount = 0;

                Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;
                LevelCount = (l) =>
                {
                    if (l.SubGroups == null)
                    {
                        tempcount++;
                    }
                    else
                    {
                        l.SubGroups.ForEach(i => LevelCount(i));
                    }
                };

                if (g.SubGroups != null)
                {
                    g.SubGroups.ForEach(i => LevelCount(i));
                }
                else
                    tempcount = 1;

                PivotCellDescriptor cel = engineSummaries[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, (tempcount * summaryInfos.Length), 1);
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
              
                if (g.ElementsCount == 0)
                {
                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            PivotCellDescriptor measureCell = engineSummaries[cRow + summaryCount, cColumn + 1];
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
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        //spanCell.Tag = g.Key;
                        cRow++;
                    }
                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;
                        g.SubGroups.ForEach(i => rowHeaderLoop(i));
                        //// Re-setting the position
                        cColumn--;
                    }
                }

            };
                        
            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }
            else
            {
                int tempCount = 0;
                if (!isLayoutChanged)
                    tempCount = 1;
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineSummaries[(subGroup) + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoop = null;

            if (!isLayoutChanged && rowData.Count != 0)
            {
                cColumn = rowLevels - 1;
            }
            else
                cColumn = rowLevels;
            cRow = 0;
            colHeaderLoop = (g) =>
            {
                cColumn++;

                if (cColumn < engineSummaries.TableColumns.Count)
                {
                    PivotCellDescriptor colCell = engineSummaries[cRow, cColumn];
                    colCell.CellValue = g.Key.ToString();
                    colCell.CellCaption = g.Key.ToString();
                    colCell.Value = g.Key.ToString();
                    colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    colCell.Level = g.Level + 1;
                    colCell.UniqueName = columnGroup[g.Level];
                    colCell.Tag = g.Key;

                    if (g.Level == 0)
                    {
                        parent = new List<string>();
                    }

                    parent =TableBuilderHelper.FillParentCells(colCell, parent);

                    if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                    {
                        colCell.HasChildren = true;
                        colCell.ExpandableState = g.ExpandableState;
                    }

                    if (g.SubGroups != null)
                    {
                        parent.Add(colCell.UniqueName + "." + colCell.CellValue);
                    }

                    int tempCount = 0;
                    Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                    LevelCount = (l) =>
                    {
                        if (l.SubGroups == null)
                        {
                            tempCount++;
                        }
                        else
                        {
                            l.SubGroups.ForEach(i => LevelCount(i));
                        }
                    };

                    if (g.SubGroups != null)
                    {
                        g.SubGroups.ForEach(i => LevelCount(i));

                    }

                    colCell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, tempCount);//(g.ElementsCount * subGroup) - columnLevels);

                    for (int i = cColumn; i < colCell.Range.Right; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[cRow, i + 1];
                        spanCell.SpanCell = colCell;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                        spanCell.Tag = g.Key;
                    }

                    if (g.ElementsCount > 0)
                    {
                        cColumn--;
                        //// Moving the child group to next row
                        cRow++;
                        g.SubGroups.ForEach(i => colHeaderLoop(i));
                        //// Re-setting the position
                        cRow--;                        
                    }
                }
            };

            columnData.ForEach(i => colHeaderLoop(i));

            /*
             * Processing Value Cells
             */

            if (!isLayoutChanged && rowData != null)
            {
                if (rowData.Count > 0)
                    rowLevels -= 1;
            }

            for (int row = rowLevels + 1; row < engineSummaries.TableColumns.Count; row++)
            {
                for (int column = subGroup; column < engineSummaries.TableColumns[row].Cells.Count; column++)
                {
                    PivotCellDescriptor cellVal = engineSummaries[column, row];
                    cellVal.CellType = PivotCellDescriptorType.Value;
                    HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineSummaries, column, row, false);
                    string result = string.Empty;
                    Syncfusion.Olap.Engine.Extension.GroupResult grpResult = null;
                    grpResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                    if (grpResult == null)
                    {
                        headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummary(engineSummaries, column, row, true);
                        grpResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);                       
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
                            if (i < rowGroup.Count())
                            {
                                summaryString.Add(rowGroup[i]);
                            }
                        }

                        if (summaryString.Count != 0)
                        {
                            //// grouping the values with header information
                            var summary = (listSource as DataTable).GroupBy(summaryString.ToArray()).ToList();
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
                                        break;
                                    };
                                case SummaryType.String:
                                    {
                                        result = DataRowHelper.Select(grpResult.Items, sumInfo.Column);
                                        break;
                                    };
                                case SummaryType.First:
                                    {
                                        result = DataRowHelper.ComputeFirst(grpResult.Items, sumInfo.Column);
                                        break;
                                    };
                                case SummaryType.Last:
                                    {
                                        result = DataRowHelper.ComputeLast(grpResult.Items, sumInfo.Column);
                                        break;
                                    };                        
                                 
                            }
                            if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                            {
                                string[] formatString = new string[] { "d", "t", "m", "y", "hh", "ss" };
                                double val = 0, dtVal = 0;
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
                            cellVal.Value = result;
                            cellVal.CellCaption = result;
                            cellVal.CellType = PivotCellDescriptorType.Value;

                        }
                    }
                    cellVal.CellData = engineSummaries.GetCellDataValueforIEnumerable(column, row);
                }
            }

            engineSummaries.ClearLevelHeadersArea();
            engineSummaries.RecalculateSpans();
           
            return engineSummaries;
        }

        internal static PivotEngine ProcessColumnMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IListSource queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged,int summaryStringCount)
        {
            int count = 0,
                rowCount = 0,
                columnCount = 0,
                rowLevels = 0,
                columnLevels = 0,
                subGroup = 0;

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

            /// Calculating column count
            /// re-setting the count variable
            count = 0;
            /// Iterating column data
            columnData.ForEach(i => countLoopin(i));
            //// updating count to the column count variable
            columnCount = count;

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

            if (isLayoutChanged)
                subGroup = columnLevels - 1;
            else
                subGroup = columnLevels;

            #endregion

            int rowscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCount = null;

            lastLevelCount = (l) =>
            {
                if (l.SubGroups == null)
                {
                    rowscount++;
                }
                else
                {
                    l.SubGroups.ForEach(i => lastLevelCount(i));
                }
            };

            rowData.ForEach(i => lastLevelCount(i));

            int columnscount = 0;

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> lastLevelCountCol = null;

            lastLevelCountCol = (c) =>
            {
                if (c.SubGroups == null)
                {
                    columnscount++;
                }
                else
                {
                    c.SubGroups.ForEach(i => lastLevelCountCol(i));
                }
            };

            columnData.ForEach(i => lastLevelCountCol(i));

            int temp = 0;
            //if (columnData.Count != null)
            {
                if (columnData.Count == 0)
                {
                    temp = summaryInfos.Length;
                }              
            }
            if (rowData.Count == 0)
            {
                rowscount++;
            }

            //Engine Creation
            PivotEngine engineSummaries = PivotEngine.CreateEngine((columnLevels + ((rowscount))), rowLevels + (columnscount * summaryInfos.Length) + temp);

            engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, engineSummaries.RowsCount - columnLevels, rowLevels);

            //Specifying the ColumnHeaderSection
            if (!isLayoutChanged && columnData.Count == 0)
                engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels , (engineSummaries.TableColumns.Count) - rowLevels);
            else
                engineSummaries.HeaderSection = GridRangeInfo.FromTlhw(0, rowLevels, columnLevels, (engineSummaries.TableColumns.Count) - rowLevels);

            //Preserving the DataSource
            engineSummaries.ItemSource = queryableSource;

            /*
             * Processing Row Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoop = null;
            List<string> parent = new List<string>();

            int cRow = 0;
            if (summaryInfos.Length > 1)
            {
                cRow = subGroup;
            }

            else if (columnData.Count != 0)
                cRow = subGroup - 1;
         
            int cColumn = 0;
            rowHeaderLoop = (g) =>
            {
                cRow++;

                int tempcount = 0;

                Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                LevelCount = (l) =>
                {
                    if (l.SubGroups == null)
                    {
                        tempcount++;
                    }
                    else
                    {
                        l.SubGroups.ForEach(i => LevelCount(i));
                    }
                };

                if (g.SubGroups != null)
                {
                    g.SubGroups.ForEach(i => LevelCount(i));

                }
                else
                    tempcount = 0;

                PivotCellDescriptor cel = engineSummaries[cRow, cColumn];
                cel.CellValue = g.Key.ToString();
                cel.CellCaption = g.Key.ToString();
                cel.Value = g.Key.ToString();
                cel.CellType = PivotCellDescriptorType.RowHeader;
                cel.Level = g.Level + 1;
                cel.Tag = g.Key;
                cel.UniqueName = rowGroup[g.Level];

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

                if (tempcount == 1)
                {
                    tempcount = 0;
                }

                cel.Range = GridRangeInfo.FromTlhw(cRow, cColumn, tempcount, 1);

                if (g.ElementsCount == 0)
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        spanCell.Tag = g.Key;
                        cRow++;
                    }

                }
                else
                {
                    for (int i = cRow; i < cel.Range.Bottom; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[i + 1, cColumn];
                        spanCell.SpanCell = cel;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.RowHeader;
                        spanCell.Tag = g.Key;
                    }

                    if (g.SubGroups != null)
                    {
                        cColumn++;
                        //// Moving the child group to next row
                        cRow--;
                        g.SubGroups.ForEach(i => rowHeaderLoop(i));
                        //// Re-setting the position                        
                        cColumn--;
                    }
                }
            };

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoop(i));
            }

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopin = null;
            cColumn = rowLevels - 1;
            cRow = 0;
            columnHeaderLoopin = (g) =>
            {
                cColumn++;
                 PivotCellDescriptor cell = engineSummaries[cRow, cColumn];
                    cell.CellValue = g.Key.ToString();
                    cell.Value = g.Key.ToString();
                    cell.CellCaption = g.Key.ToString();
                    cell.CellType = PivotCellDescriptorType.ColumnHeader;
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

                    if (g.ElementsCount == 1 || g.ElementsCount == 0)
                    {
                        if (summaryInfos.Length == 1)
                        {
                            cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, 0);
                        }
                        else
                        {                        
                            cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, summaryInfos.Length);
                        }
                    }
                    else
                    {
                        int tempCount = 0;

                        Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                        LevelCount = (l) =>
                        {
                            if (l.SubGroups == null)
                            {
                                tempCount++;
                            }
                            else
                            {
                                l.SubGroups.ForEach(i => LevelCount(i));
                            }
                        };

                        if (g.SubGroups != null)
                        {
                            g.SubGroups.ForEach(i => LevelCount(i));
                        }

                        cell.Range = GridRangeInfo.FromTlhw(cRow, cColumn, 1, summaryInfos.Length * tempCount);
                    }

        #region Inserting spanned cells

                    //// Inserting column spanned cells
                    for (int i = cColumn; i < cell.Range.Right; i++)
                    {
                        PivotCellDescriptor spanCell = engineSummaries[cRow, i + 1];
                        spanCell.SpanCell = cell;
                        spanCell.CellValue = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                        spanCell.Tag = g.Key;                      
                    }
                    #endregion

                    if (g.SubGroups != null)
                    {
                        //// Child group insert starts form the sample column
                        cColumn--;
                        //// Moving the child group to next row
                        cRow++;
                        g.SubGroups.ForEach(i => columnHeaderLoopin(i));
                        //// Re-setting the position
                        cRow--;
                    }
                    else
                    {
                        if (isLayoutChanged)
                        {
                            for (int i = 0; i < summaryInfos.Length; i++)
                            {
                                PivotCellDescriptor summaryCell = engineSummaries[cRow + 1, cColumn++];
                                summaryCell.CellValue = summaryInfos[i].Key;
                                summaryCell.Value = summaryInfos[i].Key;
                                summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                                summaryCell.CellCaption = summaryInfos[i].Key;
                                summaryCell.Tag = summaryInfos[i];

                            }
                            cColumn--;
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
                    PivotCellDescriptor summaryCell = engineSummaries[0, rowLevels + i];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                }
            }

            /*
             * Processing Value Cells
             */

            if (rowData.Count == 0)
            {
                engineSummaries.RowHeaderSection = GridRangeInfo.FromTlhw(0, 0, engineSummaries.RowsCount, engineSummaries.TableColumns.Count);
            }

            int baseCol = -1;
            for (int row = columnLevels; row < engineSummaries.RowHeaderSection.Bottom + 1; row++)
            {
                for (int column = rowLevels; column < engineSummaries.HeaderSection.Right + 1; column++)
                {
                    if (row < engineSummaries.RowsCount)
                    {
                        PivotCellDescriptor cell = engineSummaries[row, column];
                        if (cell.UniqueName == string.Empty)
                        {
                            cell.CellType = PivotCellDescriptorType.Value;
                            HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptions(engineSummaries, row, column, false);
                            string result = string.Empty;
                            Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                            groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                            if (groupResult == null)
                            {
                                headerInfo = TableBuilderHelper.GetHeaderCaptions(engineSummaries, row, column, true);
                                groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                                cell.CellExTypes.Add(PivotCellDescriptorType.RowHeader.ToString());
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
                                    if (i < rowGroup.Count())
                                    {
                                        summaryString.Add(rowGroup[i]);
                                    }
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
                                        case SummaryType.String:
                                            {
                                                result = DataRowHelper.Select(groupResult.Items, summaryInfo.Column);
                                                if (columnData.Count == 0)
                                                {
                                                    if (baseCol != row)
                                                    {
                                                        if (groupResult.SubGroups == null)
                                                        {
                                                            result = DataRowHelper.StringType(groupResult.Items, summaryInfo.Column, engineSummaries, column, row, summaryInfos, GridLayout.Normal, true);
                                                            baseCol = row;
                                                        }
                                                    }
                                                }
                                                if (groupResult.Items.AsQueryable().Count() > 1)
                                                {
                                                    result = null;
                                                }
                                            }
                                            break;
                                        case SummaryType.First:
                                            {
                                                result = DataRowHelper.ComputeFirst(groupResult.Items, summaryInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Last:
                                            {
                                                result = DataRowHelper.ComputeLast(groupResult.Items, summaryInfo.Column);
                                                break;
                                            };
                                    }
                                }
                                if (summaryInfo.FormatString != null && summaryInfo.FormatString != string.Empty)
                                {
                                    string[] formatString = new string[] { "d", "t", "m", "y", "hh", "ss" };
                                    double val = 0,dtVal=0;
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
                                        else if (val != 0)
                                            result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, DateTime.FromOADate(val));
                                    }
                                }
                            }
                            cell.CellType = PivotCellDescriptorType.Value;
                            cell.CellValue = result;
                            cell.Value = result;
                            cell.CellCaption = result;

                        }
                        cell.CellData = engineSummaries.GetCellDataValueforIEnumerable(row, column);
                    }

                }
            }

            if (rowData.Count == 0)
            {
                engineSummaries.RowHeaderSection = GridRangeInfo.Empty;
            }
            engineSummaries.ClearLevelHeadersArea();
            engineSummaries.RecalculateSpans();
          
            return engineSummaries;
        }

        #endregion
#endif
    }
}
