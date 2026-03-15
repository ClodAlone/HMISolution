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

namespace Syncfusion.RDL.DOM
{
    public abstract class MapAppearanceRule
    {
        public string DataValue { get; set; }
        public DistributionType DistributionType { get; set; }
        public int BucketCount { get; set; }
        public string StartValue { get; set; }
        public string EndValue { get; set; }
        public MapBuckets MapBuckets { get; set; }
        public string LegendName { get; set; }
        public string LegendText { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
    }
}
