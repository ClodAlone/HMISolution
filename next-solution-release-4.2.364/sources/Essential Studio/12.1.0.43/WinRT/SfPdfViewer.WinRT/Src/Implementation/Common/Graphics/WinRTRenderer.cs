#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.PdfViewer.Base;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Globalization;
#if SyncfusionFramework4_5
using Syncfusion.DirectXWrapper.WinRT;
namespace Syncfusion.Windows.PdfViewer
{
    class WinRTRenderer : Control
    {
        int textRenderingMode = 0;
        string currentTextRenderingOperator = string.Empty;
        Syncfusion.DirectXWrapper.WinRT.Matrix activeTextMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, 0);
        int pushedMatrixCount = 0;
        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
        private bool IsPathSinkClosed = false;
        Syncfusion.DirectXWrapper.WinRT.Matrix matrix;
        Syncfusion.DirectXWrapper.WinRT.Matrix mat;
        float visibleTopBounds = 0;
        bool IsCommonMatrixUpdated = false;
        bool IsTextMatrixUpdate = false;
        float[] scalingFactor = new float[2];
        internal PdfDocumentPage m_documentPage;
        internal static Graphics2D graphics = null;
        internal static Graphics2D graphics1 = null;
        internal static Graphics2D graphics2 = null;
        internal static Graphics2D graphics3 = null;
        internal static Graphics2D graphics4 = null;
        internal PointF currentTransformLocation = new PointF();
        char[] m_symbolChars = new char[] { '(', ')', '[', ']', '<', '>' };
        char[] m_startText = new char[] { '(', '[', '<', };
        char[] m_endText = new char[] { ')', ']', '>' };
        PdfPageResources m_resources;
        PdfRecordCollection m_mainContentElements;
        private PointF m_currentLocation = PointF.Empty;
        private bool m_beginText;
        private float m_wordSpacing;
        //private Graphics m_graphics;
        private Rect m_clipRectangle;
        private List<Rect> m_clipRectangleList = new List<Rect>();
        GeometrySink m_path;
        //PathRenderer ClipPath = new PathRenderer();
        private float m_mitterLength;
        private Stack<Syncfusion.DirectXWrapper.WinRT.Matrix> m_matrix = new Stack<Syncfusion.DirectXWrapper.WinRT.Matrix>();
        private Stack<DrawingStateBlock> m_graphicsState = new Stack<DrawingStateBlock>();
        private Stack<GraphicObjectData> m_objects = new Stack<GraphicObjectData>();

        private List<PathGeometry> additionalPaths = new List<PathGeometry>();
        private Stack<PdfPageResources> m_parentResources = new Stack<PdfPageResources>();
        private float m_textScaling = 100;
        bool textMatrix = false;

        //private List<GeometrySink> m_subPaths = new List<GeometrySink>();
        //private List<GeometrySink> m_tempSubPaths = new List<GeometrySink>();
        private float m_textElementWidth;
        private PointF m_endTextPosition;
        private bool m_isCurrentPositionChanged;
        private float m_characterSpacing;
        private global::Windows.UI.Color transperentStrokingColor;
        private global::Windows.UI.Color transperentNonStrokingColor;
        private Stack<PathRenderer> clipPathStack = new Stack<PathRenderer>();
        private string[] m_dashedLine;
        private bool isNegativeFont = false;
        bool IsNegativePath = false;
        bool IsTextRotated = false;
        string m_filePath = string.Empty;
        /// <summary>
        /// AutoResetEvent allows threads to communicate with each other by signaling
        /// </summary>
        private static System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(true);

        /// <summary>
        /// A delegate type for hooking up the renderer
        /// </summary>
        public event Action OnRender;

        /// <summary>
        /// Collection of rendered image stream
        /// </summary>
        internal static Dictionary<string, IRandomAccessStream> RenderedImages = new Dictionary<string, IRandomAccessStream>();

        /// <summary>
        /// Internal cancellation token source variable.
        /// </summary>
        private static CancellationTokenSource tokenSource2 = new CancellationTokenSource();

        /// <summary>
        /// Internal variable that stores token.
        /// </summary>
        private static CancellationToken token;

        /// <summary>
        /// Internal variable to hold the cancellation status.
        /// </summary>
        private static bool m_cancel;
        /// <summary>
        /// Internal variable to hold the initialized pdf document page.
        /// </summary>
        internal Dictionary<int, PdfDocumentPage> m_initializedDocumentPages = new Dictionary<int, PdfDocumentPage>();
        /// <summary>
        /// Internal variable to hold cff glyphs.
        /// </summary>
        private List<CffGlyphs> m_glyphDataCollection = new List<CffGlyphs>();

        private int ClipPushCount
        {
            get
            {
                return data.ClipCounts;
            }
            set
            {
                data.ClipCounts = value;
            }
        }

        float m_changeInHeight, m_pixelHeight;
        GraphicObjectData data = new GraphicObjectData();
        private PointF CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
                m_isCurrentPositionChanged = true;
            }
        }

        private string CurrentFont
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.CurrentFont;
                }
                else
                {
                    string tempFontName = "";
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontName = objectData.CurrentFont;
                        }
                    }
                    return tempFontName;
                }
            }
            set
            {
                Objects.CurrentFont = value;
            }
        }

        private float FontSize
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.FontSize;
                }
                else
                {
                    float tempFontSize = 0;
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontSize = objectData.FontSize;
                        }
                    }
                    return tempFontSize;
                }
            }
            set
            {
                Objects.FontSize = value;
            }
        }

        private GraphicObjectData Objects
        {
            get
            {
                return m_objects.Peek();
            }
        }

        private global::Windows.UI.Color NonStrokingColorSpace
        {
            get
            {
                if (Objects.NonStrokingColorspace != new global::Windows.UI.Color())
                {
                    return Objects.NonStrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.NonStrokingColorspace != new global::Windows.UI.Color())
                        {
                            return objectData.NonStrokingColorspace;
                        }
                    }
                    return new global::Windows.UI.Color();
                }
            }
        }

        private global::Windows.UI.Color StrokingColorSpace
        {
            get
            {
                if (Objects.StrokingColorspace != new global::Windows.UI.Color())
                {
                    return Objects.StrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.StrokingColorspace != new global::Windows.UI.Color())
                        {
                            return objectData.StrokingColorspace;
                        }
                    }
                    return new global::Windows.UI.Color();
                }
            }
        }

        private Rect ClipRectangle
        {
            get
            {
                return m_clipRectangle;
            }
            set
            {
                m_clipRectangle = value;
            }
        }

        private int LayerPushCount
        {
            get
            {
                return data.LayerCounts;
            }
            set
            {
                data.LayerCounts = value;
            }
        }

        private float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }
            set
            {
                m_wordSpacing = value;
            }
        }

        private float MitterLength
        {
            get
            {
                return m_mitterLength;
            }
            set
            {
                m_mitterLength = value;
            }

        }

        private float TextLeading
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.TextLeading;
                }
                else
                {
                    float tempTextLeading = 0;
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempTextLeading = objectData.TextLeading;
                        }
                    }
                    return tempTextLeading;
                }
            }
            set
            {
                Objects.TextLeading = value;
            }
        }

        private float TextScaling
        {
            get
            {
                return m_textScaling;
            }
            set
            {
                m_textScaling = value;
            }
        }

        internal string ImageFilePath
        {
            get
            {
                return m_filePath;
            }
        }

        private PathGeometry Path;

        //Dictionary<PathGeometry, GeometrySink> pathDict = new Dictionary<PathGeometry, GeometrySink>();
        List<Syncfusion.DirectXWrapper.WinRT.Matrix> pathMatrix = new List<Syncfusion.DirectXWrapper.WinRT.Matrix>();
        List<PathRenderer> pathRenderList = new List<PathRenderer>();
        private GeometrySink PathSink
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }

        private float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }
            set
            {
                m_characterSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets cancellation status.
        /// </summary>
        internal static bool Cancel
        {
            get
            {
                return m_cancel;
            }
            set
            {
                m_cancel = value;
                if (m_cancel)
                    StopRendering();
                else
                    ResetToken();
            }
        }

        double zoomFactor;
        bool IsZoomChanged = false;
        /// <summary>
        /// Invoke the render event
        /// </summary>
        public virtual void RenderAll()
        {
            if (OnRender != null)
                OnRender();
        }
        internal PdfLoadedDocument m_lDoc = null;
        internal PdfDocumentView m_documentView = null;
        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="pointer">The reference of the object.</param>
        public WinRTRenderer(double zoom, bool isZoomChanged, PdfDocumentView documentView)
        {
            // this.m_lDoc = documentPage;
            //this.m_mainContentElements = documentPage.m_recordCollection;
            //this.m_resources = documentPage.m_resources;
            m_documentView = documentView;
            this.IsZoomChanged = isZoomChanged;
            this.zoomFactor = zoom;
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            m_cancel = false;
            token = tokenSource2.Token;
        }

        ~WinRTRenderer()
        {
            //graphics = null;
            m_documentPage = null;
            m_objects.Clear();
            this.m_resources = null;
            this.m_mainContentElements = null;
            //m_objects = null;
            m_clipRectangleList.Clear();
            //m_convertor = null;
            m_dashedLine = null;
            //m_endText = null;
            m_graphicsState.Clear();
            m_matrix.Clear();
            m_parentResources.Clear();
            if (m_path != null)
            {
                m_path.Close();
                m_path = null;
            }
            //m_startText = null;
            //m_symbolChars = null;


        }
#if DEBUG
        internal void Dispose()
        {
            graphics.Dispose();
            graphics = null;
        }
        public Stream ExportAsImage(int i)
        {
            PdfDocumentPage pdfDocPage = null;
            if (!m_initializedDocumentPages.ContainsKey(i))
            {
                PdfPageBase pageToBeRendered = m_lDoc.Pages[i];
                pdfDocPage = new PdfDocumentPage(pageToBeRendered);
                pdfDocPage.Initialize(pageToBeRendered, true);
                m_initializedDocumentPages.Add(i, pdfDocPage);

            }
            else
                pdfDocPage = m_initializedDocumentPages[i];
            double imageWidth = 0;
            double imageHeight = 0;
            imageWidth = m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point) * 1;
            imageHeight = m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point) * 1;
            m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
            if (graphics == null)
                graphics = new Graphics2D((int)imageWidth, (int)imageHeight);
            graphics.Initialize(DisplayProperties.LogicalDpi * (float)1);
            graphics.InitializeImage();
            graphics.BeginDraw();

            int _offsetx = 0;
            int _offsety = 0;
            GraphicBrush _brush = null;

            int _left = _offsetx;
            int _top = _offsety;
            int _right = (int)imageWidth;
            int _bottom = (int)imageHeight;

            _brush = new GraphicBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));

            graphics.FillRectangle(_brush, new Rect(_left, _top, _right, _bottom));
            this.m_mainContentElements = pdfDocPage.m_recordCollection;
            this.m_resources = pdfDocPage.m_resources;
            if (m_resources.Resources.ContainsKey("Rotate"))
            {
                string rotateAngleString = (m_resources.Resources["Rotate"]).ToString();
                float rotateAngle = float.Parse(rotateAngleString, CultureInfo.InvariantCulture);
                //Need to update the rotation angle to m_renderTarget
            }
            m_pixelHeight = (float)m_convertor.ConvertFromPixels((float)graphics.Height, PdfGraphicsUnit.Point) - (float)m_convertor.ConvertFromPixels(m_changeInHeight, PdfGraphicsUnit.Point);
            matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, m_pixelHeight);
            graphics.Transform = matrix;

            RenderContent(m_mainContentElements, graphics);

            _brush.Dispose();

            graphics.EndDraw();
            Stream stream = new MemoryStream();
            using (IRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                graphics.SaveAsStream(ms);

                int lenght = (int)ms.Size;

                byte[] imgArrary = new byte[lenght];
                ms.Seek(0);
                ms.AsStream().Read(imgArrary, 0, lenght);
                stream.Write(imgArrary, 0, imgArrary.Length);
            }
            stream.Position = 0;
            return stream;
        }
