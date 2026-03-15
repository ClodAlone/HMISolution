////////////////////////////////////////////////
// DESCRIPTION:
//    BouncyView
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
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;

namespace FusionWare.SPOT.Bouncy
{
    /// <summary>This panel derived view class is used to host the "Bouncy Universe" UI</summary>
    /// <remarks>
    /// The BouncyUniverse class contains no UI to keep things cleanly separated. This
    /// class provides the UI for the "Bouncy Universe". It is a simple UIElement
    /// that renders the complere universe scene within it's boundaries in OnRender.
    /// This is the simplest drawing mechanism available. A more fully WPF style
    /// approach would use a canvas with an Image UIElement for each of the "balls"
    /// in the universe and a TextFlow for the statistics. Ultimately for an app like
    /// this where all elements change on every "frame" the addition of the the
    /// overhead of the more WPF style is more problem then a help.
    /// </remarks>
    public class BouncyView : UIElement
    {
        /// <summary>Constructs a new BouncyView UIElement</summary>
        /// <param name="BallBitmaps">
        /// Bitmaps to use for the balls in the universe. The view will pick random entries
        /// from this array each time it is "Arranged"
        /// </param>
        /// <param name="BkgColor">
        /// Background color to use for the view. This should match the backgound the bitmaps
        /// are composited to in order to create a smooth effect. You can use
        /// <see cref="FusionWare.SPOT.BitmapHelper.CompositeOnColor">BitmapHalper.CompositeOnColor</see> 
        /// to perform the composition.
        /// </param>
        /// <param name="TextFont">Font to use for the statistics information.</param>
        public BouncyView(Bitmap[] BallBitmaps, Color BkgColor, Font TextFont)
        {
            this.Background = new SolidColorBrush(BkgColor);
            this.Balls = new BouncyBall[10];
            this.TextFont = TextFont;
            this.BallBitmaps = BallBitmaps;
        }

        /// <summary>
        /// Updates the universe in response to a time "tick" and invalidates the view
        /// contents to generate a render event.
        /// </summary>
        /// <remarks>
        /// This method is called in response to a timer event of some sort (provided by the
        /// hosting window). For each tick this method is called to update the underlying
        /// <see cref="BouncyUniverse">BouncyUniverse</see> and invalidate the View's contents.
        /// This causes the system to call the view's OnRender() function to render the updated
        /// scene.
        /// </remarks>
        public void UpdateTick()
        {
            // If the panel has not yet been arranged then the universe
            // isn't initialized since the width and height are not yet
            // known. Just ignore the update ticks until it is known 
            if(!this.Arranged)
                return;

            // every 1500 frames start fresh again with a new random scene.
            if(this.Universe.FrameCount > 1500)
                this.InvalidateArrange();
            else
            {
                this.Universe.UpdateTick();
                base.Invalidate();
            }
        }

        #region UIElement Overrides
        /// <summary>Adjusts the view's "arrangement"</summary>
        /// <param name="arrangeWidth">Width to arrange the view to</param>
        /// <param name="arrangeHeight">Height to arrange the view to</param>
        /// <remarks>
        /// This will re-initialize the universe "scene". This allows the
        /// re-initialization after a fixed number of frames to pick a new
        /// set of random bitmaps for the balls. It also helps prevent the scene 
        /// from getting "stuck" from the rather simplistic "physics" of the universe
        /// in this sample. (The collision "physics" code is pretty brain dead.)
        /// </remarks>
        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {
            Debug.Assert(arrangeWidth > 0 && arrangeHeight > 0);

            base.ArrangeOverride(arrangeWidth, arrangeHeight);
            this.StartTime = DateTime.Now;
            RandomizeBalls();
            this.Universe.InitializeScene(arrangeWidth, arrangeHeight, this.Balls);
            this.Arranged = true;
        }

        private void RandomizeBalls()
        {
            for(int i = 0; i < Balls.Length; ++i)
            {
                Bitmap img = BallBitmaps[Microsoft.SPOT.Math.Random(BallBitmaps.Length - 1)];
                int radius = System.Math.Max(img.Width, img.Height) / 2;

                Balls[i] = new BouncyBall(radius);
                Balls[i].Tag = img;
                
            }
        }

        /// <summary>Handles rendering the view</summary>
        /// <remarks>
        /// This method will fill the dc with the background color, render each individual
        /// ball in the universe in it's current position and then compute and draw the statistics
        /// information.
        /// </remarks>
        /// <param name="dc">Drawing Context to use to render the view.</param>
        public override void OnRender(DrawingContext dc)
        {
            // fill the entire background with the background color
            dc.DrawRectangle(this.Background, null, 0, 0, dc.Width, dc.Height);
            
            // draw each "ball" at it's current position
            foreach(BouncyBall ball in Universe.Balls)
                dc.DrawImage((Bitmap)ball.Tag, ball.Position.x - ball.Radius, ball.Position.y - ball.Radius);

            // compute frame rate
            TotalTime = DateTime.Now.Subtract(StartTime).Ticks / TimeSpan.TicksPerSecond;
            if(TotalTime > 0)
                FrameRate = this.Universe.FrameCount / TotalTime;
            
            // render the stats to the dc
            dc.DrawText("   Window", this.TextFont, Colors.Blue, 5, 10 );
            dc.DrawText("Frame: " + this.Universe.FrameCount.ToString(), this.TextFont, Colors.Blue, 5, 20);
            dc.DrawText("Rate: " + FrameRate.ToString() + "fps", this.TextFont, Colors.Blue, 5, 30);
        }
        #endregion

        BouncyUniverse Universe = new BouncyUniverse(15);
        private DateTime StartTime = DateTime.Now;
        private long TotalTime = 0;
        private long FrameRate = 0;
        private Brush Background = new SolidColorBrush(Color.White);
        private BouncyBall[] Balls = null;
        private Bitmap[] BallBitmaps;
        private Font TextFont;
        private bool Arranged = false;
    }
}