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
using System.Text;
using System.Windows;
using System.Diagnostics;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#else
using System.Windows.Media;
using System.Windows.Threading;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class RectAnimation : DependencyObject
    {
        #region fields

        private int normalUpdateCount, count = 0;

        Rect from, to;

        double incrementWidth = 0, incrementHeight = 0, actualInterval, callCount, incrementTop = 0, increementLeft = 0;

        UIElement element;

        RectangleGeometry rectangleGeometry;

        DispatcherTimer timer;

        #endregion

        #region events

        public event RectAnimationCompletedHandler Completed;

        #endregion

        #region properties

        public bool IsActive { get; set; }

        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(RectAnimation), new PropertyMetadata(TimeSpan.FromMilliseconds(0)));

        public Rect From
        {
            get { return (Rect)GetValue(FromProperty); }
            set
            {
                SetValue(FromProperty, value);
                from = value;
            }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Rect), typeof(RectAnimation), new PropertyMetadata(new Rect()));

        public Rect To
        {
            get { return (Rect)GetValue(ToProperty); }
            set
            {
                SetValue(ToProperty, value);
                to = value;
            }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Rect), typeof(RectAnimation), new PropertyMetadata(new Rect()));

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(RectAnimation), new PropertyMetadata(TimeSpan.FromMilliseconds(10)));

        #endregion

        #region methods

        public void SetTarget(UIElement element)
        {
            this.element = element;
            rectangleGeometry = new RectangleGeometry();
            to = To;
            from = From;
            rectangleGeometry.Rect = from;
            element.Clip = rectangleGeometry;
        }

        public void Stop()
        {
            if (timer!=null)
            {
                timer.Stop();
                element.Clip = null;
            }
        }

        public void Begin()
        {

            CalculateIntervals();
            if (!double.IsNaN(actualInterval))
            {
                timer = new DispatcherTimer();
                rectangleGeometry = new RectangleGeometry();
                timer.Tick += timer_Tick;
                timer.Interval = TimeSpan.FromMilliseconds(actualInterval);
                StartUpdate();
            }
        }

#if NETFX_CORE
        async public void StartUpdate()
#else
        private void StartUpdate()
#endif

        {
#if NETFX_CORE
            //if (stopWatch.ElapsedMilliseconds != BeginTime.TotalMilliseconds)
            //    GetEllapsedTime();
            await Task.Delay(BeginTime);
#else
            System.ComponentModel.BackgroundWorker bc = new System.ComponentModel.BackgroundWorker();
            lock (this)
            {
                System.Threading.Monitor.Wait(this, (int)BeginTime.TotalMilliseconds);// System.Threading.Thread.Sleep(BeginTime);
            }
#endif
            timer.Start();
            IsActive = true;
        }

#if NETFX_CORE
        void timer_Tick(object sender, object e)
#else
        void timer_Tick(object sender, EventArgs e)
#endif
        {
            if (count >= normalUpdateCount)
            {
                incrementWidth = incrementWidth - ((6 / Duration.TotalSeconds) * incrementWidth / 100);
                incrementHeight = incrementHeight - ((6 / Duration.TotalSeconds) * incrementHeight / 100);
                increementLeft = increementLeft - ((6 / Duration.TotalSeconds) * increementLeft / 100);
                incrementTop = incrementTop - ((6 / Duration.TotalSeconds) * incrementTop / 100);
            }
            bool updated = false;
            if (Math.Round(from.Height) < Math.Round(to.Height))
            {
                from.Height += incrementHeight;
                updated = true;
            }

            if (from.Width < to.Width)
            {
                updated = true;
                from.Width += incrementWidth;
            }
            if (Math.Round(from.X) != Math.Round(to.X))
            {
                from.X += increementLeft;
                updated = true;
            }
            if (Math.Round(from.Y) != Math.Round(to.Y) && Math.Round(from.Y)>=0)
            {
                from.Y += incrementTop;
                updated = true;
            }
            rectangleGeometry.Rect = from;
            element.Clip = rectangleGeometry;
            if (!updated)
            {
                IsActive = false;
                if (Completed != null)
                    Completed(this, new RectAnimationCompletedArgs() { Duration = this.Duration, SourceObj = this.element });
                timer.Stop();
                timer.Tick -= timer_Tick;
            }
            count++;
        }

        private void CalculateIntervals()
        {
            actualInterval = 15;
            callCount = Duration.TotalMilliseconds / actualInterval;

            double diffWidth, diffHeight, diffX, diffY;
            diffHeight = to.Height - from.Height;
            diffWidth = to.Width - from.Width;
            double normalIncWidth = 80 * diffWidth / 100d;
            diffX = to.X - from.X;
            diffY = to.Y - from.Y;

            incrementWidth = diffWidth / callCount;
            incrementHeight = diffHeight / callCount;
            incrementTop = diffY / callCount;
            increementLeft = diffX / callCount;

            incrementWidth += ((4 / Duration.TotalSeconds) * incrementWidth / 100);
            increementLeft += ((4 / Duration.TotalSeconds) * increementLeft / 100);
            incrementTop += ((4 / Duration.TotalSeconds) * incrementTop / 100);
            increementLeft += ((4 / Duration.TotalSeconds) * increementLeft / 100);
            normalUpdateCount = (int)(normalIncWidth / incrementWidth);

            //double widthSpeed = (diffWidth * 0.25) / 1000;
            //double heightSpeed = (diffHeight * 0.25) / 1000;
            //double topSpeed = (diffY * 0.25) / 1000;
            //double leftSpeed = (diffX * 0.25) / 1000;
            //double durationSpeed = ((1000 / Duration.TotalMilliseconds) * 100);
            //if (diffWidth != 0)
            //{
            //    incrementWidth = diffWidth / ((diffWidth) / (durationSpeed * widthSpeed));
            //    callCount = diffWidth / (int)incrementWidth;
            //}
            //if (diffHeight != 0)
            //{
            //    incrementHeight = diffHeight / ((diffHeight) / (durationSpeed * heightSpeed));
            //    callCount += diffHeight / (int)incrementHeight;
            //}
            //if (diffY != 0)
            //{
            //    incrementTop = diffY / ((diffY) / (durationSpeed * topSpeed));
            //    callCount += diffY / (int)incrementTop;
            //}
            //if (diffX != 0)
            //{
            //    increementLeft = diffX / ((diffX) / (durationSpeed * leftSpeed));
            //    callCount += diffX / (int)increementLeft;
            //}

            //actualInterval = (Duration.TotalMilliseconds / callCount) / 4;
        }

        #endregion

    }

    public class RectAnimationCompletedArgs : EventArgs
    {
        public object SourceObj { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public delegate void RectAnimationCompletedHandler(object src, RectAnimationCompletedArgs args);

}
