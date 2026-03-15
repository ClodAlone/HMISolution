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
    public class TagCloudPropertiesBuilder
    {
        public TagCloud tagCloud;

        public TagCloudPropertiesBuilder(TagCloud tagCloud)
        { this.tagCloud = new TagCloud(tagCloud.ID, tagCloud.TagCloudModel); }

        public TagCloudPropertiesBuilder()
        {
        }
        //Boolean values
        public TagCloudPropertiesBuilder ShowTitle()
        {
            tagCloud.TagCloudModel.ShowTitle = true;
            return this;
        }
        public TagCloudPropertiesBuilder ShowTitle(bool showTitle)
        {
            tagCloud.TagCloudModel.ShowTitle = showTitle;
            return this;
        }
        public TagCloudPropertiesBuilder Rtl()
        {
            tagCloud.TagCloudModel.Rtl = true;
            return this;
        }
        public TagCloudPropertiesBuilder Rtl(bool rtl)
        {
            tagCloud.TagCloudModel.Rtl = rtl;
            return this;
        }
        //enum values
        public TagCloudPropertiesBuilder Format(Formats format )
        {
            tagCloud.TagCloudModel.Format = format;
            return this;
        }
        //object values
        public TagCloudPropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            tagCloud.TagCloudModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public TagCloudPropertiesBuilder Datasource(DataSource dataSource)
        {
            tagCloud.TagCloudModel.DataSource = dataSource;
            return this;
        }
        public TagCloudPropertiesBuilder Datasource(IEnumerable dataSource)
        {
            tagCloud.TagCloudModel.DataSource = dataSource;
            return this;
        }
        // fields
        public TagCloudPropertiesBuilder TagCloudFields(Action<TagCloudFieldsBuilder> fields)
        {
            var flds = new TagCloudFields();
            tagCloud.TagCloudModel.TagCloudFields = flds;
            var builder = new TagCloudFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //String
        public TagCloudPropertiesBuilder Query(String query)
        {
            tagCloud.TagCloudModel.Query = query;
            return this;
        }
        public TagCloudPropertiesBuilder CssClass(String cssClass)
        {
            tagCloud.TagCloudModel.CssClass = cssClass;
            return this;
        }
        public TagCloudPropertiesBuilder Title(String title)
        {
            tagCloud.TagCloudModel.Title = title;
            return this;
        }
        public TagCloudPropertiesBuilder TitleImage(String titleImage)
        {
            tagCloud.TagCloudModel.TitleImage = titleImage;
            return this;
        }
        public TagCloudPropertiesBuilder MinFontSize(String minFontSize)
        {
            tagCloud.TagCloudModel.MinFontSize = minFontSize;
            return this;
        }
        public TagCloudPropertiesBuilder MaxFontSize(String maxFontSize)
        {
            tagCloud.TagCloudModel.MaxFontSize = maxFontSize;
            return this;
        }
           public TagCloudPropertiesBuilder ClientSideEvents(Action<TagCloudClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new TagCloudClientSideEventsBuilder(this.tagCloud.TagCloudModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(tagCloud.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
