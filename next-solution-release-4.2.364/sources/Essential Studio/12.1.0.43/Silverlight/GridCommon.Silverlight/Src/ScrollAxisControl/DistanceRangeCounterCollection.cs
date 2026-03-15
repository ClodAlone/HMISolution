#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if !WinRT
using Syncfusion.Windows.Collections;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Syncfusion.WinRT.Collections;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// A collection of entities for which distances need to be counted. The
    /// collection provides methods for mapping from a distance position to
    /// an entity and vice versa.<para/>
    /// For example, in a scrollable grid control you have rows with different heights. 
    /// Use this collection to determine the total height for all rows in the grid,
    /// quickly detemine the row index for a given point and also quickly determine
    /// the point at which a row is displayed. This also allows a mapping between the 
    /// scrollbars value and the rows or columns associated with that value.
    /// </summary>
    /// <remarks>
    /// DistanceCounterCollection internally uses ranges for allocating
    /// objects up to the modified entry with the highest index. When you modify 
    /// the size of an entry the collection ensures that that objects are allocated 
    /// for all entries up to the given index. Entries that are after the modified 
    /// entry are assumed to have the DefaultSize and will not be allocated. 
    /// <para/>
    /// Ranges will only be allocated for those lines that have different sizes.
    /// If you do for example only change the size of line 100 to be 10 then the collection
    /// will internally create two ranges: Range 1 from 0-99 with DefaultSize and
    /// Range 2 from 100-100 with size 10. This approach makes this collection
    /// work very efficient with grid scenarios where often many rows have
    /// the same height.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class DistanceRangeCounterCollection : IDistanceCounterCollection, IDistancePadding
    {
        DistanceLineCounterTree rbTree;
        int count = 0;
        double defaultDistance = 1.0;
        private double paddingDistance = 0d;

        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        public static readonly DistanceRangeCounterCollection Empty = new DistanceRangeCounterCollection();

        public DistanceRangeCounterCollection()
            : this(0d)
        {
        }

        /// <summary>
        /// Constructs the class and initializes the internal tree.
        /// </summary>
        public DistanceRangeCounterCollection(double paddingDistance)
        {
            DistanceLineCounter startPos = new DistanceLineCounter(0, 0);
            rbTree = new DistanceLineCounterTree(startPos, false);
            this.paddingDistance = paddingDistance;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            rbTree.Clear();
        }

        /// <summary>
        /// The raw number of entities (lines, rows or columns).
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        /// <summary>
        /// The default distance (row height or column width) an entity spans 
        /// </summary>
        public double DefaultDistance
        {
            get
            {
                return defaultDistance;
            }
            set
            {
                defaultDistance = value;
            }
        }

        public double PaddingDistance
        {
            get { return this.paddingDistance; }
        }

        /// <summary>
        /// The total distance all entities span (e.g. total height of all rows in grid)
        /// </summary>
        public double TotalDistance
        {
            get
            {
                double delta = Count - InternalCount;
                return InternalTotalDistance + delta * DefaultDistance;
            }
        }

        /// <summary>
        /// Assigns a collection with nested entities to an item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedCollection">The nested collection.</param>
        public void SetNestedDistances(int index, IDistanceCounterCollection nestedCollection)
        {
            CheckRange("index", 0, Count - 1, index);

            if (GetNestedDistances(index) != nestedCollection)
            {
                if (index >= InternalCount)
                    EnsureTreeCount(index + 1);

                DistanceLineCounterEntry entry = Split(index);
                Split(index + 1);

                if (nestedCollection != null)
                {
                    NestedDistanceCounterCollectionSource vcs = new NestedDistanceCounterCollectionSource(this, nestedCollection, entry);
                    entry.Value = vcs;
                }
                else
                {
                    entry.Value = new DistanceLineCounterSource(0, 1);
                }
                entry.InvalidateCounterBottomUp(true);
            }
        }

        /// <summary>
        /// Gets the nested entities at a given index. If the index does not hold
        /// a mested distances collection the method returns null.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The nested collection or null.</returns>
        public IDistanceCounterCollection GetNestedDistances(int index)
        {
            //CheckRange("index", 0, Count - 1, index); This moved after the condition check.Because While scrolling the nested grid it will raises the exception.

            if (index >= InternalCount)
                return null;
            CheckRange("index", 0, Count - 1, index);
            LineIndexEntryAt e = InitDistanceLine(index, false);
            NestedDistanceCounterCollectionSource vcs = e.rbValue as NestedDistanceCounterCollectionSource;
            if (vcs != null)
                return vcs.NestedDistances;

            return null;
        }

        /// <summary>
        /// Gets the distance position of the next entity after a given point. 
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance position.</returns>
        public double GetNextScrollValue(double point)
        {
            int index = IndexOfCumulatedDistance(point);
            if (index >= Count)
                return double.NaN;

            double nestedStart = GetCumulatedDistanceAt(index);
            double delta = point - nestedStart;
            IDistanceCounterCollection nestedDcc = GetNestedDistances(index);
            if (nestedDcc != null)
            {
                double r = nestedDcc.GetNextScrollValue(delta);
                if (!double.IsNaN(r) && r >= 0 && r < nestedDcc.TotalDistance)
                    return nestedStart + r;
            }

            index = GetNextVisibleIndex(index);
            if (index >= 0 && index < Count)
                return GetCumulatedDistanceAt(index);

            return double.NaN;
        }

        /// <summary>
        /// Gets the distance position of the entity preceeding a given point. If the point
        /// is in between entities the starting point of the matching entity
        /// is returned.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance position.</returns>
        public double GetPreviousScrollValue(double point)
        {
            int index = IndexOfCumulatedDistance(point);
            double nestedStart = GetCumulatedDistanceAt(index);
            double delta = point - nestedStart;

            if (delta > 0)
            {
                IDistanceCounterCollection nestedDcc = GetNestedDistances(index);
                if (nestedDcc != null)
                {
                    double r = nestedDcc.GetPreviousScrollValue(delta);
                    if (!double.IsNaN(r) && r >= 0 && r < nestedDcc.TotalDistance)
                        return nestedStart + r;
                }

                return GetCumulatedDistanceAt(index);
            }

            index = GetPreviousVisibleIndex(index);

            if (index >= 0 && index < Count)
            {
                nestedStart = GetCumulatedDistanceAt(index);

                IDistanceCounterCollection nestedDcc = GetNestedDistances(index);
                if (nestedDcc != null)
                {
                    delta = nestedDcc.TotalDistance;
                    double r = nestedDcc.GetPreviousScrollValue(delta);
                    if (!double.IsNaN(r) && r >= 0 && r < nestedDcc.TotalDistance)
                        return nestedStart + r;
                }

                return nestedStart;
            }

            return double.NaN;
        }

        /// <summary>
        /// Gets the aligned scroll value which is the starting point of the entity
        /// found at the given distance position.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The starting point of the entity.</returns>
        public double GetAlignedScrollValue(double point)
        {
            int index = IndexOfCumulatedDistance(point);
            double nestedStart = GetCumulatedDistanceAt(index);
            double delta = point - nestedStart;

            if (delta > 0)
            {
                IDistanceCounterCollection nestedDcc = GetNestedDistances(index);
                if (nestedDcc != null)
                {
                    double r = nestedDcc.GetAlignedScrollValue(delta);
                    if (!double.IsNaN(r) && r >= 0 && r < nestedDcc.TotalDistance)
                        return nestedStart + r;
                }
            }

            return GetCumulatedDistanceAt(index);
        }

        /// <summary>
        /// Connects a nested distance collection with a parent.
        /// </summary>
        /// <param name="treeTableCounterSource">The nested tree table visible counter source.</param>
        public void ConnectWithParent(ITreeTableCounterSource treeTableCounterSource)
        {
            rbTree.Tag = treeTableCounterSource;
        }

        /// <summary>
        /// Skip subsequent entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetNextVisibleIndex(int index)
        {
            CheckRange("index", 0, Count - 1, index);

            if (index >= InternalCount)
                return index + 1;

            LineIndexEntryAt e = InitDistanceLine(index + 1, true);
            if (e.rbValue.SingleLineDistance > 0)
            {
                if (index - e.rbEntryPosition.LineCount < e.rbValue.LineCount)
                    return index + 1;
            }

            e.rbEntry = rbTree.GetNextVisibleEntry(e.rbEntry);
            if (e.rbEntry == null)
            {
                if (InternalCount < Count)
                    return InternalCount;
                return -1;
            }

            e.rbEntryPosition = e.rbEntry.GetCounterPosition();
            return e.rbEntryPosition.LineCount;
        }

        /// <summary>
        /// Skip previous entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetPreviousVisibleIndex(int index)
        {
            CheckRange("index", 0, Count, index);

            if (index > InternalCount || index == 0)
                return index - 1;

            LineIndexEntryAt e = InitDistanceLine(index - 1, false);
            if (e.rbValue.SingleLineDistance > 0)
                return index - 1;

            e.rbEntry = rbTree.GetPreviousVisibleEntry(e.rbEntry);
            if (e.rbEntry == null)
                return -1;

            e.rbEntryPosition = e.rbEntry.GetCounterPosition();
            e.rbValue = e.rbEntry.Value;
            return e.rbEntryPosition.LineCount + e.rbValue.LineCount - 1;
        }

        /// <summary>
        /// Gets the index of an entity in this collection for which
        /// the cumulated count of previous distances is greater or equal
        /// the specified cumulatedDistance. (e.g. return row index for
        /// pixel position).
        /// </summary>
        /// <param name="cumulatedDistance">The cumulated count of previous distances.</param>
        /// <returns>The entity index.</returns>
        public int IndexOfCumulatedDistance(double cumulatedDistance)
        {
            if (cumulatedDistance < ((DistanceLineCounter)rbTree.GetStartCounterPosition()).Distance)
                return -1;

            return _IndexOfCumulatedDistance(cumulatedDistance);
        }

        int _IndexOfCumulatedDistance(double cumulatedDistance)
        {
            if (InternalCount == 0)
                return (int)Math.Floor(cumulatedDistance / DefaultDistance);

            int delta = 0;
            double internalTotalDistance = InternalTotalDistance - this.paddingDistance;
            if (cumulatedDistance >= internalTotalDistance)
            {
                delta = (int)Math.Floor((cumulatedDistance - internalTotalDistance) / DefaultDistance);
                cumulatedDistance = internalTotalDistance;
                return InternalCount + delta;
            }

            DistanceLineCounter searchPosition = new DistanceLineCounter(cumulatedDistance, 0);
            DistanceLineCounterEntry rbEntry = rbTree.GetEntryAtCounterPosition(searchPosition, DistanceLineCounterKind.Distance, false);
            DistanceLineCounterSource rbValue = rbEntry.Value;
            DistanceLineCounter rbEntryPosition = rbEntry.GetCounterPosition();
            if (rbValue.SingleLineDistance > 0)
                delta = (int)Math.Floor((cumulatedDistance - rbEntryPosition.Distance) / rbValue.SingleLineDistance);
            return rbEntryPosition.LineCount + delta;
        }

        /// <summary>
        /// Gets the cumulated count of previous distances for the
        /// entity at the specifiec index. (e.g. return pixel position
        /// for a row index).
        /// </summary>
        /// <param name="index">The entity index.</param>
        /// <returns>The cumulated count of previous distances for the
        /// entity at the specifiec index.</returns>
        public double GetCumulatedDistanceAt(int index)
        {
            return _GetCumulatedDistanceAt(index);
        }

        double _GetCumulatedDistanceAt(int index)
        {
            CheckRange("index", 0, Count + 1, index);

            if (index == Count + 1) return TotalDistance;
            if (index == 0) return 0;

            int count = InternalCount;
            if (count == 0) return index * DefaultDistance;

            if (index >= count)
            {
                int delta = index - count;
                DistanceLineCounter counter = rbTree.GetCounterTotal();
                return counter.Distance + delta * DefaultDistance;
            }
            else
            {
                LineIndexEntryAt e = InitDistanceLine(index, false);
                e.rbEntryPosition = e.rbEntry.GetCounterPosition();
                return e.rbEntryPosition.Distance + (index - e.rbEntryPosition.LineCount) * e.rbValue.SingleLineDistance;
            }
        }

        /// <summary>
        /// Gets or sets the distance for an entity.
        /// </summary>
        /// <param name="index">The index for the entity</param>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                CheckRange("index", 0, Count - 1, index);

                if (index >= InternalCount)
                    return DefaultDistance;

                LineIndexEntryAt e = InitDistanceLine(index, false);
                return e.rbValue.SingleLineDistance;
            }
            set
            {
                CheckRange("index", 0, Count - 1, index);
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value must not be negative.");

                if (!value.Equals(this[index]))
                {
                    if (index >= InternalCount)
                        EnsureTreeCount(index + 1);

                    DistanceLineCounterEntry entry = Split(index);
                    Split(index + 1);
                    entry.Value.SingleLineDistance = value;
                    entry.InvalidateCounterBottomUp(true);
                }
            }

        }

        /// <summary>
        /// Hides a specified range of entities (lines, rows or colums)
        /// </summary>
        /// <param name="from">The index for the first entity&gt;</param>
        /// <param name="to">The raw index for the last entity</param>
        /// <param name="distance">The distance.</param>
        public void SetRange(int from, int to, double distance)
        {
            CheckRange("from", 0, Count - 1, from);
            CheckRange("to", 0, Count - 1, to);

            if (from == to)
            {
                this[from] = distance;
                return;
            }

            if (from >= InternalCount && distance.Equals(defaultDistance))
                return;

            int count = to - from + 1;
            EnsureTreeCount(from);
            int n = RemoveHelper(from, count);
            DistanceLineCounterEntry rb = CreateTreeTableEntry(distance, count);
            rbTree.Insert(n, rb);
            Merge(rb, true);
        }

        /// <summary>
        /// Resets the range by restoring the default distance
        /// for all entries in the specified range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public void ResetRange(int from, int to)
        {
            CheckRange("from", 0, Count - 1, from);
            CheckRange("to", 0, Count - 1, to);

            if (from >= InternalCount)
                return;

            int count = to - from + 1;
            SetRange(from, count, defaultDistance);
        }

        /// <summary>
        /// Insert entities in the collection.
        /// </summary>
        /// <param name="insertAt">Insert position.</param>
        /// <param name="count">The number of entities to be inserted.</param>
        public void Insert(int insertAt, int count)
        {
            Insert(insertAt, count, DefaultDistance);
        }

        /// <summary>
        /// Insert entities in the collection.
        /// </summary>
        /// <param name="insertAt">Insert position.</param>
        /// <param name="count">The number of entities to be inserted.</param>
        /// <param name="distance">The distance to be set.</param>
        public void Insert(int insertAt, int count, double distance)
        {
            Count += count;

            if (insertAt >= InternalCount && distance == defaultDistance)
                return;

            EnsureTreeCount(insertAt);

            LineIndexEntryAt e = InitDistanceLine(insertAt, false);
            if (e.rbValue.SingleLineDistance == distance)
            {
                e.rbValue.LineCount += count;
                e.rbEntry.InvalidateCounterBottomUp(true);
            }
            else
            {
                DistanceLineCounterEntry rbEntry0 = Split(insertAt);
                DistanceLineCounterEntry entry = CreateTreeTableEntry(distance, count);
                if (rbEntry0 == null)
                    rbTree.Add(entry);
                else
                    rbTree.Insert(rbTree.IndexOf(rbEntry0), entry);
                //entry.InvalidateCounterBottomUp(true);
                Merge(entry, true);
            }
        }

        /// <summary>
        /// Removes enities from the collection.
        /// </summary>
        /// <param name="removeAt">Index of the first entity to be removed.</param>
        /// <param name="count">The number of entities to be removed.</param>
        public void Remove(int removeAt, int count)
        {
            Count -= count;

            int icount = InternalCount;
            if (removeAt >= InternalCount)
                return;

            int n = RemoveHelper(removeAt, count);
            if (n > 0)
                Merge(rbTree[n - 1], false);
        }

        DistanceLineCounterEntry CreateTreeTableEntry(double distance, int count)
        {
            DistanceLineCounterEntry entry = new DistanceLineCounterEntry();
            entry.Value = new DistanceLineCounterSource(distance, count);
            entry.Tree = rbTree;
            return entry;
        }

        int RemoveHelper(int removeAt, int count)
        {
            if (removeAt >= InternalCount)
                return rbTree.GetCount();

            DistanceLineCounterEntry entry = Split(removeAt);
            Split(removeAt + count);
            int n = rbTree.IndexOf(entry);

            List<DistanceLineCounterEntry> toDelete = new List<DistanceLineCounterEntry>();

            int total = 0;
            while (total < count && entry != null)
            {
                total += entry.Value.LineCount;
                toDelete.Add(entry);
                entry = rbTree.GetNextEntry(entry);
            }

            for (int l = 0;l < toDelete.Count;l++)
            {
                //toDelete[l].InvalidateCounterBottomUp(true);
                rbTree.Remove(toDelete[l]);
            }
            return n;
        }

        DistanceLineCounterEntry Split(int index)
        {
            if (index >= InternalCount)
                return null;

            LineIndexEntryAt e = InitDistanceLine(index, true);
            if (e.rbEntryPosition.LineCount != index)
            {
                int count1 = index - e.rbEntryPosition.LineCount;
                int count2 = e.rbValue.LineCount - count1;

                e.rbValue.LineCount = count1;

                DistanceLineCounterEntry rbEntry2 = CreateTreeTableEntry(e.rbValue.SingleLineDistance, count2);
                rbTree.Insert(rbTree.IndexOf(e.rbEntry) + 1, rbEntry2);
                //rbEntry2.InvalidateCounterBottomUp(true);
                e.rbEntry.InvalidateCounterBottomUp(true);
                return rbEntry2;
            }

            return e.rbEntry;
        }

        void Merge(DistanceLineCounterEntry entry, bool checkPrevious)
        {
            DistanceLineCounterSource value = entry.Value;
            DistanceLineCounterEntry previousEntry = null;
            if (checkPrevious)
                previousEntry = rbTree.GetPreviousEntry(entry);
            DistanceLineCounterEntry nextEntry = rbTree.GetNextEntry(entry);

            bool dirty = false;
            if (previousEntry != null &&
                (previousEntry.Value).SingleLineDistance == value.SingleLineDistance)
            {
                value.LineCount += (previousEntry.Value).LineCount;
                //previousEntry.InvalidateCounterBottomUp(true);
                rbTree.Remove(previousEntry);
                dirty = true;
            }

            if (nextEntry != null && (nextEntry.Value).SingleLineDistance == value.SingleLineDistance)
            {
                value.LineCount += (nextEntry.Value).LineCount;
                //nextEntry.InvalidateCounterBottomUp(true);
                rbTree.Remove(nextEntry);
                dirty = true;
            }

            if (dirty)
                entry.InvalidateCounterBottomUp(true);
        }

        void EnsureTreeCount(int count)
        {
            int treeCount = InternalCount;
            int insert = count - treeCount;
            if (insert > 0)
            {
                DistanceLineCounterEntry entry = CreateTreeTableEntry(DefaultDistance, insert);
                rbTree.Add(entry);
                //entry.InvalidateCounterBottomUp(true);
            }
        }

        int InternalCount
        {
            get
            {
                if (rbTree.GetCount() == 0)
                    return 0;
                DistanceLineCounter counter = rbTree.GetCounterTotal();
                return counter.LineCount;
            }
            set
            {
                if (value >= InternalCount)
                    EnsureTreeCount(value);
                else
                {
                    int n = InternalCount - value;
                    // TODO: possible remove entries that are not needed any more.
                    //while (n-- > 0)
                    //    rbTree.RemoveAt(value + n);
                }
            }
        }

        double InternalTotalDistance
        {
            get
            {
                int treeCount = InternalCount;
                if (treeCount == 0)
                    return paddingDistance;

                DistanceLineCounter counter = rbTree.GetCounterTotal();
                return counter.Distance + paddingDistance;
            }
        }

        void CheckRange(string paramName, int from, int to, int actualValue)
        {
            if ((actualValue < from || actualValue > to) && to !=0)
                throw new ArgumentOutOfRangeException(paramName, actualValue.ToString() + " out of range " + from.ToString() + " to " + to.ToString());
        }

        LineIndexEntryAt InitDistanceLine(int lineIndex, bool determineEntryPosition)
        {
            LineIndexEntryAt e = new LineIndexEntryAt();
            e.searchPosition = new DistanceLineCounter(0, lineIndex);
            e.rbEntry = rbTree.GetEntryAtCounterPosition(e.searchPosition, DistanceLineCounterKind.Lines, false);
            e.rbValue = e.rbEntry.Value;
            e.rbEntryPosition = null;
            if (determineEntryPosition)
            {
                if (e.rbValue.LineCount > 1)
                    e.rbEntryPosition = e.rbEntry.GetCounterPosition();
                else
                    e.rbEntryPosition = e.searchPosition;
            }
            return e;
        }

        internal class LineIndexEntryAt
        {
            public DistanceLineCounter searchPosition;
            public DistanceLineCounterEntry rbEntry;
            public DistanceLineCounterSource rbValue;
            public DistanceLineCounter rbEntryPosition;
        }

        /// <summary>
        /// An object that maintains a collection of nested distances and wires
        /// it to a parent distance collection. The object is used by the 
        /// DistanceCounterCollection.SetNestedDistances method to associated
        /// the nested distances with an index in the parent collection.
        /// </summary>
        class NestedDistanceCounterCollectionSource : DistanceLineCounterSource
        {
            IDistanceCounterCollection nestedDistances;
            IDistanceCounterCollection parentDistances;
            DistanceLineCounterEntry entry;

            /// <summary>
            /// Initializes a new instance of the <see cref="NestedDistanceCounterCollectionSource"/> class.
            /// </summary>
            /// <param name="parentDistances">The parent distances.</param>
            /// <param name="nestedDistances">The nested distances.</param>
            /// <param name="entry">The entry.</param>
            public NestedDistanceCounterCollectionSource(IDistanceCounterCollection parentDistances, IDistanceCounterCollection nestedDistances, DistanceLineCounterEntry entry)
                : base(0, 1)
            {
                this.parentDistances = parentDistances;
                this.nestedDistances = nestedDistances;
                this.entry = entry;

                if (nestedDistances != null)
                    nestedDistances.ConnectWithParent(this);
            }

            /// <summary>
            /// Gets or sets the counter entry.
            /// </summary>
            /// <value>The entry.</value>
            public DistanceLineCounterEntry Entry
            {
                get { return entry; }
                set { entry = value; }
            }

            public override double SingleLineDistance
            {
                get { return nestedDistances != null ? nestedDistances.TotalDistance : 0; }
                set { throw new InvalidOperationException(""); }
            }

            /// <summary>
            /// Gets the parent distances.
            /// </summary>
            /// <value>The parent distances.</value>
            public IDistanceCounterCollection ParentDistances
            {
                get { return parentDistances; }
            }

            /// <summary>
            /// Gets the nested distances.
            /// </summary>
            /// <value>The nested distances.</value>
            public IDistanceCounterCollection NestedDistances
            {
                get { return nestedDistances; }
            }

            /// <summary>
            /// Marks all counters dirty in this object and parent nodes.
            /// </summary>
            public override void InvalidateCounterBottomUp()
            {
                if (Entry != null)
                    Entry.InvalidateCounterBottomUp(true);
            }

            /// <summary>
            /// Returns the <see cref="TreeTableVisibleCounter"/> object with counters.
            /// </summary>
            /// <returns></returns>
            public override ITreeTableCounter GetCounter()
            {
                return new DistanceLineCounter(nestedDistances == null ? 0 : nestedDistances.TotalDistance, 1);
            }


            public override string ToString()
            {
                return GetType().Name.ToString() + String.Format("( LineCount = {0}, SingleLineDistance = {1} )", LineCount, SingleLineDistance);
            }
        }
