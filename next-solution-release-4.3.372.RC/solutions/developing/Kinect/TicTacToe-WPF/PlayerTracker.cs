// -----------------------------------------------------------------------
// <copyright file="PlayerTracker.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Kinect;

    /// <summary>
    /// Keeps track of players interacting with Kinect sensor.
    /// </summary>
    /// <typeparam name="TPlayer">
    /// Type used to represent a Kinect player.
    /// </typeparam>
    public class PlayerTracker<TPlayer> where TPlayer : IPlayer
    {
        /// <summary>
        /// Player factory used to create players as we need to start tracking them.
        /// </summary>
        private readonly IPlayerFactory<TPlayer> playerFactory;

        /// <summary>
        /// Mapping of skeleton tracking IDs to players being currently tracked.
        /// </summary>
        private readonly Dictionary<int, TPlayer> id2PlayerMap = new Dictionary<int, TPlayer>();
 
        /// <summary>
        /// Kinect sensor that is currently streaming data to us.
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Array used to hold skeletons from Kinect data frame. Kept as a member variable to prevent
        /// unnecessary allocations.
        /// </summary>
        private Skeleton[] skeletonData;

        /// <summary>
        /// Filter used to decide which skeletons to track, when there are too many skeletons in scene
        /// to be tracked all at once.
        /// </summary>
        private ISkeletonFilter skeletonFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerTracker{TPlayer}" /> class.
        /// </summary>
        /// <param name="factory">
        /// Player factory used to create players as we need to start tracking them. May NOT be null.
        /// </param>
        public PlayerTracker(IPlayerFactory<TPlayer> factory)
        {
            this.playerFactory = factory;
        }

        /// <summary>
        /// Event triggered when new players join, active players leave or active players have new data.
        /// </summary>
        public event EventHandler<PlayerStatusEventArgs<TPlayer>> PlayerStatusChanged;

        /// <summary>
        /// Gets and sets filter used to decide which skeletons to track, when there are too many
        /// skeletons in scene to be tracked all at once.
        /// </summary>
        public ISkeletonFilter SkeletonFilter
        {
            get
            {
                return this.skeletonFilter;
            }
            
            set
            {
                this.skeletonFilter = value;
                this.EnsureSkeletonChoiceState();
            }
        }

        /// <summary>
        /// Perform necessary initialization steps associated with specified Kinect sensor.
        /// </summary>
        /// <param name="newSensor">
        /// Sensor to initialize with required parameters.
        /// </param>
        public void InitializeSensor(KinectSensor newSensor)
        {
            this.playerFactory.InitializeSensor(newSensor);

            this.UninitializeSensor();

            this.sensor = newSensor;

            if (null != newSensor)
            {
                this.sensor.AllFramesReady += this.AllSensorFramesReady;

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

                this.EnsureSkeletonChoiceState();
            }
        }

        /// <summary>
        /// Perform necessary uninitialization steps associated with specified Kinect sensor.
        /// </summary>
        public void UninitializeSensor()
        {
            // Let client know that all players have left
            foreach (TPlayer player in id2PlayerMap.Values)
            {
                OnPlayerStatusChanged(PlayerStatus.Left, player);
            }

            id2PlayerMap.Clear();

            if (null != this.sensor)
            {
                this.sensor.AllFramesReady -= this.AllSensorFramesReady;
                this.sensor = null;
            }

            this.playerFactory.UninitializeSensor();
        }

        /// <summary>
        /// Finds the active player whose position is closest to specified angle.
        /// </summary>
        /// <param name="angle">
        /// Angle in X-Z plane (in degrees) relative to the direction towards which
        /// the kinect sensor is looking.
        /// When facing the Kinect:
        /// 0: center
        /// positive angles: right
        /// negative angles: left
        /// </param>
        /// <returns>
        /// Active player whose skeleton position is closest to specified angle.
        /// May be null if no matching players were found.
        /// </returns>
        public TPlayer GetClosestPlayer(double angle)
        {
            // Maximum difference between player position and specified angle that is
            // still considered a player match
            const double MaximumAngle = 25.0;

            // Multiplication factor used to convert a value in radians to a value in degrees
            const double RadiansToDegreesMultiplier = 180.0 / Math.PI;

            TPlayer closest = default(TPlayer);

            double angleDiff = double.MaxValue;

            foreach (var candidate in id2PlayerMap.Values)
            {
                // Get head position if tracked, otherwise use average body position
                var position = candidate.Skeleton.Position;
                var headJoint = candidate.Skeleton.Joints[JointType.Head];
                if (JointTrackingState.Tracked == headJoint.TrackingState)
                {
                    position = headJoint.Position;
                }

                double candidateAngle = RadiansToDegreesMultiplier * Math.Atan2(position.X, position.Z);
                double candidateDiff = Math.Abs(angle - candidateAngle);

                if ((candidateDiff < MaximumAngle) && (candidateDiff < angleDiff))
                {
                    angleDiff = candidateDiff;
                    closest = candidate;
                }
            }

            return closest;
        }

        /// <summary>
        /// Ensure that skeleton ID tracking mode in Kinect sensor matches what we need.
        /// </summary>
        /// <remarks>
        /// If no SkeletonFilter is set, we use the default skeleton tracking.
        /// </remarks>
        private void EnsureSkeletonChoiceState()
        {
            if (null != this.sensor)
            {
                this.sensor.SkeletonStream.AppChoosesSkeletons = null != this.SkeletonFilter;
            }
        }

        /// <summary>
        /// Specifies to the Kinect sensor which skeleton tracking IDs should be chosen for tracking.
        /// </summary>
        /// <param name="skeletons">
        /// Set of skeletons that we'd like to track.
        /// </param>
        private void ChooseSkeletons(IEnumerable<Skeleton> skeletons)
        {
            if ((null == this.sensor) || !this.sensor.SkeletonStream.IsEnabled)
            {
                return;
            }

            int? id1 = null;
            int? id2 = null;

            // Ensure that the skeletons to be chosen are in fact being tracked
            foreach (var skeleton in skeletons.Where(skeleton => SkeletonTrackingState.NotTracked != skeleton.TrackingState))
            {
                if (!id1.HasValue)
                {
                    id1 = skeleton.TrackingId;
                    continue;
                }

                id2 = skeleton.TrackingId;
                break;
            }

            // Choose as many skeletons as are being tracked and also passed the user-supplied filter
            if (!id1.HasValue)
            {
                this.sensor.SkeletonStream.ChooseSkeletons();
                return;
            }

            if (!id2.HasValue)
            {
                this.sensor.SkeletonStream.ChooseSkeletons(id1.Value);
                return;
            }

            this.sensor.SkeletonStream.ChooseSkeletons(id1.Value, id2.Value);
        }

        /// <summary>
        /// Process skeleton data associated with an AllFramesReady event.
        /// </summary>
        /// <param name="skeletons">
        /// Skeletons to process.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void ProcessSkeletons(Skeleton[] skeletons, AllFramesReadyEventArgs e)
        {
            IEnumerable<Skeleton> filteredSkeletons = skeletons;
            if (null != SkeletonFilter)
            {
                filteredSkeletons = SkeletonFilter.Filter(skeletons).ToList();
                ChooseSkeletons(filteredSkeletons);
            }

            var idsSeen = new HashSet<int>();

            foreach (Skeleton skeleton in filteredSkeletons)
            {
                if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                {
                    TPlayer player;

                    // Determine if we're already tracking this player
                    if (id2PlayerMap.ContainsKey(skeleton.TrackingId))
                    {
                        player = id2PlayerMap[skeleton.TrackingId];
                    }
                    else
                    {
                        // If we're not yet tracking player, create one and let client know that
                        // a new player has started interacting with Kinect sensor.
                        player = playerFactory.Create();

                        id2PlayerMap.Add(skeleton.TrackingId, player);

                        OnPlayerStatusChanged(PlayerStatus.Joined, player);
                    }

                    // Update player data and let client know that player has been changed
                    player.Update(skeleton, e);

                    OnPlayerStatusChanged(PlayerStatus.Updated, player);

                    idsSeen.Add(skeleton.TrackingId);
                }
            }

            // Check if any players have left the scene
            var leftScene = new HashSet<int>();
            foreach (int tracked in id2PlayerMap.Keys)
            {
                if (!idsSeen.Contains(tracked))
                {
                    leftScene.Add(tracked);
                }
            }

            // Let client know if any players have stopped interacting with Kinect sensor
            foreach (int id in leftScene)
            {
                var leavingPlayer = id2PlayerMap[id];
                OnPlayerStatusChanged(PlayerStatus.Left, leavingPlayer);
                id2PlayerMap.Remove(id);
                playerFactory.Cleanup(leavingPlayer);
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's AllFramesReady event.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void AllSensorFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            bool haveSkeletonData = false;

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((this.skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                    haveSkeletonData = true;
                }
            }

            if (haveSkeletonData)
            {
                try
                {
                    this.ProcessSkeletons(this.skeletonData, e);
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while processing skeleton data.
                    // E.g.: sensor might be unplugged.
                } 
            }
        }

        /// <summary>
        /// Helper method that invokes PlayerStatusChanged event if there are any event subscribers registered.
        /// </summary>
        /// <param name="status">
        /// PlayerStatus we want to communicate to registered event handlers.
        /// </param>
        /// <param name="player">
        /// Player corresponding to specified status.
        /// </param>
        private void OnPlayerStatusChanged(PlayerStatus status, TPlayer player)
        {
            if (PlayerStatusChanged != null)
            {
                PlayerStatusChanged(this, new PlayerStatusEventArgs<TPlayer> { Status = status, Player = player });
            }
        }
    }
}
