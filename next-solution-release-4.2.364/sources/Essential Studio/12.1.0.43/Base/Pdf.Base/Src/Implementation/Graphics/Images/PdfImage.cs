#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE &&!WP
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;

using Syncfusion.Pdf.Graphics.Images;


namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the base class for images.
    /// </summary>
    public abstract class PdfImage :
        PdfShapeElement,
        IPdfWrapper
    {
#region Fields
        /// <summary>
        /// The stream containing image data.
        /// </summary>
        private PdfStream m_stream;
        /// <summary>
        /// The size of the image in points.
        /// </summary>
        private SizeF m_phisicalDimension;
        /// <summary>
        /// Horizontal image resolution.
        /// </summary>
        private float m_horizontalResolution;
        /// <summary>
        /// Vertical image resolution.
        /// </summary>
        private float m_verticalResolution;
        /// <summary>
        /// Holds mask type flag.
        /// </summary>
        protected bool m_softmask;
        /// <summary>
        /// Holds list of color space
        /// </summary>
        private int[] m_matte;

        internal static string m_tiffPath;
        internal static Stream m_tiffStream;
        private float m_scrollBarWidth;
        private float m_scrollBarHeight;
        #endregion

#region Properties
        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return InternalImage.Height;
            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return InternalImage.Width;
            }
        }

        /// <summary>
        /// Gets the horizontal resolution, in pixels per inch, of this Image. 
        /// </summary>
        /// <value>The horizontal resolution.</value>
        public float HorizontalResolution
        {
            get
            {
                float dpi = (m_horizontalResolution == 0f) ? InternalImage.HorizontalResolution : m_horizontalResolution;

                if (dpi <= 0)
                {
                    dpi = PdfUnitConvertor.HorizontalResolution;
                }

                return dpi;
            }
        }

        /// <summary>
        /// Gets the vertical resolution, in pixels per inch, of this Image. 
        /// </summary>
        /// <value>The vertical resolution.</value>
        public float VerticalResolution
        {
            get
            {
                float dpi = (m_verticalResolution == 0f) ? InternalImage.VerticalResolution : m_verticalResolution;

                if (dpi <= 0)
                {
                    dpi = PdfUnitConvertor.VerticalResolution;
                }

                return dpi;
            }
        }

        /// <summary>
        /// Returns the size of the image in points.
        /// </summary>
        /// <remarks>This property uses HorizontalResolution and VerticalResolution for calculating the size in points.</remarks>
        public virtual SizeF PhysicalDimension
        {
            get
            {
               
               m_phisicalDimension = GetPointSize(Width, Height, HorizontalResolution, VerticalResolution);

               return m_phisicalDimension;
            }
        }
        /// <summary>
        /// Holds list of color space
        /// </summary>
        /// <remarks>An array of component values specifying the matte color 
        /// with which the image data in the parent image has been preblended. 
        /// The array consists of nnumbers, where n is the number of components
        /// in the color space specified by the ColorSpace entry in the parent image�s 
        /// image dictionary; the numbers must be valid color components 
        /// in that color space.</remarks>
        internal int[] Matte
        {
            get
            {
                return m_matte;
            }
            set
            {
                m_matte = value;
            }
        }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        internal abstract Image InternalImage { get; }

        /// <summary>
        /// Gets the image stream.
        /// </summary>
        internal PdfStream Stream
        {
            get
            {
                if (m_stream == null)
                {
                    m_stream = new PdfStream();
                }

                return m_stream;
            }
        }

        /// <summary>
        /// Gets the mask type.
        /// </summary>
        /// <value><c>true</c> if soft mask; otherwise, hard mask <c>false</c>.</value>
        internal bool SoftMask
        {
            get
            {
                return m_softmask;
            }
        }


        internal float ScrollBarWidth
        {
            get
            {
                return m_scrollBarWidth;
            }
            set
            {
                m_scrollBarWidth = value;
            }
        }

        internal float ScrollBarHeight
        {
            get
            {
                return m_scrollBarHeight;
            }
            set
            {
                m_scrollBarHeight = value;
            }
        }
        #endregion

#region Public static methods
        /// <summary>
        /// Creates PdfImage from a file.
        /// </summary>
        /// <param name="path">Path to a file.</param>
        /// <returns>Returns a created PdfImage object.</returns>
        public static PdfImage FromFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Image image = Image.FromFile(Utils.CheckFilePath(path));
            PdfImage img = FromImage(image);
            if (image.RawFormat.Equals(ImageFormat.Tiff))
                m_tiffPath = path;
            return img;
        }

        /// <summary>
        /// Creates PdfImage from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns a created PdfImage object.</returns>
        public static PdfImage FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Image image = Image.FromStream(CheckStreamExistance(stream));
            PdfImage img = FromImage(image);
            if (image.RawFormat.Equals(ImageFormat.Tiff))
                m_tiffStream = stream;
            return img;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> object into a PDF image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Returns a created PdfImage object.</returns>
        public static PdfImage FromImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PdfImage img = null;

            if (image is Metafile)
            {
                img = new PdfMetafile(image as Metafile);
            }
            else
            {
                img = new PdfBitmap(image);
            }

            return img;
        }
