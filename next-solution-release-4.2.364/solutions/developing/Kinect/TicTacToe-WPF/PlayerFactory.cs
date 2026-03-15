// -----------------------------------------------------------------------
// <copyright file="PlayerFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Windows;

    using Microsoft.Kinect;

    /// <summary>
    /// Factory in charge of creating and updating Kinect players as the
    /// PlayerTracker sees the need to.
    /// </summary>
    public class PlayerFactory : IPlayerFactory<Player>
    {
        /// <summary>
        /// Create a new instance of a Kinect player.
        /// </summary>
        /// <returns>
        /// A new Kinect player.
        /// </returns>
        public Player Create()
        {
            return new Player();
        }

        /// <summary>
        /// Clean resources associated with specified player.
        /// </summary>
        /// <param name="player">
        /// Player whose associated resources should be cleaned up.
        /// </param>
        public void Cleanup(Player player)
        {
        }

        /// <summary>
        /// Perform necessary initialization steps associated with specified Kinect sensor.
        /// </summary>
        /// <param name="newSensor">
        /// Sensor to initialize with required parameters.
        /// </param>
        public void InitializeSensor(KinectSensor newSensor)
        {
            this.UninitializeSensor();

            if (null != newSensor)
            {
                // Ensure depth stream is enabled to be able to use image frame mapping functionality
                newSensor.DepthStream.Enable();

                // Ensure color stream is enabled to be able to get color format for mapping
                newSensor.ColorStream.Enable();
            }
        }

        /// <summary>
        /// Perform necessary uninitialization steps associated with specified Kinect sensor.
        /// </summary>
        public void UninitializeSensor()
        {
        }
    }
}
