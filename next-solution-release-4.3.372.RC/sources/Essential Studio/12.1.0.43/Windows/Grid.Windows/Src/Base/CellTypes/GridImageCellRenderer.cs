//-------------------------------------------------------------------------------------------------
// <copyright file="GridImageCellRenderer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridImageUtil : ImageUtil
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridImageUtil()
            : base()
        {
        }

        static Rectangle CenterInRectDontStretch(Rectangle rect, Size size)
        {
            int dx = 0;
            if (size.Width < rect.Width)
            {
                dx = rect.Width - size.Width;
            }

            int dy = 0;
            if (size.Height < rect.Height)
            {
                dy = rect.Height - size.Height;
            }

            return new Rectangle(rect.Left + (dx / 2), rect.Top + (dy / 2), size.Width, size.Height);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void DrawImage(Image image, Rectangle clipRectangle, System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, Syncfusion.Windows.Forms.Grid.GridStyleInfo style, bool isTextRightToLeft)
        {
            DrawImage(image, clipRectangle, g, clientRectangle, style, isTextRightToLeft, null, -1);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void DrawImage(Image image, Rectangle clipRectangle, System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, Syncfusion.Windows.Forms.Grid.GridStyleInfo style, bool isTextRightToLeft, ImageList imageList, int imageIndex)
        {
            if (imageList != null && imageIndex >= 0)
            {
                image = imageList.Images[imageIndex];
            }

            if (image != null)
            {
                System.Drawing.GraphicsUnit gu = System.Drawing.GraphicsUnit.Point;

                RectangleF srcRect = image.GetBounds(ref gu);
                Rectangle destRect = Rectangle.Empty;

                Region saveRegion = null;

                bool checkAlign = false;
                GridImageSizeMode sizeMode = style.ImageSizeMode;

                switch (sizeMode)
                {
                    case GridImageSizeMode.StretchImage:
                        destRect = clientRectangle;
                        break;

                    case GridImageSizeMode.Normal:
                        destRect = new Rectangle(clientRectangle.X, clientRectangle.Y, (int)srcRect.Width, (int)srcRect.Height);
                        destRect = CenterInRectDontStretch(clientRectangle, destRect.Size);
                        checkAlign = true;
                        break;

                    case GridImageSizeMode.CenterImage:
                        {
                            destRect = new Rectangle(clientRectangle.X, clientRectangle.Y, (int)srcRect.Width, (int)srcRect.Height);
                            if (destRect.Width > clientRectangle.Width || destRect.Height > clientRectangle.Height)
                            {
                                float srcRatio = srcRect.Width / srcRect.Height;
                                float tarRatio = (float)clientRectangle.Width / clientRectangle.Height;
                                destRect = clientRectangle;
                                if (tarRatio < srcRatio)
                                {
                                    destRect.Height = (int)(destRect.Width * srcRatio);
                                }
                                else
                                {
                                    destRect.Width = (int)(destRect.Height * srcRatio);
                                }
                            }

                            destRect = CenterInRectDontStretch(clientRectangle, destRect.Size);
                        }

                        break;

                    case GridImageSizeMode.AutoSize:
                        {
                            float srcRatio = srcRect.Width / srcRect.Height;
                            float tarRatio = (float)clientRectangle.Width / clientRectangle.Height;
                            destRect = clientRectangle;
                            if (tarRatio < srcRatio)
                            {
                                destRect.Height = (int)(destRect.Width * srcRatio);
                            }
                            else
                            {
                                destRect.Width = (int)(destRect.Height * srcRatio);
                            }

                            checkAlign = true;
                        }

                        break;

                    default:
                        break;
                }

                if ((!clipRectangle.Contains(clientRectangle) && clipRectangle.IntersectsWith(clientRectangle))
                    || destRect.Height > clientRectangle.Height || destRect.Width > clientRectangle.Width)
                {
                    clipRectangle.Intersect(clientRectangle);
                    saveRegion = g.Clip;
                    g.Clip = new Region(clipRectangle);
                }

                if (!destRect.IsEmpty)
                {
                    if (checkAlign)
                    {
                        if (destRect.Width < clientRectangle.Width)
                        {
                            if (isTextRightToLeft)
                            {
                                switch (style.HorizontalAlignment)
                                {
                                    case GridHorizontalAlignment.Right:
                                        destRect.X = clientRectangle.X;
                                        break;

                                    case GridHorizontalAlignment.Center:
                                        destRect.X = clientRectangle.X + ((clientRectangle.Width - destRect.Width) / 2);
                                        break;

                                    case GridHorizontalAlignment.Left:
                                        destRect.X = clientRectangle.X + (clientRectangle.Width - destRect.Width);
                                        break;
                                }
                            }
                            else
                            {
                                switch (style.HorizontalAlignment)
                                {
                                    case GridHorizontalAlignment.Left:
                                        destRect.X = clientRectangle.X;
                                        break;
                                    case GridHorizontalAlignment.Center:
                                        destRect.X = clientRectangle.X + ((clientRectangle.Width - destRect.Width) / 2);
                                        break;

                                    case GridHorizontalAlignment.Right:
                                        destRect.X = clientRectangle.X + (clientRectangle.Width - destRect.Width);
                                        break;
                                }
                            }
                        }

                        if (destRect.Height < clientRectangle.Height)
                        {
                            switch (style.VerticalAlignment)
                            {
                                case GridVerticalAlignment.Top:
                                    destRect.Y = clientRectangle.Y;
                                    break;

                                case GridVerticalAlignment.Middle:
                                    destRect.Y = clientRectangle.Y + ((clientRectangle.Height - destRect.Height) / 2);
                                    break;

                                case GridVerticalAlignment.Bottom:
                                    destRect.Y = clientRectangle.Y + (clientRectangle.Height - destRect.Height);
                                    break;
                            }
                        }
                    }

                    if (imageList != null)
                    {
                        // || originalClip != null) 
                        if (g.DpiY > 96 || GridControlBase.UseImageListDrawing)
                        {
                            // workaround for printing ... otherwise ImageList draws image in top left corner.
                            Rectangle rectangle = destRect;
                            rectangle.Offset(-rectangle.X, -rectangle.Y);
                            Bitmap bm = new Bitmap(destRect.Width, destRect.Height);
                            Graphics bmg = Graphics.FromImage(bm);
                            DrawingUtils.DrawImageViaImageList(bmg, imageList, imageIndex, rectangle);
                            bmg.Dispose();
                            g.DrawImageUnscaled(bm, destRect);
                        }
                        else if (destRect.Size == srcRect.Size)
                        {
                            // This method will use ImageList_DrawEx to draw the image (to use the transparency info in the embedded images).
                            // We use the PInvoke rather than ImageList.Draw because, the Draw method
                            // uses the PaintEventArgs.ClipRectangle rather than g.ClipBounds (both
                            // can be different) and here we force the ClipBounds on the DC.
                            DrawingUtils.DrawImageViaImageList(g, imageList, imageIndex, destRect);
                        }
                        else
                        {
                            // Above code has problems if destRect.Size != srcRect.Size
                            g.DrawImage(image, destRect, srcRect, gu);
                        }
                    }
                    else
                    {
                        // regular image. Much easier ...
                        g.DrawImage(image, destRect, srcRect, gu);
                    }
                }

                if (saveRegion != null)
                {
                    g.Clip = saveRegion;
                }
                ////                    if(cellDrawOption == GridImageCellDrawOption.FitToCell)
                ////                        g.DrawImage(image, clientRectangle);
                ////                    else if(cellDrawOption == GridImageCellDrawOption.NoResize)
                ////                    {
                ////                        RectangleF srcRect = image.GetBounds(ref gu);
                ////                        Rectangle destRect = new Rectangle(clientRectangle.X, clientRectangle.Y, (int) srcRect.Width, (int) srcRect.Height);
                ////                        g.DrawImage(image,  destRect, srcRect, gu);
                ////                    }
            }
        }
    }

    /// <summary>
    /// Specifies the scaling of images in a cell.
    /// </summary>
    public enum GridImageSizeMode
    {
        /// <summary>
        /// Don't scale.
        /// </summary>
        Normal,

        /// <summary>
        /// Center in cell.
        /// </summary>
        CenterImage,

        /// <summary>
        /// Make same size as cell but keep height / width ratio proportionally.
        /// </summary>
        AutoSize,

        /// <summary>
        /// Make same size as cell.
        /// </summary>
        StretchImage,
    }

    /// <summary>
    /// Implements the data / model part for an image cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridImageCellModel"/> can serve as model for several <see cref="GridImageCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridImageCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridImageCellModel : GridStaticCellModel
    {
        /// <summary>
        /// Initializes a new <see cref="GridImageCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridImageCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <overload>
        /// Initializes a new <see cref="GridImageCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridImageCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridImageCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridImageCellRenderer(control, this);
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Image image = ImageUtil.ConvertToImage(style.CellValue);

            if (image == null)
            {
                return base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            }

            return image.Size;
        }
    }

    /// <summary>
    /// Implements the renderer part of an image cell.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridImageCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// </remarks>
    public class GridImageCellRenderer : GridStaticCellRenderer
    {
        /// <summary>
        /// Initializes a new GridImageCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridImageCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (clientRectangle.IsEmpty)
            {
                return;
            }

            object cellValue = style.CellValue;
            Image image = ImageUtil.ConvertToImage(cellValue);
            ImageList styleImageList = null;
            int index = -1;

            if (image == null)
            {
                // Prefer cell Value over ImageIndex.
                styleImageList = style.ImageList;
                index = style.ImageIndex;
                if (styleImageList != null)
                {
                    if (index < 0 && cellValue != null)
                    {
                        try
                        {
                            if (!(cellValue is string) || ((string)cellValue) != string.Empty)
                            {
                                index = Convert.ToInt32(cellValue);
                            }
                        }
                        catch
                        {
                        }
                    }

                    // No valid cell value, check ImageIndex.
                    if (index < 0 || index >= styleImageList.Images.Count)
                    {
                        index = style.ImageIndex;
                    }

                    if (index >= 0 && index < styleImageList.Images.Count)
                    {
                        image = styleImageList.Images[index];
                    }
                }
            }

            Rectangle clipRectangle = this.GetCellBoundsCoreInt(rowIndex, colIndex, false);
            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
            GridImageUtil.DrawImage(image, clipRectangle, g, clientRectangle, style, isTextRightToLeft, styleImageList, index);
        }
    }
}
