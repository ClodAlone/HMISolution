#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.RDL.Data;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingChartControl : ContentControl
    {
        Syncfusion.UI.Xaml.Charts.SfChart chart = null;

        ChartItemExpVal ChartPro;

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        ChartModel chartModel;

        public ExpressionEngine ExpressionEngine { get; set; }

        ReportingBrushConverter brush;

        public ReportingChartControl(IReportItemModeler model)
        {
            this.Model = model;
            chartModel = this.Model as ChartModel;
            this.ChartPro = chartModel.ChartProperties;
            this.ExpressionEngine = chartModel.Model.ExpressionEngine;
            brush = new ReportingBrushConverter();
            this.chart = new SfChart();
            Initialize();
            this.Content = this.chart;
        }

        void Initialize()
        {
            this.Height = this.chartModel.Height;
            this.Width = this.chartModel.Width;

            if (this.chartModel.PrintPageInfo != null)
            {
                Canvas.SetLeft(this, this.chartModel.PrintPageInfo.ActualLeft);
                Canvas.SetTop(this, this.chartModel.PrintPageInfo.ActualTop);
            }
            else if (this.chartModel.IsTablixChild && this.chartModel.PageInfo != null)
            {
                Canvas.SetLeft(this, this.chartModel.PageInfo.ActualLeft);
                Canvas.SetTop(this, this.chartModel.PageInfo.ActualTop);
            }

            this.chart.Height = this.Height;
            this.chart.Width = this.Width;

            int CurrentColorIndex = -1;

            foreach (ChartAreasExpVal area in ChartPro.ChartAreas)
            {
            
                CategoryAxis cAxis = new CategoryAxis();
                NumericalAxis nAxis = new NumericalAxis();
                this.chart.PrimaryAxis = cAxis;
                this.chart.SecondaryAxis = nAxis;

                if (area.ChartSeries.First().ChartAreaXAxis.First().Visible == BooleanOptions.False)
                {
                    this.chart.PrimaryAxis.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                if (area.ChartSeries.First().ChartAreaYAxis.First().Visible == BooleanOptions.False)
                {
                    this.chart.SecondaryAxis.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                bool isXFlag = false;
                bool isYFlag = false;
                //var datas = chartModel.Engine.SeriesDataSources;
                int count = 0;
                this.chart.Series.Clear();
                if (this.ChartPro.Type == DOM.VisualizationType.Area)
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Stacked)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            StackingAreaSeries stackAreaSeries = new StackingAreaSeries();
                            this.chart.Series.Add(stackAreaSeries);
                            stackAreaSeries.XBindingPath = "X";
                            stackAreaSeries.YBindingPath = "Y";

                            stackAreaSeries.ItemsSource = datas.DataSource;

                            if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                stackAreaSeries.PointerEntered += AreaSeries_PointerEntered;
                                stackAreaSeries.PointerExited += AreaSeries_PointerExited;
                                stackAreaSeries.PointerPressed += AreaSeries_PointerPressed;
                            }
                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    stackAreaSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    stackAreaSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                stackAreaSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                stackAreaSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        stackAreaSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        stackAreaSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            stackAreaSeries.Interior = GradientColor(stackAreaSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            stackAreaSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        stackAreaSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        stackAreaSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            stackAreaSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (stackAreaSeries.AdornmentsInfo == null)
                                    stackAreaSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                stackAreaSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    stackAreaSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    stackAreaSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        stackAreaSeries.ShowEmptyPoints = true;
                                        stackAreaSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        stackAreaSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    stackAreaSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                stackAreaSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                stackAreaSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackAreaSeries.XAxis == null)
                                    {
                                        stackAreaSeries.XAxis = new CategoryAxis();
                                    }
                                    stackAreaSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackAreaSeries.YAxis == null)
                                    {
                                        stackAreaSeries.YAxis = new NumericalAxis();
                                    }
                                    stackAreaSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            AreaSeries areaSeries = new AreaSeries();
                            this.chart.Series.Add(areaSeries);
                            areaSeries.XBindingPath = "X";
                            areaSeries.YBindingPath = "Y";

                            areaSeries.ItemsSource = datas.DataSource;

                            if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                areaSeries.PointerEntered += AreaSeries_PointerEntered;
                                areaSeries.PointerExited += AreaSeries_PointerExited;
                                areaSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    areaSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    areaSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                areaSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                areaSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        areaSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        areaSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            areaSeries.Interior = GradientColor(areaSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            areaSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        areaSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        areaSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            areaSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (areaSeries.AdornmentsInfo == null)
                                    areaSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                areaSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    areaSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    areaSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        areaSeries.ShowEmptyPoints = true;
                                        areaSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        areaSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    areaSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                areaSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                areaSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (areaSeries.XAxis == null)
                                    {
                                        areaSeries.XAxis = new CategoryAxis();
                                    }
                                    areaSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (areaSeries.YAxis == null)
                                    {
                                        areaSeries.YAxis = new NumericalAxis();
                                    }
                                    areaSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Line)
                {
                    foreach (var datas in chartModel.Engine.SeriesDataSources)
                    {
                        int temp = 0;
                        if (area.ChartSeries.Count > count)
                        {
                            temp = count;
                        }
                        else
                        {
                            temp = 0;
                        }
                        var seriesItem = area.ChartSeries[temp];
                        LineSeries lineSeries = new LineSeries();
                        this.chart.Series.Add(lineSeries);
                        lineSeries.XBindingPath = "X";
                        lineSeries.YBindingPath = "Y";

                        lineSeries.ItemsSource = datas.DataSource;
						
						if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                lineSeries.PointerEntered += AreaSeries_PointerEntered;
                                lineSeries.PointerExited += AreaSeries_PointerExited;
                                lineSeries.PointerPressed += AreaSeries_PointerPressed;
                            }


                        if (seriesItem.Legend != null)
                        {
                            if (seriesItem.Legend.Hidden)
                            {
                                lineSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                            else
                            {
                                lineSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                            }
                        }

                        if (ChartPro.Type != VisualizationType.Line)
                            lineSeries.LegendIcon = ChartLegendIcon.Rectangle;
                        else
                            lineSeries.LegendIcon = ChartLegendIcon.StraightLine;

                        try
                        {
                            var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                            if (pointStyle.Style != null)
                            {
                                if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                    lineSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                else if (ChartPro.CustomPaletteColors.Count > 1)
                                {
                                    if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                    {
                                        CurrentColorIndex = -1;
                                    }
                                    CurrentColorIndex++;
                                    lineSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                }

                                if (pointStyle.Style.FillStyle != null)
                                {
                                    if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                    {
                                        lineSeries.Interior = GradientColor(lineSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                    }
                                }

                                if (pointStyle.Style.Border != null)
                                {
                                    if (pointStyle.Style.Border.BorderWidth != null)
                                        lineSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                    lineSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                    lineSeries.StrokeThickness = 2;

                                    if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                    {
                                        lineSeries.StrokeThickness = 1;
                                    }
                                }
                            }
                        }
                        catch
                        { }

                        ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                        if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                        {
                            if (lineSeries.AdornmentsInfo == null)
                                lineSeries.AdornmentsInfo = new ChartAdornmentInfo();
                            lineSeries.AdornmentsInfo.ShowLabel = label.Visible;

                            if (label.Format != null)
                            {
                                lineSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                lineSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                            }
                        }

                        if (seriesItem.EmptyPointsStyle != null)
                        {
                            if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                            {
                                if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                {
                                    lineSeries.ShowEmptyPoints = true;
                                    lineSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                    string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                    lineSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                }

                                string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                lineSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                            }
                        }


                        if (seriesItem.Legend != null)
                        {
                            lineSeries.Label = seriesItem.Legend.LegendText;
                        }
                        else
                        {
                            lineSeries.Label = datas.Label;
                        }

                        if (count == 0)
                        {
                            if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                {
                                    chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                }
                                if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                        isXFlag = true;
                                }
                                this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                            }

                            if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                {
                                    chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                }
                                if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                        isYFlag = true;
                                }
                                this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                            }

                            if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                            {
                                if (lineSeries.XAxis == null)
                                {
                                    lineSeries.XAxis = new CategoryAxis();
                                }
                                lineSeries.XAxis.OpposedPosition = true;
                            }

                            if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                            {
                                if (lineSeries.YAxis == null)
                                {
                                    lineSeries.YAxis = new NumericalAxis();
                                }
                                lineSeries.YAxis.OpposedPosition = true;
                            }

                        }
                        count++;
                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Polar)
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Radar)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            RadarSeries radarSeries = new RadarSeries();
                            this.chart.Series.Add(radarSeries);
                            radarSeries.XBindingPath = "X";
                            radarSeries.YBindingPath = "Y";

                            radarSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                radarSeries.PointerEntered += AreaSeries_PointerEntered;
                                radarSeries.PointerExited += AreaSeries_PointerExited;
                                radarSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    radarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    radarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                radarSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                radarSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        radarSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        radarSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            radarSeries.Interior = GradientColor(radarSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            radarSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        radarSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        radarSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            radarSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (radarSeries.AdornmentsInfo == null)
                                    radarSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                radarSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    radarSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    radarSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        radarSeries.ShowEmptyPoints = true;
                                        radarSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        radarSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    radarSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                radarSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                radarSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (radarSeries.XAxis == null)
                                    {
                                        radarSeries.XAxis = new CategoryAxis();
                                    }
                                    radarSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (radarSeries.YAxis == null)
                                    {
                                        radarSeries.YAxis = new NumericalAxis();
                                    }
                                    radarSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            PolarSeries polarSeries = new PolarSeries();
                            this.chart.Series.Add(polarSeries);
                            polarSeries.XBindingPath = "X";
                            polarSeries.YBindingPath = "Y";

                            polarSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                polarSeries.PointerEntered += AreaSeries_PointerEntered;
                                polarSeries.PointerExited += AreaSeries_PointerExited;
                                polarSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    polarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    polarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                polarSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                polarSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        polarSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        polarSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            polarSeries.Interior = GradientColor(polarSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            polarSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        polarSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        polarSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            polarSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (polarSeries.AdornmentsInfo == null)
                                    polarSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                polarSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    polarSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    polarSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        polarSeries.ShowEmptyPoints = true;
                                        polarSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        polarSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    polarSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                polarSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                polarSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (polarSeries.XAxis == null)
                                    {
                                        polarSeries.XAxis = new CategoryAxis();
                                    }
                                    polarSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (polarSeries.YAxis == null)
                                    {
                                        polarSeries.YAxis = new NumericalAxis();
                                    }
                                    polarSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }

                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Range)
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Candlestick)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            CandleSeries candleSeries = new CandleSeries();
                            this.chart.Series.Add(candleSeries);
                            candleSeries.XBindingPath = "X";

                            candleSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                candleSeries.PointerEntered += AreaSeries_PointerEntered;
                                candleSeries.PointerExited += AreaSeries_PointerExited;
                                candleSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    candleSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    candleSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                candleSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                candleSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        candleSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        candleSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            candleSeries.Interior = GradientColor(candleSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            candleSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        candleSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        candleSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            candleSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (candleSeries.AdornmentsInfo == null)
                                    candleSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                candleSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    candleSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    candleSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        candleSeries.ShowEmptyPoints = true;
                                        candleSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        candleSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    candleSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                candleSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                candleSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (candleSeries.XAxis == null)
                                    {
                                        candleSeries.XAxis = new CategoryAxis();
                                    }
                                    candleSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (candleSeries.YAxis == null)
                                    {
                                        candleSeries.YAxis = new NumericalAxis();
                                    }
                                    candleSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                    else if (this.ChartPro.SubType == DOM.VisualizationSubType.Stock)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            HiLoOpenCloseSeries hiloOpenCloseSeries = new HiLoOpenCloseSeries();
                            this.chart.Series.Add(hiloOpenCloseSeries);
                            hiloOpenCloseSeries.XBindingPath = "X";

                            hiloOpenCloseSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                hiloOpenCloseSeries.PointerEntered += AreaSeries_PointerEntered;
                                hiloOpenCloseSeries.PointerExited += AreaSeries_PointerExited;
                                hiloOpenCloseSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    hiloOpenCloseSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    hiloOpenCloseSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                hiloOpenCloseSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                hiloOpenCloseSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        hiloOpenCloseSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        hiloOpenCloseSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            hiloOpenCloseSeries.Interior = GradientColor(hiloOpenCloseSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            hiloOpenCloseSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        hiloOpenCloseSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        hiloOpenCloseSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            hiloOpenCloseSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (hiloOpenCloseSeries.AdornmentsInfo == null)
                                    hiloOpenCloseSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                hiloOpenCloseSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    hiloOpenCloseSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    hiloOpenCloseSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        hiloOpenCloseSeries.ShowEmptyPoints = true;
                                        hiloOpenCloseSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        hiloOpenCloseSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    hiloOpenCloseSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                hiloOpenCloseSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                hiloOpenCloseSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (hiloOpenCloseSeries.XAxis == null)
                                    {
                                        hiloOpenCloseSeries.XAxis = new CategoryAxis();
                                    }
                                    hiloOpenCloseSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (hiloOpenCloseSeries.YAxis == null)
                                    {
                                        hiloOpenCloseSeries.YAxis = new NumericalAxis();
                                    }
                                    hiloOpenCloseSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }

                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            HiLoSeries hiLoSeries = new HiLoSeries();
                            this.chart.Series.Add(hiLoSeries);
                            hiLoSeries.XBindingPath = "X";

                            hiLoSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                hiLoSeries.PointerEntered += AreaSeries_PointerEntered;
                                hiLoSeries.PointerExited += AreaSeries_PointerExited;
                                hiLoSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    hiLoSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    hiLoSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                hiLoSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                hiLoSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        hiLoSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        hiLoSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            hiLoSeries.Interior = GradientColor(hiLoSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            hiLoSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        hiLoSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        hiLoSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            hiLoSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (hiLoSeries.AdornmentsInfo == null)
                                    hiLoSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                hiLoSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    hiLoSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    hiLoSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        hiLoSeries.ShowEmptyPoints = true;
                                        hiLoSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        hiLoSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }
                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    hiLoSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                hiLoSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                hiLoSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (hiLoSeries.XAxis == null)
                                    {
                                        hiLoSeries.XAxis = new CategoryAxis();
                                    }
                                    hiLoSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (hiLoSeries.YAxis == null)
                                    {
                                        hiLoSeries.YAxis = new NumericalAxis();
                                    }
                                    hiLoSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Scatter)
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Bubble)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            BubbleSeries bubbleSeries = new BubbleSeries();
                            this.chart.Series.Add(bubbleSeries);
                            bubbleSeries.XBindingPath = "X";
                            bubbleSeries.YBindingPath = "Y";

                            bubbleSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                bubbleSeries.PointerEntered += AreaSeries_PointerEntered;
                                bubbleSeries.PointerExited += AreaSeries_PointerExited;
                                bubbleSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    bubbleSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    bubbleSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                bubbleSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                bubbleSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        bubbleSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        bubbleSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            bubbleSeries.Interior = GradientColor(bubbleSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            bubbleSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        bubbleSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        bubbleSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            bubbleSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (bubbleSeries.AdornmentsInfo == null)
                                    bubbleSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                bubbleSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    bubbleSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    bubbleSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        bubbleSeries.ShowEmptyPoints = true;
                                        bubbleSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        bubbleSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    bubbleSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                bubbleSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                bubbleSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (bubbleSeries.XAxis == null)
                                    {
                                        bubbleSeries.XAxis = new CategoryAxis();
                                    }
                                    bubbleSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (bubbleSeries.YAxis == null)
                                    {
                                        bubbleSeries.YAxis = new NumericalAxis();
                                    }
                                    bubbleSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            ScatterSeries scatterSeries = new ScatterSeries();
                            this.chart.Series.Add(scatterSeries);
                            scatterSeries.XBindingPath = "X";
                            scatterSeries.YBindingPath = "Y";

                            scatterSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                scatterSeries.PointerEntered += AreaSeries_PointerEntered;
                                scatterSeries.PointerExited += AreaSeries_PointerExited;
                                scatterSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    scatterSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    scatterSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                scatterSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                scatterSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        scatterSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        scatterSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            scatterSeries.Interior = GradientColor(scatterSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            scatterSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        scatterSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        scatterSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            scatterSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (scatterSeries.AdornmentsInfo == null)
                                    scatterSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                scatterSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    scatterSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    scatterSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        scatterSeries.ShowEmptyPoints = true;
                                        scatterSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        scatterSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    scatterSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                scatterSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                scatterSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (scatterSeries.XAxis == null)
                                    {
                                        scatterSeries.XAxis = new CategoryAxis();
                                    }
                                    scatterSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (scatterSeries.YAxis == null)
                                    {
                                        scatterSeries.YAxis = new NumericalAxis();
                                    }
                                    scatterSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Shape)
                {
                    foreach (var datas in chartModel.Engine.SeriesDataSources)
                    {
                        int temp = 0;
                        if (area.ChartSeries.Count > count)
                        {
                            temp = count;
                        }
                        else
                        {
                            temp = 0;
                        }
                        var seriesItem = area.ChartSeries[temp];
                        PieSeries pieSeries = new PieSeries();
                        this.chart.Series.Add(pieSeries);
                        pieSeries.XBindingPath = "X";
                        pieSeries.YBindingPath = "Y";

                        pieSeries.ItemsSource = datas.DataSource;

						if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                pieSeries.PointerEntered += AreaSeries_PointerEntered;
                                pieSeries.PointerExited += AreaSeries_PointerExited;
                                pieSeries.PointerPressed += AreaSeries_PointerPressed;
                            }
						
                        if (seriesItem.Legend != null)
                        {
                            if (seriesItem.Legend.Hidden)
                            {
                                pieSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                            else
                            {
                                pieSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                            }
                        }

                        if (ChartPro.Type != VisualizationType.Line)
                            pieSeries.LegendIcon = ChartLegendIcon.Rectangle;
                        else
                            pieSeries.LegendIcon = ChartLegendIcon.StraightLine;

                        try
                        {
                            var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                            if (pointStyle.Style != null)
                            {
                                //if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                //    pieSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                //else if (ChartPro.CustomPaletteColors.Count > 1)
                                //{
                                //    if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                //    {
                                //        CurrentColorIndex = -1;
                                //    }
                                //    CurrentColorIndex++;
                                //    pieSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                //}

                                if (pointStyle.Style.FillStyle != null)
                                {
                                    if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                    {
                                        pieSeries.Interior = GradientColor(pieSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                    }
                                }

                                if (pointStyle.Style.Border != null)
                                {
                                    if (pointStyle.Style.Border.BorderWidth != null)
                                        pieSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                    pieSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                    pieSeries.StrokeThickness = 2;

                                    if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                    {
                                        pieSeries.StrokeThickness = 1;
                                    }
                                }
                            }
                        }
                        catch
                        { }

                        ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                        if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                        {
                            if (pieSeries.AdornmentsInfo == null)
                                pieSeries.AdornmentsInfo = new ChartAdornmentInfo();
                            pieSeries.AdornmentsInfo.ShowLabel = label.Visible;

                            if (label.Format != null)
                            {
                                pieSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                pieSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                            }
                        }

                        if (seriesItem.EmptyPointsStyle != null)
                        {
                            if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                            {
                                if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                {
                                    pieSeries.ShowEmptyPoints = true;
                                    pieSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                    string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                    pieSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                }

                                string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                pieSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                            }
                        }


                        if (seriesItem.Legend != null)
                        {
                            pieSeries.Label = seriesItem.Legend.LegendText;
                        }
                        else
                        {
                            pieSeries.Label = datas.Label;
                        }

                        if (count == 0)
                        {
                            if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                {
                                    chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                }
                                this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                            }

                            if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                {
                                    chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                }
                                this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                            }

                        }
                        count++;
                    }
                }
                else if (this.ChartPro.Type == DOM.VisualizationType.Bar)
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Stacked)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];

                            StackingBarSeries stackBarSeries = new StackingBarSeries();
                            this.chart.Series.Add(stackBarSeries);
                            stackBarSeries.XBindingPath = "X";
                            stackBarSeries.YBindingPath = "Y";

                            stackBarSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                stackBarSeries.PointerEntered += AreaSeries_PointerEntered;
                                stackBarSeries.PointerExited += AreaSeries_PointerExited;
                                stackBarSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    stackBarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    stackBarSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                stackBarSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                stackBarSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        stackBarSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        stackBarSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            stackBarSeries.Interior = GradientColor(stackBarSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            stackBarSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        stackBarSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        stackBarSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            stackBarSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (stackBarSeries.AdornmentsInfo == null)
                                    stackBarSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                stackBarSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    stackBarSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    stackBarSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        stackBarSeries.ShowEmptyPoints = true;
                                        stackBarSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        stackBarSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    stackBarSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                stackBarSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                stackBarSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackBarSeries.XAxis == null)
                                    {
                                        stackBarSeries.XAxis = new CategoryAxis();
                                    }
                                    stackBarSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackBarSeries.YAxis == null)
                                    {
                                        stackBarSeries.YAxis = new NumericalAxis();
                                    }
                                    stackBarSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];

                            BarSeries barSeries = new BarSeries();
                            this.chart.Series.Add(barSeries);
                            barSeries.XBindingPath = "X";
                            barSeries.YBindingPath = "Y";

                            barSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                barSeries.PointerEntered += AreaSeries_PointerEntered;
                                barSeries.PointerExited += AreaSeries_PointerExited;
                                barSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    barSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    barSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                barSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                barSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        barSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        barSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            barSeries.Interior = GradientColor(barSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            barSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        barSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        barSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            barSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (barSeries.AdornmentsInfo == null)
                                    barSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                barSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    barSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    barSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        barSeries.ShowEmptyPoints = true;
                                        barSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        barSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    barSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                barSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                barSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (barSeries.XAxis == null)
                                    {
                                        barSeries.XAxis = new CategoryAxis();
                                    }
                                    barSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (barSeries.YAxis == null)
                                    {
                                        barSeries.YAxis = new NumericalAxis();
                                    }
                                    barSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }

                    }
                }
                else
                {
                    if (this.ChartPro.SubType == DOM.VisualizationSubType.Stacked)
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];
                            StackingColumnSeries stackColumnSeries = new StackingColumnSeries();
                            this.chart.Series.Add(stackColumnSeries);
                            stackColumnSeries.XBindingPath = "X";
                            stackColumnSeries.YBindingPath = "Y";

                            stackColumnSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                stackColumnSeries.PointerEntered += AreaSeries_PointerEntered;
                                stackColumnSeries.PointerExited += AreaSeries_PointerExited;
                                stackColumnSeries.PointerPressed += AreaSeries_PointerPressed;
                            }

                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    stackColumnSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    stackColumnSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                stackColumnSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                stackColumnSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        stackColumnSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        stackColumnSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            stackColumnSeries.Interior = GradientColor(stackColumnSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            stackColumnSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        stackColumnSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        stackColumnSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            stackColumnSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (stackColumnSeries.AdornmentsInfo == null)
                                    stackColumnSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                stackColumnSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    stackColumnSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    stackColumnSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        stackColumnSeries.ShowEmptyPoints = true;
                                        stackColumnSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        stackColumnSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    stackColumnSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                stackColumnSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                stackColumnSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackColumnSeries.XAxis == null)
                                    {
                                        stackColumnSeries.XAxis = new CategoryAxis();
                                    }
                                    stackColumnSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (stackColumnSeries.YAxis == null)
                                    {
                                        stackColumnSeries.YAxis = new NumericalAxis();
                                    }
                                    stackColumnSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }

                    }
                    else
                    {
                        foreach (var datas in chartModel.Engine.SeriesDataSources)
                        {
                            int temp = 0;
                            if (area.ChartSeries.Count > count)
                            {
                                temp = count;
                            }
                            else
                            {
                                temp = 0;
                            }
                            var seriesItem = area.ChartSeries[temp];

                            ColumnSeries columnSeries = new ColumnSeries();
                            this.chart.Series.Add(columnSeries);
                            columnSeries.XBindingPath = "X";
                            columnSeries.YBindingPath = "Y";

                            columnSeries.ItemsSource = datas.DataSource;
							
							if (chartModel.Engine.DrillActions != null && chartModel.Engine.DrillActions.ContainsKey(count))
                            {
                                columnSeries.PointerEntered += AreaSeries_PointerEntered;
                                columnSeries.PointerExited += AreaSeries_PointerExited;
                                columnSeries.PointerPressed += AreaSeries_PointerPressed;
                            }


                            if (seriesItem.Legend != null)
                            {
                                if (seriesItem.Legend.Hidden)
                                {
                                    columnSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Collapsed;
                                }
                                else
                                {
                                    columnSeries.VisibilityOnLegend = Windows.UI.Xaml.Visibility.Visible;
                                }
                            }

                            if (ChartPro.Type != VisualizationType.Line)
                                columnSeries.LegendIcon = ChartLegendIcon.Rectangle;
                            else
                                columnSeries.LegendIcon = ChartLegendIcon.StraightLine;

                            try
                            {
                                var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                                if (pointStyle.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                        columnSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                                    else if (ChartPro.CustomPaletteColors.Count > 1)
                                    {
                                        if (CurrentColorIndex >= ChartPro.CustomPaletteColors.Count)
                                        {
                                            CurrentColorIndex = -1;
                                        }
                                        CurrentColorIndex++;
                                        columnSeries.Interior = (Brush)brush.ConvertFromString(ChartPro.CustomPaletteColors[CurrentColorIndex]);
                                    }

                                    if (pointStyle.Style.FillStyle != null)
                                    {
                                        if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                        {
                                            columnSeries.Interior = GradientColor(columnSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                        }
                                    }

                                    if (pointStyle.Style.Border != null)
                                    {
                                        if (pointStyle.Style.Border.BorderWidth != null)
                                            columnSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                        columnSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);
                                        columnSeries.StrokeThickness = 2;

                                        if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                        {
                                            columnSeries.StrokeThickness = 1;
                                        }
                                    }
                                }
                            }
                            catch
                            { }

                            ChartDataLabelExpVal label = seriesItem.PointValues.FirstOrDefault().ChartDataLabel;
                            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
                            {
                                if (columnSeries.AdornmentsInfo == null)
                                    columnSeries.AdornmentsInfo = new ChartAdornmentInfo();
                                columnSeries.AdornmentsInfo.ShowLabel = label.Visible;

                                if (label.Format != null)
                                {
                                    columnSeries.AdornmentsInfo.SegmentLabelFormat = label.Format;
                                    columnSeries.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                                }
                            }

                            if (seriesItem.EmptyPointsStyle != null)
                            {
                                if (seriesItem.EmptyPointsStyle.ChartMarker != null)
                                {
                                    if (!string.IsNullOrEmpty(seriesItem.EmptyPointsStyle.ChartMarker.Type))
                                    {
                                        columnSeries.ShowEmptyPoints = true;
                                        columnSeries.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                                        string eps = seriesItem.EmptyPointsStyle.ChartMarker.Type;
                                        columnSeries.AdornmentsInfo.Symbol = (ChartSymbol)Enum.Parse(typeof(ChartSymbol), eps, true);
                                    }

                                    string epsc = seriesItem.EmptyPointsStyle.ChartMarker.Color;
                                    columnSeries.EmptyPointInterior = brush.ConvertFromString(epsc);
                                }
                            }


                            if (seriesItem.Legend != null)
                            {
                                columnSeries.Label = seriesItem.Legend.LegendText;
                            }
                            else
                            {
                                columnSeries.Label = datas.Label;
                            }

                            if (count == 0)
                            {
                                if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaXAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaXAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.PrimaryAxis.Header = seriesItem.ChartAreaXAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaXAxis[0].ChartMajorGridLines.Enabled != BooleanOptions.False)
                                            isXFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaXAxis[0], ChartPro.Type, true);
                                }

                                if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null && (!seriesItem.ChartAreaYAxis[0].Name.Equals("Secondary", StringComparison.OrdinalIgnoreCase)))
                                {
                                    if (seriesItem.ChartAreaYAxis[0].ChartAxisTitle != null)
                                    {
                                        chart.SecondaryAxis.Header = seriesItem.ChartAreaYAxis[0].ChartAxisTitle.Caption.ToString();
                                    }
                                    if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines != null)
                                    {
                                        if (seriesItem.ChartAreaYAxis[0].ChartMajorGridLines.Enabled == BooleanOptions.True)
                                            isYFlag = true;
                                    }
                                    this.ApplyChartLabelStyles(this.chart, seriesItem.ChartAreaYAxis[0], ChartPro.Type, false);
                                }

                                if (seriesItem.CategoryAxisName == seriesItem.ChartAreaXAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (columnSeries.XAxis == null)
                                    {
                                        columnSeries.XAxis = new CategoryAxis();
                                    }
                                    columnSeries.XAxis.OpposedPosition = true;
                                }

                                if (seriesItem.ValueAxisName == seriesItem.ChartAreaYAxis[1].Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (columnSeries.YAxis == null)
                                    {
                                        columnSeries.YAxis = new NumericalAxis();
                                    }
                                    columnSeries.YAxis.OpposedPosition = true;
                                }

                            }
                            count++;
                        }
                    }
                }

                this.chart.AreaBorderThickness = new Thickness(0);

                if (area.ChartSeries.Count > 0)
                {
                    if (area.ChartSeries[0].ChartAreaYAxis[0].Style != null)
                    {
                        if (area.ChartSeries[0].ChartAreaYAxis[0].Style.BorderWidth != null)
                            nAxis.AxisLineStyle = this.GetLineStyle(area.ChartSeries[0].ChartAreaYAxis[0].Style.BorderColor, area.ChartSeries[0].ChartAreaYAxis[0].Style.BorderWidth.PixelValue);
                    }
                    else
                    {
                        nAxis.AxisLineStyle = this.GetLineStyle("Black", 1);
                    }

                    if (area.ChartSeries[0].ChartAreaXAxis[0].Style != null)
                    {
                        if (area.ChartSeries[0].ChartAreaXAxis[0].Style.BorderWidth != null)
                            cAxis.AxisLineStyle = this.GetLineStyle(area.ChartSeries[0].ChartAreaXAxis[0].Style.BorderColor, area.ChartSeries[0].ChartAreaXAxis[0].Style.BorderWidth.PixelValue);
                    }
                    else
                    {
                        cAxis.AxisLineStyle = this.GetLineStyle("Black", 1);
                    }
                }


                nAxis.EnableAutoIntervalOnZooming = true;

                this.chart.PrimaryAxis.ShowGridLines = isXFlag;
                this.chart.SecondaryAxis.ShowGridLines = isYFlag;

                //nAxis.MajorGridLineStyle = this.GetLineStyle("Black", 1);
                //cAxis.MinorGridLineStyle = this.GetLineStyle("Black", 1);


                if (area.Style != null)
                {
                    if (area.Style.FillStyle != null)
                        this.chart.Background = (Brush)brush.ConvertFromString(area.Style.FillStyle.BackgroundColor);
                }

            }

            if (this.ChartPro.ChartLegends != null && this.ChartPro.ChartLegends.Count > 0)
            {
                ChartLegendExpVal legend = ChartPro.ChartLegends.FirstOrDefault();
                Syncfusion.UI.Xaml.Charts.ChartLegend chartLegend = new Syncfusion.UI.Xaml.Charts.ChartLegend();

                chartLegend.Background = new SolidColorBrush(Colors.Transparent);
                chartLegend.BorderThickness = new Thickness(0);
                chartLegend.Foreground = new SolidColorBrush(Colors.Black);
                if (legend.Style != null && !string.IsNullOrEmpty(legend.Style.Font.FontFamily))
                    chartLegend.FontFamily = new FontFamily(legend.Style.Font.FontFamily);

                chartLegend.Visibility = legend.Hidden ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
                chartLegend.DockPosition = this.GetChartLegendPosition(legend.Position);
                if (ChartPro.Type == VisualizationType.Shape)
                {
                    chartLegend.ItemTemplate = this.GetLegendTemplate();
                }
                this.chart.Legend = chartLegend;
            }

            if (this.ChartPro.ChartStyle != null)
            {
                if (this.ChartPro.ChartStyle.Border != null)
                {
                    if (!string.IsNullOrEmpty(this.ChartPro.ChartStyle.Border.BorderColor))
                        this.BorderBrush = brush.ConvertFromString(this.ChartPro.ChartStyle.Border.BorderColor);
                    if (this.ChartPro.ChartStyle.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && !string.IsNullOrEmpty(this.ChartPro.ChartStyle.FillStyle.BackgroundGradientEndcolor))
                        this.Background = GradientColor(this.ChartPro.ChartStyle.FillStyle.BackgroundColor, this.ChartPro.ChartStyle.FillStyle.BackgroundGradientEndcolor);
                    else if (!string.IsNullOrEmpty(this.ChartPro.ChartStyle.FillStyle.BackgroundColor))
                        this.Background = brush.ConvertFromString(this.ChartPro.ChartStyle.FillStyle.BackgroundColor);
                }
            }
        }

        Syncfusion.RDL.DOM.Parameters GetParameters(List<TextboxParameterExpVal> action)
        {
            if (action != null)
            {
                Syncfusion.RDL.DOM.Parameters parameters = new Parameters();
                foreach (var para in action)
                {
                    Syncfusion.RDL.DOM.Parameter parameter = new Parameter();
                    parameter.Name = para.Name;
                    parameter.Omit = para.Omit;
                    parameter.Value = para.Value;
                    parameters.Add(parameter);
                }
                return parameters;
            }
            return null;
        }

        private void AreaSeries_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ChartSegment segment = ((e.OriginalSource as FrameworkElement).Tag as ChartSegment);

                if (segment != null)
                {
                    ReportingChartData chartData = segment.Item as ReportingChartData;
                    if (chartData != null && chartData.ActionInfo != null)
                    {
                        TextboxActionInfoExpVal actionInfo = chartData.ActionInfo as TextboxActionInfoExpVal;
                        if (!string.IsNullOrEmpty(actionInfo.BookmarkLink))
                        {
                            Windows.System.Launcher.LaunchUriAsync(new Uri(actionInfo.BookmarkLink));
                        }
                        else if (!string.IsNullOrEmpty(actionInfo.Hyperlink))
                        {
                            Windows.System.Launcher.LaunchUriAsync(new Uri(actionInfo.Hyperlink));
                        }
                        else if (!string.IsNullOrEmpty(actionInfo.ReportName))
                        {
                            Drillthrough drillthrough = new Drillthrough()
                                {
                                    Parameters = this.GetParameters(actionInfo.Parameters),
                                    ReportName = actionInfo.ReportName
                                };
                            this.chartModel.Model.DrillThroughReport(this.chartModel.Model, drillthrough);
                        }

                    }
                }
            }
            catch{
            }
        }

        void AreaSeries_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        void AreaSeries_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);            
        }

        Windows.UI.Xaml.Style GetLineStyle(string color, double thickness)
        {
            Windows.UI.Xaml.Style style = new Windows.UI.Xaml.Style() { TargetType = typeof(Windows.UI.Xaml.Shapes.Line) };
            Setter setter1 = new Setter();
            setter1.Property = Windows.UI.Xaml.Shapes.Line.StrokeProperty;
            setter1.Value = (Brush)brush.ConvertFromInvariantString(color);
            style.Setters.Add(setter1);

            Setter setter2 = new Setter();
            setter2.Property = Windows.UI.Xaml.Shapes.Line.StrokeThicknessProperty;
            setter2.Value = thickness;
            style.Setters.Add(setter2);

            return style;
        }


        DataTemplate GetLegendTemplate()
        {
            string template = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><StackPanel Orientation=\"Horizontal\"><Rectangle Fill=\"{Binding Interior}\" Height=\"10\" Width=\"15\"/>"
                + "<TextBlock Margin=\"2,0,0,0\" VerticalAlignment=\"Center\" Height=\"15\" TextWrapping=\"Wrap\" MaxHeight=\"15\" LineHeight=\"15\" Text=\"{Binding Item.X}\"/></StackPanel></DataTemplate>";

            return XamlReader.Load(template.ToString()) as DataTemplate;
        }


        ChartDock GetChartLegendPosition(Positions position)
        {
            switch (position)
            {
                case Positions.BottomCenter:
                    return ChartDock.Bottom;
                case Positions.BottomLeft:
                    return ChartDock.Bottom;
                case Positions.BottomRight:
                    return ChartDock.Bottom;
                case Positions.LeftBottom:
                    return ChartDock.Left;
                case Positions.LeftCenter:
                    return ChartDock.Left;
                case Positions.LeftTop:
                    return ChartDock.Left;
                case Positions.RightBottom:
                    return ChartDock.Right;
                case Positions.RightCenter:
                    return ChartDock.Right;
                case Positions.RightTop:
                    return ChartDock.Right;
                case Positions.TopCenter:
                    return ChartDock.Top;
                case Positions.TopLeft:
                    return ChartDock.Top;
                case Positions.TopRight:
                    return ChartDock.Top;
            }
            return ChartDock.Top;
        }

        LinearGradientBrush GradientColor(string color, string gradientEndColor)
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            GradientStopCollection gsc = new GradientStopCollection();
            GradientStop gs = new GradientStop();

            Color bgcolor;
            Canvas colorCanvas = new Canvas();
            colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(color);
            SolidColorBrush sbrush = (SolidColorBrush)colorCanvas.Background;
            bgcolor = (Color)sbrush.Color;

            gs.Color = bgcolor;
            gs.Offset = 1;
            gsc.Add(gs);

            gs = new GradientStop();

            colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
            sbrush = (SolidColorBrush)colorCanvas.Background;
            gs.Color = (Color)sbrush.Color;

            gs.Offset = 0.282;
            gsc.Add(gs);

            lgb.GradientStops = gsc;
            lgb.EndPoint = new Point(0.495, 0.018);
            lgb.StartPoint = new Point(0.49, 0.996);
            return lgb;
        }

        //Applying Label Style
        void ApplyChartLabelStyles(Syncfusion.UI.Xaml.Charts.SfChart chartCtrl, ChartAreaAxisExpVal axis, VisualizationType chartType, bool isPrimaryAxis)
        {
            if (isPrimaryAxis)
            {
                if (axis.LabelsAutoFitDisabled)
                {
                    if (axis.Angle > 0)
                        chartCtrl.PrimaryAxis.LabelRotationAngle = axis.Angle;
                }
                chartCtrl.PrimaryAxis.LabelsIntersectAction = AxisLabelsIntersectAction.MultipleRows;

                if (axis.LabelFont != null)
                {
                    if (!string.IsNullOrEmpty(axis.LabelFont.FontFamily))
                        chartCtrl.PrimaryAxis.FontFamily = new FontFamily(axis.LabelFont.FontFamily);

                    if (axis.LabelFont.FontSize != null)
                        chartCtrl.PrimaryAxis.FontSize = axis.LabelFont.FontSize.PixelValue;
                }
                if (axis.LabelColor != null)
                {
                    chartCtrl.PrimaryAxis.Foreground = brush.ConvertFromString(axis.LabelColor);
                }
                if (!axis.PreventLabelOffset)
                    chartCtrl.PrimaryAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;
            }
            else
            {
                if (axis.LabelsAutoFitDisabled)
                {
                    if (axis.Angle > 0)
                        chartCtrl.SecondaryAxis.LabelRotationAngle = axis.Angle;// axis.Angle;
                }
                chartCtrl.SecondaryAxis.LabelsIntersectAction = AxisLabelsIntersectAction.MultipleRows;

                if (axis.LabelFont != null)
                {
                    if (!string.IsNullOrEmpty(axis.LabelFont.FontFamily))
                        chartCtrl.SecondaryAxis.FontFamily = new FontFamily(axis.LabelFont.FontFamily);

                    if (axis.LabelFont.FontSize != null)
                        chartCtrl.SecondaryAxis.FontSize = axis.LabelFont.FontSize.PixelValue;
                }
                if (axis.LabelColor != null)
                {
                    chartCtrl.SecondaryAxis.Foreground = brush.ConvertFromString(axis.LabelColor);
                }
                if (!axis.PreventLabelOffset)
                    chartCtrl.SecondaryAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;
            }
        }
    }
}
