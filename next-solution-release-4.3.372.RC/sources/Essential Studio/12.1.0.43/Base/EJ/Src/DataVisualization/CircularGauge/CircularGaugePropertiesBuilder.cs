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
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularGaugePropertiesBuilder
    {
        public CircularGauge circulargauge;

        public CircularGaugePropertiesBuilder(CircularGauge circulargauge)
        { this.circulargauge = new CircularGauge(circulargauge.ID, circulargauge.CircularGaugeModel); }

        public CircularGaugePropertiesBuilder()
        {
        }

        //Boolean values
        public CircularGaugePropertiesBuilder ReadOnly()
        {
            circulargauge.CircularGaugeModel.ReadOnly = true;
            return this;
        }
        public CircularGaugePropertiesBuilder ReadOnly(bool readOnly)
        {
            circulargauge.CircularGaugeModel.ReadOnly = readOnly;
            return this;
        }
        public CircularGaugePropertiesBuilder Animate()
        {
            circulargauge.CircularGaugeModel.Animate = true;
            return this;
        }
        public CircularGaugePropertiesBuilder Animate(bool animate)
        {
            circulargauge.CircularGaugeModel.Animate = animate;
            return this;
        }
        public CircularGaugePropertiesBuilder CanResize(bool canResize)
        {
            circulargauge.CircularGaugeModel.CanResize = canResize;
            return this;
        }
        public CircularGaugePropertiesBuilder IsRadialGradient()
        {
            circulargauge.CircularGaugeModel.IsRadialGradient = false;
            return this;
        }
        public CircularGaugePropertiesBuilder IsRadialGradient(bool isRadialGradient)
        {
            circulargauge.CircularGaugeModel.IsRadialGradient = isRadialGradient;
            return this;
        }

        //EnumValues
        public CircularGaugePropertiesBuilder Theme(Themes theme)
        {
            circulargauge.CircularGaugeModel.Theme = theme;
            return this;
        }
        public CircularGaugePropertiesBuilder FrameType(FrameTypes frameType)
        {
            circulargauge.CircularGaugeModel.Frametype = frameType;
            return this;
        }
        //String Values
        public CircularGaugePropertiesBuilder BackgroundColor(String backgroundColor)
        {
            circulargauge.CircularGaugeModel.BackgroundColor = backgroundColor;
            return this;
        }                       
        //Integers
        public CircularGaugePropertiesBuilder Height(int height)
        {
            circulargauge.CircularGaugeModel.Height = height;
            return this;
        }
        public CircularGaugePropertiesBuilder Width(int width)
        {
            circulargauge.CircularGaugeModel.Width = width;
            return this;
        }
        public CircularGaugePropertiesBuilder Radius(int radius)
        {
            circulargauge.CircularGaugeModel.Radius = radius;
            return this;
        }
        public CircularGaugePropertiesBuilder Value(int value)
        {
            circulargauge.CircularGaugeModel.Value = value;
            return this;
        }
        public CircularGaugePropertiesBuilder Minimum(int minimum)
        {
            circulargauge.CircularGaugeModel.Minimum = minimum;
            return this;
        }
        public CircularGaugePropertiesBuilder Maximum(int maximum)
        {
            circulargauge.CircularGaugeModel.Maximum = maximum;
            return this;
        }
        public CircularGaugePropertiesBuilder HalfCircleFrameStartAngle(int halfCircleFrameStartAngle)
        {
            circulargauge.CircularGaugeModel.HalfCircleFrameStartAngle = halfCircleFrameStartAngle;
            return this;
        }       
        public CircularGaugePropertiesBuilder HalfCircleFrameEndAngle(int halfCircleFrameEndAngle)
        {
            circulargauge.CircularGaugeModel.HalfCircleFrameEndAngle = halfCircleFrameEndAngle;
            return this;
        }
        public CircularGaugePropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            circulargauge.CircularGaugeModel.AnimationSpeed = animationSpeed;
            return this;
        }
        //InteriorGradient values
        public CircularGaugePropertiesBuilder InteriorGradient(Action<InteriorGradientBuilder> interiorgradient)
        {
            var interiorGradient = new List<InteriorGradients>();
            circulargauge.CircularGaugeModel.InteriorGradient = interiorGradient;
            var builder = new InteriorGradientBuilder(this.circulargauge);
            if (interiorgradient != null)
                interiorgradient.Invoke(builder);
            return this;
        }
        //Scales value
        public CircularGaugePropertiesBuilder Scales(Action<CircularScalesPropertiesBuilder> scale)
        {
            var scales = new List<CircularScales>();
            circulargauge.CircularGaugeModel.Scales = scales;
            var builder = new CircularScalesPropertiesBuilder(this.circulargauge);
            if (scale != null)
                scale.Invoke(builder);
            return this;
        }
        //Events
        public CircularGaugePropertiesBuilder ClientSideEvents(Action<CircularGaugeClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new CircularGaugeClientSideEventsBuilder(this.circulargauge.CircularGaugeModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }               
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(circulargauge.Render().ToString());
        }
        public override String ToString()
        {
            return Render().ToString();
        }        
    }
}
