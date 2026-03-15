#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
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
        private EmfRenderer m_renderer;

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
        public EmfRenderer Renderer
        {
            get
            {
                return (Parser.Renderer != null ? Parser.Renderer : m_renderer);
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
        /// Gets parser object.
        /// </summary>
        private MetafileParser Parser
        {
            get
            {
                return m_parser;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="MetaRecordParser"/> class.
        /// </summary>
        static MetaRecordParser()
        {
            IntSize = Marshal.SizeOf(typeof(Int32));
            ShortSize = Marshal.SizeOf(typeof(Int16));
            FloatSize = Marshal.SizeOf(typeof(Single));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaRecordParser"/> class.
        /// </summary>
        private MetaRecordParser()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaRecordParser"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="metaFile">Metafile for parsing.</param>
        public MetaRecordParser(EmfRenderer renderer, Metafile metaFile)
            : this()
        {
            if (metaFile == null)
                throw new ArgumentNullException("metaFile");

            AssignMetaFile(metaFile);
            Renderer = renderer;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
        /// <returns>
        /// True - successful enumeration, False otherwise.
        /// </returns>
        public bool Enumerate()
        {
            EventArgs e = new EventArgs();
            Parser.OnBeginParse(e);

            bool result = true;
            RectangleF rect = new RectangleF(0, 0, MetaFile.Width, MetaFile.Height);

            try
            {
                using (Bitmap bmp = new Bitmap(1, 1))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.EnumerateMetafile(MetaFile, rect.Location, Parser.ParsingHandler);
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

        /// <summary>
        /// Check if the parser support current metafile format.
        /// </summary>
        /// <param name="metaFile">The metafile image.</param>
        /// <returns>The <see cref="System.Boolean"/>.</returns>
        public static bool SupportFormat(Metafile metaFile)
        {
            MetafileHeader header = metaFile.GetMetafileHeader();
            header = metaFile.GetMetafileHeader();

            return SupportFormat(header.Type);
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

                // NOTE: If metafile is of WMF type - convert it to EMF type.
                if (!header.IsEmfOrEmfPlus())
                {
                    m_bImgWMF = true;
                    metaFile = ConvertToEmf(metaFile);

                    if (metaFile == null)
                        throw new ArgumentException("Can't parse metafile. Format is unknown.");
                }

                m_metaFile = metaFile;
                SizeF dpi = new SizeF(m_metaFile.HorizontalResolution, m_metaFile.VerticalResolution);
                header = m_metaFile.GetMetafileHeader();

                RecognizeParser(header, dpi);
            }
        }

        /// <summary>
        /// Check if parser support the current metafile format.
        /// </summary>
        /// <param name="metaType">Type of the metafile image.</param>
        /// <returns><b>True</b> if parser support metafile type.</returns>
        private static bool SupportFormat(MetafileType metaType)
        {
            return (metaType == MetafileType.Emf || metaType == MetafileType.Wmf || metaType == MetafileType.WmfPlaceable);
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

            if (SupportFormat(header.Type))
                m_parser = new EmfParser(header.Type, dpi);

            if (m_parser == null)
                throw new ArgumentNullException("Parser. Can't parse metafile. Format is unknown.");

            if (Renderer != null)
            {
                Parser.Renderer = Renderer;
            }

            Parser.Metafile = MetaFile;
            Parser.BeginParse += new MetafileParser.ParseEventHandler(MetafileBeginParse);
        }

        /// <summary>
        /// Converts WMF metafile to EMF metafile.
        /// </summary>
        /// <param name="wmfImage">WMF metafile.</param>
        /// <returns>EMF metafile converted from WMF metafile.</returns>
        private static Metafile ConvertToEmf(Metafile wmfImage)
        {
            if (wmfImage == null)
                throw new ArgumentNullException("wmfImage");

            MetafileHeader header = wmfImage.GetMetafileHeader();
            Metafile result = null;

            if (!header.IsEmfOrEmfPlus())
            {
                // Clone metafile because it'll be damaged after converting.
                wmfImage = (Metafile)wmfImage.Clone();
                SizeF dimension = wmfImage.PhysicalDimension;

                // Gets size of the data.
                IntPtr wmfHdc = wmfImage.GetHenhmetafile();
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

                        METAFILEPICT str = new METAFILEPICT();
                        str.xExt = (int)dimension.Width;
                        str.yExt = (int)dimension.Height;
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
                wmfImage.Dispose();
            }

            return result;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        ///  Raises when metafile is going to be parsed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The args.</param>
        private void MetafileBeginParse(object sender, EventArgs e)
        {
            Parser.InitializeSettings();
        }
        #endregion
    }
}
