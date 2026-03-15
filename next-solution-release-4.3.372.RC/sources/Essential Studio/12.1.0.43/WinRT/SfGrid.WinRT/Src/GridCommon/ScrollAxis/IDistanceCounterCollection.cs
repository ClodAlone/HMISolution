#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Collections;

namespace Syncfusion.UI.Xaml.ScrollAxis
{
    /// <summary>
    /// A collection of entities for which distances need to counted. The
    /// collection provides methods for mapping from a distance position to
    /// an entity and vice versa.<para/>
    /// For example, in a scrollable grid control you have rows with different heights. 
    /// Use this collection to determine the total height for all rows in the grid,
    /// quickly detemine the row index for a given point and also quickly determine
    /// the point at which a row is displayed. This also allows a mapping between the 
    /// scrollbars value and the rows or columns associated with that value.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface IDistanceCounterCollection
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// The raw number of entities (lines, rows or columns).
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// The default distance (row height or column width) an entity spans 
        /// </summary>
        double DefaultDistance { get; set; }

        /// <summary>
        /// The total distance all entities span (e.g. total height of all rows in grid)
        /// </summary>
        double TotalDistance { get; }

        /// <summary>
        /// Hides a specified range of entities (lines, rows or colums)
        /// </summary>
        /// <param name="from">The index for the first entity&gt;</param>
        /// <param name="to">The raw index for the last entity</param>
        /// <param name="distance">The distance.</param>
        void SetRange(int from, int to, double distance);

        /// <summary>
        /// Resets the range by restoring the default distance
        /// for all entries in the specified range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        void ResetRange(int from, int to);

        /// <summary>
        /// Gets or sets the distance for an entity.
        /// </summary>
        /// <param name="index">The index for the entity</param>
        /// <returns></returns>
        double this[int index] { get; set; }

        /// <summary>
        /// Skip subsequent entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetNextVisibleIndex(int index);

        /// <summary>
        /// Skip previous entities for which the distance is 0.0 and return the next entity.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetPreviousVisibleIndex(int index);

        /// <summary>
        /// Gets the index of an entity in this collection for which
        /// the cumulated count of previous distances is greater or equal
        /// the specified cumulatedDistance. (e.g. return row index for
        /// pixel position).
        /// </summary>
        /// <param name="cumulatedDistance">The cumulated count of previous distances.</param>
        /// <returns>The entity index.</returns>
        int IndexOfCumulatedDistance(double cumulatedDistance);

        /// <summary>
        /// Gets the cumulated count of previous distances for the
        /// entity at the specifiec index. (e.g. return pixel position
        /// for a row index).
        /// </summary>
        /// <param name="index">The entity index.</param>
        /// <returns>The cumulated count of previous distances for the
        /// entity at the specifiec index.</returns>
        double GetCumulatedDistanceAt(int index);

        /// <summary>
        /// Assigns a collection with nested entities to an item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedCollection">The nested collection.</param>
        void SetNestedDistances(int index, IDistanceCounterCollection nestedCollection);

        /// <summary>
        /// Gets the nested entities at a given index. If the index does not hold
        /// a mested distances collection the method returns null.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The nested collection or null.</returns>
        IDistanceCounterCollection GetNestedDistances(int index);

        /// <summary>
        /// Gets the distance position of the next entity after a given point. 
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance position.</returns>
        double GetNextScrollValue(double point);

        /// <summary>
        /// Gets the distance position of the entity preceeding a given point. If the point
        /// is in between entities the starting point of the matching entity
        /// is returned.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance position.</returns>
        double GetPreviousScrollValue(double point);

        /// <summary>
        /// Gets the aligned scroll value which is the starting point of the entity
        /// found at the given distance position.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The starting point of the entity.</returns>
        double GetAlignedScrollValue(double point);

        /// <summary>
        /// Connects a nested distance collection with a parent.
        /// </summary>
        /// <param name="treeTableCounterSource">The nested tree table visible counter source.</param>
        void ConnectWithParent(ITreeTableCounterSource treeTableCounterSource);

        /// <summary>
        /// Insert entities in the collection.
        /// </summary>
        /// <param name="insertAt">Insert position.</param>
        /// <param name="count">The number of entities to be inserted.</param>
        void Insert(int insertAt, int count);

        /// <summary>
        /// Removes enities from the collection.
        /// </summary>
        /// <param name="removeAt">Index of the first entity to be removed.</param>
        /// <param name="count">The number of entities to be removed.</param>
        void Remove(int removeAt, int count);
    }
}
