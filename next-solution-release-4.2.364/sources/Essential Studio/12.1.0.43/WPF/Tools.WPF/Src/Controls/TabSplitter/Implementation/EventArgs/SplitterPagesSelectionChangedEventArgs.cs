// <copyright file="SplitterPagesSelectionChangedEventArgs.cs" company="Syncfusion">
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
    /// Presents event arguments for SelectionChanged event.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitterPagesSelectionChangedEventArgs : EventArgs
    {
        #region Private members
        /// <summary>
        /// Presents the new selected page.
        /// </summary>
        private readonly SplitterPage m_NewSelectedPage;
        
        /// <summary>
        /// Presents the old selected page.
        /// </summary>
        private readonly SplitterPage m_OldSelectedPage;
        #endregion

        #region Public properies
        /// <summary>
        /// Gets the new selected page.
        /// </summary>
        /// <value>The new selected page.</value>
        public SplitterPage NewSelectedPage
        {
            get
            {
                return m_NewSelectedPage;
            }
        }
        
        /// <summary>
        /// Gets the old selected page.
        /// </summary>
        /// <value>The old selected page.</value>
        public SplitterPage OldSelectedPage
        {
            get
            {
                return m_OldSelectedPage;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitterPagesSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldSelectedPage">The old selected page.</param>
        /// <param name="newSelectedPage">The new selected page.</param>
        public SplitterPagesSelectionChangedEventArgs(SplitterPage oldSelectedPage, SplitterPage newSelectedPage)
        {
            m_OldSelectedPage = oldSelectedPage;
            m_NewSelectedPage = newSelectedPage;
        }
        #endregion
    }
}