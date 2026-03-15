#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using Syncfusion.Windows.Controls.Scroll;

    public class GridTreeHeaderCellModel : GridCellModel<GridTreeHeaderCellRenderer>
    {
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
            {
                return Size.Empty;
            }

            Thickness margins = style.TextMargins.ToThickness();

            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            size = this.AddSortVisibilityToSize(size, style);
            size.Width = Math.Ceiling(size.Width);
            size.Height = Math.Ceiling(size.Height);

            return size;
        }

        private Size AddSortVisibilityToSize(Size size, GridStyleInfo style)
        {
            if (style.Tag != null)
            {
                size.Width += GridTreeHeaderCellControl.MinWidth;
                    //GridDataHeaderCellControl.MinWidth;
                return size;
            }

            return size;
        }
    }

    public class GridTreeHeaderCellRenderer : GridVirtualizingCellRenderer<GridTreeHeaderCellControl>
    {
        public GridTreeHeaderCellRenderer()
        {
            this.AllowRecycle = false;
            this.SupportsRenderOptimization = false;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.IsEditable = false;
        }

        public override void OnInitializeContent(GridTreeHeaderCellControl headerCellControl, GridRenderStyleInfo style)
        {
            var treeGrid = this.GridControl.FindParentElementOfType<GridTreeControl>();
            headerCellControl.IsInSuspend = true;
            this.OnUnwireUIElement(headerCellControl);
            VisualContainer.SetWantsMouseInput(headerCellControl, false);
            base.OnInitializeContent(headerCellControl, style);
            headerCellControl.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            headerCellControl.RenderStyle = style;
            //headerCellControl.HeaderInnerBorderBrush = treeGrid.InternalGrid.GetGridTreeHeaderInnerBorderBrush(VisualStyle);
            //headerCellControl.HeaderInnerBorderThickness = treeGrid.InternalGrid.GetGridTreeHeaderInnerBorderThickness(VisualStyle);
            headerCellControl.HoverBackground = treeGrid.InternalGrid.GetGridTreeHeaderHoverBackgroundBrush(VisualStyle);
            headerCellControl.HoverForeground = treeGrid.InternalGrid.GetGridTreeHeaderHoverForegroundBrush(VisualStyle);
            headerCellControl.SortWidgetBorderBrush = treeGrid.InternalGrid.GetGridTreeSortWidgetBorderBrush(VisualStyle);
            headerCellControl.SortWidgetBorderHoverBackgroundBrush = treeGrid.InternalGrid.GetGridTreeSortWidgetBorderHoverBackgroundBrush(VisualStyle);
            headerCellControl.SortBrush = treeGrid.InternalGrid.GetGridTreeSortWidgetBrush(VisualStyle);
            headerCellControl.Foreground = treeGrid.InternalGrid.GetGridTreeHeaderForegroundBrush(VisualStyle);
            if (style.Tag != null)
            {
                headerCellControl.SortVisibility = Visibility.Visible;
                var sortDirection = (ListSortDirection)style.Tag;
                if (sortDirection == ListSortDirection.Ascending)
                {
                    headerCellControl.AscVisibility = Visibility.Visible;
                    headerCellControl.DescVisibility = Visibility.Collapsed;
                }
                else if (sortDirection == ListSortDirection.Descending)
                {
                    headerCellControl.AscVisibility = Visibility.Collapsed;
                    headerCellControl.DescVisibility = Visibility.Visible;
                }
                headerCellControl.SortDirection = sortDirection;
            }
            else
            {
                headerCellControl.SortVisibility = Visibility.Collapsed;
                headerCellControl.AscVisibility = Visibility.Collapsed;
                headerCellControl.DescVisibility = Visibility.Collapsed;
            }

            this.OnWireUIElement(headerCellControl);
            headerCellControl.IsInSuspend = false;
        }


        private IGridTreeVisualStyle visualStyle = new GridTreeDefaultGridVisualStyle();

        public IGridTreeVisualStyle VisualStyle
        {
            get { return visualStyle; }
            set { visualStyle = value; }
        }

    }
}