#if !WinRT
#if !SILVERLIGHT && DEBUG
        /// <summary>
        /// Tests this instance.
        /// </summary>
        public static void Test()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 1");
            DistanceRangeCounterCollection dcc = new DistanceRangeCounterCollection();
            dcc.DefaultDistance = 10;
            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 2");
            dcc.Count = 5;
            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            double[] arr = new double[dcc.count];
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0;n < arr.Length;n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 3");

            dcc[4] = 30;
            dcc[2] = 25;
            dcc.Count = 7;

            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            arr = new double[dcc.count];
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0;n < arr.Length;n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));



            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 4");

            dcc.Remove(0, 1);

            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            arr = new double[dcc.count];
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0;n < arr.Length;n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));

            TreeTableWithCounter.DumpNode(dcc.rbTree.Root);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 5");


            dcc.Insert(1, 2, 50);

            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            arr = new double[dcc.count];
            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0;n < arr.Length;n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));

            TreeTableWithCounter.DumpNode(dcc.rbTree.Root);
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 6");

            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetNextVisibleIndex({0}) = {1}", n, dcc.GetNextVisibleIndex(n));

            for (int n = 0;n < dcc.Count;n++)
                Console.WriteLine("{0}: GetPreviousVisibleIndex({0}) = {1}", n, dcc.GetPreviousVisibleIndex(n));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 7");

            dcc.ResetRange(0, 5);
            dcc.SetRange(3, 3, 100);
            TreeTableWithCounter.DumpNode(dcc.rbTree.Root);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 8");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 9");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 10");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 11");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 12");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 13");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 14");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 15");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 16");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 17");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 18");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 19");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
        }
