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
    using Syncfusion.Windows.GridCommon;

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
                size.Width += GridTreeHeaderCellControl.MinimumWidth;
                return size;
            }

            return size;
        }
    }

    public class GridTreeHeaderCellRenderer : GridVirtualizingCellRenderer<GridTreeHeaderCellControl>
    {
        public GridTreeHeaderCellRenderer()
        {
            this.AllowRecycle = true;
            this.SupportsRenderOptimization = false;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.IsEditable = false;
        }

        

        public override void CreateRendererElement(GridTreeHeaderCellControl headerCellControl, GridRenderStyleInfo style)
        {
            VisualContainer.SetWantsMouseInput(headerCellControl, false);
            GridTreeControl gridTree = this.GridControl.FindParentElementOfType<GridTreeControl>();
            base.CreateRendererElement(headerCellControl, style);
            headerCellControl.IsInSuspend = true;
            this.OnUnwireUIElement(headerCellControl);
           
            headerCellControl.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            headerCellControl.RenderStyle = style;
            //headerCellControl.HeaderInnerBorderBrush = gridTree.InternalGrid.GetGridTreeHeaderInnerBorderBrush(VisualStyle);
            
            if (style.HasFont)
            {
                headerCellControl.FontFamily = style.Font.FontFamily;
                headerCellControl.FontSize = style.Font.FontSize;
            }
            else
            {
                headerCellControl.FontFamily = gridTree.InternalGrid.GetGridTreeHeaderFont(VisualStyle).FontFamily;
                headerCellControl.FontSize = gridTree.InternalGrid.GetGridTreeHeaderFont(VisualStyle).FontSize;
            }

            if (style.HasBackground)
            {
                headerCellControl.Background = style.Background;
                headerCellControl.HoverBackground = GridUtil.GetHoverColor(style.Background, 0.20);
                headerCellControl.SortWidgetBorderHoverBackgroundBrush = headerCellControl.HoverBackground;
                headerCellControl.SortWidgetBorderBrush = style.Background;
            }
            else
            {
                headerCellControl.Background = gridTree.ColumnHeaderStyle.Background;
                headerCellControl.HoverBackground = gridTree.InternalGrid.GetGridTreeHeaderHoverBackgroundBrush(VisualStyle);
                headerCellControl.SortWidgetBorderHoverBackgroundBrush = gridTree.InternalGrid.GetGridTreeSortWidgetBorderHoverBackgroundBrush(VisualStyle);
                headerCellControl.SortWidgetBorderBrush = gridTree.InternalGrid.GetGridTreeSortWidgetBorderBrush(VisualStyle);
            }

            if (style.HasForeground)
            {
                headerCellControl.Foreground = style.Foreground;
                headerCellControl.SortBrush = style.Foreground;
                headerCellControl.HoverForeground = GridUtil.GetHoverColor(style.Foreground, 0.20);
            }
            else
            {
                headerCellControl.Foreground = gridTree.ColumnHeaderStyle.Foreground;
                headerCellControl.SortBrush = gridTree.InternalGrid.GetGridTreeSortWidgetBrush(VisualStyle);
                headerCellControl.HoverForeground = gridTree.InternalGrid.GetGridTreeHeaderHoverForegroundBrush(VisualStyle);
            }

            //if (style.CellItemTemplate != null)
            //{
            //    headerCellControl.ContentDataTemplate = style.CellItemTemplate;
            //}

            if (style.Tag != null)
            {
                if (gridTree.AllowSort)
                    headerCellControl.SortVisibility = Visibility.Visible;
                else
                    headerCellControl.SortVisibility = Visibility.Collapsed;
                var sortDirection = (ListSortDirection)style.Tag;               
                headerCellControl.SortDirection = sortDirection;
            }
            else 
            {
                headerCellControl.SortVisibility = Visibility.Collapsed;
               
            }

            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                headerCellControl.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = headerCellControl.ActualWidth;
                double offsetY = 0;
                headerCellControl.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            }
            else
            {
                headerCellControl.LayoutTransform = MatrixTransform.Identity;
            }

            this.OnWireUIElement(headerCellControl);
            headerCellControl.IsInSuspend = false;
        }

        protected override void OnRenderForPrinting(DrawingContext dc, Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            /// If we resize the column means Text will displace from its origional.
            /// Because While increasing the column size margin.left also increase.
            /// So I have set the Constant value for margin.Left
            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            if (textRectangle.IsEmpty)
                return;

            string text;
            if (IsCurrentCell(style) && HasControlText && this.CurrentCell.IsEditing)
                text = ControlText;
            else
                text = GetControlText(style);

            double dValue = 0;

            if (style.CellValue != null && double.TryParse(style.CellValue.ToString(), out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                }
            }

            if (text.Length != 0 && text.Length > style.MaxLength && style.HasMaxLength)
                text = text.Remove(style.MaxLength);
            // Draw the formatted text string to the DrawingContext of the control.
                        
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        private IGridTreeVisualStyle visualStyle = new GridTreeSunBlackVisualStyle();

        public IGridTreeVisualStyle VisualStyle
        {
            get { return visualStyle; }
            set { visualStyle = value; }
        }

    }
}
