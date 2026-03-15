using DevExpress.Xpo;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UFUAHistorianModel;
using UFUAHistorianModel.Helpers;
using WPFUtilities.HistoricalHelpers;
#if !NET_STANDARD
using System.Threading.Tasks;
#endif

namespace WPFPenHelpers
{
    public class ChartXYDataGenerator : BaseDataGenerator
    {
        #region Declarations
        string absolutenodeid1;
        string absolutenodeid2;
        #endregion

        #region Constructors
        public ChartXYDataGenerator(string XTagName, string YTagName, DataGeneratorSettings settings, int commandTimeout)
            : base(XTagName, settings, commandTimeout)
        {
            absolutenodeid1 = XTagName;
            absolutenodeid2 = YTagName;
        }

        public ChartXYDataGenerator(string penId, DataGeneratorSettings settings, int commandTimeout)
            : base(penId, settings, commandTimeout)
        { }
        #endregion

        #region Public Properties
        string nodeID1;
        public String NodeID1
        {
            get
            {
                return nodeID1;
            }
        }

        string nodeID2;
        public String NodeID2
        {
            get
            {
                return nodeID2;
            }
        }

        XYDataValue[] values;
        public ReadOnlyList<XYDataValue> Values
        {
            get
            {
                if (values == null)
                    return null;
                return new ReadOnlyList<XYDataValue>(values);
            }
        }

        XYDataValue lastValue;
        public new XYDataValue LastValue
        {
            get
            {
                return lastValue;
            }
        }
        #endregion

