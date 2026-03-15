using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using HelpProvider.ComponentService;
using Ookii.Dialogs.Wpf;
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.Core;
using System.ComponentModel;

namespace CommonControls
{
    public enum ProviderTypes : int
    {
        SQLServer = 0x01,
        SQLAzure = 0x02,
        Access = 0x04,
        XmlFile = 0x08,
        MySQL = 0x10,
        SQLite = 0x12,
        PostgreSQL = 0x14,
        All = 0xFF
    }

    /// <summary>
    /// Interaction logic for ConnectionWizard.xaml
    /// </summary>
    public partial class ConnectionWizard : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Declarations
        IUIMsgBoxAlertService uiService;
        IHelpProvider helpProvider;
        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        private static readonly String integratedSecurityHeader = "integrated security";
        private static readonly String useridHeader = "user id";
        private static readonly String passwordHeader = "password";
        private static readonly String mysqlserverHeader = "server";
        private static readonly String mysqldatabaseHeader = "database";
        private static readonly String portNumberHeader = "port";
        private static readonly String sqliteDateTimeKindHeader = "DateTimeKind";
        private static readonly String postgreTimezoneHeader = "Timezone";
        #endregion

        public ConnectionWizard(IUIMsgBoxAlertService uiservice, IHelpProvider helpProvider) :
            this(ProviderTypes.All, uiservice, helpProvider)
        { }

