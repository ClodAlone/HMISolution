// <copyright file="LayoutChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Specifies state information and event data associated with <see cref="Syncfusion.Windows.Tools.Controls.GroupBar.OrientationChangedEvent"/> routed event.
    /// </summary>
    public class OrientationChangeEventArgs : RoutedEventArgs
    {
        #region Class members
        /// <summary>
        /// New orientation.
        /// </summary>
        private Orientation m_to;
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets new orientation.
        /// </summary>
        /// <value>
        /// Type: <see cref="Orientation"/>
        /// </value>
        /// <seealso cref="Orientation"/>
        public Orientation To
        {
            get
            {
                return m_to;
            }

            internal set
            {
                m_to = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrientationChangeEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public OrientationChangeEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }
        #endregion
    }
}
