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
    /// ExcelLikeLayout - Pivot Engine Generation for IListSource and IEnumerable Source
    /// </summary>
    public class GenerateExcelLikeLayout
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
          subGroup = 0,
          temp_subGroup = 1;

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

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin1 = null;
            List<int> Index = new List<int>();
            levelLoopin1 = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin1);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    temp_subGroup += 1;
                    g.SubGroups.ForEach(levelLoopin);
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                    temp_subGroup = 1;
                }
                else
                {
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                }
            };

            //// resetting the subGroup for row processing
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            rowLevels = subGroup;
            //// resetting the subGroup for column processing
            //subGroup = 0;
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin1(i));
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

            PivotEngine engineRowLayout = null;
            int tempCount = 0;

            if (rowData.Count == 0)
            {
                if (summaryInfos.Length == 0)
                    rowCount = summaryInfos.Length;
                else
                    tempCount = summaryInfos.Length;
            }
            else if (columnData.Count == 0)
            {
                columnCount = 1;
            }

            if (summaryInfos.Length == 1)
            {
                //Engine Creation
                engineRowLayout = PivotEngine.CreateEngine(columnLevels + rowCount, (columnCount + 1));

                //Specifying the Row Header Section
                engineRowLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, (engineRowLayout.RowsCount) - summaryInfos.Length, 1);

                //Specifying the Column Header Section
                engineRowLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, subGroup, columnCount);
            }

            else
            {
                //Engine Creation
                engineRowLayout = PivotEngine.CreateEngine(subGroup + ((rowCount * summaryInfos.Length) + rowCount + tempCount), (columnCount + 1));

                //Specifying the Row Header Section
                engineRowLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels - 1, 0, (engineRowLayout.RowsCount) - subGroup, 1);

                //Specifying the Column Header Section
                engineRowLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, subGroup, columnCount);
            }

            //Preserving the DataSource
            engineRowLayout.ItemSource = queryableSource;

            List<PivotCellDescriptor> parentList = new List<PivotCellDescriptor>();

            /*
             * Processing Row Headers 
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopLayout = null;
            int cRowLayout = subGroup - 1;
            int cColumnLayout = 0;
            List<string> parent = new List<string>();

            rowHeaderLoopLayout = (g) =>
            {
                cRowLayout++;
                PivotCellDescriptor celLayout = engineRowLayout[cRowLayout, cColumnLayout];
                celLayout.CellValue = g.Key.ToString();
                celLayout.CellCaption = g.Key.ToString();
                celLayout.Value = g.Key.ToString();
                celLayout.Level = g.Level + 1;
                celLayout.CellType = PivotCellDescriptorType.RowHeader;

                celLayout.UniqueName = rowGroup[g.Level];
                if (!isLayoutChanged)
                {
                    celLayout.Tag = summaryInfos[0];
                }

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(celLayout, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    celLayout.HasChildren = true;
                    celLayout.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(celLayout.UniqueName + "." + celLayout.CellValue);
                }

                if (g.ElementsCount == 0)
                {
                    celLayout.IsLastLevel = true;
                    if (!isLayoutChanged)
                    {
                        parentList.Add(celLayout);
                        PivotCellDescriptor measureCell = engineRowLayout[cRowLayout, cColumnLayout];
                        foreach (PivotCellDescriptor item in parentList)
                        {
                            measureCell.ParentCellDescriptors.Add(item);
                        }
                        parentList.Remove(celLayout);
                    }

                    if (isLayoutChanged)
                    {
                        parentList.Add(celLayout);
                    }

                    if (isLayoutChanged)
                    {
                        PivotCellDescriptor cellSpan = engineRowLayout[cRowLayout, cColumnLayout + 1];
                        cellSpan.CellCaption = "Span";
                        cellSpan.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout + 1, 1, engineRowLayout.TableColumns.Count - 1);

                        for (int row = cellSpan.Range.Height; row < cellSpan.Range.Width; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout, cColumnLayout + row + 1];
                            SpanCell.SpanCell = cellSpan;
                            SpanCell.CellCaption = "Span";
                        }
                    }

                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            cRowLayout++;
                            PivotCellDescriptor measureCell = engineRowLayout[cRowLayout, cColumnLayout];
                            measureCell.CellValue = summaryInfos[summaryCount].Key;
                            measureCell.CellCaption = summaryInfos[summaryCount].Key;
                            measureCell.Value = summaryInfos[summaryCount].Key;
                            measureCell.Level = g.Level + 2;
                            measureCell.IsLastLevel = true;

                            foreach (PivotCellDescriptor item in parentList)
                            {
                                measureCell.ParentCellDescriptors.Add(item);
                            }
                            measureCell.CellType = PivotCellDescriptorType.RowHeader;
                            measureCell.Tag = summaryInfos[summaryCount];
                        }
                        parentList.Remove(celLayout);
                    }
                }

                else
                {
                    parentList.Add(celLayout);

                    if (isLayoutChanged)
                    {
                        PivotCellDescriptor cellSpan = engineRowLayout[cRowLayout, cColumnLayout + 1];
                        cellSpan.CellCaption = "Span";

                        cellSpan.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout + 1, 1, engineRowLayout.TableColumns.Count - 1);

                        for (int row = cellSpan.Range.Height; row < cellSpan.Range.Width; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout, cColumnLayout + row + 1];
                            SpanCell.SpanCell = cellSpan;
                            SpanCell.CellCaption = "Span";
                        }
                    }                    

                    g.SubGroups.ForEach(i => rowHeaderLoopLayout(i));

                    parentList.Remove(celLayout);

                    for (int sumCount = 0; sumCount < summaryInfos.Length; sumCount++)
                    {
                        if (isLayoutChanged)
                        {
                            cRowLayout++;
                            PivotCellDescriptor cellDescSummary = engineRowLayout[cRowLayout, cColumnLayout];
                            cellDescSummary.CellValue = g.Key.ToString() + " " + summaryInfos[sumCount].Key;
                            cellDescSummary.Level = g.Level + 1;
                            cellDescSummary.CellCaption = summaryInfos[sumCount].Key;
                            cellDescSummary.Value = summaryInfos[sumCount].Key;
                            cellDescSummary.CellType = PivotCellDescriptorType.SummaryRow;
                            cellDescSummary.Tag = summaryInfos[sumCount];
                            
                            foreach (PivotCellDescriptor item in parentList)
                            {
                                if (isLayoutChanged)
                                {
                                    cellDescSummary.ParentCellDescriptors.Add(item);
                                }
                            }

                            PivotCellDescriptor temp = new PivotCellDescriptor();
                            temp.CellValue = g.Key.ToString();
                            temp.CellCaption = g.Key.ToString();
                            temp.Value = g.Key.ToString();
                            temp.Level = g.Level + 2;
                            temp.CellType = PivotCellDescriptorType.RowHeader;
                            cellDescSummary.ParentCellDescriptors.Add(temp);
                        }
                    }
                }
            };

            parentList = new List<PivotCellDescriptor>();

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopLayout(i));
            }
            else
            {
                int temp = 0;
                if (isLayoutChanged)
                    temp = 1;
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineRowLayout[(columnLevels - temp) + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                    summaryCell.IsLastLevel = true;
                }
            }

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoopLayout = null;
            cColumnLayout = 0;
            cRowLayout = 0;
            colHeaderLoopLayout = (g) =>
            {
                cColumnLayout++;
                PivotCellDescriptor colCell = engineRowLayout[cRowLayout, cColumnLayout];
                colCell.CellValue = g.Key.ToString();
                colCell.CellCaption = g.Key.ToString();
                colCell.Value = g.Key.ToString();
                colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                colCell.Level = g.Level + 1;
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

                if (g.ElementsCount == 1)
                {
                    colCell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, 0);
                }
                else
                    colCell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, (g.ElementsCount));

                for (int i = cColumnLayout; i < colCell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineRowLayout[cRowLayout, i + 1];
                    spanCell.SpanCell = colCell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                }

                if (g.ElementsCount > 0)
                {
                    cColumnLayout--;
                    //// Moving the child group to next row
                    cRowLayout++;
                    g.SubGroups.ForEach(i => colHeaderLoopLayout(i));
                    //// Re-setting the position
                    cRowLayout--; cColumnLayout++;

                    if (g.SubGroups != null)
                    {
                        PivotCellDescriptor cellDesc = engineRowLayout[cRowLayout, cColumnLayout];
                        cellDesc.CellValue = g.Key.ToString() + " Total";
                        cellDesc.CellCaption = g.Key.ToString();
                        cellDesc.Value = g.Key.ToString();
                        cellDesc.CellType = PivotCellDescriptorType.SummaryColumn;

                        if (!isLayoutChanged)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, (columnLevels - g.Level), 1);
                        }
                        else
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, (columnLevels - 1) - (g.Level), 1);
                     
                        for (int row = cellDesc.Range.Width; row < cellDesc.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout + row, cColumnLayout];
                            SpanCell.SpanCell = cellDesc;
                            SpanCell.CellType = cellDesc.CellType;
                        }
                    }
                }
            };

            if (columnData != null)
            {
                columnData.ForEach(i => colHeaderLoopLayout(i));
            }

            /*
             * Processing Value Cells
             */

