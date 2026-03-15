#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    /// <summary>
    /// Extended history for <see cref="NavigationView"/>.
    /// </summary>
    public class HistoryManager :
        Syncfusion.Windows.Forms.HistoryManager
    {
        #region Properties

        /// <summary>
        /// Gets collection of undo command.
        /// </summary>
        public IList<ICommand> Undoes
        {
            get
            {
                return GetCommands(m_stackUndo);
            }
        }

        /// <summary>
        /// Gets collection of redo command.
        /// </summary>
        public IList<ICommand> Redoes
        {
            get
            {
                return GetCommands(m_stackRedo);
            }
        }

        #endregion

        #region Implementation

        private IList<ICommand> GetCommands(Stack stack)
        {
            List<ICommand> cmds = new List<ICommand>(stack.Count);
            IEnumerator enumer = stack.GetEnumerator();

            while (enumer.MoveNext())
            {
                ICommand cmd = enumer.Current as ICommand;

                if (cmd != null)
                {
                    cmds.Add(cmd);
                }
            }

            return cmds;
        }

        #endregion
    }
}
