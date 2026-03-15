#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ServiceModel;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.OlapSilverlight.Manager
{
    /// <summary>
    /// Defines some constants for Data Provider.
    /// </summary>
    [ServiceContract(Namespace = "")]
    [ServiceKnownType(typeof(Element))]
    [ServiceKnownType(typeof(ElementCollection))]
    [ServiceKnownType(typeof(DimensionElement))]
    [ServiceKnownType(typeof(MemberProperty))]
    [ServiceKnownType(typeof(MemberPropertyCollection))]
    [ServiceKnownType(typeof(HierarchyElement))]
    [ServiceKnownType(typeof(HierarchyElementCollection))]
    [ServiceKnownType(typeof(LevelElement))]
    [ServiceKnownType(typeof(LevelElementCollection))]
    [ServiceKnownType(typeof(MemberElement))]
    [ServiceKnownType(typeof(MemberElementCollection))]
    [ServiceKnownType(typeof(MeasureElement))]
    [ServiceKnownType(typeof(MemberElementCollection))]
    [ServiceKnownType(typeof(MeasureElements))]
    [ServiceKnownType(typeof(KpiElement))]
    [ServiceKnownType(typeof(KpiElementCollection))]
    [ServiceKnownType(typeof(KpiElements))]
    [ServiceKnownType(typeof(NamedSetElement))]
    [ServiceKnownType(typeof(SortElement))]
    [ServiceKnownType(typeof(FilterElement))]
    [ServiceKnownType(typeof(FilterValue))]
    [ServiceKnownType(typeof(CalculatedMember))]
    [ServiceKnownType(typeof(CalculatedMembers))]
    [ServiceKnownType(typeof(CalculatedMemberCollection))]
    [ServiceKnownType(typeof(TopCountElement))]
    [ServiceKnownType(typeof(SubsetElement))]
    [ServiceKnownType(typeof(VirtualKpiElement))]
    [ServiceKnownType(typeof(Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object>))]
    [ServiceKnownType(typeof(System.Collections.ObjectModel.ObservableCollection<System.Dynamic.ExpandoObject>))]
    [ServiceKnownType(typeof(System.Collections.ObjectModel.ObservableCollection<object>))]
    [ServiceKnownType(typeof(System.Dynamic.ExpandoObject))]

    public interface IOlapDataProvider
    {
        /// <summary>
        /// Executes the cell set.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns>A CellSet.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.CellSet ExecuteOlapReport(OlapReport report);

        /// <summary>
        /// Executes the OlapReport.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns>A CellSet.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.CellSet ExecuteOlapReportWithLevelTypeAll(OlapReport report, bool showLevelTypeAll);

        /// <summary>
        /// Executes the olap report on drill state settings enabled.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <returns>Drilled states based on Row and Columns.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> ExecuteOlapReportOnDrillState(OlapReport report, bool showLevelTypeAll);

        /// <summary>
        /// Executes the OlapReport with total count.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns>Combination of CellSet as well RowCount and ColumnCount.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> ExecuteOlapReportWithTotalCount(OlapReport report);

        /// <summary>
        /// Executes the MDX query.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <returns>A CellSet.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.CellSet ExecuteMdxQuery(string mdxQuery);

        /// <summary>
        /// Executes the specified MDX query.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <returns>An object.</returns>
        [OperationContract]
        object Execute(string mdxQuery);

        /// <summary>
        /// Gets the cubes.
        /// </summary>
        /// <returns>Cube information as a collection.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.CubeInfoCollection GetCubes();

        /// <summary>
        /// Gets the cube schema.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Cube Schema information.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.CubeSchema GetCubeSchema(string cubeName);

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="levelUniqueName">Name of the level unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Level Member collection.</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.MemberCollection GetLevelMembers(string levelUniqueName, string cubeName);

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="memberUniqueName">Name of the member unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>Child Member collection</returns>
        [OperationContract]
        Syncfusion.OlapSilverlight.Data.MemberCollection GetChildMembers(string memberUniqueName, string cubeName);

        [OperationContract]
        Syncfusion.OlapSilverlight.Data.MemberCollection GetChildrenByMDX(string commandText);

        /// <summary>
        /// Gets the MDX query.
        /// </summary>
        /// <param name="olapReport">The OlapReport.</param>
        /// <returns>MDX Query as string.</returns>
        [OperationContract]
        string GetMdxQuery(OlapReport olapReport);
    }
}