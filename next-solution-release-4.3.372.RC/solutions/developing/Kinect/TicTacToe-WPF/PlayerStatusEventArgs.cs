// -----------------------------------------------------------------------
// <copyright file="PlayerStatusEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;

    /// <summary>
    /// Event arguments for PlayerTracker.PlayerStatusChanged event.
    /// </summary>
    /// <typeparam name="TPlayer">
    /// Type used to represent a Kinect player.
    /// </typeparam>
    public class PlayerStatusEventArgs<TPlayer> : EventArgs
    {
        /// <summary>
        /// Player whose status has changed.
        /// </summary>
        public TPlayer Player { get; set; }

        /// <summary>
        /// Current player status.
        /// </summary>
        public PlayerStatus Status { get; set; }
    }
}
