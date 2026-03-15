//-------------------------------------------------------------------------------------------------
// <copyright file="Counter.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

using Syncfusion.Collections.BinaryTree;

namespace Syncfusion.Grouping.Internals
{
    class CounterKind
    {
        public const int CountAll = 0xffff;
        public const int DisplayElementCount = 0x8000;
        public const int YAmountCount = 0x0001;
        public const int FilteredRecordsCount = 0x0002;
        public const int ElementsCount = 0x0004;
        public const int RecordsCount = 0x0008;
        public const int CustomCount = 0x0010;
        public const int VisibleCustomCount = 0x0020;
        public static bool IsVisibleCounter(int kind) 
        { 
            return kind == DisplayElementCount || kind == YAmountCount || kind == VisibleCustomCount; 
        }
    }

    interface ICounterFactory
    {
        TreeTableVisibleCounter Empty { get; }
        TreeTableVisibleCounter CreateCounter();
        TreeTableVisibleCounter CreateCounter(int count);
        TreeTableVisibleCounter CreateCounter(double count, int kind);
        TreeTableVisibleCounter CreateCounter(int visibleCount, double yAmountCount, int filteredRecordCount, int elementCount, int recordCount, double customCount, double visibleCustomCount);
        TreeTableVisibleCounter CreateDisplayElementCounter(int count);
        TreeTableVisibleCounter CreateFilteredRecordCounter(int count);
        TreeTableVisibleCounter CreateElementCounter(int count);
        TreeTableVisibleCounter CreateRecordCounter(int count);
        TreeTableVisibleCounter CreateYAmountCounter(double count);
        TreeTableVisibleCounter CreateCustomCounter(double count);
        TreeTableVisibleCounter CreateVisibleCustomCounter(double count);
    }

    class DefaultCounterFactory : ICounterFactory
    {
        static readonly TreeTableVisibleCounter _Empty = new Counter(0, 0, 0, 0, 0, 0, 0);

        public TreeTableVisibleCounter Empty
        {
            get
            {
                return _Empty;
            }
        }

        public TreeTableVisibleCounter CreateCounter()
        {
            return new Counter(0);
        }

        public TreeTableVisibleCounter CreateCounter(int count)
        {
            return new Counter(count);
        }

        public TreeTableVisibleCounter CreateCounter(double count, int kind)
        {
            return new CounterWithKind(count, kind);
        }

        public TreeTableVisibleCounter CreateCounter(int visibleCount, double yAmountCount, int filteredRecordCount, int elementCount, int recordCount, double customCount, double visibleCustomCount)
        {
            return new Counter(visibleCount, yAmountCount, filteredRecordCount, elementCount, recordCount, customCount, visibleCustomCount);
        }

        public TreeTableVisibleCounter CreateDisplayElementCounter(int count)
        {
            return CreateCounter(count, CounterKind.DisplayElementCount);
        }

