////////////////////////////////////////////////
// DESCRIPTION:
//    BouncyViewWindow
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
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using FusionWare.SPOT.Bouncy;

namespace MFRuntime.UI
{
    /// <summary>Main Window for the Bouncy sample application</summary>
    /// <remarks>
    /// Creates a child view for the Bouncy "Universe" view and
    /// periodically triggers updates to the BouncyUniverseView
    /// to provide for animation. The window handles pausing and resuming
    /// the animation when the "select" button is pressed. This essentially
    /// implements and demonstrates a model view controller design pattern
    /// where the <see cref="BouncyUniverse">BouncyUniverse</see> and
    /// <see cref="BouncyBall">BouncyBall</see> are the Model,
    /// <see cref="BouncyView">BouncyView</see> serves as the view and
    /// BouncyViewWindow acts as the controller.
    /// </remarks>
    internal sealed class BouncyViewWindow : PresentationWindow
    {
        private Bitmap[] BallBitmaps;

        /// <summary> Constructor for the window</summary>
        public BouncyViewWindow(Program program)
            : base(program)
        {
            Color bkgColor = Color.White;
            BallBitmaps = InitBallBitMaps(bkgColor);
            this.Child = new BouncyView(BallBitmaps, bkgColor, Program.SmallFont);
            StartTimer();
        }

        #region Window Overrides
        /// <summary>Pauses/plays the animation when "Select" button pressed</summary>
        /// <param name="e">ButtonEventArgs describing the button event causing this event</param>
        /// <remarks>
        /// While the window itself does not have the input focus the button event
        /// "bubbles" up to the window to handle
        /// </remarks>
        protected override void OnButtonDown(Microsoft.SPOT.Input.ButtonEventArgs e)
        {
            base.OnButtonDown(e);
            if (e.Button == Microsoft.SPOT.Hardware.Button.VK_SELECT)
                ToggleTimer();
        }
        #endregion

        /// <summary>Initializes the ball bitmap images alpha blending them against the background color</summary>
        /// <param name="BkgColor">Background color to use for the alpha blend</param>
        /// <returns>Bitmaps for all of the balls</returns>
        /// <remarks>
        /// The Micro Framework does not have any direct support for
        /// alpha blend compositing so it has to be done in managed code,
        /// which is a bit of an expensive process in terms of CPU cycles.
        /// So the blending is done once to composite to the background color
        /// of the "universe". After that is done the pure background color
        /// parts can be marked as a transparency map to get the actual balls
        /// to render nice and smooth. This trick only works with a solid color 
        /// background. 
        /// </remarks>
        private static Bitmap[] InitBallBitMaps(Color BkgColor)
        {
            // Alpha blend the bitmaps onto the background color
            Bitmap[] bitmaps = new Bitmap[5];
            bitmaps[0] = Resources.GetBitmap(Resources.BitmapResources.YellowBall);
            bitmaps[1] = Resources.GetBitmap(Resources.BitmapResources.BlueBall);
            bitmaps[2] = Resources.GetBitmap(Resources.BitmapResources.GreenBall);
            bitmaps[3] = Resources.GetBitmap(Resources.BitmapResources.RedBall);
            bitmaps[4] = Resources.GetBitmap(Resources.BitmapResources.Logo_32);

            foreach(Bitmap bmp in bitmaps)
                bmp.MakeTransparent(bmp.GetPixel(0, 0));

            // now mark the background color as transparent so the ball images look smooth
            for(int i = 0; i < bitmaps.Length; ++i)
                bitmaps[i].MakeTransparent(BkgColor);

            return bitmaps;
        }

        #region DispatchTimer as Timer
        private DispatcherTimer Timer1;
        void StartTimer()
        {
            this.Timer1 = new DispatcherTimer();
            this.Timer1.Interval = new TimeSpan( 55 * TimeSpan.TicksPerMillisecond );
            this.Timer1.Tick += new EventHandler( this.TimerCallback );
            this.Timer1.Start();
        }

        void ToggleTimer()
        {
            if (Timer1.IsEnabled)
                Timer1.Stop();
            else
                Timer1.Start();
        }

        private void TimerCallback( object sender, EventArgs e )
        {
            BouncyView view = this.Child as BouncyView;
            view.UpdateTick();
        }
        #endregion
    }
}
