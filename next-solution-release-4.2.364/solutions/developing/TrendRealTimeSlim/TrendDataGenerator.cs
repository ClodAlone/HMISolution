using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Utilities;
using WPFPenHelpers;

namespace TrendRealTimeSlim
{
    class TrendDataGenerator : BaseDataGenerator
    {
        #region Declarations
        public DataCollection collectionValues;
        bool statisticsInvalidated;
        #endregion
        #region Public Properties
        double minValue;
        public double MinValue
        {
            get
            {
                if (statisticsInvalidated)
                {
                    statisticsInvalidated = false;
                    UpdateStat();
                }
                return minValue;
            }
        }

        double maxValue;
        public double MaxValue
        {
            get
            {
                if (statisticsInvalidated)
                {
                    statisticsInvalidated = false;
                    UpdateStat();
                }
                return maxValue;
            }
        }

        double avgValue;
        public double AvgValue
        {
            get
            {
                if (statisticsInvalidated)
                {
                    statisticsInvalidated = false;
                    UpdateStat();
                }
                return avgValue;
            }
        }
        public ReadOnlyList<MyDataValue> HistoryData
        {
            get
            {
                if (collectionValues == null)
                    return null;
                return new ReadOnlyList<MyDataValue>(collectionValues);
            }
        }
        #endregion
        #region Constructors
        public TrendDataGenerator(string penId, DataGeneratorSettings settings, TimeSpan recordEvery)
            : base(penId, settings)
        {
            collectionValues = new DataCollection(settings.HDataCount, settings.ClientTimezoneOffset, recordEvery);
        }
        #endregion
        #region Overrides
        protected override void SetDataSources()
        {
           
        }
        #endregion
        #region Methods
        public void PlotData()
        {
            collectionValues.PlotData();
        }
        public void AddData(object newValue)
        {
            if (newValue == null)
                return;

            bool bHasValue = false;
            LastValue = new DataValue()
            {
                SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now
            };
            double? dValue = 0.0;
            if (newValue is Boolean)
            {
                bHasValue = true;
                dValue = (bool)newValue ? 1.0 : 0.0;
            }
            else if (!(newValue is double) || (!double.IsNaN((double)newValue) && !double.IsInfinity((double)newValue)))
            {
                try
                {
                    dValue = Convert.ToDouble(newValue, System.Globalization.CultureInfo.InvariantCulture);
                    bHasValue = true;
                } catch { }
            }
            if (bHasValue)
                LastValue.Value = dValue;

            collectionValues.AddData(bHasValue ? dValue : null, LastValue.SourceTimestamp);

            statisticsInvalidated = true;
        }
        void UpdateStat()
        {
            try
            {
                var statDouble = (from c in HistoryData
                                    where c.dValue != null
                                    select Convert.ToDouble(c.dValue, System.Globalization.CultureInfo.InvariantCulture)).ToList();
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
        #endregion
        protected override void OnDispose()
        {
            base.OnDispose();
            collectionValues.Clear();
        }
    }

    public class DataCollection : SafeObservableCollection<MyDataValue>
    {
        #region Declarations
        int maxItems;
        List<MyDataValue> dataValuesBuffer = new List<MyDataValue>();
        DateTime lastPlottedTimestamp = DateTime.MinValue;
        TimeSpan recordEvery;
        TimeSpan clientTimezoneOffset;
        double? lastValue = 0.0;
        #endregion
        #region Ctor
        public DataCollection(int maxitems, TimeSpan clientTimezoneOffset, TimeSpan recordEvery)
        {
            maxItems = maxitems;
            this.recordEvery = recordEvery;
            this.clientTimezoneOffset = clientTimezoneOffset;
        }
        #endregion
        #region Methods
        public void AddItem(MyDataValue item)
        {
            Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public void AddRange(List<MyDataValue> items)
        {
            if (items == null || items.Count == 0)
                return;

            foreach (MyDataValue item in items)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, Items.Count - items.Count));
        }
        public void PrependList(List<MyDataValue> items, bool reverse = false)
        {
            if (items == null || items.Count == 0)
                return;
            if (reverse)
                items.Reverse();
            items.AddRange(Items);
            Items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            AddRange(items);
        }
        public void RemoveFromBegin(int count)
        {
            IList<MyDataValue> removedItems = new List<MyDataValue>(count);
            for (int i = 0; i < count; i++)
            {
                removedItems.Add(Items[0]);
                Items.RemoveAt(0);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)removedItems, 0));
        }
        public void AddData(double? dValue, DateTime timestamp)
        {
            lastValue = dValue;
            lock (dataValuesBuffer)
            {
                if (lastPlottedTimestamp != DateTime.MinValue)
                {
                    if (dataValuesBuffer.Count == 0)
                    {
                        var modTicks = (timestamp.Ticks - lastPlottedTimestamp.Ticks) % recordEvery.Ticks;
                        timestamp = new DateTime(timestamp.Ticks + recordEvery.Ticks - modTicks, timestamp.Kind);
                    }
                    else
                    {
                        var lastDataValue = dataValuesBuffer.Last();
                        if (timestamp > dataValuesBuffer.Last().SourceTimeStamp)
                        {
                            timestamp = new DateTime(lastDataValue.SourceTimeStamp.Ticks + recordEvery.Ticks, lastDataValue.SourceTimeStamp.Kind);
                            dataValuesBuffer.Add(new MyDataValue(timestamp, dValue));
                        }
                        else
                            lastDataValue.dValue = dValue;
                    }
                }
                else
                    dataValuesBuffer.Add(new MyDataValue(timestamp, dValue));
            }
        }
        public void PlotData()
        {
            List<MyDataValue> tempBuffer;
            lock (dataValuesBuffer)
            {
                tempBuffer = new List<MyDataValue>(dataValuesBuffer);
                dataValuesBuffer.Clear();
            }

            if (tempBuffer.Count > 0)
            {
                AddRange(tempBuffer);
                lastPlottedTimestamp = tempBuffer.Last().SourceTimeStamp;
            }
            else
            {
                var dv = new MyDataValue()
                {
                    SourceTimeStamp = clientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + clientTimezoneOffset : DateTime.Now,
                    dValue = lastValue
                };
                AddItem(dv);
                lastPlottedTimestamp = dv.SourceTimeStamp;
            }

            if (Items.Count > maxItems)
                RemoveFromBegin(Items.Count - maxItems);
        }
        #endregion
    }
}
