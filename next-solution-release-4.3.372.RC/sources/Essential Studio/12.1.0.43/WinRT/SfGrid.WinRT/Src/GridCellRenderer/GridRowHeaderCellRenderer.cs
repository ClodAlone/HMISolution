#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if WPF
using System.Windows.Controls;
#endif
#if WinRT
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    class GridRowHeaderCellRenderer : GridVirtualizingCellRenderer<GridRowHeaderCell, GridRowHeaderCell>
    {
        public GridRowHeaderCellRenderer()
        {
            
            this.SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
            IsFocusible = false;
            IsEditable = false;
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridRowHeaderCell uiElement, GridColumn column, object dataContext)
        {
            uiElement.DataContext = dataContext;
            if(uiElement is GridRowHeaderCell)
                (uiElement as GridRowHeaderCell).RowIndex = rowColumnIndex.RowIndex; 
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridRowHeaderCell element, GridColumn column, object dataContext)
        {

        }

        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var element = cell as GridRowHeaderCell;
            if (element != null)
                element.RowIndex = cellRowColumnIndex.RowIndex;
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
        }
    }
}
