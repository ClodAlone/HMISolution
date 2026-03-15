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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class ChartSeriesCollection:List<ChartSeries>
    {
    }

    public class ChartSeries
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string Hidden { get; set; }
        public ChartDataPoints ChartDataPoints { get; set; }
        public VisualizationType Type { get; set; }
        public VisualizationSubType Subtype { get; set; }
        public ChartEmptyPoints ChartEmptyPoints { get; set; }
        public Style Style { get; set; }
        public ChartDataLabel ChartDataLabel { get; set; }
        public ChartMarker ChartMarker { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public string LegendName { get; set; }
        public ChartItemInLegend ChartItemInLegend { get; set; }
        public string ChartAreaName { get; set; }
        public string ValueAxisName { get; set; }
        public string CategoryAxisName { get; set; }
        public ChartSmartLabel ChartSmartLabel { get; set; }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }
    }
}
