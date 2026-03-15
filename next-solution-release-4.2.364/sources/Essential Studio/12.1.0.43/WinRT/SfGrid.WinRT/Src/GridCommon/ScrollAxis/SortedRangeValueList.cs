#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.UI.Xaml.ScrollAxis
{

    /// <summary>
    /// Holds a range together with a value assigned to the range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ClassReference(IsReviewed = false)]
    public class RangeValuePair<T> : IComparable
    {
        private int start;
        private int count;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValuePair{T}"/> class.
        /// </summary>
        /// <param name="start">The start and end of the range.</param>
        public RangeValuePair(int start)
        {
            this.start = start;
            this.count = 1;
            this.value = default(T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValuePair{T}"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="value">The value.</param>
        public RangeValuePair(int start, int count, T value)
        {
            this.start = start;
            this.count = count;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the start of the range.
        /// </summary>
        /// <value>The start.</value>
        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// Gets or sets the count of the range.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the end of the range.
        /// </summary>
        /// <value>The end.</value>
        public int End
        {
            get { return start + count - 1; }
            set { count = value - start + 1; }
        }

        /// <summary>
        /// Compares the current range with the range of the other object. The value is 
        /// ignored.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        public int CompareTo(object obj)
        {
            RangeValuePair<T> x = this;
            var y = (RangeValuePair<T>)obj;

            if (x.start >= y.start && x.start < y.start + y.count
                || y.start >= x.start && y.start < x.start + x.count)
                return 0;

            return start.CompareTo(y.start);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> with state information about this object.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="System.String"/> with state information about this object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} ( Start = {1} Count = {2} End = {3} Value = {4} )",
                GetType().Name, start, count, End, value);
        }
    }

    /// <summary>
    /// A sorted list with <see cref="RangeValuePair{T}"/> ordered by the
    /// start index of the ranges. SortedRangeValueList ensures that ranges 
    /// of the elements inside the list do not overlap and it also ensures
    /// that there are no empty gaps meaning that the subsequent range will
    /// always have the Start position be set to the End position of the previous
    /// range plus one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ClassReference(IsReviewed = false)]
    public class SortedRangeValueList<T> : IEnumerable<RangeValuePair<T>>
    {
        List<RangeValuePair<T>> rangeValues = new List<RangeValuePair<T>>();
        T defaultValue = default(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedRangeValueList{T}"/> class.
        /// </summary>
        public SortedRangeValueList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedRangeValueList{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value used for filling gaps.</param>
        public SortedRangeValueList(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the default value used for filling gaps.
        /// </summary>
        /// <value>The default value.</value>
        public T DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Gets the count which is the same as the end position of
        /// the last range.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                if (rangeValues.Count == 0)
                    return 0;

                RangeValuePair<T> rv = rangeValues[rangeValues.Count - 1];
                return rv.Start + rv.Count;
            }
        }

        RangeValuePair<T> GetRangeValue(int index)
        {
            int n = rangeValues.BinarySearch(new RangeValuePair<T>(index));
            return rangeValues[n];
        }

        /// <summary>
        /// Gets the value of the range that contains the specified index
        /// or changes the value of the range. When necessary it splits a range and creates
        /// a new range value pair to hold the new value for the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                return GetRangeValue(index).Value;
            }
            set
            {
                bool b = false;
                if (index >= Count)
                {
                    if (value.Equals(defaultValue))
                        return;
                    b = true;
                }

                if (b || !value.Equals(this[index]))
                {
                    EnsureCount(index);
                    int n = Split(index);
                    Split(index + 1);
                    if (n == rangeValues.Count)
                    {
                        if (n > 0 && value.Equals(rangeValues[n - 1].Value))
                            rangeValues[n - 1].Count++;
                        else
                            rangeValues.Add(new RangeValuePair<T>(index, 1, value));
                    }
                    else
                        rangeValues[n].Value = value;
                }
            }
        }

        /// <summary>
        /// Gets a range that countains the specified index and also 
        /// returns a count indicating the delta between the index and the 
        /// end of the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public T GetRange(int index, out int count)
        {
            if (index >= Count)
            {
                count = int.MaxValue;
                return defaultValue;
            }

            RangeValuePair<T> rv = GetRangeValue(index);
            count = rv.End - index + 1;
            return rv.Value;
        }

        /// <summary>
        /// Inserts a range intialized with <see cref="DefaultValue"/> at
        /// the specified index. When necessary it splits a range and creates
        /// a new range value pair.
        /// </summary>
        /// <param name="insertAt">The insertion point.</param>
        /// <param name="count">The count.</param>
        public void Insert(int insertAt, int count)
        {
            Insert(insertAt, count, DefaultValue, null);
        }

        /// <summary>
        /// Inserts a range intialized with a given value at
        /// the specified index. When necessary it splits a range and creates
        /// a new range value pair.
        /// </summary>
        /// <param name="insertAt">The insertion point.</param>
        /// <param name="count">The count.</param>
        /// <param name="value">The value.</param>
        public void Insert(int insertAt, int count, T value)
        {
            Insert(insertAt, count, value, null);
        }

        /// <summary>
        /// Inserts a range intialized with <see cref="DefaultValue"/> at
        /// the specified index. When necessary it splits a range and creates
        /// a new range value pair.
        /// </summary>
        /// <param name="insertAt">The insertion point.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveRanges">Allocate this object before a preceeding Remove call when moving ranges. 
        /// Otherwise specify null.</param>
        public void Insert(int insertAt, int count, SortedRangeValueList<T> moveRanges)
        {
            Insert(insertAt, count, DefaultValue, moveRanges);
        }

        /// <summary>
        /// Inserts a range intialized with a given value at
        /// the specified index. When necessary it splits a range and creates
        /// a new range value pair.
        /// </summary>
        /// <param name="insertAt">The insertion point.</param>
        /// <param name="count">The count.</param>
        /// <param name="value">The value.</param>
        /// <param name="moveRanges">Allocate this object before a preceeding Remove call when moving ranges. 
        /// Otherwise specify null.</param>
        public void Insert(int insertAt, int count, T value, SortedRangeValueList<T> moveRanges)
        {
            if (insertAt >= Count)
            {
                if (value.Equals(defaultValue) && (moveRanges == null || moveRanges.Count == 0))
                    return;

                EnsureCount(insertAt);
                rangeValues.Add(new RangeValuePair<T>(insertAt, count, value));
                if (rangeValues.Count >= 2)
                    Merge(rangeValues.Count - 2);
            }
            else
            {
                int n = rangeValues.BinarySearch(new RangeValuePair<T>(insertAt));
                RangeValuePair<T> rv = rangeValues[n];
                if (value.Equals(rv.Value))
                {
                    rv.Count += count;
                    AdjustStart(n + 1, count);
                }
                else
                {
                    n = Split(insertAt, n);
                    Split(insertAt + 1);
                    var rv2 = new RangeValuePair<T>(insertAt, count, value);
                    rangeValues.Insert(n, rv2);
                    AdjustStart(n + 1, count);
                    Merge(n);
                    if (n > 0)
                        Merge(n - 1);
                }
            }

            if (moveRanges != null)
            {
                foreach (RangeValuePair<T> rv in moveRanges)
                    SetRange(rv.Start + insertAt, rv.Count, rv.Value);
            }
        }

        /// <summary>
        /// Removes a range at the specified index. When necessary ranges
        /// are merged when preceeding and subsquent ranges have the same
        /// value.
        /// </summary>
        /// <param name="removeAt">The index for the range to be removed.</param>
        /// <param name="count">The count.</param>
        public void Remove(int removeAt, int count)
        {
            Remove(removeAt, count, null);
        }

        /// <summary>
        /// Removes a range at the specified index. When necessary ranges
        /// are merged when preceeding and subsquent ranges have the same
        /// value.
        /// </summary>
        /// <param name="removeAt">The index for the range to be removed.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveRanges">Allocate this object before a Remove call when moving ranges
        /// and pass it to a subsequent Insert call. Otherwise specify null.</param>
        public void Remove(int removeAt, int count, SortedRangeValueList<T> moveRanges)
        {
            if (removeAt >= Count)
                return;

            int n = RemoveHelper(removeAt, count, moveRanges);
            AdjustStart(n, -count);
            if (n > 0)
                Merge(n - 1);
        }

        /// <summary>
        /// Sets the value for a range at the specified index. When necessary ranges
        /// are split or merged to make sure integrity of the list is maintained.
        /// (SortedRangeValueList ensures that ranges
        /// of the elements inside the list do not overlap and it also ensures
        /// that there are no empty gaps meaning that the subsequent range will
        /// always have the Start position be set to the End position of the previous
        /// range plus one.)
        /// </summary>
        /// <param name="index">The index for the range to be changed.</param>
        /// <param name="count">The count.</param>
        /// <param name="value">The value.</param>
        public void SetRange(int index, int count, T value)
        {
            if (index >= Count && value.Equals(defaultValue))
                return;

            EnsureCount(index);
            int n = RemoveHelper(index, count, null);
            var rv = new RangeValuePair<T>(index, count, value);
            rangeValues.Insert(n, rv);
            Merge(n);
            if (n > 0)
                Merge(n - 1);
        }

        private void EnsureCount(int index)
        {
            if (index - Count > 0)
                rangeValues.Add(new RangeValuePair<T>(Count, index - Count, defaultValue));
        }

        private void AdjustStart(int n, int delta)
        {
            while (n < rangeValues.Count)
                rangeValues[n++].Start += delta;
        }

        private int RemoveHelper(int removeAt, int count, SortedRangeValueList<T> moveRanges)
        {
            if (removeAt >= Count)
                return rangeValues.Count;
            int n = Split(removeAt);
            Split(removeAt + count);
            int total = 0;
            int deleteCount = 0;
            while (total < count && n + deleteCount < rangeValues.Count)
            {
                RangeValuePair<T> rv = rangeValues[n + deleteCount];
                total += rv.Count;
                deleteCount++;
                if (moveRanges != null && !rv.Value.Equals(defaultValue))
                    moveRanges.rangeValues.Add(new RangeValuePair<T>(rv.Start - removeAt, rv.Count, rv.Value));
            }
            rangeValues.RemoveRange(n, deleteCount);
            return n;
        }

        int Split(int index)
        {
            if (index >= Count)
                return rangeValues.Count;
            int n = rangeValues.BinarySearch(new RangeValuePair<T>(index));
            return Split(index, n);
        }

        int Split(int index, int n)
        {
            RangeValuePair<T> rv = rangeValues[n];
            if (rangeValues[n].Start == index)
                return n;

            int count1 = index - rangeValues[n].Start;
            int count2 = rangeValues[n].Count - count1;
            rv.Count = count1;

            var rv2 = new RangeValuePair<T>(index, count2, rv.Value);
            rangeValues.Insert(n + 1, rv2);
            return n + 1;
        }

        void Merge(int n)
        {
            if (n >= rangeValues.Count)
                return;
            RangeValuePair<T> rv1 = rangeValues[n];
            if (n == rangeValues.Count - 1)
            {
                if (rv1.Value.Equals(defaultValue))
                    rangeValues.RemoveAt(n);
                return;
            }
            RangeValuePair<T> rv2 = rangeValues[n + 1];
            if (rv1.Value.Equals(rv2.Value))
            {
                rv1.Count += rv2.Count;
                rangeValues.RemoveAt(n + 1);
            }
        }


        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return rangeValues.GetEnumerator();
        }

        #endregion

        #region IEnumerable<RangeValuePair<T>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<RangeValuePair<T>> IEnumerable<RangeValuePair<T>>.GetEnumerator()
        {
            return rangeValues.GetEnumerator();
        }

        #endregion

    }
}
