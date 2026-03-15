using DataGridElementSettings;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UFWebClient.HTML5.Hubs
{
    [DataContract]
    public class DataElement
    {
        [DataMember]
        public String Name;
        [DataMember]
        public String Error;

        [DataMember]
        public List<DateTime> DateTimes;
        [DataMember]
        public List<String> Names;
        [DataMember]
        public List<List<Double>> Values;

        [DataMember]
        public DateTime Start;
        [DataMember]
        public DateTime End;
    }

    public class DataHub : Hub
    {
        public static DataGridRetrieverSection _Config =
                ConfigurationManager.GetSection("DataAnalisysRetriever") as DataGridRetrieverSection;

        public static string StartDateParameter = "@StartDate";
        public static string EndDateParameter = "@EndDate";
        public static DateTime dateTimeUnsetValue = new DateTime(1970, 1, 1);

        public override System.Threading.Tasks.Task OnConnected()
        {
            Debug.WriteLine("New connection : " + Context.ConnectionId);
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Disconnection : " + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public override System.Threading.Tasks.Task OnReconnected()
        {
            Debug.WriteLine("Reconnection : " + Context.ConnectionId);
            return base.OnReconnected();
        }

        public DataElement GetDataSource(String dataSourceSettingsName, DateTime start, DateTime end)
        {
            var ret = new DataElement();
            ret.Name = dataSourceSettingsName;
            DataGridElement foundElement = null;
            foreach (DataGridElement element in _Config.DataGrids)
            {
                if (element.Name == dataSourceSettingsName)
                {
                    foundElement = element;
                    break;
                }
            }

            if (foundElement == null)
            {
                ret.Error = Properties.Resources.NoDataSourceFound;
                return ret;
            }

            ret.Names = new List<String>();
            ret.DateTimes = new List<DateTime>();
            ret.Values = new List<List<double>>();

            int colTime = -1;
            string colTimeName = null;
            var listColValue = new List<int>();
            try
            {
                using (var dataSet = GetCurrentTable(foundElement, start, end))
                {
                    var dataTable = dataSet.Tables[0];

                    for (var iCol = 0; iCol < dataTable.Columns.Count; iCol++)
                    {
                        if (dataTable.Columns[iCol].DataType == typeof(DateTime))
                        {
                            colTime = iCol;
                            colTimeName = dataTable.Columns[iCol].ColumnName;
                            break;
                        }
                    }

                    if (colTime == -1)
                    {
                        ret.Error = Properties.Resources.MissingTimeColumn;
                        return ret;
                    }

                    for (var iCol = 0; iCol < dataTable.Columns.Count; iCol++)
                    {
                        if (dataTable.Columns[iCol].DataType.IsPrimitive)
                        {
                            listColValue.Add(iCol);

                            ret.Names.Add(dataTable.Columns[iCol].ColumnName);
                            ret.Values.Add(new List<double>());
                        }
                    }

                    if (listColValue.Count <= 0)
                    {
                        ret.Error = Properties.Resources.MissingValuesColumn;
                        return ret;
                    }

                    using (DataView dataView = new DataView(dataTable))
                    {
                        if (!String.IsNullOrEmpty(colTimeName))
                            dataView.Sort = String.Format("[{0}] ASC", colTimeName);

                        foreach (DataRowView rowView in dataView)
                        {
                            var dateTime = (DateTime)rowView.Row[colTime];
                            if (start != dateTimeUnsetValue && dateTime < start)
                                continue;
                            if (end != dateTimeUnsetValue && dateTime > end)
                                continue;

                            ret.DateTimes.Add(dateTime);

                            int n = 0;
                            listColValue.ForEach(col =>
                            {
                                double d = 0;

                                try
                                {
                                    d = Convert.ToDouble(rowView.Row[col]);
                                }
                                catch
                                {
                                }

                                ret.Values[n++].Add(d);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("{0}: {1}", Properties.Resources.ErrorAccessingDataSource, ex.Message);
                ret.Error = error;
            }

            if (ret.DateTimes.Count > 0)
            {
                ret.Start = ret.DateTimes[0];
                ret.End = ret.DateTimes[ret.DateTimes.Count - 1];
            }
            else
            {
                ret.Start = start;
                ret.End = end;
            }

      return ret;
        }

        DataSet GetCurrentTable(DataGridElement dataSource, DateTime start, DateTime end)
        {
            string defaultDataProvider = null;
            string defaultConnectionString = null;
            if (XpoHelpers.XpoHelper.IsDataSource(dataSource.ConnectionString))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(dataSource.ConnectionString);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(dataSource.ConnectionString);
            }
            else
            {
                var helper = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(dataSource.ConnectionString);
                defaultDataProvider = helper.GetPartByName("DataProvider");
                helper.RemovePartByName("DataProvider");
                defaultConnectionString = helper.GetConnectionString();
            }

            using (var connection = DataReader.DataReader.CreateDbConnection(defaultDataProvider, defaultConnectionString))
            {
                try
                {
                    connection.Open();

                    var ret = new DataSet();
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(defaultDataProvider);
                    DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(defaultDataProvider, defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(defaultDataProvider);
                    dbdapater.SelectCommand.Connection = connection;

                    var query = dataSource.Query;
                    if (query.Contains(StartDateParameter))
                    {
                        if (start < SqlDateTime.MinValue.Value || start == dateTimeUnsetValue)
                            start = SqlDateTime.MinValue.Value;
                        else if (start > SqlDateTime.MaxValue.Value)
                            start = SqlDateTime.MaxValue.Value;

                        var datestart = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                        datestart.DbType = System.Data.DbType.DateTime;
                        datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                        datestart.Value = start;
                        dbdapater.SelectCommand.Parameters.Add(datestart);
                        query = query.Replace(StartDateParameter, datestart.ParameterName);
                    }

                    if (query.Contains(EndDateParameter))
                    {
                        if (end > SqlDateTime.MaxValue.Value || end == dateTimeUnsetValue)
                            end = SqlDateTime.MaxValue.Value;
                        else if (end > SqlDateTime.MaxValue.Value)
                            end = SqlDateTime.MaxValue.Value;

                        var dateend = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                        dateend.DbType = System.Data.DbType.DateTime;
                        dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                        dateend.Value = end;
                        dbdapater.SelectCommand.Parameters.Add(dateend);
                        query = query.Replace(EndDateParameter, dateend.ParameterName);
                    }
                    
                    dbdapater.SelectCommand.CommandText = query;
                    
                    dbdapater.Fill(ret);

                    return ret;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}