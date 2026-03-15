#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class VisibleOverlappingCellInfo: VisibleCellSpanInfo
    {
        internal int arrangeId;

        public VisibleOverlappingCellInfo()
        {
        }

        public VisibleOverlappingCellInfo(int top, int left, OverlappingCellInfo coveredCell)
            : base(top, left, coveredCell)
        {
        }

        public OverlappingCellInfo OverlappingCell
        {
            get { return (OverlappingCellInfo)cellSpan; }
        }

        public override string ToString()
        {
            return String.Format("OverlappingCellSpan: {0},{1},{2},{3},{4}", top, left, bottom, right, cellSpan);
        }
    }
}
