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
using System.Reflection;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Command used with PathNode's GraphicsPath updating.
    /// </summary>
    public sealed class SetPointsCmd
        : ICommand
    {
        #region Class members
        private PathNode m_pathNode;
        private PointF[] m_pts;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPointsCmd"/> class.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        public SetPointsCmd(PathNode nodePath)
        {
            if (nodePath == null)
                throw new ArgumentNullException("nodePath");

            m_pathNode = nodePath;
            m_pts = (PointF[])GetOldValue();
        }
        #endregion

        #region ICommand Members

        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return "Update Path"; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value></value>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Undo()
        {
            bool success = true;
            // disable merging control points
            bool bMerge = DisableMerging();

            PointF[] ptsCur = (PointF[])GetOldValue();
            m_pathNode.SetPoints(m_pts);

            // restore previous state
            RestoreMerging(bMerge);

            m_pts = ptsCur;

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

            return success;
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
        private ArrayList commands = new ArrayList();
        #endregion

        #region IVerb Members

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Do(object target)
        {
            bool success = true;
            // disable merging control points
            bool bMerge = DisableMerging();

            PointF[] ptsCur = (PointF[])GetOldValue();
            m_pathNode.SetPoints(m_pts);

            // restore previous state
            RestoreMerging(bMerge);

            m_pts = ptsCur;

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

        /// <summary>
        /// Get the old path points array.
        /// </summary>
        /// <returns>The object.</returns>
        private object GetOldValue()
        {
            // get property current value
            PropertyInfo infoProperty = m_pathNode.GetType().GetProperty(
                DPN.PathPoints, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            object valueToReturn = infoProperty.GetValue(m_pathNode, new object[] { });

            // 3 - copy value
            ICloneable clone = valueToReturn as ICloneable;

            if (clone != null)
            {
                valueToReturn = clone.Clone();
            }

            return valueToReturn;
        }
        #endregion
    }
}
