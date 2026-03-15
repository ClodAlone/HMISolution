//-------------------------------------------------------------------------------------------------
// <copyright file="PageModelTextbox.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.DOM;

#if !SILVERLIGHT
using System.Data;
using System.Threading;
using Syncfusion.RDL.Controls;
#endif


#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class ChartModel
        : ReportItemModeler
    {
        #region members

        ChartItemExp chartExpProp;

        #endregion

        #region  properties

        public ChartItemExpVal ChartProperties
        {
            get;
            set;
        }

        internal int PrintPageColumnCount
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get;
            set;
        }

        internal List<double> PageWidths
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PrintPageSizes
        {
            get;
            set;
        }

        internal List<double> PrintPageWidths
        {
            get;
            set;
        }

        public ChartEngine Engine
        {
            get;
            set;
        }

        internal List<string> DataFields
        {
            get;
            set;
        }

        internal List<string> SeriesFields
        {
            get;
            set;
        }

        internal List<string> CategoryFiels
        {
            get;
            set;
        }

        internal Dictionary<int,Syncfusion.RDL.DOM.Action> ActionInfo
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        internal ChartModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.ModelType = ModelType.ChartModel;
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.IsTablixChild = isTablixChild;

            this.Name = reportItem.Name;

            if (reportItem.Top != null)
            {
                this.Top = reportItem.Top.PixelValue;
            }

            if (reportItem.Left != null)
            {
                this.Left = reportItem.Left.PixelValue;
            }

            if (reportItem.Width != null)
            {
                this.Width = reportItem.Width.PixelValue;
            }

            if (reportItem.Height != null)
            {
                this.Height = reportItem.Height.PixelValue;
            }

            this.DataFields = new List<string>();
            this.SeriesFields = new List<string>();
            this.CategoryFiels = new List<string>();

            Chart chart = (reportItem as Chart);
            this.DataSetName = chart.DataSetName;

            if (chart.PageBreak != null)
            {
                this.PageBreak = chart.PageBreak.BreakLocation;
            }

            if (this.DataSetName == null)
            {
                this.DataSetName = dataSetName;
            }

            this.ParseChart();
        }

        public ChartModel()
        {
            // TODO: Complete member initialization
        }

        #endregion

        #region Parsing ReportItem

        void ParseChart()
        {
            try
            {
                Chart chart = this.ReportItem as Chart;
                this.chartExpProp = new ChartItemExp();

                var dataRegion = this.ReportItem as DataRegion;
                if (dataRegion != null)
                {
                    this.ExpFilters = this.Model.ParseFilters(dataRegion.Filters, this.DataSetName, true);
                }

                chartExpProp.ToolTip = this.Model.ExpressionEngine.GetExpressionKey(chart.ToolTip);
                this.AddDataFields(chart.ToolTip);
                chartExpProp.DocumentMapLable = this.Model.ExpressionEngine.GetExpressionKey(chart.DocumentMapLabel);
                this.DocumentMapLable = chartExpProp.DocumentMapLable;
                this.AddDataFields(chart.DocumentMapLabel);
                if (chart.Visibility != null)
                {
                    chartExpProp.Visibility = this.Model.ExpressionEngine.GetExpressionKey(chart.Visibility.Hidden);
                    this.AddDataFields(chart.Visibility.Hidden);
                    this.ToggleItem = chart.Visibility.ToggleItem;
                    this.AddDataFields(chart.Visibility.ToggleItem);
                }
                if (chart.Style != null)
                {
                    this.chartExpProp.Border = new BorderExp();
                    if (chart.Style.Border != null)
                    {
                        this.chartExpProp.Border.Default = new BorderExpProperties();
                        this.chartExpProp.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.Border.Color);
                        this.AddDataFields(chart.Style.Border.Color);
                        this.chartExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.Border.Style);
                        this.AddDataFields(chart.Style.Border.Style);
                        if (chart.Style.Border.Width != null)
                        {
                            this.chartExpProp.Border.Default.Thickness = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.Border.Width.size);
                            this.AddDataFields(chart.Style.Border.Width.size);
                        }
                    }
                    if (chart.Style.LeftBorder != null)
                    {
                        this.chartExpProp.Border.LeftBorder = new BorderExpProperties();
                        this.chartExpProp.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.LeftBorder.Color);
                        this.AddDataFields(chart.Style.LeftBorder.Color);
                        this.chartExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.LeftBorder.Style);
                        this.AddDataFields(chart.Style.LeftBorder.Style);
                        if (chart.Style.LeftBorder.Width != null)
                        {
                            this.chartExpProp.Border.LeftBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.LeftBorder.Width.size);
                            this.AddDataFields(chart.Style.LeftBorder.Width.size);
                        }
                    }
                    if (chart.Style.TopBorder != null)
                    {
                        this.chartExpProp.Border.TopBorder = new BorderExpProperties();
                        this.chartExpProp.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.TopBorder.Color);
                        this.AddDataFields(chart.Style.TopBorder.Color);
                        this.chartExpProp.Border.TopBorder.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.TopBorder.Style);
                        this.AddDataFields(chart.Style.TopBorder.Style);
                        if (chart.Style.TopBorder.Width != null)
                        {
                            this.chartExpProp.Border.TopBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.TopBorder.Width.size);
                            this.AddDataFields(chart.Style.TopBorder.Width.size);
                        }
                    }
                    if (chart.Style.RightBorder != null)
                    {
                        this.chartExpProp.Border.RightBorder = new BorderExpProperties();
                        this.chartExpProp.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.RightBorder.Color);
                        this.AddDataFields(chart.Style.RightBorder.Color);
                        this.chartExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.RightBorder.Style);
                        this.AddDataFields(chart.Style.RightBorder.Style);
                        if (chart.Style.RightBorder.Width != null)
                        {
                            this.chartExpProp.Border.RightBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.RightBorder.Width.size);
                            this.AddDataFields(chart.Style.RightBorder.Width.size);
                        }
                    }
                    if (chart.Style.BottomBorder != null)
                    {
                        this.chartExpProp.Border.BottomBorder = new BorderExpProperties();
                        this.chartExpProp.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.BottomBorder.Color);
                        this.AddDataFields(chart.Style.BottomBorder.Color);
                        this.chartExpProp.Border.BottomBorder.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.BottomBorder.Style);
                        this.AddDataFields(chart.Style.BottomBorder.Style);
                        if (chart.Style.BottomBorder.Width != null)
                        {
                            this.chartExpProp.Border.BottomBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(chart.Style.BottomBorder.Width.size);
                            this.AddDataFields(chart.Style.BottomBorder.Width.size);
                        }
                    }
                }
                chartExpProp.ColorPalette = this.Model.ExpressionEngine.GetExpressionKey(chart.Palette);
                this.AddDataFields(chart.Palette);

                if (chart.ChartCustomPaletteColors != null)
                {
                    chartExpProp.CustomPaletteColors = new List<string>();
                    foreach (var color in chart.ChartCustomPaletteColors)
                    {
                        chartExpProp.CustomPaletteColors.Add(this.Model.ExpressionEngine.GetExpressionKey(color.Color));
                        this.AddDataFields(color.Color);
                    }
                }

                this.chartExpProp.Type = chart.ChartData.ChartSeriesCollection.First().Type.ToString();

                this.chartExpProp.SubType = chart.ChartData.ChartSeriesCollection.First().Subtype.ToString();

                if (chart.ChartTitles != null)
                {
                    chartExpProp.ChartTiles = new List<ChartTileExp>();
                    foreach (DOM.ChartTitle title in chart.ChartTitles)
                    {
                        chartExpProp.ChartTiles.Add(GetChartTile(title));
                    }
                }

                if (chart.ChartBorderSkin != null)
                {
                    chartExpProp.ChartBorderSkin = new ChartBorderSkinExp();
                    chartExpProp.ChartBorderSkin.ChartBorderSkinType = chart.ChartBorderSkin.ToString();
                    if (chart.ChartBorderSkin.Style != null)
                    {
                        chartExpProp.ChartBorderSkin.Style = new ChartStyleExp();
                        if (chart.ChartBorderSkin.Style.BackgroundImage != null)
                        {
                            chartExpProp.ChartBorderSkin.Style.BackgroundImage = this.GetImage(chart.ChartBorderSkin.Style);
                        }
                        if (chart.ChartBorderSkin.Style.Border != null)
                        {
                            chartExpProp.ChartBorderSkin.Style.Border = this.GetBorderStyleExp(chart.ChartBorderSkin.Style.Border);
                        }
                        chartExpProp.ChartBorderSkin.Style.BorderSyle = this.GetEdgesBorder(chart.ChartBorderSkin.Style);
                        chartExpProp.ChartBorderSkin.Style.FillStyle = this.GetFillStyle(chart.ChartBorderSkin.Style);
                    }
                }

                if (chart.ChartNoDataMessage != null)
                {
                    chartExpProp.ChartNoDataMessage = this.GetChartTile(chart.ChartNoDataMessage);
                }

                chartExpProp.DataSetName = chart.DataSetName;

                chartExpProp.ChartLegends = new List<ChartLegendExp>();
                if (chart.ChartLegends != null)
                {
                    foreach (DOM.ChartLegend legend in chart.ChartLegends)
                    {
                        chartExpProp.ChartLegends.Add(GetChartLegend(legend));
                    }
                }

                if (chart.ChartAreas != null)
                {
                    chartExpProp.ChartAreas = new List<ChartAreasExp>();
                    foreach (DOM.ChartArea chartArea in chart.ChartAreas)
                    {
                        chartExpProp.ChartAreas.Add(this.GetChartArea(chartArea, chart));
                    }
                }

                if (chart.ChartSeriesHierarchy.ChartMembers != null)
                {
                    chartExpProp.ChartSeriesHierarchy = new ChartMembersExp();
                    foreach (DOM.ChartMember chartMember in chart.ChartSeriesHierarchy.ChartMembers)
                    {
                        chartExpProp.ChartSeriesHierarchy.Add(this.GetChartMember(chartMember, true));
                    }
                }

                if (chart.ChartCategoryHierarchy.ChartMembers != null)
                {
                    chartExpProp.ChartCategoryHierarchy = new ChartMembersExp();
                    foreach (DOM.ChartMember chartMember in chart.ChartCategoryHierarchy.ChartMembers)
                    {
                        chartExpProp.ChartCategoryHierarchy.Add(this.GetChartMember(chartMember, false));
                    }
                }

                if (chart.Style != null)
                {
                    chartExpProp.ChartStyle = this.GetStyleExp(chart.Style);
                }
            }
            catch
            {

            }
        }

        ChartMemberExp GetChartMember(DOM.ChartMember chartMember, bool isCheck)
        {
            ChartMemberExp chartMem = new ChartMemberExp();
            chartMem.DataElementName = chartMember.DataElementName;
            if (chartMember.CustomProperties != null)
            {
                chartMem.CustomProperties = new CustomPropertiesExp();
                foreach (DOM.CustomProperty property in chartMember.CustomProperties)
                {
                    CustomPropertyExp pro = new CustomPropertyExp();
                    pro.Name = this.Model.ExpressionEngine.GetExpressionKey(property.Name);
                    this.AddDataFields(property.Name);
                    pro.Value = property.Value;
                    chartMem.CustomProperties.Add(pro);
                }
            }
            chartMem.DataElementOutput = chartMember.DataElementOutput.ToString();
            // chartMem.Label = this.Model.ExpressionEngine.GetExpressionKey(chartMember.Label);
            chartMem.Label = chartMember.Label;
            if (chartMember.Group != null)
            {
                chartMem.Group = new GroupExp();
                if (chartMember.Label != null)
                {
                    if (isCheck)
                    {
                        this.SeriesFields.Add(chartMember.Label);
                    }
                    else
                    {
                        this.CategoryFiels.Add(chartMember.Label);
                    }
                }

                if (chartMember.Group.PageBreak != null)
                {
                    chartMem.Group.BreakLocation = chartMember.Group.PageBreak.BreakLocation.ToString();
                }
                chartMem.Group.DataElementName = chartMember.Group.DataElementName;
                chartMem.Group.DocumentMapLabel = this.Model.ExpressionEngine.GetExpressionKey(chartMember.Group.DocumentMapLabel);
                this.AddDataFields(chartMember.Group.DocumentMapLabel);
                chartMem.Group.DomainScope = chartMember.Group.DomainScope;
                chartMem.Group.Name = chartMember.Group.Name;
                chartMem.Group.Parent = this.Model.ExpressionEngine.GetExpressionKey(chartMember.Group.Parent);
                this.AddDataFields(chartMember.Group.Parent);
                if (chartMember.Group.GroupExpressions != null)
                {
                    chartMem.Group.GroupExpressions = new List<string>();
                    foreach (DOM.GroupExpression grpExp in chartMember.Group.GroupExpressions)
                    {
                        //    chartMem.Group.GroupExpressions.Add(this.Model.ExpressionEngine.GetExpressionKey(grpExp.Value));
                        chartMem.Group.GroupExpressions.Add(grpExp.Value);
                    }
                }
                if (chartMember.Group.Filters != null)
                {
                    chartMem.Group.Filters = new FiltersExp();
                    foreach (DOM.Filter filter in chartMember.Group.Filters)
                    {
                        FilterExp fl = new FilterExp();
                        fl.FilterExpression = this.Model.ExpressionEngine.GetExpressionKey(filter.FilterExpression);
                        this.AddDataFields(filter.FilterExpression);
                        if (filter.FilterValues != null)
                        {
                            fl.FilterValues = new FilterValuesExp();
                            foreach (DOM.FilterValue flValue in filter.FilterValues)
                            {
                                FilterValueExp flv = new FilterValueExp();
                                flv.DataType = flValue.DataType.ToString();
                                // flv.Value = this.Model.ExpressionEngine.GetExpressionKey(flValue.Value);
                                flv.Value = flValue.Value;
                                fl.FilterValues.Add(flv);
                            }
                        }
                        fl.Operator = filter.Operator.ToString();
                        chartMem.Group.Filters.Add(fl);
                    }
                }
            }
            chartMem.SortExpressions = new SortExpressionsExp();
            if (chartMember.SortExpressions != null)
            {
                foreach (DOM.SortExpression sortExp in chartMember.SortExpressions)
                {
                    SortExpressionExp exp = new SortExpressionExp();
                    exp.Direction = sortExp.Direction.ToString();
                    //exp.Value = this.Model.ExpressionEngine.GetExpressionKey(sortExp.Value);
                    exp.Value = sortExp.Value;
                    chartMem.SortExpressions.Add(exp);
                }
            }
            chartMem.ChartMembers = new ChartMembersExp();
            if (chartMember.ChartMembers != null)
            {
                foreach (DOM.ChartMember chartMemb in chartMember.ChartMembers)
                {
                    chartExpProp.ChartSeriesHierarchy.Add(this.GetChartMember(chartMemb, isCheck));
                }
            }

            return chartMem;
        }

        ChartAreasExp GetChartArea(DOM.ChartArea chartArea, Chart chart)
        {
            ChartAreasExp chartAreaExp = new ChartAreasExp();
            chartAreaExp.Name = chartArea.Name;
            if (chartArea.Style != null)
            {
                chartAreaExp.Style = this.GetStyleExp(chartArea.Style);
            }
            chartAreaExp.Hidden = this.Model.ExpressionEngine.GetExpressionKey(chartArea.Hidden.ToString());
            this.AddDataFields(chartArea.Hidden.ToString());
            chartAreaExp.EquallySizedAxesFont = this.Model.ExpressionEngine.GetExpressionKey(chartArea.EquallySizedAxesFont.ToString());
            this.AddDataFields(chartArea.EquallySizedAxesFont.ToString());

            if (chartArea.ChartThreeDProperties != null)
            {
                chartAreaExp.Chart3D = new ChartThreeDPropertiesExp();
                chartAreaExp.Chart3D.Clustered = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Clustered.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Clustered.ToString());
                chartAreaExp.Chart3D.DepthRatio = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.DepthRatio.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.DepthRatio.ToString());
                chartAreaExp.Chart3D.Enabled = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Enabled.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Enabled.ToString());
                chartAreaExp.Chart3D.GapDepth = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.GapDepth.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.GapDepth.ToString());
                chartAreaExp.Chart3D.Inclination = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Inclination.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Inclination.ToString());
                chartAreaExp.Chart3D.Perspective = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Perspective.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Perspective.ToString());
                chartAreaExp.Chart3D.ProjectionMode = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.ProjectionMode.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.ProjectionMode.ToString());
                chartAreaExp.Chart3D.Rotation = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Rotation.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Rotation.ToString());
                chartAreaExp.Chart3D.Shading = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.Shading.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.Shading.ToString());
                chartAreaExp.Chart3D.WallThickness = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartThreeDProperties.WallThickness.ToString());
                this.AddDataFields(chartArea.ChartThreeDProperties.WallThickness.ToString());
            }

            chartAreaExp.AlignOrientation = this.Model.ExpressionEngine.GetExpressionKey(chartArea.AlignOrientation.ToString());
            this.AddDataFields(chartArea.AlignOrientation.ToString());

            chartAreaExp.AlignWithChartArea = this.Model.ExpressionEngine.GetExpressionKey(chartArea.AlignWithChartArea);
            this.AddDataFields(chartArea.AlignWithChartArea);

            chartAreaExp.AlignOrientation = this.Model.ExpressionEngine.GetExpressionKey(chartArea.AlignOrientation.ToString());
            this.AddDataFields(chartArea.AlignOrientation.ToString());

            if (chartArea.ChartAlignType != null)
            {
                chartAreaExp.ChartAlignType = new ChartAlignTypeExp();
                chartAreaExp.ChartAlignType.AxesView = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartAlignType.AxesView.ToString());
                this.AddDataFields(chartArea.ChartAlignType.AxesView.ToString());
                chartAreaExp.ChartAlignType.Cursor = this.Model.ExpressionEngine.GetExpressionKey(chartArea.ChartAlignType.Cursor.ToString());
                this.AddDataFields(chartArea.ChartAlignType.Cursor.ToString());
                chartAreaExp.ChartAlignType.InnerPlotPosition = chartArea.ChartAlignType.InnerPlotPosition;
                chartAreaExp.ChartAlignType.Position = chartArea.ChartAlignType.Position;
            }

            chartAreaExp.ChartSeries = new List<ChartSeriesExp>();

            var collectionData = from series in chart.ChartData.ChartSeriesCollection where (series.ChartAreaName != null ? series.ChartAreaName == chartArea.Name : chartArea.Name == chart.ChartAreas[0].Name) select series;

            int pos = 0;
            foreach (var item in collectionData)
            {
                ChartSeriesExp series = new ChartSeriesExp();
                series.CategoryAxisName = item.CategoryAxisName;
                series.ValueAxisName = item.ValueAxisName;
                series.Name = item.Name;
                if (item.Style != null)
                {
                    series.Style = this.GetStyleExp(item.Style);
                }
                series.LegendName = item.LegendName;
                series.PointValues = new List<PointValuesExp>();
                series.DataPointsStyle = new List<PointStylesExp>();

                if (item.ChartDataPoints != null && item.ChartDataPoints.Count > 0)
                {
                    var actionInfo = item.ChartDataPoints.First().ActionInfo;
                    if(actionInfo != null && actionInfo.Actions != null && actionInfo.Actions.Count > 0)
                    {
                        if (this.ActionInfo == null)
                        {
                            this.ActionInfo = new Dictionary<int, DOM.Action>();
                        }
                        this.ActionInfo.Add(pos, actionInfo.Actions.First());
                    }
                }
                pos++;
                foreach (var point in item.ChartDataPoints)
                {
                    PointValuesExp pointValues = new PointValuesExp();
                    pointValues.End = point.ChartDataPointValues.End;
                    pointValues.High = point.ChartDataPointValues.High;
                    pointValues.Mean = point.ChartDataPointValues.Mean;
                    pointValues.Median = point.ChartDataPointValues.Median;
                    pointValues.Low = point.ChartDataPointValues.Low;
                    pointValues.Size = point.ChartDataPointValues.Size;
                    pointValues.Start = point.ChartDataPointValues.Start;
                    //get field name
                    //      pointValues.X = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataPointValues.X);
                    pointValues.X = point.ChartDataPointValues.X;
                    this.AddDataFields(point.ChartDataPointValues.X);
                    //           pointValues.Y = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataPointValues.Y);
                    pointValues.Y = point.ChartDataPointValues.Y;
                    if (point.ChartDataPointValues.Y != null)
                    {
                        this.DataFields.Add(point.ChartDataPointValues.Y);
                    }
                    this.AddDataFields(point.ChartDataPointValues.Y);
                    if (point.ChartItemInLegend != null)
                    {
                        pointValues.LegendHidden = point.ChartItemInLegend.Hidden;
                        pointValues.LegendText = point.ChartItemInLegend.LegendText;
                    }
                    if (point.ChartDataLabel != null)
                    {
                        pointValues.ChartDataLabel = new ChartDataLabelExp();
                        //pointValues.ChartDataLabel.ActionInfo=this.Model.ExpressionEngine.GetExpressionKey();
                        if (point.ChartDataLabel.Style != null)
                        {
                            if (point.ChartDataLabel.Style.Border != null)
                            {
                                pointValues.ChartDataLabel.BorderColor = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Style.Border.Color);
                                this.AddDataFields(point.ChartDataLabel.Style.Border.Color);
                                pointValues.ChartDataLabel.BorderStyle = this.GetBorderStyleExp(point.ChartDataLabel.Style.Border);
                            }
                            pointValues.ChartDataLabel.Font = this.GetFontExp(point.ChartDataLabel.Style);
                            pointValues.ChartDataLabel.Format = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Style.Format);
                            this.AddDataFields(point.ChartDataLabel.Style.Format);
                            pointValues.ChartDataLabel.TextColor = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Style.Color);
                            this.AddDataFields(point.ChartDataLabel.Style.Color);
                            pointValues.ChartDataLabel.TextDecoration = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Style.TextDecoration);
                            this.AddDataFields(point.ChartDataLabel.Style.TextDecoration);
                        }
                        pointValues.ChartDataLabel.Label = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Label,this.DataSetName);
                        this.AddDataFields(point.ChartDataLabel.Label);
                        pointValues.ChartDataLabel.Position = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Position.ToString());
                        this.AddDataFields(point.ChartDataLabel.Position.ToString());
                        pointValues.ChartDataLabel.Rotation = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Rotation.ToString());
                        this.AddDataFields(point.ChartDataLabel.Rotation.ToString());
                        pointValues.ChartDataLabel.ToolTip = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.ToolTip);
                        this.AddDataFields(point.ChartDataLabel.ToolTip);
                        pointValues.ChartDataLabel.UseValueAsLabel = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.UseValueAsLabel.ToString());
                        this.AddDataFields(point.ChartDataLabel.UseValueAsLabel.ToString());
                        pointValues.ChartDataLabel.Visible = this.Model.ExpressionEngine.GetExpressionKey(point.ChartDataLabel.Visible.ToString());
                        this.AddDataFields(point.ChartDataLabel.Visible.ToString());
                    }
                    series.PointValues.Add(pointValues);

                    PointStylesExp pointStyle = new PointStylesExp();
                    if (point.Style != null)
                    {
                        pointStyle.Style = this.GetStyleExp(point.Style);
                    }
                    if (point.ChartMarker != null)
                    {
                        pointStyle.ChartMarker = new ChartMarkerExp();
                        if (item.ChartDataPoints != null && item.ChartDataPoints.First().ChartMarker != null &&
                            item.ChartDataPoints.First().ChartMarker.Style != null)
                        {
                            if (point.ChartMarker.Style.BackgroundImage != null)
                            {
                                pointStyle.ChartMarker.Image = this.GetImage(point.ChartMarker.Style);
                            }
                            pointStyle.ChartMarker.ShadowColor = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Style.ShadowColor);
                            this.AddDataFields(point.ChartMarker.Style.ShadowColor);
                            pointStyle.ChartMarker.Shadowoffset = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Style.ShadowOffset);
                            this.AddDataFields(point.ChartMarker.Style.ShadowOffset);

                            if (point.ChartMarker.Style.Border != null)
                            {
                                if (point.ChartMarker.Style.Border.Width != null)
                                {
                                    pointStyle.ChartMarker.Borderwidth = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Style.Border.Width.size);
                                    this.AddDataFields(point.ChartMarker.Style.Border.Width.size);
                                }

                                pointStyle.ChartMarker.BorderColor = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Style.Border.Color);
                                this.AddDataFields(point.ChartMarker.Style.Border.Color);
                            }
                            pointStyle.ChartMarker.Color = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Style.Color);
                            this.AddDataFields(point.ChartMarker.Style.Color);
                        }
                        pointStyle.ChartMarker.MarkerType = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Type);
                        this.AddDataFields(point.ChartMarker.Type);
                        if (point.ChartMarker.Size != null)
                        {
                            pointStyle.ChartMarker.Size = this.Model.ExpressionEngine.GetExpressionKey(point.ChartMarker.Size.size);
                            this.AddDataFields(point.ChartMarker.Size.size);
                        }
                    }
                    series.DataPointsStyle.Add(pointStyle);
                }

                series.EmptyPointsStyle = new PointStylesExp();
                if (item.ChartEmptyPoints != null)
                {
                    if (item.ChartEmptyPoints.Style != null)
                    {
                        series.EmptyPointsStyle.Style = this.GetStyleExp(item.ChartEmptyPoints.Style);
                    }
                    if (item.ChartEmptyPoints.ChartMarker != null)
                    {
                        series.EmptyPointsStyle.ChartMarker = new ChartMarkerExp();
                        if (item.ChartEmptyPoints.ChartMarker.Style != null)
                        {
                            if (item.ChartEmptyPoints.ChartMarker.Style.BackgroundImage != null)
                            {
                                series.EmptyPointsStyle.ChartMarker.Image = this.GetImage(item.ChartEmptyPoints.ChartMarker.Style);
                            }
                            series.EmptyPointsStyle.ChartMarker.ShadowColor = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Style.ShadowColor);
                            this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Style.ShadowColor);
                            series.EmptyPointsStyle.ChartMarker.Shadowoffset = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Style.ShadowOffset);
                            this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Style.ShadowOffset);
                            if (item.ChartEmptyPoints.ChartMarker.Style.Border != null)
                            {
                                if (item.ChartEmptyPoints.ChartMarker.Style.Border.Width != null)
                                {
                                    series.EmptyPointsStyle.ChartMarker.Borderwidth = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Style.Border.Width.size);
                                    this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Style.Border.Width.size);
                                }
                                series.EmptyPointsStyle.ChartMarker.BorderColor = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Style.Border.Color);
                                this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Style.Border.Color);
                            }
                            series.EmptyPointsStyle.ChartMarker.Color = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Style.Color);
                            this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Style.Color);
                        }
                        series.EmptyPointsStyle.ChartMarker.MarkerType = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Type);
                        this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Type);
                        if (item.ChartEmptyPoints.ChartMarker.Size != null)
                        {
                            series.EmptyPointsStyle.ChartMarker.Size = this.Model.ExpressionEngine.GetExpressionKey(item.ChartEmptyPoints.ChartMarker.Size.size);
                            this.AddDataFields(item.ChartEmptyPoints.ChartMarker.Size.size);
                        }
                    }
                }

                series.Hidden = this.Model.ExpressionEngine.GetExpressionKey(item.Hidden);
                this.AddDataFields(item.Hidden);

                series.ChartAreaYAxis = new List<ChartAreaAxisExp>();
                foreach (var cat in chartArea.ChartValueAxes)
                {
                    ChartAreaAxisExp areaAxis = new ChartAreaAxisExp();
                    areaAxis = this.GetAreaAxis(cat);
                    series.ChartAreaYAxis.Add(areaAxis);
                }

                series.ChartAreaXAxis = new List<ChartAreaAxisExp>();
                foreach (var cat in chartArea.ChartCategoryAxes)
                {
                    ChartAreaAxisExp areaAxis = new ChartAreaAxisExp();
                    areaAxis = this.GetAreaAxis(cat);
                    series.ChartAreaXAxis.Add(areaAxis);
                }

                series.CustomProperties = new CustomPropertiesExp();
                if (item.CustomProperties != null)
                {
                    foreach (DOM.CustomProperty property in item.CustomProperties)
                    {
                        CustomPropertyExp pro = new CustomPropertyExp();
                        pro.Name = this.Model.ExpressionEngine.GetExpressionKey(property.Name);
                        pro.Value = property.Value;
                        series.CustomProperties.Add(pro);
                    }
                }
                if (item.ChartSmartLabel != null)
                {
                    series.ChartSmartLabel = new ChartSmartLabelExp();
                    series.ChartSmartLabel.AllowOutSidePlotArea = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.AllowOutSidePlotArea.ToString());
                    this.AddDataFields(item.ChartSmartLabel.AllowOutSidePlotArea.ToString());
                    series.ChartSmartLabel.CalloutBackColor = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutBackColor);
                    this.AddDataFields(item.ChartSmartLabel.CalloutBackColor);
                    series.ChartSmartLabel.CalloutLineAnchor = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutLineAnchor.ToString());
                    this.AddDataFields(item.ChartSmartLabel.CalloutLineAnchor.ToString());
                    series.ChartSmartLabel.CalloutLineColor = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutLineColor);
                    this.AddDataFields(item.ChartSmartLabel.CalloutLineColor);
                    series.ChartSmartLabel.CalloutLineStyle = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutLineStyle.ToString());
                    this.AddDataFields(item.ChartSmartLabel.CalloutLineStyle.ToString());
                    if (item.ChartSmartLabel.CalloutLineWidth != null)
                    {
                        series.ChartSmartLabel.CalloutLineWidth = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutLineWidth.size);
                        this.AddDataFields(item.ChartSmartLabel.CalloutLineWidth.size);
                    }
                    series.ChartSmartLabel.CalloutStyle = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.CalloutStyle.ToString());
                    this.AddDataFields(item.ChartSmartLabel.CalloutStyle.ToString());
                    if (item.ChartSmartLabel.ChartNoMoveDirection != null)
                    {
                        series.ChartSmartLabel.ChartNoMoveDirection = new ChartNoMoveDirectionExp();
                        series.ChartSmartLabel.ChartNoMoveDirection.Down = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.Down.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.Down.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.DownLeft = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.DownLeft.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.DownLeft.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.DownRight = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.DownRight.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.DownRight.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.Left = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.Left.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.Left.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.Right = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.Right.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.Right.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.Up = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.Up.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.Up.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.UpLeft = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.UpLeft.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.UpLeft.ToString());
                        series.ChartSmartLabel.ChartNoMoveDirection.UpRight = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ChartNoMoveDirection.UpRight.ToString());
                        this.AddDataFields(item.ChartSmartLabel.ChartNoMoveDirection.UpRight.ToString());
                    }
                    series.ChartSmartLabel.Disabled = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.Disabled.ToString());
                    this.AddDataFields(item.ChartSmartLabel.Disabled.ToString());
                    series.ChartSmartLabel.MarkerOverlapping = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.MarkerOverlapping.ToString());
                    this.AddDataFields(item.ChartSmartLabel.MarkerOverlapping.ToString());
                    if (item.ChartSmartLabel.MaxMovingDistance != null)
                    {
                        series.ChartSmartLabel.MaxMovingDistance = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.MaxMovingDistance.size);
                        this.AddDataFields(item.ChartSmartLabel.MaxMovingDistance.size);
                    }
                    if (item.ChartSmartLabel.MinMovingDistance != null)
                    {
                        series.ChartSmartLabel.MinMovingDistance = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.MinMovingDistance.size);
                        this.AddDataFields(item.ChartSmartLabel.MinMovingDistance.size);
                    }
                    series.ChartSmartLabel.ShowOverlapped = this.Model.ExpressionEngine.GetExpressionKey(item.ChartSmartLabel.ShowOverlapped.ToString());
                    this.AddDataFields(item.ChartSmartLabel.ShowOverlapped.ToString());
                }

                if (item.ChartItemInLegend != null)
                {
                    series.Legend = new ChartItemInLegendExp();
                    //series.Legend.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey();
                    series.Legend.Hidden = this.Model.ExpressionEngine.GetExpressionKey(item.ChartItemInLegend.Hidden);
                    this.AddDataFields(item.ChartItemInLegend.Hidden);
                    series.Legend.LegendText = this.Model.ExpressionEngine.GetExpressionKey(item.ChartItemInLegend.LegendText);
                    this.AddDataFields(item.ChartItemInLegend.LegendText);
                    series.Legend.ToolTip = this.Model.ExpressionEngine.GetExpressionKey(item.ChartItemInLegend.ToolTip);
                    this.AddDataFields(item.ChartItemInLegend.ToolTip);
                }
                chartAreaExp.ChartSeries.Add(series);
            }
            return chartAreaExp;
        }

        ChartAreaAxisExp GetAreaAxis(DOM.ChartAxis cat)
        {
            ChartAreaAxisExp areaAxis = new ChartAreaAxisExp();

            areaAxis.AllowLabelRotation = this.Model.ExpressionEngine.GetExpressionKey(cat.AllowLabelRotation.ToString());
            this.AddDataFields(cat.AllowLabelRotation.ToString());
            areaAxis.Angle = this.Model.ExpressionEngine.GetExpressionKey(cat.Angle.ToString());
            this.AddDataFields(cat.Angle.ToString());
            areaAxis.Arrows = this.Model.ExpressionEngine.GetExpressionKey(cat.Arrows.ToString());
            this.AddDataFields(cat.Arrows.ToString());
            if (cat.ChartAxisScaleBreak != null)
            {
                areaAxis.ChartAxisScaleBreak = new ChartAxisScaleBreakExp();
                areaAxis.ChartAxisScaleBreak.BreakLineType = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.BreakLineType.ToString());
                this.AddDataFields(cat.ChartAxisScaleBreak.BreakLineType.ToString());
                areaAxis.ChartAxisScaleBreak.CollapsibleSpaceThreshold = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.CollapsibleSpaceThreshold.ToString());
                this.AddDataFields(cat.ChartAxisScaleBreak.CollapsibleSpaceThreshold.ToString());
                areaAxis.ChartAxisScaleBreak.Enabled = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.Enabled);
                this.AddDataFields(cat.ChartAxisScaleBreak.Enabled);
                areaAxis.ChartAxisScaleBreak.IncludeZero = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.IncludeZero.ToString());
                this.AddDataFields(cat.ChartAxisScaleBreak.IncludeZero.ToString());
                areaAxis.ChartAxisScaleBreak.MaxNumberOfBreaks = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.MaxNumberOfBreaks.ToString());
                this.AddDataFields(cat.ChartAxisScaleBreak.MaxNumberOfBreaks.ToString());
                areaAxis.ChartAxisScaleBreak.Spacing = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisScaleBreak.Spacing.ToString());
                this.AddDataFields(cat.ChartAxisScaleBreak.Spacing.ToString());
                if (cat.ChartAxisScaleBreak.Style != null)
                {
                    if (cat.ChartAxisScaleBreak.Style.Border != null)
                        areaAxis.ChartAxisScaleBreak.Style = this.GetBorderStyleExp(cat.ChartAxisScaleBreak.Style.Border);
                }
            }

            if (cat.ChartAxisTitle != null)
            {
                areaAxis.ChartAxisTitle = new ChartAxiesExp();
                areaAxis.ChartAxisTitle.Caption = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisTitle.Caption);
                this.AddDataFields(cat.ChartAxisTitle.Caption);
                if (cat.ChartAxisTitle.Style != null)
                {
                    areaAxis.ChartAxisTitle.Font = this.GetFontExp(cat.ChartAxisTitle.Style);
                    areaAxis.ChartAxisTitle.Textalign = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisTitle.Style.TextAlign);
                    this.AddDataFields(cat.ChartAxisTitle.Style.TextAlign);
                    areaAxis.ChartAxisTitle.TextColor = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisTitle.Style.Color);
                    this.AddDataFields(cat.ChartAxisTitle.Style.Color);
                    areaAxis.ChartAxisTitle.TextDecoration = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisTitle.Style.TextDecoration);
                    this.AddDataFields(cat.ChartAxisTitle.Style.TextDecoration);
                }
                areaAxis.ChartAxisTitle.Textorientation = this.Model.ExpressionEngine.GetExpressionKey(cat.ChartAxisTitle.TextOrientation.ToString());
                this.AddDataFields(cat.ChartAxisTitle.TextOrientation.ToString());
                if (cat.ChartMajorGridLines != null)
                {
                    areaAxis.ChartMajorGridLines = this.GetChartGridLines(cat.ChartMajorGridLines);
                }
                if (cat.ChartMinorGridLines != null)
                {
                    areaAxis.ChartMinorGridLines = this.GetChartGridLines(cat.ChartMinorGridLines);
                }
                if (cat.ChartMajorTickMarks != null)
                {
                    areaAxis.ChartMajorTickMarks = this.GetChartTickMarks(cat.ChartMajorTickMarks);
                }
                if (cat.ChartMinorTickMarks != null)
                {
                    areaAxis.ChartMinorTickMarks = this.GetChartTickMarks(cat.ChartMinorTickMarks);
                }
                areaAxis.ChartStripLines = new List<ChartStripLineExp>();
                if (cat.ChartStripLines != null)
                {
                    foreach (DOM.ChartStripLine stripln in cat.ChartStripLines)
                    {
                        ChartStripLineExp strip = new ChartStripLineExp();
                        // strip.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey();
                        strip.Interval = this.Model.ExpressionEngine.GetExpressionKey(stripln.Interval.ToString());
                        this.AddDataFields(stripln.Interval.ToString());
                        strip.IntervalOffset = this.Model.ExpressionEngine.GetExpressionKey(stripln.IntervalOffset.ToString());
                        this.AddDataFields(stripln.IntervalOffset.ToString());
                        strip.IntervalOffsetType = this.Model.ExpressionEngine.GetExpressionKey(stripln.IntervalOffsetType.ToString());
                        this.AddDataFields(stripln.IntervalOffsetType.ToString());
                        strip.IntervalType = this.Model.ExpressionEngine.GetExpressionKey(stripln.IntervalType.ToString());
                        this.AddDataFields(stripln.IntervalType.ToString());
                        strip.StripWidth = this.Model.ExpressionEngine.GetExpressionKey(stripln.StripWidth.ToString());
                        this.AddDataFields(stripln.StripWidth.ToString());
                        strip.StripWidthType = this.Model.ExpressionEngine.GetExpressionKey(stripln.StripWidthType.ToString());
                        this.AddDataFields(stripln.StripWidthType.ToString());
                        strip.Style = this.GetStyleExp(stripln.Style);
                        strip.TextOrientation = this.Model.ExpressionEngine.GetExpressionKey(stripln.TextOrientation.ToString());
                        this.AddDataFields(stripln.TextOrientation.ToString());
                        strip.Title = this.Model.ExpressionEngine.GetExpressionKey(stripln.Title);
                        this.AddDataFields(stripln.Title);
                        strip.ToolTip = this.Model.ExpressionEngine.GetExpressionKey(stripln.ToolTip);
                        this.AddDataFields(stripln.ToolTip);
                        areaAxis.ChartStripLines.Add(strip);
                    }
                }
            }
            if (cat.CrossAt != null)
            {
                areaAxis.CrossAt = this.Model.ExpressionEngine.GetExpressionKey(cat.CrossAt.ToString());
                this.AddDataFields(cat.CrossAt.ToString());
            }
            areaAxis.CustomProperties = new CustomPropertiesExp();
            if (cat.CustomProperties != null)
            {
                foreach (DOM.CustomProperty property in cat.CustomProperties)
                {
                    CustomPropertyExp pro = new CustomPropertyExp();
                    pro.Name = this.Model.ExpressionEngine.GetExpressionKey(property.Name);
                    this.AddDataFields(property.Name);
                    pro.Value = property.Value;
                    areaAxis.CustomProperties.Add(pro);
                }
            }
            areaAxis.HideEndLabels = this.Model.ExpressionEngine.GetExpressionKey(cat.HideEndLabels.ToString());
            this.AddDataFields(cat.HideEndLabels.ToString());
            areaAxis.HideLabels = this.Model.ExpressionEngine.GetExpressionKey(cat.HideLabels.ToString());
            this.AddDataFields(cat.HideLabels.ToString());
            areaAxis.IncludeZero = this.Model.ExpressionEngine.GetExpressionKey(cat.IncludeZero.ToString());
            this.AddDataFields(cat.IncludeZero.ToString());
            areaAxis.Interlaced = this.Model.ExpressionEngine.GetExpressionKey(cat.Interlaced.ToString());
            this.AddDataFields(cat.Interlaced.ToString());
            areaAxis.InterlacedColor = this.Model.ExpressionEngine.GetExpressionKey(cat.InterlacedColor);
            this.AddDataFields(cat.InterlacedColor);
            areaAxis.Interval = this.Model.ExpressionEngine.GetExpressionKey(cat.Interval);
            this.AddDataFields(cat.Interval);
            areaAxis.IntervalOffset = this.Model.ExpressionEngine.GetExpressionKey(cat.IntervalOffset.ToString());
            this.AddDataFields(cat.IntervalOffset.ToString());
            areaAxis.IntervalOffsetType = this.Model.ExpressionEngine.GetExpressionKey(cat.IntervalOffsetType.ToString());
            this.AddDataFields(cat.IntervalOffsetType.ToString());
            areaAxis.IntervalType = this.Model.ExpressionEngine.GetExpressionKey(cat.IntervalType.ToString());
            this.AddDataFields(cat.IntervalType.ToString());
            areaAxis.LabelInterval = this.Model.ExpressionEngine.GetExpressionKey(cat.LabelInterval.ToString());
            this.AddDataFields(cat.LabelInterval.ToString());
            areaAxis.LabelIntervalOffset = this.Model.ExpressionEngine.GetExpressionKey(cat.LabelIntervalOffset.ToString());
            this.AddDataFields(cat.LabelIntervalOffset.ToString());
            areaAxis.LabelIntervalOffsetType = this.Model.ExpressionEngine.GetExpressionKey(cat.LabelIntervalOffsetType.ToString());
            this.AddDataFields(cat.LabelIntervalOffsetType.ToString());
            areaAxis.LabelIntervalType = this.Model.ExpressionEngine.GetExpressionKey(cat.LabelIntervalType.ToString());
            this.AddDataFields(cat.LabelIntervalType.ToString());
            areaAxis.LabelsAutoFitDisabled = this.Model.ExpressionEngine.GetExpressionKey(cat.LabelsAutoFitDisabled.ToString());
            this.AddDataFields(cat.LabelsAutoFitDisabled.ToString());
            areaAxis.Location = this.Model.ExpressionEngine.GetExpressionKey(cat.Location.ToString());
            this.AddDataFields(cat.Location.ToString());
            areaAxis.LogBase = this.Model.ExpressionEngine.GetExpressionKey(cat.LogBase.ToString());
            this.AddDataFields(cat.LogBase.ToString());
            areaAxis.LogScale = this.Model.ExpressionEngine.GetExpressionKey(cat.LogScale.ToString());
            this.AddDataFields(cat.LogScale.ToString());
            areaAxis.Margin = this.Model.ExpressionEngine.GetExpressionKey(cat.Margin);
            this.AddDataFields(cat.Margin);
            areaAxis.MarksAlwaysAtPlotEdge = this.Model.ExpressionEngine.GetExpressionKey(cat.MarksAlwaysAtPlotEdge.ToString());
            this.AddDataFields(cat.MarksAlwaysAtPlotEdge.ToString());
            if (cat.MaxFontSize != null)
            {
                areaAxis.MaxFontSize = this.Model.ExpressionEngine.GetExpressionKey(cat.MaxFontSize.size);
                this.AddDataFields(cat.MaxFontSize.size);
            }
            areaAxis.Maximum = this.Model.ExpressionEngine.GetExpressionKey(cat.Maximum);
            this.AddDataFields(cat.Maximum);
            if (cat.MinFontSize != null)
            {
                areaAxis.MinFontSize = this.Model.ExpressionEngine.GetExpressionKey(cat.MinFontSize.size);
                this.AddDataFields(cat.MinFontSize.size);
            }
            areaAxis.Minimum = this.Model.ExpressionEngine.GetExpressionKey(cat.Minimum);
            this.AddDataFields(cat.Minimum);
            areaAxis.Name = this.Model.ExpressionEngine.GetExpressionKey(cat.Name);
            this.AddDataFields(cat.Name);
            areaAxis.OffsetLabels = this.Model.ExpressionEngine.GetExpressionKey(cat.OffsetLabels);
            this.AddDataFields(cat.OffsetLabels);
            areaAxis.PreventFontGrow = this.Model.ExpressionEngine.GetExpressionKey(cat.PreventFontGrow.ToString());
            this.AddDataFields(cat.PreventFontGrow.ToString());
            areaAxis.PreventFontShrink = this.Model.ExpressionEngine.GetExpressionKey(cat.PreventFontShrink.ToString());
            this.AddDataFields(cat.PreventFontShrink.ToString());
            areaAxis.PreventLabelOffset = this.Model.ExpressionEngine.GetExpressionKey(cat.PreventLabelOffset.ToString());
            this.AddDataFields(cat.PreventLabelOffset.ToString());
            areaAxis.PreventWordWrap = this.Model.ExpressionEngine.GetExpressionKey(cat.PreventWordWrap.ToString());
            this.AddDataFields(cat.PreventWordWrap.ToString());
            areaAxis.Reverse = this.Model.ExpressionEngine.GetExpressionKey(cat.Reverse.ToString());
            this.AddDataFields(cat.Reverse.ToString());
            areaAxis.Scalar = cat.Scalar;
            areaAxis.VariableAutoInterval = this.Model.ExpressionEngine.GetExpressionKey(cat.VariableAutoInterval.ToString());
            this.AddDataFields(cat.VariableAutoInterval.ToString());
            areaAxis.Visible = this.Model.ExpressionEngine.GetExpressionKey(cat.Visible.ToString());
            this.AddDataFields(cat.Visible.ToString());
            if (cat.Style != null)
            {
                if (cat.Style.Border != null)
                    areaAxis.Style = this.GetBorderStyleExp(cat.Style.Border);
                areaAxis.LabelColor = this.Model.ExpressionEngine.GetExpressionKey(cat.Style.Color);
                this.AddDataFields(cat.Style.Color);
                areaAxis.LabelFont = this.GetFontExp(cat.Style);
                areaAxis.LabelFormat = this.Model.ExpressionEngine.GetExpressionKey(cat.Style.Format);
                this.AddDataFields(cat.Style.Format);
                areaAxis.TextDecoration = this.Model.ExpressionEngine.GetExpressionKey(cat.Style.TextDecoration);
                this.AddDataFields(cat.Style.TextDecoration);
            }
            return areaAxis;
        }

        ChartGridLinesExp GetChartGridLines(DOM.ChartGridLines chartGridline)
        {
            ChartGridLinesExp gridLines = new ChartGridLinesExp();
            gridLines.Enabled = this.Model.ExpressionEngine.GetExpressionKey(chartGridline.Enabled.ToString());
            this.AddDataFields(chartGridline.Enabled.ToString());
            gridLines.Interval = this.Model.ExpressionEngine.GetExpressionKey(chartGridline.Interval.ToString());
            this.AddDataFields(chartGridline.Interval.ToString());
            gridLines.IntervalOffset = this.Model.ExpressionEngine.GetExpressionKey(chartGridline.IntervalOffset.ToString());
            this.AddDataFields(chartGridline.IntervalOffset.ToString());
            gridLines.IntervalOffsetType = this.Model.ExpressionEngine.GetExpressionKey(chartGridline.IntervalOffsetType.ToString());
            this.AddDataFields(chartGridline.IntervalOffsetType.ToString());
            gridLines.IntervalType = this.Model.ExpressionEngine.GetExpressionKey(chartGridline.IntervalType.ToString());
            this.AddDataFields(chartGridline.IntervalType.ToString());
            if (chartGridline.Style != null)
            {
                if (chartGridline.Style.Border != null)
                    gridLines.Style = this.GetBorderStyleExp(chartGridline.Style.Border);
            }
            return gridLines;
        }

        ChartTickMarksExp GetChartTickMarks(DOM.ChartTickMarks chartTickMark)
        {
            ChartTickMarksExp tickMarks = new ChartTickMarksExp();
            tickMarks.Enabled = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.Enabled.ToString());
            this.AddDataFields(chartTickMark.Enabled.ToString());
            tickMarks.Interval = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.Interval.ToString());
            this.AddDataFields(chartTickMark.Interval.ToString());
            tickMarks.IntervalOffset = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.IntervalOffset.ToString());
            this.AddDataFields(chartTickMark.IntervalOffset.ToString());
            tickMarks.IntervalOffsetType = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.IntervalOffsetType.ToString());
            this.AddDataFields(chartTickMark.IntervalOffsetType.ToString());
            tickMarks.IntervalType = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.IntervalType.ToString());
            this.AddDataFields(chartTickMark.IntervalType.ToString());
            tickMarks.Length = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.Length.ToString());
            this.AddDataFields(chartTickMark.Length.ToString());
            if (chartTickMark.Style != null)
            {
                if (chartTickMark.Style.Border != null)
                {
                    tickMarks.Style = this.GetBorderStyleExp(chartTickMark.Style.Border);
                }
            }
            tickMarks.Type = this.Model.ExpressionEngine.GetExpressionKey(chartTickMark.Type.ToString());
            this.AddDataFields(chartTickMark.Type.ToString());
            return tickMarks;
        }

        ChartLegendExp GetChartLegend(DOM.ChartLegend legend)
        {
            ChartLegendExp legendExp = new ChartLegendExp();

            if (legend.ChartLegendTitle != null)
            {
                legendExp.LegendTile = new ChartLegandTileExp();
                legendExp.LegendTile.Caption = legend.ChartLegendTitle.Caption;
                if (legend.ChartLegendTitle.Style != null)
                {
                    legendExp.LegendTile.Font = this.GetFontExp(legend.ChartLegendTitle.Style);
                }
                //Not 
                //legendExp.LegendTile.SeperatorColor = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                //legendExp.LegendTile.SeperatorStyle = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                //legendExp.LegendTile.SeperatorStyle = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                legendExp.LegendTile.TitleSeparator = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.TitleSeparator.ToString());
                this.AddDataFields(legend.ChartLegendTitle.TitleSeparator.ToString());
            }
            legendExp.Name = legend.Name;
            legendExp.Position = this.Model.ExpressionEngine.GetExpressionKey(legend.Position.ToString());
            this.AddDataFields(legend.Position.ToString());
            legendExp.Style = this.GetStyleExp(legend.Style);
            legendExp.TextWrapThreshold = this.Model.ExpressionEngine.GetExpressionKey(legend.TextWrapThreshold.ToString());
            this.AddDataFields(legend.TextWrapThreshold.ToString());
            legendExp.AutoFitTextDisabled = this.Model.ExpressionEngine.GetExpressionKey(legend.AutoFitTextDisabled.ToString());
            this.AddDataFields(legend.AutoFitTextDisabled.ToString());
            legendExp.ColumnSeparator = this.Model.ExpressionEngine.GetExpressionKey(legend.ColumnSeparator.ToString());
            this.AddDataFields(legend.ColumnSeparator.ToString());
            legendExp.ColumnSeparatorColor = this.Model.ExpressionEngine.GetExpressionKey(legend.ColumnSeparatorColor);
            this.AddDataFields(legend.ColumnSeparatorColor);
            legendExp.ColumnSpacing = this.Model.ExpressionEngine.GetExpressionKey(legend.ColumnSpacing.ToString());
            this.AddDataFields(legend.ColumnSpacing.ToString());
            legendExp.DockOutsideChartArea = this.Model.ExpressionEngine.GetExpressionKey(legend.DockOutsideChartArea.ToString());
            this.AddDataFields(legend.DockOutsideChartArea.ToString());
            legendExp.DockToChartArea = legend.DockToChartArea;
            legendExp.EquallySpacedItems = this.Model.ExpressionEngine.GetExpressionKey(legend.EquallySpacedItems.ToString());
            this.AddDataFields(legend.EquallySpacedItems.ToString());
            legendExp.HeaderSeparator = this.Model.ExpressionEngine.GetExpressionKey(legend.HeaderSeparator.ToString());
            this.AddDataFields(legend.HeaderSeparator.ToString());
            legendExp.HeaderSeparatorColor = this.Model.ExpressionEngine.GetExpressionKey(legend.HeaderSeparatorColor);
            this.AddDataFields(legend.HeaderSeparatorColor);
            legendExp.Hidden = this.Model.ExpressionEngine.GetExpressionKey(legend.Hidden.ToString());
            this.AddDataFields(legend.Hidden.ToString());
            legendExp.InterlacedRows = this.Model.ExpressionEngine.GetExpressionKey(legend.InterlacedRows.ToString());
            this.AddDataFields(legend.InterlacedRows.ToString());
            legendExp.InterlacedRowsColor = this.Model.ExpressionEngine.GetExpressionKey(legend.InterlacedRowsColor);
            this.AddDataFields(legend.InterlacedRowsColor);
            legendExp.Layout = this.Model.ExpressionEngine.GetExpressionKey(legend.Layout.ToString());
            this.AddDataFields(legend.Layout.ToString());
            legendExp.MaxAutoSize = this.Model.ExpressionEngine.GetExpressionKey(legend.MaxAutoSize.ToString());
            this.AddDataFields(legend.MaxAutoSize.ToString());
            if (legend.MinFontSize != null)
            {
                legendExp.MinFontSize = this.Model.ExpressionEngine.GetExpressionKey(legend.MinFontSize.size);
                this.AddDataFields(legend.MinFontSize.size);
            }
            legendExp.Reversed = this.Model.ExpressionEngine.GetExpressionKey(legend.Reversed.ToString());
            this.AddDataFields(legend.Reversed.ToString());
            return legendExp;
        }

        ChartTileExp GetChartTile(DOM.ChartTitle title)
        {
            ChartTileExp tileExp = new ChartTileExp();
            // tileExp.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey(title.ActionInfo.Actions.First());
            tileExp.Caption = this.Model.ExpressionEngine.GetExpressionKey(title.Caption);
            tileExp.Name = this.Model.ExpressionEngine.GetExpressionKey(title.Name);
            tileExp.Hidden = this.Model.ExpressionEngine.GetExpressionKey(title.Hidden.ToString());
            this.AddDataFields(title.Hidden.ToString());
            tileExp.ToolTip = this.Model.ExpressionEngine.GetExpressionKey(title.ToolTip);
            this.AddDataFields(title.ToolTip);
            if (title.Style != null)
            {
                tileExp.Style = this.GetStyleExp(title.Style);
            }
            tileExp.DocToChartArea = title.DockToChartArea;
            tileExp.DockOutChartArea = this.Model.ExpressionEngine.GetExpressionKey(title.DockOutChartArea.ToString());
            this.AddDataFields(title.DockOutChartArea.ToString());
            tileExp.DockOffset = this.Model.ExpressionEngine.GetExpressionKey(title.DockOffset.ToString());
            this.AddDataFields(title.DockOffset.ToString());
            tileExp.Position = this.Model.ExpressionEngine.GetExpressionKey(title.Position.ToString());
            this.AddDataFields(title.Position.ToString());
            tileExp.TextOerientation = this.Model.ExpressionEngine.GetExpressionKey(title.TextOrientation.ToString());
            this.AddDataFields(title.TextOrientation.ToString());
            return tileExp;
        }

        BorderStyleExp GetBorderStyleExp(DOM.Border border)
        {
            BorderStyleExp borderStyle = new BorderStyleExp();
            borderStyle.BorderColor = this.Model.ExpressionEngine.GetExpressionKey(border.Color);
            this.AddDataFields(border.Color);
            borderStyle.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(border.Style);
            this.AddDataFields(border.Style);
            if (border.Width != null)
            {
                borderStyle.BorderWidth = this.Model.ExpressionEngine.GetExpressionKey(border.Width.size);
                this.AddDataFields(border.Width.size);
            }
            return borderStyle;
        }

        BorderSideExp GetEdgesBorder(DOM.Style style)
        {
            bool isCheck = false;
            BorderSideExp borderEdg = new BorderSideExp();
            Border border = new Border();
            if (style.LeftBorder != null)
            {
                border.Color = style.LeftBorder.Color;
                border.Style = style.LeftBorder.Style;
                border.Width = style.LeftBorder.Width;
                borderEdg.Left = this.GetBorderStyleExp(border);
                isCheck = true;
            }
            if (style.TopBorder != null)
            {
                border.Color = style.TopBorder.Color;
                border.Style = style.TopBorder.Style;
                border.Width = style.TopBorder.Width;
                borderEdg.Top = this.GetBorderStyleExp(border);
                isCheck = true;
            }
            if (style.BottomBorder != null)
            {
                border.Color = style.BottomBorder.Color;
                border.Style = style.BottomBorder.Style;
                border.Width = style.BottomBorder.Width;
                borderEdg.Bottem = this.GetBorderStyleExp(border);
                isCheck = true;
            }
            if (style.RightBorder != null)
            {
                border.Color = style.RightBorder.Color;
                border.Style = style.RightBorder.Style;
                border.Width = style.RightBorder.Width;
                borderEdg.Right = this.GetBorderStyleExp(border);
                isCheck = true;
            }
            if (!isCheck)
                return null;
            return borderEdg;
        }

        BackGroundImageExp GetImage(DOM.Style style)
        {
            BackGroundImageExp imageExp = new BackGroundImageExp();
            imageExp.BackgroundRepeat = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundImage.BackgroundRepeat);
            this.AddDataFields(style.BackgroundImage.BackgroundRepeat);
            imageExp.MimeType = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundImage.MIMEType);
            this.AddDataFields(style.BackgroundImage.MIMEType);
            imageExp.Position = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundImage.Position.ToString());
            this.AddDataFields(style.BackgroundImage.Position.ToString());
            imageExp.Source = style.BackgroundImage.Source.ToString();
            imageExp.Value = style.BackgroundImage.Value;
            imageExp.TransparentColor = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundImage.TransparentColor);
            this.AddDataFields(style.BackgroundImage.TransparentColor);
            return imageExp;
        }

        FillStyleExp GetFillStyle(DOM.Style style)
        {
            FillStyleExp fillStyle = new FillStyleExp();
            fillStyle.BackgroundColor = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundColor);
            this.AddDataFields(style.BackgroundColor);
            fillStyle.BackgroundGradientEndcolor = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundGradientEndColor);
            this.AddDataFields(style.BackgroundGradientEndColor);
            fillStyle.BackgroundGradientType = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundGradientType.ToString());
            this.AddDataFields(style.BackgroundGradientType.ToString());
            fillStyle.BackgroundHatchType = this.Model.ExpressionEngine.GetExpressionKey(style.BackgroundHatchType.ToString());
            this.AddDataFields(style.BackgroundHatchType.ToString());
            return fillStyle;
        }

        TileStyleExp GetStyleExp(DOM.Style style)
        {
            TileStyleExp styleExp = new TileStyleExp();
            if (style.BackgroundImage != null)
            {
                styleExp.BackgroundImage = this.GetImage(style);
            }
            if (style.Border != null)
            {
                styleExp.Border = this.GetBorderStyleExp(style.Border);
            }
            styleExp.FillStyle = this.GetFillStyle(style);
            styleExp.ShadowColor = this.Model.ExpressionEngine.GetExpressionKey(style.ShadowColor);
            this.AddDataFields(style.ShadowColor);
            styleExp.ShadowOffset = this.Model.ExpressionEngine.GetExpressionKey(style.ShadowOffset);
            this.AddDataFields(style.ShadowOffset);
            styleExp.Color = this.Model.ExpressionEngine.GetExpressionKey(style.Color);
            this.AddDataFields(style.Color);
            styleExp.TextDecoration = this.Model.ExpressionEngine.GetExpressionKey(style.TextDecoration);
            this.AddDataFields(style.TextDecoration);
            styleExp.TextEffect = this.Model.ExpressionEngine.GetExpressionKey(style.TextEffect.ToString());
            this.AddDataFields(style.TextEffect.ToString());
            styleExp.Font = this.GetFontExp(style);
            return styleExp;
        }

        FontExp GetFontExp(DOM.Style style)
        {
            FontExp font = new FontExp();
            font.FontFamily = this.Model.ExpressionEngine.GetExpressionKey(style.FontFamily);
            this.AddDataFields(style.FontFamily);
            font.FontSize = this.Model.ExpressionEngine.GetExpressionKey(style.FontSize.size);
            this.AddDataFields(style.FontSize.size);
            font.FontStyle = this.Model.ExpressionEngine.GetExpressionKey(style.FontStyle);
            this.AddDataFields(style.FontStyle);
            font.FontWeight = this.Model.ExpressionEngine.GetExpressionKey(style.FontWeight);
            this.AddDataFields(style.FontWeight);
            return font;
        }

        #endregion

        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                if (!string.IsNullOrEmpty(key) && this.Model.ExpressionEngine.FieldInformations.ContainsKey(key))
                {
                    var fields = this.Model.ExpressionEngine.FieldInformations[key];

                    foreach (var field in fields)
                    {
                        var fieldsCount = (from expField in this.DataSetFields
                                           where (field.FunctionName.Equals(expField.FunctionName)
                                           && field.Name.Equals(expField.Name) && !field.IsDataSetField)
                                           select expField).Count();

                        if (fieldsCount == 0)
                        {
                            this.DataSetFields.Add(field);
                        }
                    }
                }
            }
        }

