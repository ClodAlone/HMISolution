using DataReader;
using DataReader.Extensions;
using DataReader.SchemaInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utilities;
using ViewModelLib;

namespace SQLDatabaseConfiguration
{
    public class SqlConfigViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Declarations
        readonly bool isInteractive;

        const int MaxMessageSize = 2048;
        #endregion

        #region Constructors
        public SqlConfigViewModel(bool isInteractive) :
            this(null, isInteractive)
        { }

        public SqlConfigViewModel(String dataSource, bool isInteractive)
        {
            this.isInteractive = isInteractive;

            if (dataSource != null)
            {
                if (DataReader.Helpers.XpoConversionHelper.IsDotNetConvertible(dataSource))
                {
                    var dataReaderModel = new DataReaderModel()
                    {
                        DataProvider = DataReader.Helpers.XpoConversionHelper.GetDataProviderFromXpoConnection(dataSource),
                        Connection = DataReader.Helpers.XpoConversionHelper.GetConnectionStringFromXpoConnection(dataSource)
                    };
                    DataReaderModel = dataReaderModel;
                }
                else
                    DataReaderModel = new DataReaderModel("SQLConnection", dataSource);
            }
        }
        #endregion

        #region Public Properties
        OperationType operationType;
        public OperationType OperationType
        {
            get
            {
                return operationType;
            }
            set
            {
                if (operationType == value)
                    return;

                operationType = value;
                OnPropertyChanged("OperationType");
            }
        }

        CommandType commandType;
        public CommandType CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                if (commandType == value)
                    return;

