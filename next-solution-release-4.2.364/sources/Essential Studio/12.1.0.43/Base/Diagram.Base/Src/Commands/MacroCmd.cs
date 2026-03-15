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
    /// List of commands that are bundled into a single command.
    /// </summary>
    public class MacroCmd : ICommand
    {
        private string m_strDescription = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroCmd"/> class.
        /// </summary>
        /// <param name="strDescription">The description.</param>
        public MacroCmd(string strDescription)
        {
            m_strDescription = strDescription;
        }

        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Executes all commands in the macro.
        /// </summary>
        /// <param name="cmdTarget">Command target object.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Do(object cmdTarget)
        {
            bool success = true;

            foreach (ICommand cmd in this.cmds)
            {
                success = cmd.Do(cmdTarget);

                if (!success)
                {
                    break;
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
            ICommand cmd;

            // foreach( ICommand cmd in cmds )
            for (int nCounter = cmds.Count - 1; nCounter >= 0; nCounter--)
            {
                cmd = (ICommand)this.cmds[nCounter];
                success = cmd.Undo();

                if (!success)
                {
                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Add a command to the macro.
        /// </summary>
        /// <param name="cmd">Command to add.</param>
        public void AddCommand(ICommand cmd)
        {
            cmds.Add(cmd);
        }

        /// <summary>
        /// Removes all commands from the macro.
        /// </summary>
        public void Clear()
        {
            cmds.Clear();
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
            if (cmd != null && !(cmds.Contains(cmd)))
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
            cmds.Add(cmd);
        }

        /// <summary>
        /// List of commands to execute.
        /// </summary>
        protected ArrayList cmds = new ArrayList();
    }
}