#endif
        public void RenderAsImage(int i, bool print)
        {
            if (!print)
                InitializeTarget(i, print);
            else
            {
                graphics.InitializeImage();
                InitializeTarget(i, print);
                using (IRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    graphics.SaveAsStream(ms);

                    int lenght = (int)ms.Size;

                    byte[] imgArrary = new byte[lenght];
                    ms.Seek(0);
                    ms.AsStream().Read(imgArrary, 0, lenght);

                    StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

                    IAsyncOperation<StorageFile> stFile = roamingFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".png");
                    stFile.AsTask().Wait();
                    StorageFile _stFile = stFile.GetResults();

                    Stream stream = new MemoryStream();
                    stream.Write(imgArrary, 0, imgArrary.Length);
                    if (stFile != null)
                    {
                        IAsyncOperation<IRandomAccessStream> fileStream1Task = _stFile.OpenAsync(FileAccessMode.ReadWrite);
                        fileStream1Task.AsTask().Wait();
                        IRandomAccessStream fileStream1 = fileStream1Task.GetResults();
                        Stream st = fileStream1.AsStreamForWrite();

                        st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                        st.Flush();
                        st.Dispose();
                        fileStream1.Dispose();
                    }

                    m_filePath = stFile.GetResults().Path;
                    stFile.Close();
                }
            }
        }
        private Canvas m_docPage;
        internal Canvas docPage
        {
            get
            {
                return m_docPage;
            }
            set
            {
                m_docPage = value;
            }
        }
        int m_Pagerender;
        internal void InitializeTarget(int pageToBeRendered, bool print)
        {
            if (print)
                Render(pageToBeRendered);
            else
            {
                this.m_Pagerender = pageToBeRendered;
                this.OnRender += WinRTRenderer_OnRender;
                this.RenderAll();
            }
        }

        /// <summary>
        /// Cancels all pending async operations.
        /// </summary>
        private static void StopRendering()
        {
            if (tokenSource2 != null)
                tokenSource2.Cancel();
        }

        /// <summary>
        /// Resets the cancellation token.
        /// </summary>
        private static void ResetToken()
        {
            tokenSource2 = new CancellationTokenSource();
            token = tokenSource2.Token;
        }

        void WinRTRenderer_OnRender()
        {
            Func<int, CancellationToken, Task<int>> invokeRenderContent;
            invokeRenderContent = RenderAsync;

            try
            {
                Task.Factory.StartNew(() => invokeRenderContent(this.m_Pagerender, token), token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        internal void Render(int pageIn)
        {
            try
            {
                PdfDocumentPage pdfDocPage = null;
                if (!m_initializedDocumentPages.ContainsKey(pageIn))
                {
                    if (m_lDoc.PageCount < pageIn)
                        return;
                    PdfPageBase pageToBeRendered = m_lDoc.Pages[pageIn];
                    pdfDocPage = new PdfDocumentPage(pageToBeRendered);
                    pdfDocPage.Initialize(pageToBeRendered, true);
                    m_initializedDocumentPages.Add(pageIn, pdfDocPage);

                }
                else
                    pdfDocPage = m_initializedDocumentPages[pageIn];
                this.m_mainContentElements = pdfDocPage.m_recordCollection;
                this.m_resources = pdfDocPage.m_resources;

                double imageWidth = 0;
                double imageHeight = 0;
                imageWidth = m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point) * zoomFactor;
                imageHeight = m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point) * zoomFactor;
                m_changeInHeight = (float)imageHeight - pdfDocPage.Height;

                double aspectRatio;
                bool isZoomSet = false;
                if (imageWidth > 2000 || imageHeight > 2000)
                {
                    isZoomSet = true;
                    if (imageWidth > imageHeight)
                    {
                        aspectRatio = imageWidth / imageHeight;

                        imageWidth = 2000;
                        imageHeight = 2000 / aspectRatio;
                        zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point);
                        zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                        m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                    }
                    else
                    {
                        aspectRatio = imageWidth / imageHeight;

                        imageHeight = 2000;
                        imageWidth = 2000 * aspectRatio;
                        zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point);
                        zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                        m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                    }
                }

                if (graphics == null || IsZoomChanged)
                {
                    graphics = new Graphics2D((int)imageWidth, (int)imageHeight);
                    graphics1 = new Graphics2D((int)imageWidth, (int)imageHeight);
                    graphics2 = new Graphics2D((int)imageWidth, (int)imageHeight);
                    graphics3 = new Graphics2D((int)imageWidth, (int)imageHeight);
                    graphics4 = new Graphics2D((int)imageWidth, (int)imageHeight);
                }

                float graphicsSoruce = pageIn % 4;
                if (graphicsSoruce == 0)
                    graphics = graphics4;
                else if (graphicsSoruce == 1)
                    graphics = graphics3;
                else if (graphicsSoruce == 2)
                    graphics = graphics2;
                else
                    graphics = graphics1;

                graphics.InitializeImage();

                if (isZoomSet)
                    graphics.Initialize(DisplayProperties.LogicalDpi * (float)zoomFactor);
                else
                {
                    zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                    graphics.Initialize(DisplayProperties.LogicalDpi * (float)zoomFactor);
                }
                graphics.BeginDraw();

                int _offsetx = 0;
                int _offsety = 0;
                GraphicBrush _brush = null;

                int _left = _offsetx;
                int _top = _offsety;
                int _right = (int)imageWidth;
                int _bottom = (int)imageHeight;

                _brush = new GraphicBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));

                graphics.FillRectangle(_brush, new Rect(_left, _top, _right, _bottom));

                if (m_resources.Resources.ContainsKey("Rotate"))
                {
                    string rotateAngleString = (m_resources.Resources["Rotate"]).ToString();
                    float rotateAngle = float.Parse(rotateAngleString, CultureInfo.InvariantCulture);
                    //Need to update the rotation angle to m_renderTarget
                }
                m_pixelHeight = (float)m_convertor.ConvertFromPixels((float)graphics.Height, PdfGraphicsUnit.Point) - (float)m_convertor.ConvertFromPixels(m_changeInHeight, PdfGraphicsUnit.Point);
                matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, m_pixelHeight);
                graphics.Transform = matrix;

                RenderContent(m_mainContentElements, graphics);

                _brush.Dispose();

                graphics.EndDraw();
                Canvas PdfDocumentPanel = docPage;
                UIElement element = PdfDocumentPanel.Children[pageIn];
                if (element is Canvas)
                {
                    Canvas page = element as Canvas;
                    Image image = new Image();
                    WriteableBitmap writeBitmap = new WriteableBitmap((int)page.Width, (int)page.Height);
                    IRandomAccessStream ms = new InMemoryRandomAccessStream();
                    graphics.SaveAsStream(ms);
                    ms.Seek(0);

                    writeBitmap.SetSource(ms);
                    ms.Seek(0);
                    if (!RenderedImages.ContainsKey(pageIn.ToString() + "-" + zoomFactor.ToString() + "-" + m_documentView.controlName))
                        RenderedImages.Add(pageIn.ToString() + "-" + zoomFactor.ToString() + "-" + m_documentView.controlName, ms);
                    image.Source = writeBitmap;
                    image.Width = page.Width;
                    image.Height = page.Height;
                    page.Children.Add(image);
                }
            }
            catch (Exception)
            {
            }
        }

        internal async Task<int> RenderAsync(int pageIn, CancellationToken cts)
        {
#if DEBUG
            if (cts.IsCancellationRequested)
                System.Diagnostics.Debug.WriteLine("Task Cancelled: " + pageIn.ToString());
#endif

            try
            {
                autoEvent.WaitOne();
                PdfDocumentPage pdfDocPage = null;
                if (!m_initializedDocumentPages.ContainsKey(pageIn))
                {
                    if (m_lDoc.PageCount < pageIn)
                        return 0;
                    PdfPageBase pageToBeRendered = m_lDoc.Pages[pageIn];
                    pdfDocPage = new PdfDocumentPage(pageToBeRendered);
                    pdfDocPage.Initialize(pageToBeRendered, true);
                    m_initializedDocumentPages.Add(pageIn, pdfDocPage);

                }
                else
                    pdfDocPage = m_initializedDocumentPages[pageIn];
                this.m_mainContentElements = pdfDocPage.m_recordCollection;
                this.m_resources = pdfDocPage.m_resources;

                double imageWidth = 0;
                double imageHeight = 0;
                imageWidth = m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point) * zoomFactor;
                imageHeight = m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point) * zoomFactor;
                m_changeInHeight = (float)imageHeight - pdfDocPage.Height;

                double aspectRatio;
                bool isZoomSet = false;
                if (imageWidth > 2000 || imageHeight > 2000)
                {
                    isZoomSet = true;
                    if (imageWidth > imageHeight)
                    {
                        aspectRatio = imageWidth / imageHeight;

                        imageWidth = 2000;
                        imageHeight = 2000 / aspectRatio;
                        zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point);
                        zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                        m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                    }
                    else
                    {
                        aspectRatio = imageWidth / imageHeight;

                        imageHeight = 2000;
                        imageWidth = 2000 * aspectRatio;
                        zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point);
                        zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                        m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                    }
                }
                await Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    if (graphics == null || IsZoomChanged)
                    {
                        graphics = new Graphics2D((int)imageWidth, (int)imageHeight);
                        graphics1 = new Graphics2D((int)imageWidth, (int)imageHeight);
                        graphics2 = new Graphics2D((int)imageWidth, (int)imageHeight);
                        graphics3 = new Graphics2D((int)imageWidth, (int)imageHeight);
                        graphics4 = new Graphics2D((int)imageWidth, (int)imageHeight);
                    }
                });

                float graphicsSoruce = pageIn % 4;
                if (graphicsSoruce == 0)
                    graphics = graphics4;
                else if (graphicsSoruce == 1)
                    graphics = graphics3;
                else if (graphicsSoruce == 2)
                    graphics = graphics2;
                else
                    graphics = graphics1;

                graphics.InitializeImage();
                if (isZoomSet)
                    graphics.Initialize(DisplayProperties.LogicalDpi * (float)zoomFactor);
                else
                {
                    zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                    graphics.Initialize(DisplayProperties.LogicalDpi * (float)zoomFactor);
                }
                await Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.High, () =>
                 {
                     graphics.BeginDraw();
                 });

                //src.Remove(cts);
                int _offsetx = 0;
                int _offsety = 0;
                GraphicBrush _brush = null;

                int _left = _offsetx;
                int _top = _offsety;
                int _right = (int)imageWidth;
                int _bottom = (int)imageHeight;

                _brush = new GraphicBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));

                graphics.FillRectangle(_brush, new Rect(_left, _top, _right, _bottom));

                if (m_resources.Resources.ContainsKey("Rotate"))
                {
                    string rotateAngleString = (m_resources.Resources["Rotate"]).ToString();
                    float rotateAngle = float.Parse(rotateAngleString, CultureInfo.InvariantCulture);
                    //Need to update the rotation angle to m_renderTarget
                }
                m_pixelHeight = (float)m_convertor.ConvertFromPixels((float)graphics.Height, PdfGraphicsUnit.Point) - (float)m_convertor.ConvertFromPixels(m_changeInHeight, PdfGraphicsUnit.Point);
                matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 0, m_pixelHeight);
                graphics.Transform = matrix;
                RenderContent(m_mainContentElements, graphics);

                _brush.Dispose();
                await Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.High, () =>
                 {
                     graphics.EndDraw();
                     Canvas PdfDocumentPanel = docPage;
                     UIElement element = PdfDocumentPanel.Children[pageIn];
                     if (element is Canvas)
                     {
                         Canvas page = element as Canvas;
                         global::Windows.UI.Xaml.Shapes.Rectangle PageBorder = new global::Windows.UI.Xaml.Shapes.Rectangle();
                         PageBorder.Height = (int)page.Height + 5;
                         PageBorder.Width = (int)page.Width + 5;
                         PageBorder.Stroke = new global::Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 0, 0, 0));
                         double dble = -2.5;

                         Canvas.SetLeft(PageBorder, dble);
                         Canvas.SetTop(PageBorder, dble);
                         Canvas.SetZIndex(PageBorder, 0);

                         page.Children.Add(PageBorder);
                         Image image = new Image();
                         WriteableBitmap writeBitmap = new WriteableBitmap((int)page.Width, (int)page.Height);
                         IRandomAccessStream ms = new InMemoryRandomAccessStream();
                         graphics.SaveAsStream(ms);
                         ms.Seek(0);

                         writeBitmap.SetSource(ms);
                         ms.Seek(0);
                         if (!RenderedImages.ContainsKey(pageIn.ToString() + "-" + zoomFactor.ToString() + "-" + m_documentView.controlName))
                             RenderedImages.Add(pageIn.ToString() + "-" + zoomFactor.ToString() + "-" + m_documentView.controlName, ms);
                         image.Source = writeBitmap;
                         image.Width = page.Width;
                         image.Height = page.Height;
                         page.Children.Add(image);
                     }
                 });
                foreach (CffGlyphs gl in m_glyphDataCollection)
                {
                    gl.RenderedPath.Clear();
                }
                m_glyphDataCollection.Clear();
            }
            catch (Exception)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(pageIn.ToString());
#endif
            }
            finally
            {
                autoEvent.Set();
            }

#if DEBUG
            if (cts.IsCancellationRequested)
                System.Diagnostics.Debug.WriteLine("Task Cancelled: " + pageIn.ToString());