#if !SILVERLIGHT

        private ReportingChartControl chartControl = null;

        internal Stream GetImageStream()
        {
            chartControl = new ReportingChartControl(this);
            return (new ImageConversion().CovertToImage(chartControl));
        }

        internal List<ChartModel.SegmentActionInfo> GetChartSegementPoints()
        {
            var segments = chartControl.GetChartSegments();
            chartControl = null;
            return segments;
        }

#else
        internal Stream GetImageStream()
        {
            UIElement chart = null;
            try
            {
                 chart = ReportModel.UICollection[this.Name];
            }
            catch
            {
                return null;
            }
#if !WINRT
                WriteableBitmap _bitmap = new WriteableBitmap(chart, chart.RenderTransform);
            MemoryStream fs = new MemoryStream();
            int width = _bitmap.PixelWidth;
            int height = _bitmap.PixelHeight;

            Syncfusion.Windows.Chart.ChartImage ei = new Syncfusion.Windows.Chart.ChartImage(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int pixel = _bitmap.Pixels[(i * width) + j];
                    ei.SetPixel(j, i,
                                (byte)((pixel >> 16) & 0xFF),
                                (byte)((pixel >> 8) & 0xFF),
                                (byte)(pixel & 0xFF),
                                (byte)((pixel >> 24) & 0xFF)
                    );
                }
            }
            Stream png = ei.GetStream();
            int len = (int)png.Length;
            byte[] bytes = new byte[len];
            png.Read(bytes, 0, len);
            fs.Write(bytes, 0, len);
            fs.Position = 0;
            return fs;
