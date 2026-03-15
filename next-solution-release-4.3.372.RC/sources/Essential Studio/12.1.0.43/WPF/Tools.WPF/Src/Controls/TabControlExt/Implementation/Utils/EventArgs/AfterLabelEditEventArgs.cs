// <copyright file="AfterLabelEditEventArgs.cs" company="Syncfusion">
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
    /// Represent the class for AfterLabelEditEventArgs
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AfterLabelEditEventArgs:EventArgs
    {

        #region Public properies
        /// <summary>
        /// Gets or sets the header after edit.
        /// </summary>
        /// <value>The header after edit.</value>
        public object HeaderAfterEdit
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
        /// Initializes a new instance of the <see cref="AfterLabelEditEventArgs"/> class.
        /// </summary>
        /// <param name="headerAfterEdit">Value for HeaderAfterEdit</param>
        public AfterLabelEditEventArgs(TabItemExt tabitem)
        {
            HeaderAfterEdit = tabitem.Header;
            TabItem = tabitem;
        }
        #endregion
    }
}
