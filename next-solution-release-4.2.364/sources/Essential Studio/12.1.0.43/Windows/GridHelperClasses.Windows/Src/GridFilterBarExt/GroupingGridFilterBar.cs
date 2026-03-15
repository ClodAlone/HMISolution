//-------------------------------------------------------------------------------------------------
// <copyright file="GroupingGridFilterBar.cs" company="Syncfusion">
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
    using System.Text;
    using System.Data;
    using System.Collections;
    using System.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.GridHelperClasses;
    using Syncfusion.Grouping;
    using System.ComponentModel;

    /// <summary>
    /// Defines a custom filter bar for grouping grid that lets you filter the records by display member.
    /// </summary>
    public class GroupingGridFilterBarExt
    {
        GridGroupingControl grid;
        private bool allowIndividualColumnWiring = false;
       /// <summary>
       /// It allows to set the desired filter on specified column,if the value is set to True, 
       /// through which the filter can be set in column using 'this.gridGroupingControl1.TableDescriptor.Columns[ColumnName].Appearance.FilterBarCell.CellType ="FilterByDisplayMemberCell"'
       /// </summary>
       [DefaultValue(false)]
        public bool AllowIndividualColumnWiring
        {
            get { return allowIndividualColumnWiring; }
            set { allowIndividualColumnWiring = value; }
        }
      
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GroupingGridFilterBarExt()
        {
        }

        /// <summary>
        /// Associates the grid with this filter bar.
        /// </summary>
        /// <param name="grid">Grouping grid.</param>
        public void WireGrid(GridGroupingControl grid)
        {
            this.grid = grid;
            GridTableCellStyleInfo filterStyle;
            grid.TableModel.CellModels.Add("FilterByDisplayMemberCell", new GridFilterByDisplayMemberCellModel(new GridModel()));
            for (int i = 0; i < grid.TableDescriptor.Columns.Count; i++)
            {
                GridColumnDescriptor col = grid.TableDescriptor.Columns[i];
                if (col.AllowFilter)
                {
                    filterStyle = col.Appearance.FilterBarCell;
                    if (!AllowIndividualColumnWiring)
                        filterStyle.CellType = "FilterByDisplayMemberCell";
                    GridTableCellStyleInfo tableStyle = col.Appearance.AnyRecordFieldCell;
                    filterStyle.DataSource = tableStyle.DataSource;
                    filterStyle.DisplayMember = tableStyle.DisplayMember;
                    filterStyle.ValueMember = tableStyle.ValueMember;
                }
            }
        }

        /// <summary>
        /// Disassociates the grid with this filter bar.
        /// </summary>
        /// <param name="grid">Grouping grid.</param>
        public void UnwireGrid(GridGroupingControl grid)
        {
            grid.TableModel.CellModels.Remove("FilterByDisplayMemberCell");
        }
    }
}
