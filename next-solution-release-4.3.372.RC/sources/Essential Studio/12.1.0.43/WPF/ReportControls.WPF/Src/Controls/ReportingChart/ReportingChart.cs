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
using Syncfusion.Windows.Chart;
using System.Windows.Media;
using System.Windows;
using Syncfusion.Windows.Shared;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Visibility = System.Windows.Visibility;

namespace Syncfusion.RDL.Controls
{
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    internal class ReportingChartControl : Syncfusion.Windows.Chart.Chart
    {
        bool isPrintMode = false;

        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {
                isPrintMode = value;

                if (isPrintMode)
                {
                    this.Margin = new Thickness(ChartModel.PrintPageInfo.ActualLeft, ChartModel.PrintPageInfo.ActualTop, 0, 0);
                }
                else
                {
                    this.Margin = new Thickness(ChartModel.PageInfo.ActualLeft, ChartModel.PageInfo.ActualTop, 0, 0);
                }
            }
        }

        internal ChartModel ChartModel { get; set; }

        internal ChartItemExpVal ChartItemPro
        {
            get;
            set;
        }

        internal ChartTypes Type { get; set; }


        public ExpressionEngine ExpressionEngine { get; set; }

        public ReportingChartControl(IReportItemModeler pageContent)
        {
#if SILVERLIGHT
			Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, Windows.Controls.Theming.VisualStyle.Default);
#else
            SkinStorage.SetVisualStyle(this, "Default");
#endif
            ChartModel model = pageContent as ChartModel;
            ChartItemPro = model.ChartProperties;
            this.ExpressionEngine = model.Model.ExpressionEngine;
            this.ChartModel = (pageContent as ChartModel);
            ReportItem reportItem = model.ReportItem;
            ReportingBrushConverter brush = new ReportingBrushConverter();
            string controlName = pageContent.Name;
            IntializeChartType(ChartItemPro.Type, ChartItemPro.SubType);

            this.Name = controlName;
            this.Height = pageContent.Height;
            this.Width = pageContent.Width;

            if (this.ChartItemPro.ChartTiles != null && this.ChartItemPro.ChartTiles.Count() > 0)
            {
                var chartHeader = this.ChartItemPro.ChartTiles.First();
                TextBlock header = new TextBlock();
                header.Text = chartHeader.Caption;

                if (chartHeader.Style != null)
                {
                    if (chartHeader.Style.Color != null)
                    {
                        header.Foreground = brush.ConvertFromInvariantString(chartHeader.Style.Color);
                    }
#if !SILVERLIGHT
                    if (chartHeader.Style.FillStyle != null && chartHeader.Style.FillStyle.BackgroundColor != null)
                    {
                        header.Background = brush.ConvertFromInvariantString(chartHeader.Style.FillStyle.BackgroundColor);
                    }
#endif
                    if (chartHeader.Style.Font != null)
                    {
                        header.FontFamily = new FontFamily(chartHeader.Style.Font.FontFamily);
                        header.FontSize = chartHeader.Style.Font.FontSize.PixelValue;

                        if (chartHeader.Style.Font.FontStyle != DOM.FontStyle.Default)
                        {
                            header.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(chartHeader.Style.Font.FontStyle.ToString());
                        }
                        if (chartHeader.Style.Font.FontWeight != DOM.FontWeight.Default)
                        {
                            header.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(chartHeader.Style.Font.FontWeight.ToString());
                        }
                    }
                }

                this.Header = header;
            }

