#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Provides a <see cref="CreateCellModel"/> method that instantiates <see cref="GridCellModelBase"/> 
    /// objects on demand when a cell is touched that has a <see cref="GridStyleInfo.CellType"/> that is 
    /// not found in the <see cref="GridModel.CellModels"/> collection.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridCellModelFactory
    {
        /// <summary>
        /// Instantiates <see cref="GridCellModelBase"/> objects on demand when a cell is touched that has a <see cref="GridStyleInfo.CellType"/> that is 
        /// not found in the <see cref="GridModel.CellModels"/> collection.
        /// </summary>
        GridCellModelBase CreateCellModel(string controlId, GridModel pGrid);

        /// <summary>
        /// Returns true when the grid is allowed to replace this factory with a derived factory at any time.
        /// </summary>
        bool IsDefault { get; }
    };
	
}
