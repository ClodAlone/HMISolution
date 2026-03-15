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
using System.Windows;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Describes a KPI element.
    /// </summary>
    public class ReportKpiElement
        : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportKpiElement"/> class.
        /// </summary>
        public ReportKpiElement()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show KPI goal].
        /// </summary>
        /// <value><c>true</c> if [show KPI goal]; otherwise, <c>false</c>.</value>
        public bool ShowKPIGoal
        {
            get { return (bool)GetValue(ShowKPIGoalProperty); }
            set { SetValue(ShowKPIGoalProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show KPI status].
        /// </summary>
        /// <value><c>true</c> if [show KPI status]; otherwise, <c>false</c>.</value>
        public bool ShowKPIStatus
        {
            get { return (bool)GetValue(ShowKPIStatusProperty); }
            set { SetValue(ShowKPIStatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show KPI trend].
        /// </summary>
        /// <value><c>true</c> if [show KPI trend]; otherwise, <c>false</c>.</value>
        public bool ShowKPITrend
        {
            get { return (bool)GetValue(ShowKPITrendProperty); }
            set { SetValue(ShowKPITrendProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show KPI value].
        /// </summary>
        /// <value><c>true</c> if [show KPI value]; otherwise, <c>false</c>.</value>
        public bool ShowKPIValue
        {
            get { return (bool)GetValue(ShowKPIValueProperty); }
            set { SetValue(ShowKPIValueProperty, value); }
        }

        #endregion

        #region Dependency Properties

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(ReportKpiElement), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for ShowKPIGoal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowKPIGoalProperty =
            DependencyProperty.Register("ShowKPIGoal", typeof(bool), typeof(ReportKpiElement), new UIPropertyMetadata(true));

        // Using a DependencyProperty as the backing store for ShowKPIStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowKPIStatusProperty =
            DependencyProperty.Register("ShowKPIStatus", typeof(bool), typeof(ReportKpiElement), new UIPropertyMetadata(true));

        // Using a DependencyProperty as the backing store for ShowKPITrend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowKPITrendProperty =
            DependencyProperty.Register("ShowKPITrend", typeof(bool), typeof(ReportKpiElement), new UIPropertyMetadata(true));

        // Using a DependencyProperty as the backing store for ShowKPIValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowKPIValueProperty =
            DependencyProperty.Register("ShowKPIValue", typeof(bool), typeof(ReportKpiElement), new UIPropertyMetadata(true));

        #endregion
    }
}
