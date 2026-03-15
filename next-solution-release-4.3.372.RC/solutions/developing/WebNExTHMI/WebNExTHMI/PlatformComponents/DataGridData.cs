using DataReader.Helpers;
using DocumentManager.ComponentService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Helpers;
using UFUAEditor.ComponentService;
using Utilities;

namespace WebNExTHMI.PlatformComponents
{
    public class DataGridData
    {
        #region Declarations
        string defaultDataProvider = String.Empty;
        string defaultConnectionString = String.Empty;
        uint maxTransactionsBeforeCommit;
        bool allowPrimaryKeyChanging;
        string tableName;
        DataSet dataSet;
        DataView gridDataView = new DataView();
        Dictionary<string, object> columnDefaultValues;
        Dictionary<int, string> columnDataTypes;
        Dictionary<String, DataReader.SchemaInfo.ColumnInfo> columnsInfo;
        Object lockObject = new Object();
        #endregion

        public DataGridData()
        {
            
        }

        public string LoadDataGridData(string connectionString, string sessionString, int commandTimeout, string dataProvider, string connection, string select, string where, string groupBy, string sort, string tableName, uint maxTransactionsBeforeCommit, bool allowPrimaryKeyChanging, CancellationToken ct)
        {
            if (dataSet == null)
            {
                dataSet = new DataSet();
                this.maxTransactionsBeforeCommit = maxTransactionsBeforeCommit;
                this.allowPrimaryKeyChanging = allowPrimaryKeyChanging;
                this.tableName = tableName;
            }
            bool bConstraintsDisabled = false;
            var ret = String.Empty;
            var projectDocument = PlatformComponents.GetProjectDocument();

            var UFUAEditor = projectDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ct.IsCancellationRequested || UFUAEditor == null)
                return ret;

