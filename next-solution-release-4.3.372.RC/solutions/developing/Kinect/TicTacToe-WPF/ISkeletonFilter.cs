// -----------------------------------------------------------------------
// <copyright file="ISkeletonFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System.Collections.Generic;
    using Microsoft.Kinect;

    /// <summary>
    /// Used to filter a set of skeletons into a subset of interest.
    /// </summary>
    public interface ISkeletonFilter
    {
        /// <summary>
        /// Filters the specified enumerable set of skeletons to obtain a smaller subset of interest.
        /// </summary>
        /// <param name="skeletons">
        /// Enumerable set of skeletons to be filtered.
        /// </param>
        /// <returns>
        /// Enumerable set of skeletons output by filtering operation.
        /// </returns>
        IEnumerable<Skeleton> Filter(IEnumerable<Skeleton> skeletons);
    }
}
