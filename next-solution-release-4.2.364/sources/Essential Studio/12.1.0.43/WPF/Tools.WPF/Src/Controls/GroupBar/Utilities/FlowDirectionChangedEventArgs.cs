// <copyright file="FlowDirectionChangedEventArgs.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Contains state information and event data associated with <see cref="Syncfusion.Windows.Tools.Controls.GroupBar.FlowDirectionChangedEvent"/> routed event.
    /// </summary>
    public class FlowDirectionChangedEventArgs : RoutedEventArgs
    {
        #region Class members
        /// <summary>
        /// New flow direction.
        /// </summary>
        private FlowDirection m_to;
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets new flow direction.
        /// </summary>
        /// <value>
        /// Type: <see cref="FlowDirection"/>
        /// </value>
        /// <seealso cref="FlowDirection"/>
        public FlowDirection To
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
        /// Initializes a new instance of the <see cref="FlowDirectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public FlowDirectionChangedEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }
        #endregion
    }
}
