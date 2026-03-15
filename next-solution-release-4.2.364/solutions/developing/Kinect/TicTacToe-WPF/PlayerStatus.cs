// -----------------------------------------------------------------------
// <copyright file="PlayerStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    /// <summary>
    /// Represents the status of a Kinect player.
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// A new player has started interacting with Kinect sensor.
        /// </summary>
        Joined,

        /// <summary>
        /// A player has stopped interacting with Kinect sensor.
        /// </summary>
        Left,

        /// <summary>
        /// Data for a current player has been updated.
        /// </summary>
        Updated
    }
}
