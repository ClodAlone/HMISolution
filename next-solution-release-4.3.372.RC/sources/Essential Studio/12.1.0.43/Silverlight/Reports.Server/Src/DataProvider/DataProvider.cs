//-------------------------------------------------------------------------------------------------
// <copyright file="DataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Syncfusion.Reports.Server.Data
{
    /// <summary>
    /// Defines the abstract methods of different type of data providers
    /// </summary>
    internal interface IDataProvider
    {
        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A schema.</returns>
        DataTable GetSchema(string query, DbConnection connection);

        /// <summary>
        /// Gets the schema column.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A schema column.</returns>
        DataTable GetSchemaColumn(string query, DbConnection connection);

        /// <summary>
        /// Gets the schema table.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A schema table.</returns>
        DataTable GetSchemaTable(string query, DbConnection connection);

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A table.</returns>
        DataTable GetTable(string query, DbConnection connection);

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="connectionstring">The connectionstring.</param>
        /// <param name="query">The query.</param>
        /// <returns>A Table.</returns>
        DataTable GetTable(string connectionstring, string query);

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="query">The query.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A Table.</returns>
        DataTable GetTable(string connectionString, string query, string tableName);        
        
    }
}
