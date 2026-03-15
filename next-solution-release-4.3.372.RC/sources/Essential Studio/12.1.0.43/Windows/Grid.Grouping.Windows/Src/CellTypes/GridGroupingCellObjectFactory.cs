//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingCellObjectFactory.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// GridCellModelFactory creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
    /// </summary>
    public class GridGroupingCellObjectFactory: GridCellModelFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellModelFactory"/> class.
        /// </summary>
        public GridGroupingCellObjectFactory()
        {
        }

        /// <summary>
        /// Creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="cellTypeName">A cell type name that identifies the cell model to be instantiated.</param>
        /// <param name="pGrid">The <see cref="GridModel"/> the new cell model object should be associated with.</param>
        /// <returns>returns GridCellModelBase</returns>
        public override GridCellModelBase CreateCellModel(string cellTypeName, GridModel pGrid)
        {
            switch (cellTypeName)
            {
                case "RowHeaderCell":
                    return new GridTableRowHeaderCellModel(pGrid);

                case "StackedHeaderCell":
                case "ColumnHeaderCell":
                    return new GridSortColumnHeaderCellModel(pGrid);

                case "FilterBarCell":
                    if (pGrid.EnableLegacyStyle)
                    {
                        return new GridTableFilterBarCellModel(pGrid);
                    }
                    else
                    {
                        return new GridTableFilterBarGridListCellModel(pGrid, true);
                    }

                case "ForeignKeyCell":
                    return new GridTableDropDownListCellModel(pGrid);

                default:
                    return base.CreateCellModel(cellTypeName, pGrid);
            }
        }
    }
}
