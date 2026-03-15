#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.ComponentModel
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  This class holds a stop watch
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class Stopwatch
    {
        /// <summary>
        ///  Gets and Set the bool value to determine whether high resolution or not
        /// </summary>
        public static readonly bool IsHighResolution = false;
        /// <summary>
        ///  sets long value as TimeSpan.TicksPerSeconds
        /// </summary>
        public static readonly long Frequency = TimeSpan.TicksPerSecond;

        /// <summary>
        /// Gets  the Elapsed time span value
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (!this.StartUtc.HasValue)
                {
                    return TimeSpan.Zero;
                }
                if (!this.EndUtc.HasValue)
                {
                    return (DateTime.UtcNow - this.StartUtc.Value);
                }
                return (this.EndUtc.Value - this.StartUtc.Value);
            }
        }

        /// <summary>
        /// Gets long value for ElapsedMilliseconds
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                return this.ElapsedTicks / TimeSpan.TicksPerMillisecond;
            }
        }
        /// <summary>
        /// Gets long value for Elapsed ticks
        /// </summary>
        public long ElapsedTicks { get { return this.Elapsed.Ticks; } }
        /// <summary>
        /// Gets bool value for IsRunning
        /// </summary>
        public bool IsRunning { get; private set; }
        private DateTime? StartUtc { get; set; }
        private DateTime? EndUtc { get; set; }

        /// <summary>
        /// This method returns the time stamp values
        /// </summary>
        public static long GetTimestamp()
        {
            return DateTime.UtcNow.Ticks;
        }
        /// <summary>
        ///  This method reset the stopwatch.
        /// </summary>
        public void Reset()
        {
            Stop();
            this.EndUtc = null;
            this.StartUtc = null;
        }
        /// <summary>
        ///  This method start a new stopwatch or resume the previous one
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }
            if ((this.StartUtc.HasValue) &&
                (this.EndUtc.HasValue))
            {
                // Resume the timer from its previous state
                this.StartUtc = this.StartUtc.Value +
                    (DateTime.UtcNow - this.EndUtc.Value);
            }
            else
            {
                // Start a new time-interval from scratch
                this.StartUtc = DateTime.UtcNow;
            }
            this.IsRunning = true;
            this.EndUtc = null;
        }
        /// <summary>
        ///  This method stops the stopwatch.
        /// </summary>
        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.EndUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        ///  This method start a new stopwatch.
        /// </summary>
        public static Stopwatch StartNew()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }
    }
}
