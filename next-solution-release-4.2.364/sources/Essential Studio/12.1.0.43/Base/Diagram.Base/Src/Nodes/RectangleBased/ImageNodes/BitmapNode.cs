#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Stores and renders a bitmap image.
    /// </summary>
    [Serializable]
    public class BitmapNode
        : Node
    {
        #region Class members
        /// <summary>
        /// Bitmap image reference.
        /// </summary>
        private Bitmap m_bitmap;           
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="stream">IO stream containing the bitmap.</param>
        public BitmapNode(Stream stream)
            : base()
        {
            Bitmap bitmap = new Bitmap(stream);
            InitializeBitmapNode(bitmap, new RectangleF(0, 0, bitmap.Width, bitmap.Height), MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="filename">Name of file to load bitmap from.</param>
        public BitmapNode(string filename)
            : base()
        {
            using (Bitmap bitmap = new Bitmap(filename))
            {
                Bitmap newBitmap = CloneImage(bitmap);
                InitializeBitmapNode(newBitmap, new RectangleF(0, 0, newBitmap.Width, newBitmap.Height), MeasureUnits.Pixel);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="srcbitmap">The bitmap for the node.</param>
        public BitmapNode(Bitmap srcbitmap)
            : base()
        {
            Bitmap bitmap = CloneImage(srcbitmap);
            InitializeBitmapNode(bitmap, new RectangleF(0, 0, bitmap.Width, bitmap.Height), MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="src">The bitmap for the node.</param>
        /// <param name="rcBounds">The node bounds.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public BitmapNode(Bitmap src, RectangleF rcBounds, MeasureUnits measureUnits)
            : base()
        {
            InitializeBitmapNode(CloneImage(src), rcBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="src">The bitmap for the node.</param>
        /// <param name="rcBounds">The node bounds.</param>
        public BitmapNode(Bitmap src, RectangleF rcBounds)
            : base()
        {
            Bitmap newBitmap = CloneImage(src);
            InitializeBitmapNode(newBitmap, rcBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public BitmapNode(BitmapNode src)
            : base(src)
        {
            m_bitmap = CloneImage(src.m_bitmap);

            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapNode"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected BitmapNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_bitmap = (Bitmap)info.GetValue("bitmap", typeof(Bitmap));
        }
        #endregion

        #region Class properties        
        /// <summary>
        /// Gets or sets the <see cref="System.Drawing.Image"/>.
        /// </summary>
        /// <value>The image.</value>
        public Bitmap Image
        {
            get 
            { 
                return m_bitmap; 
            }
            set
            {
                if (m_bitmap != value)
                {
                    m_bitmap = value;
                    //raise property changed event
                    OnPropertyChanged(this.FullContainerName, "Image");
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Renders the shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw interior
            DrawInterior(gfx);

            // 3 - Draw border
            DrawBorder(gfx);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new BitmapNode(this);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("bitmap", m_bitmap);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Clones the image.
        /// </summary>
        /// <param name="src">The original image.</param>
        private Bitmap CloneImage(Bitmap src)
        {
            if (src == null)
                return src;
            Bitmap bitmap = new Bitmap(src.Size.Width, src.Size.Height, src.PixelFormat);
            if (src.Palette.Entries.Length > 0)
                bitmap.Palette = src.Palette;
            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(0, 0, src.Width, src.Height);
            System.Drawing.Imaging.BitmapData bmpData = src.LockBits(bounds, System.Drawing.Imaging.ImageLockMode.ReadWrite, src.PixelFormat);
            System.Drawing.Imaging.BitmapData newBmpData = bitmap.LockBits(bounds, System.Drawing.Imaging.ImageLockMode.ReadWrite, src.PixelFormat);
            IntPtr bPtr = bmpData.Scan0;
            IntPtr nbPtr = newBmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * src.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(bPtr, rgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, nbPtr, bytes);
            bitmap.UnlockBits(newBmpData);
            src.UnlockBits(bmpData);
            return bitmap;
        }

        /// <summary>
        /// Draws the image border.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        private void DrawBorder(Graphics gfx)
        {
            if (this.LineStyle.LineWidth > 0)
            {
                using (Pen pen = this.LineStyle.CreatePen())
                    gfx.DrawPath(pen, this.GraphicsPath);
            }
        }

        /// <summary>
        /// Draws node's interior on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        private void DrawInterior(Graphics gfx)
        {
            SizeF szSize = MeasureUnitsConverter.ToPixels(this.Size, this.MeasurementUnit);
            gfx.DrawImage(m_bitmap, 0, 0, szSize.Width, szSize.Height);
        }

        /// <summary>
        /// Initializes the bitmap node from bitmap and bounds.
        /// </summary>
        /// <param name="bitmap">The bitmap image.</param>
        /// <param name="rectBounds">The bounds rectangle.</param>
        /// <param name="measureUnits">The bounds rectangle measure units.</param>
        /// <exception cref="System.ArgumentNullException">When bitmap parameter is null.</exception>
        private void InitializeBitmapNode(Bitmap bitmap, RectangleF rectBounds, MeasureUnits measureUnits)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            m_bitmap = (Bitmap)bitmap.Clone();

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;           
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = measureUnits;

            UpdateBoundingRectangle();
        }
        #endregion
    }
}
