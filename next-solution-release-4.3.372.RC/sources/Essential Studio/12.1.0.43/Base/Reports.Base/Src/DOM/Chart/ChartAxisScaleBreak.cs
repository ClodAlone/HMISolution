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
    public class ChartAxisScaleBreak
    {
        public string Enabled { get; set; }
        public BreakLineType BreakLineType { get; set; }
        public int CollapsibleSpaceThreshold { get; set; }
        public int MaxNumberOfBreaks { get; set; }
        public float Spacing { get; set; }
        public BooleanOptions IncludeZero { get; set; }
        public Style Style { get; set; }
    }
}
