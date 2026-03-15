// <copyright file="SelectedItemChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class represents the Selected Item Changed Event
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SelectedItemChangedEventArgs
    {
        #region Private members

        /// <summary>
        /// Local variable which represents cancel flag
        /// </summary>
        bool m_Cancel = false;
        #endregion

        #region Public properies
        /// <summary>
        /// Gets or sets the old selected item.
        /// </summary>
        /// <value>The old selected item.</value>
        public TabItemExt OldSelectedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new selected item.
        /// </summary>
        /// <value>The new selected item.</value>
        public TabItemExt NewSelectedItem
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets the Cancel of Selected TabItem
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_Cancel;
            }
            set
            {
                m_Cancel = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldSelectedItem">The old selected item.</param>
        /// <param name="newSelectedItem">The new selected item.</param>
        public SelectedItemChangedEventArgs(TabItemExt oldSelectedItem, TabItemExt newSelectedItem)
        {
            OldSelectedItem = oldSelectedItem;
            NewSelectedItem = newSelectedItem;
        }
        #endregion
    }
}
