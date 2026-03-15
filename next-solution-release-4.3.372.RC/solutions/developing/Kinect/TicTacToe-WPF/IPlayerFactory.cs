// -----------------------------------------------------------------------
// <copyright file="IPlayerFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using Microsoft.Kinect;

    /// <summary>
    /// A factory of players interacting with a Kinect sensor.
    /// </summary>
    /// <typeparam name="TPlayer">
    /// Type used to represent a Kinect player.
    /// </typeparam>
    public interface IPlayerFactory<TPlayer> where TPlayer : IPlayer
    {
        /// <summary>
        /// Create a new instance of a Kinect player.
        /// </summary>
        /// <returns>
        /// A new Kinect player.
        /// </returns>
        TPlayer Create();

        /// <summary>
        /// Clean resources associated with specified player.
        /// </summary>
        /// <param name="player">
        /// Player whose associated resources should be cleaned up.
        /// </param>
        void Cleanup(TPlayer player);

        /// <summary>
        /// Perform necessary initialization steps associated with specified Kinect sensor.
        /// </summary>
        /// <param name="newSensor">
        /// Sensor to initialize with required parameters.
        /// </param>
        void InitializeSensor(KinectSensor newSensor);

        /// <summary>
        /// Perform necessary uninitialization steps associated with specified Kinect sensor.
        /// </summary>
        void UninitializeSensor();
    }
}
