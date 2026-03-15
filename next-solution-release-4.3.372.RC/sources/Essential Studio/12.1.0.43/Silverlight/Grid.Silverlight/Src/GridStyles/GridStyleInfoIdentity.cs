#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Text;

#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Fields
        GridVolatileCellStyles data;
        bool offLine;
        RowColumnIndex cellRowColumnIndex;

        // Cache.
        IStyleInfo[] cachedBaseStyles = null;

        #endregion

        #region Ctor
        /// <overload>
        /// Initializes a <see cref="GridStyleInfoIdentity"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="VolatileData"/>, row, and column index.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/>.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        public GridStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex)
        {
            this.data = data;
            this.cellRowColumnIndex = new RowColumnIndex(rowIndex, colIndex);
            this.offLine = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="VolatileData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/>.</param>
        /// <param name="pos">Cell coordinates.</param>
        public GridStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos)
        {
            this.data = data;
            this.cellRowColumnIndex = pos;
            this.offLine = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="VolatileData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/></param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="offLine">True if changes in this style object should not be stored in the associated <see cref="VolatileData"/>.</param>
        public GridStyleInfoIdentity(GridVolatileCellStyles data, int rowIndex, int colIndex, bool offLine)
        {
            this.data = data;
            this.cellRowColumnIndex = new RowColumnIndex(rowIndex, colIndex);
            this.offLine = offLine;

            // GridStyleInfoIdentity implements a finalizer that is not needed when object is offLine.
            // Therefore, we call GC.SupressFinalize to
            // immediately take this object off the finalization queue
            // and prevent finalization code for this object.
            if (offLine)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="VolatileData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/>.</param>
        /// <param name="pos">Cell coordinates.</param>
        /// <param name="offLine">True if changes in this style object should not be stored in the associated <see cref="VolatileData"/>.</param>
        public GridStyleInfoIdentity(GridVolatileCellStyles data, RowColumnIndex pos, bool offLine)
        {
            this.data = data;
            this.cellRowColumnIndex = pos;
            this.offLine = offLine;

            // GridStyleInfoIdentity implements a finalizer that is not needed when an object is offLine.
            // Therefore, we call GC.SupressFinalize to
            // immediately take this object off the finalization queue
            // and prevent finalization code for this object.
            if (offLine)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridStyleInfoIdentity(GridStyleInfoIdentity other)
        {
            this.data = other.data;
            this.cellRowColumnIndex = other.cellRowColumnIndex;

            // GridStyleInfoIdentity implements a finalizer that is not needed when object is offLine.
            // Therefore, we call GC.SupressFinalize to
            // immediately take this object off the finalization queue
            // and prevent finalization code for this object.
            if (offLine)
                GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Removes the associated cell cache object from the volatile data store.
        /// </summary>
        ~GridStyleInfoIdentity()
        {
            if (!offLine && data != null)
                data.ResetItem(cellRowColumnIndex);
        }

        /// <override/>
        public override void Dispose()
        {
            if (!offLine && data != null)
                data.ResetItem(cellRowColumnIndex);
            data = null;
            cachedBaseStyles = null;
            base.Dispose(); // will call GC.SupressFinalize
        }

        #endregion

        #region Offline
        /// <summary>
        /// True if changes in this style object should not be stored in the associated <see cref="VolatileData"/>.
        /// </summary>
        public virtual bool OffLine
        {
            get
            {
                return offLine;
            }
        }


        /// <summary>
        /// Creates a new <see cref="GridStyleInfoIdentity"/> and copies its identity information from the current object. The new
        /// instance will be detached from <see cref="VolatileData"/> so that changes in this style object are not be stored in the associated <see cref="VolatileData"/>.
        /// </summary>
        /// <returns>A new <see cref="GridStyleInfoIdentity"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        public GridStyleInfoIdentity MakeOfflineIdentity()
        {
            return new GridStyleInfoIdentity(data, cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, true);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Reference to <see cref="VolatileData"/>.
        /// </summary>
        public GridVolatileCellStyles Data
        {

            get { return data; }
        }

        public GridModel GridModel
        {
            get { return (GridModel)data.Host; }
        }

        /// <summary>
        /// The row index.
        /// </summary>
        public int RowIndex
        {

            get { return cellRowColumnIndex.RowIndex; }
        }

        /// <summary>
        /// The column index.
        /// </summary>
        public int ColumnIndex
        {

            get { return cellRowColumnIndex.ColumnIndex; }
        }

        /// <summary>
        /// The cell coordinates.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        public void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            this.cellRowColumnIndex = cellRowColumnIndex; 
        }

        #endregion

        #region BaseStyles

        /// <summary>
        /// Overriden. Returns base styles from <see cref="VolatileData"/> by calling <see cref="VolatileData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null && data != null && data.Host != null)
                cachedBaseStyles = data.Host.QueryBaseStyles(CellRowColumnIndex, (GridStyleInfo) thisStyleInfo);
            return cachedBaseStyles;
        }

        /// <exclude/>
        public void ResetBaseStylesCache()
        {
            cachedBaseStyles = null;
        }

        #endregion

        /// <summary>
        /// Overridden. If the style is not offline, saves its changes in the <see cref="VolatileData"/>.
        /// </summary>
        /// <param name="style">A reference to the <see cref="GridStyleInfo"/> object.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> that identifies the changed style property.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            // Make style permanent in GridData.
            if (!offLine && data != null)
            {
                data.CommitStyle(CellRowColumnIndex, (GridStyleInfo)style, sip);
            }
            cachedBaseStyles = null;
            base.OnStyleChanged(style, sip);
        }


        /// <summary>
        /// Returns a <see cref="GridCellModelBase"/> for the specified id / cell type name.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>The <see cref="GridCellModelBase"/> for the given id.</returns>
        /// <remarks>
        /// Calls <see cref="IGridData.LookupCellModel"/>.
        /// </remarks>
        public virtual GridCellModelBase LookupCellModel(string id)
        {
            return data.LookupCellModel(id);
        }


        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.Append("rowIndex = ");
            sb.Append(RowIndex.ToString());
            sb.Append(", colIndex = ");
            sb.Append(ColumnIndex.ToString());
            sb.Append(" }");
            return sb.ToString();
        }

    }
}
