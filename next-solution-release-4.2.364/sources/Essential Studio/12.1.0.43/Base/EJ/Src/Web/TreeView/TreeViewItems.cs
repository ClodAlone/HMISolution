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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using System.Collections;

namespace Syncfusion.JavaScript.Models
{
    public class TreeViewItems
    {
        #region treeViewItems
        private object dataSource = new object();
        private String query = null;
        private String tableName = null;
        private String id = "id";
        private String parentId = "parentId";
        private String text = "text";
        private String spriteCssClass = "spriteCssClass";
        private String expanded = "expanded";
        private String hasChild = "hasChild";
        private String selected = "selected";
        private String linkAttribute = "linkAttribute";
        private String value = "value";
        private String imageAttribute = "imageAttribute";
        private String htmlAttribute = "htmlAttribute";
        private String imageUrl = "imageUrl";
        private String isChecked = "isChecked";
        #endregion
        #region Properties
        //object values
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }
        [JsonProperty("tableName")]
        [DefaultValue(null)]
        public String TableName
        {
            get { return this.tableName; }
            set { this.tableName = value; }
        }
        [JsonProperty("id")]
        [DefaultValue("id")]
        public String Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        [JsonProperty("parentId")]
        [DefaultValue("parentId")]
        public String ParentId
        {
            get { return this.parentId; }
            set { this.parentId = value; }
        }
        [JsonProperty("text")]
        [DefaultValue("text")]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        [JsonProperty("spriteCssClass")]
        [DefaultValue("spriteCssClass")]
        public String SpriteCssClass
        {
            get { return this.spriteCssClass; }
            set { this.spriteCssClass = value; }
        }
        [JsonProperty("expanded")]
        [DefaultValue("expanded")]
        public String Expanded
        {
            get { return this.expanded; }
            set { this.expanded = value; }
        }
        [JsonProperty("hasChild")]
        [DefaultValue("hasChild")]
        public String HasChild
        {
            get { return this.hasChild; }
            set { this.hasChild = value; }
        }
        [JsonProperty("selected")]
        [DefaultValue("selected")]
        public String Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }
        [JsonProperty("linkAttribute")]
        [DefaultValue("linkAttribute")]
        public String LinkAttribute
        {
            get { return this.linkAttribute; }
            set { this.linkAttribute = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("value")]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("imageAttribute")]
        [DefaultValue("imageAttribute")]
        public String ImageAttribute
        {
            get { return this.imageAttribute; }
            set { this.imageAttribute = value; }
        }
        [JsonProperty("htmlAttribute")]
        [DefaultValue("htmlAttribute")]
        public String HtmlAttribute
        {
            get { return this.htmlAttribute; }
            set { this.htmlAttribute = value; }
        }
        [JsonProperty("imageUrl")]
        [DefaultValue("imageUrl")]
        public String ImageUrl
        {
            get { return this.imageUrl; }
            set { this.imageUrl = value; }
        }
        [JsonProperty("isChecked")]
        [DefaultValue("isChecked")]
        public String IsChecked
        {
            get { return this.isChecked; }
            set { this.isChecked = value; }
        }
        #endregion
        #region ShouldSerialize Methods
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