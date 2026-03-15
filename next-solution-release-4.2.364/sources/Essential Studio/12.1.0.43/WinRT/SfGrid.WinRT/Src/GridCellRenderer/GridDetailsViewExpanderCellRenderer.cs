#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif
using Syncfusion.UI.Xaml.ScrollAxis;


namespace Syncfusion.UI.Xaml.Grid.Cells
{
    public class GridDetailsViewExpanderCellRenderer : GridVirtualizingCellRenderer<GridDetailsViewExpanderCell, GridDetailsViewExpanderCell>
    {
        public GridDetailsViewExpanderCellRenderer()
        {
            this.SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
            IsFocusible = false;
            IsEditable = false;
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridDetailsViewExpanderCell uiElement, GridColumn column, object dataContext)
        {
            uiElement.DataGrid = DataGrid;
            uiElement.RowColumnIndex = rowColumnIndex;
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridDetailsViewExpanderCell element, GridColumn column, object dataContext)
        {
            element.RowColumnIndex = cellRowcolumnIndex;
        }

        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var element = cell as GridDetailsViewExpanderCell;
            if (element != null) element.RowColumnIndex = cellRowColumnIndex;
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
        }
    }
}
