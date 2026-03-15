#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;

namespace Syncfusion.UI.Xaml.Utility
{
    /// <summary>
    /// Holds a start and end value with double precision.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public struct DoubleSpan
    {
        private double start;
        private double end;

        /// <summary>
        /// An empty object.
        /// </summary>
        public static readonly DoubleSpan Empty = new DoubleSpan(0, -1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleSpan"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public DoubleSpan(double start, double end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return end < start;
            }
        }


        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start
        {
            get
            {
                return this.start;
            }
            set
            {
                this.start = value;
            }
        }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public double End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.end = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get
            {
                return end - start;
            }
            set
            {
                end = start + value;
            }
        }

        /// <summary>
        /// Returns a string with start and end values.
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return String.Format("DoubleSpan Start = {0}, End = {1}", Start, End);
        }
    }
}
