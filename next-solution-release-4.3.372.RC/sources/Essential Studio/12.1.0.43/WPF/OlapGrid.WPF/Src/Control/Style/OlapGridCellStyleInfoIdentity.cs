#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using Syncfusion.Olap.Engine;
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System.Windows;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    
    /// <summary>
    /// Custom type for defining the cell style type identity
    /// </summary>
    public class OlapGridCellStyleInfoIdentity : GridStyleInfoIdentity
    {
        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridCellStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="offline">if set to <c>true</c> [offline].</param>
        public OlapGridCellStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex, bool offline)
            : base(data, rowIndex, colIndex, offline)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridCellStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="offline">if set to <c>true</c> [offline].</param>
        public OlapGridCellStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos, bool offline)
            : base(data, pos, offline)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridCellStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/>.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <overload>
        /// Initializes a <see cref="GridStyleInfoIdentity"/>.
        /// </overload>
        public OlapGridCellStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex)
            : base(data, rowIndex, colIndex)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridCellStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="pos">The pos.</param>
        public OlapGridCellStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos)
            : base(data, pos)
        {
            this.Data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cell descriptor.
        /// </summary>
        /// <value>The cell descriptor.</value>
        public PivotCellDescriptor CellDescriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Reference to <see cref="VolatileData"/>.
        /// </summary>
        /// <value></value>
        public new GridVolatileCellStyles Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hyperlink cell.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hyperlink cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsHyperlinkCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public Style Style 
        { 
            get; 
            set; 
        }

        #endregion
    }
}
