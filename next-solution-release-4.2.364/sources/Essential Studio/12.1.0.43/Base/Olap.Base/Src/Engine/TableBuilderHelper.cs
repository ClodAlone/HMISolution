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

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.Olap.Engine.Extension;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Engine
#endif

#if !SILVERLIGHT
using System.Data;
using Syncfusion.Olap.Reports;
namespace Syncfusion.Olap.Engine
#endif
{
    /// <summary>
    /// Helper class for table builder.
    /// </summary>
    public class TableBuilderHelper
    {
        /// <summary>
        /// Returns the sorted data of IEnumerable source based on the Sort Order.
        /// </summary>
        /// <param name="queryableSource">The IQueryable source.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        internal static IQueryable GetSortedData(IQueryable queryableSource, SortType sortOrder, string[] names)
        {
            var queryableObject = queryableSource;
            //// if the sort order is normal the return it without processing
            if (sortOrder == SortType.UnSpecified)
                return queryableObject;
            //// sorting the columns in reverse order to maintain it in proper order while grouping
            for (int i = names.Length - 1; i >= 0; i--)
            {
                Action<IQueryable> loopin = null;
                loopin = (o) =>
                {
                    if (sortOrder == SortType.Ascending)
                    {
                        queryableObject = o.OrderBy(names[i]);
                    }
                    else if (sortOrder == SortType.Descending)
                    {
                        queryableObject = o.OrderByDescending(names[i]);
                    }
                };

                loopin(queryableObject);
            }
            return queryableObject;
        }
#if !SILVERLIGHT

        /// <summary>
        /// Returns the sorted data of IEnumerable source based on the Sort Order.
        /// </summary>
        /// <param name="listSource">The list source.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        internal static IListSource GetSortedData(IListSource listSource, SortType sortOrder, string[] names)
        {
            var queryableObject = listSource;
            //// if the sort order is normal the return it without processing
            if (sortOrder == SortType.UnSpecified)
                return queryableObject;
            //// sorting the columns in reverse order to maintain it in proper order while grouping
            for (int i = names.Length - 1; i >= 0; i--)
            {
                Action<IListSource> loopin = null;
                loopin = (o) =>
                {
                    if (sortOrder == SortType.Ascending)
                    {

                        DataRow[] rows = (o as DataTable).Select("1=1", names[i]);
                        DataTable dataTable = (o as DataTable).Clone();
                        foreach (var item in rows)
                        {
                            dataTable.ImportRow(item);
                        }
                        queryableObject = dataTable;
                    }
                    else if (sortOrder == SortType.Descending)
                    {

                        DataRow[] rows = (o as DataTable).Select("1=1", names[i] + " DESC");
                        DataTable dataTable = (o as DataTable).Clone();
                        foreach (var item in rows)
                        {
                            dataTable.ImportRow(item);
                        }
                        queryableObject = dataTable;
                       
                    }
                };

                loopin(queryableObject);
            }
            return queryableObject;
        }

#endif

        /// <summary>
        /// Fills the parent cells.
        /// </summary>
        /// <param name="cel">The cel.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        internal static List<string> FillParentCells(PivotCellDescriptor cel, List<string> parent)
        {
            cel.ParentCellValues = new List<string>();
            if (parent.Count > 0)
            {
                for (int i = parent.Count - 1; i >= cel.Level - 1; i--)
                {
                    parent.RemoveAt(i);
                }
            }
            foreach (string item in parent)
            {
                cel.ParentCellValues.Add(item);
            }
            return parent;
        }

        /// <summary>
        /// Checks whether the row is a Summary
        /// </summary>
        /// <param name="engineRow">The engine row.</param>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        internal static bool CheckIsSummaryRow(PivotEngine engineRow, int column, int row)
        {
            PivotRowDescriptor rowDesc = engineRow.GetRowAt(row);

            foreach (PivotCellDescriptor cellDesc in rowDesc.Cells)
            {
                if (cellDesc.CellValue != null)
                {
                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn || cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        SummaryInfo sumInfo = cellDesc.Tag as SummaryInfo;
                        if (sumInfo != null)
                        {
                            if (sumInfo.Key == "Total" && cellDesc.CellValue == "Total")
                            {
                                break;
                            }
                        }
                        return true;
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.Value)
                        break;
                }
                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn || cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                {
                    return true;
                }
            }
            PivotColumnDescriptor colDesc = engineRow.TableColumns[column];

            foreach (PivotCellDescriptor cellDesc in colDesc.Cells)
            {
                if (cellDesc.CellValue != null)
                {
                    if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn || cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        SummaryInfo sumInfo = cellDesc.Tag as SummaryInfo;
                        if (sumInfo != null)
                        {
                            if (sumInfo.Key == "Total" && cellDesc.CellValue == "Total")
                            {
                                break;
                            }
                        }
                        return true;
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.Value)
                        break;
                }
                if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn || cellDesc.CellType== PivotCellDescriptorType.SummaryRow)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the value from GroupResult
        /// </summary>
        /// <param name="groupResult">The group result.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.GroupResult GetValue(List<Syncfusion.Olap.Engine.Extension.GroupResult> groupResult, string key)
        {
            foreach (var item in groupResult)
            {
                if (item.Key.ToString() == key)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Gets the value from GroupResult
        /// </summary>
        /// <param name="groupResultCollection">The group result collection.</param>
        /// <param name="headerInfo">The header info.</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.GroupResult GetValue(List<Syncfusion.Olap.Engine.Extension.GroupResult> groupResultCollection, Syncfusion.Olap.Engine.Extension.HeaderInfo headerInfo)
        {
            Syncfusion.Olap.Engine.Extension.GroupResult groupResult = null;
            List<Syncfusion.Olap.Engine.Extension.GroupResult> listOfGroupedResults = groupResultCollection;
            
            foreach (var item in headerInfo.HeaderCaptions)
            {
                groupResult = GetValue(listOfGroupedResults, item);
                if (groupResult != null && groupResult.SubGroups != null)
                    listOfGroupedResults = groupResult.SubGroups.ToList();
                else
                    break;
            }

            return groupResult;
        }

        /// <summary>
        /// Gets the header captions.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.HeaderInfo GetHeaderCaptions(PivotEngine engine, int rowIndex, int columnIndex, bool isRowSummary)
        {
            Syncfusion.Olap.Engine.Extension.HeaderInfo headerInfo = new Syncfusion.Olap.Engine.Extension.HeaderInfo();
            List<string> headerCaptions = new List<string>();
            if (!isRowSummary)
            {
                for (int i = 0; i < engine.RowHeaderSection.Right + 1; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];
                    if (cell.Value == string.Empty || cell.Value == null)
                    {
                        if (cell.Tag != null && cell.CellType != PivotCellDescriptorType.SummaryRow)
                        {
                            if (cell.Tag.ToString() != string.Empty)
                                break;
                        }
                        else if (cell.SpanCell != null)
                        {
                            if (cell.Tag != null)
                            {
                                if (cell.Tag.ToString() != string.Empty)
                                    break;
                            }
                            else
                                break;
                        }
                        else
                            if (cell.CellType != PivotCellDescriptorType.SummaryRow)
                                break;
                    }

                    if (cell.CellType != PivotCellDescriptorType.Value)
                    {
                        headerCaptions.Add(cell.Value);
                        headerInfo.RowHeaderCaptions.Add(cell.Value);
                    }
                }

                for (int i = 0; i < engine.HeaderSection.Bottom + 1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Tag is SummaryInfo)
                    {
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }
                    if (cell.CellType != PivotCellDescriptorType.Value)
                    {
                        headerCaptions.Add(cell.Value);
                        headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                    }
                }
            }
            else
            {
                for (int i = 0; i < engine.HeaderSection.Right+1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Value == string.Empty || cell.Value == null)
                    {
                        if (cell.Tag != null)
                        {
                            if (cell.Tag.ToString() != string.Empty)
                                break;
                        }
                        else
                            break;
                    }

                    if (cell.Tag is SummaryInfo)
                    {
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }
                    if (cell.CellType != PivotCellDescriptorType.Value)
                    {
                        headerCaptions.Add(cell.Value);
                        headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                    }
                }

                for (int i = 0; i < engine.RowHeaderSection.Width; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];
                    if (cell.Value == string.Empty || cell.Value == null)
                    {
                        if (cell.Tag != null && cell.CellType != PivotCellDescriptorType.SummaryRow)
                        {
                            if (cell.Tag.ToString() != string.Empty)
                                break;
                        }
                        else if (cell.SpanCell != null)
                        {
                            if (cell.Tag != null)
                            {
                                if (cell.Tag.ToString() != string.Empty)
                                    break;
                            }
                            else
                                break;
                        }
                        else
                            if (cell.CellType != PivotCellDescriptorType.SummaryRow)
                                break;
                    }
                    if (cell.CellType != PivotCellDescriptorType.Value)
                    {
                        headerCaptions.Add(cell.Value);
                        headerInfo.RowHeaderCaptions.Add(cell.Value);
                    }
                }
            }
            headerInfo.HeaderCaptions = headerCaptions;
            return headerInfo;
        }

        /// <summary>
        /// Gets the header captions for row summary.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.HeaderInfo GetHeaderCaptionsRowSummary(PivotEngine engine, int rowIndex, int columnIndex, bool isRowSummary)
        {
            Syncfusion.Olap.Engine.Extension.HeaderInfo headerInfo = new Syncfusion.Olap.Engine.Extension.HeaderInfo();
            List<string> headerCaptions = new List<string>();
            if (!isRowSummary)
            {
                for (int i = 0; i < engine.RowHeaderSection.Width + 1; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];
                    if (cell.Value == string.Empty || cell.Value == null)
                        break;

                    if (cell.Tag is SummaryInfo)
                    {
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }

                    headerCaptions.Add(cell.Value);
                    headerInfo.RowHeaderCaptions.Add(cell.Value);
                }

                for (int i = 0; i < engine.HeaderSection.Height; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Value == string.Empty || cell.Value == null)
                    {
                        break;
                    }
                    headerCaptions.Add(cell.Value);
                    headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                }
            }
            else
            {
                for (int i = 0; i < engine.HeaderSection.Height; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Value == string.Empty || cell.Value == null)
                        break;
                 
                        headerCaptions.Add(cell.Value);
                        headerInfo.ColumnHeaderCaptions.Add(cell.Value);                    
                }

                for (int i = 0; i < engine.RowHeaderSection.Width + 1; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];
                    if (cell.Value == string.Empty || cell.Value == null)
                        break;

                    if (cell.Tag is SummaryInfo)
                    {
                        if (cell.ParentCellValues != null)
                        {
                            headerInfo.RowHeaderCaptions.Add(cell.ParentCellValues[0]);
                            headerCaptions.Add(cell.ParentCellValues[0]);
                        }
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }

                    headerCaptions.Add(cell.Value);
                    headerInfo.RowHeaderCaptions.Add(cell.Value);
                }
            }
            headerInfo.HeaderCaptions = headerCaptions;
            return headerInfo;
        }

        /// <summary>
        /// Gets the header captions for column summary in ExcelLikeLayout.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.HeaderInfo GetHeaderCaptionsColumnSummaryLayout(PivotEngine engine, int rowIndex, int columnIndex, bool isRowSummary)
        {
            Syncfusion.Olap.Engine.Extension.HeaderInfo headerInfo = new Syncfusion.Olap.Engine.Extension.HeaderInfo();
            List<string> headerCaptions = new List<string>();
            if (!isRowSummary)
            {
                for (int i = 0; i < engine.RowHeaderSection.Left+1; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];

                    foreach (PivotCellDescriptor item in cell.ParentCellDescriptors)
                    {
                        headerCaptions.Add(item.Value);
                        headerInfo.RowHeaderCaptions.Add(item.Value);
                    }

                    headerCaptions.Add(cell.Value);
                    headerInfo.RowHeaderCaptions.Add(cell.Value);

                }

                for (int i = 0; i < engine.HeaderSection.Bottom+1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Tag is SummaryInfo)
                    {
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }
                    headerCaptions.Add(cell.Value);
                    headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                }
            }
            else
            {
                for (int i = 0; i < engine.HeaderSection.Bottom+1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];

                    if (cell.Value == string.Empty)
                    {
                        if (cell.Tag != null)
                        {
                            if (cell.Tag.ToString() != string.Empty)
                                break;
                        }
                        else
                            break;
                    }

                    if (cell.Tag is SummaryInfo)
                    {
                        headerInfo.SummaryInfo = cell.Tag as SummaryInfo;
                        break;
                    }
                    headerCaptions.Add(cell.Value);
                    headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                }

                for (int i = 0; i < engine.RowHeaderSection.Left+1; i++)
                {
                    PivotCellDescriptor cell = engine[rowIndex, i];

                    foreach (PivotCellDescriptor item in cell.ParentCellDescriptors)
                    {
                        headerCaptions.Add(item.Value);
                        headerInfo.RowHeaderCaptions.Add(item.Value);
                    }

                    headerCaptions.Add(cell.Value);
                    headerInfo.RowHeaderCaptions.Add(cell.Value);
                }
            }
            headerInfo.HeaderCaptions = headerCaptions;
            return headerInfo;
        }

        /// <summary>
        /// Gets the header captions for row summary in ExcelLikeLayout.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="isRowSummary">if set to <c>true</c> [is row summary].</param>
        /// <returns></returns>
        internal static Syncfusion.Olap.Engine.Extension.HeaderInfo GetHeaderCaptionsRowSummaryLayout(PivotEngine engine, int rowIndex, int columnIndex, bool isRowSummary)
        {
            Syncfusion.Olap.Engine.Extension.HeaderInfo headerInfo = new Syncfusion.Olap.Engine.Extension.HeaderInfo();
            List<string> headerCaptions = new List<string>();
            if (!isRowSummary)
            {
                PivotCellDescriptor cellDesc = engine[rowIndex, 0];

                if (cellDesc.ParentCellDescriptors.Count == 0)
                {
                    headerInfo.RowHeaderCaptions.Add(cellDesc.Value);
                    headerCaptions.Add(cellDesc.Value);
                   
                }
                else
                {
                    foreach (PivotCellDescriptor cell in cellDesc.ParentCellDescriptors)
                    {
                        headerInfo.RowHeaderCaptions.Add(cell.Value);
                        headerCaptions.Add(cell.Value);
                    }
                }

                if (cellDesc.Tag is SummaryInfo)
                {
                    headerInfo.SummaryInfo = cellDesc.Tag as SummaryInfo;
                }

                for (int i = 0; i < engine.HeaderSection.Bottom+1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    headerCaptions.Add(cell.Value);
                    headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                }
            }

            else
            {
                for (int i = 0; i < engine.HeaderSection.Bottom + 1; i++)
                {
                    PivotCellDescriptor cell = engine[i, columnIndex];
                    if (cell.Value == string.Empty || cell.Value ==null)
                        break;

                    headerCaptions.Add(cell.Value);
                    headerInfo.ColumnHeaderCaptions.Add(cell.Value);
                }

                PivotCellDescriptor cellDesc = engine[rowIndex, 0];

                if (cellDesc.ParentCellDescriptors.Count == 0)
                {
                    headerInfo.RowHeaderCaptions.Add(cellDesc.Value);
                    headerCaptions.Add(cellDesc.Value);                  
                }
                else
                {
                    foreach (PivotCellDescriptor cell in cellDesc.ParentCellDescriptors)
                    {
                        headerInfo.RowHeaderCaptions.Add(cell.Value);
                        headerCaptions.Add(cell.Value);
                    }
                }

                if (cellDesc.Tag is SummaryInfo)
                {
                    headerInfo.SummaryInfo = cellDesc.Tag as SummaryInfo;
                }
            }

            headerInfo.HeaderCaptions = headerCaptions;

            return headerInfo;
        }

        /// <summary>
        /// Checks whether the current cell is summary.
        /// </summary>
        /// <param name="engineRowLayout">The engine row layout.</param>
        /// <param name="col">The col.</param>
        /// <param name="cel">The cel.</param>
        /// <returns></returns>
        internal static bool CheckIsSummary(PivotEngine engineRowLayout, int col, int cel)
        {
            PivotColumnDescriptor colDesc = engineRowLayout.TableColumns[cel];

            for (int i = 0; i < col; i++)
            {
                PivotCellDescriptor cellDesc = colDesc.Cells[i];
                if (cellDesc.CellValue != null)
                {
                    if (cellDesc.CellValue.Contains("Total"))
                    {
                        SummaryInfo sumInfo = cellDesc.Tag as SummaryInfo;
                        if (sumInfo != null)
                        {
                            if (sumInfo.Key == "Total" && cellDesc.CellValue == "Total")
                            {
                                break;
                            }
                        }
                        return true;
                    }
                }
            }

            PivotRowDescriptor rowDesc = engineRowLayout.GetRowAt(col);

            for (int i = 0; i < cel; i++)
            {
                PivotCellDescriptor cellDesc = rowDesc.Cells[i];
                
                if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                {
                    return true ;
                }                
            }
            return false;
        }

        /// <summary>
        /// Checks whether the current cell is summary if layout is ExcelLike
        /// </summary>
        /// <param name="engineRowLayout">The engine row layout.</param>
        /// <param name="col">The col.</param>
        /// <param name="cel">The cel.</param>
        /// <returns></returns>
        internal static bool CheckIsSummaryRowforExcelLayout(PivotEngine engineRowLayout, int col, int cel)
        {
            PivotCellDescriptor cellDesc = engineRowLayout[cel, 0];

            if (!cellDesc.IsLastLevel || cellDesc.CellExTypes.Contains("SummaryRow"))
            {
                return true;
            }

            PivotColumnDescriptor colDesc = engineRowLayout.TableColumns[col];
            foreach (PivotCellDescriptor cellDes in colDesc.Cells)
            {
                if (cellDes.CellValue != null)
                {
                    if (cellDes.CellValue.Contains("Total"))
                    {
                        SummaryInfo sumInfo = cellDes.Tag as SummaryInfo;
                        if (sumInfo != null)
                        {
                            if (sumInfo.Key == "Total" && cellDes.CellValue == "Total")
                            {
                                break;
                            }
                        }
                        return true;
                    }
                }
                if(cellDes.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()))
                {
                    return true;
                }
                if (cellDes.CellType == PivotCellDescriptorType.SummaryRow || cellDes.CellType == PivotCellDescriptorType.SummaryColumn )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
