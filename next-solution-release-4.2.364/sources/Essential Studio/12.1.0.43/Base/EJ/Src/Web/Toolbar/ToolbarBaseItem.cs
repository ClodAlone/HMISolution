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
    public class ToolbarBaseItem
    {
        #region Constructor
        public ToolbarBaseItem()
        {
            this.Id = string.Empty;
            this.TooltipText = string.Empty;
            this.ImageUrl = string.Empty;
            this.ImageAttributes = string.Empty;
            this.SpriteCSS = string.Empty;
            this.Text = string.Empty;
            this.HtmlAttributes = string.Empty;
        }
        #endregion
        #region Properties
        public string Id { get; set; }

        public string TooltipText { get; set; }

        public string ImageUrl { get; set; }

        public string ImageAttributes { get; set; }

        public string SpriteCSS { get; set; }

        public string Text { get; set; }

        public string HtmlAttributes { get; set; }

        #endregion

    }

}
namespace Syncfusion.JavaScript
{

    public class ToolbarBaseItemBuilder
    {

        #region Constructor

        internal ToolbarBaseItem Item { get; set; }

        public ToolbarBaseItemBuilder(ToolbarBaseItem item)
        {
            this.Item = item;
        }
        #endregion
        public ToolbarBaseItemBuilder Id(string itemId)
        {
            this.Item.Id = itemId;
            return this;
        }
        public ToolbarBaseItemBuilder TooltipText(string itemTooltipText)
        {
            this.Item.TooltipText = itemTooltipText;
            return this;
        }
        public ToolbarBaseItemBuilder ImageUrl(string itemImageUrl)
        {
            this.Item.ImageUrl = itemImageUrl;
            return this;
        }
        public ToolbarBaseItemBuilder ImageAttributes(string itemImageAttributes)
        {
            this.Item.ImageAttributes = itemImageAttributes;
            return this;
        }
        public ToolbarBaseItemBuilder SpriteCSS(string itemSpriteCSS)
        {
            this.Item.SpriteCSS = itemSpriteCSS;
            return this;
        }
        public ToolbarBaseItemBuilder Text(string itemText)
        {
            this.Item.Text = itemText;
            return this;
        }
        public ToolbarBaseItemBuilder HtmlAttributes(string itemHtmlAttributes)
        {
            this.Item.HtmlAttributes = itemHtmlAttributes;
            return this;
        }

    }
    public class ToolbarBaseItemAdder
    {

        private List<ToolbarBaseItem> ItemList;
        #region Constructor

        public ToolbarBaseItemAdder(List<ToolbarBaseItem> itemList)
        {
            this.ItemList = itemList;
        }

        #endregion

        public ToolbarBaseItemBuilder Add()
        {
            ToolbarBaseItem newToolbar = new ToolbarBaseItem();
            this.ItemList.Add(newToolbar);
            return new ToolbarBaseItemBuilder(newToolbar);
        }
    }
}