        public ConnectionWizard(ProviderTypes selectionTypes, IUIMsgBoxAlertService uiservice, IHelpProvider helpProvider)
        {
            InitializeComponent();

            UIService = uiservice;
            this.helpProvider = helpProvider;

            if ((selectionTypes & ProviderTypes.SQLServer) == 0)
                tabSQL.Visibility = Visibility.Collapsed;
            if ((selectionTypes & ProviderTypes.SQLAzure) == 0)
                tabSQLAzure.Visibility = Visibility.Collapsed;
            if ((selectionTypes & ProviderTypes.Access) == 0)
                tabAccess.Visibility = Visibility.Collapsed;
            if ((selectionTypes & ProviderTypes.XmlFile) == 0)
                tabXML.Visibility = Visibility.Collapsed;
            if ((selectionTypes & ProviderTypes.MySQL) == 0)
                tabMySQL.Visibility = Visibility.Collapsed;
            if ((selectionTypes & ProviderTypes.PostgreSQL) == 0)
                tabPostgreSQL.Visibility = Visibility.Collapsed;

            /*
            if (System.Environment.Is64BitProcess)
                tabControlExt1.Items.Remove(tabAccess);
            */

            if ((selectionTypes & ProviderTypes.SQLServer) != 0)
            {
                sqlConnectControl.tbServer.Loaded += (o, e) =>
                {
                    var editTextBox = sqlConnectControl.tbServer.Template.FindName("PART_EditableTextBox", sqlConnectControl.tbServer) as TextBox;
                    if (editTextBox != null)
                        editTextBox.TextChanged += tbServer_TextInput;
                };
                sqlConnectControl.cbDatabase.Loaded += (o, e) =>
                    {
                        var editTextBox = sqlConnectControl.cbDatabase.Template.FindName("PART_EditableTextBox", sqlConnectControl.cbDatabase) as TextBox;
                        if (editTextBox != null)
                            editTextBox.TextChanged += tbServer_TextInput;
                    };

                sqlConnectControl.tbLogin.TextChanged += tbServer_TextInput;
                sqlConnectControl.tbPassword.EditValueChanged += tbServer_TextInput;
                sqlConnectControl.rbWindowsAuthentication.Unchecked += rbWindowsAuthentication_Unchecked;
                sqlConnectControl.rbWindowsAuthentication.Checked += rbWindowsAuthentication_Unchecked;
                sqlConnectControl.rbSQLServerAuthentication.Unchecked += rbWindowsAuthentication_Unchecked;
                sqlConnectControl.rbSQLServerAuthentication.Checked += rbWindowsAuthentication_Unchecked;
            }

            if ((selectionTypes & ProviderTypes.SQLAzure) != 0)
            {
                sqlAzureConnectControl.tbServer.Loaded += (o, e) =>
                {
                    var editTextBox = sqlAzureConnectControl.tbServer.Template.FindName("PART_EditableTextBox", sqlAzureConnectControl.tbServer) as TextBox;
                    if (editTextBox != null)
                        editTextBox.TextChanged += tbServer_TextInput;
                };
                sqlAzureConnectControl.cbDatabase.Loaded += (o, e) =>
                {
                    var editTextBox = sqlAzureConnectControl.cbDatabase.Template.FindName("PART_EditableTextBox", sqlAzureConnectControl.cbDatabase) as TextBox;
                    if (editTextBox != null)
                        editTextBox.TextChanged += tbServer_TextInput;
                };

                sqlAzureConnectControl.tbServer.TextChanged += tbServer_TextInput;
                sqlAzureConnectControl.cbDatabase.TextChanged += tbServer_TextInput;
                sqlAzureConnectControl.tbLogin.TextChanged += tbServer_TextInput;
                sqlAzureConnectControl.tbPassword.EditValueChanged += tbServer_TextInput;
            }

            if ((selectionTypes & ProviderTypes.Access) != 0)
                buttonEditAccess.TextChanged += tbServer_TextInput;

            if ((selectionTypes & ProviderTypes.XmlFile) != 0)
                buttonEditXML.TextChanged += tbServer_TextInput;

            if ((selectionTypes & ProviderTypes.MySQL) != 0)
            {
                mysqlConnectControl.tbServer.TextChanged += tbServer_TextInput;
                mysqlConnectControl.tbDatabase.TextChanged += tbServer_TextInput;
                mysqlConnectControl.tbLogin.TextChanged += tbServer_TextInput;
                mysqlConnectControl.tbPassword.EditValueChanged += tbServer_TextInput;
            }

            if ((selectionTypes & ProviderTypes.SQLite) != 0)
            {
                sqliteConnectControl.tbDatabase.TextChanged += tbServer_TextInput;                
                sqliteConnectControl.tbPassword.EditValueChanged += tbServer_TextInput;
                sqliteConnectControl.cbDateTimeKind.SelectedIndexChanged += tbServer_TextInput;
                sqliteConnectControl.cbDateTimeKind.SelectedIndex = 1;
            }

            if ((selectionTypes & ProviderTypes.PostgreSQL) != 0)
            {
                postgresqlConnectControl.tbServer.TextChanged += tbServer_TextInput;
                postgresqlConnectControl.portNumber.EditValueChanged += tbServer_TextInput;
                postgresqlConnectControl.tbDatabase.TextChanged += tbServer_TextInput;
                postgresqlConnectControl.tbLogin.TextChanged += tbServer_TextInput;
                postgresqlConnectControl.tbPassword.EditValueChanged += tbServer_TextInput;
                postgresqlConnectControl.tbTimezone.TextChanged += tbServer_TextInput;
            }

            tabControlExt1.SelectionChanged += (s, e) =>
            {
                if (bDisposed)
                    return;

                UpdateConnectionString();
            };
        }

