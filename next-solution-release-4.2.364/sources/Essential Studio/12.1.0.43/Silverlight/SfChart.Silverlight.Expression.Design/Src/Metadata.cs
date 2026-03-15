#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Media;
using System.Windows.Controls;
using Microsoft.Windows.Design;
using Syncfusion.UI.Xaml.Charts;

[assembly: ProvideMetadata(typeof(Syncfusion.SfChart.Silverlight.Expression.Design.Metadata))]

namespace Syncfusion.SfChart.Silverlight.Expression.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering
                builder.AddCustomAttributes(typeof(ChartStripLine), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AccumulationDistributionIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AreaSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AverageTrueRangeIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BarSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BollingerBandIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BubbleSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CandleSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CategoryAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAdornmentContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAdornmentPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAxisLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartRootPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartSeriesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColumnSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DateTimeAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DoughnutSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ErrorBarSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExponentialAverageIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastColumnBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastHiLoOpenCloseBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastHiLoBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastLineBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastLineSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FunnelSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HiLoOpenCloseSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HiLoSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HistogramSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LogarithmicAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MACDTechnicalIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MomentumTechnicalIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NumericalAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PieSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PolarSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PyramidSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RSITechnicalIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RadarSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeAreaSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeColumnSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResizableScrollBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SfChartResizableBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScatterSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SimpleAverageIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplineAreaSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplineSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingAreaSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingBarSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingColumnSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingArea100Series), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingBar100Series), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingColumn100Series), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StepAreaSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StepLineSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StochasticTechnicalIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimeSpanAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Trendline), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TriangularAverageIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastBarBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastCandleBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastScatterBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastStepLineBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastStackingColumnBitmapSeries), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Resizer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeNavigatorPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartTooltip), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartTrackBallControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DateTimeCategoryAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Watermark), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EllipseAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalLineAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ImageAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RectangleAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TextAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VerticalLineAnnotation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PieSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NumericalAxis3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LogarithmicAxis3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DoughnutSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColumnSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BarSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingBar100Series3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingBarSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingColumnSeries3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StackingColumn100Series3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CategoryAxis3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DateTimeAxis3D), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TimeSpanAxis3D), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TrendlineBase), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(ChartTrendlinePanel), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();

            }
        }

        #endregion
    }
}
