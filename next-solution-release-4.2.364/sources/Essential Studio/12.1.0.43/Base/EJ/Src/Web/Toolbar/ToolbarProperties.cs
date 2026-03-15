#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class ToolbarProperties
    {
        #region Fields
        //Boolean Values
        private bool enabled = true;
        private bool hide = false;
        private bool itemSeparator = true;
        private bool rtl = false;
        private bool roundedCorner = false;
        //Enum Values
        private Orientation orientation = Orientation.Horizontal;
        //Object Values
        private object dataSource = new object();
        private object fields = new ToolbarFields();
        //String Values
        private String cssClass = "";
        private String height = "";
        private String width = "";
        private String query = null;
        //Events
        private String create = null;
        private String click = null;
        private String itemHover = null;
        private String itemLeave = null;
        private String destroy = null;

        //Toolbar
        private Toolbar toolbar = new Toolbar();
        #endregion
        public ToolbarProperties()
        {
            this.Items = new List<ToolbarBaseItem>();
        }
        //public ToolbarProperties(String id, Toolbar toolbar)
        //{
        //    toolbar.ID = id;
        //    this.toolbar = toolbar;
        //}
        #region Properties
        //Boolean values
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("hide")]
        [DefaultValue(false)]
        public bool Hide
        {
            get { return this.hide; }
            set { this.hide = value; }
        }
        [JsonProperty("itemSeparator")]
        [DefaultValue(true)]
        public bool ItemSeparator
        {
            get { return this.itemSeparator; }
            set { this.itemSeparator = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        //enum values
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }
        //object values
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("fields")]
        public object ToolbarFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //String values
        [JsonProperty("height")]
        [DefaultValue("")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue("")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("click")]
        [DefaultValue(null)]
        public String Click
        {
            get { return this.click; }
            set { this.click = value; }
        }
        [JsonProperty("itemHover")]
        [DefaultValue(null)]
        public String ItemHover
        {
            get { return this.itemHover; }
            set { this.itemHover = value; }
        }
        [JsonProperty("itemLeave")]
        [DefaultValue(null)]
        public String ItemLeave
        {
            get { return this.itemLeave; }
            set { this.itemLeave = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        
        [JsonIgnore]
        public List<ToolbarBaseItem> Items
        {
            get;
            set;
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeToolbarFields()
        {
            if (Utils.PropertyCompare(ToolbarFields, new ToolbarFields()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeToolbarBaseItem()
        {
            if (Utils.PropertyCompare(Items, new ToolbarBaseItem()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDataSource()
        {
            if (typeof(DataSource).IsAssignableFrom(this.DataSource.GetType()))
            {
                if (Utils.PropertyCompare(DataSource, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.DataSource is IEnumerable)
            {
                ICollection data = DataSource as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

        #endregion
    }
}
