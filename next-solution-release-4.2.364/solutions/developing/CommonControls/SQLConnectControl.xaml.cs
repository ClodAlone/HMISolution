using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Sql;
using Utilities;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for SQLConnectControl.xaml
    /// </summary>
    public partial class SQLConnectControl : UserControl
    {
        public SQLConnectControl()
        {
            InitializeComponent();
        }

        internal bool IsValid
        {
           get { return cbDatabase.SelectedItem != null || !string.IsNullOrEmpty(cbDatabase.Text); }
        }

        bool bDataSourcesProcessed;
        private void tbServer_PopupOpening(object sender, EventArgs e)
        {
            if (bDataSourcesProcessed)
                return;
            bDataSourcesProcessed = true;

            using (var cursor = new WaitCursor())
            {
                SqlDataSourceEnumerator instance =
                      SqlDataSourceEnumerator.Instance;
                using (System.Data.DataTable table = instance.GetDataSources())
                {
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        String name = String.Empty;
                        foreach (System.Data.DataColumn col in table.Columns)
                        {
                            if (col.ColumnName == "ServerName")
                                name = row[col].ToString();
                            else
                                if (col.ColumnName == "InstanceName")
                                {
                                    var instanceName = row[col].ToString();
                                    if (!String.IsNullOrEmpty(instanceName))
                                        name = String.Format("{0}\\{1}", name, instanceName);
                                }
                        }
                        if (!String.IsNullOrEmpty(name))
                        {
                            tbServer.Items.Add(name);
                            name = String.Empty;
                        }
                    }
                }
            }
        }


        private void tbServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbDatabase.Items.Clear();
        }

        private void cbDatabase_PopupOpening(object sender, EventArgs e)
        {
            using (new WaitCursor())
            {
                cbDatabase.Items.Clear();
                using (SqlConnection sqlConx = new SqlConnection(GetServerConnectionString()))
                {
                    sqlConx.Open();
                    using (DataTable tblDatabases = sqlConx.GetSchema("Databases"))
                    {
                        sqlConx.Close();
                        foreach (DataRow row in tblDatabases.Rows)
                        {
                            cbDatabase.Items.Add(row["database_name"].ToString());
                        }
                    }
                }                
            }
        }

        public string GetDataBaseConnectionString()
        {
            string connectionString = GetServerConnectionString();
            return String.Format("{0};initial catalog={1}", connectionString, cbDatabase.Text);
        }
        string GetServerConnectionString()
        {
            string connectionString;
            if (!rbWindowsAuthentication.IsChecked.Value)
                connectionString = String.Format("data source={0};user id={1};password={2}", tbServer.Text, tbLogin.Text, tbPassword.Password);
            else
                connectionString = String.Format("data source={0};integrated security=SSPI", tbServer.Text);
            return connectionString;
        }

        private void tbServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            cbDatabase.Items.Clear();
        }

    }
}
