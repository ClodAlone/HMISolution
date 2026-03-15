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
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Chart;

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Chart.Silverlight.dll.Design.Metadata))]
#endif

namespace Syncfusion.Chart.Silverlight.dll.Design
{
#if SyncfusionFramework3_5
    internal class Metadata : IRegisterMetadata
    {
    #region IRegisterMetadata Members

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
            builder.AddCallback(typeof(ZoomingScrollBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ZoomingToolKit), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

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

            builder.AddCallback(typeof(HeatMapItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HorizontalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(VerticalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SquarifiedHeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false))); 
            builder.AddCallback(typeof(AnnotationPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HeatMapItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HorizontalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(VerticalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SquarifiedHeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
   	        builder.AddCallback(typeof(LegendEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(IndicatorPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ContextMenuAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ContextMenuItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SeparatorAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            MetadataStore.AddAttributeTable(builder.CreateTable());

        }

        

    #endregion
    }
#elif SyncfusionFramework4_0

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCustomAttributes(typeof(ChartArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UniformChartGrid), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(SeriesPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartSeries), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DataAxis), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ZoomingScrollBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ZoomingToolKit), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ChartAxesGridLinesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAxesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAxisElementPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAxisHeaderPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SmallTickLinesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StripLinePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LegendItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AnnotationPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TickLinesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAreaWatermarkControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartScaleBreakPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartScaleBreakPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SparklinePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InteractiveCursor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SyncAreasPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(HeatMapItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VerticalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AnnotationPresenter), new ToolboxBrowsableAttribute(false));  
                builder.AddCallback(typeof(HeatMapItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HorizontalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(VerticalSlicesPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SquarifiedHeatMapsPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(IndicatorPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ContextMenuAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ContextMenuItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SeparatorAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

   	 builder.AddCallback(typeof(LegendEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                return builder.CreateTable();
            }
        }

    #endregion
    }

#endif
}