#endif
            return 0;
        }

        private void RenderContent(PdfRecordCollection recordCollection, Graphics2D graphics)
        {
            PdfRecordCollection m_contentElements = recordCollection;
            try
            {
                if (m_contentElements != null)
                {
                    foreach (PdfRecord record in m_contentElements)
                    {
                        int token = record.OperatorName;
                        string[] element = record.Operands;

                        //foreach (char ch in m_symbolChars)
                        //{
                        //    if (token.Contains(ch.ToString()))
                        //        token = token.Replace(ch.ToString(), "");
                        //}
                        switch (token)
                        {
                            case 40://q
                                {
                                    PathRenderer clipPathTemp = new PathRenderer();
                                    if (data.LayerCounts > 0)
                                    {
                                        clipPathTemp = data.ClipPath;
                                    }
                                    mat = graphics.Transform;
                                    m_matrix.Push(mat);
                                    visibleTopBounds = graphics.Transform.OffsetY;
                                    DrawingStateBlock drawingState = graphics.SaveDrawingState();
                                    m_graphicsState.Push(drawingState);
                                    data = new GraphicObjectData();
                                    m_objects.Push(data);
                                    if (clipPathTemp.PathGeomentry != null)
                                    {
                                        data.ClipPath = clipPathTemp;
                                    }
                                    break;
                                }
                            case 41://"Q"
                                {
                                    data = m_objects.Pop();
                                    for (int i = 0; i < ClipPushCount; i++)
                                    {
                                        try
                                        {
                                            graphics.PopAxisAlignedClip();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    ClipPushCount = 0;

                                    bool layer = false;
                                    for (int j = 0; j < data.LayerCounts; j++)
                                    {
                                        graphics.PopLayer();
                                        layer = true;
                                    }

                                    if (layer)
                                    {
                                        data.LayerCounts = 0;
                                        data.ClipPath = new PathRenderer();
                                    }
                                    //ClipPath.PathGeomentry = null;
                                    graphics.RestoreDrawingState(m_graphicsState.Pop());
                                    mat = m_matrix.Pop();
                                    graphics.Transform = mat;
                                    matrix = mat;
                                    IsTextMatrixUpdate = false;
                                    textMatrix = false;
                                    //ClipPath = clipPathStack.Pop();
                                    m_characterSpacing = 0;
                                    textRenderingMode = 0;
                                    break;
                                }
                            case 63://Tr
                                {
                                    textRenderingMode = int.Parse(element[0], CultureInfo.InvariantCulture);
                                    if (float.Parse(element[0], CultureInfo.InvariantCulture) == 3)
                                    {
                                        transperentNonStrokingColor = Objects.NonStrokingColorspace;
                                        transperentStrokingColor = Objects.StrokingColorspace;
                                        Objects.NonStrokingColorspace = global::Windows.UI.Colors.Transparent;
                                        Objects.StrokingColorspace = global::Windows.UI.Colors.Transparent;
                                    }
                                    else
                                    {
                                        if (Objects.NonStrokingColorspace == global::Windows.UI.Colors.Transparent || Objects.StrokingColorspace == global::Windows.UI.Colors.Transparent)
                                        {
                                            Objects.NonStrokingColorspace = transperentNonStrokingColor != global::Windows.UI.Color.FromArgb(0, 0, 0, 0) ? transperentNonStrokingColor : global::Windows.UI.Colors.Black;
                                            Objects.StrokingColorspace = transperentStrokingColor != global::Windows.UI.Color.FromArgb(0, 0, 0, 0) ? transperentStrokingColor : global::Windows.UI.Colors.Black;
                                        }
                                    }
                                    break;
                                }
                            case 62://"Tm"
                                {
                                    float a = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    float b = float.Parse(element[1], CultureInfo.InvariantCulture);
                                    float c = float.Parse(element[2], CultureInfo.InvariantCulture);
                                    float d = float.Parse(element[3], CultureInfo.InvariantCulture);
                                    float e = float.Parse(element[4], CultureInfo.InvariantCulture);
                                    float f = float.Parse(element[5], CultureInfo.InvariantCulture);

                                    if (textMatrix)
                                    {
                                        graphics.RestoreDrawingState(m_graphicsState.Pop());
                                        matrix = activeTextMatrix;
                                    }

                                    DrawingStateBlock drawingState = graphics.SaveDrawingState();
                                    m_graphicsState.Push(drawingState);


                                    if (IsTextMatrixUpdate == true)
                                    {
                                        //Scale Y
                                        float tempY = ((float)matrix.M22 * -(f));//f+d
                                        //Scale X
                                        float tempX = ((e * matrix.M11));
                                        matrix = graphics.Multiply(matrix, (new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, tempX, tempY)));
                                        matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(a * matrix.M11, -b, -c, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        graphics.Transform = matrix;
                                    }
                                    else
                                    {
                                        if (matrix == new Syncfusion.DirectXWrapper.WinRT.Matrix(0, 0, 0, 0, 0, 0))
                                        {
                                            matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(a, b, c, d, e, -f);
                                        }
                                        else
                                        {
                                            activeTextMatrix = matrix;
                                            //Scale Y
                                            float tempY = matrix.M22 * (-f);
                                            //Scale X
                                            float tempX = e * matrix.M11;
                                            matrix = graphics.Multiply(matrix, (new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, tempX, tempY)));
                                            matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(a * matrix.M11, -b, -c, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                            if (b < 0 && c < 0) //Vertical Rotation
                                            {
                                                float rotX = -graphics.Transform.M11 * matrix.M12;
                                                float rotY = -graphics.Transform.M22 * matrix.M21;
                                                matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(matrix.M11, rotX, rotY, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                            }
                                            graphics.Transform = matrix;
                                            IsTextMatrixUpdate = true;
                                        }

                                        if (IsCommonMatrixUpdated)
                                        {
                                            graphics.Transform = matrix;
                                            CurrentLocation = new PointF(0, 0);
                                        }

                                        CurrentLocation = new PointF(0, 0);
                                    }
                                    CurrentLocation = new PointF(0, 0);
                                    textMatrix = true;
                                    break;
                                }
                            case 10://"cm":
                                {
                                    IsCommonMatrixUpdated = true;
                                    IsTextMatrixUpdate = false;
                                    bool scalingUpdated = false;
                                    ////[a b c d e f]
                                    float a = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    float b = float.Parse(element[1], CultureInfo.InvariantCulture);
                                    float c = float.Parse(element[2], CultureInfo.InvariantCulture);
                                    float d = float.Parse(element[3], CultureInfo.InvariantCulture);
                                    float e = float.Parse(element[4], CultureInfo.InvariantCulture);
                                    float f = float.Parse(element[5], CultureInfo.InvariantCulture);

                                    //Checking the scaling in matrix
                                    if (matrix.M22 != 0 && matrix.M22 != 1)
                                    {
                                        //Scale Y
                                        float tempY = (matrix.M22 * -(f + d));

                                        //Scale X
                                        float tempX = ((e * matrix.M11));

                                        matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX + tempX, tempY + matrix.OffsetY);
                                        matrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(a * matrix.M11, 0, 0, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        scalingUpdated = true;
                                    }
                                    else
                                    {
                                        matrix = graphics.Multiply(matrix, (new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, e, -f)));
                                    }
                                    graphics.Transform = matrix;
                                    if ((a != 0 || d != 0) && (a != 1.0f || d != 1.0f))
                                    {
                                        if (scalingUpdated == false)
                                        {
                                            graphics.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(a, 0, 0, d, graphics.Transform.OffsetX, (graphics.Transform.OffsetY - d));
                                            matrix = graphics.Transform;
                                        }
                                    }
                                    //check for rotate transform
                                    double rad = Math.Acos(a);
                                    double degree = Math.Round((180 / Math.PI) * rad);

                                    double checkRad = Math.Asin(b);
                                    double checkDegree = Math.Round((180 / Math.PI) * checkRad);
                                    if (degree == checkDegree && degree != 0)
                                    {
                                        graphics.Transform = new Syncfusion.DirectXWrapper.WinRT.Matrix(matrix.M11, -b, -c, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        matrix = graphics.Transform;
                                    }
                                    break;
                                }
                            case 7://"BT":
                                {
                                    m_beginText = true;
                                    CurrentLocation = PointF.Empty;
                                    m_matrix.Push(matrix);
                                    pushedMatrixCount++;
                                    break;
                                }
                            case 20://"ET":
                                {
                                    //TextLeading = 0;
                                    CurrentLocation = PointF.Empty;
                                    if (textMatrix)
                                    {
                                        graphics.RestoreDrawingState(m_graphicsState.Pop());
                                        textMatrix = false;
                                    }
                                    matrix = m_matrix.Pop();
                                    pushedMatrixCount--;
                                    graphics.Transform = matrix;
                                    break;
                                }
                            case 73://"T*":
                                {
                                    DrawNewLine();
                                    break;
                                }
                            case 60://"TJ":
                                {
                                    RenderTextElementWithSpacing(element, "TJ", graphics);
                                    break;
                                }
                            case 59://"Tj":
                                {
                                    RenderTextElement(element, "Tj", graphics);
                                    break;
                                }
                            case 76://"'":
                                {
                                    DrawNewLine();
                                    RenderTextElement(element, "Tj", graphics);
                                    //RenderTextElementWithLeading(element, "'", graphics);
                                    break;
                                }
                            case 58://"Tf":
                                {
                                    RenderFont(element);
                                    break;
                                }
                            case 57://"TD":
                                {
                                    if (IsTextRotated)
                                    {
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture) * 1000, CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)) * 1000);
                                        TextLeading = -(float.Parse(element[1], CultureInfo.InvariantCulture) * 1000);
                                    }
                                    else
                                    {
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture), CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)));
                                        TextLeading = -(float.Parse(element[1], CultureInfo.InvariantCulture));
                                    }
                                    break;
                                }
                            case 56://"Td":
                                {
                                    if (IsTextRotated)
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture) * 1000, CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture) * 1000));
                                    else
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture), CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)));
                                    break;
                                }
                            case 61://"TL":
                                {
                                    TextLeading = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    break;
                                }
                            case 65://"Tw":
                                {
                                    GetWordSpacing(element);
                                    break;
                                }

                            case 55://"Tc":
                                {
                                    GetCharacterSpacing(element);
                                    break;
                                }
                            case 66://"Tz":
                                {
                                    GetScalingFactor(element);
                                    break;
                                }
                            case 43://"RG":
                            case 48://"SC":
                            case 12://"cs":
                            case 50://"SCN":
                                {
                                    GetColorspace(element, "nonstroking", "RGB");
                                    break;
                                }
                            case 34://"k":
                                {
                                    GetColorspace(element, "stroking", "DeviceCMYK");
                                    break;
                                }
                            case 33://"K":
                                {
                                    GetColorspace(element, "nonstroking", "DeviceCMYK");
                                    break;
                                }
                            case 44://"rg":
                            case 49://"sc":
                            case 51://"scn":
                            case 11://"CS":
                                {
                                    GetColorspace(element, "stroking", "RGB");
                                    break;
                                }
                            case 26://"g":
                                {
                                    GetColorspace(element, "stroking", "Gray");
                                    break;
                                }
                            case 25://"G":
                                {
                                    GetColorspace(element, "nonstroking", "Gray");
                                    break;
                                }
                            case 16://"Do":
                                {
                                    GetXObject(element, graphics);
                                    break;
                                }
                            case 42://"re":
                                {
                                    GetClipRectangle(element);
                                    break;
                                }
                            case 13://"d":
                                {
                                    if (element[0] != "[]" && !element[0].Contains("\n"))
                                    {
                                        m_dashedLine = element;
                                    }
                                    break;
                                }
                            case 0://"b":
                                {
                                    /*CloseFillStrokePath*/
                                    FillPath("NonZeroWindingNumberRule", true, true, graphics);
                                    break;
                                }
                            case 74://"b*":
                                {
                                    /*CloseFillStrokePath*/
                                    FillPath("EvenOddRule", true, true, graphics);
                                    break;
                                }
                            case 1://"B":
                                {
                                    /*FillStrokePath*/
                                    FillPath("NonzeroWindingNumberRule", false, true, graphics);
                                    break;
                                }
                            case 75://"B*":
                                {
                                    /*FillStrokePath*/
                                    FillPath("EvenOddRule", false, true, graphics);
                                    break;
                                }
                            case 28://"h":
                                {
                                    if (PathSink != null && !IsPathSinkClosed)
                                    {
                                        PathSink.EndFigure(FigureEnd.Closed);
                                        PathSink.Close();
                                        IsPathSinkClosed = true;
                                    }
                                    break;
                                }
                            case 39://"n":
                                {
                                    try
                                    {
                                        Path = null;
                                        PathSink = null;
                                        pathRenderList.Clear();
                                        m_clipRectangleList.Clear();
                                    }
                                    catch
                                    {
                                    }
                                    ClipRectangle = Rect.Empty;
                                    break;
                                }
                            case 69://"W":
                                {
                                    if ((ClipRectangle != new Rect(0, 0, 0, 0)))
                                    {
                                        LayerParameters layerparameters = new LayerParameters();
                                        //Layer layer = graphics.CreateLayer();
                                        layerparameters.Opacity = 1f;

                                        RectF bounds = new RectF();
                                        bounds.Left = (float)ClipRectangle.Left;
                                        bounds.Top = (float)ClipRectangle.Top;
                                        bounds.Right = (float)ClipRectangle.Right;
                                        bounds.Bottom = (float)ClipRectangle.Bottom;

                                        layerparameters.ContentBounds = bounds;
                                        graphics.PushLayer(layerparameters);
                                        data.LayerCounts++;
                                    }
                                    else
                                    {
#region PathClliping

                                        if (PathSink != null && IsPathSinkClosed == false)
                                        {
                                            PathSink.EndFigure(FigureEnd.Closed);
                                            PathSink.Close();
                                            IsPathSinkClosed = true;
                                        }

                                        if (PathSink != null)
                                        {
                                            if (data.ClipPath.PathGeomentry != null)
                                            {
                                                PathGeometry m_pPathGeometryUnion = graphics.CreatePathGeometry();
                                                GeometrySink pGeometrySink = m_pPathGeometryUnion.Open();
                                                GeometrySink geoSink = data.ClipPath.PathSink;
                                                PathGeometry geoPath = data.ClipPath.PathGeomentry;
                                                if (!data.ClipPath.IsCurrentPathSinkClosed)
                                                {
                                                    geoSink.EndFigure(FigureEnd.Closed);
                                                    geoSink.Close();
                                                }
                                                Path.Combine(geoPath, CombineMode.Intersect, pGeometrySink);
                                                pGeometrySink.Close();
                                                PathGeometry[] pathGeos = new PathGeometry[1];
                                                pathGeos[0] = Path;
                                                GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Winding, pathGeos);
                                                LayerParameters layerparameters = new LayerParameters();
                                                // Layer layer = graphics.CreateLayer();
                                                layerparameters.Opacity = 1f;
                                                layerparameters.GeometricMask = geoGroup;
                                                layerparameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;
                                                layerparameters.MaskTransform = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 1, 1);
                                                string scaleY = matrix.M22.ToString();
                                                string multiplyerY = "1";
                                                if (scaleY.Contains("."))
                                                {
                                                    int length = scaleY.Substring(scaleY.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerY += "0";
                                                    }
                                                }
                                                string scaleX = matrix.M22.ToString();
                                                string multiplyerX = "1";
                                                if (scaleX.Contains("."))
                                                {
                                                    int length = scaleX.Substring(scaleX.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerX += "0";
                                                    }
                                                }
                                                int mulX = Int32.Parse(multiplyerX, CultureInfo.InvariantCulture);
                                                int mulY = Int32.Parse(multiplyerY, CultureInfo.InvariantCulture);
                                                float layerSizeY = 0;
                                                if (IsNegativePath)
                                                {
                                                    layerSizeY = -graphics.Height * mulY;
                                                }
                                                else
                                                {
                                                    layerSizeY = graphics.Height * mulY;
                                                }
                                                if (mulY > 1 || mulX > 1)
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Left = bounds.Top = 1;
                                                    bounds.Right = graphics.Width * mulX;
                                                    bounds.Bottom = layerSizeY;
                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                else
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Left = bounds.Top = 1;
                                                    bounds.Right = graphics.Width;
                                                    bounds.Bottom = layerSizeY;
                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                graphics.PushLayer(layerparameters);
                                                data.LayerCounts++;
                                                data.ClipPath = new PathRenderer();
                                                data.ClipPath.PathGeomentry = m_pPathGeometryUnion;
                                                data.ClipPath.PathSink = pGeometrySink;
                                                data.ClipPath.IsCurrentPathSinkClosed = true;
                                            }
                                            else
                                            {
                                                data.ClipPath = new PathRenderer();
                                                data.ClipPath.PathGeomentry = Path;
                                                data.ClipPath.PathSink = PathSink;
                                                data.ClipPath.IsCurrentPathSinkClosed = true;
                                                LayerParameters layerparameters = new LayerParameters();
                                                //Layer layer = graphics.CreateLayer();
                                                layerparameters.Opacity = 1f;
                                                layerparameters.GeometricMask = Path;
                                                layerparameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;
                                                layerparameters.MaskTransform = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 1, 1);
                                                string scaleY = matrix.M22.ToString();
                                                string multiplyerY = "1";
                                                if (scaleY.Contains("."))
                                                {
                                                    int length = scaleY.Substring(scaleY.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerY += "0";
                                                    }
                                                }
                                                string scaleX = matrix.M22.ToString();
                                                string multiplyerX = "1";
                                                if (scaleX.Contains("."))
                                                {
                                                    int length = scaleX.Substring(scaleX.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerX += "0";
                                                    }
                                                }
                                                int mulX = Int32.Parse(multiplyerX, CultureInfo.InvariantCulture);
                                                int mulY = Int32.Parse(multiplyerY, CultureInfo.InvariantCulture);
                                                float layerSizeY = 0;
                                                if (IsNegativePath)
                                                {
                                                    layerSizeY = -graphics.Height * mulY;
                                                }
                                                else
                                                {
                                                    layerSizeY = graphics.Height * mulY;
                                                }
                                                if (mulY > 1 || mulX > 1)
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width * mulX;
                                                    bounds.Bottom = layerSizeY;
                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                else
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width;
                                                    bounds.Bottom = layerSizeY;
                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                graphics.PushLayer(layerparameters);
                                                data.LayerCounts++;
                                            }
                                        }
