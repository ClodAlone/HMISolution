#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridDataHeaderCellRenderer : GridVirtualizingCellRenderer<GridHeaderCellControl,GridHeaderCellControl>
    {
        public GridDataHeaderCellRenderer()
        {
            SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
        }

        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, GridHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
            
        }

        public override void OnUpdateDisplayBinding(RowColumnIndex cellRowcolumnIndex, GridHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
            
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
#if WPF
            if (!DataGrid.IsLoaded)
                column = column.Clone() as GridColumn;
#endif
            uiElement.Column = column;
            uiElement.dataGrid = this.DataGrid;
            if (column != null)
            {
                if (column.HeaderText == null)
                    column.HeaderText = column.MappingName;

                var bind = new Binding {Path = new PropertyPath("HeaderText")};
                uiElement.SetBinding(ContentControl.ContentProperty, bind);

                bind = new Binding { Path = new PropertyPath("HorizontalHeaderContentAlignment") };
                uiElement.SetBinding(Control.HorizontalContentAlignmentProperty, bind);
                uiElement.Update();
                uiElement.DataContext = column;
            }
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridHeaderCellControl element, GridColumn column, object dataContext)
        {
#if WPF
            if (!DataGrid.IsLoaded)
                column = column.Clone() as GridColumn;
#endif
            element.ClearValue(GridHeaderCellControl.ContentProperty);
            element.Column = column;
            if (column.HeaderText == null)
                column.HeaderText = column.MappingName;

            var bind = new Binding { Path = new PropertyPath("HeaderText") };
            element.SetBinding(ContentControl.ContentProperty, bind);

            bind = new Binding { Path = new PropertyPath("HorizontalHeaderContentAlignment") };
            element.SetBinding(Control.HorizontalContentAlignmentProperty, bind);
            element.Update();
            element.DataContext = column;
        }
       
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            Style newStyle = null;
            DataTemplate newTemplate = null;

            var control = cell as GridHeaderCellControl;
            if (control != null && column != null)
            {
                bool hasColumnHeaderCellStyle = column.HeaderStyle != null;
                bool hasColumnHeaderTemplate = column.HeaderTemplate != null;
                bool hasGridHeaderCellStyle = DataGrid.HeaderStyle != null;
                bool hasGridHeaderTemplate = DataGrid.HeaderTemplate != null;

                if (!hasColumnHeaderCellStyle && !hasColumnHeaderTemplate && !hasGridHeaderCellStyle && !hasGridHeaderTemplate)
                    return;
                
                control.Style = null;
                control.ContentTemplate = null;

                if (hasColumnHeaderCellStyle)
                    newStyle = column.HeaderStyle;
                else if (hasGridHeaderCellStyle)
                    newStyle = DataGrid.HeaderStyle;

                control.Style = newStyle;

                if (hasColumnHeaderTemplate)
                    newTemplate = column.HeaderTemplate;
                else if (hasGridHeaderTemplate)
                    newTemplate = DataGrid.HeaderTemplate;

                control.ContentTemplate = newTemplate;
            }
        }
       

        //protected override void OnUnwireUIElement(GridHeaderCellControl uiElement)
        //{
        //    base.OnUnwireUIElement(uiElement);
        //    uiElement.ClearValue(GridHeaderCellControl.ContentProperty);
        //}
    }

    public class GridStackedHeaderCellRenderer : GridVirtualizingCellRenderer<GridStackedHeaderCellControl, GridStackedHeaderCellControl>
    {
        public GridStackedHeaderCellRenderer()
        {
            SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
        }

        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, GridStackedHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
            throw new NotImplementedException();
        }

        public override void OnUpdateDisplayBinding(RowColumnIndex cellRowcolumnIndex, GridStackedHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
            throw new NotImplementedException();
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridStackedHeaderCellControl uiElement, GridColumn column, object dataContext)
        {
            var bind = new Binding { Path = new PropertyPath("HeaderText"), Mode = BindingMode.TwoWay };
            uiElement.SetBinding(ContentControl.ContentProperty, bind);
            uiElement.DataContext = dataContext;
        }
        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridStackedHeaderCellControl element, GridColumn column, object dataContext)
        {
            element.ClearValue(GridStackedHeaderCellControl.ContentProperty);
            var bind = new Binding { Path = new PropertyPath("HeaderText"), Mode = BindingMode.TwoWay };
            element.SetBinding(GridHeaderCellControl.ContentProperty, bind);
        }
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            //Since We don't want to Initialize Custom Style for Stacked Header Style, We are blocking the Call to the Method.
            //base.InitializeRendererCellStyle(cellRowColumnIndex, record, cell, column);
        }
    }
}
