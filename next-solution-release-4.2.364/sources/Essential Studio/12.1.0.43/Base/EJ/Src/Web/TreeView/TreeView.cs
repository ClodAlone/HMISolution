#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class TreeView:Control
    {
         public TreeViewProperties TreeViewModel
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {
                return "div";
            }
        }
        public override string PluginName
        {
            get { return "ejTreeView"; }
        }
        protected override object Model
        {
            get { return this.TreeViewModel; }
        }
        public TreeView() { }
        public TreeView(String id, TreeViewProperties propModel)
        {
            this.ID = id;
            this.TreeViewModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            if ((this.TreeViewModel.Items.Count == 0) && (!this.TreeViewModel.ShouldSerializeTreeViewFields()))
            {
                return new HtmlString("");
            }
            //var s = this.TreeViewModel.Items;
            TagBuilder tag = new TagBuilder(TagName);
            if (this.TreeViewModel.Items == null)
            {
                tag.Attributes.Add("id", controlId);
            }
            else
            {
                TagBuilder ultag=new TagBuilder("ul");
                foreach (var treeviewItem in this.TreeViewModel.Items)
                {
                    ultag.InnerHtml += RenderTreeItems(treeviewItem);
                }
                tag.InnerHtml += ultag;
                tag.Attributes.Add("id", controlId);
            }
            return new HtmlString(String.Format(tag.ToString()));
        }
        internal string RenderTreeItems(TreeViewBaseItem item)
        {
            TagBuilder litag = new TagBuilder("li");
            if (!String.IsNullOrEmpty(item.Text))
            {
                TagBuilder atag = new TagBuilder("a");
                atag.InnerHtml = item.Text;
                if (!String.IsNullOrEmpty(item.Url))
                    atag.Attributes.Add("href", item.Url);
                litag.InnerHtml += atag.ToString();
            }

            if (!String.IsNullOrEmpty(item.HtmlAttributes))
            {
                litag.Attributes.Add("style", item.HtmlAttributes);
            }
            if (item.ChildItems.Count > 0)
            {
                TagBuilder childNode = new TagBuilder("ul");
                foreach (var childItems in item.ChildItems)
                {
                    childNode.InnerHtml += RenderTreeItems(childItems);
                }
                litag.InnerHtml += childNode.ToString();
            }
            return litag.ToString();
        }
       
    }
}
