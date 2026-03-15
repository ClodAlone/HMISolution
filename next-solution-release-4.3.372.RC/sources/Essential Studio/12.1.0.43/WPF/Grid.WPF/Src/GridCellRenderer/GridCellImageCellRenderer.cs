#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Cells;

    /// <summary>
    /// Implements the model part of an image cell.
    /// </summary>
    public class GridCellImageCellModel : GridCellModel<GridCellImageCellRenderer>
    {

        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
            {

            if (text == "" || text == null)
                {
                style.CellValue = null;
                }
            else
                {
               
                style.CellValue = new BitmapImage(new Uri("@" + text, UriKind.Relative));
                }
            return true;
            
            }

    }

    /// <summary>
    /// Render images using this cell renderer. Image should be supplied as a
    /// BitmapImage instance to the GridStyleInfo.CellValue.
    /// <para></para>
    /// <code lang="C#">            
    ///             var imageStyle = this.grid.Model[1, 1];
    ///             imageStyle.CellType = &quot;ImageCell&quot;;
    ///             imageStyle.CellValue = new BitmapImage(new
    /// Uri(&quot;pack://application:,,,/SampleImage.png&quot;));
    /// </code>
    /// <para></para>
    /// </summary>
    public class GridCellImageCellRenderer : GridCellRendererBase
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellImageCellRenderer"/>.
        /// </summary>
        public GridCellImageCellRenderer()
        {
        }

        private void RenderDrawingBrush(DrawingContext dc, DrawingBrush brush, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            //var group = brush.Drawing as DrawingGroup;
            var border = new Border()
            {
                Width = rca.CellRect.Width,
                Height = rca.CellRect.Height
            };
            border.Measure(new Size(rca.CellRect.Width, rca.CellRect.Height));
            border.Arrange(rca.CellRect);
            border.UpdateLayout();
            border.Background = brush;

            this.RenderVisual(dc, border, rca, style);
            //if (group != null)
            //{
            //    this.DrawRecursively(dc, group);
            //}
        }

        private void DrawRecursively(DrawingContext dc, DrawingGroup dGroup)
        {
            //for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            //{
            //    var child = VisualTreeHelper.GetChild(visual, i) as Visual;
            //    this.DrawRecursively(dc, child, rca, style);
            //}
            //if (visual != null)
            //{
            //DrawingGroup dGroup = VisualTreeHelper.GetDrawing(visual);
            if (dGroup != null)
            {
                dGroup.Children.ForEach<Drawing>(d =>
                {
                    if (d is DrawingGroup)
                    {
                        this.DrawRecursively(dc, d as DrawingGroup);
                    }

                    dc.DrawDrawing(d);
                });
            }
            //}
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
            {
                return;
            }

            var brush = style.CellValue as DrawingBrush;
            if (brush != null)
            {
                this.RenderDrawingBrush(dc, brush, rca, style);
                return;
            }

            var drawingImage = style.CellValue as DrawingImage;
            if (drawingImage != null)
            {
                RenderImage(dc, drawingImage, rca, style);
            }

            var visual = style.CellValue as Visual;
            if (visual != null)
            {
                this.RenderVisual(dc, visual, rca, style);
                return;
            }

            var image = style.CellValue as BitmapImage;
            if (image != null)
            {
                this.RenderImage(dc, image, rca, style);
            }
        }

        private void RenderVisual(DrawingContext dc, Visual visual, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)rca.CellRect.Width, (int)rca.CellRect.Height, 96, 96, PixelFormats.Default);
            bmp.Render(visual);
            this.RenderImage(dc, bmp, rca, style);
        }

        private void RenderImage(DrawingContext dc, ImageSource image, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            Rect imageRect = new Rect(rca.CellRect.X, rca.CellRect.Y, 
                                      style.HasImageWidth ? style.GetImageWidth() : rca.CellRect.Width, 
                                      style.HasImageHeight ? style.GetImageHeight() : rca.CellRect.Height);

            imageRect = rca.SubtractBorderMargins(imageRect, style.TextMargins.ToThickness());
            imageRect = Rect.Intersect(imageRect, rca.CellRect);
           
            switch (style.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    imageRect.X = rca.CellRect.X + 1;
                    break;
                case HorizontalAlignment.Right:
                    imageRect.X += rca.CellRect.Width > imageRect.Width ? (rca.CellRect.Width - imageRect.Width) : 0;
                    break;
                case HorizontalAlignment.Stretch:
                    imageRect.Width = rca.CellRect.Width;
                    break;
                case HorizontalAlignment.Center:
                default:
                    imageRect.X += rca.CellRect.Width > imageRect.Width ? (rca.CellRect.Width - imageRect.Width) / 2 : 0;
                    break;
            }

            switch (style.VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    imageRect.Y += rca.CellRect.Height > imageRect.Height ? (rca.CellRect.Height - imageRect.Height): 0 ;
                    break;
                case VerticalAlignment.Stretch:
                    imageRect.Height = rca.CellRect.Height;
                    break;
                case VerticalAlignment.Top:
                    imageRect.Y = rca.CellRect.Y;
                    break;
                case VerticalAlignment.Center:    
                default:
                    imageRect.Y += rca.CellRect.Height > imageRect.Height ? (rca.CellRect.Height - imageRect.Height) / 2 : 0;
                    break;
            }

            dc.DrawImage(image, imageRect);
        }

        protected override void OnActivated()
        {
            ///To set the current cell background, while pressing the left and right arrow key.
            if (this.CurrentCellUIElement == null)
            {
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            }
        }

        protected override void OnDeactivated()
        {
            ///To remove the current cell background, while pressing the left and right arrow key.
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }
    }

    /// <summary>
    /// Implements the model part of an image stream cell.
    /// </summary>
    public class GridCellImageStreamCellModel : GridCellModel<GridCellImageStreamCellRenderer>
    {
    }

    /// <summary>
    /// Implements the renderer part of an image stream cell.
    /// </summary>
    public class GridCellImageStreamCellRenderer : GridCellRendererBase
    {

        protected override void OnRender(System.Windows.Media.DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            var bytes = style.CellValue as byte[];
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            var ms = new MemoryStream(bytes);
            ms.Seek(0, SeekOrigin.Begin);
            bitImg.StreamSource = ms;
            bitImg.EndInit();
            Thickness margins = style.TextMargins.ToThickness();
            Rect imageRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            dc.DrawImage(bitImg, imageRectangle);
            if(style.FlowDirection == FlowDirection.RightToLeft)
            dc.PushTransform(new ScaleTransform(-1,1,imageRectangle.X+((imageRectangle.X - imageRectangle.Width)/2) , imageRectangle.Y+((imageRectangle.Y - imageRectangle.Height)/2)));
        }
    }

    /// <summary>
    /// Renders image borders.
    /// </summary>
    public class GridCellImageContentRenderer : GridVirtualizingCellRenderer<Border>
    {
        /// <summary>
        /// Initializes the new <see cref="GridCellImageContentRenderer"/> object.
        /// </summary>
        public GridCellImageContentRenderer()
        {
            //this.AllowRecycle = false;
            //this.SupportsRenderOptimization = false;
            //this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.AllowRecycle = false;
        }

        /// <summary>
        /// Initializes the borders for the image cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The border.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(Border uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            var brush = style.CellValue as Brush;
            if (brush != null)
            {
                Thickness margins = style.TextMargins.ToThickness();

                // TextBoxView always seems to have this margin and I am not able to reset the margin.
                // Therefore I am also hard-codeing it here so that TextBox behavior is properly
                // emulated.
                margins.Left = Math.Max(0, margins.Left - 2);
                margins.Right = Math.Max(0, margins.Right - 2);
                uiElement.Padding = margins;
                uiElement.Background = brush;
                if (style.FlowDirection == FlowDirection.RightToLeft)
                    {
                    uiElement.FlowDirection = style.FlowDirection;
              
                    double m11 = -1;
                    double m22 = 1;
                    double offsetX = uiElement.Width;
                    double offsetY = 0;
                    uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
                    }
                else
                    {
                    uiElement.LayoutTransform = MatrixTransform.Identity;
                    }
            }

            uiElement.BorderThickness = new Thickness(0);
        }
    }
}
