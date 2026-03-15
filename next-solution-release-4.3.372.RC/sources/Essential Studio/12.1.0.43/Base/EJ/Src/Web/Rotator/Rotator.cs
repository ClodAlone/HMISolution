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
    public class Rotator : Control
    {
        public RotatorProperties RotatorModel
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
            get { return "ejRotator"; }
        }
        protected override object Model
        {
            get { return this.RotatorModel; }
        }
        public Rotator() { }
        public Rotator(String id, RotatorProperties propModel)
        {
            this.ID = id;
            this.RotatorModel = propModel;
        }
        public override HtmlString CreateContainer(string controlId)
        {
            HtmlTag tag = new HtmlTag(TagName);
            if (this.RotatorModel.Items == null)
                    tag.Id(controlId);
            else
            {
                
                foreach (var pane in this.RotatorModel.Items)
                {
                    HtmlTag litag = new HtmlTag("li");
                    if (!String.IsNullOrEmpty(pane.Url))
                    {
                        HtmlTag imgtag = new HtmlTag("img");
                        if (!String.IsNullOrEmpty(pane.Caption))
                        imgtag.Attributes("title", pane.Caption);
                        imgtag.Attributes("src", pane.Url);
                        litag.Add(imgtag);

                    }
                    else
                    {
                        HtmlTag PaneDiv = new HtmlTag("div");
                        if (pane.ContentTemplate.WebFormDataTemplate != null || pane.ContentTemplate.RazorViewTemplate != null)
                        {
                            RenderPaneContent(pane, PaneDiv);
                            litag.Add(PaneDiv);
                        }
                    }
                    tag.Add(litag);
                }   
            }    
            tag.Id(controlId);
            return new HtmlString(String.Format(tag.ToString()));
        }
        protected HtmlTag RenderPaneContent(RotatorBaseItem pane, HtmlTag PaneDiv)
        {
            pane.ContentTemplate.builder(null, PaneDiv);
            return PaneDiv;
        }
    }
}
