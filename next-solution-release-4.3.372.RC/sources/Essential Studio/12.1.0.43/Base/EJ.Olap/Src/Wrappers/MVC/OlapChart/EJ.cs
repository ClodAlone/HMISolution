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
using Syncfusion.JavaScript.Olap;
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public static partial class EJExtension
    {
        public static OlapChartPropertiesBuilder OlapChart(this OlapControls ej, string id)
        {
            var model = new OlapChartProperties();
            var olapChart = new OlapChart(id, model);
            return new OlapChartPropertiesBuilder(olapChart);
        }
        public static OlapChart OlapChart(this OlapControls ej, String id, OlapChartProperties model)
        {
            var olapChart = new OlapChart(id, model);
            return olapChart;
        }
    }
}