            int CurrentColorIndex = -1;
            foreach (var area in ChartItemPro.ChartAreas)
            {

                Syncfusion.Windows.Chart.ChartArea chartArea = new Syncfusion.Windows.Chart.ChartArea();

                if (area.ChartSeries.First().ChartAreaXAxis.First().Visible == BooleanOptions.False)
                {
                    chartArea.PrimaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
                }

                if (area.ChartSeries.First().ChartAreaYAxis.First().Visible == BooleanOptions.False)
                {
                    chartArea.SecondaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
                }

                if (ChartItemPro.Type == VisualizationType.Shape)
                {
#if SILVERLIGHT
                    ChartStyleModel stylemodel = new ChartStyleModel(chartArea) { Palette = ChartColorPalette.Default };
#else
                    chartArea.ColorModel = new ChartStyleModel() { Palette = ChartColorPalette.Default };
#endif
                }
                chartArea.Padding = new Thickness(1, 1, 2, 1);
                this.Areas.Add(chartArea);
#if !SILVERLIGHT
                if (area.Chart3D != null && area.Chart3D.Enabled)
                {
                    chartArea.View3DMode = true;
                    if (chartArea.Camera3D is System.Windows.Media.Media3D.PerspectiveCamera)
                    {
                        chartArea.CameraController.Length = 1.5;
                    }
                    else if (chartArea.Camera3D is System.Windows.Media.Media3D.OrthographicCamera)
                    {
                        System.Windows.Media.Media3D.OrthographicCamera camera = chartArea.Camera3D as System.Windows.Media.Media3D.OrthographicCamera;
                        camera.Width = 1.5;
                    }
                }
#endif

                try
                {
                    if (area.Style.FillStyle != null)
                        chartArea.Background = (Brush)brush.ConvertFromString(area.Style.FillStyle.BackgroundColor);
                }
                catch (Exception)
                { }

                if (area.Style.Border != null)
                {
                    BorderStyles borderstyle = area.Style.Border.BorderStyle;
                    if (borderstyle != BorderStyles.None)
                    {
                        chartArea.BorderThickness = new Thickness(1);

                        if (area.Style.Border.BorderColor != null && area.Style.Border.BorderWidth.size != null)
                            chartArea.BorderBrush = (Brush)brush.ConvertFromString(area.Style.Border.BorderColor);
                        else
                            chartArea.BorderThickness = new Thickness(0);
                    }
                }
                if (area.Style.FillStyle != null)
                {
                    if (area.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && area.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                        chartArea.Background = GradientColor(area.Style.FillStyle.BackgroundColor, area.Style.FillStyle.BackgroundGradientEndcolor);
                }

                chartArea.PrimaryAxis.ValueType = ChartValueType.String;
                chartArea.Series.Clear();

                //var datas = model.Engine.SeriesDataSources;

                int count = 0;
                foreach (var datas in model.Engine.SeriesDataSources)
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

                    Syncfusion.Windows.Chart.ChartSeries chartSeries = new Syncfusion.Windows.Chart.ChartSeries();

#if !MVC
                    if (model.Engine.DrillActions != null && model.Engine.DrillActions.ContainsKey(count))
                    {
                        chartSeries.MouseEnter += chartSeries_MouseEnter;
                        chartSeries.MouseLeave += chartSeries_MouseLeave;
                        chartSeries.MouseLeftButtonDown += chartSeries_MouseLeftButtonDown;
                    }
#endif
#if SILVERLIGHT
                    if (seriesItem.Hidden)
						{
							if (seriesItem.Hidden)
							{
								chartSeries.Visibility = System.Windows.Visibility.Collapsed;
							}
							else
							{
								chartSeries.Visibility = System.Windows.Visibility.Visible;
							}
						}
#else


                    if (seriesItem.Legend != null)
                    {
                        if (seriesItem.Legend.Hidden)
                        {
                            chartSeries.VisibilityOnLegend = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            chartSeries.VisibilityOnLegend = System.Windows.Visibility.Visible;
                        }
                    }

                    chartSeries.IsVisible = seriesItem.Hidden ? false : true;
#endif
                    if (ChartItemPro.Type == VisualizationType.Shape)
                    {
#if SILVERLIGHT
						ChartStyleModel stylemodel = new ChartStyleModel(chartArea) { Palette = ChartColorPalette.Default };
#else
                        chartArea.ColorModel = new ChartStyleModel() { Palette = ChartColorPalette.Default };
#endif
                    }

#if !SILVERLIGHT
                    if (ChartItemPro.Type != VisualizationType.Line)
                        chartSeries.LegendIcon = ChartLegendIcon.Rectangle;
                    else
                        chartSeries.LegendIcon = ChartLegendIcon.HorizontalLine;
#endif

#if SILVERLIGHT
						if (ChartItemPro.Type == VisualizationType.Polar)
							{
								ChartPolarType.SetDrawType(chartArea, ChartPolarDrawType.Area);
							}
#endif

                    chartSeries.BindingPathX = "X";
#if SILVERLIGHT
						List<string> str = new List<string>();
						str.Add("Y");
						chartSeries.BindingPathsY = str;
#else
                    chartSeries.BindingPathsY = new string[] { "Y" };
#endif
                    chartSeries.DataSource = datas.DataSource;
                    chartArea.Series.Add(chartSeries);

                    try
                    {
                        var pointStyle = seriesItem.DataPointsStyle.FirstOrDefault();

                        if (pointStyle.Style != null)
                        {
                            if (!string.IsNullOrEmpty(pointStyle.Style.Color) && pointStyle.Style.Color != "Black")
                                chartSeries.Interior = (Brush)brush.ConvertFromString(pointStyle.Style.Color);
                            else if (ChartItemPro.CustomPaletteColors.Count > 1)
                            {
                                if (CurrentColorIndex >= ChartItemPro.CustomPaletteColors.Count)
                                {
                                    CurrentColorIndex = -1;
                                }
                                CurrentColorIndex++;
                                chartSeries.Interior = (Brush)brush.ConvertFromString(ChartItemPro.CustomPaletteColors[CurrentColorIndex]);
                            }

                            if (pointStyle.Style.FillStyle != null)
                            {
                                if (pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && pointStyle.Style.FillStyle.BackgroundGradientType != BackgroundGradientTypes.Default)
                                {
                                    chartSeries.Interior = GradientColor(chartSeries.Interior.ToString(), seriesItem.Style.FillStyle.BackgroundGradientEndcolor);
                                }
                            }

                            if (ChartItemPro.Type == VisualizationType.Line)
                            {
                                chartSeries.StrokeThickness = 2;
                                var marker = seriesItem.DataPointsStyle.First().ChartMarker;
                                if (marker != null && marker.MarkerType != null)
                                {
                                    switch (marker.MarkerType.ToString())
                                    {
                                        case "Square":
                                        case "Auto":
                                            chartSeries.AdornmentsInfo.Symbol = Symbol.Square; break;
                                        case "Circle": chartSeries.AdornmentsInfo.Symbol = Symbol.Ellipse; break;
                                        case "Cross": chartSeries.AdornmentsInfo.Symbol = Symbol.Cross; break;
                                        case "Triangle": chartSeries.AdornmentsInfo.Symbol = Symbol.Triangle; break;
                                        case "Diamond": chartSeries.AdornmentsInfo.Symbol = Symbol.Diamond; break;
                                        case "Star4": chartSeries.AdornmentsInfo.Symbol = Symbol.Plus; break;
                                    }
                                    if (marker.Size != null && marker.Size.FloatValue != 0)
                                    {
                                        chartSeries.AdornmentsInfo.SymbolHeight = marker.Size.FloatValue;
                                        chartSeries.AdornmentsInfo.SymbolWidth = marker.Size.FloatValue;
                                    }
                                    else
                                    {
                                        chartSeries.AdornmentsInfo.SymbolHeight = 3.75;
                                        chartSeries.AdornmentsInfo.SymbolWidth = 3.75;
                                    }
                                    if (!string.IsNullOrEmpty(marker.Color))
                                    {
                                        chartSeries.AdornmentsInfo.SymbolStroke = brush.ConvertFromInvariantString(marker.Color);
                                    }
                                    chartSeries.AdornmentsInfo.SymbolStrokeThickness = 0.1;
                                    chartSeries.AdornmentsInfo.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                                    chartSeries.AdornmentsInfo.SymbolInterior = (Brush)brush.ConvertFromString(marker.Color);
                                    chartSeries.AdornmentsInfo.Visible = true;
#if !SILVERLIGHT
                                    chartSeries.AdornmentsInfo.OffsetY = 6;
#endif
                                }
                            }
                            else
                            {
                                chartSeries.StrokeThickness = 0;
                            }
                            if (pointStyle.Style.Border != null)
                            {
                                if (pointStyle.Style.Border.BorderWidth != null)
                                    chartSeries.StrokeThickness = pointStyle.Style.Border.BorderWidth.PixelValue;

                                chartSeries.Stroke = (Brush)brush.ConvertFromString(pointStyle.Style.Border.BorderColor);

                                if (pointStyle.Style.Border.BorderWidth == null && pointStyle.Style.Border.BorderColor != null)
                                {
                                    chartSeries.StrokeThickness = 1;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    { }

                    ApplySeriesLabelStyles(chartSeries, seriesItem.PointValues.FirstOrDefault().ChartDataLabel);// Applying the Chart Data Label Styles

#if SILVERLIGHT
						if (this.ChartItemPro.Type==VisualizationType.Area)
						{
							chartSeries.ShowEmptyPoints = true;
						}
#endif

                    if (seriesItem.EmptyPointsStyle != null)
                    {
                        ApplyChartEmptyPointStyles(chartSeries, seriesItem.EmptyPointsStyle.ChartMarker, model); //Applying the chart empty points styles
                    }

                    chartSeries.Type = this.Type;
                    if (ChartItemPro.Type == VisualizationType.Shape)
                    {
                        if (ChartItemPro.SubType == VisualizationSubType.ExplodedPie)
                        {
                            ChartPieType.SetExplodedAll(chartSeries, true);
                            ChartPieType.SetExplodedIndex(chartSeries, -1);
                            ChartPieType.SetExplodeRadius(chartSeries, 10.0);
                        }
                        else if (ChartItemPro.SubType == VisualizationSubType.ExplodedDoughnut)
                        {
                            ChartDoughnutType.SetExplodedAll(chartSeries, true);
                            ChartDoughnutType.SetDoughnutCoefficient(chartSeries, 0.4);
                            //ChartDoughnutType.SetExplodedIndex(chartSeries, 1);
                            //ChartDoughnutType.SetExplodeRadius(chartSeries, 1.0);
                        }
                        if (ChartItemPro.SubType == VisualizationSubType.Doughnut)
                        {
                            ChartDoughnutType.SetDoughnutCoefficient(chartSeries, 0.4);
                        }
                    }
                    else
                    {
                        if (seriesItem.Legend != null)
                        {
                            chartSeries.Label = seriesItem.Legend.LegendText;
                        }
                        else
                        {
                            chartSeries.Label = datas.Label;
                        }
                    }


                    if (seriesItem.CustomProperties != null)
                    {
                        ApplyCustomProperties(chartSeries, chartArea, seriesItem.CustomProperties, model); // Applying custom properties
                    }


                    foreach (ChartAreaAxisExpVal areaaxies in seriesItem.ChartAreaYAxis)
                    {
                        TextBlock header = new TextBlock();
                        if (areaaxies.ChartAxisTitle != null)
                        {
                            header.Text = areaaxies.ChartAxisTitle.Caption;
                            if (areaaxies.ChartAxisTitle.TextColor != null)
                            {
                                header.Foreground = brush.ConvertFromInvariantString(areaaxies.ChartAxisTitle.TextColor);
                            }

                            if (areaaxies.ChartAxisTitle.Font != null)
                            {
                                header.FontFamily = new FontFamily(areaaxies.ChartAxisTitle.Font.FontFamily);
                                header.FontSize = areaaxies.ChartAxisTitle.Font.FontSize.PixelValue;

                                if (areaaxies.ChartAxisTitle.Font.FontWeight != DOM.FontWeight.Default)
                                {
                                    header.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(areaaxies.ChartAxisTitle.Font.FontWeight.ToString());
                                }
                            }
                        }
                        if (seriesItem.ValueAxisName == areaaxies.Name && seriesItem.ValueAxisName.Equals("Secondary", StringComparison.InvariantCultureIgnoreCase))
                        {
                            chartSeries.YAxis = new Syncfusion.Windows.Chart.ChartAxis();
                            chartSeries.YAxis.OpposedPosition = true;
                            chartSeries.YAxis.Orientation = System.Windows.Controls.Orientation.Vertical;
                            if (areaaxies.ChartAxisTitle != null)
                            {
#if !SILVERLIGHT
                                chartSeries.YAxis.Header = header;
                                if (chartArea.View3DMode)
#endif
                                {
                                    chartSeries.YAxis.Header = areaaxies.ChartAxisTitle.Caption;
                                }
                            }
                            ApplyChartLabelStyles(chartArea, areaaxies, ChartItemPro.Type, false);
                            ApplyIntervalsMargin(chartArea, areaaxies, model, false); // Applying the chart Value Axis Intervals and Margin 
                        }
                        else
                        {
                            if (areaaxies.ChartAxisTitle != null && (!areaaxies.Name.Equals("Secondary", StringComparison.InvariantCultureIgnoreCase)))
                            {
#if !SILVERLIGHT
                                chartSeries.YAxis.Header = header;
                                if (chartArea.View3DMode)
#endif
                                {
                                    chartSeries.YAxis.Header = areaaxies.ChartAxisTitle.Caption;
                                }
                                ApplyChartLabelStyles(chartArea, areaaxies, ChartItemPro.Type, false);
                                ApplyIntervalsMargin(chartArea, areaaxies, model, false); // Applying the chart Value Axis Intervals and Margin 
                            }
                        }
                    }

                    foreach (ChartAreaAxisExpVal areaaxies in seriesItem.ChartAreaXAxis)
                    {
                        TextBlock header = new TextBlock();
                        if (areaaxies.ChartAxisTitle != null)
                        {
                            header.Text = areaaxies.ChartAxisTitle.Caption;
                            if (areaaxies.ChartAxisTitle.TextColor != null)
                            {
                                header.Foreground = brush.ConvertFromInvariantString(areaaxies.ChartAxisTitle.TextColor);
                            }

                            if (areaaxies.ChartAxisTitle.Font != null)
                            {
                                header.FontFamily = new FontFamily(areaaxies.ChartAxisTitle.Font.FontFamily);
                                header.FontSize = areaaxies.ChartAxisTitle.Font.FontSize.PixelValue;

                                if (areaaxies.ChartAxisTitle.Font.FontWeight != DOM.FontWeight.Default)
                                {
                                    header.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(areaaxies.ChartAxisTitle.Font.FontWeight.ToString());
                                }
                            }
                        }
                        if (seriesItem.CategoryAxisName == areaaxies.Name && seriesItem.CategoryAxisName.Equals("Secondary", StringComparison.InvariantCultureIgnoreCase))
                        {
                            chartSeries.XAxis = new Syncfusion.Windows.Chart.ChartAxis();
                            chartSeries.XAxis.OpposedPosition = true;
                            chartSeries.XAxis.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            if (areaaxies.ChartAxisTitle != null)
                            {
#if !SILVERLIGHT
                                chartSeries.XAxis.Header = header;
                                if (chartArea.View3DMode)
#endif
                                {
                                    chartSeries.XAxis.Header = areaaxies.ChartAxisTitle.Caption;
                                }
                            }
                            ApplyChartLabelStyles(chartArea, areaaxies, ChartItemPro.Type, true); // Applying the Label  styles and  property values
                            ApplyIntervalsMargin(chartArea, areaaxies, model, true); // Applying the chart category Axis Intervals and Margin
                        }
                        else
                        {
                            if (areaaxies.ChartAxisTitle != null && (!areaaxies.Name.Equals("Secondary", StringComparison.InvariantCultureIgnoreCase)))
                            {
#if !SILVERLIGHT
                                chartSeries.XAxis.Header = header;
                                if (chartArea.View3DMode)
#endif
                                {
                                    chartSeries.XAxis.Header = areaaxies.ChartAxisTitle.Caption;
                                }
                                ApplyChartLabelStyles(chartArea, areaaxies, ChartItemPro.Type, true); // Applying the Label  styles and  property values
                                ApplyIntervalsMargin(chartArea, areaaxies, model, true); // Applying the chart category Axis Intervals and Marg
                            }
                        }
                    }
#if !SILVERLIGHT
                    chartSeries.AdornmentIntersectAction = AdornmentIntersectActions.AdjustAroundPoints;
                    chartSeries.ShowSmartLabels = true;
#endif
                    count++;
                }

                Syncfusion.Windows.Chart.ChartAxis primaryAxis = chartArea.PrimaryAxis;
                primaryAxis.RangeCalculationMode = RangeCalculationMode.AdjustAcrossChartTypes;
                //primaryAxis.RangePadding = ChartRangePaddingType.Additional;
                //primaryAxis.AdditionalPadding = new DoubleRange(0, 1);

                Syncfusion.Windows.Chart.ChartAxis secondaryAxis = chartArea.SecondaryAxis;
                secondaryAxis.RangeCalculationMode = RangeCalculationMode.ConsistentAcrossChartTypes;
                //secondaryAxis.RangePadding = ChartRangePaddingType.Additional;
                //secondaryAxis.AdditionalPadding = new DoubleRange(0, 1);


#if !SILVERLIGHT
                if (ChartItemPro.Type == VisualizationType.Shape && ChartItemPro.ChartLegends != null && ChartItemPro.ChartLegends.Count > 0)
#else
				if (ChartItemPro.ChartLegends != null && ChartItemPro.ChartLegends.Count > 0)
#endif
                {

                    ChartLegendExpVal legend = ChartItemPro.ChartLegends.FirstOrDefault();
                    Syncfusion.Windows.Chart.ChartLegend chartLegend = new Syncfusion.Windows.Chart.ChartLegend();

                    chartLegend.IsSegmentsLegend = true;
                    chartLegend.Background = new SolidColorBrush(Colors.Transparent);
                    chartLegend.BorderThickness = new Thickness(0);
                    chartLegend.Foreground = new SolidColorBrush(Colors.Black);
                    if (legend.Style != null && !string.IsNullOrEmpty(legend.Style.Font.FontFamily))
                        chartLegend.FontFamily = new System.Windows.Media.FontFamily(legend.Style.Font.FontFamily);
#if !SILVERLIGHT
                    chartLegend.ItemTemplate = this.GetLegendTemplate();
#endif
                    ChartDock position = GetChartLegendPosition(legend.Position);
#if SILVERLIGHT   

					chartArea.Legends = chartLegend;
					chartArea.Legends.Visibility = System.Windows.Visibility.Visible;
					if (ChartItemPro.Type == VisualizationType.Line)
						chartArea.Legends.LegendIcon = ChartLegendIcon.StraightLine;
					else
						chartArea.Legends.LegendIcon = ChartLegendIcon.Rectangle;

					chartArea.Legends.IconVisibility = System.Windows.Visibility.Visible;
					
					chartArea.Legends.DockPosition = position;
#else
                    if (position == ChartDock.Top || position == ChartDock.Bottom)
                        chartLegend.LegendPanel = LegendPanelTypes.WrapPanel;
                    chartArea.Legend = chartLegend;
                    Syncfusion.Windows.Chart.Chart.SetDock(chartArea.Legend, position);
#endif
                }
#if SILVERLIGHT
				primaryAxis.ShowGridLines = false;
				secondaryAxis.GridLineStroke = new SolidColorBrush(Colors.Gray);
                if (area.ChartSeries.First().ChartAreaXAxis.First().Visible != BooleanOptions.False && area.ChartSeries.First().ChartAreaYAxis.First().Visible != BooleanOptions.False)
                {
				secondaryAxis.GridLineStrokeThickness = 0.25;
                }
                else
                {
                secondaryAxis.GridLineStrokeThickness = 0.0;
                }
#else
                Syncfusion.Windows.Chart.ChartArea.SetShowGridLines(primaryAxis, false);

                if (area.ChartSeries.First().ChartAreaXAxis.First().Visible != BooleanOptions.False && area.ChartSeries.First().ChartAreaYAxis.First().Visible != BooleanOptions.False)
                {
                    Syncfusion.Windows.Chart.ChartArea.SetGridLineStroke(secondaryAxis, new Pen(Brushes.Gray, 0.25));
                }
                else
                {
                    Syncfusion.Windows.Chart.ChartArea.SetGridLineStroke(secondaryAxis, new Pen(Brushes.Gray, 0.0));
                }
#endif
            }

#if !SILVERLIGHT
            if (ChartItemPro.ChartLegends != null && ChartItemPro.ChartLegends.Count > 0 && ChartItemPro.Type != VisualizationType.Shape)
            {
                foreach (ChartLegendExpVal legend in ChartItemPro.ChartLegends)
                {
                    Syncfusion.Windows.Chart.ChartLegend chartLegend = new Syncfusion.Windows.Chart.ChartLegend();
                    chartLegend.Background = new SolidColorBrush(Colors.Transparent);
                    chartLegend.BorderThickness = new Thickness(0);
                    chartLegend.Foreground = new SolidColorBrush(Colors.Black);
                    this.Legends.Add(chartLegend);
                    chartLegend.Visibility = legend.Hidden ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                    Syncfusion.Windows.Chart.Chart.SetDock(chartLegend, GetChartLegendPosition(legend.Position));
                }
            }
#endif

            this.Background = new SolidColorBrush(Colors.White);
            this.Foreground = new SolidColorBrush(Colors.Black);

#if !SILVERLIGHT
            if (ChartItemPro.Border != null && ChartItemPro.Border.Default != null)
            {
                try
                {
                    this.BorderThickness = new Thickness(ChartItemPro.Border.Default.Thickness);
                    this.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(ChartItemPro.Border.Default.BorderBrush);
                }
                catch { }
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
#else
            this.BorderThickness = new Thickness(0);
#endif

            if (ChartItemPro.ChartStyle != null)
            {
                if (ChartItemPro.ChartStyle.Border != null)
                {
                    if (!string.IsNullOrEmpty(ChartItemPro.ChartStyle.Border.BorderColor) && ChartItemPro.ChartStyle.Border.BorderWidth.size != null)
                        this.BorderBrush = brush.ConvertFromString(ChartItemPro.ChartStyle.Border.BorderColor);
                    if (ChartItemPro.ChartStyle.FillStyle.BackgroundGradientType != BackgroundGradientTypes.None && !string.IsNullOrEmpty(ChartItemPro.ChartStyle.FillStyle.BackgroundGradientEndcolor))
                        this.Background = GradientColor(ChartItemPro.ChartStyle.FillStyle.BackgroundColor, ChartItemPro.ChartStyle.FillStyle.BackgroundGradientEndcolor);
                    else if (!string.IsNullOrEmpty(ChartItemPro.ChartStyle.FillStyle.BackgroundColor))
                        this.Background = brush.ConvertFromString(ChartItemPro.ChartStyle.FillStyle.BackgroundColor);
                }
            }

            if (pageContent.FlowLayoutInfo != null)
            {
                this.Margin = new Thickness(pageContent.FlowLayoutInfo.ActualLeft, pageContent.FlowLayoutInfo.ActualTop, 0, 0);
            }
            else if (pageContent.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.Left, pageContent.Top, 0, 0);
            }
            this.Visibility = pageContent.Hidden ? Visibility.Collapsed : Visibility.Visible;

        }

#if !SILVERLIGHT
        void chartSeries_MouseLeftButtonDown(object sender, ChartMouseEventArgs e)
        {
            ChartSegment segment = e.Segment;
            if (segment.CorrespondingPoints != null && segment.CorrespondingPoints.Count() > 0)
            {
                ReportingChartData chartData = segment.CorrespondingPoints[0].DataPoint.Item as ReportingChartData;
                if (chartData != null && chartData.ActionInfo != null)
                {
                    var actionInfo = chartData.ActionInfo as TextboxActionInfoExpVal;
                    if (!string.IsNullOrEmpty(actionInfo.BookmarkLink))
                    {
                        System.Diagnostics.Process.Start(actionInfo.BookmarkLink);
                    }
                    else if (!string.IsNullOrEmpty(actionInfo.Hyperlink))
                    {
                        System.Diagnostics.Process.Start(actionInfo.Hyperlink);
                    }
                    else if (!string.IsNullOrEmpty(actionInfo.ReportName))
                    {
                        Drillthrough drillthrough = new Drillthrough()
                        {
                            Parameters = this.GetParameters(actionInfo.Parameters),
                            ReportName = actionInfo.ReportName
                        };
                        this.ChartModel.Model.DrillThroughReport(this.ChartModel.Model, drillthrough);
                    }

                }
            }
        }
#else
        void chartSeries_MouseLeftButtonDown(object sender, ChartMouseEventArgs e)
        {
            try
            {
                if (sender is Syncfusion.Windows.Chart.ChartSeries)
                {
                    Syncfusion.Windows.Chart.ChartSeries serious = sender as Syncfusion.Windows.Chart.ChartSeries;
                    object tag = (from data in serious.Data where data.Values == e.Segment.DataPoint.Values select data.Tag).First();
                    if (tag != null)
                    {
                        ReportingChartData chartData = tag as ReportingChartData;
                        if (chartData != null && chartData.ActionInfo != null)
                        {
                            var actionInfo = chartData.ActionInfo as TextboxActionInfoExpVal;
                            if (!string.IsNullOrEmpty(actionInfo.BookmarkLink))
                            {
                                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(actionInfo.BookmarkLink));
                            }
                            else if (!string.IsNullOrEmpty(actionInfo.Hyperlink))
                            {
                                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(actionInfo.Hyperlink));
                            }
                            else if (!string.IsNullOrEmpty(actionInfo.ReportName))
                            {
                                Drillthrough drillthrough = new Drillthrough()
                                {
                                    Parameters = this.GetParameters(actionInfo.Parameters),
                                    ReportName = actionInfo.ReportName
                                };
                                this.ChartModel.Model.DrillThroughReport(this.ChartModel.Model, drillthrough);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

        }
#endif

        void chartSeries_MouseLeave(object sender, ChartMouseEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        void chartSeries_MouseEnter(object sender, ChartMouseEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Hand;
        }

        DOM.Parameters GetParameters(List<TextboxParameterExpVal> action)
        {
            if (action != null)
            {
                DOM.Parameters parameters = new Parameters();
                foreach (var para in action)
                {
                    DOM.Parameter parameter = new Parameter();
                    parameter.Name = para.Name;
                    parameter.Omit = para.Omit;
                    parameter.Value = para.Value;
                    parameters.Add(parameter);
                }
                return parameters;
            }
            return null;
        }

        #region HeplerMethod

        //Applying Intervals & Margins
        void ApplyIntervalsMargin(Syncfusion.Windows.Chart.ChartArea chartArea, ChartAreaAxisExpVal series, ChartModel model, bool isCategory)
        {
            ReportingBrushConverter brush = new ReportingBrushConverter();
            ChartAreaAxisExpVal areaAxis = null;
            Syncfusion.Windows.Chart.ChartAxis chartAxis = null;
            if (isCategory)
            {
                chartAxis = chartArea.PrimaryAxis;
                areaAxis = series;
            }
            else
            {
                chartAxis = chartArea.SecondaryAxis;
                areaAxis = series;
            }

            if (areaAxis.Interval != null && isCategory)
            {
                chartArea.PrimaryAxis.Interval = double.Parse(areaAxis.Interval);
            }
            else if (areaAxis.Interval != null && (!isCategory))
            {
                chartArea.SecondaryAxis.Interval = double.Parse(areaAxis.Interval);
            }

            if (areaAxis.Margin != null)
            {
                string marginvalue = areaAxis.Margin;
                marginvalue = marginvalue.Equals("Auto", StringComparison.InvariantCultureIgnoreCase) ? "true" : marginvalue;
                bool margin = Convert.ToBoolean(marginvalue);

                if (margin)
                    chartAxis.RangePadding = ChartRangePaddingType.Normal;
                else
                    chartAxis.RangePadding = ChartRangePaddingType.None;
            }
            if (areaAxis.LabelFont != null)
            {
                if (!string.IsNullOrEmpty(areaAxis.LabelFont.FontFamily))
                    chartAxis.LabelFontFamily = new FontFamily(areaAxis.LabelFont.FontFamily);

                if (areaAxis.LabelFont.FontSize != null)
                    chartAxis.LabelFontSize = areaAxis.LabelFont.FontSize.PixelValue;
            }
            if (areaAxis.LabelColor != null)
            {
                chartAxis.LabelForeground = brush.ConvertFromString(areaAxis.LabelColor);
            }
        }


        //Applying Label Style
        void ApplyChartLabelStyles(Syncfusion.Windows.Chart.ChartArea chartArea, ChartAreaAxisExpVal axis, VisualizationType chartType, bool isPrimaryAxis)
        {
            Syncfusion.Windows.Chart.ChartAxis chartAxis = null;
            if (isPrimaryAxis)
                chartAxis = chartArea.PrimaryAxis;
            else
                chartAxis = chartArea.SecondaryAxis;

            if (axis.LabelsAutoFitDisabled)
            {
                if (axis.Angle > 0)
                    chartAxis.LabelRotateAngle = axis.Angle;// axis.Angle;
            }
            else
            {
                if (axis.AllowLabelRotation != AllowLabelRotation.None && axis.Angle != 0.0 && chartType != VisualizationType.Bar)
                    chartAxis.LabelRotateAngle = 270; //(double)axis.RotationAngle;
                else if (axis.Angle == 0.0)//(axis.RotationAngle == 0.0 && isPrimaryAxis == true))
                {
                    if (isPrimaryAxis && chartType != VisualizationType.Bar)
                    {
                        chartAxis.LabelRotateAngle = 270;
                    }
                    else if (!isPrimaryAxis && chartType == VisualizationType.Bar)
                    {
                        chartAxis.LabelRotateAngle = 270;
                    }
                }
                else
                {
                    if (!axis.PreventLabelOffset)
                        chartAxis.EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift;

                    chartAxis.IntersectAction = ChartLabelIntersectAction.MultipleRows;
                }

            }
        }


        //Applying the Chart Data Label Styles
        void ApplySeriesLabelStyles(Syncfusion.Windows.Chart.ChartSeries series, ChartDataLabelExpVal label)
        {
            ReportingBrushConverter brush = new ReportingBrushConverter();

            if (((label != null && !string.IsNullOrEmpty(label.Label)) || label.UseValueAsLabel) && label.Visible)
            {
                if (series.AdornmentsInfo == null)
                    series.AdornmentsInfo = new ChartAdornmentInfo();
                series.AdornmentsInfo.Visible = label.Visible;

                if (label.Format != null)
                {
                    series.AdornmentsInfo.SegmentLabelFormat = label.Format;
                    series.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                }

                if (label.Font != null)
                {
                    try
                    {
                        if (label.Font.FontSize != null)
                        {
                            series.AdornmentsInfo.SegmentLabelFontSize = Convert.ToInt32(label.Font.FontSize.PixelValue);
                        }
                        series.AdornmentsInfo.SegmentLabelFontFamily = new System.Windows.Media.FontFamily(label.Font.FontFamily);
                    }
                    catch { }
                }
            }
            else
            {
                if (series.AdornmentsInfo == null)
                    series.AdornmentsInfo = new ChartAdornmentInfo();
                if (label != null && !string.IsNullOrEmpty(label.Format))
                {
                    series.AdornmentsInfo.SegmentLabelFormat = label.Format;
                    series.AdornmentsInfo.SymbolStrokeThickness = 3d;
                    series.AdornmentsInfo.SegmentLabelContent = LabelContent.YValue;
                }
            }
        }


        //Applying the chart empty points styles
        void ApplyChartEmptyPointStyles(Syncfusion.Windows.Chart.ChartSeries series, ChartMarkerExpVal chartMarker, ChartModel model)
        {
            ReportingBrushConverter brush = new ReportingBrushConverter();
            if (chartMarker != null)
            {
                if (!string.IsNullOrEmpty(chartMarker.Type))
                {
                    series.ShowEmptyPoints = true;
                    series.EmptyPointStyle = EmptyPointStyle.SymbolAndInterior;
                    string eps = chartMarker.Type;
                    series.AdornmentsInfo.Symbol = (Syncfusion.Windows.Chart.Symbol)Enum.Parse(typeof(Syncfusion.Windows.Chart.Symbol), eps, true);
                }

                string epsc = chartMarker.Color;
                series.EmptyPointInterior = brush.ConvertFromString(epsc);
            }
        }



        //Applying the Custom properties
        void ApplyCustomProperties(Syncfusion.Windows.Chart.ChartSeries series, Syncfusion.Windows.Chart.ChartArea chartArea, CustomPropertiesExpVal customProperties, ChartModel model)
        {
            foreach (CustomPropertyExpVal property in customProperties)
            {
                switch (property.Name)
                {
                    case "FunnelLabelStyle":
                        {
                            string FunnelLabelStyle = property.Value;
                            ApplyFunnelLabelStyle(series, FunnelLabelStyle);
                            series.AdornmentsInfo.SegmentIsOut = true;
                            series.AdornmentsInfo.SegmentLabelFontSize = 12;
                            series.AdornmentsInfo.Visible = true;

                            break;
                        }
                    case "Funnel3DDrawingStyle":
                        {
                            string FunnelShape = property.Value;
                            //chartArea
                            break;
                        }
                    case "PieDrawingStyle":
                        {
                            series.EnableEffects = true;
                            break;
                        }
                    case "PieLabelStyle":
                        {
                            series.AdornmentsInfo.SegmentIsOut = true;
                            break;
                        }
                    case "PieLineColor":
                        {
                            CustomPropertyExpVal LabelsRadialLineSize = customProperties.Where(cp => cp.Name.Equals("LabelsRadialLineSize")).FirstOrDefault();
                            CustomPropertyExpVal LabelsHorizontalLineSize = customProperties.Where(cp => cp.Name.Equals("LabelsHorizontalLineSize")).FirstOrDefault();
                            if (LabelsRadialLineSize != null && LabelsHorizontalLineSize != null)
                            {
                                XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                                XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

                                List<XAttribute> radialLine = new List<XAttribute>();
                                radialLine.Add(new XAttribute("Name", "Line1"));
                                radialLine.Add(new XAttribute("X1", "0"));
                                if (LabelsRadialLineSize != null)
                                    radialLine.Add(new XAttribute("X2", LabelsRadialLineSize.Value));
                                else
                                    radialLine.Add(new XAttribute("X2", "0"));
                                radialLine.Add(new XAttribute("Y1", "0"));
                                radialLine.Add(new XAttribute("Y2", "0"));
                                radialLine.Add(new XAttribute("Stroke", property.Value));

                                List<XAttribute> HorizontalLine = new List<XAttribute>();
                                HorizontalLine.Add(new XAttribute("Name", "Line2"));
                                HorizontalLine.Add(new XAttribute("X1", LabelsRadialLineSize.Value));
                                if (LabelsHorizontalLineSize != null)
                                    HorizontalLine.Add(new XAttribute("Y1", LabelsHorizontalLineSize.Value));
                                else
                                    HorizontalLine.Add(new XAttribute("X2", "0"));
                                HorizontalLine.Add(new XAttribute("Y2", "0"));
                                HorizontalLine.Add(new XAttribute("X2", LabelsRadialLineSize.Value));
                                HorizontalLine.Add(new XAttribute("Stroke", property.Value));


                                XElement[] lineEle = new XElement[] { new XElement(ns + "Line", radialLine), new XElement(ns + "Line", HorizontalLine) };
                                //XElement lineEle2 = new XElement(ns + "Line", HorizontalLine);
                                XElement canvas = new XElement(ns + "Grid", lineEle);
                                XElement tem = new XElement(ns + "DataTemplate", canvas);
                                //XElement tem = new XElement(ns + "DataTemplate", new XElement(ns + "Line", radialLine));

#if SILVERLIGHT
								series.AdornmentsInfo.ConnectorTemplate = XamlReader.Load(tem.ToString()) as DataTemplate;
#else
                                StringReader sr = new StringReader(tem.ToString());
                                XmlReader xr = XmlReader.Create(sr);
                                series.AdornmentsInfo.ConnectorTemplate = XamlReader.Load(xr) as DataTemplate;
#endif
                                series.AdornmentsInfo.SegmentShowLine = true;
                            }
                            break;
                        }
                }
            }

        }


        DataTemplate GetLegendTemplate()
        {

            string template = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><StackPanel Orientation=\"Horizontal\"><Rectangle Fill=\"{Binding Interior}\" Height=\"10\" Width=\"15\"/>"
                + "<TextBlock Margin=\"2,0,0,0\" VerticalAlignment=\"Center\" Height=\"15\" TextWrapping=\"Wrap\" MaxHeight=\"15\" LineHeight=\"15\" Text=\"{Binding Path=CorrespondingPoints[0].DataPoint.Tag.X}\"/></StackPanel></DataTemplate>";

#if SILVERLIGHT
			return XamlReader.Load(template.ToString()) as DataTemplate;
#else
            StringReader sr = new StringReader(template.ToString());
            XmlReader xr = XmlReader.Create(sr);
            return (XamlReader.Load(xr) as DataTemplate);
#endif
        }

        void ApplyFunnelLabelStyle(Syncfusion.Windows.Chart.ChartSeries series, string style)
        {
            switch (style)
            {
                case "OutsideInColumn":
                    {
                        series.AdornmentsInfo.SegmentShowLine = true;
                        series.AdornmentsInfo.SegmentIsOut = true;
                        break;
                    }
                case "Outside":
                    {
                        series.AdornmentsInfo.SegmentIsOut = true;
                        break;
                    }
                case "Inside":
                    {
                        series.AdornmentsInfo.SegmentIsOut = true;
                        break;
                    }
                case "Disabled":
                    {
                        series.AdornmentsInfo.Visible = false;
                        break;
                    }
            }
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

#if !SILVERLIGHT
        internal List<ChartModel.SegmentActionInfo> GetChartSegments()
        {
            try
            {
                if (this.ChartModel.Engine.DrillActions != null)
                {
                    List<ChartModel.SegmentActionInfo> segments = new List<ChartModel.SegmentActionInfo>();
                    foreach (var area in this.Areas)
                    {
                        var seriousContainer = FindChild<ItemsControl>(area, "PART_SeriesContainer");
                        if (seriousContainer != null)
                        {
                            foreach (var item in seriousContainer.Items)
                            {
                                ContentPresenter contentPre = (ContentPresenter)seriousContainer.ItemContainerGenerator.ContainerFromItem(item);
                                ChartSeriesPresenter seriousPre = VisualTreeHelper.GetChild(contentPre, 0) as ChartSeriesPresenter;
                                for (int pos = 0; pos < VisualTreeHelper.GetChildrenCount(seriousPre); pos++)
                                {
                                    ChartModel.SegmentActionInfo segAct = new ChartModel.SegmentActionInfo();
                                    ChartSeriesPresenter.ChartSegmentPresenter child = VisualTreeHelper.GetChild(seriousPre, pos) as ChartSeriesPresenter.ChartSegmentPresenter;
                                    if (this.Type == ChartTypes.Column || this.Type == ChartTypes.StackingBar || this.Type == ChartTypes.StackingBar100 || this.Type == ChartTypes.Bar || this.Type == ChartTypes.StackingColumn || this.Type == ChartTypes.StackingColumn100)
                                    {
                                        Grid grid = VisualTreeHelper.GetChild(child, 0) as Grid;
                                        System.Windows.Shapes.Rectangle rect = grid.Children.OfType<System.Windows.Shapes.Rectangle>().First();
                                        GeneralTransform transform = rect.TransformToVisual(this);
                                        Rect bounds = transform.TransformBounds(new Rect(0, 0, rect.ActualWidth, rect.ActualHeight));
                                        segAct.RectPonits = new ChartModel.RectPonit() { X = bounds.X, Y = bounds.Y, Width = bounds.Width + bounds.X, Height = bounds.Height + bounds.Y };
                                        segAct.ActionInfo = (child.Segment.CorrespondingPoints[0].DataPoint.Item as ReportingChartData).ActionInfo as TextboxActionInfoExpVal;
                                    }
                                    else if (this.Type == ChartTypes.Area || this.Type == ChartTypes.StackingArea || this.Type == ChartTypes.SplineArea || this.Type == ChartTypes.StackingArea100 || this.Type == ChartTypes.Pie || this.Type == ChartTypes.Polar || this.Type == ChartTypes.Doughnut || this.Type == ChartTypes.Funnel || this.Type == ChartTypes.Pyramid)
                                    {
                                        System.Windows.Shapes.Path path = VisualTreeHelper.GetChild(child, 0) as System.Windows.Shapes.Path;
                                        var points = path.TranslatePoint(new Point(0, 0), this);
                                        var segmentPaths = this.GetSegementPaths(path, points.X, points.Y);
                                        segAct.PathPoints = segmentPaths;
                                        segAct.ActionInfo = (child.Segment.CorrespondingPoints[0].DataPoint.Item as ReportingChartData).ActionInfo as TextboxActionInfoExpVal;
                                    }
                                    else if (this.Type == ChartTypes.Line || this.Type == ChartTypes.StepLine || this.Type == ChartTypes.Spline)
                                    {
                                        Grid grid = VisualTreeHelper.GetChild(child, 0) as Grid;
                                        System.Windows.Shapes.Line line = grid.Children.OfType<System.Windows.Shapes.Line>().First();
                                        var points = line.TranslatePoint(new Point(0, 0), this);
                                        segAct.LinePonits = new ItemModel.ChartModel.LinePoint() { X1 = line.X1 + points.X, X2 = line.X2 + points.X, Y1 = line.Y1 + points.Y, Y2 = line.Y2 + points.Y };
                                        segAct.ActionInfo = (child.Segment.CorrespondingPoints[0].DataPoint.Item as ReportingChartData).ActionInfo as TextboxActionInfoExpVal;
                                    }
                                    else if (this.Type == ChartTypes.Scatter)
                                    {
                                        Canvas canvas = VisualTreeHelper.GetChild(child, 0) as Canvas;
                                        System.Windows.Shapes.Ellipse ellipse = canvas.Children.OfType<System.Windows.Shapes.Ellipse>().First();
                                        var points = ellipse.TranslatePoint(new Point(0, 0), this);
                                        segAct.CirclePonits = new ItemModel.ChartModel.CirclePonit() { X = points.X, Y = points.Y, Radius = ellipse.Width / 2 };
                                        segAct.ActionInfo = (child.Segment.CorrespondingPoints[0].DataPoint.Item as ReportingChartData).ActionInfo as TextboxActionInfoExpVal;
                                    }
                                    segments.Add(segAct);
                                }
                            }
                        }
                    }
                    return segments;
                }
            }
            catch{
            }
            return null;
        }

        private List<List<PointXY>> GetSegementPaths(System.Windows.Shapes.Path path,double relativeX, double relativeY)
        {
            PathGeometry pathGeometry = path.Data.GetFlattenedPathGeometry();

            List<List<PointXY>> points = new List<List<PointXY>>();
            foreach (var figure in pathGeometry.Figures)
            {
                List<PointXY> pointXys = new List<PointXY>();
                pointXys.Add(new PointXY() { X = figure.StartPoint.X + relativeX, Y = figure.StartPoint.Y + relativeY });
                foreach (var segment in figure.Segments.OfType<PolyLineSegment>())
                {
                    foreach (var pt in segment.Points)
                    {
                        PointXY point = new PointXY();
                        point.X = pt.X + relativeX;
                        point.Y = pt.Y + relativeY;
                        pointXys.Add(point);
                    }
                }
                points.Add(pointXys);
            }

            return points;
        }

        public T FindChild<T>(DependencyObject depObj, string childName) where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return null;

            // success case
            if (depObj is T && ((FrameworkElement)depObj).Name == childName)
                return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                T obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }
#endif

        void IntializeChartType(VisualizationType chartType, VisualizationSubType subType)
        {
            switch ((VisualizationType)chartType)
            {
                case VisualizationType.Shape:
                    switch (subType)
                    {
                        case VisualizationSubType.Plain:
                            this.Type = ChartTypes.Pie;
                            break;
                        case VisualizationSubType.Funnel:
                            this.Type = ChartTypes.Funnel;
                            break;
                        case VisualizationSubType.Pyramid:
                            this.Type = ChartTypes.Pyramid;
                            break;
                        case VisualizationSubType.Doughnut:
                            this.Type = ChartTypes.Doughnut;
                            break;
                        case VisualizationSubType.ExplodedDoughnut:
                            this.Type = ChartTypes.Doughnut;
                            break;
                        default:
                            this.Type = ChartTypes.Pie;
                            break;
                    }
                    break;
                case VisualizationType.Bar:
                    switch (subType)
                    {
                        case VisualizationSubType.Stacked:
                            this.Type = ChartTypes.StackingBar;
                            break;
                        case VisualizationSubType.PercentStacked:
                            this.Type = ChartTypes.StackingBar100;
                            break;
                        default:
                            this.Type = ChartTypes.Bar;
                            break;
                    }
                    break;
                case VisualizationType.Line:
                    switch (subType)
                    {
                        case VisualizationSubType.Stepped:
                            this.Type = ChartTypes.StepLine;
                            break;
                        case VisualizationSubType.Smooth:
                            this.Type = ChartTypes.Spline;
                            break;
                        default:
                            this.Type = ChartTypes.Line;
                            break;
                    }
                    break;
                case VisualizationType.Polar:
                    switch (subType)
                    {
                        case VisualizationSubType.Radar:
                            this.Type = ChartTypes.Polar;
                            break;
                        default:
                            this.Type = ChartTypes.Radar;
                            break;
                    }
                    break;
                case VisualizationType.Range:
                    switch (subType)
                    {
                        case VisualizationSubType.Plain:
                            this.Type = ChartTypes.RangeArea;
                            break;
                        case VisualizationSubType.Candlestick:
                            this.Type = ChartTypes.Candle;
                            break;
                        case VisualizationSubType.Stock:
                            this.Type = ChartTypes.HiLoOpenClose;
                            break;
                        case VisualizationSubType.BoxPlot:
                            this.Type = ChartTypes.BoxAndWhisker;
                            break;
                        default:
                            this.Type = ChartTypes.RangeArea;
                            break;
                    }
                    break;

                case VisualizationType.Area:
                    switch (subType)
                    {
                        case VisualizationSubType.Stacked:
                            this.Type = ChartTypes.StackingArea;
                            break;
                        case VisualizationSubType.Smooth:
                            this.Type = ChartTypes.SplineArea;
                            break;
                        case VisualizationSubType.PercentStacked:
                            this.Type = ChartTypes.StackingArea100;
                            break;
                        default:
                            this.Type = ChartTypes.Area;
                            break;
                    }
                    break;

                case VisualizationType.Scatter:
                    switch (subType)
                    {
                        case VisualizationSubType.Bubble:
                            this.Type = ChartTypes.Bubble;
                            break;
                        default:
                            this.Type = ChartTypes.Scatter;
                            break;
                    }
                    break;
                default:
                    switch (subType)
                    {
                        case VisualizationSubType.Stacked:
                            this.Type = ChartTypes.StackingColumn;
                            break;
                        case VisualizationSubType.PercentStacked:
                            this.Type = ChartTypes.StackingColumn100;
                            break;
                        default:
                            this.Type = ChartTypes.Column;
                            break;
                    }
                    break;
            }
        }
        #endregion
    }
}