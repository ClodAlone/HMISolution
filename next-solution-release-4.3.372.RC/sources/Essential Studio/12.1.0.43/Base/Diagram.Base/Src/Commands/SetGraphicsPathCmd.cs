#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Command used with PathNode's GraphicsPath updating.
    /// </summary>
    public sealed class SetGraphicsPathCmd
        : ICommand
    {
        #region Class members
        private PathNode m_pathNode;
        private GraphicsPath m_pathOld;
        private GraphicsPath m_pathToCombineWith;
        private bool m_bConnect;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SetGraphicsPathCmd"/> class.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        /// <param name="pathCurrent">The path current.</param>
        /// <param name="pathToCombineWith">The path to combine with.</param>
        /// <param name="bConnect">if set to <c>true</c>  combined graphics paths.</param>
        public SetGraphicsPathCmd(PathNode nodePath, GraphicsPath pathCurrent, GraphicsPath pathToCombineWith, bool bConnect)
        {
            if (nodePath == null)
                throw new ArgumentNullException("nodePath");

            if (pathCurrent == null)
                throw new ArgumentNullException("pathCurrent");

            if (pathToCombineWith == null)
                throw new ArgumentNullException("pathToCombineWith");

            m_pathNode = nodePath;
            m_pathOld = pathCurrent;
            m_pathToCombineWith = pathToCombineWith;
            m_bConnect = bConnect;
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
            // set old GrapicsPath
            FieldInfo infoField = m_pathNode.GetType().GetField("m_gpPath", BindingFlags.Instance | BindingFlags.NonPublic);

            infoField.SetValue(m_pathNode, m_pathOld);

            // update Path points
            PropertyInfo infoProperty = m_pathNode.GetType().GetProperty(DPN.PathPoints, BindingFlags.Instance | BindingFlags.NonPublic);

            infoProperty.SetValue(m_pathNode, m_pathOld.PathPoints.Clone(), new object[] { });

            // reset hittesting region
            infoField = m_pathNode.GetType().GetField("m_rgnCache", BindingFlags.Instance | BindingFlags.NonPublic);

            infoField.SetValue(m_pathNode, null);

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
        /// <param name="target">Object that is acted upon (noun).</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Do(object target)
        {
            bool success = true;
            // get property current value
            FieldInfo infoField = m_pathNode.GetType().GetField("m_gpPath", BindingFlags.Instance | BindingFlags.NonPublic);

            infoField.SetValue(m_pathNode, m_pathOld);

            m_pathNode.TryCombine(m_pathToCombineWith, m_bConnect);

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
    }
}
