#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
namespace Syncfusion.Windows.Controls.Cells
#else
namespace Syncfusion.WinRT.Controls.Cells
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class OverlappingCellInfo : CellSpanInfo
    {
        public OverlappingCellInfo()
        {
        }

        public OverlappingCellInfo(int top, int left, int bottom, int right)
            : base(top, left, bottom, right)
        {

        }

        public OverlappingCellInfo(int top, int left, int bottom, int right, bool clipRows, bool clipColumns)
            : base(top, left, bottom, right, clipRows, clipColumns)
        {

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
