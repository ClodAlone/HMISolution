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
using System.Drawing;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class ChartPropertiesBuilder
    {
        public Chart chart;

          public ChartPropertiesBuilder(Chart chart)
          { 
              this.chart = new Chart(chart.ID, chart.ChartModel);
          }
           public ChartPropertiesBuilder PrimaryXAxis(Action<AxisBuilder> axisValue)
          {
              var obj = new Axis();
              chart.ChartModel.PrimaryXAxis = obj;
			  obj.Orientation = Orientation.Horizontal;
              var builder = new AxisBuilder(obj);
              if (axisValue != null)
                  axisValue.Invoke(builder);
              return this;
          }
          public ChartPropertiesBuilder PrimaryYAxis(Action<AxisBuilder> axisValue)
          {
              var obj = new Axis();
              chart.ChartModel.PrimaryYAxis = obj;
			  obj.Orientation=Orientation.Vertical;
              var builder = new AxisBuilder(obj);
              if (axisValue != null)
                  axisValue.Invoke(builder);
              return this;
          }
        public ChartPropertiesBuilder Axes(Action<AxisBuilder> axis)
          {
              var obj = new Axis();
              var builder = new AxisBuilder(obj,chart);

              if (axis != null)
                  axis.Invoke(builder);
              return this;
          }
          public ChartPropertiesBuilder Axes(List<Axis> axis)
          {
              chart.ChartModel.Axes = axis;
              return this;
          }
          public ChartPropertiesBuilder Series(Action<SeriesBuilder> series)
          {
              var obj = new Series();
              var builder = new SeriesBuilder(obj, chart);

              if (series != null)
                  series.Invoke(builder);
              return this;
          }
          //public ChartPropertiesBuilder Series(List<Series> series)
          //{
          //    chart.ChartModel.Series = series;
          //    return this;
          //}
          public ChartPropertiesBuilder RowDefinitions(Action<RowDefinitionsBuilder> rowDefinition)
          {
              var obj = new RowDefinitions();
              var builder = new RowDefinitionsBuilder(obj, chart);

              if (rowDefinition != null)
                  rowDefinition.Invoke(builder);
              return this;
          }
          public ChartPropertiesBuilder RowDefinitions(List<RowDefinitions> rowDefinition)
          {
              chart.ChartModel.RowDefinitions = rowDefinition;
              return this;
          }
          public ChartPropertiesBuilder ColumnDefinitions(Action<ColumnDefinitionsBuilder> columnDefinition)
          {
              var obj = new ColumnDefinitions();
              var builder = new ColumnDefinitionsBuilder(obj, chart);

              if (columnDefinition != null)
                  columnDefinition.Invoke(builder);
              return this;
          }
          public ChartPropertiesBuilder ColumnDefinitions(List<ColumnDefinitions> columnDefinition)
          {
              chart.ChartModel.ColumnDefinitions = columnDefinition;
              return this;
          }

          public ChartPropertiesBuilder CommonSeriesOptions(Action<CommonSeriesOptionsBuilder> commonSeriesOptions)
          {
              var obj = new CommonSeriesOptions();
              chart.ChartModel.CommonSeriesOptions = obj;
              var builder = new CommonSeriesOptionsBuilder(obj);
              if (commonSeriesOptions != null)
                  commonSeriesOptions.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Crosshair(Action<CrossHairBuilder> crossHair)
          {
              var obj = new CrossHair();
              chart.ChartModel.CrossHair = obj;
              var builder = new CrossHairBuilder(obj);
              if (crossHair != null)
                  crossHair.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Margin(Action<MarginBuilder> margin)
          {
              var obj = new Margin();
              chart.ChartModel.Margin = obj;
              var builder = new MarginBuilder(obj);
              if (margin != null)
                  margin.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Legend(Action<LegendBuilder> legend)
          {
              var obj = new Legend();
              chart.ChartModel.Legend = obj;
              var builder = new LegendBuilder(obj);
              if (legend != null)
                  legend.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Size(Action<SizeBuilder> size)
          {
              var obj = new ChartSize();
              chart.ChartModel.Size = obj;
              var builder = new SizeBuilder(obj);
              if (size != null)
                  size.Invoke(builder);
              return this;
          }

           

          public ChartPropertiesBuilder Border(Action<LayoutBorderBuilder> Border)
          {
              var obj = new LayoutBorder();
              chart.ChartModel.Border = obj;
              var builder = new LayoutBorderBuilder(obj);
              if (Border != null)
                  Border.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder ChartArea(Action<ChartAreaBuilder> chartAreaBorder)
          {
              var obj = new ChartArea();
              chart.ChartModel.ChartArea = obj;
              var builder = new ChartAreaBuilder(obj);
              if (chartAreaBorder != null)
                  chartAreaBorder.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Zooming(Action<ZoomingBuilder> zooming)
          {
              var obj = new Zooming();
              chart.ChartModel.Zooming = obj;
              var builder = new ZoomingBuilder(obj);
              if (zooming != null)
                  zooming.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder Background(string color)
          {
              chart.ChartModel.Background = color;
              return this;
          }

          public ChartPropertiesBuilder ElementSpacing(double elementSpacing)
          {
              chart.ChartModel.ElementSpacing = elementSpacing;
              return this;
          }

          public ChartPropertiesBuilder Title(Action<TitleBuilder> title)
          {
              var obj = new Title();
              chart.ChartModel.Title = obj;
              var builder = new TitleBuilder(obj);
              if (title != null)
                  title.Invoke(builder);
              return this;
          }

          public ChartPropertiesBuilder BackGroundImageUrl(string backGroundImageUrl)
          {
              chart.ChartModel.BackGroundImageUrl = backGroundImageUrl;
              return this;
          }

          public ChartPropertiesBuilder CanResize(bool canResize)
          {
              chart.ChartModel.CanResize = canResize;
              return this;
          }
          public ChartPropertiesBuilder Localization(string localization)
          {
              chart.ChartModel.Localization = localization;
              return this;
          }

          public ChartPropertiesBuilder InitSeriesRender(bool initSeriesRender)
          {
              chart.ChartModel.InitSeriesRender = initSeriesRender;
              return this;
          }

          public ChartPropertiesBuilder Theme(ChartTheme theme)
          {
              chart.ChartModel.Theme = theme;
              return this;
          }

        //Client side event 
          public ChartPropertiesBuilder Load(String load)
          {
              chart.ChartModel.Load = load;
              return this;
          }
          public ChartPropertiesBuilder AxesLabelRendering(String axesLabelRendering)
          {
              chart.ChartModel.AxesLabelRendering = axesLabelRendering;
              return this;
          }
          public ChartPropertiesBuilder AxesRangeCalculate(String axesRangeCalculate)
          {
              chart.ChartModel.AxesRangeCalculate = axesRangeCalculate;
              return this;
          }
          public ChartPropertiesBuilder AxesTitleRendering(String axesTitleRendering)
          {
              chart.ChartModel.AxesTitleRendering = axesTitleRendering;
              return this;
          }
          public ChartPropertiesBuilder ChartAreaBoundsCalculate(String chartAreaBoundsCalculate)
          {
              chart.ChartModel.ChartAreaBoundsCalculate = chartAreaBoundsCalculate;
              return this;
          }
          public ChartPropertiesBuilder LegendItemRendering(String legendItemRendering)
          {
              chart.ChartModel.LegendItemRendering = legendItemRendering;
              return this;
          }
          public ChartPropertiesBuilder LengendBoundsCalculate(String lengendBoundsCalculate)
          {
              chart.ChartModel.LengendBoundsCalculate = lengendBoundsCalculate;
              return this;
          }
          public ChartPropertiesBuilder PreRender(String preRender)
          {
              chart.ChartModel.PreRender = preRender;
              return this;
          }
          public ChartPropertiesBuilder SeriesRendering(String seriesRendering)
          {
              chart.ChartModel.SeriesRendering = seriesRendering;
              return this;
          }
          public ChartPropertiesBuilder SymbolRendering(String symbolRendering)
          {
              chart.ChartModel.SymbolRendering = symbolRendering;
              return this;
          }
          public ChartPropertiesBuilder TitleRendering(String titleRendering)
          {
              chart.ChartModel.TitleRendering = titleRendering;
              return this;
          }
          public ChartPropertiesBuilder AxesLabelsInitialize(String axesLabelsInitialize)
          {
              chart.ChartModel.AxesLabelsInitialize = axesLabelsInitialize;
              return this;
          }
          public ChartPropertiesBuilder PointRegionClick(String pointRegionClick)
          {
              chart.ChartModel.PointRegionClick = pointRegionClick;
              return this;
          }
          public ChartPropertiesBuilder PointRegionMouseMove(String pointRegionMouseMove)
          {
              chart.ChartModel.PointRegionMouseMove = pointRegionMouseMove;
              return this;
          }
          public ChartPropertiesBuilder LegendItemClick(String legendItemClick)
          {
              chart.ChartModel.LegendItemClick = legendItemClick;
              return this;
          }
          public ChartPropertiesBuilder LegendItemMouseMove(String legendItemMouseMove)
          {
              chart.ChartModel.LegendItemMouseMove = legendItemMouseMove;
              return this;
          }
          public ChartPropertiesBuilder DisplayTextRendering(String displayTextRendering)
          {
              chart.ChartModel.DisplayTextRendering = displayTextRendering;
              return this;
          }
          public ChartPropertiesBuilder ToolTipInitialize(String toolTipInitialize)
          {
              chart.ChartModel.ToolTipInitialize = toolTipInitialize;
              return this;
          }
          public ChartPropertiesBuilder TrackAxisToolTip(String trackAxisToolTip)
          {
              chart.ChartModel.TrackAxisToolTip = trackAxisToolTip;
              return this;
          }
          public ChartPropertiesBuilder TrackToolTip(String trackToolTip)
          {
              chart.ChartModel.TrackToolTip = trackToolTip;
              return this;
          }
          public ChartPropertiesBuilder AnimationComplete(String animationComplete)
          {
              chart.ChartModel.AnimationComplete = animationComplete;
              return this;
          }
          public ChartPropertiesBuilder Destroy(String destroy)
          {
              chart.ChartModel.Destroy = destroy;
              return this;
          }
          public ChartPropertiesBuilder Create(String create)
          {
              chart.ChartModel.Create = create;
              return this;
          }


          //Render
          public HtmlString Render()
          {
              return new HtmlString(chart.Render().ToString());
          }

          public override String ToString()
          {

              return Render().ToString();
          }
          

    }
}
