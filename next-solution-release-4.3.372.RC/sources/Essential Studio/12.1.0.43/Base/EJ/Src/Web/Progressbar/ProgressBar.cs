#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public class ProgressBar:Control
    {
        public ProgressBarProperties ProgressBarModel
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
            get { return "ejProgressBar"; }
        }

        protected override object Model
        {
            get { return this.ProgressBarModel; }
        }
        public ProgressBar() { }
        public ProgressBar(String id, ProgressBarProperties propModel)
        {
            this.ID = id;
            this.ProgressBarModel = propModel;
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