#endregion
                                    }
                                    break;
                                }
                            case 70://"W*":
                                {
                                    if (ClipRectangle != new Rect(0, 0, 0, 0))
                                    {
                                        LayerParameters layerparameters = new LayerParameters();
                                        //Layer layer = graphics.CreateLayer();
                                        layerparameters.Opacity = 1f;

                                        RectF bounds = new RectF();
                                        bounds.Left = (float)ClipRectangle.Left;
                                        bounds.Top = (float)ClipRectangle.Top;
                                        bounds.Right = (float)ClipRectangle.Right;
                                        bounds.Bottom = (float)ClipRectangle.Bottom;

                                        layerparameters.ContentBounds = bounds;
                                        graphics.PushLayer(layerparameters);
                                        data.LayerCounts++;
                                    }
                                    else
                                    {
#region PathClliping
                                        if (PathSink != null && IsPathSinkClosed == false)
                                        {
                                            PathSink.EndFigure(FigureEnd.Closed);
                                            PathSink.Close();
                                            IsPathSinkClosed = true;
                                        }
                                        if (PathSink != null)
                                        {
                                            if (data.ClipPath.PathGeomentry != null)
                                            {
                                                PathGeometry m_pPathGeometryUnion = graphics.CreatePathGeometry();
                                                GeometrySink pGeometrySink = m_pPathGeometryUnion.Open();
                                                GeometrySink geoSink = data.ClipPath.PathSink;
                                                PathGeometry geoPath = data.ClipPath.PathGeomentry;
                                                if (!data.ClipPath.IsCurrentPathSinkClosed)
                                                {
                                                    geoSink.EndFigure(FigureEnd.Closed);
                                                    geoSink.Close();
                                                }
                                                Path.Combine(geoPath, CombineMode.Intersect, pGeometrySink);
                                                pGeometrySink.Close();
                                                PathGeometry[] pathGeos = new PathGeometry[1];
                                                pathGeos[0] = Path;
                                                GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Alternate, pathGeos);
                                                LayerParameters layerparameters = new LayerParameters();
                                                //Layer layer = graphics.CreateLayer();
                                                layerparameters.Opacity = 1f;
                                                layerparameters.GeometricMask = geoGroup;
                                                layerparameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;
                                                layerparameters.MaskTransform = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 1, 1);
                                                string scaleY = matrix.M22.ToString();
                                                string multiplyerY = "1";
                                                if (scaleY.Contains("."))
                                                {
                                                    int length = scaleY.Substring(scaleY.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerY += "0";
                                                    }
                                                }
                                                string scaleX = matrix.M22.ToString();
                                                string multiplyerX = "1";
                                                if (scaleX.Contains("."))
                                                {
                                                    int length = scaleX.Substring(scaleX.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerX += "0";
                                                    }
                                                }
                                                int mulX = Int32.Parse(multiplyerX, CultureInfo.InvariantCulture);
                                                int mulY = Int32.Parse(multiplyerY, CultureInfo.InvariantCulture);
                                                float layerSizeY = 0;
                                                if (IsNegativePath)
                                                {
                                                    layerSizeY = -graphics.Height * mulY;
                                                }
                                                else
                                                {
                                                    layerSizeY = graphics.Height * mulY;
                                                }
                                                if (mulY > 1 || mulX > 1)
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width * mulX;
                                                    bounds.Bottom = layerSizeY;

                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                else
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width * mulX;
                                                    bounds.Bottom = layerSizeY;

                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                graphics.PushLayer(layerparameters);
                                                data.LayerCounts++;
                                                data.ClipPath = new PathRenderer();
                                                data.ClipPath.PathGeomentry = m_pPathGeometryUnion;
                                                data.ClipPath.PathSink = pGeometrySink;
                                                data.ClipPath.IsCurrentPathSinkClosed = true;
                                            }
                                            else
                                            {
                                                data.ClipPath = new PathRenderer();
                                                data.ClipPath.PathGeomentry = Path;
                                                data.ClipPath.PathSink = PathSink;
                                                data.ClipPath.IsCurrentPathSinkClosed = true;
                                                PathGeometry[] pathGeos = new PathGeometry[pathRenderList.Count + 1];
                                                pathGeos[pathRenderList.Count] = Path;

                                                if (pathRenderList.Count > 0)
                                                {
                                                    int i = 0;
                                                    foreach (PathRenderer pth in pathRenderList)
                                                    {
                                                        GeometrySink geoSink = pth.PathSink;
                                                        PathGeometry geoPath = pth.PathGeomentry;
                                                        if (!pth.IsCurrentPathSinkClosed)
                                                        {
                                                            geoSink.EndFigure(FigureEnd.Closed);
                                                            geoSink.Close();
                                                        }
                                                        pathGeos[i] = geoPath;
                                                        i++;
                                                    }
                                                    pathRenderList.Clear();
                                                }

                                                GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Alternate, pathGeos);
                                                LayerParameters layerparameters = new LayerParameters();
                                                //Layer layer = graphics.CreateLayer();
                                                layerparameters.Opacity = 1f;
                                                layerparameters.GeometricMask = geoGroup;
                                                layerparameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;
                                                layerparameters.MaskTransform = new Syncfusion.DirectXWrapper.WinRT.Matrix(1, 0, 0, 1, 1, 1);

                                                string scaleY = matrix.M22.ToString();
                                                string multiplyerY = "1";
                                                if (scaleY.Contains("."))
                                                {
                                                    int length = scaleY.Substring(scaleY.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerY += "0";
                                                    }
                                                }
                                                string scaleX = matrix.M22.ToString();
                                                string multiplyerX = "1";
                                                if (scaleX.Contains("."))
                                                {
                                                    int length = scaleX.Substring(scaleX.IndexOf(".") + 1).Length;

                                                    for (int i = 0; i < length; i++)
                                                    {
                                                        multiplyerX += "0";
                                                    }
                                                }
                                                int mulX = Int32.Parse(multiplyerX, CultureInfo.InvariantCulture);
                                                int mulY = Int32.Parse(multiplyerY, CultureInfo.InvariantCulture);
                                                float layerSizeY = 0;
                                                if (IsNegativePath)
                                                {
                                                    layerSizeY = -graphics.Height * mulY;
                                                }
                                                else
                                                {
                                                    layerSizeY = graphics.Height * mulY;
                                                }
                                                if (mulY > 1 || mulX > 1)
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width * mulX;
                                                    bounds.Bottom = layerSizeY;

                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                else
                                                {
                                                    RectF bounds = new RectF();
                                                    bounds.Top = bounds.Left = 1;
                                                    bounds.Right = graphics.Width;
                                                    bounds.Bottom = layerSizeY;

                                                    layerparameters.ContentBounds = bounds;
                                                }
                                                graphics.PushLayer(layerparameters);
                                                data.LayerCounts++;
                                            }
                                        }
