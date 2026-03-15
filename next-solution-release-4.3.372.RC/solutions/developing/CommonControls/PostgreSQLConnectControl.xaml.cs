using CommonControls.Converters;
using DevExpress.Xpo.DB;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UIMsgBoxAlertService.ComponentService;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for PostgreSQLConnectControl.xaml
    /// </summary>
    public partial class PostgreSQLConnectControl : UserControl
    {
        public PostgreSQLConnectControl()
        {
            InitializeComponent();
            tbServer.Text = Properties.Settings.Default.PostgreSQLDefaultHost;
            tbDatabase.Text = Properties.Settings.Default.PostgreSQLDefaultDatabase;
            portNumber.Value = Properties.Settings.Default.PostgreSQLDefaultPortName;
            tbTimezone.Text = Properties.Settings.Default.PostgreSQLDefaultTimezone;
        }

        public string GetDataBaseConnectionString()
        {
            if (!String.IsNullOrWhiteSpace(tbTimezone.Text))
                return String.Format("{0};Timezone={1}",PostgreSqlConnectionProvider.GetConnectionString(tbServer.Text, (int)portNumber.Value, tbLogin.Text, tbPassword.Text, tbDatabase.Text), tbTimezone.Text);
            return PostgreSqlConnectionProvider.GetConnectionString(tbServer.Text, (int)portNumber.Value, tbLogin.Text, tbPassword.Text, tbDatabase.Text);
        }

        internal bool IsValid
        {
            get 
            {
                return !String.IsNullOrEmpty(tbServer.Text) && !String.IsNullOrEmpty(tbDatabase.Text);
            }
        }
    }
}
