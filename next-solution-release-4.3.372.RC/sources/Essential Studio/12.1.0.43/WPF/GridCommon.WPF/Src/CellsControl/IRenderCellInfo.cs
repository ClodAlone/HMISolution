#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{

    /// <summary>
    /// Defines the border side.
    /// </summary>
    public enum CellBorderSide
    {
        /// <summary>
        /// At top
        /// </summary>
        Top,
        /// <summary>
        /// At left side
        /// </summary>
        Left,
        /// <summary>
        /// At bottom
        /// </summary>
        Bottom,
        /// <summary>
        /// At right side
        /// </summary>
        Right
    }

    /// <summary>
    /// Defines cell render style information.
    /// </summary>
    public interface IRenderCellInfo
    {
        /// <summary>
        /// Gets the cell background.
        /// </summary>
        /// <returns></returns>
        object GetCellBackground();

        /// <summary>
        /// Determines whether this cell can combine the cell background with the specified other cell.
        /// </summary>
        /// <param name="other">The other cell.</param>
        /// <returns>
        /// 	<c>true</c> if this cell can combine the cell background with the specified other cell; otherwise, <c>false</c>.
        /// </returns>
        bool CanCombineCellBackground(IRenderCellInfo other);

        /// <summary>
        /// Gets the cell border.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        object GetCellBorder(CellBorderSide side);

        /// <summary>
        /// Determines whether this cell can combine the cells border with the the specified other cell.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="other">The other cell.</param>
        /// <returns>
        /// 	<c>true</c> if this cell can combine the cells border with the the specified other cell; otherwise, <c>false</c>.
        /// </returns>
        bool CanCombineCellBorder(CellBorderSide side, IRenderCellInfo other);

        /// <summary>
        /// Gets the border margins.
        /// </summary>
        /// <returns></returns>
        Thickness GetBorderMargins();

        /// <summary>
        /// Gets the padding.
        /// </summary>
        /// <returns></returns>
        Thickness GetPadding();
    }


}