#endif
#endif
    }

    /// <summary>
    /// An object that counts objects that are marked "Visible". It implements
    /// the ITreeTableCounterSource interface and creates a <see cref="DistanceLineCounter"/>.
    /// </summary>
    internal class DistanceLineCounterSource : ITreeTableCounterSource
    {
        double singleLineDistance;
        int lineCount;

        /// <summary>
        /// Initializes the object with visible count.
        /// </summary>
        /// <param name="visibleCount">The visible count.</param>
        /// <param name="lineCount">The line count.</param>
        public DistanceLineCounterSource(double visibleCount, int lineCount)
        {
            this.singleLineDistance = visibleCount;
            this.lineCount = lineCount;
        }

        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public int LineCount
        {
            get { return lineCount; }
            set { lineCount = value; }
        }

        /// <summary>
        /// Gets or sets the distance of a single line.
        /// </summary>
        /// <value>The single line distance.</value>
        public virtual double SingleLineDistance
        {
            get { return singleLineDistance; }
            set { singleLineDistance = value; }
        }

        /// <summary>
        /// Returns a string describing the state of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name.ToString() + String.Format("( LineCount = {0}, SingleLineDistance = {1} )", LineCount, SingleLineDistance);
        }

        #region ITreeTableCounterSource Members

        public virtual ITreeTableCounter GetCounter()
        {
            return new DistanceLineCounter(SingleLineDistance * lineCount, lineCount);
        }

        public virtual void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        public virtual void InvalidateCounterBottomUp()
        {
        }

        #endregion
    }

    /// <summary>
    /// A collection of integers used to specify various counter kinds.
    /// </summary>
    internal class DistanceLineCounterKind
    {
        /// <summary>
        /// All counters.
        /// </summary>
        public const int CountAll = 0xffff;
        /// <summary>
        /// Visible Counter.
        /// </summary>
        public const int Distance = 0x8000;

        /// <summary>
        /// Line Counter.
        /// </summary>
        public const int Lines = 1;
    }

    /// <summary>
    /// A counter that counts objects that are marked "Visible".
    /// </summary>
    internal class DistanceLineCounter : ITreeTableCounter
    {
        double distance;
        int lineCount;

        /// <summary>
        /// Returns an empty DistanceLineCounter that represents zero visible elements.
        /// </summary>
        public static readonly DistanceLineCounter Empty = new DistanceLineCounter(0, 0);

        /// <summary>
        /// Initializes a <see cref="DistanceLineCounter"/> with a pecified number of visible elements.
        /// </summary>
        /// <param name="distance">The visible count.</param>
        /// <param name="lineCount">The line count.</param>
        public DistanceLineCounter(double distance, int lineCount)
        {
            this.distance = distance;
            this.lineCount = lineCount;
        }

        /// <summary>
        /// Gets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public int LineCount
        {
            get
            {
                return lineCount;
            }
        }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance
        {
            get
            {
                return distance;
            }
        }

        /// <summary>
        /// The Counter Kind.
        /// </summary>
        public int Kind { get { return 0; } }

        /// <summary>
        /// Returns the integer value of the counter. A cookie specifies
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public double GetValue(int cookie)
        {
            if (cookie == DistanceLineCounterKind.Lines)
                return lineCount;
            return distance;
        }


        /// <summary>
        /// Combines one tree obkect with another and returns the new object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        ITreeTableCounter ITreeTableCounter.Combine(ITreeTableCounter other, int cookie)
        {
            return Combine((DistanceLineCounter)other, cookie);
        }

        /// <summary>
        /// Combines the counter values of this counter object with the values of another counter object
        /// and returns a new counter object.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public DistanceLineCounter Combine(DistanceLineCounter other, int cookie)
        {
            if (other == null || other.IsEmpty(int.MaxValue))
                return this;

            if (this.IsEmpty(int.MaxValue))
                return other;

            return new DistanceLineCounter(Distance + other.Distance, LineCount + other.LineCount);
        }

        double ITreeTableCounter.Compare(ITreeTableCounter other, int cookie)
        {
            return Compare((DistanceLineCounter)other, cookie);
        }

        /// <summary>
        /// Compares this counter with another counter. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns></returns>
        public double Compare(DistanceLineCounter other, int cookie)
        {
            if (other == null)
                return 0;

            int cmp = 0;

            if ((cookie & DistanceLineCounterKind.Distance) != 0)
                cmp = Distance.CompareTo(other.Distance);

            if (cmp == 0 && (cookie & DistanceLineCounterKind.Lines) != 0)
                cmp = LineCount.CompareTo(other.LineCount);

            return cmp;
        }

        /// <summary>
        /// Indicates whether the counter object is empty. A cookie can specify
        /// a specific counter type.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// 	<c>true</c> if the specified cookie is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty(int cookie)
        {
            return Compare(Empty, cookie) == 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} ( LineCount = {1} Distance = {2}", GetType().Name, this.LineCount, this.Distance);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class DistanceLineCounterTree : TreeTableWithCounter
    {
        public DistanceLineCounterTree(DistanceLineCounter startPos, bool sorted)
            : base(startPos, sorted)
        {
        }

        public new DistanceLineCounter GetCounterTotal()
        {
            return (DistanceLineCounter)base.GetCounterTotal();
        }

        public DistanceLineCounterEntry GetPreviousEntry(DistanceLineCounterEntry current)
        {
            return (DistanceLineCounterEntry)base.GetPreviousEntry(current);
        }

        public DistanceLineCounterEntry GetNextEntry(DistanceLineCounterEntry current)
        {
            return (DistanceLineCounterEntry)base.GetNextEntry(current);
        }

        public DistanceLineCounterEntry GetEntryAtCounterPosition(DistanceLineCounter searchPosition, int cookie)
        {
            return (DistanceLineCounterEntry)base.GetEntryAtCounterPosition(searchPosition, cookie);
        }

        public DistanceLineCounterEntry GetEntryAtCounterPosition(DistanceLineCounter searchPosition, int cookie, bool preferLeftMost)
        {
            return (DistanceLineCounterEntry)base.GetEntryAtCounterPosition(searchPosition, cookie, preferLeftMost);
        }

        public DistanceLineCounterEntry GetNextNotEmptyCounterEntry(DistanceLineCounterEntry current, int cookie)
        {
            return (DistanceLineCounterEntry)base.GetNextNotEmptyCounterEntry(current, cookie);
        }

        public DistanceLineCounterEntry GetPreviousNotEmptyCounterEntry(DistanceLineCounterEntry current, int cookie)
        {
            return (DistanceLineCounterEntry)base.GetPreviousNotEmptyCounterEntry(current, cookie);
        }

        public DistanceLineCounterEntry GetNextVisibleEntry(DistanceLineCounterEntry current)
        {
            return (DistanceLineCounterEntry)base.GetNextVisibleEntry(current);
        }

        public DistanceLineCounterEntry GetPreviousVisibleEntry(DistanceLineCounterEntry current)
        {
            return (DistanceLineCounterEntry)base.GetPreviousVisibleEntry(current);
        }

        public new DistanceLineCounterEntry this[int index]
        {
            get { return (DistanceLineCounterEntry)base[index]; }
            set { base[index] = value; }
        }

        public void Insert(int index, DistanceLineCounterEntry value)
        {
            base.Insert(index, value);
        }

        public void Remove(DistanceLineCounterEntry value)
        {
            base.Remove(value);
        }

        public bool Contains(DistanceLineCounterEntry value)
        {
            return base.Contains(value);
        }

        public int IndexOf(DistanceLineCounterEntry value)
        {
            return base.IndexOf(value);
        }

        public int Add(DistanceLineCounterEntry value)
        {
            return base.Add(value);
        }

    }

    internal class DistanceLineCounterEntry : TreeTableWithCounterEntry
    {
        public new DistanceLineCounterSource Value
        {
            get { return (DistanceLineCounterSource)base.Value; }
            set { base.Value = value; }
        }

        public new DistanceLineCounter GetCounterPosition()
        {
            return (DistanceLineCounter)base.GetCounterPosition();
        }
    }
}
