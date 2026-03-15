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
    /// HistoryEntry consists of Command to execute and reference to proceeding and preceeding history entries. 
    /// </summary>
    /// <remarks>
    /// Used in HistoryList class.
    /// </remarks>
    public class HistoryEntry
    {
        #region Class members
        private HistoryEntry m_nodeNext;
        private HistoryEntry m_nodePrevious;
        private ICommand m_command;
        #endregion

        #region Class initilize/finalize methods
        public HistoryEntry(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command", "Command can not be empty");

            m_command = command;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets history entry.
        /// </summary>
        public ICommand Command
        {
            get { return m_command; }
        }

        /// <summary>
        /// Gets or sets next history node.
        /// </summary>
        public HistoryEntry Next
        {
            get
            {
                return m_nodeNext;
            }
            set
            {
                if (value == m_nodeNext)
                    throw new ArgumentException("Next", "Node can not proceed to itself.");

                m_nodeNext = value;
            }
        }

        /// <summary>
        /// Gets or sets previous history node.
        /// </summary>
        public HistoryEntry Previous
        {
            get
            {
                return m_nodePrevious;
            }
            set
            {
                if (value == m_nodePrevious)
                    throw new ArgumentException("Previous", "Node can not preceed to itself.");

                m_nodePrevious = value;
            }
        }
        #endregion
    }
}