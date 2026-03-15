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
    /// Custom event argument base class used for events associated with a <see cref="GridModel"/>. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModelEventArgs : SyncfusionRoutedEventArgs
    {
        GridModel gridModel;

        /// <summary>
        /// Initializes a new <see cref="GridModelEventArgs"/> object.
        /// </summary>
        /// <param name="model">Reference to the <see cref="GridModel"/>.</param>
        public GridModelEventArgs(GridModel model)
        {
            this.gridModel = model;
        }

        /// <summary>
        /// Reference to <see cref="GridModel"/>.
        /// </summary>
        public GridModel GridModel
        {
            get
            {
                return gridModel;
            }
        }
    }

}
