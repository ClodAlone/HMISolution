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
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Vertex commands
    /// </summary>
    public class VertexCmd
        : ICommand
    {
        #region Class members
        private PathNode m_nodeVertexContainer;
        private VertexChangeType m_changeType;
        private int m_nVertexIdx;
        private PointF m_ptVertexLocation;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexCmd"/> class.
        /// </summary>
        /// <param name="nodeVertexContainer">The node vertex container.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nVertexIdx">The vertex index.</param>
        /// <param name="ptVertexLocation">The vertex location.</param>
        public VertexCmd(PathNode nodeVertexContainer, VertexChangeType changeType, int nVertexIdx, PointF ptVertexLocation)
        {
            if (nodeVertexContainer == null)
                throw new ArgumentNullException("nodeVertexContainer");

            m_ptVertexLocation = ptVertexLocation;
            m_changeType = changeType;
            m_nVertexIdx = nVertexIdx;
            m_nodeVertexContainer = nodeVertexContainer;
        }
        #endregion

        #region ICommand Members
        /// <summary>
        /// Does the specified command target.
        /// </summary>
        /// <param name="cmdTarget">The command target.</param>
        /// <returns>true, if do the specified command target.</returns>
        public bool Do(object cmdTarget)
        {
            bool success = true;
            switch (m_changeType)
            {
                case VertexChangeType.Remove:
                    RemovePoint(m_nodeVertexContainer, m_nVertexIdx);
                    break;
                case VertexChangeType.Insert:
                    InsertPoint(m_nodeVertexContainer, m_nVertexIdx, m_ptVertexLocation);
                    break;
                case VertexChangeType.Set:
                    SetPoint(m_nodeVertexContainer, m_nVertexIdx, m_ptVertexLocation);
                    break;
            }

            if (commands.Count > 0)
            {
                foreach (ICommand cmd in this.commands)
                {
                    success = cmd.Do(cmdTarget);

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
            switch (m_changeType)
            {
                case VertexChangeType.Remove:
                    InsertPoint(m_nodeVertexContainer, m_nVertexIdx, m_ptVertexLocation);
                    break;
                case VertexChangeType.Insert:
                    m_ptVertexLocation = RemovePoint(m_nodeVertexContainer, m_nVertexIdx);
                    break;
                case VertexChangeType.Set:
                    PointF ptOffsetNegative = new PointF(-m_ptVertexLocation.X, -m_ptVertexLocation.Y);
                    SetPoint(m_nodeVertexContainer, m_nVertexIdx, ptOffsetNegative);
                    break;
            }

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
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value></value>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return m_changeType.ToString() + "Vertex"; }
        }

        /// <summary>
        /// Determines whether this instance can merge the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMerge(ICommand command)
        {
            if (command != null && command != this)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Merges the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void Merge(ICommand command)
        {
            commands.Add(command);
        }

        /// <summary>
        /// List of commands to execute.
        /// </summary>
        protected ArrayList commands = new ArrayList();
        #endregion

        #region Class helper methods
        private void SetPoint(PathNode nodePath, int nVertexIdx, PointF ptVertexOffset)
        {
            // get previous vertex location
            PointF ptVertexPreviousLocation = nodePath.GetPoint(nVertexIdx);

            nodePath.SetPoint(nVertexIdx, ptVertexPreviousLocation);
        }
        private PointF RemovePoint(PathNode nodePath, int nVertexIdx)
        {
            // get vertex location
            PointF ptVertexLocation = nodePath.GetPoint(nVertexIdx);
            
            // remove vertex
            nodePath.RemovePoint(nVertexIdx);

            return ptVertexLocation;
        }
        private void InsertPoint(PathNode nodePath, int nVertexIdx, PointF ptVertexLocation)
        {
            nodePath.InsertPoint(nVertexIdx, ptVertexLocation);
        }
        #endregion
    }
}
