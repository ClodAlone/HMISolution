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
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using System.Collections;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Models
{
    public class TreeViewFields : TreeViewItems
    {
        #region Fields
        private object child = new object();

        #endregion
        #region Fields
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
            if (Utils.PropertyCompare(Child, new TreeViewFields()))
                return true;
            else
                return false;
        }

        #endregion

    }
}
namespace Syncfusion.JavaScript
{
   
    public class TreeViewFieldsBuilder
    {
        private TreeViewFields fields = new TreeViewFields();
      
        public TreeViewFieldsBuilder(TreeViewFields fields)
        {
            this.fields = fields;
        }

        public TreeViewFieldsBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            this.fields.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public TreeViewFieldsBuilder Datasource(DataSource dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }

        public TreeViewFieldsBuilder Datasource(IEnumerable dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }
        public TreeViewFieldsBuilder Child(Action<TreeViewFieldsBuilder> child)
        {
            var ds = new TreeViewFields();
            this.fields.Child = ds;
            var builder = new TreeViewFieldsBuilder(ds);
            if (child != null)
                child.Invoke(builder);
            return this;
        }
      
        public TreeViewFieldsBuilder Query(String query)
        {
            this.fields.Query = query;
            return this;
        }
        public TreeViewFieldsBuilder TableName(String tableName)
        {
            this.fields.TableName = tableName;
            return this;
        }
        public TreeViewFieldsBuilder Id(String id)
        {
            this.fields.Id = id;
            return this;
        }
        public TreeViewFieldsBuilder ParentId(String parentId)
        {
            this.fields.ParentId = parentId;
            return this;
        }
        public TreeViewFieldsBuilder Text(String text)
        {
            this.fields.Text = text;
            return this;
        }
        public TreeViewFieldsBuilder SpriteCssClass(String spriteCssClass)
        {
            this.fields.SpriteCssClass = spriteCssClass;
            return this;
        }
        public TreeViewFieldsBuilder Expanded(String expanded)
        {
            this.fields.Expanded = expanded;
            return this;
        }
        public TreeViewFieldsBuilder HasChild(String hasChild)
        {
            this.fields.HasChild = hasChild;
            return this;
        }
        public TreeViewFieldsBuilder Selected(String selected)
        {
            this.fields.Selected = selected;
            return this;
        }
        public TreeViewFieldsBuilder LinkAttribute(String linkAttribute)
        {
            this.fields.LinkAttribute = linkAttribute;
            return this;
        }
        public TreeViewFieldsBuilder Value(String value)
        {
            this.fields.Value = value;
            return this;
        }
        public TreeViewFieldsBuilder ImageAttribute(String imageAttribute)
        {
            this.fields.ImageAttribute = imageAttribute;
            return this;
        }
        public TreeViewFieldsBuilder HtmlAttribute(String htmlAttribute)
        {
            this.fields.HtmlAttribute = htmlAttribute;
            return this;
        }
        public TreeViewFieldsBuilder ImageUrl(String imageUrl)
        {
            this.fields.ImageUrl = imageUrl;
            return this;
        }
        public TreeViewFieldsBuilder IsChecked(String isChecked)
        {
            this.fields.IsChecked = isChecked;
            return this;
        }

    }
}
