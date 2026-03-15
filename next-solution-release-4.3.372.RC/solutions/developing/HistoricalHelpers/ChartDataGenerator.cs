using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Opc.Ua;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using System.Threading;
using WPFUtilities.HistoricalHelpers;
using System.Threading.Tasks;

namespace WPFPenHelpers
{
    public class ChartDataGenerator : BaseDataGenerator
    {
        #region Constructors
        public ChartDataGenerator(string penId, DataGeneratorSettings settings, int commandTimeout)
            : base(penId, settings, commandTimeout)
        { }

        #endregion

        #region Public Properties
        private MyDataValue[] values;
        public ReadOnlyList<MyDataValue> Values
        {
            get
            {
                if (values == null)
                    return null;
                return new ReadOnlyList<MyDataValue>(values);
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
                values[index] = LastValue = new MyDataValue()
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
                values[i] = new MyDataValue() { Value = (Double)(0.0), SourceTimestamp = DeadBandValue };
        }
        #endregion

        #region Methods
        void InitializeValues(CancellationToken ct)
        {
            values = new MyDataValue[Math.Max(Settings.HDataCount, 1)];
            for (int i = 0; i < Settings.HDataCount; i++)
            {
                if (ct.IsCancellationRequested)
                    return;
                values[i] = new MyDataValue()
                {
                    Value = Double.NaN,
                    SourceTimestamp = DeadBandValue
                };
            }
        }

        List<DataValue> CreateDataSource(string nodeid, DateTime startTime, DateTime endTime, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(nodeid))
                return null;

            CreateConnectionStringDataLayer();
            if (ufw == null)
                return new List<DataValue>();

            var utcStart = startTime.ToUniversalTime();
            var utcEnd = endTime.ToUniversalTime();

            var info = HistorianHelper.GetAuditDataLogInfo(nodeid, ufw);
            if (ct.IsCancellationRequested || info == null)
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

            InitializeValues(CancellationToken.None);
        }

        public void CallSetDataSources(CancellationToken ct)
        {
            SetDataSources(ct);
        }

        protected override void SetDataSources(CancellationToken ct)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime;

            var templ = new List<MyDataValue>();
            if (values == null)
                InitializeValues(ct);
            if (ct.IsCancellationRequested)
                return;
            templ.AddRange(values);
#if !NET_STANDARD
            var task1 = Task.Factory.StartNew(() =>
            {
                string nodeID = absolutenodeid;
#else
                string nodeID = PenId;
#endif

                var ret = CreateDataSource(nodeID, startTime, endTime, ct);
                if (ret == null || ret.Count == 0)
#if !NET_STANDARD
                    return null;
#else
                    return;
#endif

                int dcount;
                int hcount;
                dcount = templ.Count();
                hcount = ret.Count();
                var index = 0;
                if (hcount >= dcount)
                    index = hcount - dcount;

                for (int i = 0; i < dcount; i++)
                {
                    if (ct.IsCancellationRequested)
#if !NET_STANDARD
                        return null;
#else
                    return;
#endif
                    if (i >= hcount)
                        break;

                    double _value = 0d;
                    templ[i] = new MyDataValue()
                    {
                        SourceTimestamp = DeadBandValue
                    };
                    if (ret[hcount - 1 - i].Value != null)
                    {
                        try { _value = System.Convert.ToDouble(ret[hcount - 1 - i].Value, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                        templ[i].Value = _value;
                    }
                }
#if !NET_STANDARD
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
#else
                values = templ.ToArray();
#endif
#if !NET_STANDARD
                    OnHistoryLoaded();
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
#endif
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
