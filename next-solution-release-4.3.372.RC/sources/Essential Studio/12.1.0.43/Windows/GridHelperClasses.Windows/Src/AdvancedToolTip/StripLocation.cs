#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Intializes the gripbounds struct.
    /// </summary>
    internal struct GripBounds
    {
        private const int GripSize = 6;
        private const int CornerGripSize = GripSize << 1;

        /// <summary>
        /// Gets the client rectangle of the clientrectangle. 
        /// </summary>
        /// <param name="clientRectangle">client rectangle of the cripbounds.</param>
        public GripBounds(Rectangle clientRectangle)
        {
            this.clientRectangle = clientRectangle;
        }

        private Rectangle clientRectangle;
        /// <summary>
        /// Gets the clientrectangle of the gridbounds.
        /// </summary>
        public Rectangle ClientRectangle
        {
            get { return clientRectangle; }
        }

        /// <summary>
        /// Gets the Bottom rectangle area of the GripBounds.
        /// </summary>
        public Rectangle Bottom
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Y = rect.Bottom - GripSize + 1;
                rect.Height = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the BottomRight rectangle area of the GripBounds.
        /// </summary>
        public Rectangle BottomRight
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Y = rect.Bottom - CornerGripSize + 1;
                rect.Height = CornerGripSize;
                rect.X = rect.Width - CornerGripSize + 1;
                rect.Width = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the Top rectangle area of the GripBounds.
        /// </summary>
        public Rectangle Top
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Height = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the TopRight rectangle area of the GripBounds.
        /// </summary>
        public Rectangle TopRight
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Height = CornerGripSize;
                rect.X = rect.Width - CornerGripSize + 1;
                rect.Width = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the Left rectangle area of the GripBounds.
        /// </summary>
        public Rectangle Left
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Width = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the BottomLeft rectangle area of the GripBounds.
        /// </summary>
        public Rectangle BottomLeft
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Width = CornerGripSize;
                rect.Y = rect.Height - CornerGripSize + 1;
                rect.Height = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the Right rectangle area of the GripBounds.
        /// </summary>
        public Rectangle Right
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.X = rect.Right - GripSize + 1;
                rect.Width = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the TopLeft rectangle area of the GripBounds.
        /// </summary>
        public Rectangle TopLeft
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.Width = CornerGripSize;
                rect.Height = CornerGripSize;
                return rect;
            }
        }
    }
}
