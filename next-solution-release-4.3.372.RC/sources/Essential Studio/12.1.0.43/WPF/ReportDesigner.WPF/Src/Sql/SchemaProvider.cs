//-------------------------------------------------------------------------------------------------
// <copyright file="SchemaProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Windows.Reports.Relational
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Ineterface provides the Database schema related information
    /// </summary>
    internal interface ISchemaProvider
    {
        #region Interface Methods Declatraion
        /// <summary>
        /// Get data base list from the specified data provider
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetDatabases();

        /// <summary>
        /// Get table list from the specified data base
        /// </summary>
        /// <param name="query">Represents the input sql query</param>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetQueryTables(string query);

        /// <summary>
        /// Get schema list from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetSchemas();

        /// <summary>
        /// Get stored procedures list from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetStoredProcedures();

        /// <summary>
        /// Get table column list from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetTableColumns();

        /// <summary>
        /// Get table relations from the specified data base
        /// </summary>
        /// <param name="tables">Represents the relational table names</param>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetTableRelations(string tables);

        /// <summary>
        /// Get tables from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetTables();

        /// <summary>
        /// Get table value functions from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetTableValueFunctions();

        /// <summary>
        /// Get view columns from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetViewColumns();

        /// <summary>
        /// Get views list from the specified data base
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        DataTable GetViews();

        #endregion
    }
}
