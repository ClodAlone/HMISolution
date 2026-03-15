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

namespace Syncfusion.Windows.Shared.Olap
{
    public class SeriesAxis
        : IAxisElements
    {
        #region IAxisElements Members

        [DefaultValue(null)]
        public ReportDimensionElements ReportDimensionElements
        {
            get;
            set;
        }

        [DefaultValue(null)]
        public ReportMeasureElements ReportMeasureElements
        {
            get;
            set;
        }

        [DefaultValue(null)]
        public ReportKpiElements ReportKpiElements
        {
            get;
            set;
        }

        #endregion
    }
}
