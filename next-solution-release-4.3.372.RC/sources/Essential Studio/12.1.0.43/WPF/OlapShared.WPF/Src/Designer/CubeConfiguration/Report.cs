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
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Configures the report.
    /// </summary>
    public class Report
        : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class.
        /// </summary>
        public Report()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the categorical axis.
        /// </summary>
        /// <value>The categorical axis.</value>
        [DefaultValue(null)]
        public CategoricalAxis CategoricalAxis
        {
            get { return (CategoricalAxis)GetValue(CategoricalAxisProperty); }
            set { SetValue(CategoricalAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the report measure elements.
        /// </summary>
        /// <value>The report measure elements.</value>
        [DefaultValue(null)]
        public SeriesAxis SeriesAxis
        {
            get { return (SeriesAxis)GetValue(SeriesAxisProperty); }
            set { SetValue(SeriesAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the report kpi elements.
        /// </summary>
        /// <value>The report kpi elements.</value>
        [DefaultValue(null)]
        public SlicerAxis SlicerAxis
        {
            get { return (SlicerAxis)GetValue(SlicerAxisProperty); }
            set { SetValue(SlicerAxisProperty, value); }
        }

        #endregion

        #region Dependency Properties

        // Using a DependencyProperty as the backing store for ReportDimensionElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoricalAxisProperty =
            DependencyProperty.Register("CategoricalAxis", typeof(CategoricalAxis), typeof(Report), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for ReportMeasureElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesAxisProperty =
            DependencyProperty.Register("SeriesAxis", typeof(SeriesAxis), typeof(Report), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for ReportKpiElements.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlicerAxisProperty =
            DependencyProperty.Register("SlicerAxis", typeof(SlicerAxis), typeof(Report), new UIPropertyMetadata(null));

        #endregion
    }
}
