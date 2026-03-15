// <copyright file="BeforeGroupBarItemPopupOpenedEventArgs.cs" company="Syncfusion">
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
using Syncfusion.Windows.Tools.Controls;
using System.Windows;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Class Represents the before Group bar item popup open event
    /// </summary>
    public class BeforeGroupBarItemPopupOpenedEventArgs
    {
        #region Class members
        /// <summary>
        /// The <see cref="GroupBarItem"/> to display in popUp.
        /// </summary>
        private GroupBarItem m_groupBarItem;
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets the <see cref="GroupBarItem"/> that should be displayed in popUp.
        /// </summary>
        /// <value>
        /// Type: <see cref="GroupBarItem"/>
        /// </value>
        /// <seealso cref="GroupBarItem"/>
        public GroupBarItem To
        {
            get
            {
                return m_groupBarItem;
            }

            internal set
            {
                m_groupBarItem = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the BeforeGroupBarItemPopupOpenedEventArgs class.
        /// </summary>
        /// <param name="barItem">The <see cref="GroupBarItem"/> to display in popUp.</param>
        public BeforeGroupBarItemPopupOpenedEventArgs(GroupBarItem barItem)
        {
            m_groupBarItem = barItem;
        }
        #endregion
    }

    /// <summary>
    /// Class represents the Group bar Context menu item Click event.
    /// </summary>
    public class GroupBarContextMenuItemEventArgs : RoutedEventArgs
    {
        #region Class Initialization
        /// <summary>
        /// Initializes the new instance of <see cref="GroupBarContextMenuItemEventArgs"/>.
        /// </summary>
        public GroupBarContextMenuItemEventArgs()
        {
        }
        #endregion

        #region Class Members
        /// <summary>
        /// The Context menu item that is clicked.
        /// </summary>
        private object menuItem = null;
        #endregion

        #region Class Properties
        /// <summary>
        /// Get the Menu item which is clicked.
        /// </summary>
        public object MenuItem
        {
            get
            {
                return menuItem;
            }
            internal set
            {
                menuItem = value;
            }
        }
        #endregion
    }
}
