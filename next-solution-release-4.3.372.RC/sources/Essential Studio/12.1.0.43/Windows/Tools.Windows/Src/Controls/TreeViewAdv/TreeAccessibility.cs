#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.Serialization;
using Syncfusion.Drawing;
using Syncfusion.Styles;


namespace Syncfusion.Windows.Forms.Tools
{
	[Documentation.DocumentationExclude()]
	public class TreeViewAdvAcessibleObject : Control.ControlAccessibleObject
	{
		TreeViewAdv tree;
		public TreeViewAdvAcessibleObject(TreeViewAdv tree):base(tree)
		{
			this.tree = tree;
		}
		// Gets the role for the GroupView. This is used by accessibility programs.
		public override AccessibleRole Role
		{  
			get { return AccessibleRole.Outline; }
		}

		public override string Name 
		{ 
			get { return this.tree.AccessibleName; }
		}
            
		public override Rectangle Bounds 
		{ 
			get	{ return this.tree.RectangleToScreen(this.tree.ClientRectangle); } 
		}

		public override string Description
		{
			get { return this.tree.AccessibleDescription; }
		}

		public override string Help
		{
			get { return String.Empty; }
		}
		public override AccessibleObject Parent
		{
			get { return this.tree.Parent.AccessibilityObject;	}
		}

		// Gets the state for the GroupView. This is used by accessibility programs.
		public override AccessibleStates State
		{  
			get { return (this.tree.Visible ? AccessibleStates.None : AccessibleStates.Invisible);	}
		}

		// The GroupViewItem objects are "child" controls in terms of accessibility so 
		// return the number of GroupViewItems.
		public override int GetChildCount()
		{  
			return this.tree.Nodes.Count;
		}

		// Gets the Accessibility object of the GroupViewItem identified by index.
		public override AccessibleObject GetChild(int index)
		{  
			if (index < this.GetChildCount())
                return this.tree.Nodes[index].AccesibleObject;
			return null;
		}
		public override string Value
		{
			get	{ return this.tree.Text; }
			set	{ this.tree.Text = value; }
		}
		public override AccessibleObject GetFocused()
		{
            if (this.tree.Focused)
            {
                return this;
            }
            return null;
		}

