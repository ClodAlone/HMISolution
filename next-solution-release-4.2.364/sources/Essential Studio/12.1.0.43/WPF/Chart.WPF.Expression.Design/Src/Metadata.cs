#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using Microsoft.Windows.Design.Features;
using Syncfusion.Chart.Wpf.Expression.Design.SmartTagSupportingFiles.AdornerProviders;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Design;


#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
[assembly: ProvideMetadata(typeof(Syncfusion.Chart.Wpf.Expression.Design.Metadata))]
#endif

namespace Syncfusion.Chart.Wpf.Expression.Design
{
    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>

#if SyncfusionFramework4_5 || SyncfusionFramework4_5_1
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(ChartAreaPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartArea), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartStripLine), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(ChartAreaContextMenu), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegend), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartDockPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegendEditor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintDialog), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(VerticalSlicesPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeatMapItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeatMapsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAnnotationLabel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastColumnPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastLinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastScatterPresenter), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter.ChartSegmentPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter.ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PropertyWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartSeriesPresenter.IndicatorPresenter), ToolboxBrowsableAttribute.No);

                //Sets the chart attributes.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartInitializer)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartAdornerProvider)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(ChartAreaAdornerProvider)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(ChartLegendAdornerProvider)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                //Sets the area attributes.
                //builder.AddCustomAttributes(typeof(ChartArea), "VerticalScrollingAxisProperty",
                //new BrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartArea), "PrimaryAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
                builder.AddCustomAttributes(typeof(ChartArea), "SecondaryAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));

                //Sets the series attributes.
                builder.AddCustomAttributes(typeof(ChartSeries), "XAxisProperty",
      PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
                builder.AddCustomAttributes(typeof(ChartSeries), "YAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));

                return builder.CreateTable();

            }
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

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(ChartAreaPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartSeriesPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartArea), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartStripLine), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(ChartAreaContextMenu), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegend), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartDockPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartLegendEditor), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartPrintDialog), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(VerticalSlicesPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeatMapItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(HeatMapsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAnnotationLabel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastColumnPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastLinePresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FastScatterPresenter), ToolboxBrowsableAttribute.No);
                  //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
                  //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartSeriesPresenter.ChartSegmentPresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter.ChartAdornmentContainer), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartPropertiesView), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(PropertyWindow), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
                  builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartSeriesPresenter.IndicatorPresenter),ToolboxBrowsableAttribute.No);

                //Sets the chart attributes.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartInitializer)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartAdornerProvider)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(ChartAreaAdornerProvider)));
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(ChartLegendAdornerProvider)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                //Sets the area attributes.
                //builder.AddCustomAttributes(typeof(ChartArea), "VerticalScrollingAxisProperty",
       //new BrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChartArea), "PrimaryAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
                builder.AddCustomAttributes(typeof(ChartArea), "SecondaryAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));

                //Sets the series attributes.
                builder.AddCustomAttributes(typeof(ChartSeries),"XAxisProperty",
      PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
                builder.AddCustomAttributes(typeof(ChartSeries),"YAxisProperty",
                  PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));

                return builder.CreateTable();

            }
        }

        #endregion
    }
#elif SyncfusionFramework3_5
    internal class Metadata : IRegisterMetadata
  {
    #region IRegisterMetadata Members
    /// <summary>
    /// Attaches design-time metadata to a particular control type.
    /// </summary>
    public void Register()
    {
      AttributeTableBuilder builder = new AttributeTableBuilder();
      ProceedToolboxHiddenElements(builder);
      SetChartAttributes(builder);
      SetAreaAttributes(builder);
      SetSeriesAttributes(builder);

      MetadataStore.AddAttributeTable(builder.CreateTable());
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Proceeds the toolbox hidden elements.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private void ProceedToolboxHiddenElements(AttributeTableBuilder builder)
    {
        builder.AddCustomAttributes(typeof(ChartAreaPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartSeriesPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartCartesianAxisElement), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartCartesianAxisPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPolarAxisLabelsPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPolarPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPrintButton), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartCartesianAxisLabelsPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartCartesianPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartCartesianAreaGrid), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartZoomingScrollBar), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartArea), ToolboxBrowsableAttribute.No);

        builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartStripLine), ToolboxBrowsableAttribute.No);

        builder.AddCustomAttributes(typeof(ChartAreaContextMenu), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartLegend), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartDockPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartLegendEditor), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPolarAreaGrid), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPrintDialog), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(VerticalSlicesPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(HorizontalSlicesPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(HeatMapItem), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(HeatMapsPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(SquarifiedHeatMapsPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartGrid), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAnnotationLabel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAdornmentsPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAnnotationsPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartToolBar), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ToolBarItem), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(FastColumnPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(FastLinePresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(FastScatterPresenter), ToolboxBrowsableAttribute.No);
        //builder.AddCustomAttributes(typeof(ChartFastColumnPresenter), ToolboxBrowsableAttribute.No);
        //builder.AddCustomAttributes(typeof(ChartFastScatterPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartFastHiLoOpenClosePresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartFastStackingColumnPresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(SyncAreasPanel), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPrintingAdorner), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartSeriesCanvas), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartZoomingToolkit), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartPropertiesView), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAreaPropertiesView), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartSeriesPropertiesView), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartAxisPropertiesView), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(ChartLegendPropertiesView), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(PropertyWindow), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(LinePresenter), ToolboxBrowsableAttribute.No);
       builder.AddCustomAttributes(typeof(FastSplinePresenter), ToolboxBrowsableAttribute.No);
        builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartSeriesPresenter.IndicatorPresenter), ToolboxBrowsableAttribute.No);


    }
    /// <summary>
    /// Sets the chart attributes.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private void SetChartAttributes(AttributeTableBuilder builder)
    {
      builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartInitializer)));
      builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(ChartAdornerProvider)));
      builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(ChartAreaAdornerProvider)));
      builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(ChartLegendAdornerProvider)));
        builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Chart), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
        builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartArea), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
        builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.ChartLegend), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

      
    }
    /// <summary>
    /// Sets the area attributes.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private void SetAreaAttributes(AttributeTableBuilder builder)
    {
      //builder.AddCustomAttributes(typeof(ChartArea), ChartArea.VerticalScrollingAxisProperty,
        //new BrowsableAttribute(false));
      builder.AddCustomAttributes(typeof(ChartArea), ChartArea.PrimaryAxisProperty,
        PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
      builder.AddCustomAttributes(typeof(ChartArea), ChartArea.SecondaryAxisProperty,
        PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
    }
    /// <summary>
    /// Sets the series attributes.
    /// </summary>
    /// <param name="builder">The builder.</param>
    private void SetSeriesAttributes(AttributeTableBuilder builder)
    {
      builder.AddCustomAttributes(typeof(ChartSeries), ChartSeries.XAxisProperty,
        PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
      builder.AddCustomAttributes(typeof(ChartSeries), ChartSeries.YAxisProperty,
        PropertyValueEditor.CreateEditorAttribute(typeof(ForbiddenPropertyEditor)));
    }
    #endregion
  }
#endif

}