#if AllowUnsafeCode 
        /// <summary>
        /// Creates a new image instance from RTF text.
        /// </summary>
        /// <param name="rtf">RTF text data.</param>
        /// <param name="width">Width of the image in points.</param>
        /// <param name="type">Type of the image that should be created.</param>
        /// <returns>PdfImage containing RTF text.</returns>
        public static PdfImage FromRtf(string rtf, float width, PdfImageType type)
        {
            return FromRtf(rtf, width, 0, type);
        }

        /// <summary>
        /// Creates a new image instance from RTF text.
        /// </summary>
        /// <param name="rtf">RTF text data.</param>
        /// <param name="width">Width of the image in points.</param>
        /// <param name="height">Height of the image in points.</param>
        /// <param name="type">Type of the image that should be created.</param>
        /// <returns>PdfImage containing RTF text.</returns>
        public static PdfImage FromRtf(string rtf, float width, float height, PdfImageType type)
        {
            if (rtf == null)
                throw new ArgumentNullException("rtf");

            SizeF pxSize = GetPixelSize(width, height);
            Image img = RtfToImage.ConvertToImage(rtf, pxSize.Width, pxSize.Height, type);

            if (img == null)
                throw new PdfException("Couldn't convert RTF to Image");

            PdfImage pdfImage = FromImage(img);

            // Metafile was created by us and the real resolution is screen's resolution.
            pdfImage.SetResolution(PdfUnitConvertor.HorizontalResolution, PdfUnitConvertor.VerticalResolution);

            return pdfImage;
        }
#endif
#endregion

#region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_stream;
            }
        }
#endregion

