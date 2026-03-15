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
using System.Windows.Media;
using Syncfusion.XlsIO;
using System.Collections;
using System.Windows;
using System.Windows.Documents;

#if EXCELGRID
using Syncfusion.Windows.Controls.Spreadsheet;
namespace Syncfusion.Windows.Controls.Grid.Converter
#else
namespace Syncfusion.Windows.Controls.Grid.Converter
#endif
{
#if EXCELGRID
    public class ExcelGridImportTableStyleConverter
#else
    public class GridImportTableStyleConverter
#endif
    {
        /// <summary>
        /// Checks for the cells is in table.
        /// </summary>
        /// <param name="range">The Range.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="sheet">The sheet.</param>
        /// <returns></returns>
        public bool CheckForTableCell(IRange range, GridStyleInfo cell, IWorksheet sheet)
        {
            if (sheet.ListObjects.Count > 0)
            {
                foreach (IListObject table in sheet.ListObjects)
                {
                    IRange tablerange = table.Location;
                    TableBuiltInStyles tablestyle = table.BuiltInTableStyle;
                    if (tablerange.Row <= range.Row && tablerange.Column <= range.Column && tablerange.LastRow >= range.LastRow && tablerange.LastColumn >= range.LastColumn)
                    {
                        if (range.BuiltInStyle == BuiltInStyles.Normal)
                        {
                            if (tablerange.Row == range.Row)
                                ApplyHeaderStyle(table, cell, tablestyle);
                            else
                                ApplyTableContentStyle(table, cell, tablestyle);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Applies the table content style to Grid cells.
        /// </summary>
        /// <param name="table">The Table.</param>
        /// <param name="cell">The Cell.</param>
        /// <param name="tablestyle">The Tablestyle.</param>
          protected void ApplyTableContentStyle(IListObject table, GridStyleInfo cell, TableBuiltInStyles tablestyle)
        {
            int row = cell.RowIndex-table.Location.Row;
            int col = cell.ColumnIndex-table.Location.Column;
            Color firstbackground;
            Color secondbackground;
            bool entireRow = false;
            GetTableContentBackGround(tablestyle, out firstbackground, out secondbackground, out entireRow);
            if (table.ShowTableStyleRowStripes && table.ShowTableStyleColumnStripes)
            {
                if (row % 2 == 0)
                {
                    if (col % 2 == 0)
                        cell.Background = new SolidColorBrush(firstbackground);
                    else
                        cell.Background = new SolidColorBrush(secondbackground);
                }
                else
                {
                    cell.Background = new SolidColorBrush(firstbackground);
                }
            }
            else if (table.ShowTableStyleColumnStripes)
            {
                if (col % 2 == 0)
                    cell.Background = new SolidColorBrush(firstbackground);
                else
                    cell.Background = new SolidColorBrush(secondbackground);
            }
            else if(table.ShowTableStyleRowStripes)
            {
                if (row % 2 == 0)
                    cell.Background = new SolidColorBrush(secondbackground);
                else
                    cell.Background = new SolidColorBrush(firstbackground);
            }
            else
            {
                cell.Background = new SolidColorBrush(secondbackground);
            }

            if (entireRow && table.ShowFirstColumn)
            {
                if (cell.ColumnIndex == table.Location.Column)
                    cell.Background = new SolidColorBrush(GetTableHeaderBackGround(tablestyle));
            }

            if(entireRow && table.ShowLastColumn)
            {
                if (cell.ColumnIndex == table.Location.LastColumn)
                    cell.Background = new SolidColorBrush(GetTableHeaderBackGround(tablestyle));
            }

            cell.Foreground = new SolidColorBrush(GetTableContentForeground(tablestyle));
            TableBorderStyle tableBorderStyle;
            Color borderColor;
            GetTableContentBorderStyle(tablestyle, out tableBorderStyle, out borderColor);
            cell.Borders.Top = null;
            cell.Borders.Bottom = null;
            cell.Borders.Left = null;
            cell.Borders.Right = null;
            ApplyBorder(cell, tableBorderStyle, borderColor);
            if (table.Location.LastRow == cell.RowIndex)
                ApplyBorder(cell, TableBorderStyle.Bottom, borderColor);
            if (table.Location.LastColumn == cell.ColumnIndex)
                ApplyBorder(cell, TableBorderStyle.Right, borderColor);
        }

        /// <summary>
        /// Applies the header style to cell.
        /// </summary>
        /// <param name="table">The Table.</param>
        /// <param name="cell">The Cell.</param>
        /// <param name="tablestyle">The Tablestyle.</param>
        protected void ApplyHeaderStyle(IListObject table, GridStyleInfo cell, TableBuiltInStyles tablestyle)
        {
            TableBorderStyle tableBorderStyle;
            Color borderColor;
            cell.Background = new SolidColorBrush(GetTableHeaderBackGround(tablestyle));
            cell.Foreground = new SolidColorBrush(GetTableHeaderForeground(tablestyle));
            GetTableHeaderBorderStyle(tablestyle, out tableBorderStyle, out borderColor);
            ApplyBorder(cell, tableBorderStyle, borderColor);
        }

        protected void ApplyBorder(GridStyleInfo cell, TableBorderStyle tableBorderStyle, Color borderColor)
        {
            if (tableBorderStyle == TableBorderStyle.All)
            {
                cell.Borders.All = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.Top)
            {
                cell.Borders.Top = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.Left)
            {
                cell.Borders.Left = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.Bottom)
            {
                cell.Borders.Bottom = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.Right)
            {
                cell.Borders.Right = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.TopBottom)
            {
                cell.Borders.Top = new Pen(new SolidColorBrush(borderColor), 0.5);
                cell.Borders.Bottom = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
            else if (tableBorderStyle == TableBorderStyle.LeftRight)
            {
                cell.Borders.Left = new Pen(new SolidColorBrush(borderColor), 0.5);
                cell.Borders.Right = new Pen(new SolidColorBrush(borderColor), 0.5);
            }
        }

        /// <summary>
        /// Gets the table content border style.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <param name="tableBorderStyle">The Table border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        protected void GetTableContentBorderStyle(TableBuiltInStyles tablestyle, out TableBorderStyle tableBorderStyle, out Color borderColor)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                case TableBuiltInStyles.TableStyleDark10:
                case TableBuiltInStyles.TableStyleDark11:
                case TableBuiltInStyles.TableStyleDark2:
                case TableBuiltInStyles.TableStyleDark3:
                case TableBuiltInStyles.TableStyleDark4:
                case TableBuiltInStyles.TableStyleDark5:
                case TableBuiltInStyles.TableStyleDark6:
                case TableBuiltInStyles.TableStyleDark7:
                case TableBuiltInStyles.TableStyleDark8:
                case TableBuiltInStyles.TableStyleDark9:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleLight1:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleLight10:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 192, 80, 77);
                    break;
                case TableBuiltInStyles.TableStyleLight11:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 155, 187, 89);
                    break;
                case TableBuiltInStyles.TableStyleLight12:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 128, 100, 162);
                    break;
                case TableBuiltInStyles.TableStyleLight13:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 75, 172, 198);
                    break;
                case TableBuiltInStyles.TableStyleLight14:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 247, 150, 70);
                    break;
                case TableBuiltInStyles.TableStyleLight15:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleLight16:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 79, 129, 189);
                    break;
                case TableBuiltInStyles.TableStyleLight17:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 192, 80, 77);
                    break;
                case TableBuiltInStyles.TableStyleLight18:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 155, 187, 89);
                    break;
                case TableBuiltInStyles.TableStyleLight19:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 128, 100, 162);
                    break;
                case TableBuiltInStyles.TableStyleLight2:
                case TableBuiltInStyles.TableStyleLight3:
                case TableBuiltInStyles.TableStyleLight4:
                case TableBuiltInStyles.TableStyleLight5:
                case TableBuiltInStyles.TableStyleLight6:
                case TableBuiltInStyles.TableStyleLight7:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleLight20:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 75, 172, 198);
                    break;
                case TableBuiltInStyles.TableStyleLight21:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 247, 150, 70);
                    break;
                case TableBuiltInStyles.TableStyleLight8:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Colors.Black;
                    break;
                case TableBuiltInStyles.TableStyleLight9:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 79, 129, 189);
                    break;
                case TableBuiltInStyles.TableStyleMedium1:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium15:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Colors.Black;
                    break;
                case TableBuiltInStyles.TableStyleMedium16:
                case TableBuiltInStyles.TableStyleMedium17:
                case TableBuiltInStyles.TableStyleMedium18:
                case TableBuiltInStyles.TableStyleMedium19:
                case TableBuiltInStyles.TableStyleMedium20:
                case TableBuiltInStyles.TableStyleMedium21:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleMedium2:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
                case TableBuiltInStyles.TableStyleMedium22:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium23:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
                case TableBuiltInStyles.TableStyleMedium24:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 218, 150, 148);
                    break;
                case TableBuiltInStyles.TableStyleMedium25:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 196, 215, 155);
                    break;
                case TableBuiltInStyles.TableStyleMedium26:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 177, 160, 199);
                    break;
                case TableBuiltInStyles.TableStyleMedium27:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 146, 205, 220);
                    break;
                case TableBuiltInStyles.TableStyleMedium28:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 250, 191, 143);
                    break;
                case TableBuiltInStyles.TableStyleMedium3:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 218, 150, 148);
                    break;
                case TableBuiltInStyles.TableStyleMedium4:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 196, 215, 155);
                    break;
                case TableBuiltInStyles.TableStyleMedium5:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 146, 205, 220);
                    break;
                case TableBuiltInStyles.TableStyleMedium6:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 146, 205, 220);
                    break;
                case TableBuiltInStyles.TableStyleMedium7:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 250, 191, 143);
                    break;
                case TableBuiltInStyles.TableStyleMedium8:
                case TableBuiltInStyles.TableStyleMedium9:
                case TableBuiltInStyles.TableStyleMedium10:
                case TableBuiltInStyles.TableStyleMedium11:
                case TableBuiltInStyles.TableStyleMedium12:
                case TableBuiltInStyles.TableStyleMedium13:
                case TableBuiltInStyles.TableStyleMedium14:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Colors.White;
                    break;
                default:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
            }
        }

        /// <summary>
        /// Gets the table header border style.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <param name="tableBorderStyle">The Table border style.</param>
        /// <param name="borderColor">Color of the border.</param>
        protected void GetTableHeaderBorderStyle(TableBuiltInStyles tablestyle, out TableBorderStyle tableBorderStyle, out Color borderColor)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark10:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleDark11:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleDark2:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark3:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark4:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark5:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark6:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark7:
                    tableBorderStyle = TableBorderStyle.Bottom;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleDark8:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleDark9:
                    tableBorderStyle = TableBorderStyle.None;
                    borderColor = Colors.White;
                    break;
                case TableBuiltInStyles.TableStyleLight1:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Colors.Black;
                    break;
                case TableBuiltInStyles.TableStyleLight10:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 192, 80, 77);
                    break;
                case TableBuiltInStyles.TableStyleLight11:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 155, 187, 89);
                    break;
                case TableBuiltInStyles.TableStyleLight12:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 128, 100, 162);
                    break;
                case TableBuiltInStyles.TableStyleLight13:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 75, 172, 198);
                    break;
                case TableBuiltInStyles.TableStyleLight14:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 247, 150, 70);
                    break;
                case TableBuiltInStyles.TableStyleLight15:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleLight16:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 79, 129, 189);
                    break;
                case TableBuiltInStyles.TableStyleLight17:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 192, 80, 77);
                    break;
                case TableBuiltInStyles.TableStyleLight18:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 155, 187, 89);
                    break;
                case TableBuiltInStyles.TableStyleLight19:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 128, 100, 162);
                    break;
                case TableBuiltInStyles.TableStyleLight2:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 79, 129, 189);
                    break;
                case TableBuiltInStyles.TableStyleLight20:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 75, 172, 198);
                    break;
                case TableBuiltInStyles.TableStyleLight21:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 247, 150, 70);
                    break;
                case TableBuiltInStyles.TableStyleLight3:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 192, 80, 77);
                    break;
                case TableBuiltInStyles.TableStyleLight4:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 155, 187, 89);
                    break;
                case TableBuiltInStyles.TableStyleLight5:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 128, 100, 162);
                    break;
                case TableBuiltInStyles.TableStyleLight6:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 75, 172, 198);
                    break;
                case TableBuiltInStyles.TableStyleLight7:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 247, 150, 70);
                    break;
                case TableBuiltInStyles.TableStyleLight8:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleLight9:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 79, 129, 189);
                    break;
                case TableBuiltInStyles.TableStyleMedium1:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium10:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium11:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium12:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium13:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium14:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium15:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium16:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium17:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium18:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium19:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium2:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
                case TableBuiltInStyles.TableStyleMedium20:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium21:
                    tableBorderStyle = TableBorderStyle.TopBottom;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium22:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 0, 0, 0);
                    break;
                case TableBuiltInStyles.TableStyleMedium23:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
                case TableBuiltInStyles.TableStyleMedium24:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 218, 150, 148);
                    break;
                case TableBuiltInStyles.TableStyleMedium25:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 196, 215, 155);
                    break;
                case TableBuiltInStyles.TableStyleMedium26:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 177, 160, 199);
                    break;
                case TableBuiltInStyles.TableStyleMedium27:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 146, 205, 220);
                    break;
                case TableBuiltInStyles.TableStyleMedium28:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 250, 191, 143);
                    break;
                case TableBuiltInStyles.TableStyleMedium3:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 218, 150, 148);
                    break;
                case TableBuiltInStyles.TableStyleMedium4:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 196, 215, 155);
                    break;
                case TableBuiltInStyles.TableStyleMedium5:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 177, 160, 199);
                    break;
                case TableBuiltInStyles.TableStyleMedium6:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 146, 205, 220);
                    break;
                case TableBuiltInStyles.TableStyleMedium7:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 250, 191, 143);
                    break;
                case TableBuiltInStyles.TableStyleMedium8:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                case TableBuiltInStyles.TableStyleMedium9:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 255, 255, 255);
                    break;
                default:
                    tableBorderStyle = TableBorderStyle.All;
                    borderColor = Color.FromArgb(255, 149, 179, 215);
                    break;
            }
        }

        /// <summary>
        /// Gets the table content back ground.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <param name="firstBackgroundColor">First color of the background.</param>
        /// <param name="secondBackgroundColor">Color of the second background.</param>
        /// <param name="entireRow">if set to <c>true</c> apply bockground to entire row.</param>
        protected void GetTableContentBackGround(TableBuiltInStyles tablestyle, out Color firstBackgroundColor, out Color secondBackgroundColor, out bool entireRow)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                    firstBackgroundColor = Color.FromArgb(255, 64, 64, 64);
                    secondBackgroundColor = Color.FromArgb(255, 115, 115, 115);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark10:
                    firstBackgroundColor = Color.FromArgb(255, 216, 228, 188);
                    secondBackgroundColor = Color.FromArgb(255, 235, 241, 222);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleDark11:
                    firstBackgroundColor = Color.FromArgb(255, 183, 222, 232);
                    secondBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleDark2:
                    firstBackgroundColor = Color.FromArgb(255, 54, 96, 146);
                    secondBackgroundColor = Color.FromArgb(255, 79, 129, 189);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark3:
                    firstBackgroundColor = Color.FromArgb(255, 150, 54, 52);
                    secondBackgroundColor = Color.FromArgb(255, 192, 80, 77);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark4:
                    firstBackgroundColor = Color.FromArgb(255, 118, 147, 60);
                    secondBackgroundColor = Color.FromArgb(255, 155, 187, 89);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark5:
                    firstBackgroundColor = Color.FromArgb(255, 96, 73, 122);
                    secondBackgroundColor = Color.FromArgb(255, 128, 100, 162);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark6:
                    firstBackgroundColor = Color.FromArgb(255, 49, 134, 155);
                    secondBackgroundColor = Color.FromArgb(255, 75, 172, 198);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark7:
                    firstBackgroundColor = Color.FromArgb(255, 226, 107, 10);
                    secondBackgroundColor = Color.FromArgb(255, 247, 150, 70);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleDark8:
                    firstBackgroundColor = Color.FromArgb(255, 166, 166, 166);
                    secondBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleDark9:
                    firstBackgroundColor = Color.FromArgb(255, 184, 204, 228);
                    secondBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight1:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight15:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight16:
                    firstBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight17:
                    firstBackgroundColor = Color.FromArgb(255, 242, 220, 219);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight18:
                    firstBackgroundColor = Color.FromArgb(255, 255, 241, 222);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight19:
                    firstBackgroundColor = Color.FromArgb(255, 228, 223, 236);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight2:
                    firstBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight20:
                    firstBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight21:
                    firstBackgroundColor = Color.FromArgb(255, 253, 233, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight3:
                    firstBackgroundColor = Color.FromArgb(255, 242, 220, 219);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight4:
                    firstBackgroundColor = Color.FromArgb(255, 235, 241, 222);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight5:
                    firstBackgroundColor = Color.FromArgb(255, 228, 223, 236);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight6:
                    firstBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight7:
                    firstBackgroundColor = Color.FromArgb(255, 253, 233, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleLight8:
                case TableBuiltInStyles.TableStyleLight9:
                case TableBuiltInStyles.TableStyleLight10:
                case TableBuiltInStyles.TableStyleLight11:
                case TableBuiltInStyles.TableStyleLight12:
                case TableBuiltInStyles.TableStyleLight13:
                case TableBuiltInStyles.TableStyleLight14:
                    firstBackgroundColor = Colors.White;
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium1:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium10:
                    firstBackgroundColor = Color.FromArgb(255, 230, 184, 183);
                    secondBackgroundColor = Color.FromArgb(255, 242, 220, 219);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium11:
                    firstBackgroundColor = Color.FromArgb(255, 216, 228, 188);
                    secondBackgroundColor = Color.FromArgb(255, 235, 241, 222);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium12:
                    firstBackgroundColor = Color.FromArgb(255, 204, 192, 218);
                    secondBackgroundColor = Color.FromArgb(255, 228, 223, 236);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium13:
                    firstBackgroundColor = Color.FromArgb(255, 183, 222, 232);
                    secondBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium14:
                    firstBackgroundColor = Color.FromArgb(255, 252, 213, 180);
                    secondBackgroundColor = Color.FromArgb(255, 253, 233, 217);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium15:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium16:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium17:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium18:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium19:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium2:
                    firstBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium20:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium21:
                    firstBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium22:
                    firstBackgroundColor = Color.FromArgb(255, 166, 166, 166);
                    secondBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium23:
                    firstBackgroundColor = Color.FromArgb(255, 184, 201, 228);
                    secondBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium24:
                    firstBackgroundColor = Color.FromArgb(255, 230, 184, 183);
                    secondBackgroundColor = Color.FromArgb(255, 242, 220, 219);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium25:
                    firstBackgroundColor = Color.FromArgb(255, 216, 228, 188);
                    secondBackgroundColor = Color.FromArgb(255, 235, 241, 222);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium26:
                    firstBackgroundColor = Color.FromArgb(255, 204, 192, 218);
                    secondBackgroundColor = Color.FromArgb(255, 228, 223, 236);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium27:
                    firstBackgroundColor = Color.FromArgb(255, 183, 222, 232);
                    secondBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium28:
                    firstBackgroundColor = Color.FromArgb(255, 252, 213, 180);
                    secondBackgroundColor = Color.FromArgb(255, 253, 233, 217);
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium3:
                    firstBackgroundColor = Color.FromArgb(255, 242, 220, 219);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium4:
                    firstBackgroundColor = Color.FromArgb(255, 235, 241, 222);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium5:
                    firstBackgroundColor = Color.FromArgb(255, 228, 223, 236);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium6:
                    firstBackgroundColor = Color.FromArgb(255, 218, 238, 243);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium7:
                    firstBackgroundColor = Color.FromArgb(255, 53, 233, 217);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
                case TableBuiltInStyles.TableStyleMedium8:
                    firstBackgroundColor = Color.FromArgb(255, 166, 166, 166);
                    secondBackgroundColor = Color.FromArgb(255, 217, 217, 217);
                    entireRow = true;
                    break;
                case TableBuiltInStyles.TableStyleMedium9:
                    firstBackgroundColor = Color.FromArgb(255, 184, 204, 228);
                    secondBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    entireRow = true;
                    break;
                default:
                    firstBackgroundColor = Color.FromArgb(255, 220, 230, 241);
                    secondBackgroundColor = Colors.White;
                    entireRow = false;
                    break;
            }
        }

        /// <summary>
        /// Gets the table content foreground.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <returns></returns>
        protected Color GetTableContentForeground(TableBuiltInStyles tablestyle)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                case TableBuiltInStyles.TableStyleDark2:
                case TableBuiltInStyles.TableStyleDark3:
                case TableBuiltInStyles.TableStyleDark4:
                case TableBuiltInStyles.TableStyleDark5:
                case TableBuiltInStyles.TableStyleDark6:
                case TableBuiltInStyles.TableStyleDark7:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleDark8:
                case TableBuiltInStyles.TableStyleDark9:
                case TableBuiltInStyles.TableStyleDark10:
                case TableBuiltInStyles.TableStyleDark11:
                    return Colors.Black;
                case TableBuiltInStyles.TableStyleLight1:
                    return Colors.Black;
                case TableBuiltInStyles.TableStyleLight2:
                    return Color.FromArgb(255, 54, 96, 146);
                case TableBuiltInStyles.TableStyleLight3:
                    return Color.FromArgb(255, 150, 54, 52);
                case TableBuiltInStyles.TableStyleLight4:
                    return Color.FromArgb(255, 118, 147, 60);
                case TableBuiltInStyles.TableStyleLight5:
                    return Color.FromArgb(255, 96, 73, 122);
                case TableBuiltInStyles.TableStyleLight6:
                    return Color.FromArgb(255, 49, 134, 155);
                case TableBuiltInStyles.TableStyleLight7:
                    return Color.FromArgb(255, 226, 107, 10);
                case TableBuiltInStyles.TableStyleLight8:
                case TableBuiltInStyles.TableStyleLight9:
                case TableBuiltInStyles.TableStyleLight10:
                case TableBuiltInStyles.TableStyleLight11:
                case TableBuiltInStyles.TableStyleLight12:
                case TableBuiltInStyles.TableStyleLight13:
                case TableBuiltInStyles.TableStyleLight14:
                case TableBuiltInStyles.TableStyleLight15:
                case TableBuiltInStyles.TableStyleLight16:
                case TableBuiltInStyles.TableStyleLight17:
                case TableBuiltInStyles.TableStyleLight18:
                case TableBuiltInStyles.TableStyleLight19:
                case TableBuiltInStyles.TableStyleLight20:
                case TableBuiltInStyles.TableStyleLight21:
                    return Colors.Black;
                case TableBuiltInStyles.TableStyleMedium1:
                case TableBuiltInStyles.TableStyleMedium10:
                case TableBuiltInStyles.TableStyleMedium11:
                case TableBuiltInStyles.TableStyleMedium12:
                case TableBuiltInStyles.TableStyleMedium13:
                case TableBuiltInStyles.TableStyleMedium14:
                case TableBuiltInStyles.TableStyleMedium15:
                case TableBuiltInStyles.TableStyleMedium16:
                case TableBuiltInStyles.TableStyleMedium17:
                case TableBuiltInStyles.TableStyleMedium18:
                case TableBuiltInStyles.TableStyleMedium19:
                case TableBuiltInStyles.TableStyleMedium2:
                case TableBuiltInStyles.TableStyleMedium20:
                case TableBuiltInStyles.TableStyleMedium21:
                case TableBuiltInStyles.TableStyleMedium22:
                case TableBuiltInStyles.TableStyleMedium23:
                case TableBuiltInStyles.TableStyleMedium24:
                case TableBuiltInStyles.TableStyleMedium25:
                case TableBuiltInStyles.TableStyleMedium26:
                case TableBuiltInStyles.TableStyleMedium27:
                case TableBuiltInStyles.TableStyleMedium28:
                case TableBuiltInStyles.TableStyleMedium3:
                case TableBuiltInStyles.TableStyleMedium4:
                case TableBuiltInStyles.TableStyleMedium5:
                case TableBuiltInStyles.TableStyleMedium6:
                case TableBuiltInStyles.TableStyleMedium7:
                case TableBuiltInStyles.TableStyleMedium8:
                case TableBuiltInStyles.TableStyleMedium9:
                    return Colors.Black;
                default:
                    return Colors.Black;
            }
        }

        /// <summary>
        /// Gets the table header foreground.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <returns></returns>
        protected Color GetTableHeaderForeground(TableBuiltInStyles tablestyle)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                case TableBuiltInStyles.TableStyleDark10:
                case TableBuiltInStyles.TableStyleDark11:
                case TableBuiltInStyles.TableStyleDark2:
                case TableBuiltInStyles.TableStyleDark3:
                case TableBuiltInStyles.TableStyleDark4:
                case TableBuiltInStyles.TableStyleDark5:
                case TableBuiltInStyles.TableStyleDark6:
                case TableBuiltInStyles.TableStyleDark7:
                case TableBuiltInStyles.TableStyleDark8:
                case TableBuiltInStyles.TableStyleDark9:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight1:
                case TableBuiltInStyles.TableStyleLight15:
                case TableBuiltInStyles.TableStyleLight16:
                case TableBuiltInStyles.TableStyleLight17:
                case TableBuiltInStyles.TableStyleLight18:
                case TableBuiltInStyles.TableStyleLight19:
                case TableBuiltInStyles.TableStyleLight20:
                case TableBuiltInStyles.TableStyleLight21:
                    return Colors.Black;
                case TableBuiltInStyles.TableStyleLight2:
                    return Color.FromArgb(255, 54, 96, 146);
                case TableBuiltInStyles.TableStyleLight3:
                    return Color.FromArgb(255, 150, 54, 52);
                case TableBuiltInStyles.TableStyleLight4:
                    return Color.FromArgb(255, 118, 147, 60);
                case TableBuiltInStyles.TableStyleLight5:
                    return Color.FromArgb(255, 96, 73, 122);
                case TableBuiltInStyles.TableStyleLight6:
                    return Color.FromArgb(255, 49, 134, 155);
                case TableBuiltInStyles.TableStyleLight7:
                    return Color.FromArgb(255, 226, 107, 10);
                case TableBuiltInStyles.TableStyleLight8:
                case TableBuiltInStyles.TableStyleLight9:
                case TableBuiltInStyles.TableStyleLight10:
                case TableBuiltInStyles.TableStyleLight11:
                case TableBuiltInStyles.TableStyleLight12:
                case TableBuiltInStyles.TableStyleLight13:
                case TableBuiltInStyles.TableStyleLight14:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleMedium1:
                case TableBuiltInStyles.TableStyleMedium10:
                case TableBuiltInStyles.TableStyleMedium11:
                case TableBuiltInStyles.TableStyleMedium12:
                case TableBuiltInStyles.TableStyleMedium13:
                case TableBuiltInStyles.TableStyleMedium14:
                case TableBuiltInStyles.TableStyleMedium15:
                case TableBuiltInStyles.TableStyleMedium16:
                case TableBuiltInStyles.TableStyleMedium17:
                case TableBuiltInStyles.TableStyleMedium18:
                case TableBuiltInStyles.TableStyleMedium19:
                case TableBuiltInStyles.TableStyleMedium2:
                case TableBuiltInStyles.TableStyleMedium20:
                case TableBuiltInStyles.TableStyleMedium21:
                case TableBuiltInStyles.TableStyleMedium3:
                case TableBuiltInStyles.TableStyleMedium4:
                case TableBuiltInStyles.TableStyleMedium5:
                case TableBuiltInStyles.TableStyleMedium6:
                case TableBuiltInStyles.TableStyleMedium7:
                case TableBuiltInStyles.TableStyleMedium8:
                case TableBuiltInStyles.TableStyleMedium9:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleMedium22:
                case TableBuiltInStyles.TableStyleMedium23:
                case TableBuiltInStyles.TableStyleMedium24:
                case TableBuiltInStyles.TableStyleMedium25:
                case TableBuiltInStyles.TableStyleMedium26:
                case TableBuiltInStyles.TableStyleMedium27:
                case TableBuiltInStyles.TableStyleMedium28:
                    return Colors.Black;
                default:
                    return Colors.White;
            }
        }

        /// <summary>
        /// Gets the table header background.
        /// </summary>
        /// <param name="tablestyle">The Tablestyle.</param>
        /// <returns></returns>
        protected Color GetTableHeaderBackGround(TableBuiltInStyles tablestyle)
        {
            switch (tablestyle)
            {
                case TableBuiltInStyles.TableStyleDark1:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark10:
                    return Color.FromArgb(255, 128, 100, 162);
                case TableBuiltInStyles.TableStyleDark11:
                    return Color.FromArgb(255, 247, 150, 70);
                case TableBuiltInStyles.TableStyleDark2:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark3:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark4:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark5:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark6:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark7:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark8:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleDark9:
                    return Color.FromArgb(255, 192, 80, 77);
                case TableBuiltInStyles.TableStyleLight1:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight10:
                    return Color.FromArgb(255, 192, 80, 77);
                case TableBuiltInStyles.TableStyleLight11:
                    return Color.FromArgb(255, 155, 187, 89);
                case TableBuiltInStyles.TableStyleLight12:
                    return Color.FromArgb(255, 128, 100, 162);
                case TableBuiltInStyles.TableStyleLight13:
                    return Color.FromArgb(255, 75, 172, 198);
                case TableBuiltInStyles.TableStyleLight14:
                    return Color.FromArgb(255, 247, 150, 70);
                case TableBuiltInStyles.TableStyleLight15:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight16:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight17:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight18:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight19:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight2:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight20:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight21:
                    return Color.FromArgb(255, 255, 255, 255);
                case TableBuiltInStyles.TableStyleLight3:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight4:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight5:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight6:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight7:
                    return Colors.White;
                case TableBuiltInStyles.TableStyleLight8:
                    return Colors.Black;
                case TableBuiltInStyles.TableStyleLight9:
                    return Color.FromArgb(255, 79, 129, 189);
                case TableBuiltInStyles.TableStyleMedium1:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleMedium10:
                    return Color.FromArgb(255, 192, 80, 77);
                case TableBuiltInStyles.TableStyleMedium11:
                    return Color.FromArgb(255, 155, 187, 89);
                case TableBuiltInStyles.TableStyleMedium12:
                    return Color.FromArgb(255, 128, 100, 162);
                case TableBuiltInStyles.TableStyleMedium13:
                    return Color.FromArgb(255, 75, 172, 198);
                case TableBuiltInStyles.TableStyleMedium14:
                    return Color.FromArgb(255, 247, 150, 70);
                case TableBuiltInStyles.TableStyleMedium15:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleMedium16:
                    return Color.FromArgb(255, 79, 129, 189);
                case TableBuiltInStyles.TableStyleMedium17:
                    return Color.FromArgb(255, 192, 80, 77);
                case TableBuiltInStyles.TableStyleMedium18:
                    return Color.FromArgb(255, 155, 187, 89);
                case TableBuiltInStyles.TableStyleMedium19:
                    return Color.FromArgb(255, 128, 100, 162);
                case TableBuiltInStyles.TableStyleMedium2:
                    return Color.FromArgb(255, 79, 129, 189);
                case TableBuiltInStyles.TableStyleMedium20:
                    return Color.FromArgb(255, 75, 172, 198);
                case TableBuiltInStyles.TableStyleMedium21:
                    return Color.FromArgb(255, 247, 150, 70);
                case TableBuiltInStyles.TableStyleMedium22:
                    return Color.FromArgb(255, 217, 217, 217);
                case TableBuiltInStyles.TableStyleMedium23:
                    return Color.FromArgb(255, 220, 230, 241);
                case TableBuiltInStyles.TableStyleMedium24:
                    return Color.FromArgb(255, 242, 220, 219);
                case TableBuiltInStyles.TableStyleMedium25:
                    return Color.FromArgb(255, 235, 241, 222);
                case TableBuiltInStyles.TableStyleMedium26:
                    return Color.FromArgb(255, 228, 223, 236);
                case TableBuiltInStyles.TableStyleMedium27:
                    return Color.FromArgb(255, 218, 238, 243);
                case TableBuiltInStyles.TableStyleMedium28:
                    return Color.FromArgb(255, 253, 233, 217);
                case TableBuiltInStyles.TableStyleMedium3:
                    return Color.FromArgb(255, 192, 80, 77);
                case TableBuiltInStyles.TableStyleMedium4:
                    return Color.FromArgb(255, 155, 187, 89);
                case TableBuiltInStyles.TableStyleMedium5:
                    return Color.FromArgb(255, 128, 100, 162);
                case TableBuiltInStyles.TableStyleMedium6:
                    return Color.FromArgb(255, 75, 172, 198);
                case TableBuiltInStyles.TableStyleMedium7:
                    return Color.FromArgb(255, 247, 150, 70);
                case TableBuiltInStyles.TableStyleMedium8:
                    return Color.FromArgb(255, 0, 0, 0);
                case TableBuiltInStyles.TableStyleMedium9:
                    return Color.FromArgb(255, 79, 129, 189);
                default:
                    return Color.FromArgb(255, 79, 129, 189);
            }
        }

        protected enum TableBorderStyle
        {
            /// <summary>
            /// Represents the No Solidstyle.
            /// </summary>
            None,

            /// <summary>
            /// Represents the solid style for the Top ExcelBorderIndex.
            /// </summary>
            Top,

            /// <summary>
            /// Represents the solid style for the Bottom ExcelBorderIndex.
            /// </summary>
            Bottom,

            /// <summary>
            /// Represents the solid style for the Right ExcelBorderIndex.
            /// </summary>
            Right,

            /// <summary>
            /// Represents the solid style for the Left ExcelBorderIndex.
            /// </summary>
            Left,

            /// <summary>
            /// Represents the solid style for both Top and Bottom ExcelBorderIndex.
            /// </summary>
            TopBottom,

            /// <summary>
            /// Represents the solid style for both Right and Left ExcelBorderIndex.
            /// </summary>
            LeftRight,

            /// <summary>
            /// Represents that all ExcelBorderIndex is set.
            /// </summary>
            All
        }
    }

    public static class RichTextBoxHelper
    {
#if !SILVERLIGHT
        public static class LogicalTreeUtility
        {
            public static IEnumerable GetChildren(DependencyObject obj, Boolean allChildrenInHierachy)
            {
                if (!allChildrenInHierachy)
                    return LogicalTreeHelper.GetChildren(obj);
                else
                {
                    List<object> ReturnValues = new List<object>();
                    RecursionReturnAllChildren(obj, ReturnValues);
                    return ReturnValues;
                }
            }
            private static void RecursionReturnAllChildren(DependencyObject obj, List<object> returnValues)
            {
                foreach (object curChild in LogicalTreeHelper.GetChildren(obj))
                {
                    returnValues.Add(curChild);
                    if (curChild is DependencyObject)
                        RecursionReturnAllChildren((DependencyObject)curChild, returnValues);
                }
            }
            public static IEnumerable<ReturnType> GetChildren<ReturnType>(DependencyObject obj, Boolean allChildrenInHierachy)
            {
                foreach (object child in GetChildren(obj, allChildrenInHierachy))
                    if (child is ReturnType)
                        yield return (ReturnType)child;
            }
        }
        private static void RecursionReturnAllChildren(DependencyObject obj, List<object> returnValues)
        {
            foreach (object curChild in LogicalTreeHelper.GetChildren(obj))
            {
                returnValues.Add(curChild);
                if (curChild is DependencyObject)
                    RecursionReturnAllChildren((DependencyObject)curChild, returnValues);
            }
        }
        public static void SetRichTextValue(IRange range, FlowDocument flowdocument, string formattedtext)
        {
            int NoOfParagraph = flowdocument.Blocks.Count;
            int startpoint = 0;
            int endpoint = 0;
            range.RichText.Text = formattedtext;
            var runs = LogicalTreeUtility.GetChildren<Run>(flowdocument, true);
            var underlines = LogicalTreeUtility.GetChildren<Underline>(flowdocument, true);
            foreach (Run run in runs)
            {
                startpoint = range.RichText.Text.IndexOf(run.Text);
                endpoint = startpoint + run.Text.Length;
                IFont font = range.Worksheet.Workbook.CreateFont();
                foreach (Underline underline in underlines)
                {
                    var line = LogicalTreeUtility.GetChildren<Run>(underline, true);
                    foreach (Run runline in line)
                    {
                        if (runline == run)
                        {
                            font.Underline = ExcelUnderline.Single;
                        }
                    }
                }
                if (endpoint >= range.RichText.Text.Count())
                    endpoint = range.RichText.Text.Count() - 1;
                range.RichText.SetFont(startpoint, endpoint, ConvertRunToIFont(run, font));
            }
        }
#endif
        /// <summary>
        /// UpdateExcelCellVaue()is a static method invoked to update the modified cell value to Excel sheet only for RichText Cell Type.
        /// </summary>
        /// <param name="range">Current Range value in the Excelsheet</param>
        /// <param name="paragraph">Modified cells value in the Grid as Paragraph</param>
        /// <param name="PlainText">Unformatted Text </param>      
        /// <remarks></remarks>
        public static void SetRichTextValue(IRange range, Paragraph paragraph, string PlainText)
        {
            int startpoint = 0;
            int endpoint = 0;
            range.RichText.Text = PlainText;
            foreach (var inline in paragraph.Inlines)
            {
                if (inline is Run)
                {
                    Run run = inline as Run;
                    startpoint = range.RichText.Text.IndexOf(run.Text);
                    endpoint = startpoint + run.Text.Length - 1;
                    IFont font = range.Worksheet.Workbook.CreateFont();
                    range.RichText.SetFont(startpoint, endpoint, ConvertRunToIFont(run, font));
                }
               
            }           
        }

        /// <summary>
        /// ConvertRunToIFont() method converts the formatting from Run to IFont. 
        /// </summary>
        /// <param name="run"></param>
        /// <param name="fontstyle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IFont ConvertRunToIFont(Run run, IFont fontstyle)
        {
            IFont charfont = fontstyle;
            if (run.FontWeight == FontWeights.Bold)
            {
                charfont.Bold = true;
            }
            if (run.FontStyle == FontStyles.Italic)
            {
                charfont.Italic = true;
            }
            if (run.TextDecorations == TextDecorations.Underline)
            {
                charfont.Underline = ExcelUnderline.Single;
            }
            charfont.FontName = run.FontFamily.ToString();         
            System.Windows.Media.SolidColorBrush solidbrush = run.Foreground as System.Windows.Media.SolidColorBrush;
#if !SILVERLIGHT
            charfont.RGBColor = System.Drawing.Color.FromArgb(solidbrush.Color.A, solidbrush.Color.R, solidbrush.Color.G, solidbrush.Color.B);
#else
            charfont.RGBColor = Color.FromArgb(solidbrush.Color.A, solidbrush.Color.R, solidbrush.Color.G, solidbrush.Color.B);
#endif
            charfont.Size = Convert.ToInt32(run.FontSize / 1.2);
            return charfont;
        }

        /// <summary>
        /// IsFormatSame() method validates the formatting for both the character is same or not. 
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="char2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsFormatSame(IFont char1, IFont char2)
        {
            bool canappend = true;
            if (char1.Bold != char2.Bold)
            {
                canappend = false;
            }
            if (char1.Color != char2.Color)
            {
                canappend = false;
            }
            if (char1.Italic != char2.Italic)
            {
                canappend = false;
            }
            if (char1.RGBColor != char2.RGBColor)
            {
                canappend = false;
            }
            if (char1.Size != char2.Size)
            {
                canappend = false;
            }
            if (char1.Underline != char2.Underline)
            {
                canappend = false;
            }
            if (char1.VerticalAlignment != char2.VerticalAlignment)
            {
                canappend = false;
            }
            return canappend;

        }

        /// <summary>
        /// AppendFormat() method Returns <param name="run"/> append the next character when both have same font format. 
        /// </summary>
        /// <param name="charfont"></param>
        /// <param name="run"></param>
        /// <param name="isnewline">If the character falls in new line set True, other wise False.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Run AppendFormat(IFont charfont, Run run, bool isnewline)
        {
            if (charfont.Bold)
            {
                run.FontWeight = FontWeights.Bold;
            }
            if (charfont.Italic)
            {
                run.FontStyle = FontStyles.Italic;
            }
            if (charfont.Underline == ExcelUnderline.Single || charfont.Underline == ExcelUnderline.Double)
            {
                run.TextDecorations = TextDecorations.Underline;
            }
            run.FontFamily = new FontFamily(charfont.FontName);           

            run.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, charfont.RGBColor.R, charfont.RGBColor.G, charfont.RGBColor.B));
            
            run.FontSize = charfont.Size * 1.2;
            if (isnewline)
                run.Text = run.Text + Environment.NewLine;
            return run;

        }

        /// <summary>
        /// ConvertIRangeToParagraph() method convert the IRange into Paragraph value. 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Paragraph ConvertIRangeToParagraph(IRichTextString RichText)
        {
            Paragraph paragraph = new Paragraph();
            int TextLength = RichText.Text.Length;
            bool IsLastChar = false;
            bool IsNewLine = false;
            StringBuilder sb = new StringBuilder();
            bool startchar = true;
            for (int i = 0; i < TextLength - 1; i++)
            {
                if (startchar)
                {
                    if (RichText.Text[i] == '\n')
                    {
                        paragraph.Inlines.Add(new LineBreak());
                        continue;

                    }
                    else
                    {
                        sb.Append(RichText.Text[i]);
                    }
                }
                if (i == TextLength - 2)
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                    IsLastChar = true;
                }
                if ((IsFormatSame(RichText.GetFont(i), RichText.GetFont(i + 1)) && !IsLastChar && RichText.Text[i + 1].ToString() != "\n"))
                {
                    sb.Append(RichText.Text[i + 1]);
                    startchar = false;
                }
                else
                {
                    if (RichText.Text[i + 1].ToString() == "\n")
                    {
                        IsNewLine = true;
                    }
                    else
                        IsNewLine = false;

                    Run run = new Run();
                    run.Text = sb.ToString();
                    paragraph.Inlines.Add(AppendFormat(RichText.GetFont(RichText.Text.IndexOf(run.Text)), run, IsNewLine));
                    if (IsNewLine)
                    {
                        paragraph.Inlines.Add(new LineBreak());
                    }

                    startchar = true;
#if SyncfusionFramework3_5 && !SyncfusionFramework4_0
                    sb.Remove(0, sb.Length - 1);
#else
                    sb.Clear();
#endif
                    if (RichText.Text[i + 1].ToString() == "\n" || IsLastChar)
                    {
                        startchar = true;
#if SyncfusionFramework3_5 && !SyncfusionFramework4_0
                        sb.Remove(0, sb.Length - 1);
#else
                        sb.Clear();
#endif
                        i++;
                        IsLastChar = false;
                    }
                }
            }
            return paragraph;
        }
    }

}
