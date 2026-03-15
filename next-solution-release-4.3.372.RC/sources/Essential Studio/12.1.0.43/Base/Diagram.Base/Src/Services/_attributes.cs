#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A custom attribute to allow a target methods( event handler ) execute event 
    /// handlers event if DocumentEventSink is stopped.
    /// </summary>
    public class EventHandlerPriorityAttribute
        : Attribute
    {
        #region Class members
        private bool m_bExecuteAnyway;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerPriorityAttribute"/> class.
        /// </summary>
        /// <param name="bExecuteAnyway">if set to <c>true</c> to execute anyway.</param>
        public EventHandlerPriorityAttribute(bool bExecuteAnyway)
        {
            m_bExecuteAnyway = bExecuteAnyway;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether [execute regardless of service state].
        /// </summary>
        /// <value>
        /// <c>true</c> if need to execute regardless of service state; otherwise, <c>false</c>.
        /// </value>
        public bool ExecuteRegardlessOfServiceState
        {
            get { return m_bExecuteAnyway; }
            set { m_bExecuteAnyway = value; }
        }
        #endregion
    }
}
