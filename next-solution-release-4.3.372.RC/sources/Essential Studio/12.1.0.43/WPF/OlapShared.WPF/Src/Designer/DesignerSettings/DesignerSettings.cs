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
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Manager;
using System.Windows;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Creates and maintains the desinger settings for the control.
    /// </summary>
    public sealed class DesignerSettings
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerSettings"/> class.
        /// </summary>
        public DesignerSettings()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        public string CurrentCubeName { get; set; }

        /// <summary>
        /// Gets or sets the connection mode.
        /// </summary>
        /// <value>The connection mode.</value>
        public ConnectionMode ConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the report.
        /// </summary>
        /// <value>The report.</value>
        public Syncfusion.Windows.Shared.Olap.Report Report { get; set; }

        #endregion

        #region Report Generation

        #region Validation Methods

        /// <summary>
        /// Hads the valid report.
        /// </summary>
        /// <returns></returns>
        public bool HadValidReport()
        {
            if (this.ConnectionString != string.Empty && this.CurrentCubeName != string.Empty)
                return true;
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid connection string.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid connection string; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidConnectionString
        {
            get
            {
                if (this.ConnectionString != string.Empty)
                    return true;
                return false;
            }
        }

        #endregion

        #region Olap Report from XAMLReport

        /// <summary>
        /// Gets the olap report.
        /// </summary>
        /// <returns>Returns the olap report for the current report.</returns>
        public OlapReport GetOlapReport()
        {
            OlapReport olapReport = new OlapReport();

            olapReport.CurrentCubeName = this.CurrentCubeName;

            if (this.Report != null)
            {
                olapReport.CategoricalElements.AddRange(this.GenerateAxisElements(this.Report.CategoricalAxis));
                olapReport.SeriesElements.AddRange(this.GenerateAxisElements(this.Report.SeriesAxis));
                olapReport.SlicerElements.AddRange(this.GenerateAxisElements(this.Report.SlicerAxis));
            }

            return olapReport;
        }

        #endregion

        #region Axis Processing

        /// <summary>
        /// Generates the axis elements. Processes the given axis and generates items collection for the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <returns>Returns the axis elements.</returns>
        private Items GenerateAxisElements(Syncfusion.Windows.Shared.Olap.IAxisElements axis)
        {
            //// -------- For each 
            //// Process Dimension
            //// Process Dimension with exclude
            //// Process Measure
            //// Process Kpi -----

            AxisPosition axisType = AxisPosition.Categorical;

            if (axis is CategoricalAxis)
            {
                axisType = AxisPosition.Categorical;
            }
            else if (axis is SeriesAxis)
            {
                axisType = AxisPosition.Series;
            }
            else
            {
                axisType = AxisPosition.Slicer;
            }

            Items items = new Items();

            if (axis != null)
            {
                if (axis.ReportDimensionElements != null)
                {
                    foreach (Syncfusion.Windows.Shared.Olap.ReportDimensionElement rde in axis.ReportDimensionElements)
                    {
                        if (rde.ExcludedMembers == null)
                        {
                            items.Add(this.ProcessDimension(rde));
                        }
                        else
                        {
                            items.Add(this.ProcessDimension(rde), this.ProcessDimension(rde, rde.ExcludedMembers));
                        }
                    }
                }

                if (axis.ReportKpiElements != null)
                {
                    KpiElements kpiElements = new KpiElements();

                    foreach (Syncfusion.Windows.Shared.Olap.ReportKpiElement rkpi in axis.ReportKpiElements)
                    {
                        kpiElements.Elements.Add(new KpiElement() { Name = rkpi.Name, ShowKPIGoal = rkpi.ShowKPIGoal, ShowKPIStatus = rkpi.ShowKPIStatus, ShowKPITrend = rkpi.ShowKPITrend, ShowKPIValue = rkpi.ShowKPIValue });
                    }

                    items.Add(kpiElements);
                }

                if (axis.ReportMeasureElements != null)
                {
                    MeasureElements measureElements = new MeasureElements();

                    foreach (Syncfusion.Windows.Shared.Olap.ReportMeasureElement rme in axis.ReportMeasureElements)
                    {
                        if (rme.Name != null)
                        {
                            measureElements.Elements.Add(new MeasureElement() { Name = rme.Name });
                        }
                        else
                        {
                            MessageBox.Show("RME is null here");
                        }
                    }

                    items.Add(measureElements);
                }
            }

            foreach(Item item in items)
            {
                item.Axis = axisType;
            }

            return items;
        }

        /// <summary>
        /// Processes the dimension.
        /// </summary>
        /// <param name="reportDimensionElement">The report dimension element.</param>
        /// <returns>Returns olap report dimension element.</returns>
        private DimensionElement ProcessDimension(Syncfusion.Windows.Shared.Olap.ReportDimensionElement reportDimensionElement)            
        {
            return this.ProcessDimension(reportDimensionElement, null);
        }

        /// <summary>
        /// Processes the dimension.
        /// </summary>
        /// <param name="reportDimensionElement">The report dimension element.</param>
        /// <param name="excludedMembers">The excluded members.</param>
        /// <returns>Returns olap report dimension element.</returns>
        private DimensionElement ProcessDimension(Syncfusion.Windows.Shared.Olap.ReportDimensionElement reportDimensionElement,
            Syncfusion.Windows.Shared.Olap.ExcludedMembers excludedMembers)
        {
            DimensionElement de;
            DimensionElement xde;
            
            if (excludedMembers == null)
            {
                de = new DimensionElement() { Name = reportDimensionElement.DimensionName };
                de.AddLevel(reportDimensionElement.HierarchyName, reportDimensionElement.LevelName);

                return de;
            }
            else
            {
                xde = new DimensionElement() { Name = reportDimensionElement.DimensionName };
                
                foreach(Syncfusion.Windows.Shared.Olap.ExcludedMember em in excludedMembers)
                {
                    xde.AddLevel(em.HierarchyName, em.LevelName);
                    xde.AddMember(em.LevelName, em.MemberName);
                }

                return xde;
            }
        }

        #endregion

        #endregion
    }

    #region Connection modes

    /// <summary>
    /// Specifies the connection type to be used.
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None,

        /// <summary>
        /// Connect to an Offline cube.
        /// </summary>
        OfflineCube,

        /// <summary>
        /// Connect using a server using connection string.
        /// </summary>
        CustomServer,

        /// <summary>
        /// Connect using the given raw connection string.
        /// </summary>
        RawString

    }

    #endregion
}
