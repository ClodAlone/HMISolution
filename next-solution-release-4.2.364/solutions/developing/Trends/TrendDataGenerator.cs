using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using Opc.Ua;
using OPCUAViewModel;
using Utilities;
using System.ComponentModel;
using System.Windows.Data;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using System.Data;
using DataReader.SchemaInfo;
using log4net;
using System.Diagnostics;

namespace Trends
{
    public class TrendDataGenerator : WPFPenHelpers.BaseDataGenerator
    {
        #region Declarations
        readonly static string nullValue = new DataValue().ToString();
        #endregion

        #region Constructors
        public TrendDataGenerator(string penId, WPFPenHelpers.DataGeneratorSettings settings, int commandTimeout)
            : base(penId, settings, commandTimeout)
        { }
        #endregion

        #region Public Properties
        double minValue;
        public double MinValue
        {
            get
            {
                return minValue;
            }
        }

        double maxValue;
        public double MaxValue
        {
            get
            {
                return maxValue;
            }
        }

        double avgValue;
        public double AvgValue
        {
            get
            {
                return avgValue;
            }
        }

        SafeObservableCollection<DataValue> values;
        public ICollectionView Values
        {
            get
            {
                if (values == null)
                    return null;
                return CollectionViewSource.GetDefaultView(values);
            }
        }
        
        public ReadOnlyList<DataValue> HistoryData
        {
            get
            {
                if (values == null)
                    return null;
                return new ReadOnlyList<DataValue>(values);
            }
        }
        #endregion

        #region Public Methods
        public void AddData()
        {
            if (values == null || LastValue == null || LastValue.Value == null)
                return;
            
            AddData(LastValue.Value);
        }

        public void AddData(object newValue, bool onlyUpdateLastValue = false)
        {
            if (values == null)
                return;

            var dValue = double.NaN;

            if (newValue is Boolean)
                dValue = (bool)newValue ? 1.0 : 0.0;
            else
                try { dValue = System.Convert.ToDouble(newValue, System.Globalization.CultureInfo.InvariantCulture); } catch { }

            if (Double.IsInfinity(dValue) || Double.IsNaN(dValue) || newValue == null)
                dValue = double.NaN;

            var dataValue = new DataValue()
            {
                SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now,
                Value = dValue
            };

            LastValue = dataValue;

            if (!onlyUpdateLastValue)
            {
                values.Add(dataValue);
                while (values.Count > Settings.HDataCount)
                    values.RemoveAt(0);
            }

            UpdateStat();
        }
        #endregion

        #region Methods
        void InitializeValues()
        {
            var list = GetInitialBufferValues(Settings.HDataCount);
            values = new SafeObservableCollection<DataValue>(list);

            //var task1 = Task.Factory.StartNew(() =>
            //{
            //    return GetInitialBufferValues(Settings.HDataCount);
            //});
            //var task2 = task1.ContinueWith(ret =>
            //{
            //    if (ret.Exception != null)
            //    {
            //        OnError(ret.Exception.InnerException.Message);
            //    }
            //    else if (ret.Result != null)
            //    {
            //        values = new SafeObservableCollection<DataValue>(ret.Result);
            //    }
            //}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        void UpdateStat()
        {
            try
            {
                var statDouble = (from c in HistoryData
                                  where IsValidDouble(c.Value)
                                  select System.Convert.ToDouble(c.Value, System.Globalization.CultureInfo.InvariantCulture)).ToList();
                maxValue = statDouble.Max();
                minValue = statDouble.Min();
                avgValue = statDouble.Average();
            }
            catch
            {
                maxValue = 0;
                minValue = 0;
                avgValue = 0;
            }
        }

        bool IsValidDouble(object val)
        {
            try
            {
                var t = System.Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture);
                return !double.IsNaN(t);
            }
            catch
            {
                return false;
            }
        }

        DataValue GetNearestItemInDBResult(DateTime sDate, List<DataValue> ret)
        {
            var item = (from c in ret
                        where DateTime.Compare(sDate, (c as DataValue).SourceTimestamp) >= 0
                        select c).FirstOrDefault();
            if (item != null)
                ret.RemoveAll((c) => DateTime.Compare(sDate, (c as DataValue).SourceTimestamp) <= 0);

            return item;
        }
        
