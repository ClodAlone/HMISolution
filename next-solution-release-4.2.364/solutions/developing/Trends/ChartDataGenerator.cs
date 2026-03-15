using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using Opc.Ua;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using System.Diagnostics;
using log4net;

namespace Trends
{
    public class ChartDataGenerator : WPFPenHelpers.BaseDataGenerator
    {
        #region Constructors
        public ChartDataGenerator(string penId, WPFPenHelpers.DataGeneratorSettings settings, int commandTimeout) 
            : base(penId, settings, commandTimeout)
        { }

        #endregion

        #region Public Properties
        private DataValue[] values;
        public ReadOnlyList<DataValue> Values
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
        public void AddData(object newValue, int _count)
        {
            if (values == null)
                return;

            var index = _count >= 0 && _count < values.Count() ? _count : 0;

            if (newValue != null && values.Count() > index)
            {
                var dValue = 0.0;
                try { dValue = System.Convert.ToDouble(newValue, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                values[index] = LastValue = new DataValue()
                {
                    Value = dValue,
                    SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now
                };
            }
        }

        public void ResetData()
        {
            if (values == null)
                return;

            for (int i = 0; i < values.Length; i++)
                values[i] = new DataValue() { Value = (Double)(0.0), SourceTimestamp = DeadBandValue };
        }
        #endregion

        #region Methods
        void InitializeValues()
        {
            values = new DataValue[Math.Max(Settings.HDataCount, 1)];
            for (int i = 0; i < Settings.HDataCount; i++)
            {
                values[i] = new DataValue()
                {
                    Value = Double.NaN,
                    SourceTimestamp = DeadBandValue
                };
            }
        }        
        
        List<DataValue> CreateDataSource(string nodeid, DateTime startTime, DateTime endTime)
        {
            if (string.IsNullOrEmpty(nodeid))
                return null;

            CreateConnectionStringDataLayer();
            if (ufw == null)
                return new List<DataValue>();

            var utcStart = startTime.ToUniversalTime();
            var utcEnd = endTime.ToUniversalTime();

            var info = HistorianHelper.GetAuditDataLogInfo(nodeid, ufw);
            if (info == null)
                return new List<DataValue>();

            if (utcStart == DateTime.MinValue && utcEnd == DateTime.MaxValue)
            {
                var ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                            where entry.DataLogRef == info.Oid && 
                            entry.RecordDateTimeUtc != null
                            orderby entry.RecordDateTimeUtc descending
                            select new DataValue()
                            {
                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                            }).Take(Settings.HDataCount).ToList();
                return ret;
            }
            else if (utcStart == utcEnd)
            {
                var ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                            where entry.DataLogRef == info.Oid && 
                            entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc <= utcEnd
                            orderby entry.RecordDateTimeUtc descending
                            select new DataValue()
                            {
                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                            }).Take(Settings.HDataCount).ToList();
                return ret;
            }
            else
            {
                var ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                            where entry.DataLogRef == info.Oid && 
                            entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                            orderby entry.RecordDateTimeUtc descending
                            select new DataValue()
                            {
                                Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                                SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                            }).Take(Settings.HDataCount).ToList();
                return ret;
            }
        }        
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            base.Initialize();

            InitializeValues();
        }

        protected override void SetDataSources()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime;

            var templ = new List<DataValue>();
            if (values == null)
                InitializeValues();
            templ.AddRange(values);

            var task1 = Task.Factory.StartNew(() =>
            {
                string nodeID = absolutenodeid;

                var ret = CreateDataSource(nodeID, startTime, endTime);
                if (ret == null || ret.Count == 0)
                    return null;

                int dcount;
                int hcount;
                dcount = templ.Count();
                hcount = ret.Count();
                var index = 0;
                if (hcount >= dcount)
                    index = hcount - dcount;

                for (int i = 0; i < dcount; i++)
                {
                    if (i >= hcount)
                        break;

                    double _value = 0d;
                    templ[i] = new DataValue()
                    {
                        SourceTimestamp = DeadBandValue
                    };
                    if (ret[hcount - 1 - i].Value != null)
                    {
                        try { _value = System.Convert.ToDouble(ret[hcount - 1 - i].Value, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                        templ[i].Value = _value;
                    }
                }

                return templ;
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
                    values = ret.Result.ToArray();
                    OnHistoryLoaded();
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (values != null)
                Array.Clear(values, 0, values.Length);
        }
        #endregion
    }
}
