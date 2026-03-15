using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using Opc.Ua;
using OPCUAViewModel;
using System.Diagnostics;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using log4net;
using Utilities;
using System.Collections.ObjectModel;
using DataReader.SchemaInfo;
using System.Text;
using System.Data;
using WPFUtilities;
using System.Threading;

namespace WPFPenHelpers
{
    public class SCDataGenerator : BaseDataGenerator
    {
        #region Declarations
        //int realTimeDataBufferSize = Properties.Settings.Default.RealtimeBufferSize;
        List<MyDataValue> currentRecordset;
        List<MyDataValue> newRecordset;
        DateTime currentRecordset_startDate = DateTime.MaxValue;
        DateTime currentRecordset_endDate = DateTime.MinValue;
        #endregion

        #region Constructors
        public SCDataGenerator(string penId, SCDataGeneratorSettings settings, int commandTimeout)
            : base(penId, settings, commandTimeout)
        { }
        #endregion

        #region Public Properties
        public new SCDataGeneratorSettings Settings
        {
            get
            {
                return (SCDataGeneratorSettings)settings;
            }
        }

        SafeObservableCollection<MyDataValue> values;
        public SafeObservableCollection<MyDataValue> Values
        {
            get
            {
                return values;
            }
        }

        SafeObservableCollection<MyDataValue> secondValues;
        public SafeObservableCollection<MyDataValue> SecondValues
        {
            get
            {
                return secondValues;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Methods
        void InitializeValues()
        {
            var list = GetInitialBufferValues(Settings.HDataCount);
            values = new SafeObservableCollection<MyDataValue>(list);
            secondValues = new SafeObservableCollection<MyDataValue>(list);
        }

        MyDataValue[] GetLongestItemInRangeDBResult(DateTime sDate, DateTime oldsDate, List<MyDataValue> ret) //ret is ordered by date desc.
        {
            Dictionary<double, TimeSpan> timeSum = new Dictionary<double, TimeSpan>(); // foreach different value, we're summing the timespans to find the longest
            var changesBetweenDates = (from c in ret
                                       where (DateTime.Compare(sDate, c.SourceTimeStamp) < 0 && DateTime.Compare(oldsDate, c.SourceTimeStamp) >= 0)
                                       select c).ToList();
            var precedingSDateDataValue = (from d in ret
                                           where DateTime.Compare(sDate, d.SourceTimeStamp) >= 0
                                           select d).FirstOrDefault();
            if (changesBetweenDates == null || changesBetweenDates.Count == 0)
            {
                //if (precedingSDateDataValue != null)
                //    ret.RemoveAll((d) => DateTime.Compare(sDate, (d as DataValue).SourceTimestamp) <= 0);

                return new MyDataValue[] { precedingSDateDataValue, precedingSDateDataValue };
            }
            else
            {
                DateTime prevTimestamp = oldsDate;
                for (int i = 0; i < changesBetweenDates.Count; i++) //time descending
                {
                    double val = Datavalue_ToInvariantDouble(changesBetweenDates[i]);
                    DateTime actTimestamp = changesBetweenDates[i].SourceTimeStamp;
                    if (timeSum.ContainsKey(val))
                        timeSum[val] += prevTimestamp - actTimestamp;
                    else
                        timeSum.Add(val, prevTimestamp - actTimestamp);
                    prevTimestamp = actTimestamp;
                }
                if (precedingSDateDataValue != null)
                {
                    double precval = Datavalue_ToInvariantDouble(precedingSDateDataValue);
                    if (timeSum.ContainsKey(precval))
                        timeSum[precval] += prevTimestamp - sDate;
                    else
                        timeSum.Add(precval, prevTimestamp - sDate);
                }
            }
            double longestDouble = timeSum.FirstOrDefault(x => x.Value == timeSum.Values.Max()).Key;
            double secondDouble = timeSum.ElementAt(0).Key;
            int j = 1;
            while (longestDouble == secondDouble && j < timeSum.Count())
            {
                secondDouble = timeSum.ElementAt(j).Key;
                j++;
            }
            return new MyDataValue[] { new MyDataValue(longestDouble), new MyDataValue(secondDouble) };
        }

        double Datavalue_ToInvariantDouble(MyDataValue value)
        {
            if (value.Value is Boolean)
                return (bool)value.Value ? 1 : 0;
            var ret = double.NaN;
            if (value.Value == null)
                return ret;
            try
            {
                ret = Convert.ToDouble(value.Value, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            return ret;
        }

        List<MyDataValue> CreateDataSource(string nodeid, DateTime startTime, DateTime endTime, DateTime originalStartTime, DateTime originalEndTime, out bool bMaxRecordsExceeded, CancellationToken ct)
        {
            bMaxRecordsExceeded = false;
            var utcStart = startTime.ToUniversalTime();
            var utcEnd = DateTime.Compare(endTime.ToUniversalTime(), DateTime.UtcNow) <= 0 ? endTime.ToUniversalTime() : DateTime.UtcNow;
            MyDataValue firstBeforeUtcStart = null;
            List<MyDataValue> ret = new List<MyDataValue>();

            if (!Settings.DlrSource)
            {
                if (string.IsNullOrEmpty(nodeid))
                    return null;

                CreateConnectionStringDataLayer();
                if (ufw == null)
                    return null;

                var info = HistorianHelper.GetAuditDataLogInfo(nodeid, ufw);
                if (ct.IsCancellationRequested || info == null)
                    return new List<MyDataValue>();

                if (utcStart == DateTime.MinValue && utcEnd == DateTime.MaxValue)
                {
                    ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                           where entry.DataLogRef == info.Oid &&
                           entry.RecordDateTimeUtc != null
                           orderby entry.RecordDateTimeUtc descending
                           select new MyDataValue()
                           {
                               Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                               SourceTimeStamp = entry.RecordDateTimeUtc.ToLocalTime()
                           }).ToList();
                }
                else if (utcStart == utcEnd)
                {
                    ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                            where entry.DataLogRef == info.Oid &&
                            entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc <= utcEnd
                            orderby entry.RecordDateTimeUtc descending
                            select new MyDataValue()
                            {
                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                SourceTimeStamp = entry.RecordDateTimeUtc.ToLocalTime()
                            }).ToList();
                }
                else
                {
                    ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                            where entry.DataLogRef == info.Oid &&
                            entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                            orderby entry.RecordDateTimeUtc descending
                            select new MyDataValue()
                            {
                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                SourceTimeStamp = entry.RecordDateTimeUtc.ToLocalTime()
                            }).ToList();
                }
                //if (ret.Count() > 0)
                //{
                firstBeforeUtcStart = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                                            where entry.DataLogRef == info.Oid &&
                                            entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc < utcStart
                                            orderby entry.RecordDateTimeUtc descending
                                            select new MyDataValue()
                                            {
                                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                                SourceTimeStamp = entry.RecordDateTimeUtc.ToLocalTime()
                                            }).FirstOrDefault();
                if (firstBeforeUtcStart != null)
                {
                    ret.Add(firstBeforeUtcStart);
                }
                //}
            }
            else
            {
                #region Datalogger
                string _defaultDataProvider;
                string _defaultConnectionString;

                if (!GetConnectionAndDataprovider(out _defaultDataProvider, out _defaultConnectionString))
                    return null;

                string _tablename = Settings.DlrName;
                string realTagname = Settings.ColName;
                string _utccolumnname = Settings.UtcTimeColumnName;

                using (var dbconnection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                {
                    if (ct.IsCancellationRequested)
                        return null;
                    dbconnection.Open();
                    if (ct.IsCancellationRequested)
                        return null;
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);

                    DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                    dbdapater.SelectCommand.Connection = dbconnection;
                    dbdapater.SelectCommand.CommandTimeout = CommandTimeout;

                    StringBuilder commantText = new StringBuilder("SELECT ");
                    DataSet gridDataSet = new DataSet();

                    if (utcStart == DateTime.MinValue && utcEnd == DateTime.MaxValue)
                    {
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

                    //Adding the first record before utcstart, so to populate the first columns with the right value (like historian's firstBeforeUtcStart)
                    StringBuilder commantText2 = new StringBuilder("SELECT ");
                    if (dbSchemaInfo.IsSupportedTopKeyword)
                        commantText2.Append("TOP 1 ");
                    commantText2.AppendFormat("{2}, {1} FROM {0}",
                        dbSchemaInfo.WrapObjectName(_tablename),
                        dbSchemaInfo.WrapObjectName(realTagname),
                        dbSchemaInfo.WrapObjectName(_utccolumnname));

                    var dstart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                    dstart.DbType = System.Data.DbType.DateTime;
                    dstart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart2");
                    dstart.Value = utcStart;
                    dbdapater.SelectCommand.Parameters.Add(dstart);

                    commantText2.AppendFormat(" WHERE {0} < {1} ORDER BY {2} DESC",
                        dbSchemaInfo.WrapObjectName(_utccolumnname),
                        dstart.ParameterName,
                        dbSchemaInfo.WrapObjectName(_utccolumnname));

                    dbdapater.SelectCommand.CommandText = commantText2.ToString();

                    dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                    dbdapater.Fill(gridDataSet, _tablename);

                    using (DataView dataView = new DataView(gridDataSet.Tables[0]))
                    {
                        dataView.Sort = string.Format("{0} DESC", _utccolumnname);

                        foreach (DataRowView rowView in dataView)
                        {
                            if (ct.IsCancellationRequested)
                                return null;
                            firstBeforeUtcStart = new MyDataValue()
                            {
                                Value = GetNullOrValidDouble(rowView[realTagname])
                            };

                            try
                            {
                                firstBeforeUtcStart.SourceTimeStamp = ((DateTime)rowView[_utccolumnname]).ToLocalTime();
                            }
                            catch
                            {
                                firstBeforeUtcStart.SourceTimeStamp = DateTime.MinValue;
                            }

                            ret.Add(firstBeforeUtcStart);
                        }
                    }
                }
                #endregion
            }
            if (ct.IsCancellationRequested)
                return null;
            //Showing only records in the current view if count exceeds max value
            if (ret.Count > Settings.HDataCount)
            {
                bMaxRecordsExceeded = true;
                ret = (from entry in ret.AsParallel() where entry.SourceTimeStamp >= originalStartTime && entry.SourceTimeStamp <= originalEndTime select entry).Take(Settings.HDataCount).ToList();
            }
            return ret;
        }

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
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            base.Initialize();

            InitializeValues();
        }

