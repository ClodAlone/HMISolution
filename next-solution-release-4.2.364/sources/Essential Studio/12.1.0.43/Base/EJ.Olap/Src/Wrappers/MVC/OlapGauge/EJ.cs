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

namespace Syncfusion.JavaScript.Olap
{
    public static partial class EJExtension
    {
        public static OlapGaugePropertiesBuilder OlapGauge(this OlapControls ej, string id)
        {
            var model = new OlapGaugeProperties();
            var olapGauge = new OlapGauge(id, model);
            return new OlapGaugePropertiesBuilder(olapGauge);
        }
        public static OlapGauge OlapGauge(this OlapControls ej, String id, OlapGaugeProperties model)
        {
            var olapGauge = new OlapGauge(id, model);
            return olapGauge;
        }
    }
}
