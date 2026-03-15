using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabaseConfiguration
{
    public enum OperationType : int
    {
        None,
        AggregatesTables,
        PatitionTables
    }

    public enum CommandType : int
    {
        None,
        Add,
        Remove,
        Check,
        Update
    }

    class CommandLineOptions
    {
        #region Declarations
        const string parSilent = "/S";
        const string parProjectPath = "/F";
        const string parProjectPassword = "/W";
        const string parOperationType = "/O";
        const string parCommandType = "/P";
        const string parDataSourceConnection = "/D";
        const string parTableName = "/T";
        const string parUtcTimeColName = "/U";
        const string parLocalTimeColName = "/L";
        const string parDataColumnNames = "/C";
        const string parHideColumnNames = "/H";
        const string parCallingProcessId = "/I";
        #endregion

        #region Constructors
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        #region Methods
        void Parse(string[] args)
        {
            if (args.Length < 1)
                return;

            Dictionary<string, string> argTable = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.Length > 2)
                {
                    argTable[a.Substring(0, 2).ToUpper()] = a.Substring(2);
                }
                else
                {
                    argTable[a.ToUpper()] = string.Empty;
                }
            }

            if (argTable.ContainsKey(parSilent))
            {
                Silent = true;
            }

            if (argTable.ContainsKey(parProjectPath))
            {
                ProjectPath = argTable[parProjectPath];
            }

            if (argTable.ContainsKey(parProjectPassword))
            {
                ProjectPassword = argTable[parProjectPassword];
            }

            if (argTable.ContainsKey(parOperationType))
            {
                int value;
                if (int.TryParse(argTable[parOperationType], out value))
                    operationType = (OperationType)value;
            }

            if (argTable.ContainsKey(parCommandType))
            {
                int value;
                if (int.TryParse(argTable[parCommandType], out value))
                    CommandType = (CommandType)value;
            }

            if (argTable.ContainsKey(parDataSourceConnection))
            {
                DataSource = argTable[parDataSourceConnection];
            }

            if (argTable.ContainsKey(parTableName))
            {
                TableName = argTable[parTableName];
            }

            if (argTable.ContainsKey(parUtcTimeColName))
            {
                UtcTimeColName = argTable[parUtcTimeColName];
            }

            if (argTable.ContainsKey(parLocalTimeColName))
            {
                LocalTimeColName = argTable[parLocalTimeColName];
            }

            if (argTable.ContainsKey(parDataColumnNames))
            {
                Columns = argTable[parDataColumnNames];
            }

            if (argTable.ContainsKey(parHideColumnNames))
            {
                HideColumns = argTable[parHideColumnNames];
            }

            if (argTable.ContainsKey(parCallingProcessId))
            {
                int id;
                if (int.TryParse(argTable[parCallingProcessId], out id))
                    CallingProcessId = id;
            }
        }
        #endregion

        #region Properties
        bool silent;
        public bool Silent
        {
            get { return silent; }
            set
            {
                silent = value;
            }
        }

        String projectPath;
        public String ProjectPath
        {
            get { return projectPath; }
            set
            {
                projectPath = value;
            }
        }

        String projectPassword;
        public String ProjectPassword
        {
            get { return projectPassword; }
            set
            {
                projectPassword = value;
            }
        }

        OperationType operationType;
        public OperationType OperationType
        {
            get { return operationType; }
            set
            {
                operationType = value;
            }
        }

        CommandType commandType;
        public CommandType CommandType
        {
            get { return commandType; }
            set
            {
                commandType = value;
            }
        }

        String dataSource;
        public String DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
            }
        }

        String tableName;
        public String TableName
        {
            get { return tableName; }
            set
            {
                tableName = value;
            }
        }
        
        String utcTimeColName;
        public String UtcTimeColName
        {
            get { return utcTimeColName; }
            set
            {
                utcTimeColName = value;
            }
        }

        String localTimeColName;
        public String LocalTimeColName
        {
            get { return localTimeColName; }
            set
            {
                localTimeColName = value;
            }
        }

        String columns;
        public String Columns
        {
            get { return columns; }
            set
            {
                columns = value;
            }
        }

        String hideColumns;
        public String HideColumns
        {
            get { return hideColumns; }
            set
            {
                hideColumns = value;
            }
        }

        private int _CallingProcessId;
        public int CallingProcessId
        {
            get { return _CallingProcessId; }
            set
            {
                _CallingProcessId = value;
            }
        }

        public bool IsValid
        {
            get
            {
                if (!Silent)
                    return true;

                if (String.IsNullOrWhiteSpace(DataSource) && String.IsNullOrWhiteSpace(ProjectPath))
                    return false;
                else if (!String.IsNullOrWhiteSpace(ProjectPath) && OperationType == OperationType.None)
                    return false;
                else if (!String.IsNullOrWhiteSpace(DataSource) && (OperationType == OperationType.None || CommandType == CommandType.None))
                    return false;

                return true;
            }
        }       
        #endregion
    }
}
