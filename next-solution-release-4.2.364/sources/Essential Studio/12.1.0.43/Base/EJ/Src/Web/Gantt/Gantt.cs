#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Web;
using System.Web.UI;
using Syncfusion.JavaScript.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.JavaScript
{
    public class Gantt : Control 
    {
        public GanttProperties GanttModel
        {
            get;
            set;
        }

        public override string TagName
        {
            get { return "div"; }
        }

        public override string PluginName
        {
            get { return "ejGantt"; }
        }

        protected override object Model
        {
            get { return this.GanttModel; }
        }


        public Gantt(string id)
        {
            this.ID = id;
        }
        public Gantt(string id, GanttProperties propModel)
        {
            this.ID = id;
            this.GanttModel = propModel;
        }


        public override HtmlString CreateContainer(string controlId)
        {
            StringBuilder tag = new StringBuilder();

            tag.Append("<")
               .Append(TagName)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append("></")
               .Append(TagName)
               .Append(">");
            return new HtmlString(String.Format(tag.ToString()));
        }

    }
}