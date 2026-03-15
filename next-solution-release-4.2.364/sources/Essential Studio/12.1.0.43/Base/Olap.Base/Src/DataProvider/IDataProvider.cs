//-------------------------------------------------------------------------------------------------
// <copyright file="IDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.DataProvider
{
    /// <summary>
    /// Multidimensional data provider interface. An interface that must be implemented in
    /// OLAP data provider. 
    /// </summary>
    public interface IDataProvider
    {
        #region Private Method Declaration
        /// <summary>
        /// Executes the Command text and returns a cell set
        /// </summary>
        /// <param name="commandText">command text.</param>
        /// <param name="IsGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
        /// <param name="isQuery">if set to <c>true</c> [is query].</param>
        /// <param name="currentReport">The current report.</param>
        /// <returns>returns the CellSet</returns>
        CellSet ExecuteCellSet(string commandText, bool IsGrandTotalOn, bool isQuery, Syncfusion.Olap.Reports.OlapReport currentReport);

        /// <summary>
        /// Executes the count.
        /// </summary>
        /// <param name="mdxQuery">The MDX query.</param>
        /// <returns>Row and Column count result as an integer array.</returns>
        int[] ExecuteCount(string mdxQuery);

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Member Collection</returns>
        MemberCollection GetChildMembers(Member member, bool IsGrandTotalOn);

        /// <summary>
        /// Gets the child members.
        /// </summary>
        /// <param name="memberUniqueName">Unique name of the member.</param>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="IsGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
        /// <returns>Member Collection</returns>
        MemberCollection GetChildMembers(string memberUniqueName, string cubeName, bool IsGrandTotalOn);


        MemberCollection GetChildrenByMDX(string commandText);

        /// <summary>
        /// Gets the cell based on the parameter index passed.
        /// </summary>
        /// <param name="cellSet">The cell set</param>
        /// <param name="indexes">array of indexes</param>
        /// <returns>returns a Cell</returns>
        Cell GetCell(CellSet cellSet, params int[] indexes);

        /// <summary>
        /// Gets the cube schema.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>returns a CubeSchema</returns>
        CubeSchema GetCubeSchema(string cubeName);

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>returns a MemberCollection object</returns>
        MemberCollection GetLevelMembers(Level level);

        /// <summary>
        /// Gets the level members.
        /// </summary>
        /// <param name="levelUniqueName">Unique name of a level.</param>
        /// <param name="cubeName">Name of the cube</param>
        /// <returns>returns a MemberCollection object</returns>
        MemberCollection GetLevelMembers(string levelUniqueName, string cubeName);

        /// <summary>
        /// Gets the parent member.
        /// </summary>
        /// <param name="member">The member</param>
        /// <returns>returns a parent Member of the current member</returns>
        Member GetParentMember(Member member);

        /// <summary>
        /// Determines whether [is top level member] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        /// <c>true</c> if [is top level member] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        ////bool IsTopLevelMember(Member member);

        /// <summary>
        /// Gets the measures dimension unique name.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <returns>measures dimension unique name of type string</returns>
        string GetMeasuresDimensionUniqueName(string cubeName);

        /// <summary>
        /// Gets the all member unique name.
        /// </summary>
        /// <param name="cubeName">Name of the cube.</param>
        /// <param name="hierarchyUniqueName">Name of the hierarchy unique.</param>
        /// <returns>all member unique name of type string</returns>
        string GetAllMemberUniqueName(string cubeName, string hierarchyUniqueName);

        /// <summary>
        /// Executes the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="olapReport">The OLAP report.</param>
        /// <returns></returns>
        object Execute(string commandText, Reports.OlapReport olapReport);
        #endregion

        #region Private Properties
        /// <summary>
        /// Gets/sets the connection string for the data adapter;
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
        string CatalogName { get; }

        /// <summary>
        /// Gets the current cell set.
        /// </summary>
        /// <value>The current cell set.</value>
        CellSet CurrentCellSet { get; }

        /// <summary>
        /// Gets the get cubes.
        /// </summary>
        /// <value>The get cubes.</value>
        CubeInfoCollection GetCubes { get; }

        /// <summary>
        /// Gets the get all cubes.
        /// </summary>
        /// <value>The get all cubes.</value>
        CubeInfoCollection GetAllCubes { get; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        Providers ProviderName { get; set; }

        #endregion

        #region Private Methods
        /// <summary>
        /// Closes the connection.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Validates the connection string.
        /// </summary>
        /// <returns>true if connection string has proper syntax</returns>
        bool ValidateConnectionString();

        /// <summary>
        /// Determines whether [has valid cells].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has valid cells]; otherwise, <c>false</c>.
        /// </returns>
        bool HasValidCells();
        #endregion
    }

    /// <summary>
    /// This enumeration holds provider name.
    /// </summary>
    public enum Providers
    {
        /// <summary>
        /// Refers SQL Server Analysis Services.
        /// </summary>
        SSAS,
        /// <summary>
        /// Refers Mondrian XMLA Services.
        /// </summary>
        Mondrian,
        /// <summary>
        /// Refers Active Pivot XMLA Services.
        /// </summary>
        ActivePivot
    }

    public enum NodeTypes
    {
        Parent,
        Child
    }
}
