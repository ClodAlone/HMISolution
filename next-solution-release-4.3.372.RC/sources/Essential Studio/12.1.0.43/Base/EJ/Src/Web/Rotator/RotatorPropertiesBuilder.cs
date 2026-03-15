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
    public class RotatorPropertiesBuilder
    {
        public Rotator rotator;

        public RotatorPropertiesBuilder(Rotator rotator)
        { this.rotator = new Rotator(rotator.ID, rotator.RotatorModel); }

        public RotatorPropertiesBuilder()
        {
        }
        //Boolean values
        public RotatorPropertiesBuilder Enabled()
        {
            rotator.RotatorModel.Enabled = true;
            return this;
        }
        public RotatorPropertiesBuilder Enabled(bool enabled)
        {
            rotator.RotatorModel.Enabled = enabled;
            return this;
        }
        public RotatorPropertiesBuilder AutoPlay()
        {
            rotator.RotatorModel.AutoPlay = true;
            return this;
        }
        public RotatorPropertiesBuilder AutoPlay(bool autoPlay)
        {
            rotator.RotatorModel.AutoPlay = autoPlay;
            return this;
        }
        public RotatorPropertiesBuilder EnablePlaybtn()
        {
            rotator.RotatorModel.EnablePlaybtn = true;
            return this;
        }
        public RotatorPropertiesBuilder EnablePlaybtn(bool enablePlaybtn)
        {
            rotator.RotatorModel.EnablePlaybtn = enablePlaybtn;
            return this;
        }
        public RotatorPropertiesBuilder SlideButton()
        {
            rotator.RotatorModel.SlideButton = true;
            return this;
        }
        public RotatorPropertiesBuilder SlideButton(bool slideButton)
        {
            rotator.RotatorModel.SlideButton = slideButton;
            return this;
        }
        public RotatorPropertiesBuilder AllowResize()
        {
            rotator.RotatorModel.AllowResize = true;
            return this;
        }
        public RotatorPropertiesBuilder AllowResize(bool allowResize)
        {
            rotator.RotatorModel.AllowResize = allowResize;
            return this;
        }
        public RotatorPropertiesBuilder ThumbItem()
        {
            rotator.RotatorModel.ThumbItem = true;
            return this;
        }
        public RotatorPropertiesBuilder ThumbItem(bool thumbItem)
        {
            rotator.RotatorModel.ThumbItem = thumbItem;
            return this;
        }
        public RotatorPropertiesBuilder Pager()
        {
            rotator.RotatorModel.Pager = true;
            return this;
        }
        public RotatorPropertiesBuilder Pager(bool pager)
        {
            rotator.RotatorModel.Pager = pager;
            return this;
        }
        public RotatorPropertiesBuilder Caption()
        {
            rotator.RotatorModel.Caption = true;
            return this;
        }
        public RotatorPropertiesBuilder Caption(bool caption)
        {
            rotator.RotatorModel.Caption = caption;
            return this;
        }
        public RotatorPropertiesBuilder Rtl()
        {
            rotator.RotatorModel.Rtl = true;
            return this;
        }
        public RotatorPropertiesBuilder Rtl(bool rtl)
        {
            rotator.RotatorModel.Rtl = rtl;
            return this;
        }
        public RotatorPropertiesBuilder AllowKeyboardNavigation()
        {
            rotator.RotatorModel.AllowKeyboardNavigation = true;
            return this;
        }
        public RotatorPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            rotator.RotatorModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        //object values
        public RotatorPropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            rotator.RotatorModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public RotatorPropertiesBuilder Datasource(DataSource dataSource)
        {
            rotator.RotatorModel.DataSource = dataSource;
            return this;
        }
        public RotatorPropertiesBuilder Datasource(IEnumerable dataSource)
        {
            rotator.RotatorModel.DataSource = dataSource;
            return this;
        }
        // fields
        public RotatorPropertiesBuilder RotatorFields(Action<RotatorFieldsBuilder> fields)
        {
            var flds = new RotatorFields();
            rotator.RotatorModel.RotatorFields = flds;
            var builder = new RotatorFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //enumvalues
        public RotatorPropertiesBuilder Orientation(Orientation orientation)
        {
            rotator.RotatorModel.Orientation = orientation;
            return this;
        }
        public RotatorPropertiesBuilder PagerPosition(PagerPosition pagerPosition)
        {
            rotator.RotatorModel.PagerPosition = pagerPosition;
            return this;
        }
        //int values
        public RotatorPropertiesBuilder Speed(int speed)
        {
            rotator.RotatorModel.Speed = speed;
            return this;
        }
        //string values
        public RotatorPropertiesBuilder Query(String query)
        {
            rotator.RotatorModel.Query = query;
            return this;
        }
        public RotatorPropertiesBuilder ItemDisplay(String itemDisplay)
        {
            rotator.RotatorModel.ItemDisplay = itemDisplay;
            return this;
        }
        public RotatorPropertiesBuilder ItemMove(String itemMove)
        {
            rotator.RotatorModel.ItemMove = itemMove;
            return this;
        }
        public RotatorPropertiesBuilder StartIndex(String startIndex)
        {
            rotator.RotatorModel.StartIndex = startIndex;
            return this;
        }
        public RotatorPropertiesBuilder SlideWidth(String slideWidth)
        {
            rotator.RotatorModel.SlideWidth = slideWidth;
            return this;
        }
        public RotatorPropertiesBuilder SlideHeight(String slideHeight)
        {
            rotator.RotatorModel.SlideHeight = slideHeight;
            return this;
        }
        public RotatorPropertiesBuilder FrameSpace(String frameSpace)
        {
            rotator.RotatorModel.FrameSpace = frameSpace;
            return this;
        }
        public RotatorPropertiesBuilder ThumbSource(String thumbSource)
        {
            rotator.RotatorModel.ThumbSource = thumbSource;
            return this;
        }
        public RotatorPropertiesBuilder Animation(String animation)
        {
            rotator.RotatorModel.Animation  = animation;
            return this;
        }
        public RotatorPropertiesBuilder CssClass(String cssClass)
        {
            rotator.RotatorModel.CssClass = cssClass;
            return this;
        }
        //Events
        public RotatorPropertiesBuilder ClientSideEvents(Action<RotatorClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new RotatorClientSideEventsBuilder(this.rotator.RotatorModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        public RotatorPropertiesBuilder Items(Action<RotatorBaseItemAdder> items)
        {
            this.ItemsCollection = new List<RotatorBaseItem>();
            RotatorBaseItemAdder tabAdded = new RotatorBaseItemAdder(this.rotator.RotatorModel.Items);
            items.Invoke(tabAdded);
            return this as RotatorPropertiesBuilder;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(rotator.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        public List<RotatorBaseItem> ItemsCollection { get; set; }
    }
}
