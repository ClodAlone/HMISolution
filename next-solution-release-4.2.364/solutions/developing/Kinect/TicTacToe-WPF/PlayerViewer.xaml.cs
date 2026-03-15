// -----------------------------------------------------------------------
// <copyright file="PlayerViewer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Kinect;

    /// <summary>
    /// Control used to render players.
    /// </summary>
    public partial class PlayerViewer : UserControl
    {
        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register(
                "Player",
                typeof(Player),
                typeof(PlayerViewer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                "PlayerSymbol",
                typeof(PlayerSymbol),
                typeof(PlayerViewer),
                new FrameworkPropertyMetadata(PlayerSymbol.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SymbolBrushProperty =
            DependencyProperty.Register(
                "PlayerSymbolBrush",
                typeof(Brush),
                typeof(PlayerViewer),
                new FrameworkPropertyMetadata(Brushes.Green, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerViewer"/> class.
        /// </summary>
        public PlayerViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Player associated with this viewer.
        /// </summary>
        public Player Player
        {
            get { return (Player)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        /// <summary>
        /// PlayerSymbol associated with this viewer.
        /// </summary>
        public PlayerSymbol Symbol
        {
            get { return (PlayerSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// Brush used to draw symbol.
        /// </summary>
        public Brush SymbolBrush
        {
            get { return (Brush)GetValue(SymbolBrushProperty); }
            set { SetValue(SymbolBrushProperty, value); }
        }

        /// <summary>
        /// Renders associated player data.
        /// </summary>
        /// <param name="drawingContext">
        /// The DrawingContext used to draw player data.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            // Don't render if we don't have a player
            if (null == Player)
            {
                return;
            }

            var currentSkeleton = Player.Skeleton;

            // Don't render if we don't have a skeleton, or it isn't tracked
            if ((null == currentSkeleton) || (currentSkeleton.TrackingState != SkeletonTrackingState.Tracked))
            {
                return;
            }

            this.DrawSymbolHat(drawingContext);
        }

        /// <summary>
        /// Renders symbol as a "hat" above (or around, if no space above) the head joint of associated
        /// skeleton.
        /// </summary>
        /// <param name="drawingContext">
        /// The DrawingContext used to draw player symbol.
        /// </param>
        private void DrawSymbolHat(DrawingContext drawingContext)
        {
            Joint shoulderCenter = Player.Skeleton.Joints[JointType.ShoulderCenter];
            Joint head = Player.Skeleton.Joints[JointType.Head];

            // If we can't find either of these joints, exit
            if ((null == Player.JointMapping) ||
                !Player.JointMapping.ContainsKey(JointType.ShoulderCenter) ||
                !Player.JointMapping.ContainsKey(JointType.Head) ||
                (shoulderCenter.TrackingState != JointTrackingState.Tracked) ||
                (head.TrackingState != JointTrackingState.Tracked))
            {
                return;
            }

            Point shoulderCenterMapping = Player.JointMapping[JointType.ShoulderCenter];
            Point headMapping = Player.JointMapping[JointType.Head];

            double directionX = headMapping.X - shoulderCenterMapping.X;
            double directionY = headMapping.Y - shoulderCenterMapping.Y;

            double neckSize = Math.Sqrt((directionX * directionX) + (directionY * directionY));
            var hatBounds = FindBestBounds(headMapping, neckSize);

            if (!hatBounds.HasValue)
            {
                return;
            }

            switch (Symbol)
            {
                case PlayerSymbol.XSymbol:
                case PlayerSymbol.OSymbol:
                    Symbol.Draw(new Pen(SymbolBrush, neckSize * Symbol.GetRecommendedThickness()), hatBounds.Value, drawingContext);
                    break;
            }
        }

        /// <summary>
        /// Finds the best bounds for "hat", trying preferred directions in turn until one of them works.
        /// </summary>
        /// <param name="head">
        /// Position of player's head, in 2-D coordinates.
        /// </param>
        /// <param name="neckSize">
        /// Length of player's neck (distance between shoulder center and head), in 2-D coordinates.
        /// </param>
        /// <returns>
        /// Rectangle that should be used as bounds for rendered hat symbol. May be null if no good placement was found.
        /// </returns>
        private Rect? FindBestBounds(Point head, double neckSize)
        {
            const double RelativeDistanceFromHead = 1.25;

            double halfNeckSize = neckSize / 2;
            double distance = RelativeDistanceFromHead * neckSize;

            // Calculate top position of bounding box given skeleton head position and overall UI bounds
            double top = Math.Max(0, head.Y - (distance + halfNeckSize));
            double distanceY = head.Y - (top + halfNeckSize);

            if (Math.Abs(distanceY) > distance)
            {
                // If bounding box ends up getting pushed farther than we initially wanted,
                // adjust expectations appropriately
                distance = distanceY;
            }

            // Calculate angle between 0 and Pi radians that represents direction towards bounding box center
            double angle = Math.Acos(distanceY / distance);
            
            // If bounding box fits in UI area, towards the right side of head, put it there
            double distanceX = distance * Math.Sin(angle);

            if (head.X + distanceX + halfNeckSize > this.ActualWidth)
            {
                // bounding box doesn't fit to the right side of head. See if it fits to the left side.
                if (head.X - (distanceX + halfNeckSize) < 0)
                {
                    // bounding box doesn't fit to the left side of head either, so we can't place it at all.
                    return null;
                }

                distanceX = -distanceX;
            }

            return new Rect(head.X + distanceX - halfNeckSize, top, neckSize, neckSize);
        }
    }
}
