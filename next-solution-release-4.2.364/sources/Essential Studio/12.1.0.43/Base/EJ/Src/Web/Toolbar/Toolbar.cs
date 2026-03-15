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
    public class Toolbar : Control
    {
        public ToolbarProperties ToolbarModel
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
            get { return "ejToolbar"; }
        }
        protected override object Model
        {
            get { return this.ToolbarModel; }
        }
        public Toolbar() { }
        public Toolbar(String id, ToolbarProperties propModel)
        {
            this.ID = id;
            this.ToolbarModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            HtmlTag tag = new HtmlTag(TagName);
            if (this.ToolbarModel.Items == null)
            {
                tag.Id(controlId);
            }
            else
            {
                HtmlTag ultag = new HtmlTag("ul");
                foreach (var toolItem in this.ToolbarModel.Items)
                {
                    HtmlTag litag = new HtmlTag("li");
                    if (!String.IsNullOrEmpty(toolItem.Id))
                        litag.Attributes("id", toolItem.Id);
                    if (!String.IsNullOrEmpty(toolItem.TooltipText))
                        litag.Attributes("title", toolItem.TooltipText);
                    if (!String.IsNullOrEmpty(toolItem.Text))
                        litag.Text(toolItem.Text);

                    if (!String.IsNullOrEmpty(toolItem.SpriteCSS))
                    {
                        HtmlTag sptag = new HtmlTag("div");
                        sptag.AddClass(toolItem.SpriteCSS);
                        litag.Add(sptag);
                    }

                    if (!String.IsNullOrEmpty(toolItem.ImageUrl))
                    {
                        HtmlTag imgtag = new HtmlTag("img");
                        imgtag.Attributes("src", toolItem.ImageUrl);
                        imgtag.Attributes("style", toolItem.ImageAttributes);
                        litag.Add(imgtag);
                    }
                    if (!String.IsNullOrEmpty(toolItem.HtmlAttributes))
                    {
                        litag.Attributes("style", toolItem.HtmlAttributes);
                    }
                    ultag.Add(litag);
                }

                tag.Id(controlId).Add(ultag).Style("visibility","visible");
            }
            return new HtmlString(String.Format(tag.ToString()));
        }
    }
}
