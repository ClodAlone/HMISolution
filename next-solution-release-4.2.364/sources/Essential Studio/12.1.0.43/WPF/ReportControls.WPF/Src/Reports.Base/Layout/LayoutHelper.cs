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
using Syncfusion.RDL.Data;

namespace Syncfusion.RDL.Layout
{
    internal class LayoutSize
    {
        public LayoutSize(double width, double height)
        {
            this.Height = height;
            this.Width = width;
        }

        public double Height { get; set; }

        public double Width { get; set; }   
    }

    class ReportPageInfo
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public ReportModelContentCollection ReportModelCollection { get; set; }
    }

    internal class LayoutReportItemModel
    {
        /// <summary>
        /// Gets or sets the top position of the control.
        /// </summary>
        public double ActualTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the left position of the control.
        /// </summary>
        public double ActualLeft
        {
            get;
            set;
        }

        public double ActualWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        public double ActualHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start page and number of pages the control spans in a spaning control.
        /// </summary>
        public Dictionary<int, int> BelongsTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of pages in the report.
        /// </summary>
        public int TotalPages
        {
            get;
            set;
        }
    }


    internal class LayoutThicknessInfo
    {
        public LayoutThicknessInfo(double uniformLength)
        {
            this.Bottom = this.Left = this.Right = this.Top = uniformLength;
        }

        public LayoutThicknessInfo(double left, double top, double right, double bottom)
        {
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Top = top;
        }

        public double Bottom { get; set; }

        public double Left { get; set; }

        public double Right { get; set; }

        public double Top { get; set; }
    }

    /// <summary>
    /// Paging information class.
    /// </summary>
    internal class PageInfo
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        internal double Height { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        internal double Width { get; set; }
    }
}
