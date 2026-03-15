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
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicStyleInfoIdentity : StyleInfoIdentityBase
    {

        #region Fields
        GraphicVolatileCellStyles data;
        bool offLine;
        int Index;

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
        public GraphicStyleInfoIdentity(GraphicVolatileCellStyles data, int index)
        {
            this.data = data;
            this.Index = index;
            this.offLine = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="VolatileData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="VolatileData"/></param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="offLine">True if changes in this style object should not be stored in the associated <see cref="VolatileData"/>.</param>
        public GraphicStyleInfoIdentity(GraphicVolatileCellStyles data, int index, bool offLine)
        {
            this.data = data;
            this.Index = index;
            this.offLine = offLine;

            // GridStyleInfoIdentity implements a finalize that is not needed when object is offLine.
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
        protected GraphicStyleInfoIdentity(GraphicStyleInfoIdentity other)
        {
            this.data = other.data;
            this.Index = other.Index;

            // GridStyleInfoIdentity implements a finalize that is not needed when object is offLine.
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
        ~GraphicStyleInfoIdentity()
        {
            if (!offLine && data != null)
                data.ResetItem(Index);
        }

        /// <override/>
        /// <summary>Releases all the resource used by this component.</summary>
        public override void Dispose()
        {
            if (!offLine && data != null)
                data.ResetItem(Index);
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
        public GraphicStyleInfoIdentity MakeOfflineIdentity()
        {
            return new GraphicStyleInfoIdentity(data, Index, true);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Reference to <see cref="VolatileData"/>.
        /// </summary>
        public GraphicVolatileCellStyles Data
        {

            get { return data; }
        }

        /// <summary>
        /// Returns the grid model.
        /// </summary>
        public GraphicModel GridModel
        {
            get { return (GraphicModel)data.Host; }
        }

        

        /// <summary>
        /// The cell coordinates.
        /// </summary>
        public int CellIndex
        {
            get { return Index; }
        }

        /// <summary>
        /// Updates the cell row column index.
        /// </summary>
        /// <param name="cellRowColumnIndex">The <see cref="RowColumnIndex"/>.</param>
        public void UpdateCellRowColumnIndex(int index)
        {
            this.Index = index;
        }

        #endregion

        #region BaseStyles

        /// <summary>
        /// Overridden. Returns base styles from <see cref="VolatileData"/> by calling <see cref="VolatileData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null && data != null && data.Host != null)
                cachedBaseStyles = data.Host.QueryBaseGraphicStyles(Index, (GraphicStyleInfo)thisStyleInfo);
            return cachedBaseStyles;
        }

        /// <exclude/>
        /// <summary>For internal use.</summary>
        public void ResetBaseStylesCache()
        {
            cachedBaseStyles = null;
        }

        #endregion

        /// <summary>
        /// Overridden. If the style is not off line, saves its changes in the <see cref="VolatileData"/>.
        /// </summary>
        /// <param name="style">A reference to the <see cref="GridStyleInfo"/> object.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> that identifies the changed style property.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            // Make style permanent in GridData.
            if (!offLine && data != null)
            {
                data.CommitStyle(Index, (GraphicStyleInfo)style, sip);
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
        public virtual GraphicCellModelBase LookupCellModel(string id)
        {
            return data.LookupCellModel(id);
        }


        /// <override/>
        /// <summary>Returns a string representation of the current object.</summary>
        /// <returns>String equivalent of the current object.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.Append("Index = ");
            sb.Append(Index.ToString());
            sb.Append(" }");
            return sb.ToString();
        }

    }
}
