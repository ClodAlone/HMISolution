#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class TreeViewClientSideEventsBuilder
    {
        private TreeViewProperties treeviewModel;
        public TreeViewClientSideEventsBuilder(TreeViewProperties treeviewProp)
        {
            treeviewModel = treeviewProp;
        }
        //Events
        public TreeViewClientSideEventsBuilder Create(String create)
        {
            treeviewModel.Create = create;
            return this;
        }
        public TreeViewClientSideEventsBuilder Click(String click)
        {
            treeviewModel.Click = click;
            return this;
        }
        public TreeViewClientSideEventsBuilder BeforeExpand(String beforeExpand)
        {
            treeviewModel.BeforeExpand = beforeExpand;
            return this;
        }
        public TreeViewClientSideEventsBuilder Expand(String expand)
        {
            treeviewModel.Expand = expand;
            return this;
        }

        public TreeViewClientSideEventsBuilder BeforeEdit(String beforeEdit)
        {
            treeviewModel.BeforeEdit = beforeEdit;
            return this;
        }
        public TreeViewClientSideEventsBuilder BeforeCollapse(String beforeCollapse)
        {
            treeviewModel.BeforeCollapse = beforeCollapse;
            return this;
        }
        public TreeViewClientSideEventsBuilder Collapse(String collapse)
        {
            treeviewModel.Collapse = collapse;
            return this;
        }
        public TreeViewClientSideEventsBuilder Select(String select)
        {
            treeviewModel.Select = select;
            return this;
        }
        public TreeViewClientSideEventsBuilder Check(String check)
        {
            treeviewModel.Check = check;
            return this;
        }
        public TreeViewClientSideEventsBuilder Uncheck(String uncheck)
        {
            treeviewModel.Uncheck = uncheck;
            return this;
        }
        public TreeViewClientSideEventsBuilder InlineEditValidation(String inlineEditValidation)
        {
            treeviewModel.InlineEditValidation = inlineEditValidation;
            return this;
        }
        public TreeViewClientSideEventsBuilder KeyPress(String keyPress)
        {
            treeviewModel.KeyPress = keyPress;
            return this;
        }
        public TreeViewClientSideEventsBuilder DragStart(String dragStart)
        {
            treeviewModel.DragStart = dragStart;
            return this;
        }
        public TreeViewClientSideEventsBuilder Drag(String drag)
        {
            treeviewModel.Drag = drag;
            return this;
        }
        public TreeViewClientSideEventsBuilder DragStop(String dragStop)
        {
            treeviewModel.DragStop = dragStop;
            return this;
        }
        public TreeViewClientSideEventsBuilder Dropped(String dropped)
        {
            treeviewModel.Dropped = dropped;
            return this;
        }
        public TreeViewClientSideEventsBuilder Destroy(String destroy)
        {
            treeviewModel.Destroy = destroy;
            return this;
        }

    }
}
