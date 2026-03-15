#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;


namespace Syncfusion.JavaScript.Models
{
    public class ColumnBase<T> where T:class
    {
        #region private fields

        private String field = null;
        private String headerText = null;
        private bool allowSorting = true;
        private bool allowFiltering = true;
        private TextAlign textAlign =TextAlign.Left;
        private bool visible = true;
        private object dataSource = new object();
        private int width;
        private string cssClass = null;
        private String format = null;
        private Dictionary<String, String> validationRules = new Dictionary<String, String>();

        #endregion private fields

        #region properties

        [JsonProperty("field")]
        [DefaultValue(null)]
        public String Field
        {
            get { return this.field; }
            set { this.field = value; }
        }
        [JsonProperty("headerText")]
        [DefaultValue(null)]
        public String HeaderText
        {
            get { return this.headerText; }
            set { this.headerText = value; }
        }
       
        [JsonProperty("allowSorting")]
        [DefaultValue(true)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }
        [JsonProperty("allowFiltering")]
        [DefaultValue(true)]
        public bool AllowFiltering
        {
            get { return this.allowFiltering; }
            set { this.allowFiltering = value; }
        }
        [JsonProperty("textAlign")]
        [DefaultValue(TextAlign.Left)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlign TextAlign
        {
            get { return this.textAlign; }
            set { this.textAlign = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(0)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        [JsonProperty("format")]
        [DefaultValue(null)]
        public String Format
        {
            get { return this.format; }
            set { this.format = value; }
        }

        [JsonProperty("validationRules")]

        public Dictionary<String, String> ValidationRules
        {
            get { return this.validationRules; }
            set { this.validationRules = value; }
        }

        #endregion properties

    }
}
