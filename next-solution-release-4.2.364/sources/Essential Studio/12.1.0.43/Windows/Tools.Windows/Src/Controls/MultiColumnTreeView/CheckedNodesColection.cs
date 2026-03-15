#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Diagnostics;

using Syncfusion.Collections;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// This collection contains checked nodes in treeView. Use Add/Remove methods to add/remove nodes.
    /// These methods will check/uncheck proceed nodes automatically.
    /// Use Clear method to delete and uncheck all nodes from collection.
    /// </summary>
    public class CheckedNodesColection : ArrayListExt
    {
        #region Class members
        private bool m_bIsCleaning = false;
        #endregion

        #region Class properties

        public new TreeNodeAdv this[int index]
        {
            get
            {
                return base[index] as TreeNodeAdv;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the CheckedNodesColection class.
        /// </summary>
        public CheckedNodesColection()
        {
            this.ForceReadOnly = true;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Adds the specified node to the collection and checks it.
        /// </summary>
        /// <param name="value">Tree node object</param>
        /// <returns>Returns treenode index</returns>
        public override int Add(object value)
        {
            TreeNodeAdv node = value as TreeNodeAdv;
            Debug.Assert(null != node, "Treenode cannot be null");

            if (null == node || node.Checked)
            {
                return -1;
            }

            node.Checked = true;

            return base.Add(node);
        }

        /// <summary>
        /// Removes specified node from collection and unchecks it.
        /// </summary>
        /// <param name="obj">Treenode object</param>
        public override void Remove(object obj)
        {
            TreeNodeAdv node = obj as TreeNodeAdv;
            Debug.Assert(null != node, "Tree node cannot be null");
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Checked = false;

            // remove node from list
            base.Remove(node);
        }

        /// <summary>
        /// Clears collection.
        /// </summary>
        public override void Clear()
        {
            m_bIsCleaning = true;
            for (int i = 0; i < Count; i++)
            {
                this[i].Checked = false;
                i--;
            }
            m_bIsCleaning = false;
        }

        /// <summary>
        /// Add node to or remove from checked nodes collection and process the same way all node's subtree.
        /// </summary>
        /// <param name="node">Tree node</param>
        public void ResolveNode(TreeNodeAdv node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.Checked)
            {
                if (IndexOf(node) < 0)
                {
                    Add(node);
                }
            }
            else
            {
                Remove(node);
            }
        }

        /// <summary>
        /// Removes checked nodes in the specified collection from the list of checked nodes.
        /// </summary>
        /// <param name="nodes">Treenode collection</param>
        public void RemoveNodes(TreeNodeAdvCollection nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            if (nodes.Count == 0)
            {
                return;
            }

            for (int i = 0, len = nodes.Count; i < len; i++)
            {
                Remove(nodes[i]);
            }
        }

        /// <summary>
        /// Adds checked nodes from the collection to the list of checked nodes.
        /// </summary>
        /// <param name="nodes">Treenode Collection</param>
        public void AddNodes(TreeNodeAdvCollection nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            if (nodes.Count == 0)
            {
                return;
            }

            for (int i = 0, len = nodes.Count; i < len; i++)
            {
                Add(nodes[i]);
            }
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Adds the specified node and all it's checked subnodes to the collection.
        /// </summary>
        /// <param name="node">Tree node</param>
        /// <returns>Returns Treenode index</returns>
        internal int Add(TreeNodeAdv node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.Checked)
            {
                return base.Add(node);
            }

            return -1;
        }

        /// <summary>
        /// Removes specified node and all it's subnodes from collection.
        /// </summary>
        /// <param name="node">Tree node</param>
        internal void Remove(TreeNodeAdv node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (m_bIsCleaning)
            {
                node.Checked = false;
            }

            // remove node from list
            int idx = base.IndexOf(node);
            if (idx >= 0)
            {
                base.RemoveAt(idx);
            }
        }
        #endregion
    }
}