        public void CallSetDataSources(CancellationToken ct)
        {
            SetDataSources(ct);
        }

        int pickedSamples;
        void extendDateRange(out DateTime startTime, out DateTime endTime, out DateTime extendedStartTime, out DateTime extendedEndTime) //optimization: predictive navigation. We are pulling more data from DB than start/end date range.
        {
            startTime = Settings.Storage.DateTimeStart;
            TimeSpan endStartDiff = Settings.Storage.DateTimeEnd.Subtract(Settings.Storage.DateTimeStart);
            extendedStartTime = startTime.Subtract(endStartDiff).Subtract(endStartDiff);
            if (DateTime.Compare(DateTime.Now, Settings.Storage.DateTimeEnd) <= 0)
                endTime = extendedEndTime = DateTime.Now;
            else
            {
                endTime = Settings.Storage.DateTimeEnd;
                extendedEndTime = endTime.Add(endStartDiff).Add(endStartDiff);
                extendedEndTime = DateTime.Compare(DateTime.Now, extendedEndTime) > 0 ? extendedEndTime : DateTime.Now;
            }
            pickedSamples = pickedSamples * 5;
        }

        protected override void SetDataSources(CancellationToken ct)
        {
            DateTime startTime, endTime, extendedStartTime, extendedEndTime;
            pickedSamples = Settings.DataCount;
            extendDateRange(out startTime, out endTime, out extendedStartTime, out extendedEndTime);

            //var task1 = Task.Factory.StartNew(() =>
            //{
                //string nodeID = absolutenodeid;
                string nodeID = PenId;
                var templ = new List<MyDataValue>();
                var templ_secondvalues = new List<MyDataValue>();
                bool newIntervalStartsInside = currentRecordset != null && DateRangesComparer.StartsInside(startTime, currentRecordset_startDate, currentRecordset_endDate);
                bool newIntervalEndsInside = currentRecordset != null && DateRangesComparer.EndsInside(endTime, currentRecordset_endDate, currentRecordset_startDate);
                bool bMaxRecordsExceeded = false;

                if (ct.IsCancellationRequested)
                    return;

                if (!(newIntervalStartsInside && newIntervalEndsInside)) //The new interval isn't a subset of the first one (can't avoid querying for new data)
                {
                    if (currentRecordset != null && currentRecordset.Count >= Settings.MDataCount) //MDataCount = data cache memory limit
                    {
                        currentRecordset = CreateDataSource(nodeID, extendedStartTime, extendedEndTime, startTime, endTime, out bMaxRecordsExceeded, ct);
                        currentRecordset_startDate = extendedStartTime;
                        currentRecordset_endDate = extendedEndTime;
                    }
                    else
                    {
                        newRecordset = CreateDataSource(nodeID, newIntervalStartsInside ? currentRecordset_endDate + TimeSpan.FromTicks(1) : extendedStartTime, newIntervalEndsInside ? currentRecordset_startDate - TimeSpan.FromTicks(1) : extendedEndTime, startTime, endTime, out bMaxRecordsExceeded, ct);
                        if (newIntervalStartsInside || newIntervalEndsInside) //Intersecting intervals
                        {
                            if (startTime < currentRecordset_startDate)
                                currentRecordset = currentRecordset.Union(newRecordset).OrderByDescending(dv => dv.SourceTimeStamp).ToList(); //Optimizable with a more efficient sorting algorithm, for ex.: https://stackoverflow.com/questions/2767007/most-efficient-algorithm-for-merging-sorted-ienumerablet
                            else
                                currentRecordset = newRecordset.Union(currentRecordset).OrderByDescending(dv => dv.SourceTimeStamp).ToList();
                        }
                        else
                            currentRecordset = newRecordset;
                        DateTime oldStart = DateTime.MinValue;
                        if (startTime < currentRecordset_startDate || startTime > currentRecordset_endDate)
                        {
                            oldStart = currentRecordset_startDate;
                            currentRecordset_startDate = extendedStartTime;
                        }
                        if (endTime > currentRecordset_endDate || endTime < oldStart)
                            currentRecordset_endDate = extendedEndTime;
                    }
                }
                if (currentRecordset != null && currentRecordset.Count != 0)
                {
                    DateTime sDate = extendedStartTime, oldsDate;
                    sDate = new DateTime(sDate.Ticks / Settings.DeadBandInterval.Ticks * Settings.DeadBandInterval.Ticks);//Rounding down starting date based on current time axis resolution timespan (RecordEvery)
                    for (int i = 0; i < pickedSamples; i++) //Distributing datasource results to obtain one value for each sampling unit (es minute)
                    {
                        if (ct.IsCancellationRequested)
                            return;
                        oldsDate = sDate;
                        sDate = sDate.Add(Settings.DeadBandInterval);
                        if (DateTime.Compare(oldsDate, endTime) > 0)
                            break;
                        MyDataValue[] items = GetLongestItemInRangeDBResult(oldsDate, sDate, currentRecordset);

                        if (items[0] != null)
                        {
                            templ.Insert(0, new MyDataValue() { Value = items[0].Value, SourceTimeStamp = oldsDate });
                            templ_secondvalues.Insert(0, new MyDataValue() { Value = items[1].Value, SourceTimeStamp = oldsDate });
                        }
                    }
                }
                if (bMaxRecordsExceeded)
                {
                    currentRecordset_startDate = DateTime.MaxValue;
                    currentRecordset_endDate = DateTime.MinValue;
                }
                var ret = new List<MyDataValue>[] { templ, templ_secondvalues };
            //});
            //pendingTask.Add(task1);
            //var task2 = task1.ContinueWith(ret =>
            //{
            //    pendingTask.Remove(task1);

            //    if (ret.Exception != null)
            //    {
            //        OnError(ret.Exception.InnerException.Message);
            //    }
                //else if (ret.Result != null /* && ret.Result[0].Count != 0*/)
                {
                    values = new SafeObservableCollection<MyDataValue>(ret[0]);
                    secondValues = new SafeObservableCollection<MyDataValue>(ret[1]);
                    //OnHistoryLoaded();
                }
            //}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (currentRecordset != null)
                currentRecordset.Clear();

            if (newRecordset != null)
                newRecordset.Clear();

            if (values != null)
                values.Clear();

            if (secondValues != null)
                secondValues.Clear();
        }
        #endregion
    }
}
