#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Syncfusion.Pdf.Graphics.Images;
using Syncfusion.Pdf.Graphics.Images.Metafiles;
using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class representing metafiles.
    /// </summary>
#if AllowUnsafeCode
    public class PdfMetafile
        : PdfImage,
        IPdfWrapper,
        IDisposable
#else
    internal class PdfMetafile
		: PdfImage,
		IPdfWrapper,
		IDisposable
#endif
    {
#region Fields
        /// <summary>
        /// Holds image.
        /// </summary>
        private Metafile m_image;
        /// <summary>
        /// Holds template for metafile.
        /// </summary>
        private PdfTemplate m_template;
        /// <summary>
        /// Indicates if the metafile has been parsed.
        /// </summary>
        private bool m_bSaved;
        /// <summary>
        /// Contains the information about the text regions in the metafile.
        /// </summary>
        private TextRegionManager m_textRegions;
        /// <summary>
        /// Contains the information about the image regions in the metafile.
        /// </summary>
        private ImageRegionManager m_imageRegions;
        /// <summary>
        /// Indicates if the object has been disposed.
        /// </summary>
        private bool m_bDisposed;
        /// <summary>
        /// Indicates whether we should dispose image or not.
        /// </summary>
        private Metafile m_originalImage;
        /// <summary>
        /// Indicates the quality of the image.
        /// </summary>
        private long m_quality = 100;
        /// <summary>
        /// Indicates the image resolution
        /// </summary>
        private int m_imageResolution = 0;
        /// <summary>
        /// Contains the html hyperlink
        /// </summary>
        private ArrayList m_htmlHyperlinks = new ArrayList();
        /// <summary>
        /// Contains document links.
        /// </summary>
        private ArrayList m_documentLinks = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        private float m_pageScale = 1f;
        /// <summary>
        /// 
        /// </summary>
        private GraphicsUnit m_pageUnit = GraphicsUnit.Display;

        /// <summary>
        /// Internal varible to store the alpha pen.
        /// </summary>
        private float m_alphaPen = 1.0f;

        /// <summary>
        /// Internal varible to store the alpha brush.
        /// </summary>
        private float m_alphaBrush = 1.0f;

        /// <summary>
        /// Internal varible to store tranaparency is applied or not.
        /// </summary>
        private bool m_bIsTransparency = false;

        /// <summary>
        /// Internal varible to store the pdf blend mode.
        /// </summary>
        private PdfBlendMode m_blendMode=PdfBlendMode.Normal;
        private bool m_isImagePath;
        #endregion

#region Properties
        internal bool IsImagePath
        {
            get
            {
                return m_isImagePath;
            }
            set
            {
                m_isImagePath = value;
            }
        }
        /// <summary>
        ///Get or Sets transparency is applied or not.
        /// </summary>
        internal bool IsTranparency
        {
            get
            {
                return m_bIsTransparency;
            }
            set
            {
                m_bIsTransparency = value;
            }
        }

        /// <summary>
        ///Get or Sets the alpha pen
        /// </summary>
        internal float AlphaPen
        {
            get
            {
                return m_alphaPen;
            }
            set
            {
                m_alphaPen = value;
            }
        }

        /// <summary>
        ///Get or Sets the alpha brush.
        /// </summary>
        internal float AlphaBrush
        {
            get
            {
                return m_alphaBrush;
            }
            set
            {
                m_alphaBrush = value;
            }
        }

        /// <summary>
        ///Get or Sets the Blend mode.
        /// </summary>
        internal PdfBlendMode BlendMode
        {
            get
            {
                return m_blendMode;
            }
            set
            {
                m_blendMode = value;
            }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        internal override Image InternalImage
        {
            get
            {
                return m_image;
            }
        }

        /// <summary>
        /// Gets the information about the text regions in the metafile.
        /// </summary>
        internal TextRegionManager TextRegions
        {
            get
            {
                return m_textRegions;
            }
        }

        /// <summary>
        /// Gets the information about the image regions in the metafile.
        /// </summary>
        internal ImageRegionManager ImageRegions
        {
            get
            {
                return m_imageRegions;
            }
        }

        /// <summary>
        /// Returns the internal template.
        /// </summary>
        internal PdfTemplate Template
        {
            get
            {
                return m_template;
            }
        }

        /// <summary>
        /// Gets or sets the quality.
        /// </summary>
        /// <remarks>When the image is stored into PDF not as a mask,
        /// you may reduce its quality, which saves the disk space.</remarks>
        public long Quality
        {
            get
            {
                return m_quality;
            }
            set
            {
                m_quality = value;
            }
        }
        /// <summary>
        /// Gets or sets the resolution of the image
        /// </summary>
        public int ImageResolution
        {
            get
            {
                return m_imageResolution;
            }
            set
            {
                m_imageResolution = value;
            }
        }
        /// <summary>
        /// Contains the html hyperlink collection.
        /// <remarks>Used during html to pdf conversion to preserve live-links.</remarks>
        /// </summary>
        internal ArrayList HtmlHyperlinksCollection
        {
            get
            {
                return m_htmlHyperlinks;
            }
            set
            {
                m_htmlHyperlinks = value;
            }
        }

        /// <summary>
        /// Contains document links.
        /// </summary>
        internal ArrayList DocumentLinksCollection
        {
            get
            {
                return m_documentLinks;
            }
            set
            {
                m_documentLinks = value;
            }
        }

        /// <summary>
        /// Gets or sets the page scale.
        /// </summary>
        /// <value>The page scale.</value>
        public float PageScale
        {
            get
            {
                return m_pageScale;
            }
            set
            {
                m_pageScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the page unit.
        /// </summary>
        /// <value>The page unit.</value>
        public GraphicsUnit PageUnit
        {
            get
            {
                return m_pageUnit;
            }
            set
            {
                m_pageUnit = value;
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMetafile"/> class.
        /// </summary>
        /// <param name="metafile">The metafile.</param>
        public PdfMetafile(Metafile metafile)
        {
            if (metafile == null)
                throw new ArgumentNullException("metafile");

            m_image = AdjustMetafile(metafile);
            m_originalImage = metafile;

            //Lets assume metafile was created in 96 DPI environment.
            // The problem is that metafile doesn't contain the information about DPI of the base DC.
            //if( m_image != m_originalImage )
            {
                SetResolution(PdfUnitConvertor.HorizontalResolution, PdfUnitConvertor.VerticalResolution);
            }

            m_template = new PdfTemplate(m_image.Width, m_image.Height);
            SetContent(((IPdfWrapper)m_template).Element);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMetafile"/> class.
        /// </summary>
        /// <param name="path">The metafile path.</param>
        public PdfMetafile(string path)
            : this(new Metafile(Utils.CheckFilePath(path)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMetafile"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfMetafile(Stream stream)
            : this(Metafile.FromStream(CheckStreamExistance(stream)) as Metafile)
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Syncfusion.Pdf.Graphics.PdfMetafile"/> is reclaimed by garbage collection.
        /// </summary>
        ~PdfMetafile()
        {
            Dispose(false);
        }
        #endregion

#region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if it is disposing, set to <c>true</c>.</param>
        private void Dispose(bool disposing)
        {
            if (!m_bDisposed)
            {
                if (disposing) // Dispose all images.
                {
                    if (m_originalImage != null)
                    {
                        m_originalImage.Dispose();
                        m_originalImage = null;
                    }

                    if (m_image != null)
                    {
                        m_image.Dispose();
                        m_image = null;
                    }
                }
                // Dispose converted image only.
                else if (m_originalImage != null && m_image != null && m_image != m_originalImage)
                {
                    m_image.Dispose();
                    m_image = null;
                }

                m_bDisposed = true;
            }
        }
        #endregion

#region Implementation
        /// <summary>
        /// Saves the image into stream.
        /// </summary>
        /// <remarks>This methods prepares a PDF template (XObject)
        /// for saving and drawing.</remarks>
        internal override void Save()
        {
            PdfEmfRenderer renderer=null;
            if (!m_bSaved)
            {

                if (ImageResolution>0)
                {
                    renderer = new PdfEmfRenderer(m_template.Graphics, m_imageResolution, EmbedFontResource);
                }
                else
                {
                    renderer = new PdfEmfRenderer(m_template.Graphics, m_quality, EmbedFontResource);
                }
                using (Metafile metafile = m_image.Clone() as Metafile)
                using (MetaRecordParser parser = new MetaRecordParser(renderer, metafile))
                {
                    if (IsTranparency)
                    {
                        renderer.AlphaBrush = AlphaBrush;
                        renderer.AlphaPen = AlphaPen;
                        renderer.BlendMode = BlendMode;
                        renderer.IsTranparency = IsTranparency;
                    }

                    //This is required incase we run in to resolution problem,  it happens with EmfPlus files.
                    parser.Parser.PageScale = m_pageScale;
                    parser.Parser.PageUnit = m_pageUnit;

                    // Enumerate metafile.
                    parser.Enumerate();
                    m_textRegions = parser.Context as TextRegionManager;
                    m_imageRegions = parser.ImageContext as ImageRegionManager;
                }

                m_bSaved = true;
            }
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Returns lay outing results.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            m_template.Graphics.ColorSpace = param.Page.Document.ColorSpace;
            if (param == null)
                throw new ArgumentNullException("param");

            // Parse metafile.
            if (!param.Page.Document.FileStructure.TaggedPdf)
                Save();

            MetafileLayouter layouter = new MetafileLayouter(this);
            layouter.IsImagePath = IsImagePath;
            PdfLayoutResult result = layouter.Layout(param);

            return result;
        }

        /// <summary>
        /// Layouts the HtmlToPdf element.
        /// </summary>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Returns lay outing results.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected override PdfLayoutResult Layout(HtmlToPdf.HtmlToPdfLayoutParams param)
        {
            if (param == null)
                throw new ArgumentNullException("param");

            // Parse metafile.
            if (!param.Page.Document.FileStructure.TaggedPdf)
                Save();

            MetafileLayouter layouter = new MetafileLayouter(this);
            PdfLayoutResult result = layouter.Layout(param);

            return result;
        }

        /// <summary>
        /// Checks the format of the metafile. Converts it to supported format.
        /// </summary>
        /// <param name="metafile">Input metafile.</param>
        /// <returns>Resulted metafile.</returns>
        internal static Metafile AdjustMetafile(Metafile metafile)
        {
            if (metafile == null)
                throw new ArgumentNullException("metafile");

            Metafile result = metafile;
            MetafileHeader header = metafile.GetMetafileHeader();

            // NOTE: If metafile is of WMF type - convert it to EMF type.
            if (!header.IsEmfOrEmfPlus())
            {
                result = PdfMetafile.ConvertToEmf(metafile);

                if (result == null)
                    throw new ArgumentException("Can't parse metafile. Format is unknown.");
            }

            return result;
        }

        /// <summary>
        /// Converts WMF/EmfPlusDual metafile to EMF metafile.
        /// </summary>
        /// <param name="wmfImage">WMF/EmfPlusDual metafile.</param>
        /// <returns>EMF metafile converted from WMF/EmfPlusDual metafile.</returns>
        private static Metafile ConvertToEmf(Metafile image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            MetafileHeader header = image.GetMetafileHeader();
            Metafile result = null;

            if (!header.IsEmfOrEmfPlus())
            {
                // Clone metafile because it'll be damaged after converting.
                image = (Metafile)image.Clone();
                SizeF dimension = image.PhysicalDimension;

                // Gets size of the data.
                IntPtr wmfHdc = image.GetHenhmetafile();
                int size = GdiApi.GetMetaFileBitsEx(wmfHdc, 0, null);

                if (size > 0)
                {
                    // Get data of the WMF.
                    byte[] wmfData = new byte[size];
                    int copied = GdiApi.GetMetaFileBitsEx(wmfHdc, size, wmfData);

                    // Copied successfully.
                    if (copied > 0)
                    {
                        IntPtr emfDc = IntPtr.Zero;
                        IntPtr hDC = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);

                        // Get resolution that will be displayed in metafile.
                        float mDpiX = PdfUnitConvertor.PxHorizontalResolution / PdfUnitConvertor.HorizontalSize * 25.4f;
                        float mDpiY = PdfUnitConvertor.PxVerticalResolution / PdfUnitConvertor.VerticalSize * 25.4f;

                        float dx = PdfUnitConvertor.HorizontalResolution / mDpiX;
                        float dy = PdfUnitConvertor.VerticalResolution / mDpiY;

                        METAFILEPICT str = new METAFILEPICT();
                        str.xExt = (int)(dimension.Width * dx);
                        str.yExt = (int)(dimension.Height * dy);
                        str.mm = (int)MAPPING_MODE.MM_ANISOTROPIC;

                        emfDc = GdiApi.SetWinMetaFileBits(size, wmfData, hDC, ref str);

                        // Converted successfully.
                        if (emfDc != IntPtr.Zero)
                        {
                            result = new Metafile(emfDc, true);
                        }

                        GdiApi.DeleteDC(hDC);
                    }
                }

                // dispose old metafile.
                GdiApi.DeleteEnhMetaFile(wmfHdc);
                image.Dispose();
            }
            else if (header.Type == MetafileType.EmfPlusDual)
            {
                Rectangle frameRect = new Rectangle(0, 0, image.Width, image.Height);
                Bitmap bitmap = new Bitmap(1, 1);

                System.Drawing.Graphics grphics = System.Drawing.Graphics.FromImage(bitmap);
                IntPtr ipHDC = grphics.GetHdc();

                MemoryStream stream = new MemoryStream();
                result = new Metafile(stream, ipHDC, frameRect, MetafileFrameUnit.Pixel, EmfType.EmfOnly);
                grphics.Dispose();

                grphics = System.Drawing.Graphics.FromImage(result);
                Rectangle src = frameRect;

                grphics.DrawImage(image, src, src, GraphicsUnit.Pixel);

                grphics.Dispose();
                stream.Dispose();
            }

            return result;
        }
        /// <summary>
        /// Sets the transparency.
        /// </summary>
        /// <param name="alphaPen">The alpha value for pen operations.</param>
        /// <param name="alphaBrush">The alpha value for brush operations.</param>
        /// <param name="blendMode">The blend mode.</param>
        /// <param name="transparency">Transparency is applied or not.</param>
        public void SetTransparency(float alphaPen, float alphaBrush, PdfBlendMode blendMode, bool transparency)
        {
            m_alphaBrush = alphaBrush;
            m_alphaPen = alphaPen;
            m_blendMode = blendMode;
            m_bIsTransparency = transparency;
        }
        #endregion
    }
}
#endif