#else
            return null;
#endif


        }

#endif

        void Intialize(Chart chart)
        {
        }

        #region overridden methods

        public override void Evaluate()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                ChartProperties = new ChartItemExpVal();
                if (chartExpProp.ToolTip != null)
                {
                    ChartProperties.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(chartExpProp.ToolTip);
                }
                if (!string.IsNullOrEmpty(chartExpProp.DocumentMapLable))
                {
                    ChartProperties.DocumentMapLable = this.Model.ExpressionEngine.GetEvalExpressionString(chartExpProp.DocumentMapLable);
                    this.DocumentMapLable = ChartProperties.DocumentMapLable;
                    this.SetTreeModel();
                }
                if (chartExpProp.Visibility != null)
                {
                    ChartProperties.Visibility = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartExpProp.Visibility));
                }
                this.Hidden = ChartProperties.Visibility;
                ChartProperties.ColorPalette = chartExpProp.ColorPalette;

                if (chartExpProp.Border != null)
                {
                    this.ChartProperties.Border = new BorderExpval();
                    if (chartExpProp.Border.Default != null)
                    {
                        this.ChartProperties.Border.Default = new BorderExpvalProperties();
                        this.ChartProperties.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.Default.BorderBrush);
                        this.ChartProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.Default.BorderStyle));
                        if (chartExpProp.Border.Default.Thickness != null)
                        {
                            this.ChartProperties.Border.Default.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.Default.Thickness)).FloatValue;
                        }
                    }
                    if (chartExpProp.Border.LeftBorder != null)
                    {
                        this.ChartProperties.Border.LeftBorder = new BorderExpvalProperties();
                        this.ChartProperties.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.LeftBorder.BorderBrush);
                        this.ChartProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.LeftBorder.BorderStyle));
                        if (chartExpProp.Border.LeftBorder.Thickness != null)
                        {
                            this.ChartProperties.Border.LeftBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.LeftBorder.Thickness)).FloatValue;
                        }
                    }
                    if (chartExpProp.Border.TopBorder != null)
                    {
                        this.ChartProperties.Border.TopBorder = new BorderExpvalProperties();
                        this.ChartProperties.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.TopBorder.BorderBrush);
                        this.ChartProperties.Border.TopBorder.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.TopBorder.BorderStyle));
                        if (chartExpProp.Border.TopBorder.Thickness != null)
                        {
                            this.ChartProperties.Border.TopBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.TopBorder.Thickness)).FloatValue;
                        }
                    }
                    if (chartExpProp.Border.RightBorder != null)
                    {
                        this.ChartProperties.Border.RightBorder = new BorderExpvalProperties();
                        this.ChartProperties.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.RightBorder.BorderBrush);
                        this.ChartProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.RightBorder.BorderStyle));
                        if (chartExpProp.Border.RightBorder.Thickness != null)
                        {
                            this.ChartProperties.Border.RightBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.RightBorder.Thickness)).FloatValue;
                        }
                    }
                    if (chartExpProp.Border.BottomBorder != null)
                    {
                        this.ChartProperties.Border.BottomBorder = new BorderExpvalProperties();
                        this.ChartProperties.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.BottomBorder.BorderBrush);
                        this.ChartProperties.Border.BottomBorder.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.BottomBorder.BorderStyle));
                        if (chartExpProp.Border.BottomBorder.Thickness != null)
                        {
                            this.ChartProperties.Border.BottomBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(chartExpProp.Border.BottomBorder.Thickness)).FloatValue;
                        }
                    }
                }
                this.ChartProperties.Type = TryEnum<VisualizationType>(chartExpProp.Type.ToString());

                this.ChartProperties.SubType = TryEnum<VisualizationSubType>(chartExpProp.SubType.ToString());

                if (chartExpProp.CustomPaletteColors != null)
                {
                    ChartProperties.CustomPaletteColors = new List<string>();
                    foreach (var color in chartExpProp.CustomPaletteColors)
                    {
                        ChartProperties.CustomPaletteColors.Add(this.Model.ExpressionEngine.GetEvalExpressionString(color));
                    }
                }

                ChartProperties.ChartTiles = new List<ChartTileExpVal>();
                if (chartExpProp.ChartTiles != null)
                {
                    foreach (ChartTileExp title in chartExpProp.ChartTiles)
                    {
                        ChartProperties.ChartTiles.Add(GetChartTile(title));
                    }
                }

                ChartProperties.ChartBorderSkin = new ChartBorderSkinExpVal();
                if (chartExpProp.ChartBorderSkin != null)
                {
                    ChartProperties.ChartBorderSkin.ChartBorderSkinType = chartExpProp.ChartBorderSkin.ToString();
                    ChartProperties.ChartBorderSkin.Style = new ChartStyleExpVal();
                    if (chartExpProp.ChartBorderSkin.Style != null)
                    {
                        if (chartExpProp.ChartBorderSkin.Style.BackgroundImage != null)
                            ChartProperties.ChartBorderSkin.Style.BackgroundImage = this.GetImage(chartExpProp.ChartBorderSkin.Style.BackgroundImage);
                        if (chartExpProp.ChartBorderSkin.Style.Border != null)
                            ChartProperties.ChartBorderSkin.Style.Border = this.GetBorderStyleExp(chartExpProp.ChartBorderSkin.Style.Border);
                        if (chartExpProp.ChartBorderSkin.Style.BorderSyle != null)
                            ChartProperties.ChartBorderSkin.Style.BorderSyle = this.GetEdgesBorder(chartExpProp.ChartBorderSkin.Style.BorderSyle);
                        if (chartExpProp.ChartBorderSkin.Style.FillStyle != null)
                            ChartProperties.ChartBorderSkin.Style.FillStyle = this.GetFillStyle(chartExpProp.ChartBorderSkin.Style.FillStyle);
                    }
                }

                if (chartExpProp.ChartNoDataMessage != null)
                {
                    ChartProperties.ChartNoDataMessage = this.GetChartTile(chartExpProp.ChartNoDataMessage);
                }

                ChartProperties.DataSetName = this.DataSetName = chartExpProp.DataSetName;

                ChartProperties.ChartLegends = new List<ChartLegendExpVal>();
                if (chartExpProp.ChartLegends != null)
                {
                    foreach (ChartLegendExp legend in chartExpProp.ChartLegends)
                    {
                        ChartProperties.ChartLegends.Add(GetChartLegend(legend));
                    }
                }

                ChartProperties.ChartAreas = new List<ChartAreasExpVal>();
                if (chartExpProp.ChartAreas != null)
                {
                    foreach (ChartAreasExp chartArea in chartExpProp.ChartAreas)
                    {
                        ChartProperties.ChartAreas.Add(this.GetChartArea(chartArea));
                    }
                }

                ChartProperties.ChartSeriesHierarchy = new ChartMembersExpVal();
                if (chartExpProp.ChartSeriesHierarchy != null)
                {
                    foreach (ChartMemberExp chartMember in chartExpProp.ChartSeriesHierarchy)
                    {
                        ChartProperties.ChartSeriesHierarchy.Add(this.GetChartMember(chartMember));
                    }
                }

                ChartProperties.ChartCategoryHierarchy = new ChartMembersExpVal();
                if (chartExpProp.ChartCategoryHierarchy != null)
                {
                    foreach (ChartMemberExp chartMember in chartExpProp.ChartCategoryHierarchy)
                    {
                        ChartProperties.ChartCategoryHierarchy.Add(this.GetChartMember(chartMember));
                    }
                }

                if (chartExpProp.ChartStyle != null)
                {
                    ChartProperties.ChartStyle = this.GetStyleExp(chartExpProp.ChartStyle);
                }

                if (DataSetName != string.Empty && DataSetName != null)
                {
                    this.DataProcessingChart();
                }
                else
                {
                    this.Model.ExceptionDetails.Add("No Valid Item Source Exist for the Chart : " + Name);
                }

                base.Evaluate();
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }
        }

        ChartMemberExpVal GetChartMember(ChartMemberExp chartMember)
        {
            ChartMemberExpVal chartMem = new ChartMemberExpVal();
            chartMem.DataElementName = chartMember.DataElementName;
            chartMem.CustomProperties = new CustomPropertiesExpVal();
            if (chartMember.CustomProperties != null)
            {
                foreach (CustomPropertyExp property in chartMember.CustomProperties)
                {
                    CustomPropertyExpVal pro = new CustomPropertyExpVal();
                    pro.Name = this.Model.ExpressionEngine.GetEvalExpressionString(property.Name);
                    pro.Value = property.Value;
                    chartMem.CustomProperties.Add(pro);
                }
            }
            chartMem.DataElementOutput = TryEnum<DataElementOutputs>(chartMember.DataElementOutput.ToString());
            chartMem.Label = this.Model.ExpressionEngine.GetEvalExpressionString(chartMember.Label);
            if (chartMember.Group != null)
            {
                chartMem.Group = new GroupExpVal();
                if (chartMember.Group.BreakLocation != null)
                {
                    chartMem.Group.BreakLocation = TryEnum<BreakLocation>(chartMember.Group.BreakLocation);
                }
                chartMem.Group.DataElementName = chartMember.Group.DataElementName;
                chartMem.Group.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(chartMember.Group.DocumentMapLabel);
                chartMem.Group.DomainScope = chartMember.Group.DomainScope;
                chartMem.Group.Name = chartMember.Group.Name;
                chartMem.Group.Parent = this.Model.ExpressionEngine.GetEvalExpressionString(chartMember.Group.Parent);
                chartMem.Group.GroupExpressions = new List<string>();
                foreach (string grpExp in chartMember.Group.GroupExpressions)
                {
                    chartMem.Group.GroupExpressions.Add(this.Model.ExpressionEngine.GetEvalExpressionString(grpExp));
                }
                chartMem.Group.Filters = new FiltersExpVal();
                if (chartMember.Group.Filters != null)
                {
                    foreach (FilterExp filter in chartMember.Group.Filters)
                    {
                        FilterExpVal fl = new FilterExpVal();
                        fl.FilterExpression = this.Model.ExpressionEngine.GetEvalExpressionString(filter.FilterExpression);
                        fl.FilterValues = new FilterValuesExpVal();
                        foreach (FilterValueExp flValue in filter.FilterValues)
                        {
                            FilterValueExpVal flv = new FilterValueExpVal();
                            flv.DataType = TryEnum<DataTypes>(flValue.DataType.ToString());
                            flv.Value = this.Model.ExpressionEngine.GetEvalExpressionString(flValue.Value);
                            fl.FilterValues.Add(flv);
                        }
                        fl.Operator = TryEnum<FilterOperators>(filter.Operator.ToString());
                        chartMem.Group.Filters.Add(fl);
                    }
                }
            }
            chartMem.SortExpressions = new SortExpressionsExpVal();
            if (chartMember.SortExpressions != null)
            {
                foreach (SortExpressionExp sortExp in chartMember.SortExpressions)
                {
                    SortExpressionExpVal exp = new SortExpressionExpVal();
                    exp.Direction = TryEnum<SortDirection>(sortExp.Direction.ToString());
                    exp.Value = this.Model.ExpressionEngine.GetEvalExpressionString(sortExp.Value);
                    chartMem.SortExpressions.Add(exp);
                }
            }
            return chartMem;
        }

        ChartAreasExpVal GetChartArea(ChartAreasExp chartArea)
        {
            ChartAreasExpVal chartAreaExp = new ChartAreasExpVal();
            chartAreaExp.Name = chartArea.Name;
            chartAreaExp.Style = this.GetStyleExp(chartArea.Style);
            chartAreaExp.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Hidden));
            chartAreaExp.EquallySizedAxesFont = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.EquallySizedAxesFont));

            if (chartArea.Chart3D != null)
            {
                chartAreaExp.Chart3D = new ChartThreeDPropertiesExpVal();
                chartAreaExp.Chart3D.Clustered = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Clustered));
                chartAreaExp.Chart3D.DepthRatio = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.DepthRatio));
                chartAreaExp.Chart3D.Enabled = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Enabled));
                chartAreaExp.Chart3D.GapDepth = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.GapDepth));
                chartAreaExp.Chart3D.Inclination = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Inclination));
                chartAreaExp.Chart3D.Perspective = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Perspective));
                chartAreaExp.Chart3D.ProjectionMode = TryEnum<ProjectionMode>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.ProjectionMode));
                chartAreaExp.Chart3D.Rotation = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Rotation));
                chartAreaExp.Chart3D.Shading = TryEnum<Shading>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.Shading));
                chartAreaExp.Chart3D.WallThickness = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.Chart3D.WallThickness));
            }

            chartAreaExp.AlignOrientation = TryEnum<AlignOrientation>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.AlignOrientation.ToString()));

            chartAreaExp.AlignWithChartArea = this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.AlignWithChartArea);

            if (chartArea.ChartAlignType != null)
            {
                chartAreaExp.ChartAlignType = new ChartAlignTypeExpVal();
                chartAreaExp.ChartAlignType.AxesView = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.ChartAlignType.AxesView));
                chartAreaExp.ChartAlignType.Cursor = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(chartArea.ChartAlignType.Cursor));
                chartAreaExp.ChartAlignType.InnerPlotPosition = chartArea.ChartAlignType.InnerPlotPosition;
                chartAreaExp.ChartAlignType.Position = chartArea.ChartAlignType.Position;
            }

            chartAreaExp.ChartSeries = new List<ChartSeriesExpVal>();

            foreach (var item in chartArea.ChartSeries)
            {
                ChartSeriesExpVal series = new ChartSeriesExpVal();
                series.CategoryAxisName = item.CategoryAxisName;
                series.ValueAxisName = item.ValueAxisName;
                series.Name = item.Name;
                if (item.Style != null)
                {
                    series.Style = this.GetStyleExp(item.Style);
                }
                series.LegendName = item.LegendName;
                series.PointValues = new List<PointValuesExpVal>();
                series.DataPointsStyle = new List<PointStylesExpVal>();
                foreach (var point in item.PointValues)
                {
                    PointValuesExpVal pointValues = new PointValuesExpVal();
                    pointValues.End = point.End;
                    pointValues.High = point.High;
                    //pointValues.Mean = point.Mean;
                    //pointValues.Median = point.Median;
                    pointValues.LegendHidden = point.LegendHidden;
                    pointValues.LegendText = point.LegendText;
                    pointValues.Low = point.Low;
                    pointValues.Size = point.Size;
                    pointValues.Start = point.Start;
                    //get field name
                    pointValues.X = this.Model.ExpressionEngine.GetEvalExpressionString(point.X);
                    pointValues.Y = this.Model.ExpressionEngine.GetEvalExpressionString(point.Y);
                    if (point.ChartDataLabel != null)
                    {
                        pointValues.ChartDataLabel = new ChartDataLabelExpVal();
                        //pointValues.ChartDataLabel.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey();
                        pointValues.ChartDataLabel.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.BorderColor);
                        if (point.ChartDataLabel.BorderStyle != null)
                        {
                            pointValues.ChartDataLabel.BorderStyle = this.GetBorderStyleExp(point.ChartDataLabel.BorderStyle);
                        }
                        if (point.ChartDataLabel.Font != null)
                        {
                            pointValues.ChartDataLabel.Font = this.GetFontExp(point.ChartDataLabel.Font);
                        }
                        pointValues.ChartDataLabel.Format = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.Format);
                        pointValues.ChartDataLabel.TextColor = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.TextColor);
                        pointValues.ChartDataLabel.TextDecoration = TryEnum<DOM.TextDecoration>(this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.TextDecoration));
                        try
                        {
                            pointValues.ChartDataLabel.Label = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.Label);
                        }
                        catch { }
                        pointValues.ChartDataLabel.Position = TryEnum<DOM.Position>(this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.Position.ToString()));
                        pointValues.ChartDataLabel.Rotation = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.Rotation.ToString()));
                        pointValues.ChartDataLabel.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.ToolTip);
                        pointValues.ChartDataLabel.UseValueAsLabel = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.UseValueAsLabel.ToString()));
                        pointValues.ChartDataLabel.Visible = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartDataLabel.Visible.ToString()));
                    }

                    series.PointValues.Add(pointValues);
                }

                foreach (var point in item.DataPointsStyle)
                {
                    PointStylesExpVal pointStyle = new PointStylesExpVal();
                    if (point.Style != null)
                    {
                        pointStyle.Style = this.GetStyleExp(point.Style);
                    }
                    if (point.ChartMarker != null)
                    {
                        pointStyle.ChartMarker = new ChartMarkerExpVal();
                        if (point.ChartMarker.Image != null)
                        {
                            pointStyle.ChartMarker.Image = this.GetImage(point.ChartMarker.Image);
                        }
                        pointStyle.ChartMarker.MarkerType = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.MarkerType);
                        pointStyle.ChartMarker.Size = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.Size);
                        pointStyle.ChartMarker.ShadowColor = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.ShadowColor);
                        pointStyle.ChartMarker.Shadowoffset = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.Shadowoffset);
                        pointStyle.ChartMarker.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.BorderColor);
                        pointStyle.ChartMarker.Borderwidth = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.Borderwidth);
                        pointStyle.ChartMarker.Color = this.Model.ExpressionEngine.GetEvalExpressionString(point.ChartMarker.Color);
                    }
                    series.DataPointsStyle.Add(pointStyle);
                }

                series.EmptyPointsStyle = new PointStylesExpVal();
                if (item.EmptyPointsStyle != null)
                {
                    if (item.EmptyPointsStyle.Style != null)
                    {
                        series.EmptyPointsStyle.Style = this.GetStyleExp(item.EmptyPointsStyle.Style);
                    }
                    if (item.EmptyPointsStyle.ChartMarker != null)
                    {
                        series.EmptyPointsStyle.ChartMarker = new ChartMarkerExpVal();
                        if (item.EmptyPointsStyle.ChartMarker.Image != null)
                        {
                            series.EmptyPointsStyle.ChartMarker.Image = this.GetImage(item.EmptyPointsStyle.ChartMarker.Image);
                        }
                        series.EmptyPointsStyle.ChartMarker.MarkerType = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.Type);
                        series.EmptyPointsStyle.ChartMarker.Size = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.Size);
                        series.EmptyPointsStyle.ChartMarker.ShadowColor = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.ShadowColor);
                        series.EmptyPointsStyle.ChartMarker.Shadowoffset = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.Shadowoffset);
                        series.EmptyPointsStyle.ChartMarker.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.BorderColor);
                        series.EmptyPointsStyle.ChartMarker.Borderwidth = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.Borderwidth);
                        series.EmptyPointsStyle.ChartMarker.Color = this.Model.ExpressionEngine.GetEvalExpressionString(item.EmptyPointsStyle.ChartMarker.Color);
                    }
                }

                series.Hidden = TryParse<bool>(item.Hidden);

                series.ChartAreaXAxis = new List<ChartAreaAxisExpVal>();
                foreach (var xAxis in item.ChartAreaXAxis)
                {
                    ChartAreaAxisExpVal areaAxis = new ChartAreaAxisExpVal();
                    areaAxis = this.GetAreaAxis(xAxis);
                    series.ChartAreaXAxis.Add(areaAxis);
                }

                series.ChartAreaYAxis = new List<ChartAreaAxisExpVal>();
                foreach (var xAxis in item.ChartAreaYAxis)
                {
                    ChartAreaAxisExpVal areaAxis = new ChartAreaAxisExpVal();
                    areaAxis = this.GetAreaAxis(xAxis);
                    series.ChartAreaYAxis.Add(areaAxis);
                }

                series.CustomProperties = new CustomPropertiesExpVal();
                if (item.CustomProperties != null)
                {
                    foreach (CustomPropertyExp property in item.CustomProperties)
                    {
                        CustomPropertyExpVal pro = new CustomPropertyExpVal();
                        pro.Name = this.Model.ExpressionEngine.GetEvalExpressionString(property.Name);
                        pro.Value = property.Value;
                        series.CustomProperties.Add(pro);
                    }
                }
                if (item.ChartSmartLabel != null)
                {
                    series.ChartSmartLabel = new ChartSmartLabelExpVal();
                    series.ChartSmartLabel.AllowOutSidePlotArea = TryEnum<AllowOutSidePlotArea>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.AllowOutSidePlotArea.ToString()));
                    series.ChartSmartLabel.CalloutBackColor = this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutBackColor);
                    series.ChartSmartLabel.CalloutLineAnchor = TryEnum<CalloutLineAnchor>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutLineAnchor.ToString()));
                    series.ChartSmartLabel.CalloutLineColor = this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutLineColor);
                    series.ChartSmartLabel.CalloutLineStyle = TryEnum<LineStyle>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutLineStyle.ToString()));
                    series.ChartSmartLabel.CalloutLineWidth = this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutLineWidth);
                    series.ChartSmartLabel.CalloutStyle = TryEnum<CalloutStyle>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.CalloutStyle.ToString()));
                    if (item.ChartSmartLabel.ChartNoMoveDirection != null)
                    {
                        series.ChartSmartLabel.ChartNoMoveDirection = new ChartNoMoveDirectionExpVal();
                        series.ChartSmartLabel.ChartNoMoveDirection.Down = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.Down));
                        series.ChartSmartLabel.ChartNoMoveDirection.DownLeft = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.DownLeft));
                        series.ChartSmartLabel.ChartNoMoveDirection.DownRight = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.DownRight));
                        series.ChartSmartLabel.ChartNoMoveDirection.Left = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.Left));
                        series.ChartSmartLabel.ChartNoMoveDirection.Right = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.Right));
                        series.ChartSmartLabel.ChartNoMoveDirection.Up = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.Up));
                        series.ChartSmartLabel.ChartNoMoveDirection.UpLeft = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.UpLeft));
                        series.ChartSmartLabel.ChartNoMoveDirection.UpRight = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ChartNoMoveDirection.UpRight));
                    }
                    series.ChartSmartLabel.Disabled = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.Disabled.ToString()));
                    series.ChartSmartLabel.MarkerOverlapping = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.MarkerOverlapping.ToString()));
                    series.ChartSmartLabel.MaxMovingDistance = this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.MaxMovingDistance);
                    series.ChartSmartLabel.MinMovingDistance = this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.MinMovingDistance);
                    series.ChartSmartLabel.ShowOverlapped = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.ChartSmartLabel.ShowOverlapped.ToString()));
                }

                if (item.Legend != null)
                {
                    series.Legend = new ChartItemInLegendExpVal();
                    //series.Legend.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey();
                    series.Legend.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(item.Legend.Hidden));
                    series.Legend.LegendText = this.Model.ExpressionEngine.GetEvalExpressionString(item.Legend.LegendText);
                    series.Legend.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(item.Legend.ToolTip);
                }
                chartAreaExp.ChartSeries.Add(series);
            }
            return chartAreaExp;
        }


        ChartAreaAxisExpVal GetAreaAxis(ChartAreaAxisExp cat)
        {
            ChartAreaAxisExpVal areaAxis = new ChartAreaAxisExpVal();

            areaAxis.AllowLabelRotation = TryEnum<AllowLabelRotation>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.AllowLabelRotation.ToString()));
            areaAxis.Angle = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.Angle.ToString()));
            areaAxis.Arrows = TryEnum<Arrows>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.Arrows.ToString()));
            if (cat.ChartAxisScaleBreak != null)
            {
                areaAxis.ChartAxisScaleBreak = new ChartAxisScaleBreakExpVal();
                areaAxis.ChartAxisScaleBreak.BreakLineType = TryEnum<BreakLineType>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.BreakLineType.ToString()));
                areaAxis.ChartAxisScaleBreak.CollapsibleSpaceThreshold = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.CollapsibleSpaceThreshold.ToString()));
                areaAxis.ChartAxisScaleBreak.Enabled = this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.Enabled);
                areaAxis.ChartAxisScaleBreak.IncludeZero = TryEnum<BooleanOptions>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.IncludeZero.ToString()));
                areaAxis.ChartAxisScaleBreak.MaxNumberOfBreaks = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.MaxNumberOfBreaks.ToString()));
                areaAxis.ChartAxisScaleBreak.Spacing = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisScaleBreak.Spacing.ToString()));
                if (cat.ChartAxisScaleBreak.Style != null)
                    areaAxis.ChartAxisScaleBreak.Style = this.GetBorderStyleExp(cat.ChartAxisScaleBreak.Style);
            }
            if (cat.ChartAxisTitle != null)
            {
                areaAxis.ChartAxisTitle = new ChartAxiesExpVal();
                areaAxis.ChartAxisTitle.Caption = this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisTitle.Caption);
                areaAxis.ChartAxisTitle.Font = this.GetFontExp(cat.ChartAxisTitle.Font);
                areaAxis.ChartAxisTitle.Textalign = TryEnum<TextAlign>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisTitle.Textalign));
                areaAxis.ChartAxisTitle.TextColor = this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisTitle.TextColor);
                areaAxis.ChartAxisTitle.TextDecoration = TryEnum<DOM.TextDecoration>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisTitle.TextDecoration.ToString()));
                areaAxis.ChartAxisTitle.Textorientation = TryEnum<TextOrientation>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.ChartAxisTitle.Textorientation.ToString()));
                if (cat.ChartMajorGridLines != null)
                {
                    areaAxis.ChartMajorGridLines = this.GetChartGridLines(cat.ChartMajorGridLines);
                }
                if (cat.ChartMinorGridLines != null)
                {
                    areaAxis.ChartMinorGridLines = this.GetChartGridLines(cat.ChartMinorGridLines);
                }
                if (cat.ChartMajorTickMarks != null)
                {
                    areaAxis.ChartMajorTickMarks = this.GetChartTickMarks(cat.ChartMajorTickMarks);
                }
                if (cat.ChartMinorTickMarks != null)
                {
                    areaAxis.ChartMinorTickMarks = this.GetChartTickMarks(cat.ChartMinorTickMarks);
                }
            }
            areaAxis.ChartStripLines = new List<ChartStripLineExpVal>();
            if (cat.ChartStripLines != null)
            {
                foreach (ChartStripLineExp stripln in cat.ChartStripLines)
                {
                    ChartStripLineExpVal strip = new ChartStripLineExpVal();
                    //strip.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey();
                    strip.Interval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.Interval));
                    strip.IntervalOffset = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.IntervalOffset));
                    strip.IntervalOffsetType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.IntervalOffsetType.ToString()));
                    strip.IntervalType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.IntervalType.ToString()));
                    strip.StripWidth = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.StripWidth));
                    strip.StripWidthType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.StripWidthType.ToString()));
                    strip.Style = this.GetStyleExp(stripln.Style);
                    strip.TextOrientation = TryEnum<TextOrientation>(this.Model.ExpressionEngine.GetEvalExpressionString(stripln.TextOrientation.ToString()));
                    strip.Title = this.Model.ExpressionEngine.GetEvalExpressionString(stripln.Title);
                    strip.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(stripln.ToolTip);
                    areaAxis.ChartStripLines.Add(strip);
                }
            }
            if (cat.CrossAt != null)
            {
                areaAxis.CrossAt = cat.CrossAt.ToString();
            }
            areaAxis.CustomProperties = new CustomPropertiesExpVal();
            if (cat.CustomProperties != null)
            {
                foreach (CustomPropertyExp property in cat.CustomProperties)
                {
                    CustomPropertyExpVal pro = new CustomPropertyExpVal();
                    pro.Name = this.Model.ExpressionEngine.GetEvalExpressionString(property.Name);
                    pro.Value = property.Value;
                    areaAxis.CustomProperties.Add(pro);
                }
            }
            areaAxis.HideEndLabels = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.HideEndLabels));
            areaAxis.HideLabels = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.HideLabels));
            areaAxis.IncludeZero = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.IncludeZero));
            areaAxis.Interlaced = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.Interlaced));
            areaAxis.InterlacedColor = this.Model.ExpressionEngine.GetEvalExpressionString(cat.InterlacedColor);
            areaAxis.Interval = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Interval);
            areaAxis.IntervalOffset = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.IntervalOffset));
            areaAxis.IntervalOffsetType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.IntervalOffsetType));
            areaAxis.IntervalType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.IntervalType));
            areaAxis.LabelInterval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelInterval));
            areaAxis.LabelIntervalOffset = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelIntervalOffset));
            areaAxis.LabelIntervalOffsetType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelIntervalOffsetType.ToString()));
            areaAxis.LabelIntervalType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelIntervalType.ToString()));
            areaAxis.LabelsAutoFitDisabled = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelsAutoFitDisabled));
            areaAxis.Location = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Location.ToString());
            areaAxis.LogBase = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LogBase));
            areaAxis.LogScale = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.LogScale));
            areaAxis.Margin = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Margin);
            areaAxis.MarksAlwaysAtPlotEdge = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.MarksAlwaysAtPlotEdge));
            areaAxis.MaxFontSize = this.Model.ExpressionEngine.GetEvalExpressionString(cat.MaxFontSize);
            areaAxis.Maximum = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Maximum);
            areaAxis.MinFontSize = this.Model.ExpressionEngine.GetEvalExpressionString(cat.MinFontSize);
            areaAxis.Minimum = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Minimum);
            areaAxis.Name = this.Model.ExpressionEngine.GetEvalExpressionString(cat.Name);
            areaAxis.OffsetLabels = this.Model.ExpressionEngine.GetEvalExpressionString(cat.OffsetLabels);
            areaAxis.PreventFontGrow = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.PreventFontGrow));
            areaAxis.PreventFontShrink = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.PreventFontShrink));
            areaAxis.PreventLabelOffset = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.PreventLabelOffset));
            areaAxis.PreventWordWrap = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.PreventWordWrap));
            areaAxis.Reverse = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.Reverse));
            areaAxis.Scalar = cat.Scalar;
            areaAxis.VariableAutoInterval = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.VariableAutoInterval));
            areaAxis.Visible = TryEnum<BooleanOptions>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.Visible.ToString()));

            if (cat.Style != null)
            {
                areaAxis.Style = this.GetBorderStyleExp(cat.Style);
            }
            areaAxis.LabelColor = this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelColor);
            if (cat.LabelFont != null)
            {
                areaAxis.LabelFont = this.GetFontExp(cat.LabelFont);
            }
            areaAxis.LabelFormat = this.Model.ExpressionEngine.GetEvalExpressionString(cat.LabelFormat);
            areaAxis.TextDecoration = TryEnum<DOM.TextDecoration>(this.Model.ExpressionEngine.GetEvalExpressionString(cat.TextDecoration));

            return areaAxis;
        }

        ChartGridLinesExpVal GetChartGridLines(ChartGridLinesExp chartGridline)
        {
            ChartGridLinesExpVal gridLines = new ChartGridLinesExpVal();
            gridLines.Enabled = TryEnum<BooleanOptions>(this.Model.ExpressionEngine.GetEvalExpressionString(chartGridline.Enabled.ToString()));
            gridLines.Interval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(chartGridline.Interval));
            gridLines.IntervalOffset = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(chartGridline.IntervalOffset));
            gridLines.IntervalOffsetType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(chartGridline.IntervalOffsetType.ToString()));
            gridLines.IntervalType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(chartGridline.IntervalType.ToString()));
            if (chartGridline.Style != null)
            {
                gridLines.Style = this.GetBorderStyleExp(chartGridline.Style);
            }
            return gridLines;
        }

        ChartTickMarksExpVal GetChartTickMarks(ChartTickMarksExp chartTickMark)
        {
            ChartTickMarksExpVal tickMarks = new ChartTickMarksExpVal();

            tickMarks.Enabled = TryEnum<BooleanOptions>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.Enabled.ToString()));
            tickMarks.Interval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString((chartTickMark.Interval)));
            tickMarks.IntervalOffset = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.IntervalOffset));
            tickMarks.IntervalOffsetType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.IntervalOffsetType.ToString()));
            tickMarks.IntervalType = TryEnum<IntervalType>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.IntervalType.ToString()));
            tickMarks.Length = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.Length));
            if (chartTickMark.Style != null)
            {
                tickMarks.Style = this.GetBorderStyleExp(chartTickMark.Style);
            }
            tickMarks.Type = TryEnum<ChartTickMarksType>(this.Model.ExpressionEngine.GetEvalExpressionString(chartTickMark.Type.ToString()));
            return tickMarks;
        }

        ChartLegendExpVal GetChartLegend(ChartLegendExp legend)
        {
            ChartLegendExpVal legendExp = new ChartLegendExpVal();
            if (legend.LegendTile != null)
            {
                legendExp.LegendTile = new ChartLegandTileExpVal();
                legendExp.LegendTile.Caption = legend.LegendTile.Caption;
                legendExp.LegendTile.Font = this.GetFontExp(legend.Style.Font);
                //Not 
                //legendExp.LegendTile.SeperatorColor = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                //legendExp.LegendTile.SeperatorStyle = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                //legendExp.LegendTile.SeperatorStyle = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.Style);
                //legendExp.LegendTile.TitleSeparator = this.Model.ExpressionEngine.GetExpressionKey(legend.ChartLegendTitle.TitleSeparator.ToString());
            }
            legendExp.Name = legend.Name;
            legendExp.Position = TryEnum<DOM.Positions>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.Position.ToString()));
            legendExp.Style = this.GetStyleExp(legend.Style);
            legendExp.TextWrapThreshold = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.TextWrapThreshold));
            legendExp.AutoFitTextDisabled = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.AutoFitTextDisabled));
            legendExp.ColumnSeparator = this.Model.ExpressionEngine.GetEvalExpressionString(legend.ColumnSeparator.ToString());
            legendExp.ColumnSeparatorColor = this.Model.ExpressionEngine.GetEvalExpressionString(legend.ColumnSeparatorColor);
            legendExp.ColumnSpacing = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.ColumnSpacing));
            legendExp.DockOutsideChartArea = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.DockOutsideChartArea));
            legendExp.DockToChartArea = legend.DockToChartArea;
            legendExp.EquallySpacedItems = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.EquallySpacedItems));
            legendExp.HeaderSeparator = this.Model.ExpressionEngine.GetEvalExpressionString(legend.HeaderSeparator.ToString());
            legendExp.HeaderSeparatorColor = this.Model.ExpressionEngine.GetEvalExpressionString(legend.HeaderSeparatorColor);
            legendExp.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.Hidden));
            legendExp.InterlacedRows = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.InterlacedRows));
            legendExp.InterlacedRowsColor = this.Model.ExpressionEngine.GetEvalExpressionString(legend.InterlacedRowsColor);
            legendExp.Layout = this.Model.ExpressionEngine.GetEvalExpressionString(legend.Layout.ToString());
            legendExp.MaxAutoSize = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(legend.MaxAutoSize));
            legendExp.MinFontSize = this.Model.ExpressionEngine.GetEvalExpressionString(legend.MinFontSize);
            legendExp.Reversed = this.Model.ExpressionEngine.GetEvalExpressionString(legend.Reversed.ToString());
            return legendExp;
        }


        ChartTileExpVal GetChartTile(ChartTileExp title)
        {
            ChartTileExpVal tileExp = new ChartTileExpVal();
            //tileExp.ActionInfo = this.Model.ExpressionEngine.GetExpressionKey(title.ActionInfo.Actions.First());
            tileExp.Caption = this.Model.ExpressionEngine.GetEvalExpressionString(title.Caption);
            tileExp.Name =this.Model.ExpressionEngine.GetEvalExpressionString(title.Name);
            tileExp.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString((title.Hidden)));
            tileExp.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(title.ToolTip);
            tileExp.Style = this.GetStyleExp(title.Style);
            tileExp.DocToChartArea = title.DocToChartArea;
            tileExp.DockOutChartArea = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(title.DockOutChartArea));
            tileExp.DockOffset = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(title.DockOffset.ToString()));
            tileExp.Position = TryEnum<DOM.Position>(this.Model.ExpressionEngine.GetEvalExpressionString(title.Position.ToString()));
            tileExp.TextOrientation = TryEnum<DOM.TextOrientation>(this.Model.ExpressionEngine.GetEvalExpressionString(title.TextOerientation.ToString()));
            return tileExp;
        }

        TileStyleExpVal GetStyleExp(TileStyleExp style)
        {
            TileStyleExpVal styleExp = new TileStyleExpVal();
            if (style.BackgroundImage != null)
            {
                styleExp.BackgroundImage = this.GetImage(style.BackgroundImage);
            }
            if (style.Border != null)
            {
                styleExp.Border = this.GetBorderStyleExp(style.Border);
            }
            styleExp.FillStyle = this.GetFillStyle(style.FillStyle);
            styleExp.ShadowColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.ShadowColor);
            styleExp.ShadowOffset = this.Model.ExpressionEngine.GetEvalExpressionString(style.ShadowOffset);
            styleExp.Color = this.Model.ExpressionEngine.GetEvalExpressionString(style.Color);
            styleExp.TextDecoration = TryEnum<DOM.TextDecoration>(this.Model.ExpressionEngine.GetEvalExpressionString(style.TextDecoration));
            styleExp.TextEffect = TryEnum<DOM.TextEffects>(this.Model.ExpressionEngine.GetEvalExpressionString(style.TextEffect.ToString()));
            styleExp.Font = this.GetFontExp(style.Font);
            return styleExp;
        }

        BorderStyleExpVal GetBorderStyleExp(BorderStyleExp border)
        {
            BorderStyleExpVal borderStyle = new BorderStyleExpVal();
            borderStyle.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(border.BorderColor);
            borderStyle.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetEvalExpressionString((border.BorderStyle)));
            borderStyle.BorderWidth = this.Model.ExpressionEngine.GetEvalExpressionString(border.BorderWidth);
            return borderStyle;
        }


        BorderSideExpVal GetEdgesBorder(BorderSideExp style)
        {
            bool isCheck = true;
            BorderSideExpVal borderEdg = new BorderSideExpVal();
            BorderStyleExp border = new BorderStyleExp();
            if (style.Left != null)
            {
                border.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.Left.BorderColor);
                border.BorderStyle = this.Model.ExpressionEngine.GetEvalExpressionString(style.Left.BorderStyle);
                border.BorderWidth = this.Model.ExpressionEngine.GetEvalExpressionString(style.Left.BorderWidth);
                borderEdg.Left = this.GetBorderStyleExp(border);
                isCheck = false;
            }
            if (style.Right != null)
            {
                border.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.Top.BorderColor);
                border.BorderStyle = this.Model.ExpressionEngine.GetEvalExpressionString(style.Top.BorderStyle);
                border.BorderWidth = this.Model.ExpressionEngine.GetEvalExpressionString(style.Top.BorderWidth);
                borderEdg.Top = this.GetBorderStyleExp(border);
                isCheck = false;
            }
            if (style.Bottem != null)
            {
                border.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.Bottem.BorderColor);
                border.BorderStyle = this.Model.ExpressionEngine.GetEvalExpressionString(style.Bottem.BorderStyle);
                border.BorderWidth = this.Model.ExpressionEngine.GetEvalExpressionString(style.Bottem.BorderWidth);
                borderEdg.Bottem = this.GetBorderStyleExp(border);
                isCheck = false;
            }
            if (style.Right != null)
            {
                border.BorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.Right.BorderColor);
                border.BorderStyle = this.Model.ExpressionEngine.GetEvalExpressionString(style.Right.BorderStyle);
                border.BorderWidth = this.Model.ExpressionEngine.GetEvalExpressionString(style.Right.BorderWidth);
                borderEdg.Right = this.GetBorderStyleExp(border);
                isCheck = false;
            }
            if (!isCheck)
                return null;
            return borderEdg;
        }

        BackGroundImageExpVal GetImage(BackGroundImageExp style)
        {
            BackGroundImageExpVal imageExp = new BackGroundImageExpVal();
            imageExp.BackgroundRepeat = this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundRepeat);
            imageExp.MimeType = this.Model.ExpressionEngine.GetEvalExpressionString(style.MimeType);
            imageExp.Position = TryEnum<DOM.Position>(this.Model.ExpressionEngine.GetEvalExpressionString(style.Position.ToString()));
            imageExp.Source = TryEnum<DOM.Source>(this.Model.ExpressionEngine.GetEvalExpressionString(style.Source.ToString()));
            imageExp.Value = this.Model.ExpressionEngine.GetEvalExpressionString(style.Value);
            imageExp.TransparentColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.TransparentColor);
            return imageExp;
        }

        FontExpVal GetFontExp(FontExp style)
        {
            FontExpVal font = new FontExpVal();
            font.FontFamily = this.Model.ExpressionEngine.GetEvalExpressionString(style.FontFamily);
            font.FontSize = this.Model.ExpressionEngine.GetEvalExpressionString(style.FontSize);
            font.FontStyle = TryEnum<DOM.FontStyle>(this.Model.ExpressionEngine.GetEvalExpressionString(style.FontStyle.ToString()));
            font.FontWeight = TryEnum<DOM.FontWeight>(this.Model.ExpressionEngine.GetEvalExpressionString(style.FontWeight.ToString()));
            return font;
        }

        FillStyleExpVal GetFillStyle(FillStyleExp style)
        {
            FillStyleExpVal fillStyle = new FillStyleExpVal();
            fillStyle.BackgroundColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundColor);
            fillStyle.BackgroundGradientEndcolor = this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundGradientEndcolor);
            fillStyle.BackgroundGradientType = TryEnum<DOM.BackgroundGradientTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundGradientType.ToString()));
            fillStyle.BackgroundHatchType = TryEnum<DOM.BackgroundHatchTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundHatchType.ToString()));
            return fillStyle;
        }

        private string GetExpression(string value)
        {
            if (this.IsTablixChild && value != null && value.Contains("RowNumber("))
            {
                this.HasRowNumber = true;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName, true);
            this.AddDataFields(key);
            return key;
        }

        private string GetExpressionKey(string value)
        {
            if (this.IsTablixChild && value != null && value.Contains("RowNumber("))
            {
                this.HasRowNumber = true;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName, this.IsTablixChild);
            this.AddDataFields(key);
            return key;
        }

        private TextboxActionInfoExp GetActionInfoExp(ActionInfo actionInfo, bool isChild)
        {
            try
            {
                if (actionInfo != null)
                {
                    TextboxActionInfoExp ActionInfo = new TextboxActionInfoExp();
                    foreach (RDL.DOM.Action action in actionInfo.Actions)
                    {
                        ActionInfo.BookmarkLink = isChild ? this.GetExpression(action.BookmarkLink) : this.GetExpressionKey(action.BookmarkLink);
                        ActionInfo.Hyperlink = isChild ? this.GetExpression(action.Hyperlink) : this.GetExpressionKey(action.Hyperlink);
                        if (action.Drillthrough != null)
                        {
                            ActionInfo.ReportName = isChild ? this.GetExpression(action.Drillthrough.ReportName) : this.GetExpressionKey(action.Drillthrough.ReportName);
                            ActionInfo.Parameters = new List<TextboxParameterExp>();

                            foreach (Parameter parameters in action.Drillthrough.Parameters)
                            {
                                TextboxParameterExp Parameter = new TextboxParameterExp();
                                Parameter.Name = isChild ? this.GetExpression(parameters.Name) : this.GetExpressionKey(parameters.Name);
                                Parameter.Omit = isChild ? this.GetExpression(parameters.Omit) : this.GetExpressionKey(parameters.Omit);
                                Parameter.Value = isChild ? this.GetExpression(parameters.Value) : this.GetExpressionKey(parameters.Value);
                                ActionInfo.Parameters.Add(Parameter);
                            }
                        }
                    }

                    return ActionInfo;
                }

            }
            catch
            {

            }
            return null;
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType=ModelType.ChartModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualHeight = this.Hidden ? 0 : this.Height;
        }

        public override IReportItemModeler GetModel()
        {
            ChartModel itemModel = new ChartModel();
            itemModel.chartExpProp = this.chartExpProp;
            itemModel.ReportItem = this.ReportItem;
            itemModel.CanGrow = this.CanGrow;
            itemModel.SeriesFields = this.SeriesFields;
            itemModel.CategoryFiels = this.CategoryFiels;
            itemModel.ReportItemModelers = this.ReportItemModelers;
            itemModel.IsSubReportChild = this.IsSubReportChild;
            itemModel.IsTablixChild = this.IsTablixChild;
            itemModel.IsTablixInnerChild = this.IsTablixInnerChild;
            itemModel.ContainerModel = this.ContainerModel;
            itemModel.CategoryFiels = this.CategoryFiels;
            itemModel.DataFields = this.DataFields;
            itemModel.DataSetFields = this.DataSetFields;
            itemModel.DataSetName = this.DataSetName;
            itemModel.DataSource = this.DataSource;
            itemModel.Engine = this.Engine;
            itemModel.ExpFilters = this.ExpFilters;
            itemModel.KeepTogether = this.KeepTogether;
            itemModel.Height = this.Height;
            itemModel.Name = this.Name;
            itemModel.Top = this.Top;
            itemModel.Left = this.Left;
            itemModel.Width = this.Width;
            itemModel.Height = this.Height;
            itemModel.Width = this.Width;
            itemModel.Model = this.Model;
            itemModel.ModelType = this.ModelType;
            return itemModel;
        }

        public override void DisposeEvalObjects()
        {
            this.ChartProperties = null;
            this.Engine = null;
            base.DisposeEvalObjects();
        }

        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;
            this.DataSetFields = null;
            this.DataSource = null;
            this.FieldValues = null;
            this.Engine = null;

            if (this.IsTablixInnerChild)
            {
                this.FlowLayoutInfo = null;
            }
            base.DisposeReportItemObj();
        }

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualWidth = this.Hidden ? 0 : this.Width;
        }

        public override void UpdatePageNo(int pageNo)
        {
            if (this.DocumentNodeRefer != null)
            {
                this.DocumentNodeRefer.PageNo=pageNo + 1;
                this.DocumentNodeRefer.TopPos = this.Top;
                this.DocumentNodeRefer.LeftPos = this.Left;
                this.DocumentNodeRefer = null;
            }
        }

        #endregion

        private string[] ParsedFieldName(string fieldNameContainer)
        {
            string[] values = { fieldNameContainer, string.Empty };
            Regex pattern = new Regex(@"(?<Functions>(\w*\(+)*)(?<Matches>(\w)+\!(.*?)+\.(\w)+)(?<Operators>[\W*]*)");
            string fieldPttern = @"\!(.*?)+\.";
            if (pattern.IsMatch(fieldNameContainer))
            {
                values[0] = Regex.Match(fieldNameContainer, fieldPttern).Value.ToString().TrimStart('!').TrimEnd('.');
                values[1] = "";
                if (fieldNameContainer.IndexOf("(") > -1)
                    values[1] = Regex.Match(fieldNameContainer, @"\=(.*?)+\(").Value.ToString().TrimStart('=').TrimEnd('(');
            }
            return values;
        }

        internal string ParseFieldName(string fieldNameContainer)
        {
            string parsedFieldName = fieldNameContainer;
            if (fieldNameContainer.Contains("!") && fieldNameContainer.Contains("."))
            {
                int indexOfExclamatory = fieldNameContainer.IndexOf('!');
                int indexOfDot = fieldNameContainer.IndexOf('.');
                int stringLength = (indexOfDot - indexOfExclamatory) - 1;
                int startingIndex = indexOfExclamatory + 1;

                parsedFieldName = fieldNameContainer.Substring(startingIndex, stringLength);
            }

            return parsedFieldName;
        }

        internal string GetFieldName(string column, ReportDefinition report, string DataSetName)
        {
            string fieldValue = (from dataSetThis in report.DataSets
                                 from DataSetfield in dataSetThis.Fields
                                 where DataSetfield.Name == column
                                 where dataSetThis.Name == DataSetName
                                 select DataSetfield.DataField).FirstOrDefault();

            if (fieldValue == null)
            {
                return column;
            }

            return fieldValue;
        }

        internal void DataProcessingChart()
        {
            this.Engine = new ChartEngine();

            object dataSource=null;

            if (this.DataSource == null)
            {
                dataSource= (from view in this.Model.ProcessedData.DataSourceObjects
                              where view.Key == ChartProperties.DataSetName
                              select view.Value).SingleOrDefault();
            }
            else
            {
                dataSource = this.DataSource;
            }

            if (dataSource != null)
            {
                foreach (string dataFiled in DataFields)
                {
                    string[] values = this.ParsedFieldName(dataFiled);
                    string columnName = values[0];
                    string computationTypeValue = values[1];
                    ChartComputationInfo chartInfo = new ChartComputationInfo() { FieldName = columnName, FieldMappingName = this.GetFieldName(columnName, this.Model.Report, ChartProperties.DataSetName) };
                    chartInfo.ComputationType = ComputationType.DoubleTotalSum;
                    if (computationTypeValue.Equals("avg", StringComparison.CurrentCultureIgnoreCase))
                    {
                        chartInfo.ComputationType = ComputationType.DoubleAverage;
                    }
                    else if (computationTypeValue.Equals("count", StringComparison.CurrentCultureIgnoreCase))
                    {
                        chartInfo.ComputationType = ComputationType.Count;
                    }
                    this.Engine.ChartDataFields.Add(chartInfo);
                }

                if (SeriesFields != null)
                {
                    foreach (string field in SeriesFields)
                    {
                        string columnName = this.ParseFieldName(field);
                        this.Engine.SeriesFields.Add(new ChartItem() { FieldMappingName = this.GetFieldName(columnName, this.Model.Report, this.DataSetName) });
                    }
                }

                foreach (string field in CategoryFiels)
                {
                    string columnName = this.ParseFieldName(field);
                    this.Engine.CategoryFields.Add(new ChartItem() { FieldMappingName = this.GetFieldName(columnName, this.Model.Report, this.DataSetName) });
                }

                if(this.ActionInfo != null && this.ActionInfo.Count > 0)
                {
                    foreach (var item in this.ActionInfo)
                    {
                        ChartDrillAction action = new ChartDrillAction();
                        if(!string.IsNullOrEmpty(item.Value.BookmarkLink))
                        {
                            var rval = this.ParseFieldName(item.Value.BookmarkLink);
                            action.BookmarkLink = new ChartItem() { FieldMappingName = this.GetFieldName(rval, this.Model.Report, this.DataSetName) };
                        }
                        else if(!string.IsNullOrEmpty(item.Value.Hyperlink))
                        {
                            var rval = this.ParseFieldName(item.Value.Hyperlink);
                            action.Hyperlink = new ChartItem() { FieldMappingName = this.GetFieldName(rval, this.Model.Report, this.DataSetName) };
                        }
                        else if(item.Value.Drillthrough != null && !string.IsNullOrEmpty(item.Value.Drillthrough.ReportName))
                        {
                            action.DrillThrough = new ChartDrillThrough();
                            var rname = this.ParseFieldName(item.Value.Drillthrough.ReportName);
                            action.DrillThrough.ReportName = new ChartItem() { FieldMappingName = this.GetFieldName(rname, this.Model.Report, this.DataSetName) };
                            if (item.Value.Drillthrough.Parameters != null && item.Value.Drillthrough.Parameters.Count > 0)
                            {
                                action.DrillThrough.Parameters = new List<ChartDrillParameter>();
                                foreach (var para in item.Value.Drillthrough.Parameters)
                                {
                                    ChartDrillParameter par = new ChartDrillParameter();
                                    var pname = this.ParseFieldName(para.Name);
                                    var pvalue = this.ParseFieldName(para.Value);
                                    par.Name = new ChartItem() { FieldMappingName = this.GetFieldName(pname, this.Model.Report, this.DataSetName) };
                                    par.Value = new ChartItem() { FieldMappingName = this.GetFieldName(pvalue, this.Model.Report, this.DataSetName) };
                                    action.DrillThrough.Parameters.Add(par);
                                }
                            }
                        }

                        if(Engine.ActionInfo == null)
                        {
                            Engine.ActionInfo = new Dictionary<int, ChartDrillAction>();
                        }
                        Engine.ActionInfo.Add(item.Key, action);
                    }
                }

                if (this.ChartProperties.Type == VisualizationType.Shape)
                    this.Engine.EngineType = ChartEngineType.Shape;
            
                this.Engine.DataSource = this.Model.ProcessedData.FilterItemSoruce(dataSource as IEnumerable, this.ExpFilters);
            
                this.Engine.Populate();
            }
        }

        internal class RectPonit
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }            
        }

        internal class CirclePonit
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Radius { get; set; }
        }

        internal class LinePoint
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
        }

        internal class SegmentActionInfo
        {
            public List<List<PointXY>> PathPoints { get; set; }
            public CirclePonit CirclePonits { get; set; }
            public RectPonit RectPonits { get; set; }
            public LinePoint LinePonits { get; set; }
            public TextboxActionInfoExpVal ActionInfo { get; set; }
        }

        #region DataType Convert Wapper Method

        // clr generic datatype converter 
        public T TryParse<T>(object value)
        {
            try
            {
                if (value == null)
                {
                    return default(T);
                }
                T retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                return retValue;
            }
            catch
            {
                return default(T);
            }
        }

        // common enum type converter
        public T TryEnum<T>(string enumMember)
        {
            try
            {
                if (string.IsNullOrEmpty(enumMember))
                {
                    return default(T);
                }
                T value = (T)Enum.Parse(typeof(T), enumMember, true);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        #endregion
    }
}