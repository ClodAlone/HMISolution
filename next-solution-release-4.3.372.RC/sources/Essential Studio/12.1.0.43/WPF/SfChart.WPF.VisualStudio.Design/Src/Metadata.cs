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

[assembly: ProvideMetadata(typeof(Syncfusion.SfChart.WPF.VisualStudio.Design.Metadata))]

namespace Syncfusion.SfChart.WPF.VisualStudio.Design
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
                builder.AddCustomAttributes(typeof(ChartStripLine), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AccumulationDistributionIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AreaSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AverageTrueRangeIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BarSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BollingerBandIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BubbleSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CandleSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CategoryAxis), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAxisLayoutPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartDockPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegend), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintDialog), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartRootPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ColumnSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DateTimeAxis), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DoughnutSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ExponentialAverageIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ErrorBarSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastColumnBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastHiLoOpenCloseBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastHiLoBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastLineBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastLineSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FunnelSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HiLoOpenCloseSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HiLoSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HistogramSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LineSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LogarithmicAxis), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MACDTechnicalIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MomentumTechnicalIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(NumericalAxis), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PieSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PolarSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PyramidSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RSITechnicalIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RadarSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeAreaSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeColumnSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ResizableScrollBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfChartResizableBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ScatterSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SimpleAverageIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SplineAreaSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SplineSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingAreaSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingBarSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingColumnSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingArea100Series), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingBar100Series), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingColumn100Series), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StepAreaSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StepLineSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StochasticTechnicalIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SymbolControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Trendline), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TimeSpanAxis), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TriangularAverageIndicator), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartTrackBallControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastBarBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastScatterBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastCandleBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastStepLineBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastStackingColumnBitmapSeries), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Resizer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeNavigatorPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartTooltip), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(DateTimeCategoryAxis), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(Watermark), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(EllipseAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(HorizontalLineAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(ImageAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(LineAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(RectangleAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(TextAnnotation), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(VerticalLineAnnotation), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PieSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(NumericalAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LogarithmicAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DoughnutSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ColumnSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BarSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingBar100Series3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingBarSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingColumnSeries3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(StackingColumn100Series3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CategoryAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DateTimeAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TimeSpanAxis3D), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(TrendlineBase), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(ChartTrendlinePanel), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();

            }
        }

        #endregion
    }
}
