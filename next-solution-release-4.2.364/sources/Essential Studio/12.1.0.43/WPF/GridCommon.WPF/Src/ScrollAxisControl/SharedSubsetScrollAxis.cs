#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// SharedSubsetScrollAxis implements scrolling logic for both horizontal and vertical 
    /// scrolling in a <see cref="ScrollAxisControl"/> that is embeded in a parent
    /// scroll axis control.<para/>
    /// Logical units in the ScrollAxisBase are called "Lines". With the 
    /// <see cref="ScrollAxisControl.ScrollRows"/> a line representes rows in a grid 
    /// and with <see cref="ScrollAxisControl.ScrollRows"/> a line represents columns in a grid.
    /// <para/>
    /// SharedSubsetScrollAxis supports pixel scrolling and calculates the total height or
    /// width of all lines.
    /// </summary>
    public class SharedSubsetScrollAxis : PixelScrollAxis
    {
        PixelScrollAxis parentScrollAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedSubsetScrollAxis"/> class.
        /// </summary>
        /// <param name="parentScrollAxis">The parent scroll axis.</param>
        /// <param name="scrollBar">The scroll bar.</param>
        /// <param name="scrollLinesHost">The scroll lines host.</param>
        public SharedSubsetScrollAxis(PixelScrollAxis parentScrollAxis, IScrollBar scrollBar, ILineSizeHost scrollLinesHost)
            : base(scrollBar, scrollLinesHost, new DistanceCounterSubset(parentScrollAxis.Distances))
        {
            this.parentScrollAxis = parentScrollAxis;

            // Important: ScrollLinesHost could be null. Double check base class for any members
            // that access ScrollLinesHost. They need to be virtual and then overriden in this class!
        }

        /// <summary>
        /// Gets the distances collection which is used internally
        /// for mapping from a point position to
        /// a line index and vice versa.
        /// </summary>
        /// <value>The distances collection.</value>
        public new DistanceCounterSubset Distances
        {
            get
            {
                return (DistanceCounterSubset)base.Distances;
            }
        }

        /// <summary>
        /// Gets or sets the index of the first line in a parent axis. This is used for shared
        /// or nested scroll axis (e.g. a nested grid with shared axis in a covered cell).
        /// </summary>
        /// <value>The index of the first line..</value>
        public override int StartLineIndex
        {
            get { return Distances.Start; }
            set 
            { 
                MarkDirty(); 
                Distances.Start = value; 
            }
        }

        /// <summary>
        /// Gets or sets the default size of lines.
        /// </summary>
        /// <value>The default size of lines.</value>
        public override double DefaultLineSize
        {
            get
            {
                return parentScrollAxis.DefaultLineSize;
            }
            set
            {
                MarkDirty();
                parentScrollAxis.DefaultLineSize = value;
                RaiseChanged();
            }
        }

        /// <summary>
        /// Gets size from ScrollLinesHost or if the line is being resized then get temporary value
        /// previously set with <see cref="SetLineResize"/>
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatSizeCount">The number of subsequent values with same size.</param>
        /// <returns></returns>
        public override double GetLineSize(int index, out int repeatSizeCount)
        {
            return parentScrollAxis.GetLineSize(index + StartLineIndex, out repeatSizeCount);
        }

        /// <summary>
        /// Set temporary value for a line size during a resize operation without commiting
        /// value to SrollLinesHost.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size.</param>
        public override void SetLineResize(int index, double size)
        {
            MarkDirty();
            parentScrollAxis.SetLineResize(index + StartLineIndex, size);
            RaiseChanged();
        }

        /// <summary>
        /// Resets temporary value for line size after a resize operation
        /// </summary>
        public override void ResetLineResize()
        {
            MarkDirty();
            parentScrollAxis.ResetLineResize();
            RaiseChanged();
        }

    }

}
