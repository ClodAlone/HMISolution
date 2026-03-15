//-------------------------------------------------------------------------------------------------
// <copyright file="SqlSchemaProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace Syncfusion.Windows.Reports.Relational.Sql
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using Syncfusion.Windows.Reports.Sql;
    using Syncfusion.Windows.Reports.Designer.Dialogs;
    using Syncfusion.RDL.Data;

    /// <summary>
    /// Interaction logic for Schema Provider for SQL Server Data Base.
    /// </summary>
    internal class SqlSchemaProvider : ISchemaProvider
    {
        #region Member
        /// <summary>
        /// Represents the Database connection.
        /// </summary>
        private DbConnection connection;
        #endregion       

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SqlSchemaProvider class with the input connection
        /// </summary>
        /// <param name="con">Represents the database connection</param>
        public SqlSchemaProvider(DbConnection con)
        {
            this.connection = con;
        }
        #endregion       

        #region Public Properties
        /// <summary>
        /// Gets or sets the Query string
        /// </summary>
        public string QueryString { get; set; }
        #endregion 

        #region ISchemaProvider Members

        /// <summary>
        /// Get Database List from the given SQLServer's server name
        /// </summary>
        /// <returns>Containing as a  DataTable</returns>        
        public System.Data.DataTable GetDatabases()
        {
            string query = @"select name as [DataBase Name] from sys.sysDatabases order by 1";

            return new SqlDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get Table Relations from the given tables
        /// </summary>
        /// <param name="tables">Table name</param>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTableRelations(string tables)
        {
            string query = @"select object_name(f.referenced_object_id) AS LeftTable,'InnerJoin' as RelationShip,object_name(f.parent_object_id) AS RightTable, 
                            Col_name(fc.referenced_object_id, fc.referenced_column_id) AS 'ColumnName'
                            from sys.foreign_keys f
                            inner join sys.foreign_key_columns as fc
                                on f.object_id = fc.constraint_object_id
                            where object_name(f.parent_object_id) in (" + tables + @")
                            and object_name(f.referenced_object_id) in (" + tables + @")
                            and f.parent_object_id <> f.referenced_object_id";

            return new SqlDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get Schema List from the Database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetSchemas()
        {
            string query = @"select distinct sys.schemas.name as [schema] from 
                            sys.tables left outer join sys.schemas
                            on sys.tables.schema_id = sys.schemas.schema_id order by 1";

            return new SqlDataProvider().GetSchema(query, this.connection);
        }

        /// <summary>
        /// Get Tables from the Database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTables()
        {
            string query = @"select sys.schemas.name as [schema],sys.tables.name as [name] from 
                            sys.tables left outer join sys.schemas
                            on sys.tables.schema_id = sys.schemas.schema_id order by 1";

            return new SqlDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Views from the Database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetViews()
        {
            string query = @"select sys.schemas.name as [schema],
                            sys.views.name as [name]
                            from sys.views left outer join sys.schemas
                            on sys.views.schema_id = sys.schemas.schema_id order by 2";

            return new SqlDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Stored Procedures from the database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetStoredProcedures()
        {
            string query = @"select sys.schemas.name as [schema],
                            sys.procedures.name as [name]
                            from sys.procedures
                            left outer join sys.schemas
                            on sys.procedures.schema_id = sys.schemas.schema_id order by 2";

            return new SqlDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Table Value Functions from the database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTableValueFunctions()
        {
            string query = @"select sys.schemas.name as [schema],
                            sys.objects.name as [name]
                            from sys.objects left outer join sys.schemas
                            on sys.objects.schema_id = sys.schemas.schema_id where [type] = 'TF' order by 2";

            return new SqlDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Table Columns details from the database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTableColumns()
        {
            string query = @"SELECT
                            sys.schemas.name as schema_name,
                            sys.tables.name as view_name,
                            sys.columns.name as col_name, 
							sys.types.name as data_typename
                            FROM sys.columns
                            INNER JOIN sys.tables ON sys.tables.object_id = sys.columns.object_id
                            INNER JOIN sys.schemas ON sys.schemas.schema_id = sys.tables.schema_id
							LEFT OUTER JOIN sys.types ON sys.columns.system_type_id = sys.types.system_type_id
							where sys.types.system_type_id = sys.types.user_type_id
                            order by 1,2,3";

            return new SqlDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get View Columns details from the database
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetViewColumns()
        {
            string query = @"SELECT
                            sys.schemas.name as schema_name,
                            sys.views.name as view_name,
                            sys.columns.name as col_name, 
							sys.types.name as data_typename
                            FROM sys.columns
                            INNER JOIN sys.views ON sys.views.object_id = sys.columns.object_id
                            INNER JOIN sys.schemas ON sys.schemas.schema_id = sys.views.schema_id
							LEFT OUTER JOIN sys.types ON sys.columns.system_type_id = sys.types.system_type_id
							where sys.types.system_type_id = sys.types.user_type_id
                            order by 1,2,3";

            return new SqlDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get Tables from the database through the given query string
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetQueryTables(string query)
        {
            if (query.Contains("@"))
            {
                int paramRow = 1; // for setting the place for the TextBlock and TextBox in grid in QueryParameters window

                ParameterQuery parmQuery = new ParameterQuery();

                Dictionary<string, string> m_dicKeysValues = new Dictionary<string, string>();

                // find any Token for Parameterized Query followed by '@' Symbol
                Regex theReg = new Regex(@"(\@)([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);

                // get the collection of matches
                MatchCollection theMatches = theReg.Matches(query);

                // iterate through the collection
                foreach (Match theMatch in theMatches)
                {
                    if (theMatch.Length != 0)
                    {
                        try
                        {
                            m_dicKeysValues.Add(theMatch.ToString().ToUpper(), string.Empty);

                            StackPanel panel = new StackPanel();
                            panel.Height = 25;
                            panel.Width = 300;
                            panel.Orientation = Orientation.Horizontal;
                            //panel.SetValue(Grid.RowProperty, paramRow); // Setting position as Row in Grid.Row Attribute
                            //panel.SetValue(Grid.ColumnProperty, 0);  // Setting position as Column in Grid.Column Attribute
                            TextBlock block = new TextBlock();
                            block.Height = 20;
                            block.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            block.Width = 150;
                            block.Text = theMatch.ToString().ToUpper();
                            block.TextAlignment = System.Windows.TextAlignment.Center;
                            panel.Children.Add(block);
                          //  parmQuery.gridParam.Children.Add(panel);

                           // panel = null; // Flushing the StackPanel                                      

                            //panel = new StackPanel();
                            //panel.SetValue(Grid.RowProperty, paramRow); // Setting position as Row in Grid.Row Attribute
                            //panel.SetValue(Grid.ColumnProperty, 1); // Setting position as Column in Grid.Column Attribute
                            TextBox box = new TextBox();
                            box.Width = 130;
                            box.Height = 20;
                            box.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                            box.Name = "textBox" + paramRow; ////Setting name for each TextBox
                            box.TextAlignment = System.Windows.TextAlignment.Left;
                            panel.Children.Add(box);
                            parmQuery.Mainpanel.Children.Add(panel);
                            paramRow++; ////Increment the Row value for succeeding rows.
                        }
                        catch
                        {
                        }
                    }
                }

                SqlCommand sqlCmd = new SqlCommand(query);
                if (m_dicKeysValues.Count > 0)
                {
                    if (parmQuery.ShowDialog() == true)
                    {
                        List<string> m_listKeys = new List<string>(m_dicKeysValues.Keys);
                        for (int i = 0; i < m_listKeys.Count; i++)
                        {
                            sqlCmd.Parameters.AddWithValue(m_listKeys[i].ToString(), parmQuery.ListText[i].ToString());
                        }
                    }
                    else
                    {
                        DataTable nullTable = new DataTable();
                        nullTable.Clear();
                        return nullTable;
                    }
                }

                return new SqlDataProvider().GetTable(this.connection, sqlCmd);
            }

            return new SqlDataProvider().GetSchemaColumn(query, this.connection);
        }
        
        #endregion

        #region Helper Methods
        /// <summary>
        /// Genereate Query Results for the database through given information as List of Schemas
        /// </summary>
        /// <param name="schemaInfos">List of Schema informations</param>
        public void GetQueryResults(List<SchemaInfo> schemaInfos)
        {
            StringBuilder query = new StringBuilder();
            List<string> tableArray = new List<string>();
            int tempInt = 1;
            query.Append("SELECT \n\t");
            foreach (var item in schemaInfos)
            {
                if (item.IsSelected)
                {
                    foreach (var tableColumn in item.SchemaInfos)
                    {
                        if (tableColumn.IsSelected)
                        {
                            if (tempInt > 1)
                            {
                                query.Append("\n\t,");
                            }

                            query.Append(SqlUtil.QuoteIdentifier(item.Key) + "." + SqlUtil.QuoteIdentifier(tableColumn.Key));
                            tempInt++;
                        }
                    }

                    tableArray.Add(SqlUtil.QuoteIdentifier(item.GetParentSchema(item).ToString()) + "." + SqlUtil.QuoteIdentifier(item.Key));
                }
            }

            query.Append("\n FROM ");
            ////Append the tables with default joins
            if (tableArray.Count == 1)
            {
                if (this.connection is SqlConnection)
                {
                    query.Append("\n\t" + tableArray[0]);
                }

                else
                {
                    query.Append("\n\t" + tableArray[0].Substring(9));
                }
            }
            else if (tableArray.Count > 1)
            {
                    query.Append(this.AddTableWithJoins(tableArray).ToString());
            }

            this.QueryString = query.ToString();
        }

        /// <summary>
        /// Genereate Query Results for the relational tables of the database
        /// </summary>
        /// <param name="schemaInfos">List of Schema Informations</param>
        /// <param name="dataTableRelations">DataTable contains relationships among the table</param>
        public void GetQueryResults(List<SchemaInfo> schemaInfos, DataTable dataTableRelations)
        {
            StringBuilder query = new StringBuilder();
            List<string> tableArray = new List<string>();
            int tempInt = 1;
            query.Append("SELECT \n\t");
            foreach (var item in schemaInfos)
            {
                if (item.IsSelected)
                {
                    foreach (var tableColumn in item.SchemaInfos)
                    {
                        if (tableColumn.IsSelected)
                        {
                            if (tempInt > 1)
                            {
                                query.Append("\n\t,");
                            }

                            query.Append(SqlUtil.QuoteIdentifier(item.Key) + "." + SqlUtil.QuoteIdentifier(tableColumn.Key));
                            tempInt++;
                        }
                    }

                    tableArray.Add(SqlUtil.QuoteIdentifier(item.GetParentSchema(item).ToString()) + "." + SqlUtil.QuoteIdentifier(item.Key));
                }
            }

            query.Append("\n FROM ");
            ////Append the tables with default joins
            if (tableArray.Count == 1)
            {
                if (this.connection is SqlConnection)
                {
                    query.Append("\n\t" + tableArray[0]);
                }
                else
                {
                    query.Append("\n\t" + tableArray[0].Substring(9));
                }
            }
            else if (tableArray.Count > 1)
            {
                    query.Append(this.AddTableWithJoins(tableArray, dataTableRelations).ToString());
            }

            this.QueryString = query.ToString();
        }

        /// <summary>
        /// Generate Query for adding tables with joins
        /// </summary>
        /// <param name="tableArray">List of string contains Table Names</param>
        /// <param name="dataTableRelations">DataTable contains relationships among the table</param>
        /// <returns>String Builder contains the join query </returns>
        private StringBuilder AddTableWithJoins(List<string> tableArray, DataTable dataTableRelations)
        {
            StringBuilder queryString = new StringBuilder();
            int tempCount = 1;
            int i = 0;
            string lastTable = string.Empty;
            string columnName = string.Empty;
            bool innerJoinAppended = false;
            foreach (var item in tableArray)
            {
                if (tempCount > 1)
                {
                    if (dataTableRelations.Rows.Count > 0)
                    {
                        for (; i < dataTableRelations.Rows.Count;)
                        {
                            DataRow dataRow = dataTableRelations.Rows[i];
                            if ((lastTable.ToLower().Contains(dataRow.ItemArray[0].ToString().ToLower())
                               && item.ToLower().Contains(dataRow.ItemArray[2].ToString().ToLower()))
                               || (item.ToLower().Contains(dataRow.ItemArray[0].ToString().ToLower())
                               && lastTable.ToLower().Contains(dataRow.ItemArray[2].ToString().ToLower())))
                            {
                                queryString.Append("\n\tINNER JOIN ");
                                columnName = dataRow.ItemArray[3].ToString();
                                innerJoinAppended = true;
                                i++;
                                break;
                            }
                            else
                            {
                                innerJoinAppended = false;
                                columnName = string.Empty;
                                queryString.Append("\n\tCROSS JOIN ");
                                break;
                            }
                        }
                    }
                    else
                    {
                        innerJoinAppended = false;
                        columnName = string.Empty;
                        queryString.Append("\n\tCROSS JOIN ");
                    }
                }

                queryString.Append(item);
                if (innerJoinAppended)
                {
                    StringBuilder tempQuery = new StringBuilder();
                    tempQuery.Append("\n\t ON ");
                    tempQuery.Append(lastTable + "." + SqlUtil.QuoteIdentifier(columnName));
                    tempQuery.Append(" = ");
                    tempQuery.Append(item + "." + SqlUtil.QuoteIdentifier(columnName));
                    tempQuery.Append("\n\t");
                    queryString.Append(tempQuery.ToString());
                }

                lastTable = item;
                tempCount++;
            }

            return queryString;
        }

        /// <summary>
        /// Generate Query for adding tables with joins
        /// </summary>
        /// <param name="tableArray">List of string contains Table Names</param>        
        /// <returns>String Builder Contains the query</returns>
        private StringBuilder AddTableWithJoins(List<string> tableArray)
        {
            StringBuilder queryString = new StringBuilder();
            int tempCount = 1;
            foreach (var item in tableArray)
            {

                if (tempCount > 1)
                {
                    queryString.Append("\n\tCROSS JOIN ");
                }

                if (this.connection is SqlConnection)
                {
                    queryString.Append(item);
                }

                else
                {
                    queryString.Append(item.Substring(9));
                }

                tempCount++;
            }

            return queryString;
        }

        #endregion
    }
}
