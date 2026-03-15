// <copyright file="CloseButtonEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the close button event args
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CloseButtonEventArgs : EventArgs
    {
        #region Private members
        /// <summary>
        /// Represents UIElement target item
        /// </summary>
        private UIElement m_targetItem;

        /// <summary>
        /// Reoresents cancel flag
        /// </summary>
        private bool m_Cancel = false;
        #endregion

        #region Public properies
        /// <summary>
        /// Gets or sets the target item.
        /// </summary>
        /// <value>The target item.</value>
        public UIElement TargetItem
        {
            get
            {
                return m_targetItem;
            }

            set
            {
                m_targetItem = value;
            }
        }
        /// <summary>
        /// Gets or Sets the Cancel value
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
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
        /// Initializes a new instance of the <see cref="CloseButtonEventArgs"/> class.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        public CloseButtonEventArgs(UIElement targetItem)
        {
            m_targetItem = targetItem;
        }
        #endregion
    }
}
