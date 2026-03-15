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
    public class Menu : Control
    {
        public MenuProperties MenuModel
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {
                return "ul";
            }
        }
        public override string PluginName
        {
            get { return "ejMenu"; }
        }
        protected override object Model
        {
            get { return this.MenuModel; }
        }
        public Menu() { }
        public Menu(String id, MenuProperties propModel)
        {
            this.ID = id;
            this.MenuModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            TagBuilder tag = new TagBuilder(TagName);
             if (this.MenuModel.Items == null)
             {
                 tag.Attributes.Add("id",controlId);
             }
             else
             {
                 foreach (var menuItem in this.MenuModel.Items)
                 {
                     tag.InnerHtml += RenderMenuItems(menuItem);
                 }
                 tag.Attributes.Add("id", controlId);
             }
            return new HtmlString(String.Format(tag.ToString()));
        }
        internal string RenderMenuItems(MenuBaseItem item)
        {
            TagBuilder litag = new TagBuilder("li");
            if (!String.IsNullOrEmpty(item.Text))
            {
                TagBuilder atag = new TagBuilder("a");
                atag.InnerHtml=item.Text;
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
                    childNode.InnerHtml += RenderMenuItems(childItems);
                }
                litag.InnerHtml += childNode.ToString();
            }
            return litag.ToString();
        }
    }
}
