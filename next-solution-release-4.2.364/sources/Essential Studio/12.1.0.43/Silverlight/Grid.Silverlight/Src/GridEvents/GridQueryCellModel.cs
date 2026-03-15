#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCellModel"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">A <see cref="GridQueryCellModelEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCellModelEventHandler(object sender, GridQueryCellModelEventArgs e);


    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCellModel"/> event.
    /// </summary>
    /// <remarks>
    /// The GridQueryCellModelEventArgs is a custom event argument class used by the 
    /// <see cref="GridModel.QueryCellModel"/> event for querying the <see cref="GridCellModelBase"/>
    /// based on a string cellType.
    /// <para/>
    /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
    /// a new cell type that it cannot find in the table, it will raise a <see cref="GridModel.QueryCellModel"/> event.
    /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The 
    /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
    /// associated cell object. This object will be stored in the table together with its name and
    /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
    /// <para/>
    /// You should process this event if you want to add custom cell types and initialize these
    /// cell types on demand when associated cells are accessed the first time.
    /// </remarks>
    /// <seealso cref="GridQueryCellModelEventHandler"/>
    /// <seealso cref="GridModel.QueryCellModel"/> 
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridQueryCellModelEventArgs : GridModelEventArgs
    {
        string cellType;
        GridCellModelBase cellModel;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="cellType">The cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.</param>
        public GridQueryCellModelEventArgs(GridModel gridModel, string cellType)
            : base(gridModel)
        {
            this.cellType = cellType;
            this.cellModel = null;
        }


        /// <summary>
        /// The cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.
        /// </summary>
        public string CellType
        {
            get
            {
                return cellType;
            }
        }

        /// <summary>
        /// The <see cref="GridCellModelBase"/> for the cell type. You should create a new instance
        /// of the specific cell model and save it to this property.
        /// </summary>
        public GridCellModelBase CellModel
        {
            get
            {
                return cellModel;
            }
            set
            {
                cellModel = value;
            }
        }
    }

}