		public override AccessibleObject GetSelected()
		{
			if(this.tree.SelectedNode != null)
				return this.tree.SelectedNode.AccesibleObject;

			return base.GetSelected();
		}
		public override AccessibleObject HitTest(int x, int y)
		{
			TreeNodeAdv node = this.tree.GetNodeAtPoint(this.tree.PointToClient(new Point(x, y)));
			if(node != null)
				return node.AccesibleObject;
			else
				return base.HitTest(x, y);
		}

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {           
            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    if (this.tree.Root.Expanded)
                        return this.tree.Root.FirstNode.AccesibleObject;
                    break;
                case AccessibleNavigation.LastChild:
                    if (this.tree.Root != null && this.tree.Root.Expanded)
                        return this.tree.Root.LastNode.AccesibleObject;
                    break;
            }
            return null;
        }
	}

	[Documentation.DocumentationExclude()]
	public class TreeNodeAdvAccessibleObject : AccessibleObject
	{
		TreeNodeAdv node;
		public TreeNodeAdvAccessibleObject(TreeNodeAdv node)
		{
			this.node = node;
		}
		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
            TreeNodeAdv nodeValue = null;
			switch(navdir)
			{
				case AccessibleNavigation.FirstChild:
                    if (this.node.FirstNode != null && this.node.Expanded)
						nodeValue= this.node.FirstNode;
                    break;
				case AccessibleNavigation.LastChild:
                    if (this.node.LastNode != null && this.node.Expanded)
						nodeValue= this.node.LastNode;
                    break;            
                case AccessibleNavigation.Next:
                    {
                        if (this.node.Parent != null && this.node.TreeView.Root != this.node)
                        {
                            if (this.node.parent.Nodes.Count > (this.node.Index + 1))
                            {
                                nodeValue= this.node.parent.Nodes[this.node.Index + 1];
                            }
                        }
                    }
                    break;
                case AccessibleNavigation.Previous:
                    {
                        if (this.node.Parent != null && this.node.TreeView.Root != this.node)
                        {
                            if ((this.node.Index - 1) > -1)
                            {
                                nodeValue=this.node.parent.Nodes[this.node.Index - 1];
                            }
                        }
                    }
                    break;
			}

            if (nodeValue != null)
            {
                return nodeValue.AccesibleObject;
            }
			return null;
		}
		public override void DoDefaultAction()
		{
			if(this.node.HasChildren 
				|| (this.node.TreeView.LoadOnDemand && !this.node.ExpandedOnce))
				this.node.Expanded = !this.node.Expanded;
			else
				this.Select(AccessibleSelection.TakeSelection);
		}

		public override void Select(AccessibleSelection flags)
		{
			if(this.node == this.node.TreeView.Root)
				return;

			if ((flags & AccessibleSelection.TakeFocus) != 0) 
			{
				if (!this.node.TreeView.Focused)
					this.node.TreeView.Focus();
			}
			if((flags & AccessibleSelection.TakeSelection) != 0)
			{
				this.node.TreeView.SelectedNode = this.node;
			}
			else if((flags & AccessibleSelection.ExtendSelection) != 0)
			{
				this.node.TreeView.ExtendSelectionTo(this.node);
			}
			else if((flags & AccessibleSelection.AddSelection) != 0)
			{
				this.node.TreeView.SelectedNodes.Add(this.node);
			}
			else if((flags & AccessibleSelection.RemoveSelection) != 0)
			{
				this.node.TreeView.SelectedNodes.Remove(this.node);
			}
		}

		public override AccessibleStates State 
		{ 
			get
			{
				AccessibleStates astates = AccessibleStates.None;				
				if(this.node.TreeView != null)
				{
					if(this.node.Enabled)
						astates |= AccessibleStates.Selectable;
					else
						astates |= AccessibleStates.Unavailable;

					if(this.node.Checked)
						astates |= AccessibleStates.Checked;
					if(this.node.Expanded)
						astates |= AccessibleStates.Expanded;
					else
						astates |= AccessibleStates.Collapsed;
					
					if(this.node.TreeView.SelectionMode != TreeSelectionMode.Single)
						astates |= AccessibleStates.ExtSelectable;
					if(this.node.TreeView.HotTracking)
						astates |= AccessibleStates.HotTracked;
					if(!this.node.Visible)
						astates |= AccessibleStates.Offscreen|AccessibleStates.Invisible;
					if(this.node.IsSelected)
						astates |= AccessibleStates.Selected;
                    else
                        astates |=~ AccessibleStates.Selected;
				}
				else
				{
					astates |= AccessibleStates.Offscreen|AccessibleStates.Invisible;
				}

				return astates;
			} 
		}
		public override AccessibleRole Role 
		{ 
			get	{ return AccessibleRole.OutlineItem; }
		}
            
		public override AccessibleObject Parent 
		{ 
			get 
			{
				if (this.node.Parent != null && this.node.Parent!=this.node.TreeView.Root)
                    return this.node.Parent.AccesibleObject;
                else
                    return this.node.TreeView.AccessibilityObject;
			}

		}
            
		public override string Name 
		{ 
			get { return this.node.Text; }
		}
            
		public override string DefaultAction 
		{ 
			get 
			{ 
				if(this.node.HasChildren
					|| (this.node.TreeView.LoadOnDemand && !this.node.ExpandedOnce))
				{
					if(this.node.Expanded)
						return "Collapse";
					else
						return "Expand";
				}
				return "Select"; 
			} 
		}
            
		public override Rectangle Bounds 
		{ 
			get	
			{
                int x = this.node.TreeView.PointToScreen(this.node.Bounds.Location).X;
                int y = this.node.TreeView.PointToScreen(this.node.Bounds.Location).Y;

                if (this.node.TreeView.FullRowSelect)
                    return new Rectangle(x, y, node.TreeView.ClientRectangle.Width, node.Height);

                return new Rectangle(x + this.node.NodeX, y, node.Width, node.Height);               
			} 
		}

        public override AccessibleObject GetFocused()
        {
            if (this.node.TreeView.Focused)
            {
                return this.node.TreeView.AccessibilityObject;
            }
            return null;
        }

		public override string Description
		{
			get	
			{ 
				return this.node.Text;
			}
		}

        public override AccessibleObject GetChild(int index)
        {
            return this.node.Nodes[index].AccesibleObject;
        }

        public override int GetChildCount()
        {
            return this.node.Nodes.Count;
        }

        public override string Value
        {
            get
            {
                return GetLevel();
            }
            set
            {
                base.Value = GetLevel();
            }
        }

        #region HelperMethods
        private string GetLevel()
        {
            TreeNodeAdv adv = this.node;
            int count = -1;
            while (adv != null && adv != adv.TreeView.Root)
            {
                count++;
                adv = adv.parent;
            }
            return count.ToString();
        }
        #endregion
    }
}