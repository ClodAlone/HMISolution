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
    public class Splitter : Control
    {
        public SplitterProperties SplitterModel
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
            get { return "ejSplitter"; }
        }
        protected override object Model
        {
            get { return this.SplitterModel; }
        }
        public Splitter() { }
        public Splitter(String id, SplitterProperties propModel)
        {
            this.ID = id;
            this.SplitterModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            //StringBuilder tag = new StringBuilder();

            //tag.Append("<")
            //   .Append(TagName)
            //   .Append(" id=\"")
            //   .Append(controlId + "\"")
            //   .Append("></")
            //   .Append(TagName)
            //   .Append(">");
            HtmlTag tag = new HtmlTag(TagName, splitter =>
            {
                foreach (var pane in this.SplitterModel.PaneProperties)
                {
                    HtmlTag PaneDiv = new HtmlTag("div");
                    if (pane.ContentTemplate.WebFormDataTemplate != null || pane.ContentTemplate.RazorViewTemplate != null)
                    {

                        RenderPaneContent(pane, PaneDiv);

                    }
                    splitter.Add(PaneDiv);
                   
                }
                //HtmlTag content = new HtmlTag("B").Text("Hello");
                //Accordion.Add(content);

            });
            tag.Id(controlId);
       
            
            return new HtmlString(String.Format(tag.ToString()));
        }
        protected HtmlTag RenderPaneContent(PaneProperties pane, HtmlTag PaneDiv)
        {
            pane.ContentTemplate.builder(null, PaneDiv);
            return PaneDiv;
        }       
    }
}
