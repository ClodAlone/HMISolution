#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Collections;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    internal class TreeNodeHelper
    {
        public static TreeViewCounter GetInnerPosition(TreeNode node)
        {
            TreeNodeEntry entry = node.TreeNodeEntry;
            if (entry != null && entry.Tree != null)
                return (TreeViewCounter) entry.GetCounterPosition();
            return TreeViewCounter.Empty;
        }

        public static TreeViewCounter GetParentPosition(TreeNode node)
        {
            TreeNode parent = node.ParentNode;
            if (parent != null)
                return GetCumulatedPosition(parent).Combine(new TreeViewCounter(parent.GetItemRows(), parent.GetItemHeight()), TreeViewCounterKind.CountAll);
            return  TreeViewCounter.Empty;//new TreeViewCounter(node.ItemRows, node.ItemHeight);
        }

        public static TreeViewCounter GetCumulatedPosition(TreeNode node)
        {
            TreeViewCounter n1 = GetInnerPosition(node);
            TreeViewCounter n2 = GetParentPosition(node);
            return n1.Combine(n2, TreeViewCounterKind.CountAll);
        }

        public static TreeNode GetNextSiblingTreeNode(TreeNode node, int cookie)
        {
            TreeNodeEntry current = node.TreeNodeEntry;
            if (current == null)
                return null;

            TreeTableWithCounter tree = (TreeTableWithCounter)current.Tree;
            if (tree == null)
                return null;

            TreeNodeEntry next = (TreeNodeEntry) tree.GetNextNotEmptyCounterEntry(current, cookie);

            //while (next != null)
            //{
            //    if (next.TreeNode.ParentNode == null)
            //        break;
            //    next = (TreeNodeEntry) tree.GetNextNotEmptyCounterEntry(next, cookie);
            //}

            if (next == null)
                return null;

            return next.TreeNode;
        }

        public static TreeNode GetNextTreeNodeStepIn(TreeNode node, int cookie)
        {
            TreeNode next = node;

            TreeTableWithCounter treeEntries = null;
            if (node.IsPopulated && node.IsExpanded)
                treeEntries = node.ChildNodes.RBTree;

                // check if node has child nodes. If yes get first child node.
            if (treeEntries != null && (treeEntries.GetCounterTotal().GetValue(cookie) > 0))
            {
                TreeNodeEntry entry = (TreeNodeEntry)treeEntries[0];
                if (entry != null && entry.GetCounterTotal().GetValue(cookie) == 0)
                    entry = (TreeNodeEntry)treeEntries.GetNextNotEmptyCounterEntry(entry, cookie);

                if (entry != null)
                    next = entry.TreeNode;
            }
            else
            {
                next = TreeNodeHelper.GetNextSiblingTreeNode(next, cookie);
            }
            //else
            //{
            //    if (node.IsPopulated)
            //        next = node.ChildNodes[0];
            //    else
            //        next = null;
            //}

            while (next != null)
            {
                if (next.ParentNode.IsChildVisible(next))
                    break;
                next = TreeNodeHelper.GetNextSiblingTreeNode(next, cookie);
            }

            if (next != null)
                return next;

            TreeNode parent = node.ParentNode;
            while (parent != null)
            {
                next = TreeNodeHelper.GetNextSiblingTreeNode(parent, cookie);
                if (next != null)
                {
                    if (next.ParentNode.IsChildVisible(next))
                        return next;
                }
                parent = parent.ParentNode;
            }
            return null;
        }

        public static TreeNode GetNextSibling(TreeNode node)
        {
            TreeNodeEntry current = node.TreeNodeEntry;
            if (current == null)
                return null;

            TreeTableWithCounter tree = (TreeTableWithCounter)current.Tree;
            if (tree == null)
                return null;

            TreeNodeEntry next = (TreeNodeEntry)tree.GetNextEntry(current);
            if (next == null)
                return null;

            return next.TreeNode;
        }

        public static TreeNode GetPreviousSibling(TreeNode node)
        {
            TreeNodeEntry current = node.TreeNodeEntry;
            if (current == null)
                return null;

            TreeTableWithCounter tree = (TreeTableWithCounter)current.Tree;
            if (tree == null)
                return null;

            TreeNodeEntry next = (TreeNodeEntry)tree.GetPreviousEntry(current);
            if (next == null)
                return null;

            return next.TreeNode;
        }

    }

}
