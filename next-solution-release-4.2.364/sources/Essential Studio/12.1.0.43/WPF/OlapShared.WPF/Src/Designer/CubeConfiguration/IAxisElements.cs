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
    /// <summary>
    /// Repesents the each type of elements in an axis.
    /// </summary>
    public interface IAxisElements
    {
        /// <summary>
        /// Gets or sets the report dimension elements.
        /// </summary>
        /// <value>The report dimension elements.</value>
        [DefaultValue(null)]
        ReportDimensionElements ReportDimensionElements
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the report measure elements.
        /// </summary>
        /// <value>The report measure elements.</value>
        [DefaultValue(null)]
        ReportMeasureElements ReportMeasureElements
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the report kpi elements.
        /// </summary>
        /// <value>The report kpi elements.</value>
        [DefaultValue(null)]
        ReportKpiElements ReportKpiElements
        {
            get;
            set;
        }
    }
}
