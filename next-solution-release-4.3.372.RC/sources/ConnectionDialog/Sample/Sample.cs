//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.ConnectionUI;
using System.Data.Common;
using System.Data;

namespace Sample
{
	public class Sample
	{

        public static IEnumerable<String> GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            DataTable table = DbProviderFactories.GetFactoryClasses();

            // Display each row and column value.
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    yield return row[column].ToString();
                }
            }
        }

        // Given a provider name and connection string, 
        // create the DbProviderFactory and DbConnection.
        // Returns a DbConnection on success; null on failure.
        static DbConnection CreateDbConnection(
            string providerName, string connectionString)
        {
            // Assume failure.
            DbConnection connection = null;

            // Create the DbProviderFactory and DbConnection.
            if (connectionString != null)
            {
                try
                {
                    DbProviderFactory factory =
                        DbProviderFactories.GetFactory(providerName);

                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionString;
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    if (connection != null)
                    {
                        connection = null;
                    }
                    Console.WriteLine(ex.Message);
                }
            }
            // Return the connection.
            return connection;
        }

        static DbDataAdapter CreateDbDataAdapter(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory =
                        DbProviderFactories.GetFactory(providerName);

                    return factory.CreateDataAdapter();
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    Console.WriteLine(ex.Message);
                }
            }
            // Return the connection.
            return null;
        }

        static DbCommand CreateDbCommand(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory =
                        DbProviderFactories.GetFactory(providerName);

                    return factory.CreateCommand();
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    Console.WriteLine(ex.Message);
                }
            }
            // Return the connection.
            return null;
        }

		// Sample 1: 
		[STAThread]
		static void Main(string[] args)
		{
			DataConnectionDialog dcd = new DataConnectionDialog();
			DataConnectionConfiguration dcs = new DataConnectionConfiguration(null);
			dcs.LoadConfiguration(dcd);

			if (DataConnectionDialog.Show(dcd) == DialogResult.OK)
			{
				// load tables

                var provider = dcd.SelectedDataProvider;
                List<String> listString = new List<string>();
                var list = GetProviderFactoryClasses();
                foreach (var l in list)
                    listString.Add(l);

                var connection = CreateDbConnection(provider.Name, dcd.ConnectionString);
				// using (SqlConnection connection = new SqlConnection(dcd.ConnectionString))
                using (connection)
				{
					connection.Open();
                    var dbdapater = CreateDbDataAdapter(provider.Name);
                    dbdapater.SelectCommand = CreateDbCommand(provider.Name);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = "SELECT * FROM sys.Tables";
                    var ds = new DataSet();
                    dbdapater.Fill(ds);

                    for (var iCol = 0; iCol < ds.Tables[0].Columns.Count; iCol++)
                    {
                        Console.WriteLine(String.Format("name : {0}, type : {1}", ds.Tables[0].Columns[iCol].ColumnName, ds.Tables[0].Columns[iCol].DataType));
                        // columns.Add(ds.Tables[0].Columns[iCol].ColumnName, ds.Tables[0].Columns[iCol].DataType);
                    }

                    /*
					SqlCommand cmd = new SqlCommand("SELECT * FROM sys.Tables", connection);

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							Console.WriteLine(reader.HasRows);
						}
					}
                    */
				}
			}

			dcs.SaveConfiguration(dcd);
		}

		// Sample 2: 
		//[STAThread]
		//static void Main(string[] args)
		//{
		//    DataConnectionDialog dcd = new DataConnectionDialog();
		//    DataConnectionConfiguration dcs = new DataConnectionConfiguration(null);
		//    dcs.LoadConfiguration(dcd);
		//    //dcd.ConnectionString = "Data Source=ziz-vspro-sql05;Initial Catalog=Northwind;Persist Security Info=True;User ID=sa;Password=Admin_007";


		//    if (DataConnectionDialog.Show(dcd) == DialogResult.OK)
		//    {
		//        // load tables
		//        using (SqlConnection connection = new SqlConnection(dcd.ConnectionString))
		//        {
		//            connection.Open();
		//            SqlCommand cmd = new SqlCommand("SELECT * FROM sys.Tables", connection);

		//            using (SqlDataReader reader = cmd.ExecuteReader())
		//            {
		//                while (reader.Read())
		//                {
		//                    Console.WriteLine(reader.HasRows);
		//                }
		//            }

		//        }
		//    }

		//    dcs.SaveConfiguration(dcd);
		//}
	}
}