        public TreeTableVisibleCounter CreateFilteredRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.FilteredRecordsCount);
        }

        public TreeTableVisibleCounter CreateElementCounter(int count)
        {
            return CreateCounter(count, CounterKind.ElementsCount);
        }

        public TreeTableVisibleCounter CreateRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.RecordsCount);
        }

        public TreeTableVisibleCounter CreateYAmountCounter(double count)
        {
            return CreateCounter(count, CounterKind.YAmountCount);
        }

        public TreeTableVisibleCounter CreateCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.CustomCount);
        }

        public TreeTableVisibleCounter CreateVisibleCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.VisibleCustomCount);
        }
    }

    class YAmountCounterFactory : ICounterFactory
    {
        static readonly TreeTableVisibleCounter _Empty = new YAmountCounter(0, 0, 0);

        public TreeTableVisibleCounter Empty
        {
            get
            {
                return _Empty;
            }
        }

        public TreeTableVisibleCounter CreateCounter()
        {
            return new YAmountCounter(0);
        }

        public TreeTableVisibleCounter CreateCounter(int count)
        {
            return new YAmountCounter(count);
        }

        public TreeTableVisibleCounter CreateCounter(int visibleCount, double yAmountCount, int filteredRecordCount, int elementCount, int recordCount, double customCount, double visibleCustomCount)
        {
            return new YAmountCounter(visibleCount, yAmountCount, filteredRecordCount);
        }

        public TreeTableVisibleCounter CreateDisplayElementCounter(int count)
        {
            return new YAmountCounter(count);
        }

        public TreeTableVisibleCounter CreateCounter(double count, int kind)
        {
            return new YAmountCounterWithKind(count, kind);
        }

        public TreeTableVisibleCounter CreateFilteredRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.FilteredRecordsCount);
        }

        public TreeTableVisibleCounter CreateElementCounter(int count)
        {
            return CreateCounter(count, CounterKind.ElementsCount);
        }

        public TreeTableVisibleCounter CreateRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.RecordsCount);
        }

        public TreeTableVisibleCounter CreateYAmountCounter(double count)
        {
            return CreateCounter(count, CounterKind.YAmountCount);
        }

        public TreeTableVisibleCounter CreateCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.CustomCount);
        }

        public TreeTableVisibleCounter CreateVisibleCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.VisibleCustomCount);
        }
    }

    class FilteredRecordCounterFactory : ICounterFactory
    {
        static readonly TreeTableVisibleCounter _Empty = new FilteredRecordCounter(0, 0);

        public TreeTableVisibleCounter Empty
        {
            get
            {
                return _Empty;
            }
        }

        public TreeTableVisibleCounter CreateCounter()
        {
            return new FilteredRecordCounter(0);
        }

        public TreeTableVisibleCounter CreateCounter(int count)
        {
            return new FilteredRecordCounter(count);
        }

        public TreeTableVisibleCounter CreateCounter(int visibleCount, double yAmountCount, int filteredRecordCount, int elementCount, int recordCount, double customCount, double visibleCustomCount)
        {
            return new FilteredRecordCounter(visibleCount, filteredRecordCount);
        }

        public TreeTableVisibleCounter CreateDisplayElementCounter(int count)
        {
            return new FilteredRecordCounter(count);
        }

        public TreeTableVisibleCounter CreateCounter(double count, int kind)
        {
            return new FilteredRecordCounterWithKind(count, kind);
        }

        public TreeTableVisibleCounter CreateFilteredRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.FilteredRecordsCount);
        }

        public TreeTableVisibleCounter CreateElementCounter(int count)
        {
            return CreateCounter(count, CounterKind.ElementsCount);
        }

        public TreeTableVisibleCounter CreateRecordCounter(int count)
        {
            return CreateCounter(count, CounterKind.RecordsCount);
        }

        public TreeTableVisibleCounter CreateYAmountCounter(double count)
        {
            return CreateCounter(count, CounterKind.YAmountCount);
        }

        public TreeTableVisibleCounter CreateCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.CustomCount);
        }

        public TreeTableVisibleCounter CreateVisibleCustomCounter(double count)
        {
            return CreateCounter(count, CounterKind.VisibleCustomCount);
        }
    }

    class FilteredRecordCounter : TreeTableVisibleCounter
    {
        internal int filteredRecordCount;

        /// <summary>
        /// Gets an empty TreeTableVisibleCounter that represents 0 visible elements.
        /// </summary>
        public static new readonly FilteredRecordCounter Empty = new FilteredRecordCounter(0, 0);

        /// <override/>
        public override TreeTableVisibleCounter CreateCounter()
        {
            return new FilteredRecordCounter(0, 0);
        }

        public FilteredRecordCounter(int count)
            : base(count)
        {
        }

        ////public new static readonly Counter Empty = new Counter(0, 0, 0, 0, 0, 0, 0);
        public FilteredRecordCounter(int visibleCount, int filteredRecordCount)
            : base(visibleCount)
        {
            this.filteredRecordCount = filteredRecordCount;
        }

        public FilteredRecordCounter(double count, int kind)
            : base(count)
        {
            if ((kind & CounterKind.FilteredRecordsCount) != 0 || (kind & CounterKind.RecordsCount) != 0)
            {
                filteredRecordCount = (int)count;
            }
        }

        /// <override/>
        public override double GetValue(int kind)
        {
            if ((kind & CounterKind.FilteredRecordsCount) != 0 || (kind & CounterKind.RecordsCount) != 0)
            {
                return FilteredRecordCount;
            }

            return base.GetValue(kind);
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return CounterKind.DisplayElementCount;
            }
        }

        public virtual double YAmountCount
        {
            get
            {
                return 0;
            }
        }

        public int FilteredRecordCount
        {
            get
            {
                return filteredRecordCount;
            }
        }

        public int VisibleCount
        {
            get
            {
                return (int)GetVisibleCount();
            }
        }

        public virtual double CustomCount
        {
            get
            {
                return 0;
            }
        }

        public virtual double VisibleCustomCount
        {
            get
            {
                return 0;
            }
        }

        public virtual int ElementCount
        {
            get
            {
                return (int)GetVisibleCount();
            }
        }

        public virtual int RecordCount
        {
            get
            {
                return filteredRecordCount;
            }
        }

        /// <override/>
        protected override void OnCombineCounters(ITreeTableCounter x, ITreeTableCounter y, int kind)
        {
            FilteredRecordCounter gx = (FilteredRecordCounter)x;
            FilteredRecordCounter gy = (FilteredRecordCounter)y;
            filteredRecordCount = gx.FilteredRecordCount + gy.FilteredRecordCount;
            base.OnCombineCounters(x, y, kind);
        }

        /// <override/>
        public override double Compare(TreeTableVisibleCounter other, int kind)
        {
            FilteredRecordCounter x = (FilteredRecordCounter)other;
            double i = 0;

            if ((kind & CounterKind.FilteredRecordsCount) != 0 || (kind & CounterKind.RecordsCount) != 0)
            {
                i = FilteredRecordCount - x.FilteredRecordCount;
                if (i != 0)
                {
                    return i;
                }
            }

            if ((kind & CounterKind.DisplayElementCount) != 0 || (kind & CounterKind.ElementsCount) != 0)
            {
                return GetVisibleCount() - other.GetVisibleCount();
            }

            return 0;
        }

        /// <override/>
        public override bool IsEmpty(int kind)
        {
            return Compare(Empty, kind) == 0;
        }

        /// <override/>
        public override string ToString()
        {
            return base.ToString()
                + " FilteredRecordCount = " + filteredRecordCount.ToString();
        }
    }

    class FilteredRecordCounterWithKind : FilteredRecordCounter
    {
        int _kind;

        public FilteredRecordCounterWithKind(double count, int kind)
            : base(count, kind)
        {
            this._kind = kind;
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return _kind;
            }
        }
    }

    class YAmountCounter : FilteredRecordCounter
    {
        internal double yAmountCount;

        /// <summary>
        /// Gets an empty TreeTableVisibleCounter that represents 0 visible elements.
        /// </summary>
        public static new readonly YAmountCounter Empty = new YAmountCounter(0, 0, 0);

        /// <override/>
        public override TreeTableVisibleCounter CreateCounter()
        {
            return new YAmountCounter(0, 0, 0);
        }

        public YAmountCounter(int count)
            : base(count)
        {
        }

        ////public new static readonly Counter Empty = new Counter(0, 0, 0, 0, 0, 0, 0);
        public YAmountCounter(int visibleCount, double yAmountCount, int filteredRecordCount)
            : base(visibleCount)
        {
            this.yAmountCount = yAmountCount;
            this.filteredRecordCount = filteredRecordCount;
        }

        public YAmountCounter(double count, int kind)
            : base(count, kind)
        {
            if ((kind & CounterKind.YAmountCount) != 0)
            {
                yAmountCount = count;
            }
        }

        /// <override/>
        public override double GetValue(int kind)
        {
            if ((kind & CounterKind.YAmountCount) != 0)
            {
                return YAmountCount;
            }

            return base.GetValue(kind);
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return CounterKind.DisplayElementCount;
            }
        }

        public override double YAmountCount
        {
            get
            {
                return yAmountCount;
            }
        }

        /// <override/>
        protected override void OnCombineCounters(ITreeTableCounter x, ITreeTableCounter y, int kind)
        {
            YAmountCounter gx = (YAmountCounter)x;
            YAmountCounter gy = (YAmountCounter)y;
            yAmountCount = gx.YAmountCount + gy.YAmountCount;
            base.OnCombineCounters(x, y, kind);
        }

        /// <override/>
        public override double Compare(TreeTableVisibleCounter other, int kind)
        {
            YAmountCounter x = (YAmountCounter)other;
            double i = 0;
            if ((kind & CounterKind.YAmountCount) != 0)
            {
                i = YAmountCount - x.YAmountCount;
                if (i != 0)
                {
                    return i;
                }
            }

            return base.Compare(other, kind);
        }

        /// <override/>
        public override string ToString()
        {
            return base.ToString()
                + " YAmountCount = " + yAmountCount.ToString();
        }

        /// <override/>
        public override bool IsEmpty(int kind)
        {
            return Compare(Empty, kind) == 0;
        }
    }

    class YAmountCounterWithKind : YAmountCounter
    {
        int _kind;

        public YAmountCounterWithKind(double count, int kind)
            : base(count, kind)
        {
            this._kind = kind;
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return _kind;
            }
        }
    }

    class CounterWithKind : Counter
    {
        int _kind;

        public CounterWithKind(double count, int kind)
            : base(count, kind)
        {
            this._kind = kind;
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return _kind;
            }
        }
    }
        
    class Counter : YAmountCounter
    {
        internal int elementCount;
        internal int recordCount;
        internal double customCount;
        internal double visibleCustomCount;

        /// <summary>
        /// Gets an empty TreeTableVisibleCounter that represents 0 visible elements.
        /// </summary>
        public static new readonly Counter Empty = new Counter(0, 0, 0, 0, 0, 0, 0);

        public Counter(int count)
            : base(count)
        {
        }

        ////public new static readonly Counter Empty = new Counter(0, 0, 0, 0, 0, 0, 0);
        public Counter(int visibleCount, double yAmountCount, int filteredRecordCount, int elementCount, int recordCount, double customCount, double visibleCustomCount)
            : base(visibleCount)
        {
            this.yAmountCount = yAmountCount;
            this.filteredRecordCount = filteredRecordCount;
            this.elementCount = elementCount;
            this.recordCount = recordCount;
            this.customCount = customCount;
            this.visibleCustomCount = visibleCustomCount;
        }

        /// <override/>
        public override TreeTableVisibleCounter CreateCounter()
        {
            return new Counter(0, 0, 0, 0, 0, 0, 0);
        }

        /// <override/>
        public override int Kind
        {
            get
            {
                return CounterKind.DisplayElementCount;  // TODO: Could also be CountAll
            }
        }

        public Counter(double count, int kind)
            : base(count, kind)
        {
            Counter x = this;

            if ((kind & CounterKind.RecordsCount) != 0)
            {
                x.recordCount = (int)count;
            }

            if ((kind & CounterKind.ElementsCount) != 0)
            {
                x.elementCount = (int)count;
            }

            if ((kind & CounterKind.CustomCount) != 0)
            {
                x.customCount = count;
            }

            if ((kind & CounterKind.VisibleCustomCount) != 0)
            {
                x.visibleCustomCount = count;
            }
        }

        /// <override/>
        public override double GetValue(int kind)
        {
            if ((kind & CounterKind.YAmountCount) != 0)
            {
                return YAmountCount;
            }

            if ((kind & CounterKind.FilteredRecordsCount) != 0)
            {
                return FilteredRecordCount;
            }

            if ((kind & CounterKind.RecordsCount) != 0)
            {
                return RecordCount;
            }

            if ((kind & CounterKind.ElementsCount) != 0)
            {
                return ElementCount;
            }

            if ((kind & CounterKind.CustomCount) != 0)
            {
                return CustomCount;
            }

            if ((kind & CounterKind.VisibleCustomCount) != 0)
            {
                return VisibleCustomCount;
            }

            return base.GetValue(kind);
        }

        public override double CustomCount
        {
            get
            {
                return customCount;
            }
        }

        public override double VisibleCustomCount
        {
            get
            {
                return visibleCustomCount;
            }
        }

        public override int ElementCount
        {
            get
            {
                return elementCount;
            }
        }

        public override int RecordCount
        {
            get
            {
                return recordCount;
            }
        }

        /// <override/>
        protected override void OnCombineCounters(ITreeTableCounter x, ITreeTableCounter y, int kind)
        {
            Counter gx = (Counter)x;
            Counter gy = (Counter)y;
            recordCount = gx.RecordCount + gy.RecordCount;
            elementCount = gx.ElementCount + gy.ElementCount;
            customCount = gx.CustomCount + gy.CustomCount;
            visibleCustomCount = gx.VisibleCustomCount + gy.VisibleCustomCount;
            base.OnCombineCounters(x, y, kind);
        }

        /// <override/>
        public override double Compare(TreeTableVisibleCounter other, int kind)
        {
            Counter x = (Counter)other;
            double i = 0;

            if ((kind & CounterKind.RecordsCount) != 0)
            {
                i = RecordCount - x.RecordCount;
                if (i != 0)
                {
                    return i;
                }
            }

            if ((kind & CounterKind.ElementsCount) != 0)
            {
                i = ElementCount - x.ElementCount;
                if (i != 0)
                {
                    return i;
                }
            }

            if ((kind & CounterKind.CustomCount) != 0)
            {
                i = CustomCount - x.CustomCount;
                if (i != 0)
                {
                    return i;
                }
            }

            if ((kind & CounterKind.VisibleCustomCount) != 0)
            {
                i = VisibleCustomCount - x.VisibleCustomCount;
                if (i != 0)
                {
                    return i;
                }
            }

            return base.Compare(other, kind);
        }              
        
        /// <override/>
        public override bool IsEmpty(int kind)
        {         
            return Compare(Empty, kind) == 0;
        }
 
        /// <override/>
        public override string ToString()
        {
            return base.ToString()
                + " ElementCount = " + elementCount.ToString()
                + " RecordCount = " + recordCount.ToString()
                + " CustomCount = " + customCount.ToString()
                + " VisibleCustomCount = " + visibleCustomCount.ToString();
        }
    }
}