#endregion
                                    }
                                    break;
                                }
                            case 68://"w":
                                {
                                    PdfUnitConvertor m_convertor = new PdfUnitConvertor();
                                    m_mitterLength = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    break;
                                }
                            case 46://"s":
                                {
                                    DrawPath(true, graphics);
                                    break;
                                }
                            case 47://"S":
                                {
                                    DrawPath(false, graphics);
                                    break;
                                }
                            case 53://"f*":
                                {
                                    /*CloseFillStrokePath*/
                                    FillPath("EvenOddRule", true, false, graphics);
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            case 22://"f":
                                {
                                    /*CloseFillStrokePath*/
                                    FillPath("NonZeroWindingNumberRule", true, false, graphics);
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            case 67://"v":
                                {
                                    AddBezierCurve2(element);
                                    break;
                                }
                            case 72://"y":
                                {
                                    AddBezierCurve3(element);
                                    break;
                                }
                            case 36://"m":
                                {
                                    BeginPath(element, graphics);
                                    break;
                                }
                            case 9://"c":
                                {
                                    AddBezierCurve(element);
                                    break;
                                }
                            case 35://"l":
                                {
                                    AddLine(element);
                                    break;
                                }
                            default:
                                {
                                    //Debug.WriteLine(token + " not implemented");
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChageTextElementWidth(string token)
        {
            if (currentTextRenderingOperator == string.Empty)
            {
                currentTextRenderingOperator = token;
            }
            else
            {
                if (token != currentTextRenderingOperator && token == "'")
                {
                    m_textElementWidth = 0;
                }
            }
        }

        private void DrawPath(bool closePath, Graphics2D graphics)
        {
            Rect emptyRectangle = new Rect(0, 0, 0, 0);
            global::Windows.UI.Color emptyColor = new global::Windows.UI.Color();
            GraphicBrush brush1 = new GraphicBrush(StrokingColorSpace);
            GraphicBrush brush2 = new GraphicBrush(global::Windows.UI.Colors.Black);
            GraphicBrush brush3 = new GraphicBrush(NonStrokingColorSpace);

            if ((ClipRectangle != emptyRectangle) && !ClipRectangle.IsEmpty)
            {
                if (NonStrokingColorSpace != emptyColor)
                {
                    graphics.DrawRectangle(brush3, ClipRectangle);
                }
                else
                {
                    graphics.DrawRectangle(brush2, ClipRectangle);
                }
                ClipRectangle = emptyRectangle;
            }
            if (m_clipRectangleList.Count > 0)
            {
                foreach (Rect rect in m_clipRectangleList)
                {
                    if (StrokingColorSpace != emptyColor)
                    {
                        graphics.DrawRectangle(brush1, rect);
                    }
                    else
                    {
                        graphics.DrawRectangle(brush2, rect);
                    }
                }
                m_clipRectangleList.Clear();
            }
            if (PathSink != null)
            {
                if (closePath)
                {
                    PathSink.EndFigure(FigureEnd.Closed);
                }
                else
                {
                    PathSink.EndFigure(FigureEnd.Open);
                }
                if (IsPathSinkClosed == false)
                {
                    PathSink.Close();
                    IsPathSinkClosed = true;
                }
                if (MitterLength != 0)
                {
                    if (NonStrokingColorSpace == emptyColor)
                    {
                        graphics.DrawGeometry(Path, brush2, MitterLength);
                    }
                    else
                    {
                        graphics.DrawGeometry(Path, brush3, MitterLength);
                    }
                }
                else
                {

                    if (NonStrokingColorSpace == emptyColor)
                    {
                        graphics.DrawGeometry(Path, brush2);
                    }
                    else
                    {
                        graphics.DrawGeometry(Path, brush3);
                    }

                }
                PathSink = null;
            }
            if (pathRenderList.Count > 0)
            {
                Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = graphics.Transform;
                int i = 0;

                foreach (PathRenderer pth in pathRenderList)
                {
                    GeometrySink geoSink = pth.PathSink;
                    PathGeometry geoPath = pth.PathGeomentry;

                    if (!pth.IsCurrentPathSinkClosed)
                    {
                        if (closePath)
                        {
                            geoSink.EndFigure(FigureEnd.Closed);
                        }
                        else
                        {
                            geoSink.EndFigure(FigureEnd.Open);
                        }
                        geoSink.Close();
                    }
                    graphics.Transform = pathMatrix[i];
                    if (MitterLength != 0)
                    {
                        if (NonStrokingColorSpace == emptyColor)
                        {
                            graphics.DrawGeometry(geoPath, brush2, MitterLength);
                        }
                        else
                        {
                            graphics.DrawGeometry(geoPath, brush3, MitterLength);
                        }
                    }
                    else
                    {
                        if (NonStrokingColorSpace == emptyColor)
                        {
                            graphics.DrawGeometry(geoPath, brush2);
                        }
                        else
                        {
                            graphics.DrawGeometry(geoPath, brush3);
                        }

                    }
                    i++;
                }
                graphics.Transform = tempMatrix;
                pathMatrix.Clear();
                pathRenderList.Clear();
            }

            brush3.Dispose();
            brush2.Dispose();
            brush1.Dispose();
        }

        private void AddSubPaths(Graphics2D graphics)
        {
            if (PathSink != null /*&& IsPathSinkClosed == false*/)
            {
                PathRenderer renderer = new PathRenderer();
                renderer.PathGeomentry = Path;
                renderer.PathSink = PathSink;
                renderer.IsCurrentPathSinkClosed = IsPathSinkClosed;
                pathRenderList.Add(renderer);
                pathMatrix.Add(graphics.Transform);
            }
        }

        private void AddLine(string[] line)
        {
            if (float.Parse(line[1], CultureInfo.InvariantCulture) < 0)
            {
                IsNegativePath = false;
            }
            else
            {
                IsNegativePath = true;
            }

            if (!IsPathSinkClosed)
            {
                PathSink.AddLine(new global::Windows.Foundation.Point(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture)));
            }
            else
            {
                PathSink = Path.Open();
                PathSink.AddLine(new global::Windows.Foundation.Point(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture)));
            }
            CurrentLocation = new PointF(float.Parse(line[0], CultureInfo.InvariantCulture), -float.Parse(line[1], CultureInfo.InvariantCulture));
        }

        private void AddBezierCurve(string[] curve)
        {
            if (float.Parse(curve[1], CultureInfo.InvariantCulture) < 0 && float.Parse(curve[3], CultureInfo.InvariantCulture) < 0 && float.Parse(curve[5], CultureInfo.InvariantCulture) < 0)
            {
                IsNegativePath = false;
            }
            else
            {
                IsNegativePath = true;
            }
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new global::Windows.Foundation.Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            bezier.Point2 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            bezier.Point3 = new global::Windows.Foundation.Point(float.Parse(curve[4], CultureInfo.InvariantCulture), -float.Parse(curve[5], CultureInfo.InvariantCulture));
            PathSink.AddBezier(bezier);
            CurrentLocation = new PointF(float.Parse(curve[4], CultureInfo.InvariantCulture), -float.Parse(curve[5], CultureInfo.InvariantCulture));
        }

        private void AddBezierCurve2(string[] curve)
        {
            if (float.Parse(curve[1], CultureInfo.InvariantCulture) < 0 && float.Parse(curve[3], CultureInfo.InvariantCulture) < 0)
            {
                IsNegativePath = false;
            }
            else
            {
                IsNegativePath = true;
            }
            PointF point4 = new PointF(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new global::Windows.Foundation.Point(CurrentLocation.X, CurrentLocation.Y);
            bezier.Point2 = new global::Windows.Foundation.Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            bezier.Point3 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            //PathSink.AddBezier(bezier);
            QuadraticBezierSegment quadraticbezier = new QuadraticBezierSegment();
            quadraticbezier.Point1 = new global::Windows.Foundation.Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            quadraticbezier.Point2 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            PathSink.AddQuadraticBezier(quadraticbezier);
            CurrentLocation = point4;
        }

        private void AddBezierCurve3(string[] curve)
        {
            if (float.Parse(curve[1], CultureInfo.InvariantCulture) < 0 && float.Parse(curve[3], CultureInfo.InvariantCulture) < 0)
            {
                IsNegativePath = false;
            }
            else
            {
                IsNegativePath = true;
            }
            PointF point4 = new PointF(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new global::Windows.Foundation.Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            bezier.Point2 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            bezier.Point3 = new global::Windows.Foundation.Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            PathSink.AddBezier(bezier);
            CurrentLocation = point4;
        }

        private void BeginPath(string[] point, Graphics2D graphics)
        {
            AddSubPaths(graphics);
            Path = graphics.CreatePathGeometry();
            PathSink = Path.Open();
            IsPathSinkClosed = false;
            PathSink.SetFillMode(FillMode.Alternate);
            PathSink.BeginFigure(new global::Windows.Foundation.Point(float.Parse(point[0], CultureInfo.InvariantCulture), -float.Parse(point[1], CultureInfo.InvariantCulture)), FigureBegin.Filled);
        }

        private void FillPath(string rule, bool Close, bool Stroke, Graphics2D graphics)
        {
            Rect emptyRectangle = new Rect(0, 0, 0, 0);
            global::Windows.UI.Color emptyColor = new global::Windows.UI.Color();
            GraphicBrush brush1 = new GraphicBrush(StrokingColorSpace);
            GraphicBrush brush2 = new GraphicBrush(NonStrokingColorSpace, MitterLength);
            GraphicBrush brush3 = new GraphicBrush(global::Windows.UI.Colors.Black);

            if ((ClipRectangle != emptyRectangle) && !ClipRectangle.IsEmpty)
            {
                graphics.FillRectangle(brush1, ClipRectangle);
                if (Stroke)
                    graphics.DrawRectangle(brush2, ClipRectangle);
                ClipRectangle = emptyRectangle;
            }
            if (m_clipRectangleList.Count > 0)
            {
                foreach (Rect rect in m_clipRectangleList)
                {
                    graphics.FillRectangle(brush1, rect);
                    if (Stroke)
                        graphics.DrawRectangle(brush2, rect);
                }
                m_clipRectangleList.Clear();
            }
            if (PathSink != null && IsPathSinkClosed == false)
            {
                if (Close)
                    PathSink.EndFigure(FigureEnd.Closed);
                else
                    PathSink.EndFigure(FigureEnd.Open);
                PathSink.Close();
                IsPathSinkClosed = true;
            }

            if (PathSink == null && pathRenderList.Count == 0)
                return;

            PathGeometry[] pathGeos = new PathGeometry[pathRenderList.Count + 1];
            pathGeos[pathRenderList.Count] = Path;

            if (pathRenderList.Count > 0)
            {
                Syncfusion.DirectXWrapper.WinRT.Matrix tempMatrix = graphics.Transform;
                int i = 0;

                foreach (PathRenderer pth in pathRenderList)
                {
                    GeometrySink geoSink = pth.PathSink;
                    PathGeometry geoPath = pth.PathGeomentry;
                    if (!pth.IsCurrentPathSinkClosed)
                    {
                        if (Close)
                            geoSink.EndFigure(FigureEnd.Closed);
                        else
                            geoSink.EndFigure(FigureEnd.Open);
                        geoSink.Close();
                    }
                    pathGeos[i] = geoPath;
                    i++;
                }
                graphics.Transform = tempMatrix;
                pathMatrix.Clear();
                pathRenderList.Clear();
            }

            if (rule == "EvenOddRule")
            {
                GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Alternate, pathGeos);
                if (StrokingColorSpace == emptyColor)
                {
                    graphics.FillGeometry(geoGroup, brush3);
                    if (Stroke)
                        graphics.DrawGeometry(geoGroup, brush3);
                }
                else
                {
                    graphics.FillGeometry(geoGroup, brush1);
                    if (Stroke)
                        graphics.DrawGeometry(geoGroup, brush2, MitterLength);
                }
                PathSink = null;
                geoGroup = null;
            }
            else
            {
                GeometryGroup geoGroup = graphics.CreateGeometryGroup(FillMode.Winding, pathGeos);
                if (StrokingColorSpace == emptyColor)
                {
                    graphics.FillGeometry(geoGroup, brush3);
                    if (Stroke)
                        graphics.DrawGeometry(geoGroup, brush3);
                }
                else
                {
                    graphics.FillGeometry(geoGroup, brush1);
                    if (Stroke)
                        graphics.DrawGeometry(geoGroup, brush2, MitterLength);
                }
                PathSink = null;
                geoGroup = null;
            }
            pathGeos = null;
            brush3.Dispose();
            brush2.Dispose();
            brush1.Dispose();
        }

        private void GetClipRectangle(string[] rectangle)
        {
            float x = float.Parse(rectangle[0], CultureInfo.InvariantCulture);
            float y = -float.Parse(rectangle[1], CultureInfo.InvariantCulture);
            float width = float.Parse(rectangle[2], CultureInfo.InvariantCulture);
            float height = -float.Parse(rectangle[3], CultureInfo.InvariantCulture);
            RectangleF currentRect = new RectangleF(x, y, width, height);
            if (height < 0)
            {
                height = -height;
                y = y - height;
            }

            if (width < 0)
            {
                width = -width;
                x = x - width;
            }

            if ((ClipRectangle != new Rect(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangleList.Add(m_clipRectangle);
            }

            if ((ClipRectangle != new Rect(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangle = new Rect(x, y, width, height);
            }
            else
            {
                m_clipRectangle = new Rect(x, y, width, height);
            }

            ////Adding rectangles to the Path
            //float num4, num3, num2, num1;
            //num4 = float.Parse(rectangle[0]);
            //num3 = float.Parse(rectangle[1]);
            //num2 = float.Parse(rectangle[2]);
            //num1 = float.Parse(rectangle[3]);
            //AddSubPaths();
            //Path = new SharpDX.Direct2D1.PathGeometry(m_renderTarget.Factory);
            //PathSink = Path.Open();
            //IsPathSinkClosed = false;
            //PathSink.SetFillMode(SharpDX.Direct2D1.FillMode.Alternate);
            //PathSink.BeginFigure(new SharpDX.DrawingPointF(0,0), SharpDX.Direct2D1.FigureBegin.Filled);
            //PathSink.AddLine(new SharpDX.DrawingPointF(num4, num3));
            //PathSink.AddLine(new SharpDX.DrawingPointF(num4 + num2, num3));
            //PathSink.AddLine(new SharpDX.DrawingPointF(num4 + num2, num3 + num1));
            //PathSink.AddLine(new SharpDX.DrawingPointF(num4, num3 + num1));
        }

        private void RenderTextElement(string[] textElements, string tokenType, Graphics2D graphics)
        {
            ChageTextElementWidth(tokenType);
            string text = string.Join("", textElements);
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;

                TextElement element = new TextElement(text);
                element.FontName = structure.EquivalentFontName;
                element.Font = structure.CurrentFontFace;
                text = structure.Decode(text, m_resources.isSameFont());
                element.Text = text;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.FontEncoding = structure.FontEncoding;
                element.isNegativeFont = isNegativeFont;
                element.FontGlyphWidths = structure.FontGlyphWidths;
                element.CustomFontSize = structure.FontSize;
                element.CharacterMapTable = structure.CharacterMapTable;
                element.ReverseMapTable = structure.ReverseMapTable;
                if (structure.IsType1Font)
                {
                    element.IsType1Font = true;
                    element.differenceTable = structure.differenceTable;
                    element.differenceMappedTable = structure.DifferencesDictionary;
                    element.m_cffGlyphs = structure.m_cffGlyphs;
                    if (!m_glyphDataCollection.Contains(element.m_cffGlyphs))
                    {
                        m_glyphDataCollection.Add(element.m_cffGlyphs);
                    }
                }
                element.FontWeight = structure.FontWeight;
                element.FontStyle = structure.FontStyles;
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                element.CidToGidMap = structure.CidToGidMap;
                element.IsC1 = structure.IsC1;
                element.textRenderingMode = textRenderingMode;
                structure = null;
                if (element.FontSize < 0)
                {
                    element.FontSize = -element.FontSize;
                    element.isNegativeFont = true;
                }
                else
                {
                    element.isNegativeFont = false;
                }

                if (StrokingColorSpace != new global::Windows.UI.Color())
                {
                    element.BrushColor = StrokingColorSpace;
                }
                else
                {
                    element.BrushColor = global::Windows.UI.Color.FromArgb(255, 255, 255, 255);//.Black;
                }

                if (m_beginText)
                {
                    m_beginText = false;
                }
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    m_endTextPosition = CurrentLocation;
                    if (StrokingColorSpace.A == 50)
                        m_textElementWidth = element.Render(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - element.FontSize), global::Windows.UI.Colors.Black);
                    else
                        m_textElementWidth = element.Render(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - element.FontSize), StrokingColorSpace);
                }
                else
                {
                    m_endTextPosition = new PointF(m_endTextPosition.X + m_textElementWidth, m_endTextPosition.Y);
                    if (StrokingColorSpace.A == 50)
                        m_textElementWidth = element.Render(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - element.FontSize), global::Windows.UI.Colors.Black);
                    else
                        m_textElementWidth = element.Render(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - element.FontSize), StrokingColorSpace);
                }

                element = null;
                structure = null;
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }

        private void RenderTextElementWithSpacing(string[] textElements, string tokenType, Graphics2D graphics)
        {
            ChageTextElementWidth(tokenType);
            List<string> decodedList = new List<string>();
            string text = string.Join("", textElements);
            string tempText = text;
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;

                TextElement element = new TextElement(text);
                element.FontName = structure.EquivalentFontName;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.Font = structure.CurrentFontFace;
                decodedList = structure.DecodeTextTJ(text, m_resources.isSameFont());
                element.ReverseMapTable = structure.ReverseMapTable;
                element.CharacterMapTable = structure.CharacterMapTable;
                if (structure.IsType1Font)
                {
                    element.IsType1Font = true;
                    element.differenceTable = structure.differenceTable;
                    element.differenceMappedTable = structure.DifferencesDictionary;
                    element.m_cffGlyphs = structure.m_cffGlyphs;
                    if (!m_glyphDataCollection.Contains(element.m_cffGlyphs))
                    {
                        m_glyphDataCollection.Add(element.m_cffGlyphs);
                    }
                }
                element.FontEncoding = structure.FontEncoding;
                element.FontGlyphWidths = structure.FontGlyphWidths;
                element.CustomFontSize = structure.FontSize;
                element.DefaultWidth = structure.DefaultWidth;
                element.FontWeight = structure.FontWeight;
                element.FontStyle = structure.FontStyles;
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                element.CidToGidMap = structure.CidToGidMap;
                element.IsC1 = structure.IsC1;
                element.textRenderingMode = textRenderingMode;
                structure = null;
                if (element.FontSize < 0)
                {
                    element.FontSize = -element.FontSize;
                    element.isNegativeFont = true;
                }
                else
                {
                    element.isNegativeFont = false;
                }
                if (StrokingColorSpace != new global::Windows.UI.Color())
                {
                    element.BrushColor = StrokingColorSpace;
                }
                else
                {
                    element.BrushColor = global::Windows.UI.Color.FromArgb(255, 255, 255, 255);
                }
                if (m_beginText)
                {
                    m_beginText = false;
                }
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    m_endTextPosition = CurrentLocation;
                    if (StrokingColorSpace.A == 50)
                        m_textElementWidth = element.RenderWithSpace(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, global::Windows.UI.Colors.Black);
                    else
                        m_textElementWidth = element.RenderWithSpace(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, StrokingColorSpace);
                }
                else
                {
                    m_endTextPosition = new PointF(m_endTextPosition.X + m_textElementWidth, m_endTextPosition.Y);
                    if (StrokingColorSpace.A == 50)
                        m_textElementWidth = element.RenderWithSpace(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, global::Windows.UI.Colors.Black);
                    else
                        m_textElementWidth = element.RenderWithSpace(out IsTextRotated, graphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, StrokingColorSpace);
                }
                element = null;
            }
        }

        private void DrawNewLine()
        {
            m_isCurrentPositionChanged = true;
            m_currentLocation.Y = (TextLeading + m_currentLocation.Y);//0 -Tl Td
        }
        private global::Windows.UI.Xaml.Media.Matrix AppendMatrix(global::Windows.UI.Xaml.Media.Matrix matrix1, global::Windows.UI.Xaml.Media.Matrix matrix2)
        {
            return new global::Windows.UI.Xaml.Media.Matrix(matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21, matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22, matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21, matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22, matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX, matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);
        }
        private void RenderFont(string[] fontElements)
        {
            int i;
            for (i = 0; i < fontElements.Length; i++)
            {
                if (fontElements[i].Contains("/"))
                {
                    CurrentFont = fontElements[i].Replace("/", "");
                    break;
                }
            }
            FontSize = float.Parse(fontElements[i + 1], CultureInfo.InvariantCulture);
        }


        private void GetWordSpacing(string[] spacing)
        {
            m_wordSpacing = float.Parse(spacing[0], CultureInfo.InvariantCulture);
        }

        private void GetCharacterSpacing(string[] spacing)
        {
            m_characterSpacing = float.Parse(spacing[0], CultureInfo.InvariantCulture);
        }

        private void GetScalingFactor(string[] scaling)
        {
            m_textScaling = float.Parse(scaling[0], CultureInfo.InvariantCulture);
            if (m_textScaling < 0)
                m_textScaling = -m_textScaling;
        }

        private void GetColorspace(string[] colorElement, string type, string colorSpace)
        {
            float r, g, b = 1;
            byte fa = 255;
            if (colorSpace == "RGB" && colorElement.Length == 3)
            {
                r = float.Parse(colorElement[0], CultureInfo.InvariantCulture);
                g = float.Parse(colorElement[1], CultureInfo.InvariantCulture);
                b = float.Parse(colorElement[2], CultureInfo.InvariantCulture);
            }
            else if (colorSpace == "Gray" && colorElement.Length == 1)
            {
                r = g = b = float.Parse(colorElement[0], CultureInfo.InvariantCulture);
            }

            else if (colorSpace == "DeviceCMYK" && colorElement.Length == 4)
            {
                float num5 = float.Parse(colorElement[3], CultureInfo.InvariantCulture);
                float num6 = float.Parse(colorElement[2], CultureInfo.InvariantCulture);
                float num7 = float.Parse(colorElement[1], CultureInfo.InvariantCulture);
                float num8 = float.Parse(colorElement[0], CultureInfo.InvariantCulture);

                r = (float)((num8 * (num5 - 1.0) + 1.0 - num5));
                g = (float)((num7 * (num5 - 1.0) + 1.0 - num5));
                b = (float)((num6 * (num5 - 1.0) + 1.0 - num5));

            }
            else
            {
                r = g = b = 0.5f;
                fa = 50;
            }

            byte fr = (byte)Math.Floor(r == 1.0 ? 255 : r * 256.0);
            byte fg = (byte)Math.Floor(g == 1.0 ? 255 : g * 256.0);
            byte fb = (byte)Math.Floor(b == 1.0 ? 255 : b * 256.0);

            if (type == "nonstroking")
                Objects.NonStrokingColorspace = global::Windows.UI.Color.FromArgb(fa, fr, fg, fb);
            else
                Objects.StrokingColorspace = global::Windows.UI.Color.FromArgb(fa, fr, fg, fb);
        }

        private void GetXObject(string[] xobjectElement, Graphics2D graphics)
        {
            if (m_resources.ContainsKey(xobjectElement[0].Replace("/", "")))
            {
                if (m_resources[xobjectElement[0].Replace("/", "")] is XObjectElement)
                {
                    pushedMatrixCount = 0;
                    m_parentResources.Push(m_resources);
                    float temp = CharacterSpacing;
                    PdfDocumentPage childObject = (m_resources[xobjectElement[0].Replace("/", "")] as XObjectElement).Render(m_resources, matrix);
                    m_resources = childObject.m_resources;
                    //Applying the internal form matrix transformation
                    matrix = childObject.FormMatrix;
                    graphics.Transform = matrix;
                    RenderContent(childObject.m_recordCollection, graphics);
                    m_resources = m_parentResources.Pop();
                    CharacterSpacing = temp;
                    for (int i = 0; i < pushedMatrixCount; i++)
                    {
                        m_matrix.Pop();
                    }
                }
                else if (m_resources[xobjectElement[0].Replace("/", "")] is ImageStructure)
                {
                    ImageStructure structure = new ImageStructure();
                    try
                    {
                        structure = m_resources[xobjectElement[0].Replace("/", "")] as ImageStructure;

                        if (structure.ColorSpace == "Indexed" && structure.IsTransparent)
                        {
                            return;
                        }

                        Stream fileStream = structure.GetImageStream();
                        if (fileStream == null || fileStream.Length == 0)
                        {
                            structure.IsExceptionThrown = true;
                            return;
                        }
                        structure = null;

#region Save Image to Disk for Testing purpose
                        //byte[] imgArrary = new byte[fileStream.Length];
                        //fileStream.Read(imgArrary, 0, (int)fileStream.Length);
                        //IAsyncOperation<StorageFile> stFile = ApplicationData.Current.LocalFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".png");//await KnownFolders.PicturesLibrary.CreateFileAsync("chck.ttf");
                        //stFile.AsTask().Wait();
                        //StorageFile _stFile = stFile.GetResults();
                        ////StorageFile stFile = await savePicker.PickSaveFileAsync();
                        //Stream Tempstream = new MemoryStream();
                        //Tempstream.Write(imgArrary, 0, imgArrary.Length);
                        //if (stFile != null)
                        //{
                        //    //IRandomAccessStream fileStream1 =  _stFile.OpenAsync(FileAccessMode.ReadWrite);                            
                        //    IAsyncOperation<IRandomAccessStream> fileStream1Task = _stFile.OpenAsync(FileAccessMode.ReadWrite);// ApplicationData.Current.LocalFolder.CreateFileAsync(EmbdFontName);
                        //    fileStream1Task.AsTask().Wait();
                        //    IRandomAccessStream fileStream1 = fileStream1Task.GetResults();
                        //    Stream st = fileStream1.AsStreamForWrite();

                        //    st.Write((Tempstream as MemoryStream).ToArray(), 0, (int)Tempstream.Length);
                        //    st.Flush();
                        //    st.Dispose();
                        //    fileStream1.Dispose();
                        //}
#endregion

                        IRandomAccessStream stream = ImageStructure.ConvertStream(fileStream).Result;
                        var wicFactory = new ImageFactory();
                        fileStream.Position = 0;
                        var bitmapDecoder = wicFactory.CreateBitmapDecoder(stream, DecodeOptions.CacheOnDemand);

                        var formatConverter = wicFactory.CreateFormatConverter();

                        formatConverter.Initialize(
                            bitmapDecoder.GetFrame(0),
                            PixelFormat.Format32bppPRGBA,
                            BitmapDitherType.None,
                            null,
                            0.0,
                            BitmapPaletteType.Custom);

                        Bitmap direct2DBitmap = graphics.CreateBitmapFromWicBitmap(formatConverter);

                        graphics.DrawBitmap(direct2DBitmap, new Rect(0, 0, 1, 1), 1.0f, InterpolationMode.Linear);

                        //direct2DBitmap.Dispose();
                        bitmapDecoder.Dispose();
                        formatConverter.Dispose();
                        wicFactory.Dispose();
                        stream.Dispose();
                        stream = null;
                    }
                    catch
                    {
                        if (structure != null)
                        {
                            structure.IsExceptionThrown = true;
                        }
                        //Image format not supported
                    }
                }
            }
        }
    }
}
#else
namespace Syncfusion.Windows.PdfViewer
{
    class WinRTRenderer
    {
        internal double zoomFactor;
        internal bool IsZoomChanged = false;
        internal PdfLoadedDocument m_lDoc = null;
        internal PdfDocumentView m_documentView = null;
        internal Dictionary<int, List<TextSearchElements>> textSearchCache = new Dictionary<int, List<TextSearchElements>>();
        internal static global::Windows.UI.Xaml.Media.Matrix graphics = new global::Windows.UI.Xaml.Media.Matrix();
        internal List<TargetTextProperties> targetTextInstancesList;
        internal int PageIndex;
        internal string targetText = string.Empty;
        internal List<TextSearchElements> txtSearchList = new List<TextSearchElements>();
        internal List<TextSearchElements> matchTextSearchElemts = new List<TextSearchElements>();
        int textRenderingMode = 0;
        string currentTextRenderingOperator = string.Empty;
        global::Windows.UI.Xaml.Media.Matrix activeTextMatrix = new global::Windows.UI.Xaml.Media.Matrix(1, 0, 0, 1, 0, 0);
        int pushedMatrixCount = 0;
        PdfUnitConvertor m_convertor = new PdfUnitConvertor();
        global::Windows.UI.Xaml.Media.Matrix matrix;
        global::Windows.UI.Xaml.Media.Matrix mat;
        bool IsCommonMatrixUpdated = false;
        bool IsTextMatrixUpdate = false;
        float[] scalingFactor = new float[2];
        internal PdfDocumentPage m_documentPage;
        internal PointF currentTransformLocation = new PointF();
        char[] m_symbolChars = new char[] { '(', ')', '[', ']', '<', '>' };
        char[] m_startText = new char[] { '(', '[', '<', };
        char[] m_endText = new char[] { ')', ']', '>' };
        PdfPageResources m_resources;
        PdfRecordCollection m_mainContentElements;
        private PointF m_currentLocation = PointF.Empty;
        private bool m_beginText;
        private float m_wordSpacing;
        private float m_mitterLength;
        private Stack<global::Windows.UI.Xaml.Media.Matrix> m_matrix = new Stack<global::Windows.UI.Xaml.Media.Matrix>();
        private Stack<TextSearchGraphicObjectData> m_objects = new Stack<TextSearchGraphicObjectData>();
        private Stack<PdfPageResources> m_parentResources = new Stack<PdfPageResources>();
        private float m_textScaling = 100;
        bool textMatrix = false;
        private float m_characterSpacing;
        private bool isNegativeFont = false;
        bool IsTextRotated = false;
        string m_filePath = string.Empty;
        private bool m_isCurrentPositionChanged;
        /// <summary>
        /// Internal variable to hold the initialized pdf document page.
        /// </summary>
        //internal Dictionary<int, PdfDocumentPage> m_initializedDocumentPages = new Dictionary<int, PdfDocumentPage>();

