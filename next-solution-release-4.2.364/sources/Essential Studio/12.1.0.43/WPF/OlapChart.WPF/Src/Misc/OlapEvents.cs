#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion




namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Olap.Engine;


    #region Delegates
    [Serializable]
    public delegate void OlapMouseEventHandler(OlapChartAxis sender, OlapLabelClickEvenArgs e);
    public delegate void OlapRefreshEventHandler(object sender, OlapChartRefreshEventArgs e);
    #endregion

    #region EventArguments
    /// <summary>
    /// Represents olap label click event arguments class.
    /// </summary>
    public class OlapLabelClickEvenArgs : EventArgs
    {
        #region Members
        private PivotCellDescriptor m_member;
        #endregion

        #region Constructor
        public OlapLabelClickEvenArgs(PivotCellDescriptor targetOlapMember)
        {
            this.m_member = targetOlapMember;
        }
        #endregion

        #region Properties
        public PivotCellDescriptor Member
        {
            get
            {
                return m_member;
            }
        }
        #endregion 

    }

    /// <summary>
    /// Represents drill down event args.
    /// </summary>
    public class OlapChartRefreshEventArgs : EventArgs
    {
        #region Members
        private UIElement m_area;
        #endregion

        #region Public Methods
        public OlapChartRefreshEventArgs(UIElement chartArea)
        {
            ShowDefaultLoadingIndicator = true;
            m_area = chartArea;
        }
        #endregion

        #region ChartArea
        /// <summary>
        /// Gets the chart area.
        /// </summary>
        /// <value>The chart area.</value>
        public UIElement ChartArea
        {
            get
            {
                return m_area;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether default loading indicator should be shown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if default loading indicator is shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowDefaultLoadingIndicator { get; set; }
        #endregion
    }
    #endregion
}
