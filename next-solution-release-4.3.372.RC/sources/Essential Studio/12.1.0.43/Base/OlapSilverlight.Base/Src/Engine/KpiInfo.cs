//-------------------------------------------------------------------------------------------------
// <copyright file="KPIInformation.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.OlapSilverlight.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// contains Index informations of all KPI Elements in the tuple
    /// </summary>
    public class KpiInfo
    {
        #region Private Variables
        private string m_MeasureValue;
        private string m_GoalValue;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Gauge.Olap.KPIAxisInformation">KPIAxisInformation</see>
        /// class.
        /// </summary>
        public KpiInfo()
        {
            this.MeasureValue = string.Empty;
            this.GoalValue = string.Empty;
            this.ActualMeasureValue = string.Empty;
            this.ActualGoalValue = string.Empty;
            this.StatusValue = -2;
            this.TrendValue = -2;
            this.GoalCaption = string.Empty;
            this.MeasureCaption = string.Empty;
            this.MemberName = string.Empty;
            this.Kpi_Name = string.Empty;
            this.ValueIndex = -1;
            this.TrendIndex = -1;
            this.GoalIndex = -1;
            this.StatusIndex = -1;
        }

        public KpiInfo(KpiInfo kpiInfo)
        {
            this.GoalCaption = kpiInfo.GoalCaption;
            this.Kpi_Name = kpiInfo.Kpi_Name;
            this.MeasureCaption = kpiInfo.MeasureCaption;
            this.GoalValue = kpiInfo.GoalValue;
            this.ActualMeasureValue = string.Empty;
            this.ActualGoalValue = string.Empty;
            this.MeasureValue = kpiInfo.MeasureValue;
            this.MemberName = kpiInfo.MemberName;
            this.StatusValue = kpiInfo.StatusValue;
            this.TrendValue = kpiInfo.TrendValue;
            this.TrendGraphic = kpiInfo.TrendGraphic;
            this.StatusGraphic = kpiInfo.StatusGraphic;
            this.ValueIndex = kpiInfo.ValueIndex;
            this.TrendIndex = kpiInfo.TrendIndex;
            this.GoalIndex = kpiInfo.GoalIndex;
            this.StatusIndex = kpiInfo.StatusIndex;
        }
        #endregion

        /// <summary>
        /// Copies the specified source kpi info to the Destination kpi.
        /// </summary>
        /// <param name="sourceKpiInfo">The source kpi info.</param>
        /// <param name="destinationKpiInfo">The destination kpi info.</param>
        /// <returns>A new copy of the merged source kpi and destination kpi</returns>
        public KpiInfo Copy(KpiInfo sourceKpiInfo, KpiInfo destinationKpiInfo)
        {
            if (destinationKpiInfo.Kpi_Name == sourceKpiInfo.Kpi_Name && destinationKpiInfo.MemberName == destinationKpiInfo.MemberName)
            {
                if (destinationKpiInfo.GoalCaption == string.Empty || destinationKpiInfo.GoalCaption == null)
                {
                    destinationKpiInfo.GoalCaption = sourceKpiInfo.GoalCaption;
                }

                if (destinationKpiInfo.MeasureCaption == string.Empty || destinationKpiInfo.MeasureCaption == null)
                {
                    destinationKpiInfo.MeasureCaption = sourceKpiInfo.MeasureCaption;
                }

                if (destinationKpiInfo.GoalValue == string.Empty || destinationKpiInfo.GoalValue == null)
                {
                    destinationKpiInfo.GoalValue = sourceKpiInfo.GoalValue;
                }

                if (destinationKpiInfo.MeasureValue == string.Empty || destinationKpiInfo.MeasureValue == null)
                {
                    destinationKpiInfo.MeasureValue = sourceKpiInfo.MeasureValue;
                }

                if (destinationKpiInfo.StatusValue == -2 || destinationKpiInfo.StatusIndex == 0)
                {
                    destinationKpiInfo.StatusValue = sourceKpiInfo.StatusValue;
                }

                if (destinationKpiInfo.TrendValue == -2 || destinationKpiInfo.StatusIndex == 0)
                {
                    destinationKpiInfo.TrendValue = sourceKpiInfo.TrendValue;
                }

                if (destinationKpiInfo.TrendGraphic == string.Empty || destinationKpiInfo.TrendGraphic == null)
                {
                    destinationKpiInfo.TrendGraphic = sourceKpiInfo.TrendGraphic;
                }

                if (destinationKpiInfo.StatusGraphic == string.Empty || destinationKpiInfo.StatusGraphic == null)
                {
                    destinationKpiInfo.StatusGraphic = sourceKpiInfo.StatusGraphic;
                }

                if (destinationKpiInfo.ValueIndex == -1)
                {
                    destinationKpiInfo.ValueIndex = sourceKpiInfo.ValueIndex;
                }

                if (destinationKpiInfo.TrendIndex == -1)
                {
                    destinationKpiInfo.TrendIndex = sourceKpiInfo.TrendIndex;
                }

                if (destinationKpiInfo.GoalIndex == -1)
                {
                    destinationKpiInfo.GoalIndex = sourceKpiInfo.GoalIndex;
                }

                if (destinationKpiInfo.StatusIndex == -1)
                {
                    destinationKpiInfo.StatusIndex = sourceKpiInfo.StatusIndex;
                }
            }

            return destinationKpiInfo;
        }

        #region Properties
        /// <summary>
        /// Gets or sets Goal Index which is used to identify the Index of KPI_Goal.
        /// </summary>
        public string GoalValue 
        {
            get
            {
                return m_GoalValue;
            }
            set
            {
                m_GoalValue = value;
            }
        }

        /// <summary>
        /// Gets or sets StatusIndex which is used to identify the Index of KPI_Status.
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// Gets or sets TrendIndex which is used to identify the Index of KPI_Trend.
        /// </summary>
        public int TrendValue { get; set; }

        /// <summary>
        /// Gets or sets ValueIndex which is used to identify the Index of KPI_Value.
        /// </summary>
        public string MeasureValue 
        {
            get
            {
                return m_MeasureValue;
            }
            set
            {
                m_MeasureValue = value;
            }
        }

        /// <summary>
        /// Gets Actual Measure Value without $ symbol truncation.
        /// </summary>
        public string ActualMeasureValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Actual Goal Value without $ symbol truncation.
        /// </summary>
        public string ActualGoalValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the Measure Name for the corresponding KPI_Value.
        /// </summary>
        /// <value>The name of the measure unique.</value>
        public string MeasureCaption { get; set; }

        /// <summary>
        /// Gets or sets the name of the Measure Name for the corresponding KPI_Goal.
        /// </summary>
        /// <value>The name of the goal unique.</value>
        public string GoalCaption { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the KPI Name
        /// </summary>
        /// <value>The Name of KPI.</value>
        public string Kpi_Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid KPI.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid KPI; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidKpi 
        {
            get
            {
                if (this.IsVaidValue(GoalValue) && this.IsVaidValue(MeasureValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the value.
        /// </summary>
        /// <value>The index of the value.</value>
        public int ValueIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the goal.
        /// </summary>
        /// <value>The index of the goal.</value>
        public int GoalIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the status.
        /// </summary>
        /// <value>The index of the status.</value>
        public int StatusIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the trend.
        /// </summary>
        /// <value>The index of the trend.</value>
        public int TrendIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the member row.
        /// </summary>
        /// <value>The index of the member row.</value>
        public int MemberRowIndex { get; set; }

        /// <summary>
        /// Gets or sets the status graphic.
        /// </summary>
        /// <value>The status graphic.</value>
        public string StatusGraphic { get; set; }

        /// <summary>
        /// Gets or sets the trend graphic.
        /// </summary>
        /// <value>The trend graphic.</value>
        public string TrendGraphic { get; set; }

        /// <summary>
        /// Gets or sets the index of the member column.
        /// </summary>
        /// <value>The index of the member column.</value>
        public int MemberColumnIndex { get; set; }
        #endregion

        #region Private Methods
        private bool IsVaidValue(string s)
        {
            double value;
            if (s != string.Empty)
            {
                if (!double.TryParse(s, out value))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
