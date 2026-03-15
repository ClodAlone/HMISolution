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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Change the Z-order of one or more nodes.
    /// </summary>
    /// <remarks>
    /// <para>Z-order determines the order in which nodes are rendered
    /// and affects how nodes overlap.</para>
    /// </remarks>
    public class ZOrderCmd
        : ICommand
    {
        #region Class members
        private Node m_nodeAffected;
        private ZOrderUpdate m_changeType;
        private int m_nZOrder;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ZOrderCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Node which ZOrder has changed.</param>
        /// <param name="changeType">Specifies how Z-order will be changed.</param>
        public ZOrderCmd(Node nodeAffected, ZOrderUpdate changeType)
        {
            if (nodeAffected == null || !(nodeAffected.Parent is IZOrderContainer))
            {
                throw new ArgumentNullException("nodeAffected");
            }

            IZOrderContainer container = (IZOrderContainer)nodeAffected.Parent;

            m_nZOrder = container.GetZOrder(nodeAffected);
            m_nodeAffected = nodeAffected;
            m_changeType = changeType;
        }
        #endregion

        #region ICommand Members
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return m_changeType.ToString(); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value>true, if can undo.</value>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Executes the ZOrderCmd.
        /// </summary>
        /// <param name="cmdTarget">Not used.</param>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// <para>
        /// Loops through the nodes attached to the command and changes the Z-order of each
        /// node. The type of change made to the Z-order is specified by the ZOrderUpdate
        /// parameter passed in the constructor. The parent of each node must support the
        /// IZOrderContainer service.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.IZOrderContainer"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ZOrderUpdate"/>
        /// </remarks>
        public bool Do(object cmdTarget)
        {
            bool bSuccess = false;

            IZOrderContainer container = m_nodeAffected.Parent as IZOrderContainer;

            if (container != null)
            {
                m_nZOrder = container.SetZOrder(m_nodeAffected, m_nZOrder);
                bSuccess = true;
            }
            if (commands.Count > 0)
            {
                foreach (ICommand cmd in this.commands)
                {
                    bSuccess = cmd.Do(cmdTarget);

                    if (!bSuccess)
                    {
                        break;
                    }
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// Restores the Z-order of the attached nodes back to their original position.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Undo()
        {
            bool bSuccess = false;

            IZOrderContainer container = m_nodeAffected.Parent as IZOrderContainer;

            if (container != null)
            {
                m_nZOrder = container.SetZOrder(m_nodeAffected, m_nZOrder);
                bSuccess = true;
            }
            if (commands.Count > 0)
            {
                ICommand cmd;

                // foreach( ICommand cmd in cmds )
                for (int nCounter = commands.Count - 1; nCounter >= 0; nCounter--)
                {
                    cmd = (ICommand)this.commands[nCounter];
                    bSuccess = cmd.Undo();

                    if (!bSuccess)
                    {
                        break;
                    }
                }
            }

            return bSuccess;
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
    }

    /// <summary>
    /// Change the z-order of the nodes.
    /// </summary>
    public class ZOrderComparer
        : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one
        /// is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>The value indicating whether one
        /// is less than, equal to or greater than the other.</returns>
        public int Compare(object x, object y)
        {
            if (x == null || !(x is Node))
                throw new ArgumentNullException("x");

            if (y == null || !(y is Node))
                throw new ArgumentNullException("y");

            Node node1 = (Node)x;
            Node node2 = (Node)y;

            return node1.ZOrder - node2.ZOrder;
        }
        #endregion
    }

    /// <summary>
    /// Reverse the z-order of the nodes.
    /// </summary>
    public class ReverseZOrderComparer
        : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one
        /// is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>The value indicating whether one
        /// is less than, equal to or greater than the other.</returns>
        public int Compare(object x, object y)
        {
            if (x == null || !(x is Node))
                throw new ArgumentNullException("x");

            if (y == null || !(y is Node))
                throw new ArgumentNullException("y");

            Node node1 = (Node)x;
            Node node2 = (Node)y;

            return node2.ZOrder - node1.ZOrder;
        }
        #endregion
    }
}
