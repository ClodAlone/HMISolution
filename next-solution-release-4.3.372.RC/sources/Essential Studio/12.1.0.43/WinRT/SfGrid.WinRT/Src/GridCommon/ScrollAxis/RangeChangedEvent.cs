#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;
#if !WinRT
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.ScrollAxis
{
	/// <summary>
    /// Handles events with <see cref="RangeChangedEventArgs"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="RangeChangedEventArgs"/> that contains the event data.</param>
    public delegate void RangeChangedEventHandler(object sender, RangeChangedEventArgs e);

    /// <summary>
    /// Holds <see cref="From"/> and <see cref="To"/> integer properties.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public sealed class RangeChangedEventArgs : EventArgs
    {
        int from;
        int to;
        double oldSize;
        double newSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public RangeChangedEventArgs(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public RangeChangedEventArgs(int from, int to,double oldSize,double newSize)
        {
            this.from = from;
            this.to = to;
            this.oldSize = oldSize;
            this.newSize = newSize;
        }

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>From.</value>
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        public int To
        {
            get
            {
                return to;
            }
        }

        public double OldSize
        {
            get
            { 
                return oldSize;
            }
        }

        public double NewSize
        {
            get
            {
                return newSize;
            }
        }
    }

    /// <summary>
    /// Handles events with <see cref="LinesRemovedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="LinesRemovedEventArgs"/> that contains the event data.</param>
    public delegate void LinesRemovedEventHandler(object sender, LinesRemovedEventArgs e);

    /// <summary>
    /// Holds <see cref="RemoveAt"/> and <see cref="Count"/> integer properties.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public sealed class LinesRemovedEventArgs : EventArgs
    {
        int removeAt;
        int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinesRemovedEventArgs"/> class.
        /// </summary>
        /// <param name="removeAt">RemoveAt.</param>
        /// <param name="count">Count.</param>
        public LinesRemovedEventArgs(int removeAt, int count)
        {
            this.removeAt = removeAt;
            this.count = count;
        }

        /// <summary>
        /// Gets removeAt.
        /// </summary>
        /// <value>RemoveAt.</value>
        public int RemoveAt
        {
            get
            {
                return removeAt;
            }
        }

        /// <summary>
        /// Gets count.
        /// </summary>
        /// <value>Count.</value>
        public int Count
        {
            get
            {
                return count;
            }
        }
    }

    /// <summary>
    /// Handles events with <see cref="LinesInsertedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="LinesInsertedEventArgs"/> that contains the event data.</param>
    public delegate void LinesInsertedEventHandler(object sender, LinesInsertedEventArgs e);

    /// <summary>
    /// Holds <see cref="InsertAt"/> and <see cref="Count"/> integer properties.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public sealed class LinesInsertedEventArgs : EventArgs
    {
        int insertAt;
        int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinesInsertedEventArgs"/> class.
        /// </summary>
        /// <param name="insertAt">InsertAt.</param>
        /// <param name="count">Count.</param>
        public LinesInsertedEventArgs(int insertAt, int count)
        {
            this.insertAt = insertAt;
            this.count = count;
        }

        /// <summary>
        /// Gets insertAt.
        /// </summary>
        /// <value>InsertAt.</value>
        public int InsertAt
        {
            get
            {
                return insertAt;
            }
        }

        /// <summary>
        /// Gets count.
        /// </summary>
        /// <value>Count.</value>
        public int Count
        {
            get
            {
                return count;
            }
        }
    }

    public delegate void DefaultLineSizeChangedEventHandler(object sender, DefaultLineSizeChangedEventArgs e);

    [ClassReference(IsReviewed = false)]
    public sealed class DefaultLineSizeChangedEventArgs:EventArgs
    {
        double oldValue;
        double newValue;

        public DefaultLineSizeChangedEventArgs()
        {
        }

        public DefaultLineSizeChangedEventArgs(double oldValue,double newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public double OldValue
        {
            get { return oldValue; }
        }

        public double NewValue
        {
            get { return newValue; }
        }
    }

    /// <summary>
    /// Handles events with <see cref="HiddenRangeChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="HiddenRangeChangedEventArgs"/> that contains the event data.</param>
    public delegate void HiddenRangeChangedEventHandler(object sender, HiddenRangeChangedEventArgs e);

    /// <summary>
    /// Holds <see cref="From"/> and <see cref="To"/> integer properties.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public sealed class HiddenRangeChangedEventArgs : EventArgs
    {
        int from;
        int to;
        bool hide;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public HiddenRangeChangedEventArgs(int from, int to, bool hide)
        {
            this.from = from;
            this.to = to;
            this.hide = hide;
        }

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>From.</value>
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        public int To
        {
            get
            {
                return to;
            }
        }

        public bool Hide
        {
            get
            {
                return hide;
            }
        }
    }

    public enum ScrollChangedAction
    {
        LinesInserted,
        LinesRemoved,
        HeaderLineCountChanged,
        FooterLineCountChanged,
        DefaultLineSizeChanged,
        LineCountChanged,
        HiddenLineChanged,
        LineResized,
        ScrollBarValueChanged,
    }


    public class ScrollChangedEventArgs : EventArgs
    {
        ScrollChangedAction scrollChangedAction;

        public ScrollChangedAction Action
        {
            get
            {
                return scrollChangedAction;
            }
        }

        public ScrollChangedEventArgs(ScrollChangedAction action)
        {
            scrollChangedAction = action;
        }
    }


    public delegate void ScrollChangedEventHandler(object sender, ScrollChangedEventArgs e);

}
