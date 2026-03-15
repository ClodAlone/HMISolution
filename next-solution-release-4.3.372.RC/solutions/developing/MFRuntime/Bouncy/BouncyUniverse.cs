////////////////////////////////////////////////
// DESCRIPTION:
//    non-gui Bouncy universe
//
// Legal Notices:
//   Copyright (c) 2008, Telliam Consulting, LLC.
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without modification,
//   are permitted provided that the following conditions are met:
//
//   * Redistributions of source code must retain the above copyright notice, this list
//     of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright notice, this
//     list of conditions and the following disclaimer in the documentation and/or other
//     materials provided with the distribution.
//   * Neither the name of Telliam Consulting nor the names of its contributors may be
//     used to endorse or promote products derived from this software without specific
//     prior written permission.
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//   EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//   OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
//   SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//   INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
//   TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
//   BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//   CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
//   ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//   DAMAGE. 
//

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using System;

namespace FusionWare.SPOT.Bouncy
{
    /// <summary>This is the non-GUI representation of the "universe"</summary>
    /// <remarks>
    /// This class provides the underlying representation of the universe
    /// (E.g. the data model) It has no UI of it's own so that it can be used
    /// with various themed UI's or none at all for basic simulation. While this
    /// is mostly an academic point in this simnple sample application it does
    /// illustrate a valuable design strucutre that enables better unit testing
    /// and eases changes in the UI. (Afterall marketing just loves to change the UI at the
    /// last minute!)
    /// 
    /// The physics of this universe are pretty simplistic. Collision with a wall
    /// will alter the angle and randomly alter the speed. Collision between "balls"
    /// is "detected" when the distance between any 2 "balls" is less than the sum
    /// of their radii. The reaction to 2 balls colliding is to reverse the direction
    /// of both balls. This can lead to "interesting" patterns where both were moving
    /// so fast that a collision isn't detected until they are "on top of each other"
    /// and thus end up stuck together as they don't get away from each other before
    /// being detected as a collision again.
    /// 
    /// More realistic physics are left as an excercise for the reader 8^)
    /// </remarks>
    public class BouncyUniverse
    {
        /// <summary>Creates a new universe</summary>
        /// <param name="MaxDelta">Maximum pixel position delta (Speed) of a "ball"</param>
        /// <remarks>
        /// The Maximum delta is used in random number generation for handling collisions
        /// it essentially is the maximum speed of a ball.
        /// </remarks>
        public BouncyUniverse(int MaxDelta)
        {
            // make it always an odd max so that an even number
            // of integers lies on either side of 0
            if(1 == (this.MaxDelta & 1))
                this.MaxDelta = MaxDelta;
            else
                this.MaxDelta = MaxDelta - 1;

            // save the /2 version as well to keep things
            // faster in tight loops
            this.MaxDelta2 = MaxDelta >> 1;
        }
        
        #region Public Properties

        /// <summary>Height of the Universe</summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        private int _Height;
        
        /// <summary>Width of the Universe</summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        private int _Width;

        /// <summary>Number of frame ticks that have occured since the last call to InitializeScene</summary>
        public long FrameCount { get { return _FrameCount; } }
        private long _FrameCount;
        
        /// <summary>Array of Balls in the universe</summary>
        public BouncyBall[] Balls { get { return this._Balls; } }
        private BouncyBall[] _Balls;
        #endregion 

        /// <summary>Initializes the universe</summary>
        /// <param name="Width">Width of the universe</param>
        /// <param name="Height">Height of the universe</param>
        /// <param name="Balls">Array of balls to place in the universe</param>
        /// <remarks>
        /// This method picks a random start position and speed for each ball.
        /// The radius and Tag values for the ball are left alone so that the
        /// application can set the radius to match the size of a given bitmap
        /// and allows for different size balls.
        /// 
        /// To better integrate with UI code this is not done as part of the constructor
        /// as the width and height of <see cref="T:Microsoft.SPOT.Presentation.UIElement">UIElements</see>
        /// are not known at construction time. This also allows re-initialization periodically
        /// to keep things interesting and clear up "stuck" balls from the simplistic physics
        /// modeled in this class.
        /// </remarks>
        public void InitializeScene(int Width, int Height, BouncyBall[] Balls)
        {
            this._Height = Height;
            this._Width = Width;
            this._FrameCount = 0;
            this._Balls = Balls;

            Offsets = new Point[Balls.Length];
            for(int i = 0; i < Offsets.Length; ++i)
            {
                Offsets[i] = RandomOffset(true);
                this._Balls[i].MoveCenterToAbs(RandomPoint());
            }
        }

