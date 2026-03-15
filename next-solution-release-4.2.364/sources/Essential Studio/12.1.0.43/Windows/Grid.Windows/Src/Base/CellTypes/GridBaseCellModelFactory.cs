//-------------------------------------------------------------------------------------------------
// <copyright file="GridBaseCellModelFactory.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridCellModelFactory creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
    /// </summary>
    public class GridBaseCellModelFactory: IGridCellModelFactory
    {
        /// <summary>
        /// Initializes a <see cref="GridBaseCellModelFactory"/>
        /// </summary>
        public GridBaseCellModelFactory()
        {
            this.isDefault = false;
        }

        /// <summary>
        /// Initializes a <see cref="GridBaseCellModelFactory"/> and optionally marks it as "Default",
        /// allowing the grid to replace it with a derived factory at any time.
        /// </summary>
        /// <param name="isDefault">When True, marks this instance of <see cref="GridBaseCellModelFactory"/> as Default.</param>
        public GridBaseCellModelFactory(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        bool isDefault;

        /// <summary>
        /// Gets a value indicating whether the grid is allowed to replace this factory with a derived factory at any time.
        /// </summary>
        public virtual bool IsDefault
        {
            get
            {
                return this.isDefault;
            }
        }

        /// <summary>
        /// Creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="cellTypeName">A cell type name that identifies the cell model to be instantiated.</param>
        /// <param name="pGrid">The <see cref="GridModel"/> the new cell model object should be associated with.</param>
        /// <returns>A cell model.</returns>
        public virtual GridCellModelBase CreateCellModel(string cellTypeName, GridModel pGrid)
        {
            switch (cellTypeName)
            {
                case "Header":
                    return new GridHeaderCellModel(pGrid);

                case "Static":
                    return new GridStaticCellModel(pGrid);

                case "TextBox":
                    return new GridTextBoxCellModel(pGrid);

                case "ComboBox":
                    return new GridComboBoxCellModel(pGrid);

                case "PushButton":
                    return new GridPushButtonCellModel(pGrid);

                default:
                    return new GridStaticCellModel(pGrid);
            }
        }
    }
}
