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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class MenuFields : MenuDataItems
    {
        #region Fields
        private object child = new object();
        #endregion
        #region Properties
        //object values
        [JsonProperty("child")]
        public object Child
        {
            get { return this.child; }
            set { this.child = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeChild()
        {
            if (Utils.PropertyCompare(Child, new MenuFields()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
   
    public class MenuFieldsBuilder
    {
        private MenuFields fields = new MenuFields();
        public MenuFieldsBuilder(MenuFields fields)
        {
            this.fields = fields;
        }
        public MenuFieldsBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            this.fields.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public MenuFieldsBuilder Datasource(DataSource dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }
        public MenuFieldsBuilder Datasource(IEnumerable dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }
        public MenuFieldsBuilder Child(Action<MenuFieldsBuilder> child)
        {
            var ds = new MenuFields();
            this.fields.Child = ds;
            var builder = new MenuFieldsBuilder(ds);
            if (child != null)
                child.Invoke(builder);
            return this;
        }
        public MenuFieldsBuilder Query(String query)
        {
            this.fields.Query = query;
            return this;
        }
        public MenuFieldsBuilder TableName(String tableName)
        {
            this.fields.TableName = tableName;
            return this;
        }
        public MenuFieldsBuilder Id(String id)
        {
            this.fields.Id = id;
            return this;
        }
        public MenuFieldsBuilder ParentId(String parentId)
        {
            this.fields.ParentId = parentId;
            return this;
        }
        public MenuFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public MenuFieldsBuilder SpriteCssClass(String spriteCssClass)
        {
            this.fields.SpriteCssClass = spriteCssClass;
            return this;
        }
        public MenuFieldsBuilder Expanded(String expanded)
        {
            this.fields.Expanded = expanded;
            return this;
        }
        public MenuFieldsBuilder HasChild(String hasChild)
        {
            this.fields.HasChild = hasChild;
            return this;
        }
        public MenuFieldsBuilder Selected(String selected)
        {
            this.fields.Selected = selected;
            return this;
        }
        public MenuFieldsBuilder LinkAttribute(String linkAttribute)
        {
            this.fields.LinkAttribute = linkAttribute;
            return this;
        }
        public MenuFieldsBuilder Value(String value)
        {
            this.fields.Value = value;
            return this;
        }
        public MenuFieldsBuilder ImageAttribute(String imageAttribute)
        {
            this.fields.ImageAttribute = imageAttribute;
            return this;
        }
        public MenuFieldsBuilder HtmlAttribute(String htmlAttribute)
        {
            this.fields.HtmlAttribute = htmlAttribute;
            return this;
        }
        public MenuFieldsBuilder ImageUrl(String imageUrl)
        {
            this.fields.ImageUrl = imageUrl;
            return this;
        }
        public MenuFieldsBuilder IsChecked(String isChecked)
        {
            this.fields.IsChecked = isChecked;
            return this;
        }
    }
}
