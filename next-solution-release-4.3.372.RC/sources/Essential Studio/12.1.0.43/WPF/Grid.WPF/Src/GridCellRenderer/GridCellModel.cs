#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Defines the model part of a cell type.
    /// </summary>
    /// <typeparam name="T">The cell renderer type.</typeparam>
    public class GridCellModel<T> : GridCellModelBase 
        where T : IGridCellRenderer, new()
    {
        /// <summary>
        /// Creates the appropriate cell renderer.
        /// </summary>
        /// <returns>The cell renderer.</returns>
        public override IGridCellRenderer CreateRenderer()
        {
            IGridCellRenderer r = new T();
            r.RaiseCreated(this);
            return r;
        }
    }
}