        List<DataValue> CreateDataSource(string nodeid, DateTime startTime, DateTime endTime)
        {
            var utcStart = startTime.ToUniversalTime();
            var utcEnd = endTime.ToUniversalTime();

            if (!Settings.DlrSource)
            {
                #region Historical
                if (string.IsNullOrEmpty(nodeid))
                    return null;
                CreateConnectionStringDataLayer();
                if (ufw == null)
                    return new List<DataValue>();
                
                if (string.IsNullOrEmpty(nodeid))
                    return null;

                var info = HistorianHelper.GetAuditDataLogInfo(nodeid, ufw);
                if (info == null)
                    return new List<DataValue>();

                List<DataValue> ret;
                if (utcStart == DateTime.MinValue && utcEnd == DateTime.MaxValue)
                {
                    ret = isArray ? (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                where entry.DataLogRef == info.Oid &&
                                entry.RecordDateTimeUtc != null
                                //&& entry.Value != nullValue
                            orderby entry.RecordDateTimeUtc descending
                                select new DataValue()
                                {
                                    //Value = entry.dValue,
                                    Value = entry.Value,
                                    SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                                }).Take(Settings.HDataCount).ToList()

                                :

                                (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                where entry.DataLogRef == info.Oid &&
                                entry.RecordDateTimeUtc != null
                                //&& entry.Value != nullValue
                                orderby entry.RecordDateTimeUtc descending
                                select new DataValue()
                                {
                                    SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime(),
                                    Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null
                                }
                                ).Take(Settings.HDataCount).ToList();
                }
                else if (utcStart == utcEnd)
                {
                    ret = isArray ? (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                where entry.DataLogRef == info.Oid &&
                                //entry.Value != nullValue &&
                                entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc <= utcEnd
                                orderby entry.RecordDateTimeUtc descending
                                select new DataValue()
                                {
                                    //Value = entry.dValue,
                                    Value = entry.Value,
                                    SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                                }).Take(Settings.HDataCount).ToList() 
                                   
                                :
                                   
                                (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                where entry.DataLogRef == info.Oid &&
                                //entry.Value != nullValue &&
                                entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc <= utcEnd
                                orderby entry.RecordDateTimeUtc descending
                                 select new DataValue()
                                 {
                                     SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime(),
                                     Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null
                                 }
                                ).Take(Settings.HDataCount).ToList(); 
                }
                else
                {
                    ret = isArray ? (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                     where entry.DataLogRef == info.Oid &&
                                     //entry.Value != nullValue &&
                                     entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                                     orderby entry.RecordDateTimeUtc descending
                                     select new DataValue()
                                     {
                                         //Value = entry.dValue,
                                         Value = entry.Value,
                                         SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                                     }).Take(Settings.HDataCount).ToList()

                                :

                                (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                 where entry.DataLogRef == info.Oid &&
                                 //entry.Value != nullValue &&
                                 entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                                 orderby entry.RecordDateTimeUtc descending
                                 select new DataValue()
                                 {
                                     SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime(),
                                     Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null
                                 }
                                ).Take(Settings.HDataCount).ToList();
                }
                Parallel.ForEach(ret,x =>
                {
                    if (Settings.ClientTimezoneOffset != TimeSpan.Zero)
                        x.SourceTimestamp = x.SourceTimestamp + Settings.ClientTimezoneOffset;
                    if (x.Value == null)
                        x.Value = double.NaN;
                });
                return ret;
                #endregion
            }
            else
            {
                #region Datalogger
                string _defaultConnectionString;
                string _defaultDataProvider;

                if (!GetConnectionAndDataprovider(out _defaultDataProvider, out _defaultConnectionString))
                    return null;

                string _tablename = Settings.DlrName;
                string realTagname = Settings.ColName;
                string _utccolumnname = Settings.UtcTimeColumnName;

                using (var dbconnection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                {
                    dbconnection.Open();
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);

                    DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                    dbdapater.SelectCommand.Connection = dbconnection;
                    if (CommandTimeout != 0)
                        dbdapater.SelectCommand.CommandTimeout = CommandTimeout;

                    StringBuilder commantText = new StringBuilder("SELECT ");
                    DataSet gridDataSet = new DataSet();

                    if (utcStart == DateTime.MinValue && utcEnd == DateTime.MaxValue)
                    {
                        if (dbSchemaInfo.IsSupportedTopKeyword)
                            commantText.AppendFormat("TOP {0} ", Settings.HDataCount);
                        commantText.AppendFormat("{3}, {1} FROM {0} ORDER BY {2} DESC",
                            dbSchemaInfo.WrapObjectName(_tablename),
                            dbSchemaInfo.WrapObjectName(realTagname),
                            dbSchemaInfo.WrapObjectName(_utccolumnname),
                            dbSchemaInfo.WrapObjectName(_utccolumnname));

                        dbdapater.SelectCommand.CommandText = commantText.ToString();

                        dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                        dbdapater.Fill(gridDataSet, _tablename);
                    }
                    else if (utcStart == utcEnd)
                    {
                        if (dbSchemaInfo.IsSupportedTopKeyword)
                            commantText.AppendFormat("TOP {0} ", Settings.HDataCount);
                        commantText.AppendFormat("{2}, {1} FROM {0}",
                            dbSchemaInfo.WrapObjectName(_tablename),
                            dbSchemaInfo.WrapObjectName(realTagname),
                            dbSchemaInfo.WrapObjectName(_utccolumnname));

                        var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                        dateend.DbType = System.Data.DbType.DateTime;
                        dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                        dateend.Value = utcEnd;
                        dbdapater.SelectCommand.Parameters.Add(dateend);

                        commantText.AppendFormat(" WHERE {0} <= {1}  ORDER BY {2} DESC",
                            dbSchemaInfo.WrapObjectName(_utccolumnname),
                            dateend.ParameterName,
                            dbSchemaInfo.WrapObjectName(_utccolumnname));

                        dbdapater.SelectCommand.CommandText = commantText.ToString();

                        dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                        dbdapater.Fill(gridDataSet, _tablename);
                    }
                    else
                    {
                        if (dbSchemaInfo.IsSupportedTopKeyword)
                            commantText.AppendFormat("TOP {0} ", Settings.HDataCount);
                        commantText.AppendFormat("{2}, {1} FROM {0}",
                            dbSchemaInfo.WrapObjectName(_tablename),
                            dbSchemaInfo.WrapObjectName(realTagname),
                            dbSchemaInfo.WrapObjectName(_utccolumnname));

                        var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                        datestart.DbType = System.Data.DbType.DateTime;
                        datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                        datestart.Value = utcStart;
                        dbdapater.SelectCommand.Parameters.Add(datestart);

                        var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                        dateend.DbType = System.Data.DbType.DateTime;
                        dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                        dateend.Value = utcEnd;
                        dbdapater.SelectCommand.Parameters.Add(dateend);

                        commantText.AppendFormat(" WHERE {0} >= {1} AND {2} <= {3}  ORDER BY {4} DESC",
                            dbSchemaInfo.WrapObjectName(_utccolumnname),
                            datestart.ParameterName,
                            dbSchemaInfo.WrapObjectName(_utccolumnname),
                            dateend.ParameterName,
                            dbSchemaInfo.WrapObjectName(_utccolumnname));

                        dbdapater.SelectCommand.CommandText = commantText.ToString();

                        dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                        dbdapater.Fill(gridDataSet, _tablename);
                    }

                    List<DataValue> ret = new List<DataValue>();
                    using (DataView dataView = new DataView(gridDataSet.Tables[0]))
                    {
                        dataView.Sort = string.Format("{0} DESC", _utccolumnname);

                        foreach (DataRowView rowView in dataView)
                        {
                            DataValue dataValue = new DataValue()
                            {
                                Value = GetNullOrValidDouble(rowView[realTagname]) ?? double.NaN
                            };

                            try
                            {
                                dataValue.SourceTimestamp = ((DateTime)rowView[_utccolumnname]).ToLocalTime();
                            }
                            catch
                            {
                                dataValue.SourceTimestamp = DateTime.MinValue;
                            }

                            ret.Add(dataValue);
                        }
                    }
                        
                    return ret;
                }
#endregion
            }
        }
        #endregion

