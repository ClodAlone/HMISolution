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
   public class Tab:Control
    {
        public TabProperties TabModel
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
            get { return "ejTab"; }
        }
        protected override object Model
        {
            get { return this.TabModel; }
        }
        public Tab() { }
        public Tab(String id, TabProperties propModel)
        {
            this.ID = id;
            this.TabModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            if(this.TabModel.Items.Count==0)
            {
                return new HtmlString("");
            }
            //StringBuilder tag = new StringBuilder();

            //tag.Append("<")
            //   .Append(TagName)
            //   .Append(" id=\"")
            //   .Append(controlId + "\"")
            //   .Append("></")
            //   .Append(TagName)
            //   .Append(">");
            HtmlTag tag = new HtmlTag(TagName,tab=>
            {
                HtmlTag ultag = new HtmlTag("ul");
                foreach (var pane in this.TabModel.Items)
                {
                    HtmlTag litag = new HtmlTag("li");
                    if (pane.Text != null)
                    {
                        HtmlTag linkingtag = new HtmlTag("a");
                        linkingtag.Attributes("href", "#"+pane.ID);
                        linkingtag.Text(pane.Text);
                        litag.Add(linkingtag);
                    }
                    ultag.Add(litag);
                }
                    tab.Add(ultag);

                    foreach (var pane in this.TabModel.Items)
                    {
                        HtmlTag PaneDiv = new HtmlTag("div");
                        PaneDiv.Id(pane.ID);
                        if (pane.ContentTemplate.WebFormDataTemplate != null || pane.ContentTemplate.RazorViewTemplate != null)
                            {

                                RenderPaneContent(pane, PaneDiv);
                                tab.Add(PaneDiv);
                             }
                    
                }

            });
            tag.Id(controlId);
            return new HtmlString(String.Format(tag.ToString()));
        }
        protected HtmlTag RenderPaneContent(TabBaseItem pane, HtmlTag PaneDiv)
        {
            pane.ContentTemplate.builder(null, PaneDiv);
            return PaneDiv;
        }
    }
}
