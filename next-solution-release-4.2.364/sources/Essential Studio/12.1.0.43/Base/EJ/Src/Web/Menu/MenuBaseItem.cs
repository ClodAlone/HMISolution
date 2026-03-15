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
    public class MenuBaseItem
    {
        #region constructor
        public MenuBaseItem()
        {
            this.Text = string.Empty;
            this.Url = string.Empty;
            this.ChildItems = new List<MenuBaseItem>();
            this.HtmlAttributes = string.Empty;
        }
        #endregion
        #region Properties

        public string Id { get; set; }

        public string Url { get; set; }

        public string Text { get; set; }

        public string HtmlAttributes { get; set; }

        public List<MenuBaseItem> ChildItems { get; set; }

        #endregion
        public bool ShouldSerializeMenuBaseItem()
        {
            if (Utils.PropertyCompare(ChildItems, new MenuBaseItem()))
                return true;
            else
                return false;
        }
    }
  
}
namespace Syncfusion.JavaScript
{
    
   
    public class MenuBaseItemBuilder
    {
        private MenuBaseItem Item;

        #region Constructor

        public MenuBaseItemBuilder(MenuBaseItem item)
        {
            this.Item = item;
        }

        #endregion
        public MenuBaseItemBuilder Id(string itemId)
        {
            this.Item.Id = itemId;
            return this;
        }
        public MenuBaseItemBuilder Url(string itemUrl)
        {
            this.Item.Url = itemUrl;
            return this;
        }
        public MenuBaseItemBuilder Text(string itemText)
        {
            this.Item.Text = itemText;
            return this;
        }
        public MenuBaseItemBuilder HtmlAttributes(string itemHtmlAttributes)
        {
            this.Item.HtmlAttributes = itemHtmlAttributes;
            return this;
        }

        public MenuBaseItemBuilder Children(Action<MenuBaseItemAdder> child)
        {
            this.Item.ChildItems = new List<MenuBaseItem>();
            MenuBaseItemAdder addChildren = new MenuBaseItemAdder(this.Item.ChildItems);
            child.Invoke(addChildren);
            return this as MenuBaseItemBuilder;
        }
      
    }
    public class MenuBaseItemAdder
    {

        private List<MenuBaseItem> ItemList;
        #region Constructor

        public MenuBaseItemAdder(List<MenuBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public MenuBaseItemBuilder Add()
        {
            MenuBaseItem newMenu = new MenuBaseItem();
            this.ItemList.Add(newMenu);
            return new MenuBaseItemBuilder(newMenu);
        }

    }
}
