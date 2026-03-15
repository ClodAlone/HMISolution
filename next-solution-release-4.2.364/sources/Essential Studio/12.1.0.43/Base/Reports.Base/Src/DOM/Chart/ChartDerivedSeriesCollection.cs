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
    public class ChartDerivedSeriesCollection : List<ChartDerivedSeries>
    {
    }

    public class ChartDerivedSeries
    {
        public ChartSeries ChartSeries { get; set; }
        public string SourceChartSeriesName { get; set; }
        public DerivedSeriesFormula DerivedSeriesFormula { get; set; }
        public ChartFormulaParameters ChartFormulaParameters { get; set; }
    }
}
