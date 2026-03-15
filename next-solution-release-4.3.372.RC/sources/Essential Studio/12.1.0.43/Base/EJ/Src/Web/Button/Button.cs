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
    public class Button: Control
    {
        public ButtonProperties ButtonModel
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {
                return "button";
            }
        }
        public override string PluginName
        {
            get { return "ejButton"; }
        }
        protected override object Model
        {
            get { return this.ButtonModel; }
        }
        public Button() { }
        public Button(String id, ButtonProperties propModel)
        {
            this.ID = id;
            this.ButtonModel = propModel;
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