        private float m_changeInHeight, m_pixelHeight;

        private TextSearchGraphicObjectData data = new TextSearchGraphicObjectData();

        private PointF CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
                m_isCurrentPositionChanged = true;
            }
        }

        private string CurrentFont
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.CurrentFont;
                }
                else
                {
                    string tempFontName = "";
                    foreach (TextSearchGraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontName = objectData.CurrentFont;
                        }
                    }
                    return tempFontName;
                }
            }
            set
            {
                Objects.CurrentFont = value;
            }
        }

        private float FontSize
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.FontSize;
                }
                else
                {
                    float tempFontSize = 0;
                    foreach (TextSearchGraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontSize = objectData.FontSize;
                        }
                    }
                    return tempFontSize;
                }
            }
            set
            {
                Objects.FontSize = value;
            }
        }

        private TextSearchGraphicObjectData Objects
        {
            get
            {
                return m_objects.Peek();
            }
        }

        private float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }
            set
            {
                m_wordSpacing = value;
            }
        }

        private float MitterLength
        {
            get
            {
                return m_mitterLength;
            }
            set
            {
                m_mitterLength = value;
            }
        }

        private float TextLeading
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.TextLeading;
                }
                else
                {
                    float tempTextLeading = 0;
                    foreach (TextSearchGraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempTextLeading = objectData.TextLeading;
                        }
                    }
                    return tempTextLeading;
                }
            }
            set
            {
                Objects.TextLeading = value;
            }
        }

        private float TextScaling
        {
            get
            {
                return m_textScaling;
            }
            set
            {
                m_textScaling = value;
            }
        }

        private float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }
            set
            {
                m_characterSpacing = value;
            }
        }

        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="pointer">The reference of the object.</param>
        public WinRTRenderer(double zoom, bool isZoomChanged, PdfDocumentView documentView)
        {
            this.m_documentView = documentView;
            textSearchCache = documentView.textSearchCache;
            this.IsZoomChanged = isZoomChanged;
            this.zoomFactor = zoom;
            TextSearchGraphicObjectData newObject = new TextSearchGraphicObjectData();
            m_objects.Push(newObject);
        }

        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="pointer">The reference of the object.</param>
        public WinRTRenderer(double zoom, bool isZoomChanged, PdfDocumentView documentView, int PageIndex, string TargetText)
        {
            m_documentView = documentView;
            this.IsZoomChanged = isZoomChanged;
            this.zoomFactor = zoom;
            textSearchCache = documentView.textSearchCache;
            m_documentView = documentView;
            TextSearchGraphicObjectData newObject = new TextSearchGraphicObjectData();
            m_objects.Push(newObject);
            this.PageIndex = PageIndex;
            this.targetText = TargetText;
        }

        ~WinRTRenderer()
        {
            m_documentPage = null;
            m_objects.Clear();
            this.m_resources = null;
            this.m_mainContentElements = null;
            m_matrix.Clear();
            m_parentResources.Clear();
        }

#if DEBUG
        internal void Dispose()
        {

        }
