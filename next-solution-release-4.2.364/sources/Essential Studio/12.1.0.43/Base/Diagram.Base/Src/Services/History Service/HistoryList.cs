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
    /// HistoryList is LinearLinkedList containing HistoryEntries. Each HistoryEntry contains Command to execute
    /// </summary>
    public class HistoryList
    {
        #region fields
        protected bool m_bCanRedo = false;
        protected bool m_bCanUndo = false;
        protected HistoryEntry m_nodeCurrent;
        #endregion

        #region public properties
        /// <summary>
        /// Gets a value indicating whether there is HistoryEntry to Undo.
        /// </summary>
        public bool CanUndo
        {
            get { return m_bCanUndo; }
        }

        /// <summary>
        /// Gets a value indicating whether there is HistoryEntry to Redo.
        /// </summary>
        public bool CanRedo
        {
            get { return m_bCanRedo; }
        }
        #endregion

        #region public methods
        public bool CanMerge(ICommand cmdMerging)
        {
            return m_nodeCurrent.Command.CanMerge(cmdMerging);
        }
        public void Merge(ICommand cmdToMerge)
        {
            m_nodeCurrent.Command.Merge(cmdToMerge);
        }

        /// <summary>
        /// Fetches for HistoryEntry to Redo.
        /// </summary>
        /// <returns>The command.</returns>
        public ICommand GetRedoCmd()
        {
            ICommand cmdToReturn = null;
            HistoryEntry entryCurrent;

            if (m_bCanRedo)
            {
                if ((m_nodeCurrent.Previous == null) && (!m_bCanUndo))
                    entryCurrent = m_nodeCurrent;
                else
                    entryCurrent = m_nodeCurrent.Next;

                if (entryCurrent != null)
                {
                    // Update Current HistoryEntry.
                    m_nodeCurrent = entryCurrent;

                    if (!m_bCanUndo)
                        m_bCanUndo = true;

                    if (entryCurrent.Next == null)
                    {
                        // No more cmd to Redo.
                        m_bCanRedo = false;
                        m_bCanUndo = true;
                    }
                }

                cmdToReturn = m_nodeCurrent.Command;
            }
            return cmdToReturn;
        }
        
        /// <summary>
        /// Fetches HistoryEntry to Undo.
        /// </summary>
        /// <returns>The command.</returns>
        public ICommand GetUndoCmd()
        {
            ICommand cmdToReturn = null;
            HistoryEntry entryCurrent;

            if (m_bCanUndo)
            {
                cmdToReturn = m_nodeCurrent.Command;
                entryCurrent = m_nodeCurrent.Previous;

                if (entryCurrent != null)
                {
                    // Update Current HistoryEntry.
                    m_nodeCurrent = entryCurrent;

                    if (!m_bCanRedo)
                        m_bCanRedo = true;
                }
                else
                {
                    // No more HistoryEntries to Undo.                   
                    m_bCanUndo = false;
                    m_bCanRedo = true;
                }
            }

            return cmdToReturn;
        }

        /// <summary>
        /// Clears HistoryList.
        /// </summary>
        public void ClearHistory()
        {
            m_nodeCurrent = null;
            m_bCanUndo = false;
            m_bCanRedo = false;
        }

       
        /// <summary>
        /// Adds new HistoryEntry to Historylist.
        /// </summary>
        /// <param name="command">The command.</param>
        public void AddEntry(ICommand command)
        {
            HistoryEntry entryNew = new HistoryEntry(command);

            if (m_nodeCurrent != null)
            {
                if (m_nodeCurrent.Next != null)
                {
                    if (m_nodeCurrent.Previous != null)
                    {
                        HistoryEntry entry = m_nodeCurrent.Next;
                        entry.Previous = null;

                        m_nodeCurrent.Next = entryNew;
                        entryNew.Previous = m_nodeCurrent;
                    }
                }
                else
                {
                    if (m_bCanUndo)
                    {
                        m_nodeCurrent.Next = entryNew;
                        entryNew.Previous = m_nodeCurrent;
                    }                                     
                }
            }

            m_nodeCurrent = entryNew;

            m_bCanUndo = true;
            m_bCanRedo = false;
        }
        public int GetUndoDescriptions(int nDepth, out string[] strUndoDesciptions)
        {
            // Check whether there are as much HistoryEntries as requested.
            int nDescAvailable = ValidateUndoDepth(nDepth);
            strUndoDesciptions = null;

            if (nDescAvailable > 0)
            {
                strUndoDesciptions = new string[nDescAvailable];
                HistoryEntry entryCurrent = m_nodeCurrent;

                for (int nCounter = 0; nCounter < nDescAvailable; nCounter++)
                {
                    // Write Description to string array.
                    strUndoDesciptions[nCounter] = entryCurrent.Command.Description;

                    // Proceed to previous HistoryEntry.
                    entryCurrent = entryCurrent.Previous;
                }
            }

            return nDescAvailable;
        }
        public int GetRedoDescriptions(int nDepth, out string[] strRedoDesciptions)
        {
            // Check whether there are as much HistoryEntries as requested.
            int nDescAvailable = ValidateRedoDepth(nDepth);
            strRedoDesciptions = null;

            if (nDescAvailable > 0)
            {
                HistoryEntry entryCurrent;

                if ((m_nodeCurrent.Previous == null) && (!m_bCanUndo))
                    entryCurrent = m_nodeCurrent;
                else
                    entryCurrent = m_nodeCurrent.Next;

                strRedoDesciptions = new string[nDescAvailable];

                for (int nCounter = 0; nCounter < nDescAvailable; nCounter++)
                {
                    // Write Description to string array.
                    strRedoDesciptions[nCounter] = entryCurrent.Command.Description;

                    // Proceed to previous HistoryEntry.
                    entryCurrent = entryCurrent.Next;
                }
            }

            return nDescAvailable;
        }

        #endregion

        #region helper methods
        private int ValidateRedoDepth(int nDepth)
        {
            int nDepthToReturn = 0;

            if (m_nodeCurrent != null)
            {
                HistoryEntry entryCurrent;

                if ((m_nodeCurrent.Previous == null) && (!m_bCanUndo))
                    entryCurrent = m_nodeCurrent;
                else
                    entryCurrent = m_nodeCurrent.Next;

                while (((entryCurrent != null) && m_bCanRedo) && (nDepth > nDepthToReturn))
                {
                    nDepthToReturn++;
                    entryCurrent = entryCurrent.Next;
                }                
            }

            return nDepthToReturn;
        }
        private int ValidateUndoDepth(int nDepth)
        {
            int nDepthToReturn = 0;

            if (m_nodeCurrent != null)
            {
                HistoryEntry entryCurrent = m_nodeCurrent;

                while (((entryCurrent != null) && m_bCanUndo) && (nDepth > nDepthToReturn))
                {
                    nDepthToReturn++;
                    entryCurrent = entryCurrent.Previous;
                }
            }

            return nDepthToReturn;
        }
        #endregion
    }
}
