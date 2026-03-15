#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TableLayoutCalculator 
    {
        /// <summary>
        /// 
        /// </summary>
        public TableLayoutCalculator()
        {

        }        

        /// <summary>
        /// Measures the Breadths of the Paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal static BreadthsInfo MeasureParagraphBreadths(BlockAdv paragraph)
        {
            List<double> text = new List<double>();
            double width = 0.0;
            double max = 0;

            foreach (Inline inline in paragraph.Inlines)
            {
                foreach (ElementBox box in inline.ElementBoxes)
                {
                    if (!box.IsUIBox && !box.IsImageBox)
                    {
                        width += TextHelper.MeasureText(box.InternalText, box).Width;
                    }
                    else
                    {
                        if (box.IsImageBox)
                        {
                            width += (box as ImageElementBox).Width;
                        }
                        else if (box.IsUIBox)
                        {
                            width += (box as UIElementBox).Width;
                        }
                    }
                    if (box.InternalText == "\t" || box.InternalText == " ")
                    {
                        text.Add(width);
                        width = 0;
                        continue;
                    }
                }
            }
            text.Add(width);
            if (text.Count == 0)
            {
                max = width;
            }
            else
            {
                max = (from c in text select c).Max();
            }
            if (paragraph.LeftIndent > 0.0)
            {
                max += paragraph.LeftIndent;
            }
            if (paragraph.RightIndent > 0.0)
            {
                max += paragraph.RightIndent;
            }

            return new BreadthsInfo(5d, max);
        }

        /// <summary>
        /// Arranges the Table layout
        /// </summary>
        /// <param name="table"></param>
        /// <param name="width"></param>
        internal static void MeasureTableOnAutoMode(TableAdv table,double width)
        {
            MeasureTableLayout(table,width);
            table.UpdateTableCells();
            table.PrepareBoxLines();
            table.SetElementSize();
        }


        internal static void MeasureTableOnAutoMode(TableAdv table,int index,double width)
        {
            MeasureTableLayout(table, width);
            table.UpdateTableCells(index);
            table.PrepareBoxLines(index);
            table.SetElementSize(index);
        }

        internal static void MeasureTableLayout(TableAdv table, double width)
        {
            BreadthsInfo minmaxwidths = MeasureTableBreadths(table, width);

            if (minmaxwidths.MaxWidth <= width)
            {
                EnlargeTableWidth(table, width);
            }
            if (minmaxwidths.MaxWidth >= width)
            {
                SpreadWidthToRemainingRows(table, width);
            }
            if (minmaxwidths.MinWidth >= width)
            {
                for (int i = 0; i < table.TableHolder.Columns.Count; i++)
                {
                    TableColumnAdv column = table.TableHolder.Columns[i];
                    column.PreferredWidth = column.MinWidth;
                }
            }
            table.SetDesiredWidthToCells();
        }

        /// <summary>
        /// Adds the preferred width of the columns
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static double SumOfPreferredWidths(TableAdv table)
        {
            return table.TableHolder.Columns.Sum(c => c.PreferredWidth);
        }

        /// <summary>
        /// It measures the Breadths of the Table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal static BreadthsInfo MeasureTableBreadths(TableAdv table,double width)
        {
            foreach (TableRowAdv row in table.Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    BreadthsInfo cellbreadth = MeasureTableCellBreadths(table, cell, width);
                    cellbreadth = new BreadthsInfo(cellbreadth.MinWidth / cell.ColumnSpan, cellbreadth.MaxWidth / cell.ColumnSpan);
                    cellbreadth = BreadthsInfo.Max(new BreadthsInfo().DefaultBreadths, cellbreadth);
                    UpdateColumnBreadths(cellbreadth, cell, cell.ColumnIndex);
                }
            }
            return new BreadthsInfo(table.TableHolder.Columns.Sum(c => c.MinWidth), table.TableHolder.Columns.Sum(c => c.MaxWidth));
        }

        /// <summary>
        /// It measures the TableCell's breadths
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cell"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        internal static BreadthsInfo MeasureTableCellBreadths(TableAdv table,TableCellAdv cell,double available)
        {
            BreadthsInfo breadths = new BreadthsInfo().DefaultBreadths;
            foreach (BlockAdv block in cell.Blocks)
            {
                breadths = BreadthsInfo.Max(breadths, MeasureCellBlockBreadths(table, block, available));
            }
            return new BreadthsInfo(breadths.MinWidth + 10d, breadths.MaxWidth + 15d);
        }

        /// <summary>
        /// It measures the Block's breadths inside the TableCell
        /// </summary>
        /// <param name="maintable"></param>
        /// <param name="cellblock"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        internal static BreadthsInfo MeasureCellBlockBreadths(TableAdv maintable,BlockAdv cellblock, double available)
        {
            if (cellblock.IsTable)
            {
                return MeasureTableBreadths(cellblock as TableAdv, available);
            }
            else
            {
                return MeasureParagraphBreadths(cellblock);
            }
        }

        /// <summary>
        /// It updates the Column's Breadths
        /// </summary>
        /// <param name="cellbreadths"></param>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        internal static void UpdateColumnBreadths(BreadthsInfo cellbreadths,TableCellAdv cell,int index)
        {
            if (cell.OwnerTable != null)
            {
                TableAdv owner = cell.OwnerTable;
                for (int i = index; i < (index + cell.ColumnSpan); i++)
                {
                    owner.TableHolder.Columns[i].MinWidth = Math.Max(owner.TableHolder.Columns[i].MinWidth, cellbreadths.MinWidth);
                    owner.TableHolder.Columns[i].MaxWidth = Math.Max(owner.TableHolder.Columns[i].MaxWidth, cellbreadths.MaxWidth);
                }
            }
        }
        
        /// <summary>
        /// It spreads the Column width to remaining rows
        /// </summary>
        /// <param name="table"></param>
        /// <param name="width"></param>
        internal static void SpreadWidthToRemainingRows(TableAdv table, double width)
        {
            double totalminwidth = table.TableHolder.Columns.Sum(c => c.MinWidth);
            double remaining = width - totalminwidth;

            double totalmaxwidth = table.TableHolder.Columns.Sum(c => c.MaxWidth);

            double bottomvalue = totalmaxwidth - totalminwidth;

            double factor = remaining / bottomvalue;
            for (int i = 0; i < table.TableHolder.Columns.Count; i++)
            {
                TableColumnAdv column = table.TableHolder.Columns[i];
                column.PreferredWidth = column.MinWidth + (column.MaxWidth - column.MinWidth) * factor;
            }
        }

        /// <summary>
        /// It enlarges the TableWidth
        /// </summary>
        /// <param name="table"></param>
        /// <param name="width"></param>
        internal static void EnlargeTableWidth(TableAdv table,double width)
        {
            //Average width of every columns.

            double totalmaxwidth = table.TableHolder.Columns.Sum(c => c.MaxWidth);
            double totalminwidth = table.TableHolder.Columns.Sum(c => c.MinWidth);
            double totalpreferredwidth = 0.0;

            double remaining = width - totalmaxwidth;
            
            double average = width / ((double)table.TableHolder.Columns.Count);

            List<TableColumnAdv> columns = new List<TableColumnAdv>();

            for (int i = 0; i < table.TableHolder.Columns.Count; i++)
            {
                TableColumnAdv column = table.TableHolder.Columns[i];

                if (average <=column.MaxWidth)
                    column.PreferredWidth = column.MaxWidth;
                else
                    column.PreferredWidth = average;

                totalpreferredwidth += column.PreferredWidth;
            }

            double bottomvalue = totalpreferredwidth - totalmaxwidth;

            //factor to be multiplied 
            double factor = remaining / bottomvalue;

            for (int j = 0; j < table.TableHolder.Columns.Count; j++)
            {
                TableColumnAdv column = table.TableHolder.Columns[j];
                if (column.MaxWidth <= average)
                {
                    column.PreferredWidth = column.MaxWidth + (column.PreferredWidth - column.MaxWidth) * factor;
                }
            }
            
        }
    }
}
