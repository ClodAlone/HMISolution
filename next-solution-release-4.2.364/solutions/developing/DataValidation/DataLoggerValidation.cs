using DataLoggerModel;
using DataLoggerModel.Helpers;
using DataReader.Extensions;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DataValidation
{
    public class DataLoggerValidation : BaseValidation
    {
        #region Declarations
        readonly DataLoggerTable helper;
        readonly string[] skipShowColumns;
        #endregion

        #region Constructors
        public DataLoggerValidation(DataLoggerTable settings, string defaultConnection) :
            base(settings.Name, defaultConnection)
        {
            helper = settings;
            skipShowColumns = new string[]
            {
                helper.RedundancyColumnName
            };
        }
        #endregion

        #region Overrides
        protected override bool InternalValidate(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError)
        {
            bool validationResult = true;

            var dataTable = GetData(startDateTime, endDateTime);
            FillColumnsList(dataTable);
            totalRows = dataTable.Rows.Count;

            for (int ii = 0; ii < dataTable.Rows.Count; ii++)
            {
                if (Token != null)
                    Token.ThrowIfCancellationRequested();

                OnProcessingRow(ii + 1);

                var oid = (int)dataTable.Rows[ii][helper.AutoIncrementColumnName];
                bool isValid = CheckIsValid(oid);
                AddRow(dataTable.Rows[ii], isValid);

                if (!isValid)
                {
                    validationResult = false;
                    if (!bContinueOnError)
                        break;
                }
            }

            return validationResult;
        }

        protected override void ClearAllCounters()
        {
            base.ClearAllCounters();

            Rows.Clear();
        }

        ObservableCollection<Column> columns;
        public override ObservableCollection<Column> Columns
        {
            get
            {
                if (columns == null)
                    columns = new SafeObservableCollection<Column>();
                return columns;
            }
        }

        ObservableCollection<DynamicRow> rows;
        public override ObservableCollection<DynamicRow> Rows
        {
            get
            {
                if (rows == null)
                    rows = new SafeObservableCollection<DynamicRow>();
                return rows;
            }
        }

        protected override string ProviderName
        {
            get
            {
                DataReader.DataReaderModel model = helper.ConnectionSettings;
                if (model == null || model.IsEmpty())
                    return XpoConversionHelper.GetDataProviderFromXpoConnection(defaultConnection);
                else
                    return model.DataProvider;
            }
        }


        protected override String ConnectionString
        {
            get
            {
                DataReader.DataReaderModel model = helper.ConnectionSettings;
                if (model == null || model.IsEmpty())
                    return XpoConversionHelper.GetConnectionStringFromXpoConnection(defaultConnection);
                else
                    return model.Connection;
            }
        }

        protected override String TableName
        {
            get
            {
                return helper.TableName;
            }
        }

        protected override String AutoIncrementColumnName
        {
            get
            {
                return helper.AutoIncrementColumnName;
            }
        }
        #endregion

        #region Methods
        DataTable GetData(DateTime startDateTime, DateTime endDateTime)
        {
            startDateTime = AdjustDateTime(startDateTime);
            endDateTime = AdjustDateTime(endDateTime);

            var dataTable = helper.TableStructure.Clone();
            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnectionString))
            {
                connection.Open();
                var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);
                dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                dbdapater.SelectCommand.Connection = connection;

                StringBuilder commantText = new StringBuilder();
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (commantText.Length == 0)
                        commantText.AppendFormat("SELECT {0}", dbSchemaInfo.WrapObjectName(column.ColumnName));
                    else
                    commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(column.ColumnName));
                }
                commantText.AppendFormat(" FROM {0}", dbSchemaInfo.WrapObjectName(helper.TableName));
                
                var startDate = DataReader.DataReader.CreateDbParameter(ProviderName);
                startDate.DbType = System.Data.DbType.DateTime;
                startDate.ParameterName = dbSchemaInfo.FormatParameterName("StartDate");
                startDate.Value = startDateTime;
                dbdapater.SelectCommand.Parameters.Add(startDate);

                var endDate = DataReader.DataReader.CreateDbParameter(ProviderName);
                endDate.DbType = System.Data.DbType.DateTime;
                endDate.ParameterName = dbSchemaInfo.FormatParameterName("EndDate");
                endDate.Value = endDateTime;
                dbdapater.SelectCommand.Parameters.Add(endDate);

                commantText.AppendFormat(" WHERE {0} >= {1} AND {2} <= {3} ORDER BY {4} ASC",
                    dbSchemaInfo.WrapObjectName(helper.UtcTimeColumnName),
                    startDate.ParameterName,
                    dbSchemaInfo.WrapObjectName(helper.UtcTimeColumnName),
                    endDate.ParameterName,
                    dbSchemaInfo.WrapObjectName(helper.AutoIncrementColumnName));

                dbdapater.SelectCommand.CommandText = commantText.ToString();
                //dbdapater.FillSchema(dataTable, SchemaType.Source);
                dbdapater.Fill(dataTable);
            }

            return dataTable;
        }

        void FillColumnsList(DataTable dataTable)
        {
            Columns.Clear();

            foreach (DataColumn column in dataTable.Columns)
            {
                if (skipShowColumns.Contains(column.ColumnName))
                    continue;

                Columns.Add(new Column()
                {
                    HeaderName = column.Caption,
                    FieldName = column.ColumnName
                });
            }
        }

        void AddRow(DataRow row, bool isValid)
        {
            var expandoObject = new Dictionary<string, object>();
            foreach (var column in Columns)
                expandoObject.Add(column.FieldName, row[column.FieldName]);

            Rows.Add(new DynamicRow(expandoObject, isValid));
        }
        #endregion
    }
}
