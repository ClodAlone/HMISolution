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
    /// <summary>
    /// Represents a validator for a custom database connection.
    /// </summary>
    public class DataReaderValidation : BaseValidation
    {
        #region Declarations
        readonly DataReader.DataReaderModel dataModel;

        string tableName;
        string autoIncrementColumnName;
        #endregion

        #region Constructors
        public DataReaderValidation(string name, string invariantName, DataReader.DataReaderModel dataModel) :
            base(name, invariantName, dataModel.Connection)
        {
            this.dataModel = dataModel;
        }
        #endregion

        #region Overrides
        protected override bool InternalValidate(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError)
        {
            bool validationResult = true;

            var dataTable = GetData(startDateTime, endDateTime);
            tableName = dataTable.TableName;
            FillColumnsList(dataTable);
            totalRows = dataTable.Rows.Count;

            for (int ii = 0; ii < dataTable.Rows.Count; ii++)
            {
                if (Token != null)
                    Token.ThrowIfCancellationRequested();

                OnProcessingRow(ii + 1);

                var oid = Convert.ToInt32(dataTable.Rows[ii][AutoIncrementColumnName]);
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
                if (dataModel == null || dataModel.IsEmpty())
                    return XpoConversionHelper.GetDataProviderFromXpoConnection(defaultConnection);
                else
                    return dataModel.DataProvider;
            }
        }


        protected override String ConnectionString
        {
            get
            {
                if (dataModel == null || dataModel.IsEmpty())
                    return XpoConversionHelper.GetConnectionStringFromXpoConnection(defaultConnection);
                else
                    return dataModel.Connection;
            }
        }

        protected override String TableName
        {
            get
            {
                return tableName;
            }
        }

        protected override String AutoIncrementColumnName
        {
            get
            {
                return autoIncrementColumnName;
            }
        }
        #endregion

        #region Methods
        DataTable GetData(DateTime startDateTime, DateTime endDateTime)
        {
            var dataTable = new DataTable();
            startDateTime = AdjustDateTime(startDateTime);
            endDateTime = AdjustDateTime(endDateTime);

            using (var connection = DataReader.DataReader.CreateDbConnection(dataModel.DataProvider, dataModel.Connection))
            {
                connection.Open();
                var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);
                dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                dbdapater.SelectCommand.Connection = connection;
                if (queryTimeout > 0)
                    dbdapater.SelectCommand.CommandTimeout = queryTimeout;

                StringBuilder commantText = new StringBuilder(dataModel.Select);
                if (!String.IsNullOrEmpty(dataModel.TimeColumn))
                {
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

                    commantText.AppendFormat(" WHERE {0} >= {1} AND {2} <= {3}",
                    dbSchemaInfo.WrapObjectName(dataModel.TimeColumn),
                    startDate.ParameterName,
                    dbSchemaInfo.WrapObjectName(dataModel.TimeColumn),
                    endDate.ParameterName);

                    if (!String.IsNullOrEmpty(dataModel.Where))
                        commantText.AppendFormat(" AND {0}", dataModel.WhereClause());
                }
                else if (!String.IsNullOrEmpty(dataModel.Where))
                {
                    commantText.AppendFormat(" WHERE {0}", dataModel.WhereClause());
                }

                if (!string.IsNullOrEmpty(dataModel.GroupBy))
                    commantText.AppendFormat(" GROUP BY {0}", dataModel.GroupBy);
                if (!string.IsNullOrEmpty(dataModel.Sort))
                    commantText.AppendFormat(" ORDER BY {0}", dataModel.Sort);
                dbdapater.SelectCommand.CommandText = commantText.ToString();
                dbdapater.FillSchema(dataTable, SchemaType.Source);
                dbdapater.Fill(dataTable);
            }

            return dataTable;
        }

        void FillColumnsList(DataTable dataTable)
        {
            Columns.Clear();
            autoIncrementColumnName = null;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (autoIncrementColumnName == null && column.AutoIncrement)
                    autoIncrementColumnName = column.ColumnName;

                Columns.Add(new Column()
                {
                    HeaderName = column.Caption,
                    FieldName = column.ColumnName
                });
            }

            if (autoIncrementColumnName == null)
                throw new InvalidOperationException(Properties.Resources.MissingAutoIncrementColumnName);
        }

        void AddRow(DataRow row, bool isValid)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var column in Columns)
                expandoObject.Add(column.FieldName, row[column.FieldName]);

            Rows.Add(new DynamicRow(expandoObject, isValid));
        }
        #endregion
    }
}