        #region Public Methods
        public void AddData(object newValue1, object newValue2, int _count)
        {
            if (values == null)
                return;

            var index = _count >= 0 && _count < values.Count() ? _count : 0;

            if (newValue1 != null)
            {
                var newValue = 0.0;
                try { newValue = System.Convert.ToDouble(newValue1, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                values[index].Value1 = LastValue.Value1 = new MyDataValue()
                {
                    Value = newValue,
                    SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now
                };
            }

            if (newValue2 != null)
            {
                var newValue = 0.0;
                try { newValue = System.Convert.ToDouble(newValue2, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                values[index].Value2 = LastValue.Value2 = new MyDataValue()
                {
                    Value = newValue,
                    SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now
                };
            }
        }

        public enum ResetType
        {
            XY,
            X,
            Y
        }

        public void ResetData(ResetType resetType)
        {
            if (values == null)
                return;

            var _data1 = new MyDataValue() { Value = lastValue.Value1.Value, SourceTimestamp = DateTime.MaxValue };
            var _data2 = new MyDataValue() { Value = lastValue.Value2.Value, SourceTimestamp = DateTime.MaxValue };

            for (int i = 0; i < values.Length; i++)
            {
                if (resetType == ResetType.X || resetType == ResetType.XY)
                    values[i].Value1 = _data1;
                if (resetType == ResetType.Y || resetType == ResetType.XY)
                    values[i].Value2 = _data2;
            }
        }

        public void UpdateReferences(NodeIdViewModel nodeIdViewModel, PenTypeEnum penType)
        {
            var nodeid = nodeIdViewModel.nodeId.ToString();
            switch (penType)
            {
                case PenTypeEnum.X:
                    absolutenodeid1 = nodeid;
                    break;
                case PenTypeEnum.Y:
                    absolutenodeid2 = nodeid;
                    break;
                case PenTypeEnum.XY:
                    absolutenodeid1 = absolutenodeid2 = nodeid;
                    break;
                default:
                    return;
            }

            if (string.IsNullOrEmpty(absolutenodeid1) || string.IsNullOrEmpty(absolutenodeid2))
                return;

            LoadBuffer();
        }
        #endregion

        #region Methods
        void InitializeValues()
        {
            DateTime starttime = DateTime.Now;
            values = new XYDataValue[Settings.HDataCount];
            for (int i = 0; i < Settings.HDataCount; i++)
            {
                var sourceTimestamp = starttime.AddDays(i);
                values[i] = new XYDataValue()
                {
                    Value1 = new MyDataValue()
                    {
                        Value = double.NaN,
                        SourceTimestamp = sourceTimestamp
                    },
                    Value2 = new MyDataValue()
                    {
                        Value = double.NaN,
                        SourceTimestamp = sourceTimestamp
                    }
                };
            }
        }

        List<MyDataValue> CreateDataSource(string nodeid, DateTime date)
        {
            if (string.IsNullOrEmpty(nodeid))
                return null;
            CreateConnectionStringDataLayer();
            if (ufw == null)
                return new List<MyDataValue>();

            var info = HistorianHelper.GetAuditDataLogInfo(nodeid, ufw);
            if (info == null)
                return new List<MyDataValue>();

            var ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                       where entry.DataLogRef == info.Oid &&
                       entry.RecordDateTimeUtc != null && entry.RecordDateTimeUtc <= date
                       orderby entry.RecordDateTimeUtc descending
                       select new MyDataValue()
                       {
                           Value = entry.dValue.HasValue ? entry.dValue.Value : (double?)null,
                           SourceTimestamp = entry.RecordDateTimeUtc.ToLocalTime()
                       }).Take(Settings.HDataCount).ToList();
            return ret;
        }
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            base.Initialize();

            lastValue = new XYDataValue()
            {
                Value1 = new MyDataValue() { Value = (double)0.0, SourceTimestamp = DateTime.MaxValue },
                Value2 = new MyDataValue() { Value = (double)0.0, SourceTimestamp = DateTime.MaxValue }
            };

            InitializeValues();
        }
        public void CallSetDataSources(CancellationToken ct)
        {
            SetDataSources(ct);
        }

        protected override void SetDataSources(CancellationToken ct)
        {
            var templ = new List<XYDataValue>();
            if (values != null)
                templ.AddRange(values);

#if !NET_STANDARD
            var task1 = Task.Factory.StartNew(() =>
            {
#endif
            nodeID1 = absolutenodeid1;
                nodeID2 = absolutenodeid2;
                if (string.IsNullOrEmpty(absolutenodeid1) || string.IsNullOrEmpty(absolutenodeid2))
#if !NET_STANDARD
                    return null;
#else
                    return;
#endif

                DateTime date = DateTime.UtcNow;
                var xValues = CreateDataSource(nodeID1, date);
                var yValues = CreateDataSource(nodeID2, date);


                if (xValues == null || xValues.Count == 0 ||
                    yValues == null || yValues.Count == 0)
#if !NET_STANDARD
                    return null;
#else
                    return;
#endif

                bool bFirstIsX = xValues.Count >= yValues.Count;
                var DataValueCollection1 = new List<MyDataValue>();
                var DataValueCollection2 = new List<MyDataValue>();
                if (bFirstIsX)
                {
                    DataValueCollection1.AddRange(xValues);
                    DataValueCollection2.AddRange(yValues);
                }
                else
                {
                    DataValueCollection2.AddRange(xValues);
                    DataValueCollection1.AddRange(yValues);
                }

                var xypointCollection = new List<XYPoint>();

                var listRemaining = (from c in DataValueCollection2
                                     where c.SourceTimestamp > DataValueCollection1.First().SourceTimestamp
                                     orderby c.SourceTimestamp descending
                                     select c).ToList();
                foreach (var latValue in listRemaining)
                {
                    object lat = 0, lon = 0;
                    if (bFirstIsX)
                    {
                        lon = latValue.Value;
                        lat = DataValueCollection1.First().Value;
                    }
                    else
                    {
                        lat = latValue.Value;
                        lon = DataValueCollection1.First().Value;
                    }
                    var xypoint = new XYPoint(lat, lon, latValue.SourceTimestamp);
                    xypointCollection.Add(xypoint);
                }

                foreach (var latValue in DataValueCollection1)
                {
                    var lotValueList = (from c in DataValueCollection2
                                        where c.SourceTimestamp.ToString() == latValue.SourceTimestamp.ToString()
                                        select c).ToList();

                    if (lotValueList.Count <= 0)
                    {
                        lotValueList = (from c in DataValueCollection2
                                        where c.SourceTimestamp < latValue.SourceTimestamp
                                        orderby c.SourceTimestamp descending
                                        select c).ToList();
                        if (lotValueList.Count <= 0)
                        {
                            lotValueList.Add(DataValueCollection2[0]);
                        }
                    }

                    object lat = 0, lon = 0;
                    if (bFirstIsX)
                    {
                        lat = latValue.Value;
                        lon = lotValueList[0].Value;
                    }
                    else
                    {
                        lon = latValue.Value;
                        lat = lotValueList[0].Value;
                    }
                    var xypoint = new XYPoint(lat, lon, latValue.SourceTimestamp);
                    xypointCollection.Add(xypoint);
                }

                int dcount;
                var _sampnum = (from c in templ
                                where DateTime.Compare(c.Value1.SourceTimestamp, date) >= 0
                                select c).ToList().Count();

                dcount = templ.Count() > _sampnum ? templ.Count() - _sampnum : templ.Count();
                DateTime sDate;
                if (dcount == templ.Count())
                    sDate = date;
                else
                    sDate = DateTime.Compare(templ[dcount].Value1.SourceTimestamp, DeadBandValue) == 0 ? date : templ[dcount].Value1.SourceTimestamp;

                if (dcount != 0)
                {
                    for (int i = dcount - 1; i >= 0; i--)
                    {
                        var item = xypointCollection.LastOrDefault();

                        if (item != null)
                        {
                            templ[i].Value1.Value = item.Value1;
                            templ[i].Value2.Value = item.Value2;
                            templ[i].Value1.SourceTimestamp = item.Date;
                            templ[i].Value2.SourceTimestamp = item.Date;
                            xypointCollection.RemoveAt(xypointCollection.Count - 1);
                        }
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
#endif
                values = templ.ToArray();
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