            try
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connectionString);
                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(connectionString);
                }
                else
                {
                    string settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(UFUAEditor.GetHistorianDefaultConnection(projectDocument), sessionString);
                    if (!string.IsNullOrEmpty(settings))
                    {
                        defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                        defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                    }
                }
            }
            catch (Exception)
            {
            }
            string _defaultDataProvider = defaultDataProvider;
            string _defaultConnectionString = defaultConnectionString;

            if (!string.IsNullOrEmpty(dataProvider) &&
                !string.IsNullOrEmpty(connection))
            {
                _defaultDataProvider = dataProvider;
                _defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connection, projectDocument?.rootBase);
            }

            if (string.IsNullOrEmpty(_defaultDataProvider) || string.IsNullOrEmpty(_defaultConnectionString))
            {
                return ret;
            }

            defaultDataProvider = _defaultDataProvider;
            defaultConnectionString = _defaultConnectionString;

            connectionString = String.Format("DataProvider={0};{1}", defaultDataProvider, defaultConnectionString);

            if (string.IsNullOrEmpty(select))
                return ret;

            //gridDataControl1.ItemsSource = null;

            //if (lastDataReaderModel == null || !lastDataReaderModel.Equals(ControlDataSource.ControlDataSource))
            //    gridDataControl1.Columns.Clear();

            //var columnListSettings = ControlDataSource.ColumnListSettings;
            var tablename = tableName;

            using (var conn = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
            {
                try
                {
                    conn.Open();
                    if (ct.IsCancellationRequested)
                        return ret;

                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                    dbdapater.SelectCommand.CommandTimeout = commandTimeout;
                    dbdapater.SelectCommand.Connection = conn;
                    StringBuilder commantText;

                    commantText = new StringBuilder(select);
                    if (!string.IsNullOrEmpty(where))
                        commantText.AppendFormat(" Where {0}", where);
                    if (!String.IsNullOrEmpty(groupBy))
                        commantText.AppendFormat(" Group By {0}", groupBy);
                    if (!string.IsNullOrEmpty(sort))
                        commantText.AppendFormat(" Order By {0}", sort);
                    dbdapater.SelectCommand.CommandText = commantText.ToString();

                    if (dataSet.Tables.Count > 0)
                        dataSet.Tables.Clear();

                    DataTable dataTable = new DataTable();
                    dbdapater.FillSchema(dataTable, SchemaType.Source);
                    tablename = dataTable.TableName;

                    dbdapater.FillSchema(dataSet, SchemaType.Source);

                    try
                    {
                        dbdapater.Fill(dataSet);
                        gridDataView = dataSet.Tables[0].Copy().AsDataView();
                        if (!string.IsNullOrEmpty(tablename))
                            this.tableName = tablename;
                        if (columnsInfo == null)
                            InitColumnsInfo(this.tableName);
                    }
                    catch
                    {
                        bConstraintsDisabled = true;
                        dataSet.EnforceConstraints = false;

                        if (dataSet.Tables.Count > 0)
                            dataSet.Tables.Clear();

                        dbdapater.Fill(dataSet);
                    }

                    if (dataSet.Tables.Count > 0)
                    {
                        var table = dataSet.Tables[0];
                        if (columnDefaultValues == null)
                        {
                            columnDefaultValues = new Dictionary<string, object>();
                            foreach (DataColumn col in table.Columns)
                                columnDefaultValues.Add(col.ToString(), GetDataColumnDefaultValue(col));
                        }
                        if (columnDataTypes == null)
                        {
                            columnDataTypes = new Dictionary<int, string>();
                            for (var i = 0; i < table.Columns.Count; i++)
                                columnDataTypes.Add(i, GetColumnDataType(table.Columns[i]));
                        }
                    }

                    //if (columnListSettings.Count == 0)
                    //{
                    //    var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(dataSet.Tables[0]);
                    //    if (columns != null)
                    //    {
                    //        foreach (DataColumnInfo column in columns)
                    //        {
                    //            columnListSettings.Add(new ColumnItem()
                    //            {
                    //                colName = column.Name,
                    //                caption = column.Name,
                    //                isEditable = true,
                    //                isVisible = true,
                    //                isImage = false
                    //            });
                    //        }
                    //    }
                    //}
                    commantText = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            //if (!string.IsNullOrEmpty(tablename))
            //{
            //    tableName = tablename;
            //}
            //if (ControlDataSource.ColumnListSettings.Count == 0)
            //{
            //    ControlDataSource.ColumnListSettings = columnListSettings;
            //}
            if (ct.IsCancellationRequested)
                return ret;

            var jObject = PlatformComponents.GetDataSetJObject(dataSet);
            jObject.Add("bConstraintsDisabled", bConstraintsDisabled);
            jObject.Add("columnDefaultValues", JToken.Parse(JsonConvert.SerializeObject(columnDefaultValues)));
            jObject.Add("columnDataTypes", JToken.Parse(JsonConvert.SerializeObject(columnDataTypes)));
            jObject.Add("columnsInfo", JToken.Parse(JsonConvert.SerializeObject(columnsInfo)));

            return jObject.ToString(Newtonsoft.Json.Formatting.None);
        }

        public void GridControlDBSave(string sessionString, object pendingChanges)
        {
            var table = dataSet.Tables[0];
            var finalDataSet = DataSet_MergePendingChanges(pendingChanges, table);
            gridDataView = table.Copy().AsDataView();
            
            DataGridSaveTable(table);

            
            //RecipeExecuter.AcceptUpdateData(finalDataSet, recipeGuid);
            //datasetwriter methods
        }

        DataView CopyDataView(DataView dataView, bool allRecords = false)
        {
            DataView copyDataView = null;
            try
            {
                uint maxTransactions = maxTransactionsBeforeCommit;
                if (maxTransactions > 0)
                {
                    copyDataView = new DataView(dataView.Table.Clone());
                    while (dataView.Count > 0)
                    {
                        if (!allRecords && copyDataView.Count >= maxTransactions)
                            break;
                        copyDataView.Table.ImportRow(dataView[0].Row);
                        dataView.Delete(0);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return copyDataView;
        }

        void DataGridSaveTable(DataTable table)
        {
            var dataView = new DataView(table);
            using (var writer = new DataWriter.DataSetWriter(defaultDataProvider, defaultConnectionString, maxTransactionsBeforeCommit))
            {
                try
                {

                    // deleted rows
                    dataView = DeletedRows;
                    //dataView.RowStateFilter = DataViewRowState.Deleted;
                    if (dataView.Count > 0)
                    {
                        if (dataView.Table.PrimaryKey != null && dataView.Table.PrimaryKey.Count() > 0)
                        {
                            dataView.Table.TableName = tableName;
                            writer.DeleteRows(dataView);
                            //foreach (DataRowView rowView in dataView)
                            //    rowView.Row.AcceptChanges();
                        }
                        else
                        {
                            try
                            {
                                dataView.Table.TableName = tableName;
                                writer.DeleteRows(dataView, false);
                            }
                            catch (Exception)
                            {
                                //log.Info(Properties.Resources.NullPrimaryKey, null);
                            }
                        }
                    }

                    // added rows
                    dataView = AddedRows;
                    if (dataView.Count > 0)
                    {
                        dataView.Table.TableName = tableName;
                        var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                           where c.AutoIncrement == true
                                           select c.ColumnName).ToList();
                        if (columnsInfo == null)
                            InitColumnsInfo(dataView.Table.TableName);
                        writer.InsertRows(dataView, listColumns, true, columnsInfo);
                        //foreach (DataRowView rowView in dataView)
                        //    rowView.Row.AcceptChanges();
                    }

                    // update rows
                    dataView = UpdatedRows;
                    if (dataView.Count > 0)
                    {
                        if (dataView.Table.PrimaryKey != null && dataView.Table.PrimaryKey.Count() > 0)
                        {
                            dataView.Table.TableName = tableName;
                            var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                               where c.AutoIncrement == true
                                               select c.ColumnName).ToList();
                            if (columnsInfo == null)
                                InitColumnsInfo(dataView.Table.TableName);
                            writer.UpdateRows(dataView, listColumns, true, !allowPrimaryKeyChanging, columnsInfo);
                        }
                        else
                        {
                            dataView.Table.TableName = tableName;
                            var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                               where c.AutoIncrement == true
                                               select c.ColumnName).ToList();
                            if (columnsInfo == null)
                                InitColumnsInfo(dataView.Table.TableName);
                            writer.UpdateRows(dataView, listColumns, true, false, columnsInfo);
                        }
                    }

                    writer.Commit();
                }
                catch (Exception)
                {
                    writer.TryRollback();
                    throw;
                }
            }
        }

        DataSet DataSet_MergePendingChanges(object pendingChanges, DataTable table)
        {
            //merging original dataset with client pending changes
            var originalDataset = dataSet;
            var jobject = (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(pendingChanges));
            if (jobject["_edited"] != null)
            {
                foreach (JProperty entry in jobject["_edited"])
                {
                    int rowIndex = int.Parse(entry.Name.Substring(1));
                    foreach (JProperty entryValue in entry.Value)
                    {
                        var colName = entryValue.Name;
                        DataColumn col = (from DataColumn c in table.Columns where c.ToString() == colName select c).First();
                        int colIndex = table.Columns.IndexOf(col);
                        table.Rows[rowIndex][colIndex] = JsonValueToDBValue(entryValue.Value, col.DataType);
                    }
                }
            }
            if (jobject["_deleted"] != null)
            {
                foreach (JValue entry in jobject["_deleted"])
                {
                    var rowIndex = (long)entry.Value;
                    table.Rows[(int)rowIndex].Delete();
                }
            }
            if (jobject["_added"] != null)
            {
                foreach (JProperty rowEntry in jobject["_added"])
                {
                    var key = rowEntry.Name;
                    var rowJsonData = rowEntry.Value;
                    var newRow = table.NewRow();
                    foreach (DataColumn col in table.Columns)
                    {
                        var colName = col.ToString();
                        var clientValue = (from JProperty colEntry in rowEntry.First where colEntry.Name == colName select colEntry).FirstOrDefault();
                        if (clientValue != null)
                            newRow[colName] = JsonValueToDBValue(clientValue.Value, col.DataType);
                        else //setting default cell value
                            newRow[colName] = GetDataColumnDefaultValue(col);
                    }
                    table.Rows.Add(newRow);
                }
            }
            return originalDataset;
        }

        string GetColumnDataType(DataColumn col)
        {
            var dbType = col.DataType;
            if (dbType == typeof(System.DateTime))
                return "datetime";
            if (dbType == typeof(System.Boolean))
                return "boolean";
            if (dbType == typeof(float) ||
                    dbType == typeof(System.Double) ||
                    dbType == typeof(System.Single))
                return "float";
            if (dbType == typeof(System.Decimal))
                return "decimal";
            if (dbType == typeof(System.UInt16) ||
                    dbType == typeof(System.UInt32) ||
                    dbType == typeof(System.UInt64) ||
                    dbType == typeof(System.Int16) ||
                    dbType == typeof(System.Int32) ||
                    dbType == typeof(System.Int64) ||
                    dbType == typeof(System.Byte) ||
                    dbType == typeof(System.SByte))
                return "number";
            return "string";
        }

        object GetDataColumnDefaultValue(DataColumn col)
        {
            var dbType = col.DataType;
            var defValue = col.DefaultValue;
            bool isAutoIncrement = col.AutoIncrement;
            if (dbType == typeof(System.DateTime))
                defValue = DateTime.Today;
            else if (dbType == typeof(System.String))
                defValue = string.Empty;
            else
                if (dbType == typeof(System.UInt16) ||
                    dbType == typeof(System.UInt32) ||
                    dbType == typeof(System.UInt64) ||
                    dbType == typeof(System.Double) ||
                    dbType == typeof(System.Single) ||
                    dbType == typeof(System.Int16) ||
                    dbType == typeof(System.Int32) ||
                    dbType == typeof(System.Int64) ||
                    dbType == typeof(System.Double) ||
                    dbType == typeof(System.Single) ||
                    dbType == typeof(System.Byte) ||
                    dbType == typeof(System.SByte) ||
                    dbType == typeof(System.Decimal))
            {
                if (isAutoIncrement)
                    defValue = System.DBNull.Value;
                else
                    defValue = 0;
            }
            else
                defValue = defValue is System.DBNull ? System.DBNull.Value : defValue;
            return defValue;
        }

        object JsonValueToDBValue(JToken jToken, Type dbColumnDataType)
        {
            var cellValue = jToken.ToObject<object>(); //jToken.Type
            if (cellValue == null)
                return DBNull.Value;

            if (dbColumnDataType == typeof(bool))
            {
                if (Boolean.TryParse(cellValue.ToString(), out bool b))
                    cellValue = b;
                else
                {
                    try
                    {
                        cellValue = Convert.ToBoolean(Convert.ToInt32(cellValue.ToString()));
                    }
                    catch { }
                }
            }
            else if (dbColumnDataType == typeof(float) || dbColumnDataType == typeof(double) || dbColumnDataType == typeof(decimal))
            {
                var currentCultureSeparator = new System.Globalization.CultureInfo(PlatformComponents.StringEditorComponent.GetActiveCulture(PlatformComponents.GetProjectDocument())).NumberFormat.NumberDecimalSeparator;
                cellValue = cellValue.ToString().Replace(currentCultureSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }

            return cellValue;
        }

        private DataView AddedRows
        {
            get
            {
                lock (lockObject)
                {
                    using (DataView dataView = new DataView(gridDataView.Table.Copy()) { RowStateFilter = DataViewRowState.Added })
                    {
                        return CopyDataView(dataView);
                    }
                }
            }
        }
        private DataView DeletedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(gridDataView.Table.Copy())
                    {
                        RowStateFilter = DataViewRowState.Deleted
                    };
                    return dataView;
                }
            }
        }
        private DataView UpdatedRows
        {
            get
            {
                lock (lockObject)
                {
                    using (DataView dataView = new DataView(gridDataView.Table.Copy()) { RowStateFilter = DataViewRowState.ModifiedCurrent })
                    {
                        return CopyDataView(dataView);
                    }
                }
            }
        }

        void InitColumnsInfo(string tablename)
        {
            if (!string.IsNullOrEmpty(tablename))
            {
                columnsInfo = new Dictionary<string, DataReader.SchemaInfo.ColumnInfo>();
                string[] restrictions = new string[4];
                restrictions[2] = tablename;
                var dynDataSourceInformation = DataReader.DataReader.GetSchemaInfo(defaultDataProvider, defaultConnectionString, "Columns", restrictions).ToList();
                if (dynDataSourceInformation.Count > 0)
                {
                    foreach (var dynColumn in dynDataSourceInformation)
                    {
                        var info = dynColumn as IDictionary<String, Object>;
                        if (info.ContainsKey("COLUMN_NAME") && !(info["COLUMN_NAME"] is System.DBNull))
                        {
                            DataReader.SchemaInfo.ColumnInfo cinfo = new DataReader.SchemaInfo.ColumnInfo();
                            string column = info["COLUMN_NAME"] as String;
                            if (info.ContainsKey("NUMERIC_PRECISION") && !(info["NUMERIC_PRECISION"] is System.DBNull))
                            {
                                short precision = short.Parse(info["NUMERIC_PRECISION"].ToString());
                                cinfo.Precision = precision;
                            }
                            if (info.ContainsKey("NUMERIC_SCALE") && !(info["NUMERIC_SCALE"] is System.DBNull))
                            {
                                short scale = short.Parse(info["NUMERIC_SCALE"].ToString());
                                cinfo.Scale = scale;
                            }
                            if (!columnsInfo.ContainsKey(column))
                                columnsInfo.Add(column, cinfo);
                        }
                    }
                }
            }
        }
    }
}
