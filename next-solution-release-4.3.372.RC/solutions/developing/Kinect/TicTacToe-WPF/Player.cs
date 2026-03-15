// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Microsoft.Kinect;

    /// <summary>
    /// Represents a tracked tic tac toe player.
    /// </summary>
    public class Player : Freezable, IPlayer
    {
        public static readonly DependencyProperty SkeletonProperty =
            DependencyProperty.Register(
                "PlayerSkeleton",
                typeof(Skeleton),
                typeof(Player),
                new PropertyMetadata(null, null));

        /// <summary>
        /// Mapping between joint types and their corresponding 2-D coordinates in color image space.
        /// </summary>
        private readonly Dictionary<JointType, Point> joint2PointMap = new Dictionary<JointType, Point>();

        /// <summary>
        /// Last seen skeleton data for this player
        /// </summary>
        public Skeleton Skeleton
        {
            get
            {
                return (Skeleton)GetValue(SkeletonProperty);
            }

            set
            {
                if (value == GetValue(SkeletonProperty))
                {
                    // Even if the Skeleton instance being set is the same as the old skeleton instance,
                    // some of the Skeleton properties are most likely different than they were last time
                    // that Skeleton was set, so we force a property invalidation.
                    ClearValue(SkeletonProperty);
                    InvalidateProperty(SkeletonProperty);
                }

                SetValue(SkeletonProperty, value);
            }
        }

        /// <summary>
        /// Mapping between joint types and their corresponding 2-D coordinates in color image space.
        /// </summary>
        public Dictionary<JointType, Point> JointMapping
        {
            get
            {
                return joint2PointMap;
            }
        }

        /// <summary>
        /// Update player with data from Kinect sensor.
        /// </summary>
        /// <param name="skeleton">
        /// Skeleton data corresponding to player.
        /// </param>
        /// <param name="eventArgs">
        /// Event arguments corresponding to specified skeleton.
        /// </param>
        public void Update(Skeleton skeleton, AllFramesReadyEventArgs eventArgs)
        {
            Skeleton = skeleton;

            if ((null == skeleton) || (null == eventArgs))
            {
                return;
            }

            using (DepthImageFrame depthImageFrame = eventArgs.OpenDepthImageFrame())
            {
                using (ColorImageFrame colorImageFrame = eventArgs.OpenColorImageFrame())
                {
                    if ((null == depthImageFrame) || (null == colorImageFrame))
                    {
                        return;
                    }

                    JointMapping.Clear();

                    try
                    {
                        // Transform the skeleton coordinates into the color image space
                        foreach (Joint joint in skeleton.Joints)
                        {
                            Point mappedPoint = Get2DPosition(
                                depthImageFrame, joint.Position, colorImageFrame.Format);

                            JointMapping[joint.JointType] = mappedPoint;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Kinect is no longer available.
                    }
                }
            }
        }

        #region Freezable

        /// <summary>
        /// Creates a new instance of the Freezable derived class. 
        /// </summary>
        /// <returns>
        /// A new instance of Player class.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new Player();
        }

        /// <summary>
        /// Called as part of Freezable.Freeze() to determine whether this class can be frozen.
        /// We can freeze, but only if the KinectSensor is null.
        /// </summary>
        /// <returns>
        /// True if a Freeze is legal or has occurred, false otherwise.
        /// </returns>
        protected override bool FreezeCore(bool isChecking)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// Get 2-dimensional color image position corresponding to a 3-dimensional skeleton position.
        /// </summary>
        /// <param name="depthFrame">
        /// DepthImageFrame used to perform the coordinate space mapping.
        /// </param>
        /// <param name="skeletonPoint">
        /// Skeleton position to be mapped into color image space.
        /// </param>
        /// <param name="colorFormat">
        /// Color image format indicating size of destination coordinate space.
        /// </param>
        /// <returns>
        /// 2D point in color image space.
        /// </returns>
        private static Point Get2DPosition(
            DepthImageFrame depthFrame,
            SkeletonPoint skeletonPoint,
            ColorImageFormat colorFormat)
        {
            try
            {
#pragma warning disable 0618
                DepthImagePoint depthPoint = depthFrame.MapFromSkeletonPoint(skeletonPoint);
                ColorImagePoint colorPoint = depthFrame.MapToColorImagePoint(depthPoint.X, depthPoint.Y, colorFormat);
#pragma warning restore 0618

                // map back to skeleton.Width & skeleton.Height
                return new Point(colorPoint.X, colorPoint.Y);
            }
            catch (InvalidOperationException)
            {
                // The stream must have stopped abruptly
                // Handle this gracefully
                return new Point(0, 0);
            }
        }
    }
}
