// -----------------------------------------------------------------------
// <copyright file="IPlayer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using Microsoft.Kinect;

    /// <summary>
    /// An abstraction representing a player interacting with Kinect sensor.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Last seen skeleton data for this player.
        /// </summary>
        Skeleton Skeleton { get; }

        /// <summary>
        /// Update player with data from Kinect sensor.
        /// </summary>
        /// <param name="skeleton">
        /// Skeleton data corresponding to player.
        /// </param>
        /// <param name="eventArgs">
        /// Event arguments corresponding to specified skeleton.
        /// </param>
        void Update(Skeleton skeleton, AllFramesReadyEventArgs eventArgs);
    }
}
