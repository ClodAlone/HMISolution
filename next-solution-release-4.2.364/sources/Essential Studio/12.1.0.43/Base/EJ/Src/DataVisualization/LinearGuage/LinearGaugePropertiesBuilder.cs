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
    public class LinearGaugePropertiesBuilder
    {
        public LinearGauge linearGauge;

        public LinearGaugePropertiesBuilder(LinearGauge linearGauge)
        { this.linearGauge = new LinearGauge(linearGauge.ID, linearGauge.LinearGaugeModel); }

        public LinearGaugePropertiesBuilder()
        {
        }
        //Boolean values
        public LinearGaugePropertiesBuilder ReadOnly()
        {
            linearGauge.LinearGaugeModel.ReadOnly = true;
            return this;
        }
        public LinearGaugePropertiesBuilder ReadOnly(bool readOnly)
        {
            linearGauge.LinearGaugeModel.ReadOnly = readOnly;
            return this;
        }
        public LinearGaugePropertiesBuilder Animate()
        {
            linearGauge.LinearGaugeModel.Animate = true;
            return this;
        }
        public LinearGaugePropertiesBuilder Animate(bool animate)
        {
            linearGauge.LinearGaugeModel.Animate = animate;
            return this;
        }
        public LinearGaugePropertiesBuilder CanResize(bool canResize)
        {
            linearGauge.LinearGaugeModel.CanResize = canResize;
            return this;
        }
        //EnumValues
        public LinearGaugePropertiesBuilder Theme(Themes theme)
        {
            linearGauge.LinearGaugeModel.Theme = theme;
            return this;
        }
        public LinearGaugePropertiesBuilder Orientation(Orientation orientation)
        {
            linearGauge.LinearGaugeModel.Orientation = orientation;
            return this;
        }
        //String Values
        public LinearGaugePropertiesBuilder BackgroundColor(String backgroundColor)
        {
            linearGauge.LinearGaugeModel.BackgroundColor = backgroundColor;
            return this;
        }
        public LinearGaugePropertiesBuilder BorderColor(String borderColor)
        {
            linearGauge.LinearGaugeModel.BorderColor = borderColor;
            return this;
        }
        public LinearGaugePropertiesBuilder LabelColor(String labelColor)
        {
            linearGauge.LinearGaugeModel.LabelColor = labelColor;
            return this;
        }
        public LinearGaugePropertiesBuilder TickColor(String tickColor)
        {
            linearGauge.LinearGaugeModel.TickColor = tickColor;
            return this;
        }
        public LinearGaugePropertiesBuilder FrameBackgroundImageUrl(String frameBackgroundImageUrl)
        {
            linearGauge.LinearGaugeModel.FrameBackgroundImageUrl = frameBackgroundImageUrl;
            return this;
        }
        //Integers
        public LinearGaugePropertiesBuilder Height(int height)
        {
            linearGauge.LinearGaugeModel.Height = height;
            return this;
        }
        public LinearGaugePropertiesBuilder Width(int width)
        {
            linearGauge.LinearGaugeModel.Width = width;
            return this;
        }
        public LinearGaugePropertiesBuilder FrameOuterWidth(int frameOuterWidth)
        {
            linearGauge.LinearGaugeModel.FrameOuterWidth = frameOuterWidth;
            return this;
        }
        public LinearGaugePropertiesBuilder FrameInnerWidth(int frameInnerWidth)
        {
            linearGauge.LinearGaugeModel.FrameInnerWidth = frameInnerWidth;
            return this;
        }
        public LinearGaugePropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            linearGauge.LinearGaugeModel.AnimationSpeed = animationSpeed;
            return this;
        }
        public LinearGaugePropertiesBuilder Value(int value)
        {
            linearGauge.LinearGaugeModel.Value = value;
            return this;
        }
        public LinearGaugePropertiesBuilder Minimum(int minimum)
        {
            linearGauge.LinearGaugeModel.Minimum = minimum;
            return this;
        }
        public LinearGaugePropertiesBuilder Maximum(int maximum)
        {
            linearGauge.LinearGaugeModel.Maximum = maximum;
            return this;
        }
        // Scales Values
        public LinearGaugePropertiesBuilder Scales(Action<ScalesPropertiesBuilder> scale)
        {
            var scales = new List<Scales>();
            linearGauge.LinearGaugeModel.Scales = scales;
            var builder = new ScalesPropertiesBuilder(this.linearGauge);
            if (scale != null)
                scale.Invoke(builder);
            return this;
        }
        //PointerGradient1 values
        public LinearGaugePropertiesBuilder PointerGradient1(Action<PointerGradient1Builder> pointergradient1)
        {
            var pointerGradient1 = new List<PointerGradient1>();
            linearGauge.LinearGaugeModel.PointerGradient1 = pointerGradient1;
            var builder = new PointerGradient1Builder(this.linearGauge);
            if (pointergradient1 != null)
                pointergradient1.Invoke(builder);
            return this;
        }
        //PointerGradient2 values
        public LinearGaugePropertiesBuilder PointerGradient2(Action<PointerGradient2Builder> pointergradient2)
        {
            var pointerGradient2 = new List<PointerGradient2>();
            linearGauge.LinearGaugeModel.PointerGradient2 = pointerGradient2;
            var builder = new PointerGradient2Builder(this.linearGauge);
            if (pointergradient2 != null)
                pointergradient2.Invoke(builder);
            return this;
        }
        //Events
        public LinearGaugePropertiesBuilder ClientSideEvents(Action<LinearGaugeClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new LinearGaugeClientSideEventsBuilder(this.linearGauge.LinearGaugeModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(linearGauge.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
