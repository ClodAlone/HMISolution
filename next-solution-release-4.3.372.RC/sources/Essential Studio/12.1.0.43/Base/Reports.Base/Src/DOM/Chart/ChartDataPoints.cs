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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class ChartDataPoints : List<ChartDataPoint>
    {

    }
    public class ChartDataPoint
    {
        public ChartDataPointValues ChartDataPointValues { get; set; }
        public ChartDataLabel ChartDataLabel { get; set; }
        public string AxisLabel { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public Style Style { get; set; }
        public ChartMarker ChartMarker { get; set; }
        public string DataElementName { get; set; }
        [DefaultValue(DataElementOutputs.Auto)]
        public DataElementOutputs DataElementOutput { get; set; }
        public ChartItemInLegend ChartItemInLegend { get; set; }
        public CustomProperties CustomProperties { get; set; }

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