#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif

            if (!isLayoutChanged)
            {
                for (int col = 1; col < engineRowLayout.TableColumns.Count; col++)
                {
                    for (int cel = columnLevels; cel < engineRowLayout.TableColumns[col].Cells.Count; cel++)
                    {

                        PivotCellDescriptor valueDesc = engineRowLayout.TableColumns[col].Cells[cel];
                        valueDesc.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, false);
                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult grpResultLayout = null;
                        grpResultLayout = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (grpResultLayout == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, true);
                            grpResultLayout = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        }

                        if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineRowLayout, col, cel))
                        {
                            valueDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }

                        if (grpResultLayout == null)
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
                                var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();
                                grpResultLayout = TableBuilderHelper.GetValue(summary, headerInfo);
                            }
                        }

                        if (grpResultLayout != null)
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
                                            result = grpResultLayout.Items.AsQueryable().Average(sumInfo.Column).ToString();
                                            break;
                                        }
                                    case SummaryType.Count:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Count().ToString();
                                            break;
                                        };
                                    case SummaryType.Sum:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Sum(sumInfo.Column).ToString();
                                            break;
                                        };
                                    case SummaryType.Max:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Max(sumInfo.Column).ToString();
                                            break;
                                        };
                                    case SummaryType.Min:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Min(sumInfo.Column).ToString();
                                            break;
                                        }
#if !SILVERLIGHT
                                    case SummaryType.Expression:
                                        {
                                            object val = helper.ComputeSummary(sumInfo.Expression, grpResultLayout.Items);//.AsQueryable());
                                            if (val != null)
                                                result = val.ToString();
                                            break;
                                        };
#endif
                                        
                                    case SummaryType.String:
                                    case SummaryType.First:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(0).ToString();
                                            break;
                                        };
                                    case SummaryType.Last:
                                        {
                                            result = grpResultLayout.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(grpResultLayout.Items.AsQueryable().Count() - 1).ToString(); 
                                            break;
                                        };
                                        
                                }
                                if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                                {
                                    double val = 0;
                                    if (double.TryParse(result, out val))
                                        result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, val);
                                }

                                valueDesc.CellValue = result;
                                valueDesc.CellCaption = result;

                                PivotValueCellData cellData = new PivotValueCellData();
                                foreach (string  item in headerInfo.ColumnHeaderCaptions)
                                {
                                    cellData.Columns.Add(item);
                                    cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                }
                                foreach (string  item in headerInfo.RowHeaderCaptions)
                                {
                                    cellData.Rows.Add(item);
                                    cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item }); 
                                }
                                cellData.Measure = headerInfo.SummaryInfo.Key;
                                cellData.Value = valueDesc.CellValue;
                                valueDesc.CellData = cellData;
                                
                            }
                        }
                    }
                }
            }

            else
            {
                for (int col = 1; col < engineRowLayout.TableColumns.Count; col++)
                {
                    for (int cel = columnLevels - 1; cel < engineRowLayout.TableColumns[col].Cells.Count; cel++)
                    {
                        if (engineRowLayout.TableColumns[col].Cells[cel].CellCaption != "Span")
                        {
                            PivotCellDescriptor valueDesc = engineRowLayout.TableColumns[col].Cells[cel];
                            valueDesc.CellType = PivotCellDescriptorType.Value;
                            HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, false);
                            string result = string.Empty;
                            Syncfusion.Olap.Engine.Extension.GroupResult grpResultLayout = null;
                            grpResultLayout = TableBuilderHelper.GetValue(summaryData, headerInfo);

                            if (grpResultLayout == null)
                            {
                                headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, true);
                                grpResultLayout = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                            }

                            if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineRowLayout, col, cel))
                            {
                                valueDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                            }

                            if (grpResultLayout == null)
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
                                    var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();
                                    grpResultLayout = TableBuilderHelper.GetValue(summary, headerInfo);
                                }
                            }

                            if (grpResultLayout != null)
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
                                                result = grpResultLayout.Items.AsQueryable().Average(sumInfo.Column).ToString();
                                                break;
                                            }
                                        case SummaryType.Count:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Count().ToString();
                                                break;
                                            };
                                        case SummaryType.Sum:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Sum(sumInfo.Column).ToString();
                                                break;
                                            };
                                        case SummaryType.Max:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Max(sumInfo.Column).ToString();
                                                break;
                                            };
                                        case SummaryType.Min:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Min(sumInfo.Column).ToString();
                                                break;
                                            }
