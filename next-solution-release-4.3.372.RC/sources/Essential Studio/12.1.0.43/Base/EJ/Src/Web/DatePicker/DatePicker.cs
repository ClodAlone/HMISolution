#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
   public class DatePicker: Control
    {
        public DatePickerProperties DatePickerModel
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {               
                return "input";         
            }
        }
        public override string PluginName
        {
            get { return "ejDatePicker"; }
        }
        protected override object Model
        {
            get { return this.DatePickerModel; }
        }
        public DatePicker() { }
        public DatePicker(String id, DatePickerProperties propModel)
        {
            this.ID = id;
            this.DatePickerModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            string tagValue = null ;
            StringBuilder tag = new StringBuilder();
            tagValue = this.DatePickerModel.TagName == null ? TagName : this.DatePickerModel.TagName;
            tag.Append("<")
               .Append(tagValue)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append("></")
               .Append(tagValue)
               .Append(">");
            return new HtmlString(String.Format(tag.ToString()));
        }
    }
}
