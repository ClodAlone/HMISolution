#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
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
    /// DistanceCounterCollection uses a high-water mark technique for allocating
    /// objects up to the modified entry with the highest index. When you modify 
    /// the size of an entry the collection ensures that that objects are allocated 
    /// for all entries up to the given index. Entries that are after the modified 
    /// entry are assumed to have the DefaultSize and will not be allocated. 
    /// <para/>
    /// The best-case scenario is when all lines have the same DefaultSize. In such
    /// case the internal collection remains completely empty and will not cause 
    /// any overhead. This makes DistanceCounterCollection also an attractive solution
    /// for the scenario where all entries have the same size (e.g. a databound grid
    /// where all rows have same height).
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class DistanceCounterCollection : IDistanceCounterCollection
    {
        TreeTableWithCounter rbTree;
        int count = 0;
        double defaultDistance = 1.0;

        /// <summary>
        /// Constructs the class and initializes the internal tree.
        /// </summary>
        public DistanceCounterCollection()
        {
            TreeTableVisibleCounter startPos = new TreeTableVisibleCounter(0);
            rbTree = new TreeTableWithCounter(startPos, false);
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
        /// Hides a specified range of entities (lines, rows or colums)
        /// </summary>
        /// <param name="from">The index for the first entity&gt;</param>
        /// <param name="to">The raw index for the last entity</param>
        /// <param name="distance">The distance.</param>
        public void SetRange(int from, int to, double distance)
        {
            CheckRange("from", 0, Count - 1, from);
            CheckRange("to", 0, Count - 1, to);

            EnsureTreeCount(to + 1);

            for (int n = from; n <= to; n++)
            {
                TreeTableWithCounterEntry rbEntry = rbTree[n];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[n].GetCounterTotal();
                if (counter.GetVisibleCount() != distance)
                {
                    rbEntry.Value = new TreeTableVisibleCounterSource(distance);
                    rbEntry.InvalidateCounterBottomUp(true);
                }
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

            EnsureTreeCount(index + 1);

            TreeTableWithCounterEntry rbEntry = rbTree[index];
            NestedTreeTableVisibleCounterSource vcs = new NestedTreeTableVisibleCounterSource(this, nestedCollection);
            rbEntry.Value = vcs;
            rbEntry.InvalidateCounterBottomUp(false);
            vcs.Entry = rbEntry;
        }

        /// <summary>
        /// Gets the nested entities at a given index. If the index does not hold
        /// a mested distances collection the method returns null.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The nested collection or null.</returns>
        public IDistanceCounterCollection GetNestedDistances(int index)
        {
            if (index < rbTree.GetCount())
            {
                TreeTableWithCounterEntry rbEntry = rbTree[index];
                if (rbEntry != null)
                {
                    NestedTreeTableVisibleCounterSource vcs = rbEntry.Value as NestedTreeTableVisibleCounterSource;
                    if (vcs != null)
                        return vcs.NestedDistances;
                }
            }

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
        /// <param name="nestedTreeTableVisibleCounterSource">The nested tree table visible counter source.</param>
        public void ConnectWithParent(ITreeTableCounterSource nestedTreeTableVisibleCounterSource)
        {
            rbTree.Tag = nestedTreeTableVisibleCounterSource;
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

            if (from >= rbTree.GetCount())
                return;

            if (to >= rbTree.GetCount())
            {
                // TODO: Review if should check for rbTree.GetCount()-1
                if (from == 0)
                    Clear();

                to = rbTree.GetCount() - 1;
                for (int n = from; n <= to; n++)
                {
                    TreeTableWithCounterEntry rbEntry = rbTree[n];
                    TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[n].GetCounterTotal();
                    if (counter.GetVisibleCount() != DefaultDistance)
                    {
                        rbEntry.InvalidateCounterBottomUp(false);
                    } 
                    rbTree.Remove(rbEntry);
                }
            }
            else
            {
                for (int n = from; n <= to; n++)
                {
                    TreeTableWithCounterEntry rbEntry = rbTree[n];
                    TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[n].GetCounterTotal();
                    if (counter.GetVisibleCount() != DefaultDistance)
                    {
                        rbEntry.Value = DefaultDistance;
                        rbEntry.InvalidateCounterBottomUp(false);
                    }
                }
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

                TreeTableWithCounterEntry rbEntry = rbTree[index];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[index].GetCounterTotal();
                return counter.GetVisibleCount();
            }
            set
            {
                CheckRange("index", 0, Count - 1, index);

                if (index >= InternalCount)
                    EnsureTreeCount(index + 1);

                TreeTableWithCounterEntry rbEntry = rbTree[index];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[index].GetCounterTotal();
                if (counter.GetVisibleCount() != value)
                {
                    rbEntry.Value = new TreeTableVisibleCounterSource(value);
                    rbEntry.InvalidateCounterBottomUp(false);
                }
            }

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

            TreeTableWithCounterEntry rbEntry = rbTree[index];
            rbEntry = rbTree.GetNextVisibleEntry(rbEntry);
            if (rbEntry == null)
            {
                if (InternalCount < Count)
                    return InternalCount;
                return -1;
            }
            return rbTree.IndexOf(rbEntry);
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

            TreeTableWithCounterEntry rbEntry;
            if (index == InternalCount)
            {
                rbEntry = rbTree[index - 1];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbEntry.GetCounterTotal();
                if (counter.GetVisibleCount() == 0)
                    rbEntry = rbTree.GetPreviousVisibleEntry(rbEntry);
            }
            else
            {
                rbEntry = rbTree[index];
                rbEntry = rbTree.GetPreviousVisibleEntry(rbEntry);
            }

            return rbTree.IndexOf(rbEntry);
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
            if (InternalCount == 0)
                return (int)Math.Floor(cumulatedDistance / DefaultDistance);

            int delta = 0;
            double internalTotalDistance = InternalTotalDistance;
            if (cumulatedDistance >= internalTotalDistance)
            {
                delta = (int)Math.Floor((cumulatedDistance - internalTotalDistance) / DefaultDistance);
                cumulatedDistance = internalTotalDistance;
                return InternalCount + delta;
            }

            TreeTableEntry entry = rbTree.GetEntryAtCounterPosition(new TreeTableVisibleCounter(cumulatedDistance), TreeTableCounterCookies.CountVisible, false);
            return rbTree.IndexOf(entry) + delta;
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
            CheckRange("index", 0, Count + 1, index);

            if (index == Count + 1) return TotalDistance;
            if (index == 0) return 0;

            int delta = 0;
            int count = InternalCount;
            if (count == 0)
                return index * DefaultDistance;

            TreeTableVisibleCounter counter;
            if (index >= count)
            {
                delta = index - count;
                counter = (TreeTableVisibleCounter)rbTree.GetCounterTotal();
            }
            else
                counter = (TreeTableVisibleCounter)rbTree[index].GetCounterPosition();
            
            return counter.GetVisibleCount() + delta * DefaultDistance;
        }

        /// <summary>
        /// Insert entities in the collection.
        /// </summary>
        /// <param name="insertAt">Insert position.</param>
        /// <param name="count">The number of entities to be inserted.</param>
        public void Insert(int insertAt, int count)
        {
            Count += count;

            if (insertAt >= InternalCount)
                return;

            for (int n = 0; n < count; n++)
            {
                TreeTableWithCounterEntry rbEntry = new TreeTableWithCounterEntry();
                rbEntry.Value = new TreeTableVisibleCounterSource(DefaultDistance);
                rbEntry.Tree = rbTree;
                rbTree.Insert(n + insertAt, rbEntry);
                rbEntry.InvalidateCounterBottomUp(false);
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

            if (removeAt >= InternalCount)
                return;

            for (int n = count - 1; n >= 0; n--)
            {
                int index = removeAt + n;
                if (index < InternalCount)
                {
                    TreeTableWithCounterEntry rbEntry = rbTree[index];
                    rbEntry.InvalidateCounterBottomUp(false);
                    rbTree.Remove(rbEntry);
                }
            }
        }

        void EnsureTreeCount(int count)
        {
            int treeCount = rbTree.GetCount();
            if (treeCount == 0)
            {
                rbTree.BeginInit();
                for (int n = 0; n < count; n++)
                {
                    TreeTableWithCounterEntry rbEntry = new TreeTableWithCounterEntry();
                    rbEntry.Value = new TreeTableVisibleCounterSource(DefaultDistance);
                    rbEntry.Tree = rbTree;
                    rbTree.Add(rbEntry);
                }
                rbTree.EndInit();
            }
            else if (treeCount < count)
            {
                for (int n = treeCount; n < count; n++)
                {
                    TreeTableWithCounterEntry rbEntry = new TreeTableWithCounterEntry();
                    rbEntry.Value = new TreeTableVisibleCounterSource(DefaultDistance);
                    rbEntry.Tree = rbTree;
                    rbTree.Add(rbEntry);
                }
            }
        }

        int InternalCount
        {
            get
            {
                return rbTree.GetCount();
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
                    return 0;

                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree.GetCounterTotal();
                return counter.GetVisibleCount();
            }
        }

        void CheckRange(string paramName, int from, int to, int actualValue)
        {
            if (actualValue < from || actualValue > to)
                throw new ArgumentOutOfRangeException(paramName, actualValue.ToString() +  " out of range " + from.ToString() + " to " + to.ToString());
        }

        /// <summary>
        /// An object that maintains a collection of nested distances and wires
        /// it to a parent distance collection. The object is used by the 
        /// DistanceCounterCollection.SetNestedDistances method to associated
        /// the nested distances with an index in the parent collection.
        /// </summary>
        class NestedTreeTableVisibleCounterSource : TreeTableVisibleCounterSource
        {
            IDistanceCounterCollection nestedDistances;
            IDistanceCounterCollection parentDistances;
            TreeTableWithCounterEntry entry;

            /// <summary>
            /// Initializes a new instance of the <see cref="NestedTreeTableVisibleCounterSource"/> class.
            /// </summary>
            /// <param name="parentDistances">The parent distances.</param>
            /// <param name="nestedDistances">The nested distances.</param>
            public NestedTreeTableVisibleCounterSource(IDistanceCounterCollection parentDistances, IDistanceCounterCollection nestedDistances)
                : base(0)
            {
                this.parentDistances = parentDistances;
                this.nestedDistances = nestedDistances;

                nestedDistances.ConnectWithParent(this);
            }

            /// <summary>
            /// Gets or sets the counter entry.
            /// </summary>
            /// <value>The entry.</value>
            public TreeTableWithCounterEntry Entry
            {
                get { return entry; }
                set { entry = value; }
            }

            /// <summary>
            /// Gets the parent distances.
            /// </summary>
            /// <value>The parent distances.</value>
            public IDistanceCounterCollection ParentDistances
            {
                get { return parentDistances; }
                // set { parentDistances = value; }
            }

            /// <summary>
            /// Gets the nested distances.
            /// </summary>
            /// <value>The nested distances.</value>
            public IDistanceCounterCollection NestedDistances
            {
                get { return nestedDistances; }
                //set { nestedDistances = value; }
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
                return new TreeTableVisibleCounter(nestedDistances.TotalDistance);
            }
        }

#if !WinRT
        /// <summary>
        /// Tests this instance.
        /// </summary>
        public static void Test()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 1");
            DistanceCounterCollection dcc = new DistanceCounterCollection();
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
            for (int n = 0; n < dcc.Count; n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            double[] arr = new double[dcc.count];
            for (int n = 0; n < dcc.Count; n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0; n < arr.Length; n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 3");

            dcc[4] = 30;

            Console.WriteLine("Count= {0}", dcc.Count);
            Console.WriteLine("TotalDistance = {0}", dcc.TotalDistance);
            Console.WriteLine("InternalTotalDistance = {0}", dcc.InternalTotalDistance);
            Console.WriteLine("InternalCount = {0}", dcc.InternalCount);
            for (int n = 0; n < dcc.Count; n++)
                Console.WriteLine("{0}: this[{0}] = {1}", n, dcc[n]);

            arr = new double[dcc.count];
            for (int n = 0; n < dcc.Count; n++)
                Console.WriteLine("{0}: GetDistanceAt({0}) = {1}", n, arr[n] = dcc.GetCumulatedDistanceAt(n));

            for (int n = 0; n < arr.Length; n++)
                Console.WriteLine("{0}: IndexOfDistance({1}) = {2}", n, arr[n], dcc.IndexOfCumulatedDistance(arr[n]));



            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 4");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 5");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 6");
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("Test 7");
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
    }
}
