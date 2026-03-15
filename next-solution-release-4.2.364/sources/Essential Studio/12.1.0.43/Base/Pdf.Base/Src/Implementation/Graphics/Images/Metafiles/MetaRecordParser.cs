#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    internal class MetaRecordParser : IDisposable
    {
#region Constants
        /// <summary>
        /// Size of Int32 type.
        /// </summary>
        private static readonly int IntSize;
        /// <summary>
        /// Size of Short type.
        /// </summary>
        private static readonly int ShortSize;
        /// <summary>
        /// Size of Single type.
        /// </summary>
        private static readonly int FloatSize;
        /// <summary>
        /// Number of numbers in the point type.
        /// </summary>
        private const byte PointNumber = 2;
        /// <summary>
        /// Number of numbers in the rectangle type.
        /// </summary>
        private const byte RectNumber = 4;
        /// <summary>
        /// Flag for recognizing type of region.
        /// </summary>
        private const int RegionFlag = 0x10000000;
        /// <summary>
        /// Flag for objects recognizing.
        /// </summary>
        private const int ObjectFlag = 0xff00;
        /// <summary>
        /// Type of path filling.
        /// </summary>
        private const int PathFillWinding = 0x6000;
        /// <summary>
        /// Index where type of the brush is located.
        /// </summary>
        private const int BrushTypeIndex = 4;

        #endregion

#region Fields
        /// <summary>
        /// Parsing metafile object.
        /// </summary>
        private Metafile m_metaFile;
        /// <summary>
        /// Parser of metafile. It depends on the metafile.
        /// </summary>
        private MetafileParser m_parser;
        /// <summary>
        /// Gets graphics context object.
        /// </summary>
        private PdfEmfRenderer m_renderer;
        /// <summary>
        /// Checks if object is already disposed or not.
        /// </summary>
        private bool m_bDisposed;
        /// <summary>
        /// Indicates whether image is WMF file and needs to be disposed.
        /// </summary>
        private bool m_bImgWMF;

        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the renderer.
        /// </summary>
        public PdfEmfRenderer Renderer
        {
            get
            {
                return ((Parser.Renderer != null) ? Parser.Renderer : m_renderer);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Renderer");

                if (Parser != null)
                {
                    Parser.Renderer = value;
                }
                else
                {
                    m_renderer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets parsing metafile object.
        /// </summary>
        public Metafile MetaFile
        {
            get
            {
                return m_metaFile;
            }
            set
            {
                AssignMetaFile(value);
            }
        }

        /// <summary>
        /// Gets context data of the parser.
        /// </summary>
        public object Context
        {
            get
            {
                return Parser.Context;
            }
        }

        /// <summary>
        /// Gets context data of the parser.
        /// </summary>
        public object ImageContext
        {
            get
            {
                return Parser.ImageContext;
            }
        }

        /// <summary>
        /// Gets parser object.
        /// </summary>
        internal MetafileParser Parser
        {
            get
            {
                return m_parser;
            }
        }

        #endregion

#region Constructors
        /// <summary>
        /// Static constructor.
        /// </summary>
        static MetaRecordParser()
        {
            IntSize = Marshal.SizeOf(typeof(Int32));
            ShortSize = Marshal.SizeOf(typeof(Int16));
            FloatSize = Marshal.SizeOf(typeof(Single));
        }

        /// <summary>
        /// Creates new object.
        /// </summary>
        private MetaRecordParser()
        {
        }

        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="metaFile">Metafile for parsing.</param>
        public MetaRecordParser(PdfEmfRenderer renderer, Metafile metaFile)
            : this()
        {
            if (metaFile == null)
                throw new ArgumentNullException("metaFile");

            AssignMetaFile(metaFile);

            Renderer = renderer;

        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_parser != null)
                {
                    m_parser.Dispose();
                    m_parser = null;
                }

                if (m_bImgWMF && m_metaFile != null)
                {
                    m_metaFile.Dispose();
                }

                m_metaFile = null;
                m_bDisposed = true;
            }
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Enumerates a metafile.
        /// </summary>
        /// <returns>True - successful enumeration, False otherwise. </returns>
        public bool Enumerate()
        {
            bool result = true;

            RectangleF rect = new RectangleF(0, 0, MetaFile.Width,
                MetaFile.Height);

            try
            {
                using (Bitmap bmp = new Bitmap(1, 1))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.EnumerateMetafile(MetaFile, rect.Location,
                        Parser.ParsingHandler);
                }
            }
            catch (Exception ex)
            {
                result = false;
                System.Diagnostics.Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                Renderer.OnError(ex);
            }

            return result;
        }
        #endregion

#region Implementation
        /// <summary>
        /// Assigns the metafile.
        /// </summary>
        /// <param name="metaFile">The metafile.</param>
        private void AssignMetaFile(Metafile metaFile)
        {
            if (metaFile == null)
                throw new ArgumentNullException("metaFile");

            if (m_metaFile != metaFile)
            {
                MetafileHeader header = metaFile.GetMetafileHeader();
                if (!header.IsEmfOrEmfPlus())
                {
                    m_bImgWMF = true;
                }

                m_metaFile = PdfMetafile.AdjustMetafile(metaFile);
                SizeF dpi = new SizeF(m_metaFile.HorizontalResolution, m_metaFile.VerticalResolution);
                header = m_metaFile.GetMetafileHeader();

                RecognizeParser(header, dpi);
            }
        }

        /// <summary>
        /// Recognizes which parser must be created according to metafile.
        /// </summary>
        /// <param name="header">Header of metafile.</param>
        /// <param name="dpi">The dpi.</param>
        private void RecognizeParser(MetafileHeader header, SizeF dpi)
        {
            if (header == null)
                throw new ArgumentNullException("header");


            switch (header.Type)
            {
                case MetafileType.EmfPlusOnly:
                    m_parser = new EmfPlusParser(MetafileType.EmfPlusOnly, dpi);
                    break;

                case MetafileType.EmfPlusDual:
                    m_parser = new EmfPlusParser(MetafileType.EmfPlusDual, dpi);
                    break;

                case MetafileType.Emf:
                    m_parser = new EmfParser(MetafileType.Emf, dpi);
                    break;

                case MetafileType.Wmf:
                    m_parser = new EmfParser(MetafileType.Wmf, dpi);
                    break;
            }

            if (Renderer != null)
            {
                Parser.Renderer = Renderer;
            }

            Parser.Metafile = MetaFile;

        }

        #endregion

    }
}
#endif