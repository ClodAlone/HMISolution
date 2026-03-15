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
    /// Interaction logic for SQLAzureConnectControl.xaml
    /// </summary>
    public partial class SQLAzureConnectControl : UserControl
    {
        public SQLAzureConnectControl()
        {
            InitializeComponent();
            label1.Content = String.Format("{0} {1}", Properties.Resources.lblSQLAzure, Properties.Settings.Default.lblSQLAzurePrefix);
            label3.Content = Properties.Settings.Default.lblSQLAzureSuffix;
        }

        public string GetDataBaseConnectionString()
        {
            string connectionString = GetServerConnectionString();
            return String.Format("{0};initial catalog={1}", connectionString, cbDatabase.Text);
        }
        internal bool IsValid
        {
            get { return !string.IsNullOrEmpty(cbDatabase.Text); }
        }

        string GetServerConnectionString()
        {
            string connectionString = String.Format("data source={0};user id={1};password={2}", tbServer.Text, tbLogin.Text, tbPassword.Password);
            return connectionString;
        }
    }
}
