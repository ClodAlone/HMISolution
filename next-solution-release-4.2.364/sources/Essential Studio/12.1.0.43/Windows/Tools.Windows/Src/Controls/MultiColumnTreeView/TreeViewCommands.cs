#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class TreeViewCommand : ICommand
    {
        #region Class members

        private int m_index = -1;

        private TreeNodeAdv m_node = null;

        private TreeNodeAdv m_parentNode = null;

        private Action m_action = Action.None;

        private string m_strPreviousText = null;

        private string m_strNewText = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the node on whom the action is to be performed.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return m_node;
            }
        }

        /// <summary>
        /// Gets the action to be performed on the node.
        /// </summary>
        public Action Action
        {
            get
            {
                return m_action;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeViewCommand class.
        /// </summary>
        /// <param name="node">The node on which action is to be performed.</param>
        /// <param name="newText">New text for the editing action.</param>
        public TreeViewCommand(TreeNodeAdv node, string newText) :
            this(node, Action.Edit)
        {
            m_strNewText = newText;
            m_strPreviousText = node.Text;
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewCommand class.
        /// </summary>
        /// <param name="node">The node on which action is to be performed.</param>
        /// <param name="action">The action to perform.</param>
        public TreeViewCommand(TreeNodeAdv node, Action action)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            m_node = node;
            m_action = action;
            m_parentNode = node.Parent;
            m_index = node.Index;
        }
        #endregion

        #region ICommand Members
        /// <summary>
        /// Executes the action to be performed.
        /// </summary>
        public void Execute()
        {
            m_node.IsUndoRedoPerforming = true;
            switch (m_action)
            {
                case Action.Add:
                    m_parentNode.Nodes.Insert(m_index, m_node);
                    break;

                case Action.Remove:
                    m_parentNode.Nodes.Remove(m_node);
                    break;

                case Action.Edit:
                    m_node.Text = m_strNewText;
                    break;
            }

            m_node.IsUndoRedoPerforming = false;
        }

        /// <summary>
        /// Performs the reverse action on the node.
        /// </summary>
        public void Reverse()
        {
            switch (m_action)
            {
                case Action.Add:
                    m_action = Action.Remove;
                    break;

                case Action.Remove:
                    m_action = Action.Add;
                    break;

                case Action.Edit:
                    string strStore = m_strNewText;
                    m_strNewText = m_strPreviousText;
                    m_strPreviousText = strStore;
                    break;
            }
        }
        #endregion
    }

    #region enums
    /// <summary>
    /// Specifies the action to be performed.
    /// </summary>
    public enum Action
    {
        /// <summary>Represents Add</summary>
        Add,

        /// <summary>Represents Remove</summary>
        Remove,

        /// <summary>Represents Edit</summary>
        Edit,

        /// <summary>Represents None</summary>
        None
    }
    #endregion
}