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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class Diagram : Control
    {
        public DiagramProperties DiagramModel
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
            get { return "ejDiagram"; }
        }
        protected override object Model
        {
            get 
            { 
                return this.DiagramModel; 
            }
        }
        public Diagram() 
        {
            this.DiagramModel = new DiagramProperties();
        }
        public Diagram(string id, DiagramProperties propModel)
        {
            this.ID = id;
            this.DiagramModel = propModel;
        }
        public override HtmlString CreateContainer(string controlId)
        {
            TagBuilder tag = new TagBuilder(TagName);
            tag.Attributes.Add("id", controlId);
            
            return new HtmlString(String.Format(tag.ToString()));
        }
    }
}
