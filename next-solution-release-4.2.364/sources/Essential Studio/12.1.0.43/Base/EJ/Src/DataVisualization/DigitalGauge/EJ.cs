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
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public DigitalGaugePropertiesBuilder DigitalGauge(string id)
        {
            var model = new DigitalGaugeProperties();
            var digitalgauge = new DigitalGauge(id, model);
            return new DigitalGaugePropertiesBuilder(digitalgauge);
        }
        public DigitalGauge DigitalGauge(String id, DigitalGaugeProperties model)
        {
            var digitalgauge = new DigitalGauge(id, model);
            return digitalgauge;
        }

    }
}
