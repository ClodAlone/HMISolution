// <copyright file="BeforeLabelEditEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Before Label Edit EventArgs class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BeforeLabelEditEventArgs : EventArgs
    {

        #region Public properies
        /// <summary>
        /// Gets or sets the header before edit.
        /// </summary>
        /// <value>The header before edit.</value>
        public object HeaderBeforeEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tab item.
        /// </summary>
        /// <value>The tab item.</value>
        public TabItemExt TabItem
        {
            get;
            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeLabelEditEventArgs"/> class.
        /// </summary>
        /// <param name="headerBeforeEdit">Value for HeaderBeforeEdit</param>
        public BeforeLabelEditEventArgs(TabItemExt tabitem)
        {
            HeaderBeforeEdit = tabitem.Header;
            TabItem = tabitem;
        }
        #endregion
    }
}
