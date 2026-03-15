#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.UI.Xaml.Grid;
namespace Syncfusion.UI.Xaml.Utility
{
    /// <summary>
    /// Holds a start and end value with integer precision.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public struct Int32Span
    {
        private int start;
        private int end;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Span"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Int32Span(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public int Start
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
        public int End
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
        /// Gets or sets the count (equals end - start + 1)
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return end - start + 1;
            }
            set
            {
                end = start + value - 1;
            }
        }
    }
}