        /// <summary>Updates the location of balls based on their speed and detects collisions</summary>
        /// <remarks>
        /// This method works through the array of balls and moves each one based on its current speed.
        /// After moving the balls it checks for collisions with other balls and the outer wall edges
        /// and adjusts positions and/or speed accordingly. 
        /// </remarks>
        public void UpdateTick()
        {
            Microsoft.SPOT.Math.Randomize();

            for(int i = 0; i < Balls.Length; ++i)
                Balls[i].MoveCenterRel(Offsets[i]);

            for(int i = 0; i < Balls.Length; ++i)
            {
                CollisionTest(i);
                HitTestEdge(i);
            }

            ++_FrameCount;
        }

        #region Random Point support
        Point RandomPoint()
        {
            return new Point(Microsoft.SPOT.Math.Random(Width), Microsoft.SPOT.Math.Random(Height));
        }

        Point RandomOffset(bool AllowNegative)
        {
            Point retVal;

            if(AllowNegative)
            {
                retVal = new Point(Microsoft.SPOT.Math.Random(this.MaxDelta) - this.MaxDelta2
                                  , Microsoft.SPOT.Math.Random(this.MaxDelta) - this.MaxDelta2
                                  );
            }
            else
                retVal = new Point(Microsoft.SPOT.Math.Random(this.MaxDelta), Microsoft.SPOT.Math.Random(this.MaxDelta));

            return retVal;
        }
        #endregion

        #region Internal Physics Methods
        // if it hit another ball then adjust both directions.
        bool CollisionTest(int i)
        {
            bool retVal = false;
            for(int j = i + 1; j < Balls.Length; ++j)
            {
                if(Balls[i].Position.DistanceFrom(Balls[j].Position) < (Balls[i].Radius + Balls[j].Radius))
                {
                    Offsets[i].x = -Offsets[i].x;
                    Offsets[i].y = -Offsets[i].y;
                    Offsets[j].x = -Offsets[j].x;
                    Offsets[j].y = -Offsets[j].y;
                    retVal = true;
                }
            }

            return retVal;
        }

        // if it hit an edge then adjust it's direction
        bool HitTestEdge(int i)
        {
            bool retVal = false;
            Point ballPos = Balls[i].Position;
            int radius = Balls[i].Radius;
            Point offset = RandomOffset(false);

            if(ballPos.x + radius >= this.Width)
            {
                retVal = true;
                Offsets[i].x = -offset.x;

                // If it's not going in one direction let it keep going
                // E.g. If it's going in one direct don't let it stay that
                // way on an edge hit.
                if(Offsets[i].y == 0)
                    Offsets[i].y = offset.y;
            }

            if(ballPos.y + radius >= this.Height)
            {
                retVal = true;
                Offsets[i].y = -offset.y;

                // Don't let it keep going in one direction 
                if(Offsets[i].x == 0)
                    Offsets[i].y = offset.x;
            }

            if(ballPos.x <= radius)
            {
                retVal = true;
                Offsets[i].x = offset.x;

                // Don't let it keep going in one direction 
                if(Offsets[i].y == 0)
                    Offsets[i].y = offset.y;
            }

            if(ballPos.y <= radius)
            {
                retVal = true;
                Offsets[i].y = offset.y;

                // Don't let it keep going in one direction 
                if(Offsets[i].x == 0)
                    Offsets[i].y = offset.x;
            }

            // don't let it stand still...
            while(retVal && Offsets[i].x == 0 && Offsets[i].y == 0)
                Offsets[i] = RandomOffset(true);

            return retVal;
        }
        #endregion

        #region Private data members
        private Point[] Offsets;
        private int MaxDelta;
        private int MaxDelta2;
        #endregion
    }
}