                commandType = value;
                OnPropertyChanged("CommandType");
            }
        }

        string projectFilePath;
        public string ProjectFilePath
        {
            get
            {
                return projectFilePath;
            }
            set
            {
                if (projectFilePath == value)
                    return;

                projectFilePath = value;
                OnPropertyChanged("ProjectFilePath");
            }
        }

        string projectPassword;
        public string ProjectPassword
        {
            get
            {
                return projectPassword;
            }
            set
            {
                if (projectPassword == value)
                    return;

                projectPassword = value;
                OnPropertyChanged("ProjectPassword");
            }
        }

        DataReaderModel dataReaderModel;
        public DataReaderModel DataReaderModel
        {
            get
            {
                return dataReaderModel;
            }
            set
            {
                if (dataReaderModel == value)
                    return;

                dataReaderModel = value;
                FillTableNames();
                OnPropertyChanged("DataReaderModel");
                OnPropertyChanged("IsValidDataSource");
                OnPropertyChanged("IsValidParameters");
            }
        }

        String tableName;
        public String TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                if (tableName == value)
                    return;

                tableName = value;
                FillColumnNames();
                OnPropertyChanged("TableName");
                OnPropertyChanged("IsValidParameters");
            }
        }

        String utcTimeColName;
        public String UtcTimeColName
        {
            get
            {
                return utcTimeColName;
            }
            set
            {
                if (utcTimeColName == value)
                    return;

                utcTimeColName = value;
                OnPropertyChanged("UtcTimeColName");
                OnPropertyChanged("LocalTimeColName");
                OnPropertyChanged("IsValidParameters");
            }
        }

        String localTimeColName;
        public String LocalTimeColName
        {
            get
            {
                return localTimeColName;
            }
            set
            {
                if (localTimeColName == value)
                    return;

                localTimeColName = value;
                OnPropertyChanged("LocalTimeColName");
                OnPropertyChanged("UtcTimeColName");
                OnPropertyChanged("IsValidParameters");
            }
        }

        List<String> columns = new List<String>();
        public ReadOnlyCollection<String> Columns
        {
            get
            {
                return columns.AsReadOnly();
            }
        }

        List<String> hideColumns = new List<String>();
        public ReadOnlyCollection<String> HideColumns
        {
            get
            {
                return hideColumns.AsReadOnly();
            }
        }


        ObservableCollection<String> tableNames;
        public ObservableCollection<String> TableNames
        {
            get
            {
                if (tableNames == null)
                    tableNames = new ObservableCollection<String>();

                return tableNames;
            }
        }

        ObservableCollection<String> timeColumnNames;
        public ObservableCollection<String> TimeColumnNames
        {
            get
            {
                if (timeColumnNames == null)
                    timeColumnNames = new ObservableCollection<String>();

                return timeColumnNames;
            }
        }

        ObservableCollection<String> numericColumnNames;
        public ObservableCollection<String> NumericColumnNames
        {
            get
            {
                if (numericColumnNames == null)
                    numericColumnNames = new ObservableCollection<String>();

                return numericColumnNames;
            }
        }

        ObservableCollection<TraceMessageViewModel> traceMessages;
        public ObservableCollection<TraceMessageViewModel> TraceMessages
        {
            get
            {
                if (traceMessages == null)
                    traceMessages = new ObservableCollection<TraceMessageViewModel>();

                return traceMessages;
            }
        }

        public bool IsValidParameters
        {
            get
            {
                if (!ValidateDataSource(silent: true) || !ValidateParametersForAggregation(silent: true))
                    return false;

                return true;
            }
        }

        public bool IsValidDataSource
        {
            get
            {
                if (!ValidateDataSource(silent: true))
                    return false;

                return true;
            }
        }
        #endregion

        #region Public Methods
        public void Execute()
        {
            Execute(OperationType, CommandType);
        }

        public void Execute(OperationType optType, CommandType cmdType)
        {
            if (optType == OperationType.None || cmdType == CommandType.None)
                return;

            if (optType == OperationType.AggregatesTables)
            {
                AddTraceMessage(optType, Properties.Resources.TraceActionMessageStarted, ResultState.None);

                if (cmdType == CommandType.Add)
                {
                    PrepareAggregatesTables();
                }
                else if (cmdType == CommandType.Remove)
                {
                    RemoveAggregatesTables();
                }
                else if (cmdType == CommandType.Check)
                {
                    CheckAggregatesTables();
                }
                else if (cmdType == CommandType.Update)
                {
                    UpdateAggregatesTables();
                }

                AddTraceMessage(optType, Properties.Resources.TraceActionMessageTerminated, ResultState.None);
            }
        }

        public void UpdateColumns(IList list)
        {
            columns.Clear();
            foreach (String column in list)
                columns.Add(column.Trim());
            OnPropertyChanged("IsValidParameters");
        }

        public void UpdateHideColumns(IList list)
        {
            hideColumns.Clear();
            foreach (String column in list)
                hideColumns.Add(column.Trim());
            OnPropertyChanged("NumericColumnNames");
        }
        #endregion

        #region Private Methods
        void PrepareAggregatesTables()
        {
            if (!ValidateDataSource(silent: true) || !ValidateParametersForAggregation(silent: true))
            {
                AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.InvalidDataSource, ResultState.Skipped);
                return;
            }

            using (var connection = DataReader.DataReader.CreateDbConnection(DataReaderModel.DataProvider, DataReaderModel.Connection))
            {
                try
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Running);
                    connection.Open();
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Successfully);
                    using (var dbcommand = DataReader.DataReader.CreateDbCommand(DataReaderModel.DataProvider))
                    {
                        dbcommand.CommandType = System.Data.CommandType.Text;
                        dbcommand.Connection = connection;

                        // Store procedures
                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddStoredProcedures, ResultState.Running);

                        dbcommand.CommandText = Properties.Settings.Default.Remove_spAggregateDlDataWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Remove_spAggregateTotTablesWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Remove_spModifyDlDataWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Remove_spModifyTablesWithColName;
                        dbcommand.ExecuteNonQuery();

                        dbcommand.CommandText = Properties.Settings.Default.Add_spAggregateDlDataWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Add_spAggregateTotTablesWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Add_spModifyDlDataWithColName;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Add_spModifyTablesWithColName;
                        dbcommand.ExecuteNonQuery();

                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddStoredProcedures, ResultState.Successfully);

                        // Functions
                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddFunctions, ResultState.Running);

                        dbcommand.CommandText = Properties.Settings.Default.Remove_UDF_SetAggregateTime;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Remove_UDF_SplitString;
                        dbcommand.ExecuteNonQuery();

                        dbcommand.CommandText = Properties.Settings.Default.Add_UDF_SetAggregateTime;
                        dbcommand.ExecuteNonQuery();
                        dbcommand.CommandText = Properties.Settings.Default.Add_UDF_SplitString;
                        dbcommand.ExecuteNonQuery();

                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddFunctions, ResultState.Successfully);

                        // Triggers
                        if (!String.IsNullOrEmpty(TableName) &&
                            !String.IsNullOrEmpty(UtcTimeColName) &&
                            !String.IsNullOrEmpty(LocalTimeColName) &&
                            Columns.Count > 0)
                        {
                            // Add Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Running);

                            var sqlTrigger = Properties.Settings.Default.Remove_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", TableName));
                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();

                            sqlTrigger = Properties.Settings.Default.Add_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", TableName));
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", UtcTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", LocalTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", Columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Successfully);

                            // Test Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Running);
                            sqlTrigger = Properties.Settings.Default.Test_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", UtcTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", LocalTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", Columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Successfully);
                        }
                        else
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Skipped);
                    }
                }
                catch (Exception ex)
                {
                    AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
                }
                finally
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageDisconnecting, ResultState.None);
                    connection.Close();
                }
            }
        }

        void CheckAggregatesTables()
        {
            if (String.IsNullOrEmpty(ProjectFilePath))
            {
                AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.MissingProjectFilePath, ResultState.Skipped);
                return;
            }

            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageParsingProject, ResultState.None);

            try
            {
                using (var helper = new Helpers.ParseProjectHelper(ProjectFilePath, ProjectPassword, !isInteractive))
                {
                    var connections = new List<String>();
                    foreach (var datalogger in helper.DataLoggerSettings)
                    {
                        var model = datalogger.DataLoggerSettings.ConnectionSettings;
                        if (model == null || model.IsEmpty())
                            model = helper.DefaultConnection;

                        if (model == null || model.DataProvider != "System.Data.SqlClient" || String.IsNullOrWhiteSpace(model.Connection))
                        {
                            AddTraceMessage(OperationType.AggregatesTables, String.Format(Properties.Resources.TraceActionMessageDataLogger, datalogger.DataLoggerSettings.Name), ResultState.Skipped);
                            continue;
                        }

                        AddTraceMessage(OperationType.AggregatesTables, String.Format(Properties.Resources.TraceActionMessageDataLogger, datalogger.DataLoggerSettings.Name), ResultState.Running);

                        try
                        {
                            var connectionLower = model.Connection.ToLower();
                            bool onlyTrigger = connections.Contains(model.Connection.ToLower());

                            CreateDataLogger(model, datalogger);
                            CheckAggregatesTables(model, datalogger, onlyTrigger);

                            if (!onlyTrigger)
                                connections.Add(model.Connection.ToLower());
                        }
                        catch (Exception ex)
                        {
                            AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
            }
        }

        void CheckAggregatesTables(DataReader.DataReaderModel model, DataLoggerModel.Helpers.DataLoggerSettingsHelper datalogger, bool onlyTrigger = false)
        {
            using (var connection = DataReader.DataReader.CreateDbConnection(model.DataProvider, model.Connection))
            {
                try
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Running);
                    connection.Open();
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Successfully);
                    using (var dbcommand = DataReader.DataReader.CreateDbCommand(model.DataProvider))
                    {
                        dbcommand.CommandType = System.Data.CommandType.Text;
                        dbcommand.Connection = connection;

                        if (!onlyTrigger)
                        {
                            // Store procedures
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddStoredProcedures, ResultState.Running);

                            dbcommand.CommandText = Properties.Settings.Default.Remove_spAggregateDlDataWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Remove_spAggregateTotTablesWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Remove_spModifyDlDataWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Remove_spModifyTablesWithColName;
                            dbcommand.ExecuteNonQuery();

                            dbcommand.CommandText = Properties.Settings.Default.Add_spAggregateDlDataWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Add_spAggregateTotTablesWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Add_spModifyDlDataWithColName;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Add_spModifyTablesWithColName;
                            dbcommand.ExecuteNonQuery();

                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddStoredProcedures, ResultState.Successfully);

                            // Functions
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddFunctions, ResultState.Running);

                            dbcommand.CommandText = Properties.Settings.Default.Remove_UDF_SetAggregateTime;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Remove_UDF_SplitString;
                            dbcommand.ExecuteNonQuery();

                            dbcommand.CommandText = Properties.Settings.Default.Add_UDF_SetAggregateTime;
                            dbcommand.ExecuteNonQuery();
                            dbcommand.CommandText = Properties.Settings.Default.Add_UDF_SplitString;
                            dbcommand.ExecuteNonQuery();

                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddFunctions, ResultState.Successfully);
                        }

                        var columns = (from c in datalogger.DataLoggerSettings.Columns.AsParallel()
                                       select c.Name).ToList();

                        // Triggers
                        if (!String.IsNullOrEmpty(datalogger.TableName) &&
                            !String.IsNullOrEmpty(datalogger.UtcTimeColumnName) &&
                            !String.IsNullOrEmpty(datalogger.LocalTimeColumnName) &&
                            columns.Count > 0)
                        {
                            // Add Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Running);

                            var sqlTrigger = Properties.Settings.Default.Remove_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", datalogger.TableName));
                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();

                            sqlTrigger = Properties.Settings.Default.Add_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", datalogger.TableName));
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", datalogger.TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", datalogger.TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", datalogger.UtcTimeColumnName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", datalogger.LocalTimeColumnName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Successfully);

                            // Test Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Running);
                            sqlTrigger = Properties.Settings.Default.Test_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", datalogger.TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", datalogger.TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", datalogger.UtcTimeColumnName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", datalogger.LocalTimeColumnName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Successfully);
                        }
                        else
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Skipped);
                    }
                }
                catch (Exception ex)
                {
                    AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
                }
                finally
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageDisconnecting, ResultState.None);
                    connection.Close();
                }
            }
        }

        void UpdateAggregatesTables()
        {
            if (!ValidateDataSource(silent: true) || !ValidateParametersForAggregation(silent: true))
            {
                AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.InvalidDataSource, ResultState.Skipped);
                return;
            }

            using (var connection = DataReader.DataReader.CreateDbConnection(DataReaderModel.DataProvider, DataReaderModel.Connection))
            {
                try
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Running);
                    connection.Open();
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Successfully);
                    using (var dbcommand = DataReader.DataReader.CreateDbCommand(DataReaderModel.DataProvider))
                    {
                        if (!String.IsNullOrEmpty(TableName) &&
                            !String.IsNullOrEmpty(UtcTimeColName) &&
                            !String.IsNullOrEmpty(LocalTimeColName) &&
                            Columns.Count > 0)
                        {
                            // Update Tables
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageUpdateTables, ResultState.Running);
                            dbcommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dbcommand.Connection = connection;

                            dbcommand.Parameters.Add(new SqlParameter("@sTablename", TableName));
                            dbcommand.Parameters.Add(new SqlParameter("@EnableMI", 1));
                            dbcommand.Parameters.Add(new SqlParameter("@EnableHH", 1));
                            dbcommand.Parameters.Add(new SqlParameter("@EnableDD", 1));
                            dbcommand.Parameters.Add(new SqlParameter("@UtcTimeColName", UtcTimeColName));
                            dbcommand.Parameters.Add(new SqlParameter("@LocalTimeColName", LocalTimeColName));
                            dbcommand.Parameters.Add(new SqlParameter("@NumericColumnNames", String.Join(",", Columns)));

                            dbcommand.CommandText = "spModifyDlDataWithColName";
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageUpdateTables, ResultState.Successfully);
                        }
                        else
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageUpdateTables, ResultState.Skipped);
                    }

                    using (var dbcommand = DataReader.DataReader.CreateDbCommand(DataReaderModel.DataProvider))
                    { 
                        // Remove Triggers
                        dbcommand.CommandType = System.Data.CommandType.Text;
                        dbcommand.Connection = connection;

                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageRemoveTriggers, ResultState.Running);
                        var sqlTrigger = Properties.Settings.Default.Remove_TableTrigger;
                        sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", TableName));
                        dbcommand.CommandText = sqlTrigger;
                        dbcommand.ExecuteNonQuery();
                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageRemoveTriggers, ResultState.Successfully);

                        if (!String.IsNullOrEmpty(TableName) &&
                            !String.IsNullOrEmpty(UtcTimeColName) &&
                            !String.IsNullOrEmpty(LocalTimeColName) &&
                            Columns.Count > 0)
                        {
                            // Add Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Running);
                            sqlTrigger = Properties.Settings.Default.Add_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", TableName));
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", UtcTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", LocalTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", Columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Successfully);

                            // Test Triggers
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Running);
                            sqlTrigger = Properties.Settings.Default.Test_TableTrigger;
                            sqlTrigger = sqlTrigger.Replace("[dbo].[DataLogger]", TableName);
                            sqlTrigger = sqlTrigger.Replace("N'TableName'", String.Format("N'{0}'", TableName));
                            sqlTrigger = sqlTrigger.Replace("N'UtcTimeColName'", String.Format("N'{0}'", UtcTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'LocalTimeColName'", String.Format("N'{0}'", LocalTimeColName));
                            sqlTrigger = sqlTrigger.Replace("N'NumericColumnNames'", String.Format("N'{0}'", String.Join(",", Columns)));

                            dbcommand.CommandText = sqlTrigger;
                            dbcommand.ExecuteNonQuery();
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageTestTriggers, ResultState.Successfully);
                        }
                        else
                            AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageAddTriggers, ResultState.Skipped);
                    }
                }
                catch (Exception ex)
                {
                    AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
                }
                finally
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageDisconnecting, ResultState.None);
                    connection.Close();
                }
            }
        }

        void RemoveAggregatesTables()
        {
            if (!ValidateDataSource(silent: true))
            {
                AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.InvalidDataSource, ResultState.Skipped);
                return;
            }

            using (var connection = DataReader.DataReader.CreateDbConnection(DataReaderModel.DataProvider, DataReaderModel.Connection))
            {
                try
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Running);
                    connection.Open();
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageConnecting, ResultState.Successfully);
                    using (var dbcommand = DataReader.DataReader.CreateDbCommand(DataReaderModel.DataProvider))
                    {
                        dbcommand.CommandType = System.Data.CommandType.Text;
                        dbcommand.Connection = connection;

                        // Triggers
                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageRemoveTriggers, ResultState.Running);
                        var sqlTrigger = Properties.Settings.Default.Remove_TableTrigger;
                        sqlTrigger = sqlTrigger.Replace("DataLogger_AFTER_INSERT", String.Format("{0}_AFTER_INSERT", TableName));
                        dbcommand.CommandText = sqlTrigger;
                        dbcommand.ExecuteNonQuery();
                        AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageRemoveTriggers, ResultState.Successfully);
                    }
                }
                catch (Exception ex)
                {
                    AddTraceMessage(OperationType.AggregatesTables, ex.Message, ResultState.Error);
                }
                finally
                {
                    AddTraceMessage(OperationType.AggregatesTables, Properties.Resources.TraceActionMessageDisconnecting, ResultState.None);
                    connection.Close();
                }
            }
        }

        void AddTraceMessage(OperationType optType, String message, ResultState state)
        {
            string action;
            if (optType == OperationType.AggregatesTables)
                action = Properties.Resources.TraceActionAggregatedTitle;
            else if (optType == OperationType.PatitionTables)
                action = Properties.Resources.TraceActionPartitionTitle;
            else
                action = Properties.Resources.TraceActionNoneTitle;

            if (!isInteractive)
            {
                message = message.Replace(Environment.NewLine, "-newline-");
                if (state == ResultState.Error)
                    Console.Error.WriteLine(String.Format("{0} : {1}", action, message));
                else
                    Console.WriteLine(String.Format("{0} : {1} - {2}", action, message, state));
            }
            else
            {
                if (message.Length > MaxMessageSize)
                    message = String.Format("{0}...", message.Substring(0, MaxMessageSize));

                TraceMessages.Add(new TraceMessageViewModel(action, message, state));
            }
        }

        bool ValidateDataSource()
        {
            bool silet = !isInteractive;
            return ValidateDataSource(silet);
        }

        bool ValidateDataSource(bool silent)
        {
            if (DataReaderModel == null || DataReaderModel.DataProvider != "System.Data.SqlClient" || String.IsNullOrWhiteSpace(DataReaderModel.Connection))
            {
                if (!silent)
                {
                    MessageBox.Show(SQLDatabaseConfiguration.Properties.Resources.InvalidDataSource,
                        SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                return false;
            }

            return true;
        }

        bool ValidateParametersForAggregation()
        {
            bool silet = !isInteractive;
            return ValidateParametersForAggregation(silet);
        }

        bool ValidateParametersForAggregation(bool silent)
        {
            if (DataReaderModel == null)
                return false;

            if (TableName == null || UtcTimeColName == null || LocalTimeColName == null || Columns.Count == 0)
            {
                if (silent)
                    return false;

                var result = MessageBox.Show(SQLDatabaseConfiguration.Properties.Resources.MissingOptions,
                    SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return false;
            }

            bool error = false;

            WaitCursor cursor = null;
            try
            {
                if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA)
                    cursor = new WaitCursor();
                // Take first table in the database.
                if (TableName == null)
                {
                    var tables = DataReader.DataReader.ListTables(DataReaderModel.DataProvider, DataReaderModel.Connection, useSchemaQuotes: true).ToList();
                    if (tables.Count > 0)
                        TableName = tables[0];
                }

                // Take numeric and date time columns from found table.
                if (TableName != null)
                {
                    bool addNumericColumns = Columns.Count == 0;

                    var sqlSelect = String.Format("SELECT * FROM {0}", TableName);
                    var columnTypes = DataReader.DataReader.ListColumns(DataReaderModel.DataProvider, DataReaderModel.Connection, sqlSelect);

                    foreach (var columnName in columnTypes.Keys)
                    {
                        if (!Helpers.TypeHelper.IsNumeric(columnTypes[columnName]))
                            columns.Remove(columnName);
                        else if (addNumericColumns)
                            columns.Add(columnName);

                        if (UtcTimeColName != null && LocalTimeColName != null)
                            continue;
                        else if (columnTypes[columnName] == typeof(DateTime))
                        {
                            if (UtcTimeColName == null && LocalTimeColName != columnName)
                                UtcTimeColName = columnName;
                            else if (LocalTimeColName == null && UtcTimeColName != columnName)
                                LocalTimeColName = columnName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = true;
                if (!silent)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorConnectingDatabase, ex.Message),
                        SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Dispose();
            }
            
            // Check for found informations.
            if (TableName == null || UtcTimeColName == null || LocalTimeColName == null || Columns.Count == 0)
            {
                if (!error && !silent)
                {
                    MessageBox.Show(SQLDatabaseConfiguration.Properties.Resources.NotFoundData,
                        SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                return false;
            }

            return true;
        }

        void FillTableNames()
        {
            if (!isInteractive || !ValidateDataSource(silent: true))
                return;

            TableNames.Clear();
            TimeColumnNames.Clear();
            NumericColumnNames.Clear();

            var task1 = Task.Factory.StartNew(delegate
            {
                return DataReader.DataReader.ListTables(DataReaderModel.DataProvider, DataReaderModel.Connection).ToList();
            });
            var task2 = task1.ContinueWith(ret =>
            {
                if (ret.Exception != null)
                {
                    if (isInteractive)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.ErrorConnectingDatabase, ret.Exception.InnerException.Message),
                            SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (ret.Result != null)
                {
                    ret.Result.ForEach(table => TableNames.Add(table));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void FillColumnNames()
        {
            if (!isInteractive || !ValidateDataSource(silent: true) || String.IsNullOrEmpty(TableName))
                return;

            TimeColumnNames.Clear();
            NumericColumnNames.Clear();

            var timeCoulmns = new List<String>();
            var numCoulmns = new List<String>();

            var task1 = Task.Factory.StartNew(delegate
            {
                var sqlSelect = String.Format("SELECT * FROM {0}", TableName);
                var columnTypes = DataReader.DataReader.ListColumns(DataReaderModel.DataProvider, DataReaderModel.Connection, sqlSelect);

                foreach (var columnName in columnTypes.Keys)
                {
                    if (hideColumns.Contains(columnName))
                        continue;

                    if (columnTypes[columnName] == typeof(DateTime))
                        timeCoulmns.Add(columnName);
                    else if (Helpers.TypeHelper.IsNumeric(columnTypes[columnName]))
                        numCoulmns.Add(columnName);
                }
            });
            var task2 = task1.ContinueWith(ret =>
            {
                if (ret.Exception != null)
                {
                    if (isInteractive)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.ErrorConnectingDatabase, ret.Exception.InnerException.Message),
                            SQLDatabaseConfiguration.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    timeCoulmns.ForEach(column => TimeColumnNames.Add(column));
                    numCoulmns.ForEach(column => NumericColumnNames.Add(column));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        static void CreateDataLogger(DataReader.DataReaderModel model, DataLoggerModel.Helpers.DataLoggerSettingsHelper settingsHelper)
        {
            using (var writer = new DataWriter.DataSetWriter(model.DataProvider, model.Connection))
            {
                if (!writer.TryOpenConnection())
                {
                    writer.CreateDataBase();
                    writer.OpenConnection();
                }

                var dataSet = new DataSet();
                dataSet.Tables.Add(settingsHelper.CreateDataTable());
                writer.CheckTables(dataSet, settingsHelper.DataLoggerSettings.SkipCheckColumnsType);
            }
        }
        #endregion

        #region Commands
        RelayCommand clearLog;
        public ICommand ClearLog
        {
            get
            {
                if (clearLog == null)
                {
                    clearLog = new RelayCommand(param => TraceMessages.Clear());
                }
                return clearLog;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ProjectFilePath")
            {
                if (!String.IsNullOrEmpty(ProjectFilePath) &&
                    !XpoHelpers.XpoHelper.IsDataSource(ProjectFilePath) &&
                    !System.IO.File.Exists(ProjectFilePath))
                    return Properties.Resources.InvalidProjectFilePath;
            }
            else if (propertyName == "DataReaderModel")
            {
                if (!ValidateDataSource(silent: true))
                    return Properties.Resources.InvalidDataSource;
            }
            else if (propertyName == "UtcTimeColName")
            {
                if (!String.IsNullOrEmpty(UtcTimeColName) && UtcTimeColName == LocalTimeColName)
                    return Properties.Resources.InvalidUtcColumnName;
            }
            else if (propertyName == "LocalTimeColName")
            {
                if (!String.IsNullOrEmpty(LocalTimeColName) && LocalTimeColName == UtcTimeColName)
                    return Properties.Resources.InvalidLocalColumnName;
            }

            return null;
        }
        #endregion
    }
}