        void rbWindowsAuthentication_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateConnectionString();
        }

        void tbServer_TextInput(object sender, RoutedEventArgs e)
        {
            UpdateConnectionString();
        }

        bool bFillingParameters;
        void FillingFromConnectionString()
        {
            if (bFillingParameters)
                return;
            bFillingParameters = true;
            try
            {
                var helper = new ConnectionStringParser(ConnectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (providerType == InMemoryDataStore.XpoProviderTypeString)
                {
                    buttonEditXML.Text = helper.GetPartByName(DataSourceHeader);
                    tabXML.Visibility = Visibility.Visible;
                    tabXML.IsSelected = true;
                }
                else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
                {
                    buttonEditAccess.Text = helper.GetPartByName(DataSourceHeader);
                    tabAccess.Visibility = Visibility.Visible;
                    tabAccess.IsSelected = true;
                }
                else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
                {
                    var serverText = helper.GetPartByName(DataSourceHeader);
                    if (serverText.Contains(Properties.Settings.Default.lblSQLAzureSuffix))
                    {
                        sqlAzureConnectControl.cbDatabase.Text = helper.GetPartByName(CatalogSourceHeader);
                        sqlAzureConnectControl.tbServer.Text = serverText.Substring(Properties.Settings.Default.lblSQLAzurePrefix.Length, serverText.Length - Properties.Settings.Default.lblSQLAzurePrefix.Length - Properties.Settings.Default.lblSQLAzureSuffix.Length);
                        sqlAzureConnectControl.tbLogin.Text = helper.GetPartByName(useridHeader);
                        sqlAzureConnectControl.tbPassword.Password = helper.GetPartByName(passwordHeader);
                        editConnectionString.Text = MSSqlConnectionProvider.GetConnectionString(
                            String.Format("{0}{1}{2}", Properties.Settings.Default.lblSQLAzurePrefix, sqlAzureConnectControl.tbServer.Text, Properties.Settings.Default.lblSQLAzureSuffix),
                            sqlAzureConnectControl.tbLogin.Text,
                            "",
                            sqlAzureConnectControl.cbDatabase.Text);
                        tabSQLAzure.IsSelected = true;
                    }
                    else
                    {
                        sqlConnectControl.cbDatabase.Text = helper.GetPartByName(CatalogSourceHeader);
                        sqlConnectControl.tbServer.Text = serverText;
                        sqlConnectControl.tbLogin.Text = helper.GetPartByName(useridHeader);
                        sqlConnectControl.tbPassword.Password = helper.GetPartByName(passwordHeader);
                        sqlConnectControl.rbWindowsAuthentication.IsChecked =
                            !String.IsNullOrEmpty(helper.GetPartByName(integratedSecurityHeader));
                        sqlConnectControl.rbSQLServerAuthentication.IsChecked =
                            String.IsNullOrEmpty(helper.GetPartByName(integratedSecurityHeader));
                        editConnectionString.Text = MSSqlConnectionProvider.GetConnectionString(
                            sqlConnectControl.tbServer.Text,
                            sqlConnectControl.tbLogin.Text,
                            "",
                            sqlConnectControl.cbDatabase.Text);
                        tabSQL.IsSelected = true;
                    }
                }
                else if(providerType == MySqlConnectionProvider.XpoProviderTypeString)
                {
                    mysqlConnectControl.tbServer.Text = helper.GetPartByName(mysqlserverHeader);
                    mysqlConnectControl.tbDatabase.Text = helper.GetPartByName(mysqldatabaseHeader);
                    mysqlConnectControl.tbLogin.Text = helper.GetPartByName(useridHeader);
                    mysqlConnectControl.tbPassword.Password = helper.GetPartByName(passwordHeader);
                    editConnectionString.Text = MySqlConnectionProvider.GetConnectionString(
                        mysqlConnectControl.tbServer.Text,
                        mysqlConnectControl.tbLogin.Text,
                        "",
                        mysqlConnectControl.tbDatabase.Text);
                    tabMySQL.IsSelected = true;
                }
                else if (providerType == SQLiteConnectionProvider.XpoProviderTypeString)
                {
                    sqliteConnectControl.tbDatabase.Text = helper.GetPartByName(DataSourceHeader);
                    sqliteConnectControl.tbPassword.Password = helper.GetPartByName(passwordHeader);
                    if (helper.PartExists(sqliteDateTimeKindHeader))
                    {
                        var key = helper.GetPartByName(sqliteDateTimeKindHeader);
                        foreach (SQLiteDateTimeKind sqliteDateTimeKind in Enum.GetValues(typeof(SQLiteDateTimeKind)))
                        {
                            if (key == sqliteDateTimeKind.ToString())
                            {
                                sqliteConnectControl.cbDateTimeKind.EditValue = sqliteDateTimeKind;
                                break;
                            }
                        }
                    }
                    var sqliteDTKind = String.Format("{0}={1}", sqliteDateTimeKindHeader, sqliteConnectControl.cbDateTimeKind.EditValue);                    
                    editConnectionString.Text = String.Format("{0};{1}", SQLiteConnectionProvider.GetConnectionString(sqliteConnectControl.tbDatabase.Text), sqliteDTKind);
                    tabSQLite.IsSelected = true;
                }
                else if (providerType == PostgreSqlConnectionProvider.XpoProviderTypeString)
                {
                    postgresqlConnectControl.tbServer.Text = helper.GetPartByName(mysqlserverHeader);
                    postgresqlConnectControl.tbDatabase.Text = helper.GetPartByName(mysqldatabaseHeader);
                    postgresqlConnectControl.portNumber.EditValue = helper.GetPartByName(portNumberHeader);
                    postgresqlConnectControl.tbLogin.Text = helper.GetPartByName(useridHeader);
                    postgresqlConnectControl.tbPassword.Password = helper.GetPartByName(passwordHeader);
                    postgresqlConnectControl.tbTimezone.Text = helper.GetPartByName(postgreTimezoneHeader);
                    editConnectionString.Text = XpoHelpers.XpoHelper.GetConnectionStringWithoutPassword(postgresqlConnectControl.GetDataBaseConnectionString());
                    tabPostgreSQL.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                
            }
            bFillingParameters = false;
        }

        void UpdateConnectionString()
        {
            if (bFillingParameters)
                return;
            bFillingParameters = true;

            if (tabSQL.IsSelected)
            {
                if (sqlConnectControl.rbWindowsAuthentication.IsChecked == true)
                {
                    ConnectionString = editConnectionString.Text = MSSqlConnectionProvider.GetConnectionString(
                        sqlConnectControl.tbServer.Text,
                        sqlConnectControl.cbDatabase.Text);
                }
                else
                {
                    ConnectionString = MSSqlConnectionProvider.GetConnectionString(
                        sqlConnectControl.tbServer.Text,
                        sqlConnectControl.tbLogin.Text,
                        sqlConnectControl.tbPassword.Password,
                        sqlConnectControl.cbDatabase.Text);
                    editConnectionString.Text = MSSqlConnectionProvider.GetConnectionString(
                            sqlConnectControl.tbServer.Text,
                            sqlConnectControl.tbLogin.Text,
                            "",
                            sqlConnectControl.cbDatabase.Text);
                }
            }
            else if (tabSQLAzure.IsSelected)
            {
                ConnectionString = MSSqlConnectionProvider.GetConnectionString(
                    String.Format("tcp:{0}.database.windows.net", sqlAzureConnectControl.tbServer.Text),
                    sqlAzureConnectControl.tbLogin.Text,
                    sqlAzureConnectControl.tbPassword.Password,
                    sqlAzureConnectControl.cbDatabase.Text);
                editConnectionString.Text = MSSqlConnectionProvider.GetConnectionString(
                        String.Format("tcp:{0}.database.windows.net", sqlAzureConnectControl.tbServer.Text),
                        sqlAzureConnectControl.tbLogin.Text,
                        "",
                        sqlAzureConnectControl.cbDatabase.Text);
            }
            else if (tabAccess.IsSelected)
            {
                ConnectionString = editConnectionString.Text = AccessConnectionProvider.GetConnectionString(
                    buttonEditAccess.Text);
            }
            else if(tabMySQL.IsSelected)
            {
                ConnectionString = MySqlConnectionProvider.GetConnectionString(
                        mysqlConnectControl.tbServer.Text,
                        mysqlConnectControl.tbLogin.Text,
                        mysqlConnectControl.tbPassword.Password,
                        mysqlConnectControl.tbDatabase.Text);
                editConnectionString.Text = MySqlConnectionProvider.GetConnectionString(
                        mysqlConnectControl.tbServer.Text,
                        mysqlConnectControl.tbLogin.Text,
                        "",
                        mysqlConnectControl.tbDatabase.Text);
            }
            else if (tabSQLite.IsSelected)
            {
                var sqliteDateTimeKind = String.Format("{0}={1}", sqliteDateTimeKindHeader, sqliteConnectControl.cbDateTimeKind.EditValue);
                if (!String.IsNullOrWhiteSpace(sqliteConnectControl.tbPassword.Password))
                {
                    ConnectionString = String.Format("{0};{1}", SQLiteConnectionProvider.GetConnectionString(
                            sqliteConnectControl.tbDatabase.Text,
                            sqliteConnectControl.tbPassword.Password), sqliteDateTimeKind);
                }
                else
                    ConnectionString = String.Format("{0};{1}", SQLiteConnectionProvider.GetConnectionString(sqliteConnectControl.tbDatabase.Text), sqliteDateTimeKind);
                editConnectionString.Text = String.Format("{0};{1}", SQLiteConnectionProvider.GetConnectionString(sqliteConnectControl.tbDatabase.Text), sqliteDateTimeKind);
            }
            else if (tabPostgreSQL.IsSelected)
            {                
                ConnectionString = postgresqlConnectControl.GetDataBaseConnectionString();
                editConnectionString.Text = XpoHelpers.XpoHelper.GetConnectionStringWithoutPassword(ConnectionString);
            }
            else
            {
                ConnectionString = editConnectionString.Text = InMemoryDataStore.GetConnectionString(
                    buttonEditXML.Text);
            }
            bFillingParameters = false;
        }

        #region Properties
        private string _ConnectionString;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
                FillingFromConnectionString();
            }
        }
        public bool IsValid(IUIMsgBoxAlertService UIInterface)
        {
            if (tabSQL.IsSelected && !sqlConnectControl.IsValid)
            {
                UIInterface?.ShowError(Properties.Resources.TableNameError);
                return true;
            }
            else if (tabSQLAzure.IsSelected && !sqlAzureConnectControl.IsValid)
            {
                UIInterface?.ShowError(Properties.Resources.TableNameError);
                return true;
            }
            else if (tabAccess.IsSelected && (buttonEditAccess.Text.Length == 0 || !System.IO.File.Exists(buttonEditAccess.Text)))
            {
                UIInterface?.ShowError(Properties.Resources.FileNameNotFound);
                return true;
            }
            else if (tabXML.IsSelected && (buttonEditXML.Text.Length == 0 || !System.IO.File.Exists(buttonEditXML.Text)))
            {
                UIInterface?.ShowError(Properties.Resources.FileNameNotFound);
                return true;
            }
            else if (tabPostgreSQL.IsSelected && !postgresqlConnectControl.IsValid)
            {
                UIInterface?.ShowError(Properties.Resources.MissingMandatoryParameters);
                return true;
            }
            else return false;
        }
        public IUIMsgBoxAlertService UIService
        {
            get
            {
                return uiService;
            }
            set
            {
                if (uiService != value)
                {
                    uiService = value;
                    OnPropertyChanged("UIService");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        private void buttonEditXML_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = Properties.Resources.XamlFilter;
            if (dialog.ShowDialog() == true)
                buttonEditXML.Text = dialog.FileName;
        }

        private void buttonEditAccess_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = Properties.Resources.AccessFilter;
            if (dialog.ShowDialog() == true)
                buttonEditAccess.Text = dialog.FileName;
        }

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            foreach (DXTabItem item in tabControlExt1.Items)
            {
                if (item.Content is IDisposable)
                    (item.Content as IDisposable).Dispose();
            }
            tabControlExt1.Items.Clear();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                string prop = string.Empty;

                prop = this.GetType().FullName;

                if (prop.Length > 0)
                {
                    if (helpProvider != null)
                        helpProvider.OpenDialogHelpPage(prop, true);
                    e.Handled = true;
                }
            }
        }

        private void editConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editConnectionString.IsKeyboardFocused)
                _ConnectionString = editConnectionString.Text;
        }
    }
}
