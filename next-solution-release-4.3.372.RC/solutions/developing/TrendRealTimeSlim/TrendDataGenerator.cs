using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Utilities;
using Utilities.Converters;
using WPFPenHelpers;
using WPFUtilities.HistoricalHelpers;

namespace TrendRealTimeSlim
{
    class TrendDataGenerator : BaseDataGenerator
    {
        #region Declarations
        public DataCollection collectionValues;
        bool statisticsInvalidated;
        IExpressionValueConverter expressionConverter;
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
        public TrendDataGenerator(string penId, DataGeneratorSettings settings, TimeSpan recordEvery, IExpressionValueConverter expressionConverter)
            : base(penId, settings)
        {
            this.expressionConverter = expressionConverter;
            collectionValues = new DataCollection(settings.HDataCount, settings.ClientTimezoneOffset, recordEvery);
        }
        #endregion
        #region Overrides
        protected override void SetDataSources(System.Threading.CancellationToken ct)
        {
           
        }
        #endregion
        #region Methods
        public void PlotData()
        {
            collectionValues.PlotData(this.expressionConverter);
        }
        public void UpdateUnitConverter(IExpressionValueConverter expressionConverter)
        {
            this.expressionConverter = expressionConverter;
            collectionValues.UpdateExpressionConverter(expressionConverter);
        }
        public void AddData(object newValue)
        {
            if (newValue == null)
                return;

            bool bHasValue = false;
            LastValue = new MyDataValue()
            {
                SourceTimestamp = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now,
                expressionValueConverter = this.expressionConverter
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

            collectionValues.AddData(bHasValue ? dValue : null, LastValue.SourceTimestamp, this.expressionConverter);

            statisticsInvalidated = true;
        }
        void UpdateStat()
        {
            try
            {
                var statDouble = (from c in HistoryData
                                    where c.dValue != null
                                    select Convert.ToDouble(c.dValue, System.Globalization.CultureInfo.InvariantCulture));
                if (statDouble.Count() > 0)
                {
                    maxValue = statDouble.Max();
                    minValue = statDouble.Min();
                    avgValue = statDouble.Average();

                    if (this.expressionConverter != null)
                    {
                        maxValue = GetConvertedDoubleValue(maxValue, this.expressionConverter);
                        minValue = GetConvertedDoubleValue(minValue, this.expressionConverter);
                        avgValue = GetConvertedDoubleValue(avgValue, this.expressionConverter);
                    }
                }
                else
                {
                    maxValue = 0;
                    minValue = 0;
                    avgValue = 0;
                }
            }
            catch
            {
                maxValue = 0;
                minValue = 0;
                avgValue = 0;
            }
        }
        private double GetConvertedDoubleValue(double value, IExpressionValueConverter converter)
        {
            var exprValue = converter?.Convert(value, typeof(double), null, System.Globalization.CultureInfo.InvariantCulture);

            if (exprValue == null)
                return value;

            return Convert.ToDouble(exprValue, System.Globalization.CultureInfo.InvariantCulture);
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
        public void AddRange(List<MyDataValue> items, IExpressionValueConverter expConverter)
        {
            if (items == null || items.Count == 0)
                return;

            foreach (MyDataValue item in items)
            {
                item.expressionValueConverter = expConverter;
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, Items.Count - items.Count));
        }
        public void PrependList(List<MyDataValue> items, IExpressionValueConverter expConverter, bool reverse = false)
        {
            if (items == null || items.Count == 0)
                return;
            if (reverse)
                items.Reverse();
            items.AddRange(Items);
            Items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            AddRange(items, expConverter);
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
        public void AddData(double? dValue, DateTime timestamp, IExpressionValueConverter expConverter)
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
                        if (timestamp > dataValuesBuffer.Last().SourceTimestamp)
                        {
                            timestamp = new DateTime(lastDataValue.SourceTimestamp.Ticks + recordEvery.Ticks, lastDataValue.SourceTimestamp.Kind);
                            dataValuesBuffer.Add(new MyDataValue(timestamp, dValue, expConverter));
                        }
                        else
                            lastDataValue.dValue = dValue;
                    }
                }
                else
                    dataValuesBuffer.Add(new MyDataValue(timestamp, dValue, expConverter));
            }
        }
        public void PlotData(IExpressionValueConverter expConverter)
        {
            List<MyDataValue> tempBuffer;
            lock (dataValuesBuffer)
            {
                tempBuffer = new List<MyDataValue>(dataValuesBuffer);
                dataValuesBuffer.Clear();
            }

            if (tempBuffer.Count > 0)
            {
                AddRange(tempBuffer, expConverter);
                lastPlottedTimestamp = tempBuffer.Last().SourceTimestamp;
            }
            else
            {
                var dv = new MyDataValue()
                {
                    SourceTimestamp = clientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + clientTimezoneOffset : DateTime.Now,
                    dValue = lastValue
                };
                dv.expressionValueConverter = expConverter;
                AddItem(dv);
                lastPlottedTimestamp = dv.SourceTimestamp;
            }

            if (Items.Count > maxItems)
                RemoveFromBegin(Items.Count - maxItems);
        }
        public void UpdateExpressionConverter(IExpressionValueConverter expConverter)
        {
            foreach (var item in Items)
            {
                item.expressionValueConverter = expConverter;
            }
        }
        #endregion
    }
}
