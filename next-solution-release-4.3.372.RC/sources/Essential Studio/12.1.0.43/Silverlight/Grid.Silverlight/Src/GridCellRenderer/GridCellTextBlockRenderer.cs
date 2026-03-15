#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media.Imaging;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridStaticCellModel : GridCellModel<GridCellTextBlockRenderer>
    {
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellTextBlockModel : GridCellModel<GridCellTextBlockRenderer>
    {
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellTextBlockRenderer : GridVirtualizingCellRenderer<Border>
    {
        TextBlock tb;
        public GridCellTextBlockRenderer()
        {
            AllowRecycle = true;
            IsControlTextShown = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsEditable = false;
        }

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="textBlock">The text block.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(Border border, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(border, style);
            // set the background to enable hit-testing in the border element
            border.Background = new SolidColorBrush(Colors.Transparent);
            var textBlock = new TextBlock();
            border.Child = textBlock;
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            tb = textBlock;
            if (tb != null)
            {
                var font = style.ReadOnlyFont;
                tb.FontFamily = font.FontFamily;
                tb.FontSize = font.FontSize;
                tb.FontStretch = font.FontStretch;
                tb.FontWeight = font.FontWeight;
                tb.FontStyle = font.FontStyle;
                tb.Foreground = style.Foreground;
                tb.HorizontalAlignment = style.HorizontalAlignment;
                Thickness margins = style.TextMargins.ToThickness();
#if!WinRT
                if (style.HasImageIndex)
                {
                    tb.Margin = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
                }
                else
                {
                    tb.Margin = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
                }
                tb.TextDecorations = font.TextDecorations;
#endif
                tb.Padding = style.BorderMargins.ToThickness();
                tb.TextWrapping = style.TextWrapping;
                tb.TextTrimming = style.TextTrimming;
                tb.VerticalAlignment = style.VerticalAlignment;
            }
            
            textBlock.Text = GetControlText(style);
            
            VisualContainer.SetWantsMouseInput(textBlock, false);
            VisualContainer.SetWantsMouseInput(border, false);
        }

        protected override void OnArrange(ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
           // RotateTextBlock(aca, style);
            base.OnArrange(aca, style);
            
        }
        protected override string GetControlTextFromEditorCore(Border border)
        {
            if (border.Child != null)
            {
                var textBlock = border.Child as TextBlock;
                return textBlock.Text;
            }

            return string.Empty;
        }
        protected override void Dispose(bool disposing)
        {
            tb = null;
            //this.CurrentCell.Dispose();
        }
    }
}
