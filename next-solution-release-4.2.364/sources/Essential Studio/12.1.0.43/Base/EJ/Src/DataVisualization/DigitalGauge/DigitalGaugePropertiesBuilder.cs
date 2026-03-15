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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class DigitalGaugePropertiesBuilder
    {
        public DigitalGauge digitalgauge;
        

        public DigitalGaugePropertiesBuilder(DigitalGauge digitalgauge)
        { this.digitalgauge = new DigitalGauge(digitalgauge.ID, digitalgauge.DigitalGaugeModel); }

        public DigitalGaugePropertiesBuilder()
        {
        }
        //int 
        public DigitalGaugePropertiesBuilder Width(int width)
        {
            digitalgauge.DigitalGaugeModel.Width = width;
            return this;
        }
        public DigitalGaugePropertiesBuilder Height(int height)
        {
            digitalgauge.DigitalGaugeModel.Height = height;
            return this;
        }
        public DigitalGaugePropertiesBuilder FrameInnerWidth(int frameInnerWidth)
        {
            digitalgauge.DigitalGaugeModel.FrameInnerWidth = frameInnerWidth;
            return this;
        }
        public DigitalGaugePropertiesBuilder FrameOuterWidth(int frameOuterWidth)
        {
            digitalgauge.DigitalGaugeModel.FrameOuterWidth = frameOuterWidth;
            return this;
        }
        public DigitalGaugePropertiesBuilder Value(string value)
        {
            digitalgauge.DigitalGaugeModel.Value = value;
            return this;
        }
        //Enum
        public DigitalGaugePropertiesBuilder Themes(Themes themes)
        {
            digitalgauge.DigitalGaugeModel.Themes = themes;
            return this;
        }
        //String
        public DigitalGaugePropertiesBuilder FrameBackgroundImageUrl(String frameBackgroundImageUrl)
        {
            digitalgauge.DigitalGaugeModel.FrameBackgroundImageUrl = frameBackgroundImageUrl;
            return this;
        }
        public DigitalGaugePropertiesBuilder SegmentColor(String segmentColor)
        {
            digitalgauge.DigitalGaugeModel.SegmentColor = segmentColor;
            return this;
        }
        //objects
        // items
        public DigitalGaugePropertiesBuilder DigitalGaugeItems(Action<DigitalGaugeItemsBuilder> items)
        {
            var item = new List<DigitalGaugeItems>();
            digitalgauge.DigitalGaugeModel.DigitalGaugeItems =item;
            var builder = new DigitalGaugeItemsBuilder(this.digitalgauge);
            if (items != null)
                items.Invoke(builder);
            return this;
        }
        //Events
        public DigitalGaugePropertiesBuilder ClientSideEvents(Action<DigitalGaugeClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new DigitalGaugeClientSideEventsBuilder(this.digitalgauge.DigitalGaugeModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //bool
        public DigitalGaugePropertiesBuilder CanResize(bool canResize)
        {
            digitalgauge.DigitalGaugeModel.CanResize = canResize;
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(digitalgauge.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
