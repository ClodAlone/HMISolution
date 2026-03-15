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
using System.Windows;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Categorical elements.
    /// </summary>
    public class CategoricalAxis
        : DependencyObject, IAxisElements
    {
        #region IAxisElements Members

        /// <summary>
        /// Gets or sets the report dimension elements.
        /// </summary>
        /// <value>The report dimension elements.</value>
        [DefaultValue(null)]
        public ReportDimensionElements ReportDimensionElements
        {
            get { return (ReportDimensionElements)GetValue(ReportDimensionElementsProperty); }
            set { SetValue(ReportDimensionElementsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReportDimensionElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportDimensionElementsProperty =
            DependencyProperty.Register("ReportDimensionElements", typeof(ReportDimensionElements), typeof(CategoricalAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the report measure elements.
        /// </summary>
        /// <value>The report measure elements.</value>
        [DefaultValue(null)]
        public ReportMeasureElements ReportMeasureElements
        {
            get { return (ReportMeasureElements)GetValue(ReportMeasureElementsProperty); }
            set { SetValue(ReportMeasureElementsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReportMeasureElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportMeasureElementsProperty =
            DependencyProperty.Register("ReportMeasureElements", typeof(ReportMeasureElements), typeof(CategoricalAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the report kpi elements.
        /// </summary>
        /// <value>The report kpi elements.</value>
        [DefaultValue(null)]
        public ReportKpiElements ReportKpiElements
        {
            get { return (ReportKpiElements)GetValue(ReportKpiElementsProperty); }
            set { SetValue(ReportKpiElementsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReportKpiElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportKpiElementsProperty =
            DependencyProperty.Register("ReportKpiElements", typeof(ReportKpiElements), typeof(CategoricalAxis), new UIPropertyMetadata(null));      

        #endregion
    }
}
