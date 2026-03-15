#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// This class allows the user to simply implement Undo\Redo engine.
    /// To achieve this, class that supports Undo\Redo must implement ISuppportHistory interface
    /// and ICommand interface, which will actually execute specified actions for each undo\redo step.
    /// HistoryManager supports executing more undo\redo actions as one block.
    /// Use BeginBlock(), CloseBlock() for this.
    /// </summary>
    public class HistoryManager : IHistoryManager
    {
        #region Class members
        /// <summary>
        /// Used as a separator between blocks of commands.
        /// </summary>
        private static BlockSeparator s_blockSeparator = new BlockSeparator();

        /// <summary>
        /// Indicates number of opened blocks of commands.
        /// </summary>
        private int m_openedBlocksCount = 0;

        /// <summary>
        /// UnDo commands list.
        /// </summary>
        protected Stack m_stackUndo = null;

        /// <summary>
        /// ReDo commands list.
        /// </summary>
        protected Stack m_stackRedo = null;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the HistoryManager class.
        /// </summary>
        public HistoryManager()
        {
            m_stackUndo = new Stack();
            m_stackRedo = new Stack();
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        ///  Gets a value indicating whether the block of commands is opened.
        /// </summary>
        protected bool IsBlockOpened
        {
            get
            {
                return m_openedBlocksCount > 0;
            }
        }

        /// <summary>
        /// Resets history.
        /// </summary>
        public void Reset()
        {
            m_stackRedo.Clear();
            m_stackUndo.Clear();
            m_openedBlocksCount = 0;
        }

        /// <summary>
        /// Adds command to history list.
        /// </summary>
        /// <param name="cmd"> Command to add. </param>
        public void Do(ICommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (m_stackRedo.Count > 0)
            {
                m_stackRedo.Clear();
            }

            cmd.Reverse();
            m_stackUndo.Push(cmd);
        }

        /// <summary>
        /// Performs Redo operation.
        /// </summary>
        /// <returns>True if successful; false otherwise.</returns>
        public bool Redo()
        {
            if (IsBlockOpened)
            {
                throw new InvalidOperationException("History block must be closed before calling this method.");
            }

            return ProcessCommands(m_stackRedo, m_stackUndo);
        }

        /// <summary>
        /// Performs UnDo operation.
        /// </summary>
        /// <returns>True if successful; false otherwise.</returns>
        public bool Undo()
        {
            if (IsBlockOpened)
            {
                throw new InvalidOperationException("History block must be closed before calling this method.");
            }

            return ProcessCommands(m_stackUndo, m_stackRedo);
        }

        /// <summary>
        /// Begins block of commands.
        /// User may add more than one command to Undo\Redo list
        /// and unite them into one command, e.g. :
        /// Each Opened block must be closed.
        /// Block of commands may be opened multiple times,
        /// but IT MUST BE CLOSED that many times as it was opened.
        /// </summary>
        /// <remarks>
        /// <code>
        /// BeginBlock();
        /// // add action1
        /// // .....
        /// BeginBlock();
        /// //......
        /// EndBlock();
        /// // add actionN
        /// CloseBlock();
        /// </code>
        /// </remarks>
        public void BeginBlock()
        {
            if (!IsBlockOpened)
            {
                m_stackUndo.Push(s_blockSeparator);
            }

            m_openedBlocksCount++;
        }

        /// <summary>
        /// Closes block of commands.
        /// </summary>
        /// <remarks>
        /// See <see cref="BeginBlock"/> for more details.
        /// </remarks>
        public void CloseBlock()
        {
            if (!IsBlockOpened)
            {
                throw new InvalidOperationException("There is no opened history block to close.");
            }

            m_openedBlocksCount--;

            if (!IsBlockOpened)
            {
                m_stackUndo.Push(s_blockSeparator);
            }
        }

        /// <summary>
        /// Rolls back non-closed block.
        /// </summary>
        public void RollBack()
        {
            if (!IsBlockOpened)
            {
                throw new InvalidOperationException("No history block opened to rollback.");
            }

            BlockSeparator separator = null;

            while (separator == null)
            {
                separator = m_stackUndo.Pop() as BlockSeparator;
            }

            m_openedBlocksCount = 0;
        }

        /// <summary>
        ///  Gets a value indicating whether Undo Commands list is not empty. Returns true if not empty; false otherwise.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return m_stackUndo.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Redo Command list is not empty. Returns true if not empty; false otherwise.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return m_stackRedo.Count > 0;
            }
        }

        #endregion

        #region Class utility methods
        private void ProcessCommand(ICommand cmd, Stack destination)
        {
            cmd.Execute();
            cmd.Reverse();
            destination.Push(cmd);
        }
        private void ProcessCommand(ICommand cmd, Stack destination,Stack src)
        {
            cmd.Execute();
            cmd.Reverse();
            destination.Push(cmd);
            src.Pop();
        }
        private bool ProcessCommands(Stack stackSrc, Stack stackDest)
        {
            bool succeed = false;
            if (stackSrc.Count > 0)
            {
                object obj = stackSrc.Pop();

                BlockSeparator separator = obj as BlockSeparator;
                if (separator != null)
                {
                    stackDest.Push(separator);

                    separator = null;
                    do
                    {
                        obj = stackSrc.Pop();
                        ICommand cmd = obj as ICommand;

                        if (cmd != null)
                        {
                            ProcessCommand(cmd, stackDest);
                        }
                        else
                        {
                            separator = obj as BlockSeparator;
                        }
                    }
                    while (separator == null && stackSrc.Count > 0);

                    stackDest.Push(separator);

                    succeed = true;
                }
                else
                {
                    ICommand cmd = obj as ICommand;
                    if (cmd != null)
                    {
                        ProcessCommand(cmd, stackDest,stackSrc);
                        succeed = true;
                    }
                }
            }

            return succeed;
        }
        #endregion
    }

    #region interfaces
    /// <summary>
    /// Objects that support history must implement this interface.
    /// </summary>
    public interface ISuppportHistory
    {
        /// <summary>
        /// Gets or sets the HistoryManager to use.
        /// </summary>
        HistoryManager HistoryManager { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether register items are in history list.
        /// </summary>
        bool HistoryEnabled { get; set; }
    }

    /// <summary>
    /// User must implement this interface to perform specified actions for UnDo\ReDo.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes action.
        /// </summary>
        void Execute();

        /// <summary>
        /// Reverses command. 
        /// </summary>
        void Reverse();
    }

    public interface IHistoryManager
    {
        /// <summary>
        /// Resets history.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds command to the history list.
        /// </summary>
        /// <param name="cmd">Command to add.</param>
        void Do(ICommand cmd);

        /// <summary>
        /// Performs UnDo operation.
        /// </summary>
        /// <returns>True if successful; false otherwise.</returns>
        bool Undo();

        /// <summary>
        /// Performs Redo operation.
        /// </summary>
        /// <returns> True if successful; false otherwise. </returns>
        bool Redo();

        /// <summary>
        /// Begins block of commands.
        /// </summary>
        void BeginBlock();

        /// <summary>
        /// Closes block of commands.
        /// </summary>
        void CloseBlock();

        /// <summary>
        /// Rolls back last non-closed block.
        /// </summary>
        void RollBack();
    }
    #endregion

    #region Utility classes
    /// <summary>
    /// This class is used as a separator between blocks of commands.
    /// </summary>
    internal class BlockSeparator
    {
    }
    #endregion
}