#endif
        private Canvas m_docPage;

        internal Canvas docPage
        {
            get
            {
                return m_docPage;
            }
            set
            {
                m_docPage = value;
            }
        }

        void WinRTRenderer_OnRender()
        {
        }

        #region TextSearch Cache
        private static System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(true);
        internal void RenderTextSearchCache(int pageIn)
        {
            if (!textSearchCache.ContainsKey(pageIn))
            {
                try
                {
                    autoEvent.WaitOne();
                    PdfDocumentPage pdfDocPage = null;
                    if (!m_documentView.m_initializedDocumentPages.ContainsKey(pageIn))
                    {
                        if (m_lDoc.PageCount < pageIn)
                            return;
                        PdfPageBase pageToBeRendered = m_lDoc.Pages[pageIn];
                        pdfDocPage = new PdfDocumentPage(pageToBeRendered);
                        pdfDocPage.Initialize(pageToBeRendered, true);
                        m_documentView.m_initializedDocumentPages.Add(pageIn, pdfDocPage);
                    }
                    else
                        pdfDocPage = m_documentView.m_initializedDocumentPages[pageIn];
                    this.m_mainContentElements = pdfDocPage.m_recordCollection;
                    this.m_resources = pdfDocPage.m_resources;

                    double imageWidth = 0;
                    double imageHeight = 0;
                    imageWidth = pdfDocPage.Width;
                    imageHeight = pdfDocPage.Height;
                    m_changeInHeight = (float)imageHeight - pdfDocPage.Height;

                    double aspectRatio;

                    if (imageWidth > 2000 || imageHeight > 2000)
                    {
                        if (imageWidth > imageHeight)
                        {
                            aspectRatio = imageWidth / imageHeight;

                            imageWidth = 2000;
                            imageHeight = 2000 / aspectRatio;
                            zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point);
                            zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                            m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                        }
                        else
                        {
                            aspectRatio = imageWidth / imageHeight;

                            imageHeight = 2000;
                            imageWidth = 2000 * aspectRatio;
                            zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point);
                            zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                            m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                        }
                    }
                    float graphicsSoruce = pageIn % 4;
                    int _offsetx = 0;
                    int _offsety = 0;
                    int _left = _offsetx;
                    int _top = _offsety;
                    int _right = (int)imageWidth;
                    int _bottom = (int)imageHeight;
                    if (m_resources.Resources.ContainsKey("Rotate"))
                    {
                        string rotateAngleString = (m_resources.Resources["Rotate"]).ToString();
                        float rotateAngle = float.Parse(rotateAngleString, CultureInfo.InvariantCulture);
                    }
                    m_pixelHeight = (float)m_convertor.ConvertFromPixels((float)_bottom, PdfGraphicsUnit.Point) - (float)m_convertor.ConvertFromPixels(m_changeInHeight, PdfGraphicsUnit.Point);
                    matrix = new global::Windows.UI.Xaml.Media.Matrix(1.33f, 0, 0, 1.33f, 0, imageHeight);
                    graphics = matrix;
                    RenderContent(m_mainContentElements, graphics);

                    #region TextSearch

                    List<TextSearchElements> existingTexts = new List<TextSearchElements>(txtSearchList);
                    if (!textSearchCache.ContainsKey(pageIn))
                    {
                        textSearchCache.Add(pageIn, existingTexts);
                    }
                    txtSearchList.Clear();
                    #endregion
                }
                catch (Exception)
                {
                }
                finally
                {
                    autoEvent.Set();
                }
            }
        }
        #endregion

        internal void Render(int pageIn)
        {
            if (!textSearchCache.ContainsKey(pageIn))
            {
                try
                {
                    PdfDocumentPage pdfDocPage = null;
                    if (!m_documentView.m_initializedDocumentPages.ContainsKey(pageIn))
                    {
                        if (m_lDoc.PageCount < pageIn)
                            return;
                        PdfPageBase pageToBeRendered = m_lDoc.Pages[pageIn];
                        pdfDocPage = new PdfDocumentPage(pageToBeRendered);
                        pdfDocPage.Initialize(pageToBeRendered, true);
                        m_documentView.m_initializedDocumentPages.Add(pageIn, pdfDocPage);
                    }
                    else
                        pdfDocPage = m_documentView.m_initializedDocumentPages[pageIn];
                    this.m_mainContentElements = pdfDocPage.m_recordCollection;
                    this.m_resources = pdfDocPage.m_resources;

                    double imageWidth = 0;
                    double imageHeight = 0;
                    imageWidth = pdfDocPage.Width;
                    imageHeight = pdfDocPage.Height;
                    m_changeInHeight = (float)imageHeight - pdfDocPage.Height;

                    double aspectRatio;

                    if (imageWidth > 2000 || imageHeight > 2000)
                    {
                        if (imageWidth > imageHeight)
                        {
                            aspectRatio = imageWidth / imageHeight;

                            imageWidth = 2000;
                            imageHeight = 2000 / aspectRatio;
                            zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Width, Pdf.Graphics.PdfGraphicsUnit.Point);
                            zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                            m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                        }
                        else
                        {
                            aspectRatio = imageWidth / imageHeight;

                            imageHeight = 2000;
                            imageWidth = 2000 * aspectRatio;
                            zoomFactor = 2000 / m_convertor.ConvertFromPixels(pdfDocPage.Height, Pdf.Graphics.PdfGraphicsUnit.Point);
                            zoomFactor *= 96 / DisplayProperties.LogicalDpi;
                            m_changeInHeight = (float)imageHeight - pdfDocPage.Height;
                        }
                    }
                    float graphicsSoruce = pageIn % 4;
                    int _offsetx = 0;
                    int _offsety = 0;
                    int _left = _offsetx;
                    int _top = _offsety;
                    int _right = (int)imageWidth;
                    int _bottom = (int)imageHeight;
                    if (m_resources.Resources.ContainsKey("Rotate"))
                    {
                        string rotateAngleString = (m_resources.Resources["Rotate"]).ToString();
                        float rotateAngle = float.Parse(rotateAngleString, CultureInfo.InvariantCulture);
                    }
                    m_pixelHeight = (float)m_convertor.ConvertFromPixels((float)_bottom, PdfGraphicsUnit.Point) - (float)m_convertor.ConvertFromPixels(m_changeInHeight, PdfGraphicsUnit.Point);
                    matrix = new global::Windows.UI.Xaml.Media.Matrix(1.33f, 0, 0, 1.33f, 0, imageHeight);
                    graphics = matrix;
                    RenderContent(m_mainContentElements, graphics);

                    #region TextSearch

                    List<TextSearchElements> existingTexts = txtSearchList;
                    if (!textSearchCache.ContainsKey(pageIn))
                    {
                        textSearchCache.Add(pageIn, existingTexts);
                    }
                    if (existingTexts != null && existingTexts.Count > 0 && targetText != string.Empty)
                    {
                        matchTextSearchElemts.Clear();
                        graphics = new global::Windows.UI.Xaml.Media.Matrix(1, 0, 0, 1, 0, 0);
                        foreach (TextSearchElements item in existingTexts)
                        {
                            TextSearchElements txtSearchElement = item as TextSearchElements;
                            if (txtSearchElement.SearchableText.Contains(targetText) || txtSearchElement.SearchableText.IndexOf(targetText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                matchTextSearchElemts.Add(txtSearchElement);
                            }
                        }
                    }
                    if (matchTextSearchElemts.Count > 0)
                    {
                        targetTextInstancesList = HighLightText(matchTextSearchElemts, pageIn);
                    }

                    #endregion
                }
                catch (Exception)
                {
                }
            }
            else
            {
                #region TextSearch

                List<TextSearchElements> existingTexts = textSearchCache[pageIn];
                if (!textSearchCache.ContainsKey(pageIn))
                {
                    textSearchCache.Add(pageIn, existingTexts);
                }
                if (existingTexts != null && existingTexts.Count > 0 && targetText != string.Empty)
                {
                    matchTextSearchElemts.Clear();
                    graphics = new global::Windows.UI.Xaml.Media.Matrix(1, 0, 0, 1, 0, 0);
                    foreach (TextSearchElements item in existingTexts)
                    {
                        TextSearchElements txtSearchElement = item as TextSearchElements;
                        if (txtSearchElement.SearchableText.Contains(targetText) || txtSearchElement.SearchableText.IndexOf(targetText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchTextSearchElemts.Add(txtSearchElement);
                        }
                    }
                }
                if (matchTextSearchElemts.Count > 0)
                {
                    targetTextInstancesList = HighLightText(matchTextSearchElemts, pageIn);
                }
                else if (targetTextInstancesList != null)
                {
                    targetTextInstancesList.Clear();
                }

                #endregion
            }
        }

        private List<TargetTextProperties> HighLightText(List<TextSearchElements> txtSearchElements, int pageIn)
        {
            bool IsStandardFont = false;
            string tempText = string.Empty;
            List<TargetTextProperties> matchTextsPropertiesList = new List<TargetTextProperties>();
            var pageResource = m_documentView.m_initializedDocumentPages[pageIn].Resources;
            foreach (TextSearchElements txtSearchElement in txtSearchElements)
            {
                string previousText = string.Empty;
                TextElement txtElement = txtSearchElement.TextElements;
                tempText = txtElement.Text;
                int startIndex = txtSearchElement.SearchableText.IndexOf(targetText, StringComparison.OrdinalIgnoreCase);
                if (startIndex >= 0)
                    previousText = txtSearchElement.SearchableText.Substring(0, startIndex);
                string matchText = targetText;
                if (startIndex >= 0)
                {
                    matchText = txtSearchElement.SearchableText.Substring(startIndex, targetText.Length);
                }
                float X, Y, Width = 0, Height;

                if (previousText.Length == 0)
                {
                    txtElement.Text = matchText;
                    X = (txtSearchElement.CurrentLocation.X);
                    Y = txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize;
                    if (txtSearchElement.TextRenderingOperator == "Tj")
                    {
                        Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                    }
                    else if (txtSearchElement.TextRenderingOperator == "TJ")
                    {
                        Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, true, out IsStandardFont, pageResource);
                    }
                    if (IsStandardFont)
                    {
                        Y = (txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize) + (txtElement.textLeading / 4);
                    }
                    Height = txtElement.FontSize;
                    global::Windows.UI.Xaml.Media.Matrix temp = graphics;
                    global::Windows.UI.Xaml.Media.Matrix transformMatrix = new global::Windows.UI.Xaml.Media.Matrix((float)txtSearchElement.TranformPoints.M11, (float)txtSearchElement.TranformPoints.M12, (float)txtSearchElement.TranformPoints.M21, (float)txtSearchElement.TranformPoints.M22, (float)txtSearchElement.TranformPoints.OffsetX, (float)txtSearchElement.TranformPoints.OffsetY);
                    graphics = transformMatrix;
                    TargetTextProperties elements = new TargetTextProperties(transformMatrix, IsStandardFont, X, Y, Width, Height);
                    graphics = temp;
                    matchTextsPropertiesList.Add(elements);
                }
                else
                {
                    txtElement.Text = previousText;
                    float previousTextWidth = 0;
                    if (txtSearchElement.TextRenderingOperator == "Tj")
                    {
                        previousTextWidth = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                    }
                    else
                    {
                        previousTextWidth = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, true, out IsStandardFont, pageResource);
                    }

                    txtElement.Text = matchText;
                    X = txtSearchElement.CurrentLocation.X + previousTextWidth;
                    Y = (txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize);
                    if (IsStandardFont)
                    {
                        Y = (txtSearchElement.CurrentLocation.Y - txtSearchElement.TextElements.FontSize) + (txtElement.textLeading / 4);
                    }
                    Width = txtElement.CalculateTextWidthRenderer(txtSearchElement.CurrentLocation, txtSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                    Height = txtElement.FontSize;
                    global::Windows.UI.Xaml.Media.Matrix temp = graphics;
                    global::Windows.UI.Xaml.Media.Matrix transformMatrix = new global::Windows.UI.Xaml.Media.Matrix((float)txtSearchElement.TranformPoints.M11, (float)txtSearchElement.TranformPoints.M12, (float)txtSearchElement.TranformPoints.M21, (float)txtSearchElement.TranformPoints.M22, (float)txtSearchElement.TranformPoints.OffsetX, (float)txtSearchElement.TranformPoints.OffsetY);
                    graphics = transformMatrix;
                    TargetTextProperties elements = new TargetTextProperties(transformMatrix, IsStandardFont, X, Y, Width, Height);
                    graphics = temp;
                    matchTextsPropertiesList.Add(elements);
                }
                txtElement.Text = tempText;
            }
            return matchTextsPropertiesList;
        }

        private void RenderContent(PdfRecordCollection recordCollection, global::Windows.UI.Xaml.Media.Matrix graphics)//Graphics2D graphics
        {
            PdfRecordCollection m_contentElements = recordCollection;
            try
            {
                if (m_contentElements != null)
                {
                    foreach (PdfRecord record in m_contentElements)
                    {
                        int token = record.OperatorName;
                        string[] element = record.Operands;
                        switch (token)
                        {
                            case 40://q
                                {

                                    mat = matrix;
                                    m_matrix.Push(mat);

                                    data = new TextSearchGraphicObjectData();
                                    m_objects.Push(data);

                                    break;
                                }
                            case 41://"Q"
                                {
                                    data = m_objects.Pop();

                                    //ClipPushCount = 0;

                                    bool layer = false;
                                    for (int j = 0; j < data.LayerCounts; j++)
                                    {

                                        layer = true;
                                    }
                                    mat = m_matrix.Pop();
                                    graphics = matrix;
                                    matrix = mat;
                                    IsTextMatrixUpdate = false;
                                    textMatrix = false;

                                    m_characterSpacing = 0;
                                    textRenderingMode = 0;
                                    break;
                                }
                            case 62://"Tm"
                                {
                                    float a = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    float b = float.Parse(element[1], CultureInfo.InvariantCulture);
                                    float c = float.Parse(element[2], CultureInfo.InvariantCulture);
                                    float d = float.Parse(element[3], CultureInfo.InvariantCulture);
                                    float e = float.Parse(element[4], CultureInfo.InvariantCulture);
                                    float f = float.Parse(element[5], CultureInfo.InvariantCulture);

                                    if (textMatrix)
                                    {

                                        matrix = activeTextMatrix;
                                    }

                                    if (IsTextMatrixUpdate == true)
                                    {

                                        double tempY = ((double)matrix.M22 * -(f));

                                        double tempX = ((e * matrix.M11));

                                        GraphicsMatrix m1 = new GraphicsMatrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        GraphicsMatrix m2 = new GraphicsMatrix(1, 0, 0, 1, tempX, tempY);
                                        GraphicsMatrix m3 = m1 * m2;

                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(m3.M11, m3.M12, m3.M21, m3.M22, m3.OffsetX, m3.OffsetY);
                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(a * matrix.M11, -b, -c, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        graphics = matrix;
                                    }
                                    else
                                    {
                                        if (matrix == new global::Windows.UI.Xaml.Media.Matrix(0, 0, 0, 0, 0, 0))
                                        {
                                            matrix = new global::Windows.UI.Xaml.Media.Matrix(a, b, c, d, e, -f);
                                        }
                                        else
                                        {
                                            activeTextMatrix = matrix;

                                            double tempY = matrix.M22 * (-f);

                                            double tempX = e * matrix.M11;

                                            GraphicsMatrix m1 = new GraphicsMatrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                            GraphicsMatrix m2 = new GraphicsMatrix(1, 0, 0, 1, tempX, tempY);
                                            GraphicsMatrix m3 = m1 * m2;

                                            matrix = new global::Windows.UI.Xaml.Media.Matrix(m3.M11, m3.M12, m3.M21, m3.M22, m3.OffsetX, m3.OffsetY);

                                            matrix = new global::Windows.UI.Xaml.Media.Matrix(a * matrix.M11, -b, -c, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                            if (b < 0 && c < 0)
                                            {
                                                double rotX = -graphics.M11 * matrix.M12;
                                                double rotY = -graphics.M22 * matrix.M21;
                                                matrix = new global::Windows.UI.Xaml.Media.Matrix(matrix.M11, rotX, rotY, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                            }
                                            graphics = matrix;
                                            IsTextMatrixUpdate = true;
                                        }

                                        if (IsCommonMatrixUpdated)
                                        {
                                            graphics = matrix;
                                            CurrentLocation = new PointF(0, 0);
                                        }

                                        CurrentLocation = new PointF(0, 0);
                                    }
                                    CurrentLocation = new PointF(0, 0);
                                    textMatrix = true;
                                    break;
                                }
                            case 10://"cm":
                                {
                                    IsCommonMatrixUpdated = true;
                                    IsTextMatrixUpdate = false;
                                    bool scalingUpdated = false;
                                    ////[a b c d e f]
                                    float a = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    float b = float.Parse(element[1], CultureInfo.InvariantCulture);
                                    float c = float.Parse(element[2], CultureInfo.InvariantCulture);
                                    float d = float.Parse(element[3], CultureInfo.InvariantCulture);
                                    float e = float.Parse(element[4], CultureInfo.InvariantCulture);
                                    float f = float.Parse(element[5], CultureInfo.InvariantCulture);

                                    //Checking the scaling in matrix
                                    if (matrix.M22 != 0 && matrix.M22 != 1)
                                    {
                                        //Scale Y
                                        double tempY = (matrix.M22 * -(f + d));

                                        //Scale X
                                        double tempX = ((e * matrix.M11));

                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX + tempX, tempY + matrix.OffsetY);
                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(a * matrix.M11, 0, 0, d * matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        scalingUpdated = true;
                                    }
                                    else
                                    {
                                        GraphicsMatrix m1 = new GraphicsMatrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        GraphicsMatrix m2 = new GraphicsMatrix(1, 0, 0, 1, e, -f);
                                        GraphicsMatrix m3 = m1 * m2;
                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(m3.M11, m3.M12, m3.M21, m3.M22, m3.OffsetX, m3.OffsetY);

                                    }
                                    graphics = matrix;
                                    if ((a != 0 || d != 0) && (a != 1.0f || d != 1.0f))
                                    {
                                        if (scalingUpdated == false)
                                        {
                                            matrix = new global::Windows.UI.Xaml.Media.Matrix(a, 0, 0, d, graphics.OffsetX, (graphics.OffsetY - d));
                                            graphics = matrix;
                                        }
                                    }
                                    //check for rotate transform
                                    double rad = Math.Acos(a);
                                    double degree = Math.Round((180 / Math.PI) * rad);

                                    double checkRad = Math.Asin(b);
                                    double checkDegree = Math.Round((180 / Math.PI) * checkRad);
                                    if (degree == checkDegree && degree != 0)
                                    {
                                        matrix = new global::Windows.UI.Xaml.Media.Matrix(matrix.M11, -b, -c, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                                        graphics = matrix;
                                    }
                                    break;
                                }
                            case 7://"BT":
                                {
                                    m_beginText = true;
                                    CurrentLocation = PointF.Empty;
                                    m_matrix.Push(matrix);
                                    pushedMatrixCount++;
                                    break;
                                }
                            case 20://"ET":
                                {
                                    CurrentLocation = PointF.Empty;
                                    if (textMatrix)
                                    {

                                        textMatrix = false;
                                    }
                                    matrix = m_matrix.Pop();
                                    pushedMatrixCount--;
                                    graphics = matrix;
                                    break;
                                }
                            case 73://"T*":
                                {
                                    DrawNewLine();
                                    break;
                                }
                            case 60://"TJ":
                                {
                                    RenderTextElementWithSpacing(element, "TJ", graphics);
                                    break;
                                }
                            case 59://"Tj":
                                {
                                    RenderTextElement(element, "Tj", graphics);
                                    break;
                                }
                            case 76://"'":
                                {
                                    DrawNewLine();
                                    RenderTextElement(element, "Tj", graphics);
                                    break;
                                }
                            case 58://"Tf":
                                {
                                    RenderFont(element);
                                    break;
                                }
                            case 57://"TD":
                                {
                                    if (IsTextRotated)
                                    {
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture) * 1000, CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)) * 1000);
                                        TextLeading = -(float.Parse(element[1], CultureInfo.InvariantCulture) * 1000);
                                    }
                                    else
                                    {
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture), CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)));
                                        TextLeading = -(float.Parse(element[1], CultureInfo.InvariantCulture));
                                    }
                                    break;
                                }
                            case 56://"Td":
                                {
                                    if (IsTextRotated)
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture) * 1000, CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture) * 1000));
                                    else
                                        CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0], CultureInfo.InvariantCulture), CurrentLocation.Y - (float.Parse(element[1], CultureInfo.InvariantCulture)));
                                    break;
                                }
                            case 61://"TL":
                                {
                                    TextLeading = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    break;
                                }
                            case 65://"Tw":
                                {
                                    GetWordSpacing(element);
                                    break;
                                }
                            case 55://"Tc":
                                {
                                    GetCharacterSpacing(element);
                                    break;
                                }
                            case 66://"Tz":
                                {
                                    GetScalingFactor(element);
                                    break;
                                }
                            case 16://"Do":
                                {
                                    GetXObject(element, graphics);
                                    break;
                                }

                            case 68://"w":
                                {
                                    m_mitterLength = float.Parse(element[0], CultureInfo.InvariantCulture);
                                    break;
                                }
                            case 53://"f*":
                                {
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            case 22://"f":
                                {
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChageTextElementWidth(string token)
        {
            if (currentTextRenderingOperator == string.Empty)
            {
                currentTextRenderingOperator = token;
            }
        }

        internal List<TextSearchElements> GetAllTextElements(int pageNum)
        {
            if (textSearchCache.ContainsKey(pageNum))
            {
                return textSearchCache[pageNum];
            }
            return null;
        }

        private void RenderTextElement(string[] textElements, string tokenType, global::Windows.UI.Xaml.Media.Matrix graphics)
        {
            ChageTextElementWidth(tokenType);
            string text = string.Join("", textElements);
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;

                TextElement element = new TextElement(text);
                element.FontName = structure.EquivalentFontName;
                text = structure.Decode(text, m_resources.isSameFont());
                element.Text = text;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.FontEncoding = structure.FontEncoding;
                element.isNegativeFont = isNegativeFont;
                //element.FontGlyphWidths = structure.FontGlyphWidths;
                element.fontNameReference = structure.fontNameReference;
                element.CustomFontSize = structure.FontSize;
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                element.CidToGidMap = structure.CidToGidMap;
                element.IsC1 = structure.IsC1;
                element.textRenderingMode = textRenderingMode;
                //element.fontWeight = structure.FontWeight;
                element.fontWeight.Weight = structure.FontWeight;
                element.fontStyle = structure.FontStyle;
                element.textLeading = this.TextLeading;
                structure = null;
                if (element.FontSize < 0)
                {
                    element.FontSize = -element.FontSize;
                    element.isNegativeFont = true;
                }
                else
                {
                    element.isNegativeFont = false;
                }

                if (m_beginText)
                {
                    m_beginText = false;
                }
                var pageResource = m_documentView.m_initializedDocumentPages[PageIndex].Resources;
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    TextSearchElements txtSrchElement = new TextSearchElements(element, matrix, CurrentLocation, PageIndex, element.Text, "Tj", element.Text);
                    txtSearchList.Add(txtSrchElement);
                    element = null;
                    structure = null;
                }
                else
                {
                    TextSearchElements prevTextSearchElement = txtSearchList[txtSearchList.Count - 1];
                    TextElement prevTextElement = prevTextSearchElement.TextElements;
                    bool IsStandardFont;
                    float Width = 0;
                    if (prevTextSearchElement.TextRenderingOperator == "Tj")
                    {
                        Width = prevTextElement.CalculateTextWidthRenderer(prevTextSearchElement.CurrentLocation, prevTextSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                    }
                    else
                    {
                        Width = prevTextElement.CalculateTextWidthRenderer(prevTextSearchElement.CurrentLocation, prevTextSearchElement.TranformPoints, true, out IsStandardFont, pageResource);
                    }
                    PointF textEndPosition = new PointF(CurrentLocation.X + Width, CurrentLocation.Y);
                    TextSearchElements txtSrchElement = new TextSearchElements(element, matrix, textEndPosition, PageIndex, element.Text, "Tj", element.Text);
                    txtSearchList.Add(txtSrchElement);
                    element = null;
                    structure = null;
                }
            }
        }

        private void RenderTextElementWithSpacing(string[] textElements, string tokenType, global::Windows.UI.Xaml.Media.Matrix graphics)
        {
            ChageTextElementWidth(tokenType);
            List<string> decodedList = new List<string>();
            string text = string.Join("", textElements);
            string tempText = text;
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;

                TextElement element = new TextElement(text);
                element.FontName = structure.EquivalentFontName;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                decodedList = structure.DecodeTextTJ(text, m_resources.isSameFont());
                String searchableText = string.Empty;
                string stringWithTextSpace = string.Empty;
                foreach (string txtElement in decodedList)
                {
                    float textSpace;
                    if (float.TryParse(txtElement, out textSpace))
                    {
                        stringWithTextSpace += " " + txtElement + "n ";
                    }
                    else
                    {
                        searchableText += txtElement.Remove(txtElement.Length - 1, 1);
                        stringWithTextSpace += txtElement.Remove(txtElement.Length - 1, 1);
                    }
                }

                element.DecodedList = decodedList;
                element.FontEncoding = structure.FontEncoding;
                //element.FontGlyphWidths = structure.FontGlyphWidths;
                element.fontNameReference = structure.fontNameReference;
                element.CustomFontSize = structure.FontSize;
                element.DefaultWidth = structure.DefaultWidth;
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                element.CidToGidMap = structure.CidToGidMap;
                element.IsC1 = structure.IsC1;
                element.textRenderingMode = textRenderingMode;
                //element.fontWeight = structure.FontWeight;
                element.fontWeight.Weight = structure.FontWeight;
                element.fontStyle = structure.FontStyle;
                element.textLeading = this.TextLeading;
                structure = null;
                if (element.FontSize < 0)
                {
                    element.FontSize = -element.FontSize;
                    element.isNegativeFont = true;
                }
                else
                {
                    element.isNegativeFont = false;
                }

                if (m_beginText)
                {
                    m_beginText = false;
                }
                var pageResource = m_documentView.m_initializedDocumentPages[PageIndex].Resources;
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    TextSearchElements txtSrchElement = new TextSearchElements(element, matrix, CurrentLocation, PageIndex, searchableText, "TJ", stringWithTextSpace);
                    txtSearchList.Add(txtSrchElement);
                    element.Text = searchableText;
                    element = null;
                }
                else
                {
                    TextSearchElements prevTextSearchElement = txtSearchList[txtSearchList.Count - 1];
                    TextElement prevTextElement = prevTextSearchElement.TextElements;
                    bool IsStandardFont;
                    float Width=0;
                    if (prevTextSearchElement.TextRenderingOperator == "Tj")
                    {
                        Width = prevTextElement.CalculateTextWidthRenderer(prevTextSearchElement.CurrentLocation, prevTextSearchElement.TranformPoints, false, out IsStandardFont, pageResource);
                    }
                    else
                    {
                        Width = prevTextElement.CalculateTextWidthRenderer(prevTextSearchElement.CurrentLocation, prevTextSearchElement.TranformPoints, true, out IsStandardFont, pageResource);
                    }
                    PointF textEndPosition = new PointF(CurrentLocation.X + Width, CurrentLocation.Y);
                    TextSearchElements txtSrchElement = new TextSearchElements(element, matrix, textEndPosition, PageIndex, searchableText, "TJ", stringWithTextSpace);
                    txtSearchList.Add(txtSrchElement);
                    element.Text = searchableText;
                    element = null;
                }
            }
        }

        private void DrawNewLine()
        {
            m_isCurrentPositionChanged = true;
            m_currentLocation.Y = (TextLeading + m_currentLocation.Y);//0 -Tl Td
        }

        private void RenderFont(string[] fontElements)
        {
            int i;
            for (i = 0; i < fontElements.Length; i++)
            {
                if (fontElements[i].Contains("/"))
                {
                    CurrentFont = fontElements[i].Replace("/", "");
                    break;
                }
            }
            FontSize = float.Parse(fontElements[i + 1], CultureInfo.InvariantCulture);
        }

        private void GetWordSpacing(string[] spacing)
        {
            m_wordSpacing = float.Parse(spacing[0], CultureInfo.InvariantCulture);
        }

        private void GetCharacterSpacing(string[] spacing)
        {
            m_characterSpacing = float.Parse(spacing[0], CultureInfo.InvariantCulture);
        }

        private void GetScalingFactor(string[] scaling)
        {
            m_textScaling = float.Parse(scaling[0], CultureInfo.InvariantCulture);
            if (m_textScaling < 0)
                m_textScaling = -m_textScaling;
        }

        private void GetXObject(string[] xobjectElement, global::Windows.UI.Xaml.Media.Matrix graphics)
        {
            if (m_resources.ContainsKey(xobjectElement[0].Replace("/", "")))
            {
                if (m_resources[xobjectElement[0].Replace("/", "")] is XObjectElement)
                {
                    pushedMatrixCount = 0;
                    m_parentResources.Push(m_resources);
                    float temp = CharacterSpacing;
                    PdfDocumentPage childObject = (m_resources[xobjectElement[0].Replace("/", "")] as XObjectElement).Render(m_resources, matrix);
                    m_resources = childObject.m_resources;
                    //Applying the internal form matrix transformation
                    matrix = childObject.FormMatrix;
                    graphics = matrix;// new Matrix((float)matrix.M11, (float)matrix.M12, (float)matrix.M21, (float)matrix.M22, (float)matrix.OffsetX, (float)matrix.OffsetY);
                    RenderContent(childObject.m_recordCollection, graphics);
                    m_resources = m_parentResources.Pop();
                    CharacterSpacing = temp;
                    for (int i = 0; i < pushedMatrixCount; i++)
                    {
                        m_matrix.Pop();
                    }
                }
            }
        }
    }

    internal class TextSearchElements
    {
        internal global::Windows.UI.Xaml.Media.Matrix TranformPoints;
        internal PointF CurrentLocation;
        internal int PageIndex;
        internal TextElement TextElements;
        internal string SearchableText;
        internal string TextRenderingOperator;
        internal string StringWithTextSpace;

        internal TextSearchElements(TextElement textElement, global::Windows.UI.Xaml.Media.Matrix transformPoints, PointF currentLocation, int pageindex, string SearchableText, string TextRenderingOperator, string StringWithTextSapce)
        {
            TextElements = textElement;
            TranformPoints = transformPoints;
            CurrentLocation = currentLocation;
            PageIndex = pageindex;
            this.SearchableText = SearchableText;
            this.TextRenderingOperator = TextRenderingOperator;
            this.StringWithTextSpace = StringWithTextSapce;
        }
    }

    internal class TargetTextProperties
    {
        internal global::Windows.UI.Xaml.Media.Matrix matrix;
        internal float X;
        internal float Y;
        internal float width;
        internal float height;
        internal bool IsStandardFont;

        internal TargetTextProperties(global::Windows.UI.Xaml.Media.Matrix mat, bool IsStandardFont, float X, float Y, float Width, float Height)
        {
            this.matrix = mat;
            this.X = X;
            this.Y = Y;
            this.width = Width;
            this.height = Height;
            this.IsStandardFont = IsStandardFont;
        }
    }
}
#endif
