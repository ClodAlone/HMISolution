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
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Chart;


#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
[assembly:ProvideMetadata(typeof(Syncfusion.Chart.Wpf.dll.Design.Metadata))]
#endif

namespace Syncfusion.Chart.Wpf.dll.Design
{
#if SyncfusionFramework4_5 || SyncfusionFramework4_5_1
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    internal class Metadata : IProvideAttributeTable
    {

    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering

                builder.AddCustomAttributes(typeof(ChartAreaPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPrintButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ChartToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAreaContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegendEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAnnotationLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartStripLine), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(VerticalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeatMapItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastColumnPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastLinePresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastScatterPresenter), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(InteractiveCursor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter.ChartSegmentPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter.ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniformWrapPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CartesianAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PropertyWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter.IndicatorPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartBreakRange), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartWatermarkElement), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();

            }
        }

        #endregion
    }
#elif SyncfusionFramework4_0
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    internal class Metadata : IProvideAttributeTable
    {

    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering

                builder.AddCustomAttributes(typeof(ChartAreaPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPrintButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ChartToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAreaContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartLegendEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAnnotationLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartStripLine), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(VerticalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeatMapItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastColumnPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastLinePresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FastScatterPresenter), new ToolboxBrowsableAttribute(false));                
                //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(InteractiveCursor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter.ChartSegmentPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter.ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniformWrapPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CartesianAxis3D), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PropertyWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter.IndicatorPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartBreakRange), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartWatermarkElement), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();

            }
        }

    #endregion
    }
#else
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering

            builder.AddCustomAttributes(typeof(ChartAreaPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartSeriesPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPolarPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPrintButton), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(ChartToolBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ToolBarItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartCartesianPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartArea), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAreaContextMenu), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartLegend), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartDockPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartLegendEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartPrintDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAnnotationLabel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartStripLine), new ToolboxBrowsableAttribute(false));



            builder.AddCustomAttributes(typeof(VerticalSlicesPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HeatMapItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HeatMapsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartGrid), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(ChartPropertiesView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PropertyWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FastColumnPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FastLinePresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FastScatterPresenter), new ToolboxBrowsableAttribute(false));                
            //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
            //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(InteractiveCursor), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartSeriesPresenter.ChartSegmentPresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter.ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(UniformWrapPanel), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(CartesianAxis3D), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartSeriesPresenter.IndicatorPresenter), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartBreakRange), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(ChartWatermarkElement), ToolboxBrowsableAttribute.No);
            builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#endif
}
