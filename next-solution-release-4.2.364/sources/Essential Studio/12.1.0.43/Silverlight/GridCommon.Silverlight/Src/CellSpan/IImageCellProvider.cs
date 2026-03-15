#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
    public interface IOverlappingCellProvider
    {
        OverlappingCellInfo GetOverlappingCell(int rowIndex, int columnIndex);

        bool IsEmpty { get; }
    }
}
