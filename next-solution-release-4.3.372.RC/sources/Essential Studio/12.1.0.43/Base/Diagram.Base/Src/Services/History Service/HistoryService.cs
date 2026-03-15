#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Manager used to record all user actions to return to primary document.
    /// </summary>
    public class HistoryManager
        : Service
    {
        #region Class members
        private Model m_model;
        private bool m_bAtomicAction;
        private bool m_bInAction;
        protected HistoryList m_history;
        private MacroCmd m_cmdMacro;
        private int m_nSubTransactions;
        private int m_nMacroCommandCmds;
        private bool m_bUndoRedo;
        private bool m_enabled = true;
        #endregion

        #region Class initialize/finalize members
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
       
        public HistoryManager(Model model)
        {
            m_history = new HistoryList();
            m_model = model;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a value indicating whether this instance can undo last operation.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        public bool CanUndo
        {
            get { return m_history.CanUndo; }
        }        
        /// <summary>
        /// Gets a value indicating whether this instance can redo last backup user operation.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo
        {
            get { return m_history.CanRedo; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the history services is enabled or disabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.m_enabled;
            }

            set
            {
                this.m_enabled = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Clear all the history records.
        /// </summary>
        public void ClearHistory()
        {
            this.m_history.ClearHistory();
        }

        /// <summary>
        /// Gets HistoryEntries Descriptions.
        /// </summary>
        /// <param name="nDepth">Depth proceed.</param>
        /// <param name="strCmdDescriptions">string array which holds descriptions</param>
        /// <returns>Number of returned descriptions.</returns>
        public int GetRedoDescriptions(int nDepth, out string[] strCmdDescriptions)
        {
            return m_history.GetRedoDescriptions(nDepth, out strCmdDescriptions);
        }

        /// <summary>
        /// Gets HistoryEntries Descriptions.
        /// </summary>
        /// <param name="nDepth">Depth proceed.</param>
        /// <param name="strCmdDescriptions">string array which holds descriptions</param>
        /// <returns>Number of returned descriptions.</returns>
        public int GetUndoDescriptions(int nDepth, out string[] strCmdDescriptions)
        {
            return m_history.GetUndoDescriptions(nDepth, out strCmdDescriptions);
        }

        /// <summary>
        /// Determines whether this instance can merge the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to one; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMerge(ICommand cmd)
        {
            return m_history.CanMerge(cmd);
        }

        /// <summary>
        /// Merges the specified command to one.
        /// </summary>
        /// <param name="cmdToMerge">The command to merge.</param>
        public void Merge(ICommand cmdToMerge)
        {
            if (!m_bInAction)
                m_history.Merge(cmdToMerge);
        }

        /// <summary>
        /// Backup last user operation.
        /// </summary>
        /// <returns>true, if undo.</returns>
        public bool Undo()
        {
            bool bSuccess = false;
            m_bInAction = true;

            // Get cmd to Undo.
            ICommand cmdUndo = m_history.GetUndoCmd();

            if (cmdUndo != null && cmdUndo.CanUndo)
            {
                m_model.LinkManager.Pause();
                OnUndoCommandStarted(new EventArgs());

                bSuccess = cmdUndo.Undo();

                OnUndoCommandCompleted(new EventArgs());
                m_model.LinkManager.Resume();
            }

            m_bInAction = false;
            return bSuccess;
        }

        /// <summary>
        /// Step forward to operation history records.
        /// </summary>
        /// <returns>true, if redo.</returns>
        public bool Redo()
        {
            bool bSuccess = false;
            m_bInAction = true;

            // Get cmd to Redo.
            ICommand cmdRedo = m_history.GetRedoCmd();

            if (cmdRedo != null)
            {
                m_model.LinkManager.Pause();
                OnRedoCommandStarted(new EventArgs());

                // Perform Redo.
                bSuccess = cmdRedo.Do(m_model);

                OnRedoCommandCompleted(new EventArgs());
                m_model.LinkManager.Resume();
            }

            m_bInAction = false;
            return bSuccess;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Start record any changes to history command.
        /// </summary>
        /// <param name="strDescription">The short command description.</param>
        public void StartAtomicAction(string strDescription)
        {
            m_nSubTransactions++;
            
            if (!m_bAtomicAction && !m_bUndoRedo)
            {
                OnRecordRequest(new EventArgs());
                m_bAtomicAction = true;
                m_cmdMacro = new MacroCmd(strDescription);
            }
        }

        /// <summary>
        /// Stop recording changes and save command to history.
        /// </summary>
        public void EndAtomicAction()
        {
            m_nSubTransactions--;
            if (m_nSubTransactions == 0 && !m_bUndoRedo)
            {
                m_bAtomicAction = false;
                // if we have at least one command in MacroCommand
                // add new HistoryEntry
                if (m_nMacroCommandCmds > 0)
                {                 
                    AddHistoryEntry(m_cmdMacro);
                    m_nMacroCommandCmds = 0;
                }               
                OnRecordComplete(new EventArgs());
                m_cmdMacro = null;
            }
        }

        /// <summary>
        /// Called when history are stopped.
        /// </summary>
        protected override void OnStop()
        {
            m_history.ClearHistory();

            base.OnStop();
        }

        /// <summary>
        /// Record the move pin point action.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyContainerName">Name of the container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="szMoveOffset">The pin point move offset.</param>
        public void RecordMovePinPoint(IPropertyContainer propertyContainer, string strPropertyContainerName, string strPropertyName, SizeF szMoveOffset)
        {
            if (propertyContainer is PseudoGroup) return;

            if (propertyContainer == null)
                throw new ArgumentNullException("propertyContainer");

            if (strPropertyName == null)
                throw new ArgumentNullException("strPropertyName");

            if (CanAddNewHistoryEntry())
            {
                ICommand cmdNewEntry = new SetPropertyCmd(propertyContainer, strPropertyContainerName, strPropertyName, szMoveOffset);
                AddHistoryEntry(cmdNewEntry);
            }
        }

        /// <summary>
        /// Record the property changed action.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public void RecordPropertyChanged(IPropertyContainer propertyContainer, string strPropertyContainerName, string strPropertyName)
        {
            if (propertyContainer is PseudoGroup) return;

            if (propertyContainer == null)
                throw new ArgumentNullException("propertyContainer");

            if (strPropertyName == null)
                throw new ArgumentNullException("strPropertyName");

            if (CanAddNewHistoryEntry())
            {
                ICommand cmdNewEntry = new SetPropertyCmd(propertyContainer, strPropertyContainerName, strPropertyName);
                AddHistoryEntry(cmdNewEntry);
            }
        }

        /// <summary>
        /// Record the collection changed action.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="collection">The changed collection.</param>
        /// <param name="elements">The elements of collection.</param>
        /// <param name="nIndex">The elements insert index.</param>
        public void RecordCollectionChanged(CollectionExChangeType changeType, CollectionEx collection, ICollection elements, int nIndex)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand command = new CollectionModifyCommand(changeType, collection, elements, nIndex);
                AddHistoryEntry(command);
            }
        }

        /// <summary>
        /// Record the vertex changed.
        /// </summary>
        /// <param name="nodeVertexContainer">The node vertex container.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nVertexIdx">The vertex index in path points array.</param>
        /// <param name="ptNewLocation">The new vertex location.</param>
        public void RecordVertexChanged(PathNode nodeVertexContainer, VertexChangeType changeType, int nVertexIdx, PointF ptNewLocation)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand command = new VertexCmd(nodeVertexContainer, changeType, nVertexIdx, ptNewLocation);
                AddHistoryEntry(command);
            }
        }

        /// <summary>
        /// Record the zOrder changed value.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="changeType">Type of the change.</param>
        public void RecordZorderChanged(Node nodeChanged, ZOrderUpdate changeType)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand command = new ZOrderCmd(nodeChanged, changeType);
                AddHistoryEntry(command);
            }
        }

        /// <summary>
        /// Record the set points action.
        /// </summary>
        /// <param name="nodePath">The path points owner.</param>
        public void RecordSetPoints(PathNode nodePath)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand cmdNewEntry = new SetPointsCmd(nodePath);
                AddHistoryEntry(cmdNewEntry);
            }
        }

        /// <summary>
        /// Record the set graphics path action.
        /// </summary>
        /// <param name="nodePath">The graphics path owner.</param>
        /// <param name="pathCurrent">The path current.</param>
        /// <param name="pathToCombineWith">The path to combine with.</param>
        /// <param name="bConnect">if set to <c>true</c> path end points need to connect.</param>
        public void RecordSetGraphicsPath(PathNode nodePath, GraphicsPath pathCurrent, GraphicsPath pathToCombineWith, bool bConnect)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand cmdNewEntry = new SetGraphicsPathCmd(nodePath, pathCurrent, pathToCombineWith, bConnect);
                AddHistoryEntry(cmdNewEntry);
            }
        }

        /// <summary>
        /// Record the handler move action.
        /// </summary>
        /// <param name="pathNode">The handler container.</param>
        public void RecordMoveHandle(PathNode pathNode)
        {
            if (CanAddNewHistoryEntry())
            {
                ICommand cmdNewEntry = new MoveHandleCmd(pathNode);
                AddHistoryEntry(cmdNewEntry);
            }
        }
        #endregion

        #region	Class events
        /// <summary>
        /// Occurs when undo command started.
        /// </summary>
        public event EventHandler UndoCommandStarted;

        /// <summary>
        /// Occurs when redo command started.
        /// </summary>
        public event EventHandler RedoCommandStarted;

        /// <summary>
        /// Occurs when undo command completed.
        /// </summary>
        public event EventHandler UndoCommandCompleted;

        /// <summary>
        /// Occurs when redo command completed.
        /// </summary>
        public event EventHandler RedoCommandCompleted;

        /// <summary>
        /// Occurs when record request.
        /// </summary>
        public event EventHandler RecordRequest;

        /// <summary>
        /// Occurs when record complete.
        /// </summary>
        public event EventHandler RecordComplete;
        #endregion

        #region Class Event raising
        /// <summary>
        /// Raises the <see cref="E:UndoCommandStarted"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUndoCommandStarted(EventArgs evtArgs)
        {
            m_bUndoRedo = true;

            if (UndoCommandStarted != null)
                UndoCommandStarted(this, evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:RedoCommandStarted"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRedoCommandStarted(EventArgs evtArgs)
        {
            m_bUndoRedo = true;

            if (RedoCommandStarted != null)
                RedoCommandStarted(this, evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:UndoCommandCompleted"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUndoCommandCompleted(EventArgs evtArgs)
        {
            m_bUndoRedo = false;

            if (UndoCommandCompleted != null)
                UndoCommandCompleted(this, evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:RedoCommandCompleted"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRedoCommandCompleted(EventArgs evtArgs)
        {
            m_bUndoRedo = false;

            if (RedoCommandCompleted != null)
                RedoCommandCompleted(this, evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:RecordRequest"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRecordRequest(EventArgs evtArgs)
        {
            if (RecordRequest != null)
                RecordRequest(this, evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:RecordComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRecordComplete(EventArgs evtArgs)
        {
            if (RecordComplete != null)
                RecordComplete(this, evtArgs);
        }
        #endregion

        #region Class Helper methods
        private bool CanAddNewHistoryEntry()
        {
            if (this.Enabled == false)
                return false;

            return (this.ServiceStatus == ServiceStatus.Started || this.ServiceStatus == ServiceStatus.Resumed)
                && !m_bInAction;
        }
        private void AddHistoryEntry(ICommand cmdNewEntry)
        {
            if (Enabled)
            {
                if (m_bAtomicAction)
                {
                    m_cmdMacro.AddCommand(cmdNewEntry);
                    m_nMacroCommandCmds++;

                }
                else if(cmdNewEntry is MacroCmd)
                {     
                    
                    m_history.AddEntry(cmdNewEntry);
                }
            }
        }
        #endregion
    }
}