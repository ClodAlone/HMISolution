//------------------------------------------------------------------------------
// <copyright file="FallingShapes.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

// This module contains code to do display falling shapes, and do
// hit testing against a set of segments provided by the Kinect NUI, and
// have shapes react accordingly.

namespace ShapeGame.Utils
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Kinect;

    [Flags]
    public enum PolyType
    {
        None = 0x00,
        Triangle = 0x01,
        Square = 0x02,
        Star = 0x04,
        Pentagon = 0x08,
        Hex = 0x10,
        Star7 = 0x20,
        Circle = 0x40,
        Bubble = 0x80,
        All = 0x7f
    }

    [Flags]
    public enum HitType
    {
        None = 0x00,
        Hand = 0x01,
        Arm = 0x02,
        Squeezed = 0x04,
        Popped = 0x08
    }

    public enum GameMode
    {
        Off = 0,
        Solo = 1,
        TwoPlayer = 2
    }

    // For hit testing, a dictionary of BoneData items, keyed off the endpoints
    // of a segment (Bone) is used.  The velocity of these endpoints is estimated
    // and used during hit testing and updating velocity vectors after a hit.
    public struct Bone
    {
        public JointType Joint1;
        public JointType Joint2;

        public Bone(JointType j1, JointType j2)
        {
            this.Joint1 = j1;
            this.Joint2 = j2;
        }
    }

    public struct Segment
    {
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;
        public double Radius;

        public Segment(double x, double y)
        {
            this.Radius = 1;
            this.X1 = this.X2 = x;
            this.Y1 = this.Y2 = y;
        }

        public Segment(double x1, double y1, double x2, double y2)
        {
            this.Radius = 1;
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public bool IsCircle()
        {
            return (this.X1 == this.X2) && (this.Y1 == this.Y2);
        }
    }

    public struct BoneData
    {
        public Segment Segment;
        public Segment LastSegment;
        public double XVelocity;
        public double YVelocity;
        public double XVelocity2;
        public double YVelocity2;
        public DateTime TimeLastUpdated;

        private const double Smoothing = 0.8;

        public BoneData(Segment s)
        {
            this.Segment = this.LastSegment = s;
            this.XVelocity = this.YVelocity = 0;
            this.XVelocity2 = this.YVelocity2 = 0;
            this.TimeLastUpdated = DateTime.Now;
        }

        // Update the segment's position and compute a smoothed velocity for the circle or the
        // endpoints of the segment based on  the time it took it to move from the last position
        // to the current one.  The velocity is in pixels per second.
        public void UpdateSegment(Segment s)
        {
            this.LastSegment = this.Segment;
            this.Segment = s;
            
            DateTime cur = DateTime.Now;
            double fMs = cur.Subtract(this.TimeLastUpdated).TotalMilliseconds;
            if (fMs < 10.0)
            {
                fMs = 10.0;
            }

            double fps = 1000.0 / fMs;
            this.TimeLastUpdated = cur;

            if (this.Segment.IsCircle())
            {
                this.XVelocity = (this.XVelocity * Smoothing) + ((1.0 - Smoothing) * (this.Segment.X1 - this.LastSegment.X1) * fps);
                this.YVelocity = (this.YVelocity * Smoothing) + ((1.0 - Smoothing) * (this.Segment.Y1 - this.LastSegment.Y1) * fps);
            }
            else
            {
                this.XVelocity = (this.XVelocity * Smoothing) + ((1.0 - Smoothing) * (this.Segment.X1 - this.LastSegment.X1) * fps);
                this.YVelocity = (this.YVelocity * Smoothing) + ((1.0 - Smoothing) * (this.Segment.Y1 - this.LastSegment.Y1) * fps);
                this.XVelocity2 = (this.XVelocity2 * Smoothing) + ((1.0 - Smoothing) * (this.Segment.X2 - this.LastSegment.X2) * fps);
                this.YVelocity2 = (this.YVelocity2 * Smoothing) + ((1.0 - Smoothing) * (this.Segment.Y2 - this.LastSegment.Y2) * fps);
            }
        }

        // Using the velocity calculated above, estimate where the segment is right now.
        public Segment GetEstimatedSegment(DateTime cur)
        {
            Segment estimate = this.Segment;
            double fMs = cur.Subtract(this.TimeLastUpdated).TotalMilliseconds;
            estimate.X1 += fMs * this.XVelocity / 1000.0;
            estimate.Y1 += fMs * this.YVelocity / 1000.0;
            if (this.Segment.IsCircle())
            {
                estimate.X2 = estimate.X1;
                estimate.Y2 = estimate.Y1;
            }
            else
            {
                estimate.X2 += fMs * this.XVelocity2 / 1000.0;
                estimate.Y2 += fMs * this.YVelocity2 / 1000.0;
            }

            return estimate;
        }
    }

    // BannerText generates a scrolling or still banner of text (along the bottom of the screen).
    // Only one banner exists at a time.  Calling NewBanner() will erase the old one and start the new one.
    public class BannerText
    {
        private readonly System.Windows.Media.Color color;
        private readonly string text;
        private readonly bool doScroll;
        private static BannerText myBannerText;
        private System.Windows.Media.Brush brush;
        private Label label;
        private Rect boundsRect;
        private Rect renderedRect;
        private double offset;

        public BannerText(string s, Rect rect, bool scroll, System.Windows.Media.Color col)
        {
            this.text = s;
            this.boundsRect = rect;
            this.doScroll = scroll;
            this.brush = null;
            this.label = null;
            this.color = col;
            this.offset = this.doScroll ? 1.0 : 0.0;
        }

        public static void NewBanner(string s, Rect rect, bool scroll, System.Windows.Media.Color col)
        {
            myBannerText = (s != null) ? new BannerText(s, rect, scroll, col) : null;
        }

        public static void UpdateBounds(Rect rect)
        {
            if (myBannerText == null)
            {
                return;
            }

            myBannerText.boundsRect = rect;
            myBannerText.label = null;
        }

        public static void Draw(UIElementCollection children)
        {
            if (myBannerText == null)
            {
                return;
            }

            Label text = myBannerText.GetLabel();
            if (text == null)
            {
                myBannerText = null;
                return;
            }

            children.Add(text);
        }

        private Label GetLabel()
        {
            if (this.brush == null)
            {
                this.brush = new SolidColorBrush(this.color);
            }

            if (this.label == null)
            {
                this.label = FallingThings.MakeSimpleLabel(this.text, this.boundsRect, this.brush);
                if (this.doScroll)
                {
                    this.label.FontSize = Math.Max(20, this.boundsRect.Height / 30);
                    this.label.Width = 10000;
                }
                else
                {
                    this.label.FontSize = Math.Min(
                        Math.Max(10, this.boundsRect.Width * 2 / this.text.Length), Math.Max(10, this.boundsRect.Height / 20));
                }

                this.label.VerticalContentAlignment = VerticalAlignment.Bottom;
                this.label.HorizontalContentAlignment = this.doScroll
                                                            ? HorizontalAlignment.Left
                                                            : HorizontalAlignment.Center;
                this.label.SetValue(Canvas.LeftProperty, this.offset * this.boundsRect.Width);
            }

            this.renderedRect = new Rect(this.label.RenderSize);

            if (this.doScroll)
            {
                this.offset -= 0.0015;
                if (this.offset * this.boundsRect.Width < this.boundsRect.Left - 10000)
                {
                    return null;
                }

                this.label.SetValue(Canvas.LeftProperty, (this.offset * this.boundsRect.Width) + this.boundsRect.Left);
            }

            return this.label;
        }
    }
}
