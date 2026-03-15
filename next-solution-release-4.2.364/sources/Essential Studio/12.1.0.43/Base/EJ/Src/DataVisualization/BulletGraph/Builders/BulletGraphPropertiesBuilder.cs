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
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class BulletGraphPropertiesBuilder
    {
        public BulletGraph bullet;

        public BulletGraphPropertiesBuilder(BulletGraph bullet)
        { this.bullet = new BulletGraph(bullet.ID, bullet.BulletGraphModel); }

        public BulletGraphPropertiesBuilder()
        {
        }

        public BulletGraphPropertiesBuilder Height(int height)
        {
            bullet.BulletGraphModel.Height = height;
            return this;
        }

        public BulletGraphPropertiesBuilder Width(int width)
        {
            bullet.BulletGraphModel.Width = width;
            return this;
        }

        public BulletGraphPropertiesBuilder Theme(String theme)
        {
            bullet.BulletGraphModel.Theme = theme;
            return this;
        }

        public BulletGraphPropertiesBuilder Orientation(Orientation orientation)
        {
            bullet.BulletGraphModel.Orientation = orientation;
            return this;
        }

        public BulletGraphPropertiesBuilder FlowDirection(FlowDirection direction)
        {
            bullet.BulletGraphModel.FlowDirection = direction;
            return this;
        }

        public BulletGraphPropertiesBuilder QualitativeRangeSize(int rangeSize)
        {
            bullet.BulletGraphModel.QualitativeRangeSize = rangeSize;
            return this;
        }

        public BulletGraphPropertiesBuilder QuantitativeScaleLength(int scaleLength)
        {
            bullet.BulletGraphModel.QuantitativeScaleLength = scaleLength;
            return this;
        }

        public BulletGraphPropertiesBuilder ShowTooltip(bool value)
        {
            bullet.BulletGraphModel.ShowTooltip = value;
            return this;
        }

        public BulletGraphPropertiesBuilder TooltipTemplateId(String templId)
        {
            bullet.BulletGraphModel.TooltipTemplateId = templId;
            return this;
        }

        public BulletGraphPropertiesBuilder EnableAnimation(bool value)
        {
            bullet.BulletGraphModel.EnableAnimation = value;
            return this;
        }

        public BulletGraphPropertiesBuilder BindRangeStrokeToTicks(bool value)
        {
            bullet.BulletGraphModel.BindRangeStrokeToTicks = value;
            return this;
        }

        public BulletGraphPropertiesBuilder BindRangeStrokeToLabels(bool value)
        {
            bullet.BulletGraphModel.BindRangeStrokeToLabels = value;
            return this;
        }

        public BulletGraphPropertiesBuilder Fields(Action<FieldsBuilder> fields)
        {
            var obj = new Fields();
            bullet.BulletGraphModel.Fields = fields;
            var builder = new FieldsBuilder(obj,bullet);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }

        public BulletGraphPropertiesBuilder QuantitativeScale(Action<QuantitativeScaleBuilder> scale)
        {
            var obj = new QuantitativeScale();
            bullet.BulletGraphModel.QuantitativeScale = scale;
            var builder = new QuantitativeScaleBuilder(bullet,obj);
            if (scale != null)
                scale.Invoke(builder);
            return this;
        }

        public BulletGraphPropertiesBuilder QualitativeRanges(Action<QualitativeRangesBuilder> range)
        {
            var obj = new QualitativeRanges();
            var builder = new QualitativeRangesBuilder(obj,bullet);
            if (range != null)
                range.Invoke(builder);
            return this;
        }

        public BulletGraphPropertiesBuilder QualitativeRanges(List<QualitativeRanges> ranges)
        {
            bullet.BulletGraphModel.QualitativeRanges = ranges;
            return this;
        }

        public BulletGraphPropertiesBuilder Caption(Action<CaptionBuilder> captionOptions)
        {
            var obj = new Caption();
            bullet.BulletGraphModel.Caption = captionOptions;
            var builder = new CaptionBuilder(this.bullet, obj);
            if (captionOptions != null)
                captionOptions.Invoke(builder);
            return this;
        }

        //Events
        public BulletGraphPropertiesBuilder ClientSideEvents(Action<ClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new ClientSideEventsBuilder(this.bullet.BulletGraphModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        public BulletGraphPropertiesBuilder Value(double value)
        {
            bullet.BulletGraphModel.Value = value;
            return this;
        }
        public BulletGraphPropertiesBuilder ComparativeMeasureValue(double comparativeMeasureValue)
        {
            bullet.BulletGraphModel.ComparativeMeasureValue = comparativeMeasureValue;
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(bullet.Render().ToString());
        }

        public override String ToString()
        {

            return Render().ToString();
        }

    }
}
