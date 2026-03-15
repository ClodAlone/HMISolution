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
using Syncfusion.Windows.Shared.Olap;
using System.Windows;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Contains the report element such as dimension, hierarchy, level and members
    /// </summary>
    public class ReportDimensionElement
        : DependencyObject, IReportDimensionElement
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportElements"/> class.
        /// </summary>
        public ReportDimensionElement()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the dimension.
        /// </summary>
        /// <value>The name of the dimension.</value>
        public string DimensionName
        {
            get { return (string)GetValue(DimensionNameProperty); }
            set { SetValue(DimensionNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the hierarchy.
        /// </summary>
        /// <value>The name of the hierarchy.</value>
        public string HierarchyName
        {
            get { return (string)GetValue(HierarchyNameProperty); }
            set { SetValue(HierarchyNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        /// <value>The name of the level.</value>
        public string LevelName
        {
            get { return (string)GetValue(LevelNameProperty); }
            set { SetValue(LevelNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public ExcludedMembers ExcludedMembers
        {
            get { return (ExcludedMembers)GetValue(ExcludedMembersProperty); }
            set { SetValue(ExcludedMembersProperty, value); }
        }

        #endregion

        #region Dependency Properties

        // Using a DependencyProperty as the backing store for DimensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimensionNameProperty =
            DependencyProperty.Register("DimensionName", typeof(string), typeof(ReportDimensionElement), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for HierarchyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HierarchyNameProperty =
            DependencyProperty.Register("HierarchyName", typeof(string), typeof(ReportDimensionElement), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for LevelName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelNameProperty =
            DependencyProperty.Register("LevelName", typeof(string), typeof(ReportDimensionElement), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for MembersProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExcludedMembersProperty =
            DependencyProperty.Register("ExcludedMembers", typeof(ExcludedMembers), typeof(ReportDimensionElement), new UIPropertyMetadata(null));

        #endregion
    }
}
