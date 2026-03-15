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
using System.Web;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGaugePropertiesBuilder
    {
        private OlapGauge olapGauge;

        public OlapGaugePropertiesBuilder(OlapGauge olapGauge)
        { this.olapGauge = new OlapGauge(olapGauge.ID, olapGauge.OlapGaugeModel);
        }

        public OlapGaugePropertiesBuilder()
        {
        }

        public OlapGaugePropertiesBuilder ReadOnly()
        {
            olapGauge.OlapGaugeModel.ReadOnly = true;
            return this;
        }
        public OlapGaugePropertiesBuilder ReadOnly(bool readOnly)
        {
            olapGauge.OlapGaugeModel.ReadOnly = readOnly;
            return this;
        }
        public OlapGaugePropertiesBuilder Animate()
        {
            olapGauge.OlapGaugeModel.Animate = true;
            return this;
        }
        public OlapGaugePropertiesBuilder Animate(bool animate)
        {
            olapGauge.OlapGaugeModel.Animate = animate;
            return this;
        }
        public OlapGaugePropertiesBuilder IsRadialGradient()
        {
            olapGauge.OlapGaugeModel.IsRadialGradient = false;
            return this;
        }
        public OlapGaugePropertiesBuilder IsRadialGradient(bool isRadialGradient)
        {
            olapGauge.OlapGaugeModel.IsRadialGradient = isRadialGradient;
            return this;
        }

        //EnumValues
        public OlapGaugePropertiesBuilder Theme(Themes theme)
        {
            olapGauge.OlapGaugeModel.Theme = theme;
            return this;
        }
        public OlapGaugePropertiesBuilder FrameType(FrameTypes frameType)
        {
            olapGauge.OlapGaugeModel.Frametype = frameType;
            return this;
        }
        //String Values
        public OlapGaugePropertiesBuilder BackgroundColor(String backgroundColor)
        {
            olapGauge.OlapGaugeModel.BackgroundColor = backgroundColor;
            return this;
        }
        //Integers
        public OlapGaugePropertiesBuilder Height(int height)
        {
            olapGauge.OlapGaugeModel.Height = height;
            return this;
        }
        public OlapGaugePropertiesBuilder Width(int width)
        {
            olapGauge.OlapGaugeModel.Width = width;
            return this;
        }
        public OlapGaugePropertiesBuilder Radius(int radius)
        {
            olapGauge.OlapGaugeModel.Radius = radius;
            return this;
        }
        public OlapGaugePropertiesBuilder HalfCircleFrameStartAngle(int halfCircleFrameStartAngle)
        {
            olapGauge.OlapGaugeModel.HalfCircleFrameStartAngle = halfCircleFrameStartAngle;
            return this;
        }
        public OlapGaugePropertiesBuilder HalfCircleFrameEndAngle(int halfCircleFrameEndAngle)
        {
            olapGauge.OlapGaugeModel.HalfCircleFrameEndAngle = halfCircleFrameEndAngle;
            return this;
        }
        public OlapGaugePropertiesBuilder AnimationSpeed(int animationSpeed)
        {
            olapGauge.OlapGaugeModel.AnimationSpeed = animationSpeed;
            return this;
        }

        public OlapGaugePropertiesBuilder Url(string url)
        {
            olapGauge.OlapGaugeModel.Url = url;
            return this;
        }
        public OlapGaugePropertiesBuilder RowsCount(int rowsCount)
        {
            olapGauge.OlapGaugeModel.RowsCount = rowsCount;
            return this;
        }
        public OlapGaugePropertiesBuilder ColumnsCount(int columnsCount)
        {
            olapGauge.OlapGaugeModel.ColumnsCount = columnsCount;
            return this;
        }
        public OlapGaugePropertiesBuilder ShowTooltip(bool showTooltip)
        {
            olapGauge.OlapGaugeModel.ShowTooltip = showTooltip;
            return this;
        }
        public OlapGaugePropertiesBuilder ShowHeaderLabels(bool showHeaderLabels)
        {
            olapGauge.OlapGaugeModel.ShowHeaderLabels = showHeaderLabels;
            return this;
        }
        public OlapGaugePropertiesBuilder ProgressMode(ProgressMode progressMode)
        {
            olapGauge.OlapGaugeModel.ProgressMode = progressMode;
            return this;
        }
        public OlapGaugePropertiesBuilder ServiceMethods(Action<OlapGaugeServiceMethodsBuilder> servicemethods)
        {
            var serviceMethods = new OlapGaugeServiceMethods();
            olapGauge.OlapGaugeModel.ServiceMethods = serviceMethods;
            var builder = new OlapGaugeServiceMethodsBuilder(serviceMethods);
            if (servicemethods != null)
                servicemethods.Invoke(builder);
            return this;
        }
        //InteriorGradient values
        public OlapGaugePropertiesBuilder InteriorGradient(Action<OlapInteriorGradientBuilder> interiorgradient)
        {
            var interiorGradient = new List<InteriorGradients>();
            olapGauge.OlapGaugeModel.InteriorGradient = interiorGradient;
            this.olapGauge.OlapGaugeModel.InteriorGradient = interiorGradient;
            var builder = new OlapInteriorGradientBuilder(this.olapGauge);
            if (interiorgradient != null)
                interiorgradient.Invoke(builder);
            return this;
        }
        //Scales value
        public OlapGaugePropertiesBuilder Scales(Action<OlapScalesPropertiesBuilder> scale)
        {
            var scales = new List<CircularScales>();
            olapGauge.OlapGaugeModel.Scales = scales;
            this.olapGauge.OlapGaugeModel.Scales = scales;
            var builder = new OlapScalesPropertiesBuilder(this.olapGauge);
            if (scale != null)
                scale.Invoke(builder);
            return this;
        }
        public OlapGaugePropertiesBuilder CustomObject(Dictionary<String, Object> customObject)
        {
            olapGauge.OlapGaugeModel.CustomObject = customObject;
            return this;
        }
        
        //Events
        public OlapGaugePropertiesBuilder ClientSideEvents(Action<OlapGaugeClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new OlapGaugeClientSideEventsBuilder(this.olapGauge.OlapGaugeModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(olapGauge.Render().ToString());
        }
        public override String ToString()
        {
            return Render().ToString();
        }
    }
}