#region Implementation
        /// <summary>
        /// Checks the stream existence.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream if it exists.</returns>
        /// <exception cref="ArgumentNullException">It's thrown if the stream is null.</exception>
        internal static Stream CheckStreamExistance(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.Length <= 0)
                throw new ArgumentException("The stream can't be empty", "stream");

            return stream;
        }

        /// <summary>
        /// Calculates the width and height of the image.
        /// </summary>
        /// <param name="width">Width of the image in points.</param>
        /// <param name="height">Height of the image in points.</param>
        /// <returns> Calculates the width and height of the image.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected static SizeF GetPixelSize(float width, float height)
        {
            float dpix = PdfUnitConvertor.HorizontalResolution;
            float dpiy = PdfUnitConvertor.VerticalResolution;

            PdfUnitConvertor convertor = new PdfUnitConvertor(dpix);
            float pxWidth = convertor.ConvertToPixels(width, PdfGraphicsUnit.Point);

            convertor = new PdfUnitConvertor(dpiy);
            float pxHeight = convertor.ConvertToPixels(height, PdfGraphicsUnit.Point);

            SizeF pxSize = new SizeF(pxWidth, pxHeight);
            return pxSize;
        }
        /// <summary>
        /// Saves the image into stream.
        /// </summary>
        internal abstract void Save();

        /// <summary>
        /// Sets the content stream.
        /// </summary>
        /// <param name="content">The content.</param>
        internal void SetContent(IPdfPrimitive content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            if (!(content is PdfStream))
                throw new ArgumentException("The content is not a stream.", "content");

            m_stream = content as PdfStream;
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override void DrawInternal(PdfGraphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            graphics.DrawImage(this, PointF.Empty);
        }

        /// <summary>
        /// Gets bounds of image.
        /// </summary>
        /// <remarks>The DPI is standard, not image DPI.</remarks>
        /// <returns>Bounds of image.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override RectangleF GetBoundsInternal()
        {
            return new RectangleF(PointF.Empty, PhysicalDimension);
        }

        /// <summary>
        /// Calculates size of the image in points.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <returns>size of the image in points.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal SizeF GetPointSize(float width, float height)
        {
            float dpiX = PdfUnitConvertor.HorizontalResolution;
            float dpiY = PdfUnitConvertor.VerticalResolution;
            SizeF size = GetPointSize(width, height, dpiX, dpiY);

            return size;
        }
        /// <summary>
        /// Calculates size of the image in points.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="horizontalResolution">Horizontal resolution.</param>
        /// <param name="verticalResolution">Vertical resolution.</param>
        /// <returns>size of the image in points.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected internal SizeF GetPointSize(float width, float height, float horizontalResolution, float verticalResolution)
        {
            PdfUnitConvertor ucX = new PdfUnitConvertor(horizontalResolution);
            PdfUnitConvertor ucY = new PdfUnitConvertor(verticalResolution);

            float ptWidth = ucX.ConvertUnits(width, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
            float ptHeight = ucY.ConvertUnits(height, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
            SizeF size = new SizeF(ptWidth, ptHeight);

            return size;
        }
        /// <summary>
        /// Sets resolution of the image.
        /// </summary>
        /// <param name="horizontalResolution">Horizontal resolution of the image.</param>
        /// <param name="verticalResolution">Vertical resolution of the image.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected void SetResolution(float horizontalResolution, float verticalResolution)
        {
            m_horizontalResolution = horizontalResolution;
            m_verticalResolution = verticalResolution;
        }
#endregion
    }
}
#else
using System;
using System.Drawing;
using System.IO;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Graphics.Images.Decoder;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the base class for images.
    /// </summary>
    public abstract class PdfImage :
        PdfShapeElement,
        IPdfWrapper
    {
        #region Properties
        public int Height { get; internal set; }
        public int Width { get; internal set; }
        public float HorizontalResolution { get; internal set; }
        public float VerticalResolution { get; internal set; }
        public virtual SizeF PhysicalDimension { get { return new SizeF(Width, Height); } }
        internal PdfStream ImageStream { get; set; }
        private float m_scrollBarWidth;
        private float m_scrollBarHeight;
        internal float ScrollBarWidth
        {
            get
            {
                return m_scrollBarWidth;
            }
            set
            {
                m_scrollBarWidth = value;
            }
        }
        internal float ScrollBarHeight
        {
            get
            {
                return m_scrollBarHeight;
            }
            set
            {
                m_scrollBarHeight = value;
            }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Creates PdfImage from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns a created PdfImage object.</returns>
        public static PdfImage FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            PdfImage image = null;
            image = new PdfBitmap(stream);

            return image;
        }


        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return ImageStream;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Checks the stream existence.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream if it exists.</returns>
        /// <exception cref="ArgumentNullException">It's thrown if the stream is null.</exception>
        internal static Stream CheckStreamExistance(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.Length <= 0)
                throw new ArgumentException("The stream can't be empty", "stream");

            return stream;
        }

        /// <summary>
        /// Calculates the width and height of the image.
        /// </summary>
        /// <param name="width">Width of the image in points.</param>
        /// <param name="height">Height of the image in points.</param>
        /// <returns> Calculates the width and height of the image.</returns>
        protected static SizeF GetPixelSize(float width, float height)
        {
            float dpix = PdfUnitConvertor.HorizontalResolution;
            float dpiy = PdfUnitConvertor.VerticalResolution;

            PdfUnitConvertor convertor = new PdfUnitConvertor(dpix);
            float pxWidth = convertor.ConvertToPixels(width, PdfGraphicsUnit.Point);

            convertor = new PdfUnitConvertor(dpiy);
            float pxHeight = convertor.ConvertToPixels(height, PdfGraphicsUnit.Point);

            SizeF pxSize = new SizeF(pxWidth, pxHeight);
            return pxSize;
        }
        /// <summary>
        /// Saves the image into stream.
        /// </summary>
        internal abstract void Save();

        /// <summary>
        /// Sets the content stream.
        /// </summary>
        /// <param name="content">The content.</param>
        internal void SetContent(IPdfPrimitive content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            if (!(content is PdfStream))
                throw new ArgumentException("The content is not a stream.", "content");


            this.ImageStream = content as PdfStream;
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        protected override void DrawInternal(PdfGraphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            graphics.DrawImage(this, PointF.Empty);
        }

        /// <summary>
        /// Gets bounds of image.
        /// </summary>
        /// <remarks>The DPI is standard, not image DPI.</remarks>
        /// <returns>Bounds of image.</returns>
        protected override RectangleF GetBoundsInternal()
        {
            return new RectangleF(PointF.Empty, PhysicalDimension);
        }

        /// <summary>
        /// Calculates size of the image in points.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <returns>size of the image in points.</returns>
        protected internal SizeF GetPointSize(float width, float height)
        {
            float dpiX = PdfUnitConvertor.HorizontalResolution;
            float dpiY = PdfUnitConvertor.VerticalResolution;
            SizeF size = GetPointSize(width, height, dpiX, dpiY);

            return size;
        }
        /// <summary>
        /// Calculates size of the image in points.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="horizontalResolution">Horizontal resolution.</param>
        /// <param name="verticalResolution">Vertical resolution.</param>
        /// <returns>size of the image in points.</returns>
        protected internal SizeF GetPointSize(float width, float height, float horizontalResolution, float verticalResolution)
        {
            PdfUnitConvertor ucX = new PdfUnitConvertor(horizontalResolution);
            PdfUnitConvertor ucY = new PdfUnitConvertor(verticalResolution);

            float ptWidth = ucX.ConvertUnits(width, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
            float ptHeight = ucY.ConvertUnits(height, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Point);
            SizeF size = new SizeF(ptWidth, ptHeight);

            return size;
        }
        /// <summary>
        /// Sets resolution of the image.
        /// </summary>
        /// <param name="horizontalResolution">Horizontal resolution of the image.</param>
        /// <param name="verticalResolution">Vertical resolution of the image.</param>
        protected void SetResolution(float horizontalResolution, float verticalResolution)
        {
            this.HorizontalResolution = horizontalResolution;
            this.VerticalResolution = verticalResolution;
        }
        #endregion
    }
}

#endif