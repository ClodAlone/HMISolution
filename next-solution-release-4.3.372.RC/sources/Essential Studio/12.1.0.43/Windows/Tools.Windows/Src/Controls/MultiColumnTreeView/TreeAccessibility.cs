#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class TreeViewAdvAcessibleObject : Control.ControlAccessibleObject
    {
        #region Class members
        private MultiColumnTreeView m_tree;
        #endregion

        #region Class properties
        /// <summary>Gets the role for the GroupView. This is used by accessibility programs.</summary>
        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Outline;
            }
        }

        public override string Name
        {
            get
            {
                return m_tree.AccessibleName;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return m_tree.RectangleToScreen(m_tree.ClientRectangle);
            }
        }

        public override string Description
        {
            get
            {
                return m_tree.AccessibleDescription;
            }
        }

        public override string Help
        {
            get
            {
                return String.Empty;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return m_tree.Parent.AccessibilityObject;
            }
        }

        /// <summary>Gets the state for the GroupView. This is used by accessibility programs.</summary>
        public override AccessibleStates State
        {
            get
            {
                return m_tree.Visible ? AccessibleStates.None : AccessibleStates.Invisible;
            }
        }

        public override string Value
        {
            get
            {
                return m_tree.Text;
            }
            set
            {
                m_tree.Text = value;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
   
        public TreeViewAdvAcessibleObject(MultiColumnTreeView tree)
            : base(tree)
        {
            m_tree = tree;
        }

        #endregion

        #region Class overrides
        /// <summary> The GroupViewItem objects are "child" controls in terms of accessibility so
        /// return the number of GroupViewItems.</summary>
        /// <returns>Returns the number of GroupViewItems</returns>
        public override int GetChildCount()
        {
            return m_tree.Root.Nodes.Count;
        }

        /// <summary> Gets the Accessibility object of the GroupViewItem identified by index.</summary>
        /// <returns>Returns Child</returns>
        /// <param name="index">Tree node index</param>
        public override AccessibleObject GetChild(int index)
        {
            if (index < this.GetChildCount())
            {
                return m_tree.Root.Nodes[index].AccesibleObject;
            }

            return null;
        }

        public override AccessibleObject GetFocused()
        {
            if (m_tree.Focused)
            {
                return this.GetSelected();
            }
            else
            {
                return base.GetFocused();
            }
        }

        public override AccessibleObject GetSelected()
        {
            if (m_tree.SelectedNode != null)
            {
                return m_tree.SelectedNode.AccesibleObject;
            }

            return base.GetSelected();
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            TreeNodeAdv node = m_tree.GetNodeAtPoint(m_tree.PointToClient(new Point(x, y)));
            if (node != null)
            {
                return node.AccesibleObject;
            }
            else
            {
                return base.HitTest(x, y);
            }
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            switch (navdir)
            {
                case AccessibleNavigation.Down:
                case AccessibleNavigation.FirstChild:
                case AccessibleNavigation.Next:
                case AccessibleNavigation.Right:
                    if (m_tree.Root.FirstNode != null)
                    {
                        return m_tree.Root.FirstNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
            }
            return base.Navigate(navdir);
        }
        #endregion
    }

    [DocumentationExclude()]
    public class TreeNodeAdvAccessibleObject : AccessibleObject
    {
        #region Class members

        private TreeNodeAdv m_node;
        #endregion

        #region Class properties
        public override AccessibleStates State
        {
            get
            {
                AccessibleStates astates = AccessibleStates.None;
                if (m_node.TreeView != null)
                {
                    if (m_node.Enabled)
                    {
                        astates |= AccessibleStates.Selectable;
                    }
                    else
                    {
                        astates |= AccessibleStates.Unavailable;
                    }

                    if (m_node.Checked)
                    {
                        astates |= AccessibleStates.Checked;
                    }
                    if (m_node.Expanded)
                    {
                        astates |= AccessibleStates.Expanded;
                    }
                    else
                    {
                        astates |= AccessibleStates.Collapsed;
                    }

                    if (m_node.TreeView.SelectionMode != TreeSelectionMode.Single)
                    {
                        astates |= AccessibleStates.ExtSelectable;
                    }
                    if (m_node.TreeView.HotTracking)
                    {
                        astates |= AccessibleStates.HotTracked;
                    }
                    if (!m_node.Visible)
                    {
                        astates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                    }
                    if (!m_node.IsSelected)
                    {
                        astates |= AccessibleStates.Selected;
                    }
                }
                else
                {
                    astates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                return astates;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.OutlineItem;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                if (m_node.TreeView != null)
                {
                    return m_node.TreeView.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string Name
        {
            get
            {
                return m_node.Text;
            }
        }

        public override string DefaultAction
        {
            get
            {
                if (m_node.HasNodes
                  || (m_node.TreeView.LoadOnDemand && !m_node.ExpandedOnce))
                {
                    if (m_node.Expanded)
                    {
                        return "Collapse";
                    }
                    else
                    {
                        return "Expand";
                    }
                }
                return "Select";
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                if (m_node.TreeView != null)
                {
                    return m_node.TreeView.RectangleToScreen(m_node.Bounds);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
        }

        public override string Description
        {
            get
            {
                return m_node.Text;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvAccessibleObject(TreeNodeAdv node)
        {
            m_node = node;
        }

        #endregion

        #region Class overrides
     
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            switch (navdir)
            {
                case AccessibleNavigation.Down:
                    if (m_node.NextVisibleNode != null)
                    {
                        return m_node.NextVisibleNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.FirstChild:
                    if (m_node.FirstNode != null)
                    {
                        return m_node.FirstNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.LastChild:
                    if (m_node.LastNode != null)
                    {
                        return m_node.LastNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.Left:
                    if (m_node.Parent != null)
                    {
                        return m_node.Parent.AccesibleObject;
                    }
                    else
                    {
                        return m_node.TreeView.AccessibilityObject;
                    }
                case AccessibleNavigation.Next:
                    if (m_node.NextVisibleNode != null)
                    {
                        return m_node.NextVisibleNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.Previous:
                    if (m_node.PrevVisibleNode != null)
                    {
                        return m_node.PrevVisibleNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.Right:
                    if (m_node.FirstNode != null)
                    {
                        return m_node.FirstNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
                case AccessibleNavigation.Up:
                    if (m_node.PrevVisibleNode != null)
                    {
                        return m_node.PrevVisibleNode.AccesibleObject;
                    }
                    else
                    {
                        return null;
                    }
            }

            return null;
        }

        public override void DoDefaultAction()
        {
            if (m_node.HasNodes
              || (m_node.TreeView.LoadOnDemand && !m_node.ExpandedOnce))
            {
                m_node.Expanded = !m_node.Expanded;
            }
            else
            {
                this.Select(AccessibleSelection.TakeSelection);
            }
        }

        public override void Select(AccessibleSelection flags)
        {
            if (m_node == m_node.TreeView.Root)
            {
                return;
            }

            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!m_node.TreeView.Focused)
                {
                    m_node.TreeView.Focus();
                }
            }
            if ((flags & AccessibleSelection.TakeSelection) != 0)
            {
                m_node.TreeView.SelectedNode = m_node;
            }
            else if ((flags & AccessibleSelection.ExtendSelection) != 0)
            {
                m_node.TreeView.ExtendSelectionTo(m_node);
            }
            else if ((flags & AccessibleSelection.AddSelection) != 0)
            {
                m_node.TreeView.SelectedNodes.Add(m_node);
            }
            else if ((flags & AccessibleSelection.RemoveSelection) != 0)
            {
                m_node.TreeView.SelectedNodes.Remove(m_node);
            }
        }

        #endregion
    }
}