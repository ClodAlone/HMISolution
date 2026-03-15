#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Command used for save handle changes to history.
    /// </summary>
    public class MoveHandleCmd : ICommand
    {
        #region Class members
        /// <summary>
        /// Move handle container. Used to check if handle is valid.
        /// </summary>
        private PathNode m_pathNode;

        /// <summary>
        /// Path points in model coordinates what use to undo changes.
        /// </summary>
        private PointF[] m_ptsPrevPathPoints;

        /// <summary>
        /// Path points in model coordinates what use to redo changes.
        /// </summary>
        private PointF[] m_ptsNewPathPoints;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveHandleCmd"/> class.
        /// </summary>
        /// <param name="pathNode">The path node.</param>
        public MoveHandleCmd(PathNode pathNode)
        {
            if (pathNode == null)
                throw new ArgumentNullException("MoveHandleCmd.ctor( )");
                        
            m_pathNode = pathNode.Clone() as PathNode;
            m_pathNode.Parent = pathNode.Parent;
            // save current path points in model coordinate
            Matrix mtxTransform = pathNode.GetTransformations();
            m_ptsPrevPathPoints = pathNode.GetPoints();
            mtxTransform.TransformPoints(m_ptsPrevPathPoints);

            m_ptsNewPathPoints = null;
        }
        #endregion

        #region ICommand
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        public string Description
        {
            get { return "Move Handle"; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon (noun).</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Do(object target)
        {
            bool success = true;
            if (m_ptsNewPathPoints == null)
                throw new ArgumentNullException("m_ptsNewPathPoints not initialized.");

            // clone new path points
            PointF[] ptsPath = (PointF[])m_ptsNewPathPoints.Clone();

            // convert new points to local coordinates
            Matrix mtxTransform = m_pathNode.GetTransformations();
            mtxTransform.Invert();
            mtxTransform.TransformPoints(ptsPath);

            // disable merging control points
            bool bMerge = DisableMerging();

            // set new path points
            m_pathNode.SetPoints(ptsPath);

            // restore previous state
            RestoreMerging(bMerge);

            if (commands.Count > 0)
            {
                foreach (ICommand cmd in this.commands)
                {
                    success = cmd.Do(target);

                    if (!success)
                    {
                        break;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Undo()
        {
            bool success = true;
            // clione old path points
            PointF[] ptsPath = (PointF[])m_ptsPrevPathPoints.Clone();
            Matrix mtxTransform = m_pathNode.GetTransformations();

            // save new path points
            if (m_ptsNewPathPoints == null)
            {
                PointF[] ptsPrevPath = m_pathNode.GetPoints();
                mtxTransform.TransformPoints(ptsPrevPath);
                m_ptsNewPathPoints = ptsPrevPath;
            }

            // conver points to local coordinates
            mtxTransform.Invert();
            mtxTransform.TransformPoints(ptsPath);

            // disable merging control points
            bool bMerge = DisableMerging();

            // set old path points
            m_pathNode.SetPoints(ptsPath);

            // restore previous state
            RestoreMerging(bMerge);

            if (commands.Count > 0)
            {
                ICommand cmd;

                // foreach( ICommand cmd in cmds )
                for (int nCounter = commands.Count - 1; nCounter >= 0; nCounter--)
                {
                    cmd = (ICommand)this.commands[nCounter];
                    success = cmd.Undo();

                    if (!success)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMerge(ICommand cmd)
        {
            if (cmd != null && cmd != this)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Merges the specified command with last recorded user operation.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        public void Merge(ICommand cmd)
        {
            commands.Add(cmd);
        }

        /// <summary>
        /// List of commands to execute.
        /// </summary>
        protected ArrayList commands = new ArrayList();
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets or sets the state of the path node connector.
        /// </summary>
        /// <value>The state of the path node connector.</value>
        private ConnectorState PathNodeConnectorState
        {
            get
            {
                ConnectorState state = ConnectorState.Default;

                Line line = m_pathNode as Line;
                ConnectorBase connector = m_pathNode as ConnectorBase;

                if (line != null)
                {
                    state = line.ConnectorState;
                }
                else if (connector != null)
                {
                    state = connector.ConnectorState;
                }

                return state;
            }
            set
            {
                Line line = m_pathNode as Line;
                ConnectorBase connector = m_pathNode as ConnectorBase;

                if (line != null)
                {
                    line.ConnectorState = value;
                }
                else if (connector != null)
                {
                    connector.ConnectorState = value;
                }
            }
        }

        /// <summary>
        /// Disable the merging of control points.
        /// </summary>
        /// <returns>true, if disable merging.</returns>
        private bool DisableMerging()
        {
            ConnectorState state = PathNodeConnectorState;
            ConnectorState merge = ConnectorState.MergeControlPoints;

            bool bSuccess = (state & merge) == merge;

            if (bSuccess)
                this.PathNodeConnectorState = state & (~ConnectorState.MergeControlPoints);

            return bSuccess;
        }

        /// <summary>
        /// Restore node previous state.
        /// </summary>
        /// <param name="bMerge">if set to <c>true</c> to enable merge flag.</param>
        private void RestoreMerging(bool bMerge)
        {
            if (bMerge)
            {
                PathNodeConnectorState |= ConnectorState.MergeControlPoints;
            }
        }
        #endregion
    }
}