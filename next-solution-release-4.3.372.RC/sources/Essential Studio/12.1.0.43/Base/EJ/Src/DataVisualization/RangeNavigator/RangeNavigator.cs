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
using System.Web;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class RangeNavigator : Control
    {
        public RangeNavigatorProperties RangeNavigatorModel
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
            get { return "ejRangeNavigator"; }
        }
        protected override object Model
        {
            get { return this.RangeNavigatorModel; }
        }
        public RangeNavigator() { }
        public RangeNavigator(String id, RangeNavigatorProperties propModel)
        {
            this.ID = id;
            this.RangeNavigatorModel = propModel;
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
