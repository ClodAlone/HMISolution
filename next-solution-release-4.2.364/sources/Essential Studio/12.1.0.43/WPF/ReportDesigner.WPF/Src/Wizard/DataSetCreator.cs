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
using System.Data;
using System.Data.SqlClient;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    internal static class DataSetCreator
    {
        public static DataSet CreateDataSet(string connectionString)
        {
            SqlDataAdapter dataAdapter;
            DataSet ds = new DataSet();

            // Create the parent table.
            // ***************************************
            DataTable dataTable = new DataTable("Schema");
            string query = @"select distinct sys.schemas.schema_id as [schema_id],sys.schemas.name as [schema] from sys.tables left outer join sys.schemas on sys.tables.schema_id = sys.schemas.schema_id";
            dataAdapter = new SqlDataAdapter(query,connectionString);
            dataAdapter.Fill(dataTable);
            ds.Tables.Add(dataTable);
            dataAdapter.Dispose();

            // Create the child table.
            // ***************************************
            dataTable = new DataTable("Tables");
            query = @"select sys.tables.name as [name],sys.schemas.name as [schema],sys.schemas.schema_id as [schema_id] from sys.tables left outer join sys.schemas on sys.tables.schema_id = sys.schemas.schema_id";
            dataAdapter = new SqlDataAdapter(query, connectionString);
            dataAdapter.Fill(dataTable);
            ds.Tables.Add(dataTable);
            dataAdapter.Dispose();

            // Create the child table.
            // ***************************************
            dataTable = new DataTable("Views");
            query = @"select sys.views.name as [name],sys.schemas.name as [schema],sys.schemas.schema_id as [schema_id] from sys.views left outer join sys.schemas on sys.views.schema_id = sys.schemas.schema_id";
            dataAdapter = new SqlDataAdapter(query, connectionString);
            dataAdapter.Fill(dataTable);
            ds.Tables.Add(dataTable);
            dataAdapter.Dispose();

            // Create the child table.
            // ***************************************
            dataTable = new DataTable("Stored Procedures");
            query = @"select sys.views.name as [name],sys.schemas.name as [schema],sys.schemas.schema_id as [schema_id] from sys.views left outer join sys.schemas on sys.views.schema_id = sys.schemas.schema_id";
            dataAdapter = new SqlDataAdapter(query, connectionString);
            dataAdapter.Fill(dataTable);
            ds.Tables.Add(dataTable);
            dataAdapter.Dispose();

            // Create the child table.
            // ***************************************
            dataTable = new DataTable("Table-valued Functions");
            query = @"select sys.objects.name as [name],sys.schemas.name as [schema],sys.schemas.schema_id as [schema_id] from sys.objects left outer join sys.schemas on sys.objects.schema_id = sys.schemas.schema_id where [type] = 'TF'";
            dataAdapter = new SqlDataAdapter(query, connectionString);
            dataAdapter.Fill(dataTable);
            ds.Tables.Add(dataTable);
            dataAdapter.Dispose();
            
            DataColumn[] parentColumn = new DataColumn[1];
            DataColumn[] childColumn = new DataColumn[1];
            parentColumn[0] = ds.Tables["Schema"].Columns["schema_id"];
            childColumn[0] = ds.Tables["Tables"].Columns["schema_id"];
            //childColumn[1] = ds.Tables["Views"].Columns["schema_id"];
            //childColumn[2] = ds.Tables["Stored Procedures"].Columns["schema_id"];
            //childColumn[3] = ds.Tables["Table-valued Functions"].Columns["schema_id"];

            // Associate the tables.
            // ***************************************
            ds.Relations.Add(
                "Schema2Tables",
                parentColumn,
                childColumn);

            return ds;
        }
    }
}