#if !SILVERLIGHT
                                        case SummaryType.Expression:
                                            {
                                                object val = helper.ComputeSummary(sumInfo.Expression, grpResultLayout.Items);//.AsQueryable());
                                                if (val != null)
                                                    result = val.ToString();
                                                break;
                                            };                                      
#endif
                                        case SummaryType.String:
                                        case SummaryType.First:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(0).ToString();
                                                break;
                                            };
                                        case SummaryType.Last:
                                            {
                                                result = grpResultLayout.Items.AsQueryable().Select(sumInfo.Column).ElementAtOrDefault(grpResultLayout.Items.AsQueryable().Count() - 1).ToString();
                                                break;
                                            };
                                         
                                    }

                                    if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                                    {
                                        double val = 0;
                                        if (double.TryParse(result, out val))
                                            result = string.Format(CultureInfo.CurrentCulture, sumInfo.FormatString, val);
                                    }
                                    valueDesc.CellValue = result;
                                    valueDesc.CellCaption = result;
                                    valueDesc.CellType = PivotCellDescriptorType.Value;

                                    PivotValueCellData cellData = new PivotValueCellData();
                                    foreach (string item in headerInfo.ColumnHeaderCaptions)
                                    {
                                        cellData.Columns.Add(item);
                                        cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                    }
                                    foreach (string item in headerInfo.RowHeaderCaptions)
                                    {
                                        cellData.Rows.Add(item);
                                        cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                    }
                                    cellData.Measure = headerInfo.SummaryInfo.Key;
                                    cellData.Value = valueDesc.CellValue;
                                    valueDesc.CellData = cellData;
                                }
                            }
                        }
                    }
                }
            }

            engineRowLayout.ClearLevelHeadersArea();
            engineRowLayout.RecalculateColumnHeaderSpans(true);
            return engineRowLayout;
        }

        internal static PivotEngine ProcessColumnMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IQueryable queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged)
        {
            int count = 0,
                           rowCount = 0,
                           columnCount = 0,
                           rowLevels = 0,
                           columnLevels = 0,
                           subGroup = 0,
                           temp_subGroup = 1;

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

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin1 = null;
            List<int> Index = new List<int>();
            levelLoopin1 = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin1);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    temp_subGroup += 1;
                    g.SubGroups.ForEach(levelLoopin);
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                    temp_subGroup = 1;
                }
                else
                {
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                }
            };

            //// resetting the subGroup for row processing
            // subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            rowLevels = subGroup;
            //// resetting the subGroup for column processing
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin1(i));
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
            }

            #endregion

            if (rowData.Count == 0)
            {
                rowCount = summaryInfos.Length;
            }
            if (columnData.Count == 0)
            {
                columnLevels = 1;
                columnCount = 1;
            }

            //Engine Creation
            PivotEngine engineColLayout = PivotEngine.CreateEngine(rowCount + columnLevels, 1 + (columnCount * summaryInfos.Length));

            //Specifying the ColumnHeaderSection
            engineColLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, columnLevels, columnCount * summaryInfos.Length);

            //Specifying the RowHeaderSection
            engineColLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, engineColLayout.RowsCount - subGroup, 1);

            //Preserving the DataSource
            engineColLayout.ItemSource = queryableSource;

            
            /*
             * Processing Row Headers
             */

            List<PivotCellDescriptor> parentList = new List<PivotCellDescriptor>();
            List<string> parent = new List<string>();

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopColLayout = null;

            int cRowLayout = 0;
            if (summaryInfos.Length == 1)
            {
                cRowLayout = subGroup - 1;
            }
            else
                cRowLayout = subGroup;

            int cColumnLayout = 0;

            rowHeaderLoopColLayout = (g) =>
            {
                cRowLayout++;
                PivotCellDescriptor celLayout = engineColLayout[cRowLayout, cColumnLayout];
                celLayout.CellValue = g.Key.ToString();
                celLayout.CellCaption = g.Key.ToString();
                celLayout.Value = g.Key.ToString();
                celLayout.Level = g.Level + 1;
                celLayout.CellType = PivotCellDescriptorType.RowHeader;
                celLayout.Tag = g.Key;
                celLayout.UniqueName = rowGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(celLayout, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    celLayout.HasChildren = true;
                    celLayout.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(celLayout.UniqueName + "." + celLayout.CellValue);
                }

                if (parentList.Count > 0)
                {
                    foreach (PivotCellDescriptor item in parentList)
                    {
                        celLayout.ParentCellDescriptors.Add(item);
                    }
                }

                if (g.ElementsCount == 0)
                {
                    celLayout.IsLastLevel = true;
                    parentList.Remove(celLayout);
                }

                else
                {
                    parentList.Add(celLayout);
                    g.SubGroups.ForEach(i => rowHeaderLoopColLayout(i));
                    parentList.Remove(celLayout);
                }

            };

            parentList = new List<PivotCellDescriptor>();

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopColLayout(i));
            }         

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopColLayout = null;
            cColumnLayout = 0;
            cRowLayout = 0;
            columnHeaderLoopColLayout = (g) =>
            {
                cColumnLayout++;
                PivotCellDescriptor cell = engineColLayout[cRowLayout, cColumnLayout];
                cell.CellValue = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.ColumnHeader;
                cell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, (g.ElementsCount + 1) * summaryInfos.Length);
                cell.Level = g.Level + 1;

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
                for (int i = cColumnLayout; i < cell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineColLayout[cRowLayout, i + 1];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;                    
                }
                #endregion

                if (!isLayoutChanged)
                {
                    if (g.ElementsCount > 0)
                    {
                        PivotCellDescriptor spanCell = engineColLayout[cRowLayout + 1, cColumnLayout + g.ElementsCount];
                        spanCell.CellValue = g.Key.ToString() + " Total";
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        if (summaryInfos.Length > 0)
                        {
                            spanCell.Tag = summaryInfos[0];
                        }

                        spanCell.Range = GridRangeInfo.FromTlhw(cRowLayout + 1, cColumnLayout + g.ElementsCount, (columnLevels - g.Level) - 1, 1);

                        for (int row = spanCell.Range.Width; row < spanCell.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineColLayout[cRowLayout + row + 1, cColumnLayout + g.ElementsCount];
                            SpanCell.SpanCell = spanCell;
                            SpanCell.CellType = spanCell.CellType;
                        }

                    }
                }
                if (g.SubGroups != null)
                {
                    //// Child group insert starts form the sample column
                    cColumnLayout--;
                    //// Moving the child group to next row
                    cRowLayout++;
                    g.SubGroups.ForEach(i => columnHeaderLoopColLayout(i));
                    //// Re-setting the position
                    cRowLayout--; cColumnLayout++;

                    if (isLayoutChanged)
                    {

                        string value = g.Key.ToString();
                        int tempCount = 0;
                        int level = 0;
                        Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                        LevelCount = (l) =>
                        {
                            if (l.SubGroups != null)
                            {
                                l.SubGroups.ForEach(i => LevelCount(i));
                                tempCount++;
                            }
                            else
                            {
                                if (level != l.Level)
                                {
                                    tempCount++;
                                    level = l.Level;
                                }
                            }
                        };

                        if (g.SubGroups != null)
                        {
                            g.SubGroups.ForEach(i => LevelCount(i));
                        }

                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engineColLayout[cRowLayout + 1, cColumnLayout++];
                            summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                            summaryCell.Range = GridRangeInfo.FromTlhw(cRowLayout + 1, cColumnLayout - 1, (columnLevels - 1) - g.Level, 1);

                            for (int row = summaryCell.Range.Width; row < summaryCell.Range.Height; row++)
                            {
                                PivotCellDescriptor SpanCell = engineColLayout[cRowLayout + row + 1, cColumnLayout - 1];
                                SpanCell.SpanCell = summaryCell;
                                SpanCell.CellType = summaryCell.CellType;
                            }
                        }
                        cColumnLayout--;
                    }

                }
                else
                {
                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engineColLayout[cRowLayout + 1, cColumnLayout++];
                            summaryCell.CellValue = summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];                          
                        }
                        cColumnLayout--;
                    }
                }
            };

            if (columnData.Count > 0)
            {
                columnData.ForEach(i => columnHeaderLoopColLayout(i));
            }
            else
            {
                // Include the summaries in the column headers
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineColLayout[0, 1 + i];
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

#if !SILVERLIGHT
            ExpressionHelper helper = new ExpressionHelper();
#endif
            int baseCol = -1;
            for (int row = columnLevels; row < engineColLayout.RowsCount; row++)
            {
                for (int column = 1; column < engineColLayout.HeaderSection.Right + 1; column++)
                {
                    PivotCellDescriptor cell = engineColLayout[row, column];
                    if (cell.UniqueName == string.Empty)
                    {
                        cell.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsColumnSummaryLayout(engineColLayout, row, column, false);
                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                        groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (groupResult == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptionsColumnSummaryLayout(engineColLayout, row, column, true);
                            groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        }

                        if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineColLayout, column, row))
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
                                if (i < rowGroup.Length)
                                    summaryString.Add(rowGroup[i]);
                            }

                            //// grouping the values with header information
                            var summary = queryableSource.GroupBy(summaryString.ToArray()).ToList();

                            groupResult = TableBuilderHelper.GetValue(summary, headerInfo);                            
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
                                                            engineColLayout.InsertAdditionalRows(groupResult.Items, summaryInfo.Column, column, row, summaryInfos, GridLayout.Normal, false);
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

                        PivotValueCellData cellData = new PivotValueCellData();
                        foreach (string item in headerInfo.ColumnHeaderCaptions)
                        {
                            cellData.Columns.Add(item);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                        }
                        foreach (string item in headerInfo.RowHeaderCaptions)
                        {
                            cellData.Rows.Add(item);
                            cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                        }
                        if (headerInfo.SummaryInfo != null)
                        {
                            cellData.Measure = headerInfo.SummaryInfo.Key;
                        }
                        cellData.Value = cell.CellValue;
                        cell.CellData = cellData;
                       

                    }
                }
            }

            engineColLayout.ClearLevelHeadersArea();
            engineColLayout.RecalculateColumnHeaderSpans(true);
            return engineColLayout;

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
        internal static PivotEngine ProcessRowMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IListSource listSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged,int summaryStringCount)
        {
          int count = 0,
          rowCount = 0,
          columnCount = 0,
          rowLevels = 0,
          columnLevels = 0,
          subGroup = 0,
          temp_subGroup = 1;

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

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin1 = null;
            List<int> Index = new List<int>();
            levelLoopin1 = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin1);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    temp_subGroup += 1;
                    g.SubGroups.ForEach(levelLoopin);
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                    temp_subGroup = 1;
                }
                else
                {
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                }
            };

            //// resetting the subGroup for row processing
            // subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            rowLevels = subGroup;
            //// resetting the subGroup for column processing
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin1(i));
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

            PivotEngine engineRowLayout = null;
            int tempCount = 0;

            if (rowData.Count == 0)
            {
                if (summaryInfos.Length == 0)
                    rowCount = summaryInfos.Length;
                else
                    tempCount = summaryInfos.Length;
            }
            else if (columnData.Count == 0)
            {
                columnCount = 1;
            }

            if (summaryInfos.Length == 1)
            {
                //Engine Creation
                engineRowLayout = PivotEngine.CreateEngine(columnLevels + rowCount, (columnCount + 1));

                //Specifying the Row Header Section
                engineRowLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, (engineRowLayout.RowsCount) - summaryInfos.Length, 1);

                //Specifying the Column Header Section
                engineRowLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, subGroup, columnCount);
            }

            else
            {
                //Engine Creation
                engineRowLayout = PivotEngine.CreateEngine(subGroup + ((rowCount * summaryInfos.Length) + rowCount + tempCount), (columnCount + 1));

                //Specifying the Row Header Section
                engineRowLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels - 1, 0, (engineRowLayout.RowsCount) - subGroup, 1);

                //Specifying the Column Header Section
                engineRowLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, subGroup, columnCount);
            }

            //Preserving the DataSource
            engineRowLayout.ItemSource = listSource;

            System.Collections.ArrayList parentList = new System.Collections.ArrayList();

            /*
             * Processing Row Headers 
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopLayout = null;
            int cRowLayout = subGroup - 1;
            int cColumnLayout = 0;
            List<string> parent = new List<string>();

            rowHeaderLoopLayout = (g) =>
            {
                cRowLayout++;
                PivotCellDescriptor celLayout = engineRowLayout[cRowLayout, cColumnLayout];
                celLayout.CellValue = g.Key.ToString();
                celLayout.CellCaption = g.Key.ToString();
                celLayout.Value = g.Key.ToString();
                celLayout.Level = g.Level + 1;
                celLayout.CellType = PivotCellDescriptorType.RowHeader;

                celLayout.UniqueName = rowGroup[g.Level];
                if (!isLayoutChanged)
                {
                    celLayout.Tag = summaryInfos[0];
                }

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent =TableBuilderHelper.FillParentCells(celLayout, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    celLayout.HasChildren = true;
                    celLayout.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(celLayout.UniqueName + "." + celLayout.CellValue);
                }
               
                if (g.ElementsCount == 0)
                {
                    celLayout.IsLastLevel = true;
                    if (!isLayoutChanged)
                    {
                        parentList.Add(celLayout);

                        PivotCellDescriptor measureCell = engineRowLayout[cRowLayout, cColumnLayout];
                        foreach (PivotCellDescriptor item in parentList)
                        {
                            measureCell.ParentCellDescriptors.Add(item);
                        }
                        parentList.Remove(celLayout);
                    }

                    if (isLayoutChanged)
                    {
                        parentList.Add(celLayout);
                    }

                    if (isLayoutChanged)
                    {
                        PivotCellDescriptor cellSpan = engineRowLayout[cRowLayout, cColumnLayout + 1];
                        cellSpan.CellCaption = "Span";

                        cellSpan.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout + 1, 1, engineRowLayout.TableColumns.Count - 1);

                        for (int row = cellSpan.Range.Height; row < cellSpan.Range.Width; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout, cColumnLayout + row + 1];
                            SpanCell.SpanCell = cellSpan;
                            SpanCell.CellCaption = "Span";
                        }
                    }

                    if (isLayoutChanged)
                    {
                        for (int summaryCount = 0; summaryCount < summaryInfos.Length; summaryCount++)
                        {
                            cRowLayout++;
                            PivotCellDescriptor measureCell = engineRowLayout[cRowLayout, cColumnLayout];
                            measureCell.CellValue = summaryInfos[summaryCount].Key;
                            measureCell.CellCaption = summaryInfos[summaryCount].Key;
                            measureCell.Value = summaryInfos[summaryCount].Key;
                            measureCell.Level = g.Level + 2;
                            measureCell.IsLastLevel = true;

                            foreach (PivotCellDescriptor item in parentList)
                            {
                                measureCell.ParentCellDescriptors.Add(item);
                            }
                            measureCell.CellType = PivotCellDescriptorType.RowHeader;
                            measureCell.Tag = summaryInfos[summaryCount];
                        }
                        parentList.Remove(celLayout);
                    }
                }

                else
                {
                    parentList.Add(celLayout);

                    if (isLayoutChanged)
                    {
                        PivotCellDescriptor cellSpan = engineRowLayout[cRowLayout, cColumnLayout + 1];
                        cellSpan.CellCaption = "Span";
                        cellSpan.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout + 1, 1, engineRowLayout.TableColumns.Count - 1);

                        for (int row = cellSpan.Range.Height; row < cellSpan.Range.Width; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout, cColumnLayout + row + 1];
                            SpanCell.SpanCell = cellSpan;
                            SpanCell.CellCaption = "Span";
                        }
                    }

                    g.SubGroups.ForEach(i => rowHeaderLoopLayout(i));

                    parentList.Remove(celLayout);

                    for (int sumCount = 0; sumCount < summaryInfos.Length; sumCount++)
                    {
                        if (isLayoutChanged)
                        {
                            cRowLayout++;
                            PivotCellDescriptor cellDescSummary = engineRowLayout[cRowLayout, cColumnLayout];
                            cellDescSummary.CellValue = g.Key.ToString() + " " + summaryInfos[sumCount].Key;
                            cellDescSummary.Level = g.Level + 1;
                            cellDescSummary.CellCaption = summaryInfos[sumCount].Key;
                            cellDescSummary.Value = summaryInfos[sumCount].Key;
                            cellDescSummary.CellType = PivotCellDescriptorType.SummaryRow;
                            cellDescSummary.Tag = summaryInfos[sumCount];
                            
                            foreach (PivotCellDescriptor item in parentList)
                            {
                                if (isLayoutChanged)
                                {
                                    cellDescSummary.ParentCellDescriptors.Add(item);
                                }
                            }

                            PivotCellDescriptor temp = new PivotCellDescriptor();
                            temp.CellValue = g.Key.ToString();
                            temp.CellCaption = g.Key.ToString();
                            temp.Value = g.Key.ToString();
                            temp.Level = g.Level + 2;
                            temp.CellType = PivotCellDescriptorType.RowHeader;
                            cellDescSummary.ParentCellDescriptors.Add(temp);
                        }
                    }
                }
            };

            parentList = new System.Collections.ArrayList();

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopLayout(i));
            }
            else
            {
                int temp = 0;
                if (isLayoutChanged)
                    temp = 1;
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineRowLayout[(columnLevels-temp) + i, 0];
                    summaryCell.CellValue = summaryInfos[i].Key;
                    summaryCell.Value = summaryInfos[i].Key;
                    summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                    summaryCell.CellCaption = summaryInfos[i].Key;
                    summaryCell.Tag = summaryInfos[i];
                    summaryCell.IsLastLevel = true;
                }
            }            

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> colHeaderLoopLayout = null;
            cColumnLayout = 0;
            cRowLayout = 0;
            colHeaderLoopLayout = (g) =>
            {
                cColumnLayout++;
                PivotCellDescriptor colCell = engineRowLayout[cRowLayout, cColumnLayout];
                colCell.CellValue = g.Key.ToString();
                colCell.CellCaption = g.Key.ToString();
                colCell.Value = g.Key.ToString();
                colCell.CellType = PivotCellDescriptorType.ColumnHeader;
                colCell.Level = g.Level + 1;
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
               
                if (g.ElementsCount == 1)
                {
                    colCell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, 0);
                }
                else
                    colCell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, (g.ElementsCount));

                for (int i = cColumnLayout; i < colCell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineRowLayout[cRowLayout, i + 1];
                    spanCell.SpanCell = colCell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;
                }

                if (g.ElementsCount > 0)
                {
                    cColumnLayout--;
                    //// Moving the child group to next row
                    cRowLayout++;
                    g.SubGroups.ForEach(i => colHeaderLoopLayout(i));
                    //// Re-setting the position
                    cRowLayout--; cColumnLayout++;

                    if (g.SubGroups != null)
                    {
                        PivotCellDescriptor cellDesc = engineRowLayout[cRowLayout, cColumnLayout];
                        cellDesc.CellValue = g.Key.ToString() + " Total";
                        cellDesc.CellCaption = g.Key.ToString();
                        cellDesc.Value = g.Key.ToString();
                        cellDesc.CellType = PivotCellDescriptorType.SummaryColumn;

                        if (!isLayoutChanged)
                        {
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, (columnLevels - g.Level), 1);
                        }
                        else
                            cellDesc.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, (columnLevels - 1) - (g.Level), 1);
                     
                        for (int row = cellDesc.Range.Width; row < cellDesc.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineRowLayout[cRowLayout + row, cColumnLayout];
                            SpanCell.SpanCell = cellDesc;
                            SpanCell.CellType = cellDesc.CellType;
                        }
                    }
                }
            };

            if (columnData != null)
            {
                columnData.ForEach(i => colHeaderLoopLayout(i));
            }

            /*
             * Processing Value Cells
             */


            if (!isLayoutChanged)
            {
                for (int col = 1; col < engineRowLayout.TableColumns.Count; col++)
                {
                    for (int cel = columnLevels; cel < engineRowLayout.TableColumns[col].Cells.Count; cel++)
                    {
                        PivotCellDescriptor valueDesc = engineRowLayout.TableColumns[col].Cells[cel];
                        valueDesc.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, false);
                        if (headerInfo.RowHeaderCaptions.Count == 0)
                        {
                            headerInfo = new HeaderInfo();
                        }

                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult grpResultLayout = null;
                        grpResultLayout = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (grpResultLayout == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, true);
                            if (headerInfo.RowHeaderCaptions.Count == 0)
                            {
                                headerInfo = new HeaderInfo();
                            }
                            grpResultLayout = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        }

                        if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineRowLayout, col, cel))
                        {
                            valueDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                        }

                        if (grpResultLayout == null)
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
                                var summary = (listSource as DataTable).GroupBy(summaryString.ToArray()).ToList();
                                grpResultLayout = TableBuilderHelper.GetValue(summary, headerInfo);
                            }
                        }

                        if (grpResultLayout != null)
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
                                            result = DataRowHelper.ComputeAvg(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        }
                                    case SummaryType.Count:
                                        {
                                            result = DataRowHelper.ComputeCount(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Sum:
                                        {
                                            result = DataRowHelper.ComputeSum(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Max:
                                        {
                                            result = DataRowHelper.ComputeMax(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Min:
                                        {
                                            result = DataRowHelper.ComputeMin(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Expression:
                                        {
                                            result = "Expression Not spported";
                                            break;
                                        }

                                    case SummaryType.String:
                                        {
                                            result = DataRowHelper.Select(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.First:
                                        {
                                            result = DataRowHelper.ComputeFirst(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };
                                    case SummaryType.Last:
                                        {
                                            result = DataRowHelper.ComputeLast(grpResultLayout.Items, sumInfo.Column);
                                            break;
                                        };  
                                      
                                }
                                if (sumInfo.FormatString != null && sumInfo.FormatString != string.Empty)
                                {
                                    string[] formatString = new string[] { "d", "t", "m", "y", "hh", "ss" };
                                    double val = 0,dtVal=0;
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

                                valueDesc.CellValue = result;
                                valueDesc.CellCaption = result;
                                valueDesc.CellType = PivotCellDescriptorType.Value;

                                PivotValueCellData cellData = new PivotValueCellData();
                                foreach (string item in headerInfo.ColumnHeaderCaptions)
                                {
                                    cellData.Columns.Add(item);
                                    cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                }
                                foreach (string item in headerInfo.RowHeaderCaptions)
                                {
                                    cellData.Rows.Add(item);
                                    cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                }
                                cellData.Measure = headerInfo.SummaryInfo.Key;
                                cellData.Value = valueDesc.CellValue;
                                valueDesc.CellData = cellData;

                            }
                        }
                    }
                }
            }

            else
            {
                for (int col = 1; col < engineRowLayout.TableColumns.Count; col++)
                {
                    for (int cel = columnLevels - 1; cel < engineRowLayout.TableColumns[col].Cells.Count; cel++)
                    {
                        if (engineRowLayout.TableColumns[col].Cells[cel].CellCaption != "Span")
                        {
                            PivotCellDescriptor valueDesc = engineRowLayout.TableColumns[col].Cells[cel];
                            valueDesc.CellType = PivotCellDescriptorType.Value;
                            HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, false);
                            string result = string.Empty;
                            Syncfusion.Olap.Engine.Extension.GroupResult grpResultLayout = null;
                            grpResultLayout = TableBuilderHelper.GetValue(summaryData, headerInfo);

                            if (grpResultLayout == null)
                            {
                                headerInfo = TableBuilderHelper.GetHeaderCaptionsRowSummaryLayout(engineRowLayout, cel, col, true);
                                grpResultLayout = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                            }

                            if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineRowLayout, col, cel))
                            {
                                valueDesc.CellExTypes.Add(PivotCellDescriptorType.SummaryRow.ToString());
                            }

                            if (grpResultLayout == null)
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
                                    var summary = (listSource as DataTable).GroupBy(summaryString.ToArray()).ToList();
                                    grpResultLayout = TableBuilderHelper.GetValue(summary, headerInfo);
                                }
                            }

                            if (grpResultLayout != null)
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
                                                result = DataRowHelper.ComputeAvg(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            }
                                        case SummaryType.Count:
                                            {
                                                result = DataRowHelper.ComputeCount(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Sum:
                                            {
                                                result = DataRowHelper.ComputeSum(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Max:
                                            {
                                                result = DataRowHelper.ComputeMax(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Min:
                                            {
                                                result = DataRowHelper.ComputeMin(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Expression:
                                            {
                                                result = "Expression Not spported";
                                                break;
                                            }
                                        case SummaryType.String:
                                            {
                                                result = DataRowHelper.Select(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.First:
                                            {
                                                result = DataRowHelper.ComputeFirst(grpResultLayout.Items, sumInfo.Column);
                                                break;
                                            };
                                        case SummaryType.Last:
                                            {
                                                result = DataRowHelper.ComputeLast(grpResultLayout.Items, sumInfo.Column);
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

                                    valueDesc.CellValue = result;
                                    valueDesc.CellCaption = result;
                                    valueDesc.CellType = PivotCellDescriptorType.Value;
                                    PivotValueCellData cellData = new PivotValueCellData();
                                    foreach (string item in headerInfo.ColumnHeaderCaptions)
                                    {
                                        cellData.Columns.Add(item);
                                        cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                    }
                                    foreach (string item in headerInfo.RowHeaderCaptions)
                                    {
                                        cellData.Rows.Add(item);
                                        cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                                    }
                                    cellData.Measure = headerInfo.SummaryInfo.Key;
                                    cellData.Value = valueDesc.CellValue;
                                    valueDesc.CellData = cellData;
                                }
                            }
                        }
                    }
                }
            }

            engineRowLayout.ClearLevelHeadersArea();
            engineRowLayout.RecalculateColumnHeaderSpans(true);
            if (summaryStringCount > 0)
            {
                engineRowLayout.RecalculateSpans(summaryStringCount,summaryInfos.Count(),true,GridLayout.ExcelLikeLayout);  
            }
            return engineRowLayout;
        }

        internal static PivotEngine ProcessColumnMeasure(List<Syncfusion.Olap.Engine.Extension.GroupResult> rowData, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnData, SummaryInfo[] summaryInfos, List<Syncfusion.Olap.Engine.Extension.GroupResult> summaryData, IListSource queryableSource, string[] rowGroup, string[] columnGroup, List<Syncfusion.Olap.Engine.Extension.GroupResult> columnSummaryData, bool isLayoutChanged,int summaryStringCount)
        {
            int count = 0,
                          rowCount = 0,
                          columnCount = 0,
                          rowLevels = 0,
                          columnLevels = 0,
                          subGroup = 0,
                          temp_subGroup = 1;

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

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin1 = null;
            List<int> Index = new List<int>();
            levelLoopin1 = (g) =>
            {
                if (g.SubGroups != null)
                {
                    Index.Add(g.Level);
                    g.SubGroups.ForEach(levelLoopin1);
                    Index.Add(g.Level);
                }
                else
                {
                    Index.Add(g.Level);
                }
            };

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> levelLoopin = null;

            levelLoopin = (g) =>
            {
                if (g.SubGroups != null)
                {
                    temp_subGroup += 1;
                    g.SubGroups.ForEach(levelLoopin);
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                    temp_subGroup = 1;
                }
                else
                {
                    if (temp_subGroup > subGroup)
                        subGroup = temp_subGroup;
                }
            };

            //// resetting the subGroup for row processing
            // subGroup = 0;
            rowData.ForEach(i => levelLoopin(i));
            //// updating the subgroup with the processed value
            rowLevels = subGroup;
            //// resetting the subGroup for column processing
            Index = new List<int>();
            if (columnData.Count > 0)
            {
                columnData.ForEach(i => levelLoopin1(i));
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

            if(!isLayoutChanged)
            {
                subGroup = columnLevels;
            }
            else
            {
                subGroup = columnLevels - 1;
            }

            #endregion

            if (rowData.Count == 0)
            {
                rowCount = summaryInfos.Length;
            }
            if (columnData.Count == 0)
            {
                columnLevels = 1;
                columnCount = 1;
            }  

            //Engine Creation
            PivotEngine engineColLayout = PivotEngine.CreateEngine(rowCount + columnLevels, 1 + (columnCount * summaryInfos.Length));

            //Specifying the ColumnHeaderSection
            engineColLayout.HeaderSection = GridRangeInfo.FromTlhw(0, 1, columnLevels, columnCount * summaryInfos.Length);

            //Specifying the RowHeaderSection
            engineColLayout.RowHeaderSection = GridRangeInfo.FromTlhw(columnLevels, 0, engineColLayout.RowsCount - subGroup, 1);

            //Preserving the DataSource
            engineColLayout.ItemSource = queryableSource;
                       

            /*
             * Processing Row Headers
             */


            System.Collections.ArrayList parentList = new System.Collections.ArrayList();
            List<string> parent = new List<string>();

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> rowHeaderLoopColLayout = null;

            int cRowLayout = 0;
            if (summaryInfos.Length == 1)
            {
                cRowLayout = subGroup - 1;
            }
            else
                cRowLayout = subGroup;

            int cColumnLayout = 0;

            rowHeaderLoopColLayout = (g) =>
            {
                cRowLayout++;
                PivotCellDescriptor celLayout = engineColLayout[cRowLayout, cColumnLayout];
                celLayout.CellValue = g.Key.ToString();
                celLayout.CellCaption = g.Key.ToString();
                celLayout.Value = g.Key.ToString();
                celLayout.Level = g.Level + 1;
                celLayout.CellType = PivotCellDescriptorType.RowHeader;
                celLayout.Tag = g.Key;
                celLayout.UniqueName = rowGroup[g.Level];

                if (g.Level == 0)
                {
                    parent = new List<string>();
                }

                parent = TableBuilderHelper.FillParentCells(celLayout, parent);

                if ((g.HasChildren || g.SubGroups != null) && g.ExpandableState != ExpandableState.None)
                {
                    celLayout.HasChildren = true;
                    celLayout.ExpandableState = g.ExpandableState;
                }

                if (g.SubGroups != null)
                {
                    parent.Add(celLayout.UniqueName + "." + celLayout.CellValue);
                }

                if (parentList.Count > 0)
                {
                    foreach (PivotCellDescriptor item in parentList)
                    {
                        celLayout.ParentCellDescriptors.Add(item);
                    }
                }

                if (g.ElementsCount == 0)
                {
                    celLayout.IsLastLevel = true;
                    parentList.Remove(celLayout);
                }

                else
                {
                    parentList.Add(celLayout);
                    g.SubGroups.ForEach(i => rowHeaderLoopColLayout(i));
                    parentList.Remove(celLayout);
                }
            };

            parentList = new System.Collections.ArrayList();

            if (rowData.Count > 0)
            {
                rowData.ForEach(i => rowHeaderLoopColLayout(i));
            }          

            /*
             * Processing Column Headers
             */

            Action<Syncfusion.Olap.Engine.Extension.GroupResult> columnHeaderLoopColLayout = null;
            cColumnLayout = 0;
            cRowLayout = 0;
            columnHeaderLoopColLayout = (g) =>
            {
                cColumnLayout++;
                PivotCellDescriptor cell = engineColLayout[cRowLayout, cColumnLayout];
                cell.CellValue = g.Key.ToString();
                cell.Value = g.Key.ToString();
                cell.CellCaption = g.Key.ToString();
                cell.CellType = PivotCellDescriptorType.ColumnHeader;
                cell.Range = GridRangeInfo.FromTlhw(cRowLayout, cColumnLayout, 1, (g.ElementsCount + 1) * summaryInfos.Length);
                cell.Level = g.Level + 1;

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
                for (int i = cColumnLayout; i < cell.Range.Right; i++)
                {
                    PivotCellDescriptor spanCell = engineColLayout[cRowLayout, i + 1];
                    spanCell.SpanCell = cell;
                    spanCell.CellValue = g.Key.ToString();
                    spanCell.Value = g.Key.ToString();
                    spanCell.CellCaption = g.Key.ToString();
                    spanCell.CellType = PivotCellDescriptorType.ColumnHeader;                   
                }
                #endregion

                if (!isLayoutChanged)
                {
                    if (g.ElementsCount > 0)
                    {
                        PivotCellDescriptor spanCell = engineColLayout[cRowLayout + 1, cColumnLayout + g.ElementsCount];
                        spanCell.CellValue = g.Key.ToString() + " Total";
                        spanCell.CellCaption = g.Key.ToString();
                        spanCell.Value = g.Key.ToString();
                        spanCell.CellType = PivotCellDescriptorType.SummaryColumn;
                        if (summaryInfos.Length > 0)
                        {
                            spanCell.Tag = summaryInfos[0];
                        }

                        spanCell.Range = GridRangeInfo.FromTlhw(cRowLayout + 1, cColumnLayout + g.ElementsCount, (columnLevels - g.Level) - 1, 1);

                        for (int row = spanCell.Range.Width; row < spanCell.Range.Height; row++)
                        {
                            PivotCellDescriptor SpanCell = engineColLayout[cRowLayout + row + 1, cColumnLayout + g.ElementsCount];
                            SpanCell.SpanCell = spanCell;
                            SpanCell.CellType = spanCell.CellType;
                        }
                    }
                }
                if (g.SubGroups != null)
                {
                    //// Child group insert starts form the sample column
                    cColumnLayout--;
                    //// Moving the child group to next row
                    cRowLayout++;
                    g.SubGroups.ForEach(i => columnHeaderLoopColLayout(i));
                    //// Re-setting the position
                    cRowLayout--; cColumnLayout++;

                    if (isLayoutChanged)
                    {
                        string value = g.Key.ToString();
                        int tempCount = 0;
                        int level = 0;
                        
                        Action<Syncfusion.Olap.Engine.Extension.GroupResult> LevelCount = null;

                        LevelCount = (l) =>
                        {
                            if (l.SubGroups != null)
                            {

                                l.SubGroups.ForEach(i => LevelCount(i));
                                tempCount++;

                            }
                            else
                            {
                                if (level != l.Level)
                                {
                                    tempCount++;
                                    level = l.Level;
                                }
                            }
                        };

                        if (g.SubGroups != null)
                        {
                            g.SubGroups.ForEach(i => LevelCount(i));

                        }

                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engineColLayout[cRowLayout + 1, cColumnLayout++];
                            summaryCell.CellValue = g.Key.ToString() + " " + summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.SummaryColumn;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];
                            summaryCell.Range = GridRangeInfo.FromTlhw(cRowLayout + 1, cColumnLayout - 1, (columnLevels - 1) - g.Level, 1);

                            for (int row = summaryCell.Range.Width; row < summaryCell.Range.Height; row++)
                            {
                                PivotCellDescriptor SpanCell = engineColLayout[cRowLayout + row + 1, cColumnLayout - 1];
                                SpanCell.SpanCell = summaryCell;
                                SpanCell.CellType = summaryCell.CellType;
                            }
                        }
                        cColumnLayout--;
                    }
                }
                else
                {
                    if (isLayoutChanged)
                    {
                        for (int i = 0; i < summaryInfos.Length; i++)
                        {
                            PivotCellDescriptor summaryCell = engineColLayout[cRowLayout + 1, cColumnLayout++];
                            summaryCell.CellValue = summaryInfos[i].Key;
                            summaryCell.Value = summaryInfos[i].Key;
                            summaryCell.CellType = PivotCellDescriptorType.ColumnHeader;
                            summaryCell.CellCaption = summaryInfos[i].Key;
                            summaryCell.Tag = summaryInfos[i];                          
                        }
                        cColumnLayout--;
                    }
                }
            };

            if (columnData.Count > 0)
            {
                columnData.ForEach(i => columnHeaderLoopColLayout(i));
            }
            else
            {
                // Include the summaries in the column headers
                for (int i = 0; i < summaryInfos.Length; i++)
                {
                    PivotCellDescriptor summaryCell = engineColLayout[0, 1 + i];
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

            int baseCol = -1;
            for (int row = columnLevels; row < engineColLayout.RowsCount; row++)
            {
                for (int column = 1; column < engineColLayout.HeaderSection.Right + 1; column++)
                {
                    PivotCellDescriptor cell = engineColLayout[row, column];
                    if (cell.UniqueName == string.Empty)
                    {
                        cell.CellType = PivotCellDescriptorType.Value;
                        HeaderInfo headerInfo = TableBuilderHelper.GetHeaderCaptionsColumnSummaryLayout(engineColLayout, row, column, false);
                        string result = string.Empty;
                        Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
                        groupResult = TableBuilderHelper.GetValue(summaryData, headerInfo);

                        if (groupResult == null)
                        {
                            headerInfo = TableBuilderHelper.GetHeaderCaptionsColumnSummaryLayout(engineColLayout, row, column, true);
                            groupResult = TableBuilderHelper.GetValue(columnSummaryData, headerInfo);
                        }

                        if (TableBuilderHelper.CheckIsSummaryRowforExcelLayout(engineColLayout, column, row))
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
                                if (i < rowGroup.Length)
                                    summaryString.Add(rowGroup[i]);
                            }

                            //// grouping the values with header information
                            var summary = (queryableSource as DataTable).GroupBy(summaryString.ToArray()).ToList();
                            groupResult = TableBuilderHelper.GetValue(summary, headerInfo);                          
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
                            //if (summaryInfo != null)
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
                                            //object o = helper.ComputeSummary(sumInfo.Expression, grpResult.Items);
                                            //if (null != o)
                                            //    result = o.ToString();
                                            break;
                                        }
                                    case SummaryType.String:
                                        {
                                            result = DataRowHelper.Select(groupResult.Items, summaryInfo.Column);
                                            if (columnData.Count == 0)
                                            {
                                                if (baseCol != row)
                                                {
                                                    if (groupResult.SubGroups == null)
                                                    {
                                                        result = DataRowHelper.StringType(groupResult.Items, summaryInfo.Column, engineColLayout, column, row, summaryInfos, GridLayout.Normal, true);
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
                                    else if (val != 0)
                                        result = string.Format(CultureInfo.CurrentCulture, summaryInfo.FormatString, DateTime.FromOADate(val));
                                }
                            }
                        }

                        cell.CellType = PivotCellDescriptorType.Value;
                        cell.CellValue = result;
                        cell.CellCaption = result;

                        PivotValueCellData cellData = new PivotValueCellData();
                        foreach (string item in headerInfo.ColumnHeaderCaptions)
                        {
                            cellData.Columns.Add(item);
                            cellData.ColumnInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                        }
                        foreach (string item in headerInfo.RowHeaderCaptions)
                        {
                            cellData.Rows.Add(item);
                            cellData.RowInfo.Add(new CellHeaderInfo { Name = item, UniqueName = item });
                        }
                        cellData.Measure = headerInfo.SummaryInfo.Key;
                        cellData.Value = cell.CellValue;
                        cell.CellData = cellData;

                    }
                }
            }

            engineColLayout.ClearLevelHeadersArea();
            engineColLayout.RecalculateColumnHeaderSpans(true);
            return engineColLayout;
        }
      
#endif
        #endregion
    }

}
