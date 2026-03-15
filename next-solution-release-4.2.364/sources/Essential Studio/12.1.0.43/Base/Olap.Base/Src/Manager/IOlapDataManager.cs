//-------------------------------------------------------------------------------------------------
// <copyright file="IOlapDataManager.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using Syncfusion.Olap.Data;
using Syncfusion.Olap.DataProvider;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.MDXQueryBuilder;
using System.IO;
using System.Globalization;

namespace Syncfusion.Olap.Manager
{
    /// <summary>
    /// Represents the constants for <see cref="OlapDataManager"/> class.
    /// </summary>
    public interface IOlapDataManager
    {
        // Properties
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }
        /// <summary>
        /// Gets the current cell set.
        /// </summary>
        /// <value>The current cell set.</value>
        CellSet CurrentCellSet { get; }
        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        string CurrentCubeName { get; set; }
        /// <summary>
        /// Gets or sets the current cube schema.
        /// </summary>
        /// <value>The current cube schema.</value>
        CubeSchema CurrentCubeSchema { get; set; }
        /// <summary>
        /// Gets or sets the current report.
        /// </summary>
        /// <value>The current report.</value>
        OlapReport CurrentReport { get; set; }
        /// <summary>
        /// Gets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        IDataProvider DataProvider { get; }
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        CultureInfo Culture { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is current report modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current report modified; otherwise, <c>false</c>.
        /// </value>
        bool IsCurrentReportModified { get; }
        /// <summary>
        /// Gets the pivot engine.
        /// </summary>
        /// <value>The pivot engine.</value>
        PivotEngine PivotEngine { get; }
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        PropertyCollection Properties { get; }
        /// <summary>
        /// Gets the report path.
        /// </summary>
        /// <value>The report path.</value>
        string ReportPath { get; }
        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        OlapReportCollection Reports { get; set; }
        /// <summary>
        /// Gets or sets the MDX query.
        /// </summary>
        /// <value>The MDX query.</value>
        string MdxQuery { get; set; }
        /// <summary>
        /// Gets or sets the item source.
        /// </summary>
        /// <value>The item source.</value>
        object ItemSource { get; set; }

        // Events
        /// <summary>
        /// Occurs when [cube changed].
        /// </summary>
        event CubeChangedEventHandler CubeChanged;
        /// <summary>
        /// Occurs when [axis element changed].
        /// </summary>
        event AxisElementChangedEventHandler AxisElementChanged;
        /// <summary>
        /// Occurs when [axis element modified].
        /// </summary>
        event AxisElementModifiedEventHandler AxisElementModified;

        //event DrillDownEventHandler DrillDown;
        /// <summary>
        /// Occurs when [report changed].
        /// </summary>
        event ReportChangedEventHandler ReportChanged;
        /// <summary>
        /// Occurs when [Active Report changed].
        /// </summary>
        event ActiveReportChangedEventHandler ActiveReportChanged;

        // Methods
        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        void AddReport(string reportName);
        /// <summary>
        /// Adds the report.
        /// </summary>
        /// <param name="olapReport">The olap report.</param>
        void AddReport(OlapReport olapReport);
        /// <summary>
        /// Clones the olap data manager elements.
        /// </summary>
        /// <returns></returns>
        OlapDataManager CloneOlapDataManagerElements();
        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <returns></returns>
        CellSet ExecuteCellSet();

        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <param name="querySpecification">The query specification.</param>
        /// <returns></returns>
        CellSet ExecuteCellSet(MDXQuerySpecification querySpecification);
        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        CellSet ExecuteCellSet(string commandText);
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <returns></returns>
        PivotEngine ExecuteOlapTable();
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <returns></returns>
        PivotEngine ExecuteOlapTable(CellSet cellSet);
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        PivotEngine ExecuteOlapTable(string commandText);
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        PivotEngine ExecuteOlapTable(GridLayout layout);
        /// <summary>
        /// Executes the olap table.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        PivotEngine ExecuteOlapTable(CellSet cellSet, GridLayout layout);

        /// <summary>
        /// Gets the expanded rows.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <returns></returns>
        PivotEngine GetExpandedRows(PivotCellDescriptor cellDescriptor);
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        OlapReportCollection GetReport(string fileName);
        /// <summary>
        /// Gets the report as stream.
        /// </summary>
        /// <returns></returns>
        Stream GetReportAsStream();
        /// <summary>
        /// Loads the olap data manager.
        /// </summary>
        /// <param name="report">The report.</param>
        void LoadOlapDataManager(OlapReport report);
        /// <summary>
        /// Loads the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        void LoadReport(string reportName);
        /// <summary>
        /// Loads the report definition file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void LoadReportDefinitionFile(string fileName);
        /// <summary>
        /// Loads the report definition from stream.
        /// </summary>
        /// <param name="reportStream">The report stream.</param>
        void LoadReportDefinitionFromStream(Stream reportStream);
        /// <summary>
        /// Notifies the report changed.
        /// </summary>
        void NotifyReportChanged();
        /// <summary>
        /// Notifies the report changed.
        /// </summary>
        /// <param name="currentReport">The current report.</param>
        void NotifyReportChanged(OlapReport currentReport);
        /// <summary>
        /// Notifies the element modified.
        /// </summary>
        void NotifyElementModified();
        /// <summary>
        /// Notifies the element modified.
        /// </summary>
        /// <param name="axisPosition">The axis position.</param>
        void NotifyElementModified(AxisPosition axisPosition);
        /// <summary>
        /// Raises the axis element modified.
        /// </summary>
        void RaiseAxisElementModified();
        /// <summary>
        /// Removes the report.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        void RemoveReport(string reportName);
        /// <summary>
        /// Renames the report.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="newReportName">New name of the report.</param>
        void RenameReport(int index, string newReportName);
        /// <summary>
        /// Renames the report.
        /// </summary>
        /// <param name="oldReportName">Old name of the report.</param>
        /// <param name="newReportName">New name of the report.</param>
        void RenameReport(string oldReportName, string newReportName);
        /// <summary>
        /// Saves the report.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void SaveReport(string fileName);
        /// <summary>
        /// Transpose the data beetween the axis.
        /// </summary>
        /// <param name="olapReport">Get the current report.</param>
        void ToggleAxis(OlapReport olapReport);
        /// <summary>
        /// Sets the current report.
        /// </summary>
        /// <param name="report">The report.</param>
        void SetCurrentReport(OlapReport report);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        void ToggleExpandableState(PivotCellDescriptor cellDescriptor);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridLayout">The grid layout.</param>
        void ToggleExpandableState(PivotCellDescriptor cellDescriptor, GridLayout gridLayout);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        void ToggleExpandableState(PivotCellDescriptorType cellType, Member member);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        void ToggleExpandableState(PivotCellDescriptorType cellType, Member member, bool triggerEvents);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="pivotCellDescriptorType">Type of the pivot cell descriptor.</param>
        /// <param name="member">The member.</param>
        /// <param name="gridLayout">The grid layout.</param>
        void ToggleExpandableState(PivotCellDescriptorType pivotCellDescriptorType, Member member, GridLayout gridLayout);
        /// <summary>
        /// Toggles the state of the expandable.
        /// </summary>
        /// <param name="cellType">Type of the cell.</param>
        /// <param name="member">The member.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="triggerEvents">if set to <c>true</c> [trigger events].</param>
        void ToggleExpandableState(PivotCellDescriptorType cellType, Member member, GridLayout gridLayout, bool triggerEvents);
    }
}