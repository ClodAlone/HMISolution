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
using Syncfusion.JavaScript.Shared;
using System.Web;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapChartPropertiesBuilder
    {
        public OlapChart olapChart;

        public OlapChartPropertiesBuilder(OlapChart olapChart)
        { this.olapChart = new OlapChart(olapChart.ID, olapChart.OlapChartModel); }

        public OlapChartPropertiesBuilder()
        {
        }
        public OlapChartPropertiesBuilder Url(string url)
        {
            this.olapChart.OlapChartModel.Url = url;
            return this;
        }
        public OlapChartPropertiesBuilder Title(string title)
        {
            this.olapChart.OlapChartModel.Title = title;
            return this;
        }
        public OlapChartPropertiesBuilder Legend(Action<LegendBuilder> legend)
        {
            var obj = new Legend();
            this.olapChart.OlapChartModel.Legend = obj;
            var builder = new LegendBuilder(obj);
            if (legend != null)
                legend.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder PrimaryXAxis(Action<AxisBuilder> axisValue)
        {
            var obj = new Axis();
            this.olapChart.OlapChartModel.PrimaryXAxis = obj;
            obj.Orientation = Orientation.Horizontal;
            var builder = new AxisBuilder(obj);
            if (axisValue != null)
                axisValue.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder PrimaryYAxis(Action<AxisBuilder> axisValue)
        {
            var obj = new Axis();
            this.olapChart.OlapChartModel.PrimaryYAxis = obj;
            obj.Orientation = Orientation.Vertical;
            var builder = new AxisBuilder(obj);
            if (axisValue != null)
                axisValue.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder CommonSeriesOptions(Action<CommonSeriesOptionsBuilder> commonSeriesOptions)
        {
            var obj = new CommonSeriesOptions();
            this.olapChart.OlapChartModel.CommonSeriesOptions = obj;
            var builder = new CommonSeriesOptionsBuilder(obj);
            if (commonSeriesOptions != null)
                commonSeriesOptions.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder CrossHair(Action<CrossHairBuilder> crossHair)
        {
            var obj = new CrossHair();
            this.olapChart.OlapChartModel.CrossHair = obj;
            var builder = new CrossHairBuilder(obj);
            if (crossHair != null)
                crossHair.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder Size(Action<SizeBuilder> size)
        {
            var obj = new ChartSize();
            this.olapChart.OlapChartModel.Size = obj;
            var builder = new SizeBuilder(obj);
            if (size != null)
                size.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder Title(Action<TitleBuilder> title)
        {
            var obj = new Title();
            this.olapChart.OlapChartModel.Title = obj;
            var builder = new TitleBuilder(obj);
            if (title != null)
                title.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder Border(Action<ChartBorderBuilder> chartBorder)
        {
            var obj = new ChartBorder();
            this.olapChart.OlapChartModel.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (chartBorder != null)
                chartBorder.Invoke(builder);
            return this;
        }

        public OlapChartPropertiesBuilder ChartArea(Action<ChartAreaBuilder> chartAreaBorder)
        {
            var obj = new ChartArea();
            this.olapChart.OlapChartModel.ChartArea = obj;
            var builder = new ChartAreaBuilder(obj);
            if (chartAreaBorder != null)
                chartAreaBorder.Invoke(builder);
            return this;
        }

        public OlapChartPropertiesBuilder Zooming(Action<ZoomingBuilder> zooming)
        {
            var obj = new Zooming();
            this.olapChart.OlapChartModel.Zooming = obj;
            var builder = new ZoomingBuilder(obj);
            if (zooming != null)
                zooming.Invoke(builder);
            return this;
        }

        public OlapChartPropertiesBuilder Background(string color)
        {
            this.olapChart.OlapChartModel.Background = color;
            return this;
        }

        public OlapChartPropertiesBuilder ElementSpacing(double elementSpacing)
        {
            this.olapChart.OlapChartModel.ElementSpacing = elementSpacing;
            return this;
        }
        public OlapChartPropertiesBuilder CanResize(bool canResize)
        {
            this.olapChart.OlapChartModel.CanResize = canResize;
            return this;
        }

        public OlapChartPropertiesBuilder InitSeriesRender(bool initSeriesRender)
        {
            this.olapChart.OlapChartModel.InitSeriesRender = initSeriesRender;
            return this;
        }

        public OlapChartPropertiesBuilder Theme(ChartTheme theme)
        {
            this.olapChart.OlapChartModel.Theme = theme;
            return this;
        }
        public OlapChartPropertiesBuilder ProgressMode(ProgressMode progressMode)
        {
            this.olapChart.OlapChartModel.ProgressMode = progressMode;
            return this;
        }
        public OlapChartPropertiesBuilder ServiceMethods(Action<OlapChartServiceMethodsBuilder> servicemethods)
        {
            var serviceMethods = new OlapChartServiceMethods();
            this.olapChart.OlapChartModel.ServiceMethods = serviceMethods;
            var builder = new OlapChartServiceMethodsBuilder(serviceMethods);
            if (servicemethods != null)
                servicemethods.Invoke(builder);
            return this;
        }
        public OlapChartPropertiesBuilder CustomObject(Dictionary<String, Object> customObject)
        {
            this.olapChart.OlapChartModel.CustomObject = customObject;
            return this;
        }
        //Events
        public OlapChartPropertiesBuilder ClientSideEvents(Action<OlapChartClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new OlapChartClientSideEventsBuilder(this.olapChart.OlapChartModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(olapChart.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
