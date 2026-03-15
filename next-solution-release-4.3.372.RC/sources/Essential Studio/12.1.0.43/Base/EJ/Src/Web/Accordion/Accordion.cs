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
   public class Accordion : Control
    {
       public AccordionProperties AccordionModel
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
            get { return "ejAccordion"; }
        }
        protected override object Model
        {
            get { return this.AccordionModel; }
        }
        public Accordion() { }
        public Accordion(String id, AccordionProperties propModel)
        {
            this.ID = id;
            this.AccordionModel = propModel;
        }
        public override HtmlString CreateContainer(string controlId)
        {
            if(this.AccordionModel.Items.Count==0)
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

            //var ss = this.AccordionModel.Items;


            HtmlTag tag = new HtmlTag(TagName, accordion =>
            {

                foreach (var pane in this.AccordionModel.Items)
                {
                    HtmlTag headtag = new HtmlTag("h3");
                    if(pane.Text!=null){
                        HtmlTag linkingtag = new HtmlTag("a");
                        linkingtag.Attributes("href", "#");
                        linkingtag.Text(pane.Text);
                        headtag.Add(linkingtag);
                    }
                    accordion.Add(headtag);
                    HtmlTag PaneDiv = new HtmlTag("div");
                    if (pane.ContentTemplate.WebFormDataTemplate != null || pane.ContentTemplate.RazorViewTemplate != null)
                    {
                        
                        RenderPaneContent(pane, PaneDiv);
                      
                    }
                    accordion.Add(PaneDiv);
                }
                
                //HtmlTag content = new HtmlTag("B").Text("Hello");
                //Accordion.Add(content);
            
            });
            tag.Id(controlId);
            
            return new HtmlString(String.Format(tag.ToString()));
        }
        protected HtmlTag RenderPaneContent(AccordionBaseItem pane, HtmlTag PaneDiv)
        {
            pane.ContentTemplate.builder(null, PaneDiv);
            return PaneDiv;
        }

    }
}
