// <copyright file="TakeDragItemEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents event args for TakeDragItem handler.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TakeDragItemEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// Gets or sets the dragged item.
        /// </summary>
        /// <value>The dragged item.</value>
        public TabItemExt DragedItem
        {
            get;
            set;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes a new instance of the <see cref="TakeDragItemEventArgs"/> class.
        /// </summary>
        /// <param name="tabItemExt">The tab item.</param>
        public TakeDragItemEventArgs(TabItemExt tabItemExt)
        {
            DragedItem = tabItemExt;
        }
        #endregion
    }
}
