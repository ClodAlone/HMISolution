#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
    [ServiceKnownType(typeof(HierarchyElement))]
    [ServiceKnownType(typeof(HierarchyElementCollection))]
    [ServiceKnownType(typeof(LevelElement))]
    [ServiceKnownType(typeof(LevelElementCollection))]
    [ServiceKnownType(typeof(MemberElement))]
    [ServiceKnownType(typeof(MemberElementCollection))]
    [ServiceKnownType(typeof(MeasureElement))]
    [ServiceKnownType(typeof(MeasureElementCollection))]
    [ServiceKnownType(typeof(MeasureElements))]
    [ServiceKnownType(typeof(KpiElement))]
    [ServiceKnownType(typeof(KpiElementCollection))]
    [ServiceKnownType(typeof(KpiElements))]
    [ServiceKnownType(typeof(NamedSetElement))]
    [ServiceKnownType(typeof(SortElement))]
    [ServiceKnownType(typeof(FilterElement))]
    [ServiceKnownType(typeof(FilterValue))]
    [ServiceKnownType(typeof(MemberProperty))]
    [ServiceKnownType(typeof(MemberPropertyCollection))]
    [ServiceKnownType(typeof(CalculatedMember))]
    [ServiceKnownType(typeof(CalculatedMembers))]  
    [ServiceKnownType(typeof(CalculatedMemberCollection))]
    [ServiceKnownType(typeof(TopCountElement))]
    [ServiceKnownType(typeof(VirtualKpiElement))]
    [ServiceKnownType(typeof(SubsetElement))]
    [ServiceKnownType(typeof(System.Collections.ObjectModel.ObservableCollection<System.Dynamic.ExpandoObject>))]
    [ServiceKnownType(typeof(System.Collections.ObjectModel.ObservableCollection<object>))]
    [ServiceKnownType(typeof(System.Dynamic.ExpandoObject))]
    
    public interface IOlapDataProvider
    {
        /// <summary>
        /// Begins the execute cell set.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of Execute OlapReport method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapReport(Syncfusion.OlapSilverlight.Reports.OlapReport report, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the execute OlapReport.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A CellSet.</returns>
        Syncfusion.OlapSilverlight.Data.CellSet EndExecuteOlapReport(IAsyncResult result);

        /// <summary>
        /// Begins the execute OlapReport with level type all.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of Execute OlapReport method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapReportWithLevelTypeAll(Syncfusion.OlapSilverlight.Reports.OlapReport report, bool showLevelTypeAll, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the execute OlapReport with level type all.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A CellSet.</returns>
        Syncfusion.OlapSilverlight.Data.CellSet EndExecuteOlapReportWithLevelTypeAll(IAsyncResult result);

        /// <summary>
        /// Begins the execute OlapReport with level type all.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="showLevelTypeAll">if set to <c>true</c> [show level type all].</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapReportOnDrillState(Syncfusion.OlapSilverlight.Reports.OlapReport report, bool showLevelTypeAll, AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> EndExecuteOlapReportOnDrillState(IAsyncResult result);

        /// <summary>
        /// Begins the execute MDX query.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of Execute MdxQuery method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteMdxQuery(string mdxQuery, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the execute MDX query.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        Syncfusion.OlapSilverlight.Data.CellSet EndExecuteMdxQuery(IAsyncResult result);


        /// <summary>
        /// Begins the get cubes.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of ExecuteCube method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCubes(AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the get cubes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Cube information as a collection.</returns>
        Syncfusion.OlapSilverlight.Data.CubeInfoCollection EndGetCubes(IAsyncResult result);


        /// <summary>
        /// Begins the get cube schema.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of GetCubeSchema method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCubeSchema(string cubeName, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the get cube schema.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Cube Schema information.</returns>
        Syncfusion.OlapSilverlight.Data.CubeSchema EndGetCubeSchema(IAsyncResult result);


        /// <summary>
        /// Begins the get level members.
        /// </summary>
        /// <param name="levelUniqueName">Name of the level unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of GetLevelMembers method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLevelMembers(string levelUniqueName,string cubeName, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the get level members.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Level members as a collection.</returns>
        Syncfusion.OlapSilverlight.Data.MemberCollection EndGetLevelMembers(IAsyncResult result);


        /// <summary>
        /// Begins the get child members.
        /// </summary>
        /// <param name="memberUniqueName">Name of the member unique.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of GetChildMembers method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetChildMembers(string memberUniqueName, string cubeName, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the get child members.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Child Members as a collection.</returns>
        Syncfusion.OlapSilverlight.Data.MemberCollection EndGetChildMembers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetChildrenByMDX(string commandText,AsyncCallback callback, Object state);

        Syncfusion.OlapSilverlight.Data.MemberCollection EndGetChildrenByMDX(IAsyncResult rusult);

        /// <summary>
        /// Begins the execute.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of Execute method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecute(string mdxQuery, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the execute.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Result as an object type.</returns>
        object EndExecute(IAsyncResult result);

        /// <summary>
        /// Begins the execute OlapReport with total count.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of ExecuteOlapReportWithTotalCount method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecuteOlapReportWithTotalCount(Syncfusion.OlapSilverlight.Reports.OlapReport report, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the execute OlapReport with total count.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Combination of CellSet and RowCount, ColumnCount information.</returns>
        Syncfusion.OlapSilverlight.Common.SerializableDictionary<string, object> EndExecuteOlapReportWithTotalCount(IAsyncResult result);

        /// <summary>
        /// Begins the get MDX query.
        /// </summary>
        /// <param name="olapReport">The OlapReport.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns><see cref="IAsyncResult"/> of GetMdxQuery method.</returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMdxQuery(OlapReport olapReport, AsyncCallback callback, Object state);

        /// <summary>
        /// Ends the get MDX query.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>MDX Query as string.</returns>
        string EndGetMdxQuery(IAsyncResult result);
    }
}