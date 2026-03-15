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
using Syncfusion.JavaScript;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class TreeViewBaseItem
    {
        #region constructor
        public TreeViewBaseItem()
        {
            this.Text = string.Empty;
            this.Url = string.Empty;
            this.ChildItems = new List<TreeViewBaseItem>();
            this.HtmlAttributes = string.Empty;
        }
        #endregion
        #region Properties

        public string Id { get; set; }

        public string Url { get; set; }

        public string Text { get; set; }

        public string HtmlAttributes { get; set; }

        public List<TreeViewBaseItem> ChildItems { get; set; }

        #endregion
        public bool ShouldSerializeTreeViewBaseItem()
        {
            if (Utils.PropertyCompare(ChildItems, new TreeViewBaseItem()))
                return true;
            else
                return false;
        }
    }
}
namespace Syncfusion.JavaScript
{


    public class TreeViewBaseItemBuilder
    {
        private TreeViewBaseItem Item;

        #region Constructor

        public TreeViewBaseItemBuilder(TreeViewBaseItem item)
        {
            this.Item = item;
        }

        #endregion
        public TreeViewBaseItemBuilder Id(string itemId)
        {
            this.Item.Id = itemId;
            return this;
        }
        public TreeViewBaseItemBuilder Url(string itemUrl)
        {
            this.Item.Url = itemUrl;
            return this;
        }
        public TreeViewBaseItemBuilder Text(string itemText)
        {
            this.Item.Text = itemText;
            return this;
        }
        public TreeViewBaseItemBuilder HtmlAttributes(string itemHtmlAttributes)
        {
            this.Item.HtmlAttributes = itemHtmlAttributes;
            return this;
        }

        public TreeViewBaseItemBuilder Children(Action<TreeViewBaseItemAdder> child)
        {
            this.Item.ChildItems = new List<TreeViewBaseItem>();
            TreeViewBaseItemAdder addChildren = new TreeViewBaseItemAdder(this.Item.ChildItems);
            child.Invoke(addChildren);
            return this as TreeViewBaseItemBuilder;
        }

    }
    public class TreeViewBaseItemAdder
    {

        private List<TreeViewBaseItem> ItemList;
        #region Constructor

        public TreeViewBaseItemAdder(List<TreeViewBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public TreeViewBaseItemBuilder Add()
        {
            TreeViewBaseItem newTreeView = new TreeViewBaseItem();
            this.ItemList.Add(newTreeView);
            return new TreeViewBaseItemBuilder(newTreeView);
        }

    }
}
