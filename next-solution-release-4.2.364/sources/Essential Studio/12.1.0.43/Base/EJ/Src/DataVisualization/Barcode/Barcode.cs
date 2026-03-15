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
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;

using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class Barcode : Control
    {
        public BarcodeProperties BarcodeModel
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
            get { return "ejBarcode"; }
        }

        protected override object Model
        {
            get { return this.BarcodeModel; }
        }
        
        public Barcode() { }

        public Barcode(String id, BarcodeProperties propModel)
        {
            this.ID = id;
            this.BarcodeModel = propModel;
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

    public class QuietZone
    {
        private int left = 1;
        private int right = 1;
        private int top = 1;
        private int bottom = 1;
        private int all = 1;

        [JsonProperty("left")]
        [DefaultValue(1)]
        public int Left
        {
            get { return this.left; }
            set { this.left = value; }
        }

        [JsonProperty("right")]
        [DefaultValue(1)]
        public int Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        [JsonProperty("top")]
        [DefaultValue(1)]
        public int Top
        {
            get { return this.top; }
            set { this.top = value; }
        }
        
        [JsonProperty("bottom")]
        [DefaultValue(1)]
        public int Bottom
        {
            get { return this.bottom; }
            set { this.bottom = value; }
        }

        [JsonProperty("all")]
        [DefaultValue(1)]
        public int All
        {
            get { return this.all; }
            set { this.all = value; }
        }
    }
}
