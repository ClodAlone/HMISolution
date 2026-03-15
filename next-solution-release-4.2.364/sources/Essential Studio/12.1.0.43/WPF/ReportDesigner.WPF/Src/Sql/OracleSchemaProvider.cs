//-------------------------------------------------------------------------------------------------
// <copyright file="OracleSchemaProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Windows.Reports.Relational.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OracleClient;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using Syncfusion.Windows.Reports.Designer.Dialogs;
    using Syncfusion.RDL.Data;

    /// <summary>
    /// Interaction logic for Schema Provider for ORACLE Data Base.
    /// </summary>
    internal class OracleSchemaProvider : ISchemaProvider
    {
        #region Members
        /// <summary>
        /// Local member for Database Connection
        /// </summary>
        private DbConnection connection;        

        /// <summary>
        /// Internal member for User ID as string
        /// </summary>
        private string userId;
        #endregion        
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the OracleSchemaProvider class with DbConnection.
        /// </summary>
        /// <param name="con">Represents Data Base Connection as DbConnection</param>
        public OracleSchemaProvider(DbConnection con)
        {
            this.connection = con;
        }       

        /// <summary>
        /// Initializes a new instance of the OracleSchemaProvider class with DbConnection and User Id. 
        /// </summary>
        /// <param name="con">Represents the database connection</param>
        /// <param name="userID">Represents which user logged in</param>
        public OracleSchemaProvider(DbConnection con, string userID)           
        {
            this.connection = con;
            this.userId = userID;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets Query String.
        /// </summary>
        public string QueryString { get; set; }

        #endregion

        #region ISchemaProvider Members
        /// <summary>
        /// Get Data base list as a DataTable
        /// </summary>
        /// <returns>Containing as DataTable</returns>
        public System.Data.DataTable GetDatabases()
        {
            ////Oracle has only one Database called System.
            return null;
        }

        /// <summary>
        /// Get Table relcations with tables name
        /// </summary>
        /// <param name="tables">Represents the tables which are mentioning the relationships</param>
        /// <returns>Containing as a Data Table</returns>
        public System.Data.DataTable GetTableRelations(string tables)
        {
            /*string query = @"select object_name(f.referenced_object_id) AS LeftTable,'InnerJoin' as RelationShip,object_name(f.parent_object_id) AS RightTable, 
                            Col_name(fc.referenced_object_id, fc.referenced_column_id) AS 'ColumnName'
                            from sys.foreign_keys f
                            inner join sys.foreign_key_columns as fc
                                on f.object_id = fc.constraint_object_id
                            where object_name(f.parent_object_id) in (" + tables + @")
                            and object_name(f.referenced_object_id) in (" + tables + @")
                            and f.parent_object_id <> f.referenced_object_id";

            return new OracleDataProvider().GetSchemaColumn(query, base.Connection);*/
            return null;
        }

        /// <summary>
        /// Get the schema informations from data base for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetSchemas()
        {
            string query = "select distinct OWNER as schema from sys.all_tables where OWNER='" + this.userId + "'";

            return new OracleDataProvider().GetSchema(query, this.connection);
        }

        /// <summary>
        /// Get Table list from database for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTables()
        {
            string query = "select OWNER as schema, TABLE_NAME as name from sys.all_tables where OWNER='" + this.userId + "' order by 2";

            return new OracleDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get the Views from the database for the specified user login.
        /// </summary>
        /// <returns>Contains as a DataTable</returns>
        public System.Data.DataTable GetViews()
        {
            string query = "select OWNER as schema, VIEW_NAME as name from sys.all_views where OWNER='" + this.userId + "' order by 2";

            return new OracleDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Stored Procedures from the database for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetStoredProcedures()
        {
            string query = "select OWNER as schema, OBJECT_NAME as name from SYS.ALL_OBJECTS where upper(OBJECT_TYPE) = upper('PROCEDURE') and OWNER='" + this.userId + "' order by 2";

            return new OracleDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get Table value functions from the database for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTableValueFunctions()
        {
            string query = "select OWNER as schema, OBJECT_NAME as name from SYS.ALL_OBJECTS where upper(OBJECT_TYPE) = upper('FUNCTION') and OWNER='" + this.userId + "' order by 2";

            return new OracleDataProvider().GetSchemaTable(query, this.connection);
        }

        /// <summary>
        /// Get All Table Columns name from the database for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetTableColumns()
        {
            string query = "select t1.OWNER as schema_name, t2.TABLE_NAME as view_name, t2.COLUMN_NAME as col_name, DATA_TYPE as data_typename from sys.all_tables t1 LEFT OUTER JOIN cols t2 on t1.TABLE_NAME = t2.TABLE_NAME where t1.OWNER='" + this.userId + "' order by 2,3";

            return new OracleDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get All View Columns name from the database for the specified user login.
        /// </summary>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetViewColumns()
        {
            string query = "select t1.OWNER as schema_name, t1.VIEW_NAME as view_name, t2.COLUMN_NAME as col_name, DATA_TYPE as data_typename from sys.all_views t1,cols t2 where OWNER='" + this.userId + "' order by 2,3";

            return new OracleDataProvider().GetSchemaColumn(query, this.connection);
        }

        /// <summary>
        /// Get the Table from the database for the specied query
        /// </summary>
        /// <param name="query">Represents the query as string</param>
        /// <returns>Containing as a DataTable</returns>
        public System.Data.DataTable GetQueryTables(string query)
        {
            if (query.Contains(":"))
            {
                int paramRow = 1; // for setting the place for the TextBlock and TextBox in grid in QueryParameters window
                ParameterQuery parmQuery = new ParameterQuery();

                Dictionary<string, string> m_dicKeysValues = new Dictionary<string, string>();

                // find any Token for Parameterized Query followed by : Symbol
                Regex theReg = new Regex(@"([:])([A-Za-z0-9]+)", RegexOptions.IgnoreCase);

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
                            panel.SetValue(Grid.RowProperty, paramRow); // Setting position as Row in Grid.Row Attribute
                            panel.SetValue(Grid.ColumnProperty, 0);  // Setting position as Column in Grid.Column Attribute
                            TextBlock block = new TextBlock();
                            block.Text = theMatch.ToString().ToUpper();
                            block.TextAlignment = System.Windows.TextAlignment.Center;
                            panel.Children.Add(block);
                            parmQuery.gridParam.Children.Add(panel);

                            panel = null; // Flushing the StackPanel                                      

                            panel = new StackPanel();
                            panel.SetValue(Grid.RowProperty, paramRow); // Setting position as Row in Grid.Row Attribute
                            panel.SetValue(Grid.ColumnProperty, 1); // Setting position as Column in Grid.Column Attribute
                            TextBox box = new TextBox();
                            box.Name = "textBox" + paramRow; ////Setting name for each TextBox
                            box.TextAlignment = System.Windows.TextAlignment.Left;
                            panel.Children.Add(box);
                            parmQuery.gridParam.Children.Add(panel);
                            paramRow++; ////Increment the Row value for succeeding rows.
                        }
                        catch
                        {
                        }
                    }
                }

                OracleCommand orclCmd = new OracleCommand(query);
                if (m_dicKeysValues.Count > 0)
                {
                    if (parmQuery.ShowDialog() == true)
                    {
                        List<string> m_listKeys = new List<string>(m_dicKeysValues.Keys);
                        for (int i = 0; i < m_listKeys.Count; i++)
                        {
                            orclCmd.Parameters.AddWithValue(m_listKeys[i].ToString(), parmQuery.ListText[i].ToString());
                        }
                    }
                    else
                    {
                        DataTable nullTable = new DataTable();
                        nullTable.Clear();
                        return nullTable;
                    }
                }

                return new OracleDataProvider().GetTable(this.connection, orclCmd);
            }

            return new OracleDataProvider().GetSchemaColumn(query, this.connection);
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
            query.Append("SELECT ");
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
                                query.Append(",");
                            }

                            query.Append(item.Key + "." + tableColumn.Key);
                            tempInt++;
                        }
                    }

                    tableArray.Add(item.GetParentSchema(item).ToString() + "." + item.Key);
                }
            }

            query.Append(" FROM ");
            ////Append the tables with default joins
            if (tableArray.Count == 1)
            {
                query.Append(tableArray[0]);
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
            query.Append("SELECT ");
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
                                query.Append(",");
                            }

                            query.Append(item.Key + "." + tableColumn.Key);
                            tempInt++;
                        }
                    }

                    tableArray.Add(item.GetParentSchema(item).ToString() + "." + item.Key);
                }
            }

            query.Append(" FROM ");
            ////Append the tables with default joins
            if (tableArray.Count == 1)
            {
                query.Append(tableArray[0]);
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
                                queryString.Append(" INNER JOIN ");
                                columnName = dataRow.ItemArray[3].ToString();
                                innerJoinAppended = true;
                                i++;
                                break;
                            }
                            else
                            {
                                innerJoinAppended = false;
                                columnName = string.Empty;
                                queryString.Append(" CROSS JOIN ");
                                break;
                            }
                        }
                    }
                    else
                    {
                        innerJoinAppended = false;
                        columnName = string.Empty;
                        queryString.Append(" CROSS JOIN ");
                    }
                }

                queryString.Append(item);
                if (innerJoinAppended)
                {
                    StringBuilder tempQuery = new StringBuilder();
                    tempQuery.Append(" ON ");
                    tempQuery.Append(lastTable + "." + columnName);
                    tempQuery.Append(" = ");
                    tempQuery.Append(item + "." + columnName);
                    tempQuery.Append(" ");
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
                    queryString.Append(" CROSS JOIN ");
                }

                queryString.Append(item);
                tempCount++;
            }

            return queryString;
        }
        #endregion        
    }        
}
