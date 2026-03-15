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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class TreeViewPropertiesBuilder
    {
        public TreeView treeview;

        public TreeViewPropertiesBuilder(TreeView treeview)
        { this.treeview = new TreeView(treeview.ID, treeview.TreeViewModel); }

        public TreeViewPropertiesBuilder()
        {
        }
        //Boolean values
        public TreeViewPropertiesBuilder ShowCheckbox()
        {
            treeview.TreeViewModel.ShowCheckbox = true;
            return this;
        }
        public TreeViewPropertiesBuilder ShowCheckbox(bool showCheckbox)
        {
            treeview.TreeViewModel.ShowCheckbox = showCheckbox;
            return this;
        }
        public TreeViewPropertiesBuilder DragAndDrop()
        {
            treeview.TreeViewModel.DragAndDrop = true;
            return this;
        }
        public TreeViewPropertiesBuilder DragAndDrop(bool dragAndDrop)
        {
            treeview.TreeViewModel.DragAndDrop = dragAndDrop;
            return this;
        }
        public TreeViewPropertiesBuilder DropChild()
        {
            treeview.TreeViewModel.DropChild = true;
            return this;
        }
        public TreeViewPropertiesBuilder DropChild(bool dropChild)
        {
            treeview.TreeViewModel.DropChild = dropChild;
            return this;
        }
        public TreeViewPropertiesBuilder DropSibling()
        {
            treeview.TreeViewModel.DropSibling = true;
            return this;
        }
        public TreeViewPropertiesBuilder DropSibling(bool dropSibling)
        {
            treeview.TreeViewModel.DropSibling = dropSibling;
            return this;
        }
        public TreeViewPropertiesBuilder DragAndDropAcrossControl()
        {
            treeview.TreeViewModel.DragAndDropAcrossControl = true;
            return this;
        }
        public TreeViewPropertiesBuilder DragAndDropAcrossControl(bool dragAndDropAcrossControl)
        {
            treeview.TreeViewModel.DragAndDropAcrossControl = dragAndDropAcrossControl;
            return this;
        }
        public TreeViewPropertiesBuilder AllowEdit()
        {
            treeview.TreeViewModel.AllowEdit = true;
            return this;
        }
        public TreeViewPropertiesBuilder AllowEdit(bool allowEdit)
        {
            treeview.TreeViewModel.AllowEdit = allowEdit;
            return this;
        }
        public TreeViewPropertiesBuilder AllowKeyboardNavigation()
        {
            treeview.TreeViewModel.AllowKeyboardNavigation = true;
            return this;
        }
        public TreeViewPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            treeview.TreeViewModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        public TreeViewPropertiesBuilder AutoCheckParentNode()
        {
            treeview.TreeViewModel.AutoCheckParentNode = true;
            return this;
        }
        public TreeViewPropertiesBuilder AutoCheckParentNode(bool autoCheckParentNode)
        {
            treeview.TreeViewModel.AutoCheckParentNode = autoCheckParentNode;
            return this;
        }
        public TreeViewPropertiesBuilder LoadOnDemand()
        {
            treeview.TreeViewModel.LoadOnDemand = true;
            return this;
        }
        public TreeViewPropertiesBuilder LoadOnDemand(bool loadOnDemand)
        {
            treeview.TreeViewModel.LoadOnDemand = loadOnDemand;
            return this;
        }
        public TreeViewPropertiesBuilder Rtl()
        {
            treeview.TreeViewModel.Rtl = true;
            return this;
        }
        public TreeViewPropertiesBuilder Rtl(bool rtl)
        {
            treeview.TreeViewModel.Rtl = rtl;
            return this;
        }
        public TreeViewPropertiesBuilder Persist()
        {
            treeview.TreeViewModel.Persist = true;
            return this;
        }
        public TreeViewPropertiesBuilder Persist(bool persist)
        {
            treeview.TreeViewModel.Persist = persist;
            return this;
        }
        public TreeViewPropertiesBuilder Enabled()
        {
            treeview.TreeViewModel.Enabled = true;
            return this;
        }
        public TreeViewPropertiesBuilder Enabled(bool enabled)
        {
            treeview.TreeViewModel.Enabled = enabled;
            return this;
        }
        //object values
        // fields
        public TreeViewPropertiesBuilder TreeViewFields(Action<TreeViewFieldsBuilder> fields)
        {
            var flds = new TreeViewFields();
            treeview.TreeViewModel.TreeViewFields = flds;
            var builder = new TreeViewFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //Int Values
        public TreeViewPropertiesBuilder ExpandedNodes(List<int> expandedNodes)
        {
            treeview.TreeViewModel.ExpandedNodes = expandedNodes;
            return this;
        }
        //String Values
        public TreeViewPropertiesBuilder CssClass(String cssClass)
        {
            treeview.TreeViewModel.CssClass = cssClass;
            return this;
        }
        public TreeViewPropertiesBuilder Template(String template)
        {
            treeview.TreeViewModel.Template = template;
            return this;
        }
        public TreeViewPropertiesBuilder ExpandEvent(String expandEvent)
        {
            treeview.TreeViewModel.ExpandEvent = expandEvent;
            return this;
        }
        public TreeViewPropertiesBuilder Width(String width)
        {
            treeview.TreeViewModel.Width = width;
            return this;
        }
        public TreeViewPropertiesBuilder Height(String height)
        {
            treeview.TreeViewModel.Height = height;
            return this;
        }
        //
        public TreeViewPropertiesBuilder Items(Action<TreeViewBaseItemAdder> items)
        {
            this.ItemsCollection = new List<TreeViewBaseItem>();
            TreeViewBaseItemAdder accordionAdded = new TreeViewBaseItemAdder(this.treeview.TreeViewModel.Items);
            items.Invoke(accordionAdded);
            return this as TreeViewPropertiesBuilder;
        }
        //Events
        public TreeViewPropertiesBuilder ClientSideEvents(Action<TreeViewClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new TreeViewClientSideEventsBuilder(this.treeview.TreeViewModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(treeview.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        internal List<TreeViewBaseItem> ItemsCollection { get; set; }
    }
}
