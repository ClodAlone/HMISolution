#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class Map : Control 
    {
        public MapProperties MapModel
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
            get { return "ejMap"; }
        }

        protected override object Model
        {
            get { return this.MapModel; }
        }

        public Map(string id)
        {
            this.ID = id;
            this.MapModel = new MapProperties();
        }

        public Map(string id, MapProperties propModel)
        {
            this.ID = id;
            this.MapModel = propModel;
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
