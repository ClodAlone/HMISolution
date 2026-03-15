#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class OverviewResizer : ContentControl
    {
        Overview ov;

        /// <summary>
        /// 
        /// </summary>
        public OverviewResizer()
        {
            this.DefaultStyleKey = typeof(OverviewResizer);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Thumb bottomLeft;
            Thumb top;
            Thumb topLeft;
            Thumb topRight;
            Thumb bottomRight;
            Thumb bottom;
            Thumb right;
            Thumb left;
            ov = OverviewResizer.FindParent<Overview>(this);

            topLeft = this.GetTemplateChild("PART_OverViewTopLeftCorner") as Thumb;
            top = this.GetTemplateChild("PART_OverViewTop") as Thumb;
            topRight = this.GetTemplateChild("PART_OverViewTopRightCorner") as Thumb;
            bottom = this.GetTemplateChild("PART_OverViewBottom") as Thumb;
            bottomRight = this.GetTemplateChild("PART_OverViewBottomRightCorner") as Thumb;
            bottomLeft = this.GetTemplateChild("PART_OverViewBottomLeftCorner") as Thumb;
            right = this.GetTemplateChild("PART_OverViewRight") as Thumb;
            left = this.GetTemplateChild("PART_OverViewLeft") as Thumb;

            top.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            top.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            top.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            //topLeft.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            //topLeft.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            //topLeft.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            //topRight.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            //topRight.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            //topRight.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            bottom.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            bottom.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            bottom.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            bottomRight.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            bottomRight.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            bottomRight.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            //bottomLeft.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            //bottomLeft.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            //bottomLeft.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            right.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            right.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            right.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);

            left.DragStarted += new DragStartedEventHandler(OverviewResizer_DragStarted);
            left.DragDelta += new DragDeltaEventHandler(OverviewResizer_DragDelta);
            left.DragCompleted += new DragCompletedEventHandler(OverviewResizer_DragDeltaComplete);
        }

        private double oldHeight;
        private double horChange;
        private double verChange;
        private double initWidth;
        private double initHeight;
        private double initx;
        private double inity;

        void OverviewResizer_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (ov.AllowResize)
            {
                oldHeight = ov.VpHeight;
                ov.IsResizing = true;
                horChange = verChange = 0;
                initWidth = ov.VpWidth;
                initHeight = ov.VpHeight;
                initx = ov.VpOffsetX;
                inity = ov.VpOffsetY;
            }
        }

        void OverviewResizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //double y_delta = e.VerticalChange;
            //double y_oldS = ov.VpHeight;
            //double y_newS = ov.VpHeight + y_delta;
            //double y_scale = y_newS / y_oldS;

            //double x_delta = e.HorizontalChange;
            //double x_oldS = ov.VpWidth;
            //double x_newS = ov.VpWidth + x_delta;
            //double x_scale = x_newS / x_oldS;
            if (ov.AllowResize)
            {
                VerticalAlignment ver = (sender as Thumb).VerticalAlignment;
                HorizontalAlignment hor = (sender as Thumb).HorizontalAlignment;
                horChange += e.HorizontalChange;
                verChange += e.VerticalChange;
                ov.IsResized = true;
                double y_scale = (ov.VpHeight + e.VerticalChange) / ov.VpHeight;
                double y_increase = ov.VpWidth * y_scale - ov.VpWidth;

                double x_scale = (ov.VpWidth + e.HorizontalChange) / ov.VpWidth;
                double x_increase = ov.VpHeight * x_scale - ov.VpHeight;

                double y_scaleN = (ov.VpHeight - e.VerticalChange) / ov.VpHeight;
                double y_increaseN = ov.VpWidth * y_scaleN - ov.VpWidth;

                double x_scaleN = (ov.VpWidth - e.HorizontalChange) / ov.VpWidth;
                double x_increaseN = ov.VpHeight * x_scaleN - ov.VpHeight;

                double x_init_scale = (initWidth + horChange) / initWidth;
                double y_init_scale = (initHeight + verChange) / initHeight;

                double x_init_scale_N = (initWidth - horChange) / initWidth;
                double y_init_scale_N = (initHeight - verChange) / initHeight;

                if (ver == System.Windows.VerticalAlignment.Top)
                {
                    if (hor == System.Windows.HorizontalAlignment.Left)
                    {
                        if (Math.Abs(y_init_scale_N) > Math.Abs(x_init_scale_N))
                        {
                            ov.VpOffsetY = inity - (initHeight * y_init_scale_N - initHeight);
                            (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                            ov.VpOffsetX = initx - (initWidth * y_init_scale_N - initWidth);
                            (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                            ov.VpWidth = initWidth * y_init_scale_N;
                            ov.VpHeight = initHeight * y_init_scale_N;
                        }
                        else
                        {
                            ov.VpOffsetY = inity - (initHeight * x_init_scale_N - initHeight);
                            (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                            ov.VpOffsetX = initx - (initWidth * x_init_scale_N - initWidth);
                            (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                            ov.VpWidth = initWidth * x_init_scale_N;
                            ov.VpHeight = initHeight * x_init_scale_N;
                        }
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Stretch)
                    {
                        //double scale = (ov.VpHeight - e.VerticalChange) / ov.VpHeight;
                        //double increase = ov.VpWidth * scale - ov.VpWidth;

                        //ov.VpWidth *= y_scaleN;
                        //ov.VpHeight *= y_scaleN;
                        ov.VpWidth = initWidth * y_init_scale_N;
                        ov.VpHeight = initHeight * y_init_scale_N;
                        //ov.VpOffsetY += e.VerticalChange;
                        ov.VpOffsetY = inity - (initHeight * y_init_scale_N - initHeight);
                        (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                        //ov.VpOffsetX -= y_increaseN / 2;
                        ov.VpOffsetX = initx - (initWidth * y_init_scale_N - initWidth) / 2;
                        (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Right)
                    {
                        if (Math.Abs(y_init_scale_N) > Math.Abs(x_init_scale))
                        {
                            ov.VpOffsetY = inity - (initHeight * y_init_scale_N - initHeight);
                            (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                            ov.VpWidth = initWidth * y_init_scale_N;
                            ov.VpHeight = initHeight * y_init_scale_N;
                        }
                        else
                        {
                            ov.VpOffsetY = inity - (initHeight * x_init_scale - initHeight);
                            (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                            ov.VpWidth = initWidth * x_init_scale;
                            ov.VpHeight = initHeight * x_init_scale;
                        }
                    }
                }
                else if (ver == System.Windows.VerticalAlignment.Stretch)
                {
                    if (hor == System.Windows.HorizontalAlignment.Left)
                    {
                        //double scale = (ov.VpWidth - e.HorizontalChange) / ov.VpWidth;
                        //double increase = ov.VpHeight * scale - ov.VpHeight;

                        //ov.VpWidth *= x_scaleN;
                        ov.VpWidth = initWidth * x_init_scale_N;
                        //ov.VpHeight *= x_scaleN;
                        ov.VpHeight = initHeight * x_init_scale_N;

                        //ov.VpOffsetX += e.HorizontalChange;
                        ov.VpOffsetX = initx - (initWidth * x_init_scale_N - initWidth);
                        (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                        //ov.VpOffsetY -= x_increaseN / 2;
                        ov.VpOffsetY = inity - (initHeight * x_init_scale_N - initHeight) / 2;
                        (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Stretch)
                    {
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Right)
                    {
                        //double scale = (ov.VpWidth + e.HorizontalChange) / ov.VpWidth;
                        //double increase = ov.VpHeight * scale - ov.VpHeight;
                        //ov.VpWidth *= x_scale;
                        ov.VpWidth = initWidth * x_init_scale;
                        //ov.VpHeight *= x_scale;
                        ov.VpHeight = initHeight * x_init_scale;
                        //ov.VpOffsetY -= x_increase / 2;
                        ov.VpOffsetY = inity - (initHeight * x_init_scale - initHeight) / 2;
                        (ov.Trans as TranslateTransform).Y = ov.VpOffsetY;
                    }
                }
                else if (ver == System.Windows.VerticalAlignment.Bottom)
                {
                    if (hor == System.Windows.HorizontalAlignment.Left)
                    {
                        if (Math.Abs(y_init_scale) > Math.Abs(x_init_scale_N))
                        {
                            ov.VpOffsetX = initx - (initWidth * y_init_scale - initWidth);
                            (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                            ov.VpWidth = initWidth * y_init_scale;
                            ov.VpHeight = initHeight * y_init_scale;
                        }
                        else
                        {
                            ov.VpOffsetX = initx - (initWidth * x_init_scale_N - initWidth);
                            (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                            ov.VpWidth = initWidth * x_init_scale_N;
                            ov.VpHeight = initHeight * x_init_scale_N;
                        }
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Stretch)
                    {
                        //double scale = (ov.VpHeight + e.VerticalChange) / ov.VpHeight;
                        //double increase =  ov.VpWidth * scale - ov.VpWidth;
                        //ov.VpWidth *= y_scale;
                        ov.VpWidth = initWidth * y_init_scale;
                        //ov.VpHeight *= y_scale;
                        ov.VpHeight = initHeight * y_init_scale;
                        //ov.VpOffsetX -= y_increase / 2;
                        ov.VpOffsetX = initx - (initWidth * y_init_scale - initWidth) / 2;
                        (ov.Trans as TranslateTransform).X = ov.VpOffsetX;
                    }
                    else if (hor == System.Windows.HorizontalAlignment.Right)
                    {
                        if (y_init_scale > x_init_scale)
                        {
                            ov.VpWidth = initWidth * y_init_scale;
                            ov.VpHeight = initHeight * y_init_scale;
                        }
                        else
                        {
                            ov.VpWidth = initWidth * x_init_scale;
                            ov.VpHeight = initHeight * x_init_scale;
                        }
                    }
                }
            }
        }

        void OverviewResizer_DragDeltaComplete(object sender, DragCompletedEventArgs e)
        {
            if (ov.AllowResize)
            {
                ov.UpdateSourceScroll();
                ov.UpdateScrollViewer();
                ov.IsResizing = false;
            }
        }

        private static T FindParent<T>(UIElement control) where T : UIElement
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T)
                    return p as T;
                else
                    return FindParent<T>(p);
            }
            return null;
        }
    }
}
