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
    /// A collection of entities that is shared with a parent collection for which distances
    /// need to counted. The collection only is a subset for a specific range in
    /// the parent distance collection.
    /// <para/>
    /// When you change the size of an element in this collection the change will
    /// also be reflected in the parent collection and vice versa.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class DistanceCounterSubset : IDistanceCounterCollection, IDistancesHost
    {
        IDistanceCounterCollection trackDCC;
        int count;
        int start;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceCounterSubset"/> class.
        /// </summary>
        /// <param name="trackedParentCollection">The parent collection for which a subset is "tracked".</param>
        public DistanceCounterSubset(IDistanceCounterCollection trackedParentCollection)
        {
            this.trackDCC = trackedParentCollection;
        }

        /// <summary>
        /// Gets or sets the starting index of this collection in the parent collection.
        /// </summary>
        /// <value>The start.</value>
        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// Gets or sets the ending index of this collection in the parent collection.
        /// </summary>
        /// <value>The start.</value>
        public int End
        {
            get { return start + count - 1; }
        }

        #region IDistanceCounterCollection Members

        /// <summary>
        /// Restores the distances in the parent collection for this subset to their default distance.
        /// </summary>
        public void Clear()
        {
            trackDCC.ResetRange(Start, End);
        }

        /// <summary>
        /// The raw number of entities (lines, rows or columns).
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                if (count != value)
                {
                    count = value;
                }
            }
        }

        /// <summary>
        /// The default distance (row height or column width) an entity spans
        /// </summary>
        /// <value></value>
        public double DefaultDistance
        {
            get
            {
                return trackDCC.DefaultDistance;
            }
            set
            {
                trackDCC.DefaultDistance = value;
            }
        }

        /// <summary>
        /// The total distance all entities span (e.g. total height of all rows in grid)
        /// </summary>
        /// <value></value>
        public double TotalDistance
        {
            get
            {
                if (Count == 0) return 0;
                return trackDCC.GetCumulatedDistanceAt(End) - trackDCC.GetCumulatedDistanceAt(start) + trackDCC[End];
            }
        }

        /// <summary>
        /// Hides a specified range of entities (lines, rows or colums)
        /// </summary>
        /// <param name="from">The index for the first entity&gt;</param>
        /// <param name="to">The raw index for the last entity</param>
        /// <param name="distance"></param>
        public void SetRange(int from, int to, double distance)
        {
            trackDCC.SetRange(from + start, to + start, distance);
        }

        /// <summary>
        /// Resets the range by restoring the default distance
        /// for all entries in the specified range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public void ResetRange(int from, int to)
        {
            trackDCC.ResetRange(from + start, to + start);
        }

        /// <summary>
        /// Gets or sets the distance for an entity.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                return trackDCC[index + start];
            }
            set
            {
                trackDCC[index + start] = value;
            }
        }

        /// <summary>
        /// Skip subsequent entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetNextVisibleIndex(int index)
        {
            int n = trackDCC.GetNextVisibleIndex(index + start);
            if (n > End)
                return -1;
            return n - start;
        }

        /// <summary>
        /// Skip previous entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetPreviousVisibleIndex(int index)
        {
            int n = trackDCC.GetPreviousVisibleIndex(index + start);
            if (n < start)
                return -1;
            return n - start;
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
            if (Count == 0 && cumulatedDistance == 0) return 0;
            int n = trackDCC.IndexOfCumulatedDistance(cumulatedDistance + trackDCC.GetCumulatedDistanceAt(start));
            if (n > End || n < start)
                return -1;

            return n - start;
        }

        /// <summary>
        /// Gets the cumulated count of previous distances for the
        /// entity at the specifiec index. (e.g. return pixel position
        /// for a row index).
        /// </summary>
        /// <param name="index">The entity index.</param>
        /// <returns>
        /// The cumulated count of previous distances for the
        /// entity at the specifiec index.
        /// </returns>
        public double GetCumulatedDistanceAt(int index)
        {
            return trackDCC.GetCumulatedDistanceAt(index + start) - trackDCC.GetCumulatedDistanceAt(start);
        }

        /// <summary>
        /// Assigns a collection with nested entities to an item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedCollection">The nested collection.</param>
        public void SetNestedDistances(int index, IDistanceCounterCollection nestedCollection)
        {
            trackDCC.SetNestedDistances(index + start, nestedCollection);
        }

        /// <summary>
        /// Gets the nested entities at a given index. If the index does not hold
        /// a mested distances collection the method returns null.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The nested collection or null.</returns>
        public IDistanceCounterCollection GetNestedDistances(int index)
        {
            return trackDCC.GetNestedDistances(index + start);
        }

        /// <summary>
        /// Gets the distance position of the next entity after a given point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance position.</returns>
        public double GetNextScrollValue(double point)
        {
            double offset = trackDCC.GetCumulatedDistanceAt(start);
            double d = trackDCC.GetNextScrollValue(point + offset);
            if (double.IsNaN(d) || d < offset || d - offset > TotalDistance)
                return double.NaN;
            return d - offset;
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
            double offset = trackDCC.GetCumulatedDistanceAt(start);
            double d = trackDCC.GetPreviousScrollValue(point + offset);
            if (double.IsNaN(d) || d < offset || d - offset > TotalDistance)
                return double.NaN;
            return d - offset;
        }

        /// <summary>
        /// Gets the aligned scroll value which is the starting point of the entity
        /// found at the given distance position.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The starting point of the entity.</returns>
        public double GetAlignedScrollValue(double point)
        {
            double offset = trackDCC.GetPreviousScrollValue(start);
            double d = trackDCC.GetAlignedScrollValue(point + offset);
            if (double.IsNaN(d) || d < offset || d - offset > TotalDistance)
                return double.NaN;
            return d - offset;
        }

        /// <summary>
        /// This method is not supported for DistanceCounterSubset.
        /// </summary>
        /// <param name="treeTableCounterSource">The nested tree table visible counter source.</param>
        void IDistanceCounterCollection.ConnectWithParent(ITreeTableCounterSource treeTableCounterSource)
        {
            throw new NotSupportedException("Do not use DistanceCounterSubset as nested collection!");
        }

        /// <summary>
        /// Insert entities in the collection.
        /// </summary>
        /// <param name="insertAt">Insert position.</param>
        /// <param name="count">The number of entities to be inserted.</param>
        public void Insert(int insertAt, int count)
        {
            trackDCC.Insert(insertAt + start, count);
        }

        /// <summary>
        /// Removes enities from the collection.
        /// </summary>
        /// <param name="removeAt">Index of the first entity to be removed.</param>
        /// <param name="count">The number of entities to be removed.</param>
        public void Remove(int removeAt, int count)
        {
            trackDCC.Remove(removeAt + start, count);
        }

        #endregion

        #region IDistancesHost Members

        IDistanceCounterCollection IDistancesHost.Distances
        {
            get
            {
                return this;
            }
        }

        #endregion

        public void Dispose()
        {

        }
    }

}
