#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;
#if !WinRT
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridPrepareRenderCellEventArgs : SyncfusionRoutedEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;

        public GridPrepareRenderCellEventArgs(RowColumnIndex cell, GridStyleInfo style, object source)
            : base(source)
        {
            this.cell = cell;
            this.style = style;
        }

        
        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }
    }

    public delegate void GridPrepareRenderCellEventHandler(object sender, GridPrepareRenderCellEventArgs e);

}