        double? GetNullOrValidDouble(object item)
        {
            double? val = null;
            if (item is DBNull)
                return val;
            try
            {
                val = System.Convert.ToDouble(item, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            return val;
        }

        #region Overrides
        protected override void Initialize()
        {
            //base.Initialize(); //Case 20920

            InitializeValues();
        }

        protected override void SetDataSources()
        {
            DateTime startTime = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now;
            DateTime endTime = startTime;

            var templ = new List<DataValue>();
            if (values != null)
                templ.AddRange(values);

            var task1 = Task.Factory.StartNew(() =>
            {
                string nodeID = absolutenodeid;
                string name = string.Empty;

                var ret = CreateDataSource(nodeID, startTime, endTime);

                if (ret != null && ret.Count != 0)
                {
                    int dcount;
                    var _sampnum = (from c in templ
                                    where DateTime.Compare((c as DataValue).SourceTimestamp, endTime) >= 0
                                    select c).ToList().Count();

                    dcount = templ.Count() > _sampnum ? templ.Count() - _sampnum : templ.Count();
                    DateTime sDate;
                    if (dcount == templ.Count())
                        sDate = endTime;
                    else
                        sDate = DateTime.Compare(templ[dcount].SourceTimestamp, DeadBandValue) == 0 ? endTime : templ[dcount].SourceTimestamp;

                    if (dcount == 0)
                        return null;

                    if (isArray)
                    {
                        bool bError = false;
                        if (isBool)
                        {
                            for (int i = dcount - 1; i >= 0; i--)
                            {
                                sDate = sDate.Subtract(Settings.DeadBandInterval);
                                DataValue item = GetNearestItemInDBResult(sDate, ret);

                                if (item != null)
                                {
                                    string _value = item.Value.ToString();
                                    string source = _value.Substring(1, _value.Length - 2);
                                    string[] stringSeparators = new string[] { " |" };
                                    string[] result = source.Split(stringSeparators, StringSplitOptions.None);
                                    if (result.Length > Settings.ArrayIndex)
                                    {
                                        var recValue = result[Settings.ArrayIndex];
                                        if (recValue != nullValue)
                                        {
                                            if (String.Compare(recValue, "False", StringComparison.OrdinalIgnoreCase) == 0)
                                                templ[i].Value = 0;
                                            else
                                                templ[i].Value = 1;
                                        }
                                        else
                                            templ[i].Value = 0;
                                        templ[i].SourceTimestamp = sDate;
                                    }
                                    else
                                    {
                                        bError = true;
                                        templ[i].Value = 0;
                                        templ[i].SourceTimestamp = sDate;
                                        templ[i].StatusCode = StatusCodes.BadIndexRangeNoData;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = dcount - 1; i >= 0; i--)
                            {
                                sDate = sDate.Subtract(Settings.DeadBandInterval);
                                DataValue item = GetNearestItemInDBResult(sDate, ret);

                                if (item != null)
                                {
                                    string _value = item.Value.ToString();
                                    string source = _value.Substring(1, _value.Length - 2);
                                    string[] stringSeparators = new string[] { " |" };
                                    string[] result = source.Split(stringSeparators, StringSplitOptions.None);
                                    if (result.Length > Settings.ArrayIndex)
                                    {
                                        var recValue = result[Settings.ArrayIndex];
                                        var dValue = 0.0;
                                        try { dValue = System.Convert.ToDouble(recValue, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                                        templ[i].Value = dValue;
                                        templ[i].SourceTimestamp = sDate;
                                    }
                                    else
                                    {
                                        bError = true;
                                        templ[i].Value = 0.0;
                                        templ[i].SourceTimestamp = sDate;
                                        templ[i].StatusCode = StatusCodes.BadIndexRangeNoData;
                                    }
                                }
                            }
                        }

                        if (bError)
                        {
                            OnError(string.Format(Properties.Resources.OutOfRangeIndexOnLoadingHistoricalValues, Settings.ArrayIndex));
                        }
                    }
                    else
                    {
                        for (int i = dcount - 1; i >= 0; i--)
                        {
                            sDate = sDate.Subtract(Settings.DeadBandInterval);
                            DataValue item = GetNearestItemInDBResult(sDate, ret);

                            if (item != null)
                            {
                                templ[i].Value = item.Value;
                                templ[i].SourceTimestamp = sDate;
                            }
                        }
                    }

                    return templ.Take(Settings.HDataCount);
                }

                return null;
            });
            pendingTask.Add(task1);
            task1.ContinueWith(ret =>
            {
                pendingTask.Remove(task1);

                if (ret.Exception != null)
                {
                    OnError(ret.Exception.InnerException.Message);
                }
                else if (ret.Result != null)
                {
                    values = new SafeObservableCollection<DataValue>(ret.Result);
                    UpdateStat();
                    OnHistoryLoaded();
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (values != null)
                values.Clear();
        }
        #endregion
    }
 
}
