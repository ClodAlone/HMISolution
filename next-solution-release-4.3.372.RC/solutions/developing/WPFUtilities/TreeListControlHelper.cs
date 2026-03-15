using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFUtilities
{
    public static class TreeListControlHelper
    {
        public readonly static object DummyNode = new Object();
        public static TreeListNode AddNode(this TreeListControl tree, object dataModel = null, TreeListNode parentNode = null, object tag = null, int nPos = -1)
        {
            if (dataModel == null)
                dataModel = new object();
            var node = new TreeListNode(dataModel);
            if (dataModel is TreeItemControl)
            {
                (dataModel as TreeItemControl).PropertyChanged += (s, e) =>
                {
                    //node.Content = null;
                    //node.Content = s as TreeItemControl;
                    var isFitting = tree.Tag as bool? ?? false;
                    if (!isFitting && e.PropertyName == nameof(TreeItemControl.ItemHeader))
                    {
                        tree.Tag = true;
                        tree.View.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            tree.View.BestFitColumn(tree.Columns[0]);
                            tree.Tag = false;
                        }), System.Windows.Threading.DispatcherPriority.Render);
                    }
                    tree.RefreshRow(node.RowHandle);
                };
            }
            node.Tag = tag;
            //node.IsExpandButtonVisible = showExpandButton ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
            if (parentNode == null)
            {
                if (nPos == -1)
                    tree.View.Nodes.Add(node);
                else
                    tree.View.Nodes.Insert(nPos, node);
            }
            else
            {
                if (nPos == -1)
                    parentNode.Nodes.Add(node);
                else
                    parentNode.Nodes.Insert(nPos, node);
            }
            return node;
        }

        public static List<T> GetSelectedNodesOfType<T>(this TreeListControl tree)
        {
            var list = new List<T>();

            foreach (TreeListNode newitem in tree.GetSelectedNodes())
            {
                if (newitem.Tag.GetType().IsAssignableFrom(typeof(T)))
                    list.Add((T)newitem.Tag);
            }

            return list;
        }

        public static TreeListNode GetPreviousNode(this TreeListControl tree, TreeListNode startingNode = null)
        {
            if (tree.View.Nodes.Count == 0)
                return null;

            TreeListNode prevNode = tree.View.Nodes[0];
            var currentNode = startingNode ?? tree.GetSelectedNodes().FirstOrDefault();
            if (currentNode != null && currentNode.ParentNode != null)
            {
                prevNode = currentNode.ParentNode;
                if (currentNode.ParentNode.Nodes?.IndexOf(currentNode) > 0)
                    prevNode = currentNode.ParentNode.Nodes[currentNode.ParentNode.Nodes.IndexOf(currentNode) - 1];
            }
            return prevNode;
        }

        public static void SelectNode(this TreeListControl tree, TreeListNode node)
        {
            if (node == null)
                return;

            DeleteParentDummy(node);

            tree.View.FocusedNode = node;
            tree.SelectedItems = new List<object>() { node.Content };
            tree.SelectedItem = node.Content;
        }

        public static bool CanBeExpanded(TreeListNode parent)
        {
            return parent.Nodes.Count == 1 && parent.Nodes[0].Tag == DummyNode;
        }

        public static TreeListNode GetTreeItem(this TreeListControl tree, Object tag, TreeListNode parent = null)
        {
            if (parent == null)
            {
                if (tree.View.Nodes.Count > 0)
                    parent = tree.View.Nodes[0];
                else
                    return null;
            }

            if (!parent.IsExpanded)
                parent.IsExpanded = true;

            var list = tag is string ? (from p in parent.Nodes where p.Tag.ToString() == (string)tag select p).ToList() : (from p in parent.Nodes where p.Tag == tag select p).ToList();
            if (list.Count > 0)
                return list[0];

            return null;
        }

        public static void AddDummyNodeIfNeeded(this TreeListControl tree, TreeListNode parentNode, Func<TreeListNode, bool> NeedToBeExpanded)
        {
            if (parentNode != null && !parentNode.IsExpanded && NeedToBeExpanded(parentNode) && parentNode.Nodes.Count == 0)
                tree.AddNode(null, parentNode, TreeListControlHelper.DummyNode);
        }

        public static bool WasExpanded(this TreeListNode node)
        {
            return node.IsExpanded || node.Nodes.Count > 0 && node.Nodes[0].Tag != TreeListControlHelper.DummyNode || node.Nodes.Count == 0;
        }

        public static List<object> GetTreeSelectedItems(this TreeListControl tree)
        {
            return GetTreeSelectedItems<object>(tree);
        }

        public static List<T> GetTreeSelectedItems<T>(this TreeListControl tree)
        {
            var selecteditems = new List<T>();
            var nodes = tree.GetSelectedNodes();
            foreach (var node in nodes)
            {
                if (node.Tag != null && node.Tag is T)
                    selecteditems.Add((T)node.Tag);
            }

            return selecteditems;
        }

        static void DeleteParentDummy(TreeListNode node)
        {
            if (node.ParentNode != null && node.ParentNode.Nodes.Count > 0 && node.ParentNode.Nodes[0].Tag == DummyNode)
                node.ParentNode.Nodes.RemoveAt(0);
        }

        public static void SelectNodes(this TreeListControl tree, List<TreeListNode> nodes, bool bClear = false, bool bExpand = false)
        {
            if (nodes == null)
                return;

            if (bClear)
                tree.UnselectAll();
            tree.BeginSelection();
            foreach (var n in nodes)
            {
                if (n == null)
                    continue;
                if (!tree.SelectedItems.Contains(n?.Content))
                    tree.SelectItem(n);
                if (bExpand && n.ParentNode != null)
                {
                    var isParentExpanding = (n.ParentNode.Content as TreeItemControl)?.IsNodeExpanding ?? false;
                    if (!n.ParentNode.IsExpanded && !isParentExpanding)
                    {
                        DeleteParentDummy(n);
                        n.ParentNode.IsExpanded = true;
                    }
                }
            }
            tree.EndSelection();
            if (nodes.Count > 0)
                tree.View.FocusedNode = nodes.Last();
        }

        public static void EditNode(this TreeListControl tree, TreeListNode node, bool forceEditing = true)
        {
            if (forceEditing)
                tree.EnableEditing();
            tree.View.Focus();
            tree.SelectNode(node);
            tree.View.ShowEditor(true);
        }

        public static void EnableEditing(this TreeListControl tree)
        {
            if (!tree.View.AllowEditing)
            {
                tree.View.AllowEditing = true;
                tree.View.NavigationStyle = GridViewNavigationStyle.Cell;
            }
        }

        public static void DisableEditing(this TreeListControl tree, bool bSetRowNavigationStyle = true)
        {
            if (tree.View.AllowEditing)
            {
                var focusedRowHandle = tree.View.FocusedRowHandle;
                tree.View.AllowEditing = false;
                if (bSetRowNavigationStyle)
                    tree.View.NavigationStyle = GridViewNavigationStyle.Row;
                tree.View.FocusedRowHandle = focusedRowHandle;
            }
        }

        public static void ClearSelection(this TreeListControl tree)
        {
            tree.View.FocusedNode = null;
            tree.SelectedItems.Clear();
        }

        public static List<TreeListNode> AddGetNodesList(this TreeListControl treeListControl, TreeListNode itemRoot, IEnumerable<object> nodesList, bool bAddOrGet, Func<object, TreeListNode, TreeListNode> AddTreeItem, TreeListNode parentNode = null)
        {
            if (parentNode == null)
                parentNode = itemRoot;
            var items = new List<TreeListNode>();
            foreach (var n in nodesList)
            {
                TreeListNode item;
                if (bAddOrGet)
                    item = AddTreeItem(n, parentNode);
                else
                    item = treeListControl.GetTreeItem(n, parentNode);
                if (item != null)
                    items.Add(item);
            }
            return items;
        }
    }
}
