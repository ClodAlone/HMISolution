// -----------------------------------------------------------------------
// <copyright file="ClosestSkeletonFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System.Collections.Generic;

    using Microsoft.Kinect;

    /// <summary>
    /// ISkeletonFilter implementation that keeps skeletons closest to Kinect sensor.
    /// </summary>
    public class ClosestSkeletonFilter : ISkeletonFilter
    {
        /// <summary>
        /// Default number of skeletons to keep after filtering operation.
        /// </summary>
        private const int DefaultKeepCount = 2;

        /// <summary>
        /// Maximum number of skeletons to keep after filtering operation.
        /// </summary>
        private int keepCount = DefaultKeepCount;

        /// <summary>
        /// Maximum number of skeletons to keep after filtering operation.
        /// </summary>
        public int KeepCount
        {
            get
            {
                return keepCount;
            }

            set
            {
                keepCount = value;
            }
        }

        /// <summary>
        /// Filters the specified enumerable set of skeletons to obtain a smaller subset of interest.
        /// </summary>
        /// <param name="skeletons">
        /// Enumerable set of skeletons to be filtered.
        /// </param>
        /// <returns>
        /// Enumerable set of skeletons output by filtering operation.
        /// </returns>
        public IEnumerable<Skeleton> Filter(IEnumerable<Skeleton> skeletons)
        {
            // Amount to added to skeleton depth to differentiate from other skeletons in case of
            // depth value collisions.
            const float DepthCollisionOffset = 0.0001f;

            var depthSorted = new SortedList<float, Skeleton>();

            if (null == skeletons)
            {
                return null;
            }

            foreach (Skeleton s in skeletons)
            {
                if (s.TrackingState != SkeletonTrackingState.NotTracked)
                {
                    float valueZ = s.Position.Z;
                    while (depthSorted.ContainsKey(valueZ))
                    {
                        // Avoid collisions
                        valueZ += DepthCollisionOffset;
                    }

                    depthSorted.Add(valueZ, s);
                }
            }

            // Truncate list of returned skeletons to desired size
            while (depthSorted.Count > KeepCount)
            {
                depthSorted.RemoveAt(KeepCount);
            }

            return depthSorted.Values;
        }
    }
}
