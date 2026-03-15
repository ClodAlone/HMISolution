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
    public class TagCloudProperties
    {
        #region Fields
        //Boolean Values
        private bool showTitle = true;
        private bool rtl = false;

        //Enumeration Values
        private Formats format = Formats.Cloud;

        //Object Values
        private object dataSource = new object();
        private object fields = new TagCloudFields();
        

        //String Values
        private String cssClass = "";
        private String title = "Title";
        private String titleImage = null;
        private String minFontSize = "10px";
        private String maxFontSize = "40px";
        private String query = null;
        
        //Events
        private String create = null;
        private String mouseover = null;
        private String mouseout = null;
        private String click = null;
        private String destroy = null;
        #endregion
        //Auto Complete
        private TagCloud tagcloud = new TagCloud();
       
        public TagCloudProperties() {  }
        //public TagCloudProperties(String id, TagCloud tagcloud)
        //{
        //    tagcloud.ID = id;
        //    this.tagcloud= tagcloud;
        //}

        #region Properties
        //Boolean values
        [JsonProperty("showTitle")]
        [DefaultValue(true)]
        public bool ShowTitle
        {
            get { return this.showTitle; }
            set { this.showTitle = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        //Enum values
        [JsonProperty("format")]
        [DefaultValue(Formats.Cloud)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Formats Format
        {
            get { return this.format; }
            set { this.format = value; }
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
        public object TagCloudFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //String
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("title")]
        [DefaultValue("Title")]
        public String Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        [JsonProperty("titleImage")]
        [DefaultValue(null)]
        public String TitleImage
        {
            get { return this.titleImage; }
            set { this.titleImage = value; }
        }
        [JsonProperty("minFontSize")]
        [DefaultValue("10px")]
        public String MinFontSize
        {
            get { return this.minFontSize; }
            set { this.minFontSize = value; }
        }
        [JsonProperty("maxFontSize")]
        [DefaultValue("40px")]
        public String MaxFontSize
        {
            get { return this.maxFontSize; }
            set { this.maxFontSize = value; }
        }
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("mouseover")]
        [DefaultValue(null)]
        public String MouseOver
        {
            get { return this.mouseover; }
            set { this.mouseover = value; }
        }
        [JsonProperty("mouseout")]
        [DefaultValue(null)]
        public String MouseOut
        {
            get { return this.mouseout; }
            set { this.mouseout = value; }
        }
        [JsonProperty("click")]
        [DefaultValue(null)]
        public String Click
        {
            get { return this.click; }
            set { this.click = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeTagCloudFields()
        {
            if (Utils.PropertyCompare(TagCloudFields, new TagCloudFields()))
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
