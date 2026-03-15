//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundGridFilterBar.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;
    using System.Data;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Creates a custom filter bar that filters the grid data by display member.
    /// </summary>
    public class GridDataBoundGridFilterBarExt : GridFilterBar
    {
        Hashtable DisplayToValues = new Hashtable();
        private GridDataBoundGrid boundGrid = null;

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundGridFilterBarExt"/>
        /// </summary>
        public GridDataBoundGridFilterBarExt()
            : base()
        {
        }

        /// <summary>
        /// Swaps out the filter droplists for any combobox col with displaymember != valuemember and
        /// replace the default single item dropdown with a dropdown that handles both display and values.
        /// </summary>
        /// <param name="grid">Databound grid.</param>
        /// <param name="style">GridStyleInfo of the filter bar cell.</param>
        public override void WireGrid(Syncfusion.Windows.Forms.Grid.GridDataBoundGrid grid, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            boundGrid = grid;
            base.WireGrid(grid, style);

            GridBoundColumnsCollection gbcc;
            gbcc = (grid.GridBoundColumns.Count == 0) ? grid.Binder.InternalColumns : grid.GridBoundColumns;

            for (int gbcIndex = 0; gbcIndex < gbcc.Count; ++gbcIndex)
            {
                GridStyleInfo cellStyle = gbcc[gbcIndex].StyleInfo;

                if (cellStyle.CellType == "ComboBox" && cellStyle.DataSource != null &&
                    cellStyle.DisplayMember != cellStyle.ValueMember)
                {
                    string colName = gbcc[gbcIndex].MappingName;
                    int col = grid.Binder.NameToColIndex(colName);
                    int row = GetFilterRow();

                    DataView dv = null;

                    if (cellStyle.DataSource is DataTable)
                    {
                        DataTable dt = (DataTable)cellStyle.DataSource;
                        dv = new DataView(dt, string.Empty, string.Empty, DataViewRowState.CurrentRows);
                    }
                    else if (style.DataSource is DataView)
                    {
                        dv = (DataView)cellStyle.DataSource;
                    }

                    if (dv != null)
                    {
                        dv.Sort = cellStyle.DisplayMember;
                        grid[row, col].DataSource = this.CreateEntries(dv, cellStyle.DisplayMember, cellStyle.ValueMember);
                        grid[row, col].DisplayMember = cellStyle.DisplayMember;
                        grid[row, col].ValueMember = cellStyle.ValueMember;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a dropdown with both display and value members.Overriden from base filterbar.
        /// </summary>
        /// <param name="dv">Data view.</param>
        /// <param name="colName">Value member</param>
        /// <returns>The data table.</returns>
        protected override DataTable CreateUniqueEntries(DataView dv, string colName)
        {
            if (boundGrid != null && !string.IsNullOrEmpty(boundGrid.CurrentCell.Renderer.StyleInfo.DisplayMember))
            {
                GridStyleInfo cellStyle = boundGrid.CurrentCell.Renderer.StyleInfo;
                return cellStyle.DataSource as DataTable;
            }
            return base.CreateUniqueEntries(dv, colName);
        }

        /// <summary>
        /// Swaps display and value strings in the default filter.
        /// </summary>
        /// <param name="grid">Databound grid.</param>
        /// <returns>Filter string.</returns>
        protected override string GetFilterFromRow(GridDataBoundGrid grid)
        {
            string filter = base.GetFilterFromRow(grid);

            if (filter.Equals(string.Empty))
            {
                return filter;
            }

            GridBoundColumnsCollection gbcc;
            int row = this.GetFilterRow();

            gbcc = (grid.GridBoundColumns.Count == 0) ? grid.Binder.InternalColumns : grid.GridBoundColumns;

            string[] filterStrings = filter.Split(new string[] { " and " }, StringSplitOptions.None);
            for (int i = 0; i < filterStrings.Length; i++)
            {
                int startIndex = filterStrings[i].IndexOf('[') + 1;
                int endIndex = filterStrings[i].IndexOf(']');
                string fieldName = filterStrings[i].Substring(startIndex, endIndex - startIndex);
                int colIndex = grid.NameToColIndex(fieldName);
                GridStyleInfo style = gbcc[fieldName].StyleInfo;

                //// int col = grid.CurrentCell.ColIndex;

                if (style.CellType == "ComboBox" && style.DataSource != null &&
                    style.DisplayMember != style.ValueMember)
                {
                    string s = string.Empty;
                    string s1 = string.Empty;
                    if (grid.CurrentCell.ColIndex == colIndex)
                    {
                        s = "'" + grid.CurrentCell.Renderer.ControlText + "'";
                        s1 = "'" + grid.CurrentCell.Renderer.ControlValue.ToString() + "'";
                        if (!this.DisplayToValues.ContainsKey(s))
                        {
                            this.DisplayToValues.Add(s, s1);
                        }
                    }
                    else
                    {
                        s = "'" + grid[row, colIndex].CellValue.ToString() + "'";
                        s1 = this.DisplayToValues[s].ToString();
                    }

                    filter = filter.Replace(s, s1);
                }
            }

            return filter;
        }

        /// <summary>
        /// Creates a dropdown with both display and value members.
        /// </summary>
        /// <param name="dv">The Data View.</param>
        /// <param name="displayName">The Display Member.</param>
        /// <param name="valueName">The Value Member.</param> 
        /// <returns>Data Table.</returns>
        protected DataTable CreateEntries(DataView dv, string displayName, string valueName)
        {
            DataTable dt = new DataTable(displayName);
            dt.Columns.Add(new DataColumn(displayName));
            dt.Columns.Add(new DataColumn(valueName));
            DataRow dr = dt.NewRow();
            dr[0] = "(none)";
            dr[1] = "-2";
            dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr[0] = "(custom)";
            //dr[1] = "-1";
            //dt.Rows.Add(dr);

            string s = string.Empty;
            for (int i = 0; i < dv.Count; ++i)
            {
                if (s != dv[i].Row[displayName].ToString())
                {
                    s = dv[i].Row[displayName].ToString();
                    dr = dt.NewRow();
                    dr[0] = s;
                    dr[1] = dv[i].Row[valueName].ToString();
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
    }
}
