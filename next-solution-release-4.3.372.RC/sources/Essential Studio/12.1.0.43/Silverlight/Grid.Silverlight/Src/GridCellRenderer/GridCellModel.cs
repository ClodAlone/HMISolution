#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellModel<T> : GridCellModelBase 
        where T : IGridCellRenderer, new()
    {
        public override IGridCellRenderer CreateRenderer()
        {
            IGridCellRenderer r = new T();
            r.RaiseCreated(this);
            return r;
        }
    }
}
