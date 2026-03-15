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
using Syncfusion.JavaScript.Olap.Models;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapChartClientSideEventsBuilder
    {
        private OlapChartProperties olapChartModel;
        public OlapChartClientSideEventsBuilder(OlapChartProperties olapChartProp)
        {
            olapChartModel = olapChartProp;
        }
        public OlapChartClientSideEventsBuilder BeforeServiceInvoke(string beforeServiceInvoke)
        {
            olapChartModel.BeforeServiceInvoke = beforeServiceInvoke;
            return this;
        }
        public OlapChartClientSideEventsBuilder AfterServiceInvoke(string afterServiceInvoke)
        {
            olapChartModel.AfterServiceInvoke = afterServiceInvoke;
            return this;
        }
        public OlapChartClientSideEventsBuilder DrillSuccess(string drillSuccess)
        {
            olapChartModel.DrillSuccess = drillSuccess;
            return this;
        }
        public OlapChartClientSideEventsBuilder Load(String load)
        {
            olapChartModel.Load = load;
            return this;
        }
        public OlapChartClientSideEventsBuilder AxesLabelRendering(String axesLabelRendering)
        {
            olapChartModel.AxesLabelRendering = axesLabelRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder AxesRangeCalculate(String axesRangeCalculate)
        {
            olapChartModel.AxesRangeCalculate = axesRangeCalculate;
            return this;
        }
        public OlapChartClientSideEventsBuilder AxesTitleRendering(String axesTitleRendering)
        {
            olapChartModel.AxesTitleRendering = axesTitleRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder ChartAreaBoundsCalculate(String chartAreaBoundsCalculate)
        {
            olapChartModel.ChartAreaBoundsCalculate = chartAreaBoundsCalculate;
            return this;
        }
        public OlapChartClientSideEventsBuilder LegendItemRendering(String legendItemRendering)
        {
            olapChartModel.LegendItemRendering = legendItemRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder LengendBoundsCalculate(String lengendBoundsCalculate)
        {
            olapChartModel.LengendBoundsCalculate = lengendBoundsCalculate;
            return this;
        }
        public OlapChartClientSideEventsBuilder PreRender(String preRender)
        {
            olapChartModel.PreRender = preRender;
            return this;
        }
        public OlapChartClientSideEventsBuilder SeriesRendering(String seriesRendering)
        {
            olapChartModel.SeriesRendering = seriesRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder SymbolRendering(String symbolRendering)
        {
            olapChartModel.SymbolRendering = symbolRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder TitleRendering(String titleRendering)
        {
            olapChartModel.TitleRendering = titleRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder AxesLabelsInitialize(String axesLabelsInitialize)
        {
            olapChartModel.AxesLabelsInitialize = axesLabelsInitialize;
            return this;
        }
        public OlapChartClientSideEventsBuilder PointRegionClick(String pointRegionClick)
        {
            olapChartModel.PointRegionClick = pointRegionClick;
            return this;
        }
        public OlapChartClientSideEventsBuilder PointRegionMouseMove(String pointRegionMouseMove)
        {
            olapChartModel.PointRegionMouseMove = pointRegionMouseMove;
            return this;
        }
        public OlapChartClientSideEventsBuilder LegendItemClick(String legendItemClick)
        {
            olapChartModel.LegendItemClick = legendItemClick;
            return this;
        }
        public OlapChartClientSideEventsBuilder LegendItemMouseMove(String legendItemMouseMove)
        {
            olapChartModel.LegendItemMouseMove = legendItemMouseMove;
            return this;
        }
        public OlapChartClientSideEventsBuilder DisplayTextRendering(String displayTextRendering)
        {
            olapChartModel.DisplayTextRendering = displayTextRendering;
            return this;
        }
        public OlapChartClientSideEventsBuilder ToolTipInitialize(String toolTipInitialize)
        {
            olapChartModel.ToolTipInitialize = toolTipInitialize;
            return this;
        }
        public OlapChartClientSideEventsBuilder TrackAxisToolTip(String trackAxisToolTip)
        {
            olapChartModel.TrackAxisToolTip = trackAxisToolTip;
            return this;
        }
        public OlapChartClientSideEventsBuilder TrackToolTip(String trackToolTip)
        {
            olapChartModel.TrackToolTip = trackToolTip;
            return this;
        }
        public OlapChartClientSideEventsBuilder AnimationComplete(String animationComplete)
        {
            olapChartModel.AnimationComplete = animationComplete;
            return this;
        }
        public OlapChartClientSideEventsBuilder Create(String create)
        {
            olapChartModel.Create = create;
            return this;
        }
        public OlapChartClientSideEventsBuilder Destroy(string destroy)
        {
            olapChartModel.Destroy = destroy;
            return this;
        }
    }
}
