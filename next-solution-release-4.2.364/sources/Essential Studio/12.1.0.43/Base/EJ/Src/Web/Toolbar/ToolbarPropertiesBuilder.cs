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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class ToolbarPropertiesBuilder
    {
        public Toolbar toolbar;
        public ToolbarPropertiesBuilder(Toolbar toolbar)
        { this.toolbar = new Toolbar(toolbar.ID, toolbar.ToolbarModel); }

        public ToolbarPropertiesBuilder()
        {
        }
        //Boolean values
        public ToolbarPropertiesBuilder Enabled()
        {
            toolbar.ToolbarModel.Enabled = true;
            return this;
        }
        public ToolbarPropertiesBuilder Enabled(bool enabled)
        {
            toolbar.ToolbarModel.Enabled = enabled;
            return this;
        }
        public ToolbarPropertiesBuilder Hide()
        {
            toolbar.ToolbarModel.Hide = true;
            return this;
        }
        public ToolbarPropertiesBuilder Hide(bool hide)
        {
            toolbar.ToolbarModel.Hide = hide;
            return this;
        }
        public ToolbarPropertiesBuilder ItemSeparator()
        {
            toolbar.ToolbarModel.ItemSeparator = true;
            return this;
        }
        public ToolbarPropertiesBuilder ItemSeparator(bool itemSeparator)
        {
            toolbar.ToolbarModel.ItemSeparator = itemSeparator;
            return this;
        }
        public ToolbarPropertiesBuilder Rtl()
        {
            toolbar.ToolbarModel.Rtl = true;
            return this;
        }
        public ToolbarPropertiesBuilder Rtl(bool rtl)
        {
            toolbar.ToolbarModel.Rtl = rtl;
            return this;
        }
        public ToolbarPropertiesBuilder RoundedCorner()
        {
            toolbar.ToolbarModel.RoundedCorner = true;
            return this;
        }
        public ToolbarPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            toolbar.ToolbarModel.RoundedCorner = roundedCorner;
            return this;
        }
        //object values
        public ToolbarPropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            toolbar.ToolbarModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public ToolbarPropertiesBuilder Datasource(DataSource dataSource)
        {
            toolbar.ToolbarModel.DataSource = dataSource;
            return this;
        }
        //public ToolbarPropertiesBuilder Datasource(String dataURL)
        //{
        //    toolbar.ToolbarModel.DataSource = dataURL;
        //    return this;
        //}
        public ToolbarPropertiesBuilder Datasource(IEnumerable dataSource)
        {
            toolbar.ToolbarModel.DataSource = dataSource;
            return this;
        }
        // fields
        public ToolbarPropertiesBuilder ToolbarFields(Action<ToolbarFieldsBuilder> fields)
        {
            var flds = new ToolbarFields();
            toolbar.ToolbarModel.ToolbarFields = flds;
            var builder = new ToolbarFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        public ToolbarPropertiesBuilder Orientation(Orientation orientation)
        {
            toolbar.ToolbarModel.Orientation = orientation;
            return this;
        }
        //String Values
        public ToolbarPropertiesBuilder Height(String height)
        {
            toolbar.ToolbarModel.Height = height;
            return this;
        }
        public ToolbarPropertiesBuilder Width(String width)
        {
            toolbar.ToolbarModel.Width = width;
            return this;
        }
        public ToolbarPropertiesBuilder CssClass(String cssClass)
        {
            toolbar.ToolbarModel.CssClass = cssClass;
            return this;
        }
        public ToolbarPropertiesBuilder Query(String query)
        {
            toolbar.ToolbarModel.Query = query;
            return this;
        }
        //
        public ToolbarPropertiesBuilder Items(Action<ToolbarBaseItemAdder> items)
        {
            this.ItemsCollection = new List<ToolbarBaseItem>();
            ToolbarBaseItemAdder toolbarAdded = new ToolbarBaseItemAdder(this.toolbar.ToolbarModel.Items);
            items.Invoke(toolbarAdded);
            return this as ToolbarPropertiesBuilder;
        }
        //Events
        public ToolbarPropertiesBuilder ClientSideEvents(Action<ToolbarClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new ToolbarClientSideEventsBuilder(this.toolbar.ToolbarModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(toolbar.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        public List<ToolbarBaseItem> ItemsCollection { get; set; }
    }
}
