using DataReader.Helpers;
using DataReader.SchemaInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataValidation
{
    internal class BackupData
    {
        #region Constructors
        public BackupData(string filePath)
        {
            FilePath = filePath;
        }
        #endregion

        #region Properties
        public String FilePath { get; set; }
        public DateTime StartDate { get; set; }
        public short Position { get; set; }
        public decimal FirstLSN { get; set; }
        public decimal LastLSN { get; set; }
        public decimal CheckPointLSN { get; set; }
        #endregion
    }

    internal class BackupFileAnalyzer
    {
        #region Declarations
        readonly string backupFile;
        readonly CancellationToken token;
        #endregion

        #region Constructors
        public BackupFileAnalyzer(string backupFile) : 
            this(backupFile, CancellationToken.None)
        { }

        public BackupFileAnalyzer(string backupFile, CancellationToken token)
        {
            this.backupFile = backupFile;
            this.token = token;
        }
        #endregion

        #region Public Properties
        BackupData fullBackup;
        public BackupData FullBackup
        {
            get
            {
                return fullBackup;
            }
        }

        List<BackupData> logInfo;
        public List<BackupData> LogInfo
        {
            get
            {
                if (logInfo == null)
                    logInfo = new List<BackupData>();
                return logInfo;
            }
        }
        #endregion

        #region Public Methods
        public void Analyze(System.Data.Common.DbConnection connection, String providerName, String connectionString)
        {
            var dataReaderModel = new DataReader.DataReaderModel()
            {
                DataProvider = providerName,
                Connection = connectionString
            };

            Analyze(connection, dataReaderModel);
        }

        public void Analyze(System.Data.Common.DbConnection connection, DataReader.DataReaderModel dataReaderModel)
        {
            var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataReaderModel.DataProvider, dataReaderModel.Connection);

            if (!dbSchemaInfo.DataSourceProductName.Contains("Microsoft SQL Server"))
                throw new InvalidOperationException(string.Format(Properties.Resources.MSSQLEngineRequired, dbSchemaInfo.DataSourceProductName));

            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(dataReaderModel.DataProvider);
            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(dataReaderModel.DataProvider);
            dbdapater.SelectCommand.Connection = connection;

            //dbdapater.SelectCommand.CommandText = "RESTORE HEADERONLY FROM DISK = N'C:\<File>.bak'";

            var commantText = new StringBuilder();
            commantText.AppendFormat("RESTORE HEADERONLY FROM DISK = N'{0}'", backupFile);

            dbdapater.SelectCommand.CommandText = commantText.ToString();

            using (var reader = dbdapater.SelectCommand.ExecuteReader())
            {
                var dbName = XpoConversionHelper.GetDataBaseName(dataReaderModel.Connection);
                while (reader.Read())
                {
                    if (token != null)
                        token.ThrowIfCancellationRequested();

                    if (reader["DatabaseName"].ToString() == dbName)
                    {
                        if ((byte)reader["BackupType"] == 1 && (decimal)reader["DatabaseBackupLSN"] == 0)
                        {
                            var backupData = new BackupData(backupFile);
                            backupData.StartDate = (DateTime)reader["BackupStartDate"];
                            backupData.Position = (short)reader["Position"];
                            backupData.FirstLSN = (decimal)reader["FirstLSN"];
                            backupData.LastLSN = (decimal)reader["LastLSN"];
                            backupData.CheckPointLSN = (decimal)reader["CheckPointLSN"];
                            fullBackup = backupData;
                        }
                        else if ((byte)reader["BackupType"] == 2 && !(bool)reader["IsCopyOnly"])
                        {
                            var backupData = new BackupData(backupFile);
                            backupData.StartDate = (DateTime)reader["BackupStartDate"];
                            backupData.Position = (short)reader["Position"];
                            backupData.FirstLSN = (decimal)reader["FirstLSN"];
                            backupData.LastLSN = (decimal)reader["LastLSN"];
                            backupData.CheckPointLSN = (decimal)reader["CheckPointLSN"];
                            LogInfo.Add(backupData);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
