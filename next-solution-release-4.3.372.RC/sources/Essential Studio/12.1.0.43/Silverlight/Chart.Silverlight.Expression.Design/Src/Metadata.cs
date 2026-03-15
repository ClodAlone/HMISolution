#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Chart;
#if SyncfusionFramework3_5
using Microsoft.Windows.Design.Features;

public class Metadata : IRegisterMetadata
{

    public void Register()
    {
        AttributeTableBuilder builder = new AttributeTableBuilder();
        builder.AddCallback(typeof(ChartArea), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartAxis), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(UniformChartGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        
        builder.AddCallback(typeof(SeriesPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartLegend), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartSeries), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        
        builder.AddCallback(typeof(DataAxis), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ZoomingToolKit), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ZoomingScrollBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

        builder.AddCallback(typeof(ChartAxesGridLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartAxesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartAxisElementPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartAxisHeaderPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(SmallTickLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(StripLinePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(LegendItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(AnnotationPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(TickLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartDockPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartAreaWatermarkControl), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(SparklinePanel), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartScaleBreakPanel), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartScaleBreakPresenter), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(InteractiveCursor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(SyncAreasPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(ChartPrintDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

        builder.AddCallback(typeof(HeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(HorizontalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(VerticalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(SquarifiedHeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        builder.AddCallback(typeof(IndicatorPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

        MetadataStore.AddAttributeTable(builder.CreateTable());
    }
}
#elif SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Chart.Silverlight.Expression.Design.Metadata))]
namespace Syncfusion.Chart.Silverlight.Expression.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                ChartControlsAttributeTableBuilder builder = new ChartControlsAttributeTableBuilder();
                builder.AddCallback(typeof(Syncfusion.Windows.Chart.Chart), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(ChartArea), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Windows.Chart.AreasCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                
                builder.AddCallback(typeof(ChartAxis), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartPoint), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(ChartSeries), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SeriesPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DataAxis), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(AnnotationPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                
                builder.AddCallback(typeof(ZoomingScrollBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ZoomingToolKit), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartLegend), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(UniformChartGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(ChartAxesGridLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartAxesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartAxisElementPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartAxisHeaderPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SmallTickLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(StripLinePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(LegendItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(AnnotationPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TickLinesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartDockPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartAreaWatermarkControl), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SparklinePanel), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartScaleBreakPanel), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartScaleBreakPresenter), b=> b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(InteractiveCursor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SyncAreasPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ChartPrintDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(HeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HorizontalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(VerticalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SquarifiedHeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(IndicatorPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
               
                return builder.CreateTable();
            }
        }
    }
}
#endif
