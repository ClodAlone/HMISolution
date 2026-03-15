#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using mshtml;
using Syncfusion.HtmlConverter.Natives;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.HtmlToPdf;
using System.Collections.Generic;






namespace Syncfusion.HtmlConverter
{
    /// <summary>
    /// Class which allows converting Html to the Image.
    /// </summary>
    [ToolboxItem(false)]
    public class HtmlConverter
      : System.Windows.Forms.UserControl
      , IDocHostUIHandler,IDocHostShowUI
      , IOleClientSite
      , Syncfusion.HtmlConverter.Natives.IServiceProvider
      , IAuthenticate
      , IInternetSecurityManager
    {
        #region Internals
        /// <summary>
        /// Loading flags
        /// </summary>
        private enum BrowserNavConstants
        {
            navOpenInNewWindow = 0x1,
            navNoHistory = 0x2,
            navNoReadFromCache = 0x4,
            navNoWriteToCache = 0x8,
            navAllowAutosearch = 0x10,
            navBrowserBar = 0x20,
            navHyperlink = 0x40
        }
        #endregion

        #region Fields
        private string m_username;
        private string m_password;
        private bool m_enableJavaScript = false;
        private bool m_enableActiveXContents;
        private bool m_enableBinaryBehaviors = false;
        private bool m_autoDetectPageBreak = true;
        private bool m_enableHyperlinks = true;
        private int m_additionalDelay = 0;
        private bool m_clearInternetCache = false;
		private Size m_initialBrowserSize;
        private float dx;
        private float dy;
        private IHtmlRenderer m_otherHtmlRenderer;
        Stream m_docStream;
        
        /// <summary>
        /// Internal variable to store webbrowser scroll position.
        /// </summary>
        private float m_startFrom = 0.0f;

        /// <summary>
        /// Internal variable to store current regions.
        /// </summary>
        private int[] currentregions;

        /// <summary>
        /// Internal variable to store size of the web page.
        /// </summary>
        private float m_htmlHeight;

        /// <summary>
        /// Internal variable to store if the specified size should be used. Must be true for Tagged PDF.
        /// </summary>
        private bool m_forceSize = false;
        private bool m_clipScrollBars = false;
        private Size m_controlSize;
        private bool m_isImagePath = false;
        #endregion

        #region Events & Events Args
        internal event ProcessUrlActionEventHandler ProcessUrlAction = null;
        private ProcessUrlActionEventArgs ProcessUrlActionEvent = new ProcessUrlActionEventArgs();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Username
        /// </summary>
        public string Username
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }


        /// <summary>
        /// Gets or Sets the password
        /// </summary>
        public string Password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }
        /// <summary>
        /// Gets or Sets a value indicating whether to Enable/Disable Javascripts in the webpage.
        /// </summary>
        public bool EnableJavaScript
        {
            get
            {
                return m_enableJavaScript;
            }
            set
            {
                m_enableJavaScript = value;
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether to Enable/Disable Javascripts in the webpage.
        /// </summary>
        public bool EnableActiveXContents
        {
            get
            {
                return m_enableActiveXContents;
            }
            set
            {
                m_enableActiveXContents = value;
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether to Enable/Disable binary behaviors in the webpage.
        /// </summary>
        public bool EnableBinaryBehaviors
        {
            get
            {
                return m_enableBinaryBehaviors;
            }
            set
            {

                m_enableBinaryBehaviors = value;
                if (value == true)
                    Native.CoInternetSetFeatureEnabled(INTERNETFEATURELIST.FEATURE_BEHAVIORS, 2, true);
                else
                    Native.CoInternetSetFeatureEnabled(INTERNETFEATURELIST.FEATURE_BEHAVIORS, 2, false);
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether to auto-detect page-break in the webpage.
        /// </summary>
        public bool AutoDetectPageBreak
        {
            get
            {
                return m_autoDetectPageBreak;
            }
            set
            {
                m_autoDetectPageBreak = value;
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether to preserve the live-links in the converted document or not.
        /// </summary>
        public bool EnableHyperlinks
        {
            get
            {
                return m_enableHyperlinks;
            }
            set
            {
                m_enableHyperlinks = value;
            }
        }

        public int AdditionalDelay
        {
            get
            {
                return m_additionalDelay;
            }
            set
            {
                m_additionalDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to clear Internet cache.
        /// </summary>
        public bool ClearInternetCache
        {
            get
            {
                return m_clearInternetCache;
            }
            set
            {
                m_clearInternetCache = value;
            }
        }

        /// <summary>
        /// Gets or sets the scroll position.
        /// </summary>
        private float StartFrom
        {
            get
            {
                return m_startFrom;
            }
            set
            {
                m_startFrom = value;
            }
        }

        /// <summary>
        /// Gets or sets if specified size should be used. Must be true for Tagged PDF.
        /// </summary>
        private bool ForceSize
        {
            get
            {
                return m_forceSize;
            }
            set
            {
                m_forceSize = value;
            }
        }

        private bool ClipScrollBars
        {
            get
            {
                return m_clipScrollBars;
            }
            set
            {
                m_clipScrollBars = value;
            }
        }

        #endregion

        #region Class constants
        /// <summary>
        /// Offset to the width/height.
        /// </summary>
        private const int DEF_OFFSET = 30;
        /// <summary>
        /// Timeout of the thread while document is lodaing.
        /// </summary>
        private const int DEF_TIMEOUT = 50;
        /// <summary>
        /// Blank page.
        /// </summary>
        private const string DEF_BLANK = "about:blank";
        /// <summary>
        /// IDispatch:: Invoke params.
        /// </summary>
        private const int DISPID_AMBIENT_DLCONTROL = -5512;
        /// <summary>
        /// Metafile Execute parameters.
        /// </summary>
        private const INVOKE_PARAMS DEF_META_EXEC_PARAMS =
          INVOKE_PARAMS.DLCTL_DLIMAGES |
          INVOKE_PARAMS.DLCTL_NO_JAVA |
          INVOKE_PARAMS.DLCTL_SILENT |
          INVOKE_PARAMS.DLCTL_NO_DLACTIVEXCTLS |
          INVOKE_PARAMS.DLCTL_NO_RUNACTIVEXCTLS |
          INVOKE_PARAMS.DLCTL_NO_SCRIPTS;
        /// <summary>
        /// Bitmap Execute parameters.
        /// </summary>
        private const INVOKE_PARAMS DEF_BITMAP_EXEC_PARAMS =
          INVOKE_PARAMS.DLCTL_DLIMAGES |
          INVOKE_PARAMS.DLCTL_SILENT |
          INVOKE_PARAMS.DLCTL_NO_SCRIPTS;

        private static Guid IID_IAuthenticate = new Guid("79eac9d0-baf9-11ce-8c82-00aa004ba90b");
        private const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
        private const int S_OK = unchecked((int)0x00000000);

        private const string DEF_REGEX_URL_PATTERN = @"(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?";
        private const string DEF_REGEX_HEADTAG_PATTERN = @"(?<HEAD_TAG_GROUP>\<\s*HEAD\s*[^\>]*\>)";

        private DOCHOSTUIFLAG DEF_DOCHOSTUI_FLAG =
            DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO |
            DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DBORDER |
            DOCHOSTUIFLAG.DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE;

        private const DOCHOSTUIDBLCLK DEF_DOCHOSTUIDBLCLKFLAG = DOCHOSTUIDBLCLK.DOCHOSTUIDBLCLK_DEFAULT;
        #endregion

        #region Class members
        /// <summary>
        /// Indicates whether loading of the document is completed.
        /// </summary>
        private bool m_DocComplete;
        /// <summary>
        /// Url to the Html resource.
        /// </summary>
        private string m_url;
        /// <summary>
        /// Type of the image.
        /// </summary>
        private ImageType m_type;
        /// <summary>
        /// Indicates whether we should stop navigation.
        /// </summary>
        private bool m_bCancelNavigate;
        /// <summary>
        /// String containing html data.
        /// </summary>
        private string m_html;
        /// <summary>
        /// Original size of the image.
        /// </summary>
        private Size m_size;
        /// <summary>
        /// The aspect ratio of the image.
        /// </summary>
        private AspectRatio m_aspectRatio;
		/// <summary>
        /// Checks whether link wrapped
        /// </summary>
		private bool isLinkWrapped = false;
        private bool isCustomHeight = false;

        #endregion

        #region Form controls
        private Container components = null;
        private AxWebBrowser m_webBrowser;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates new object.
        /// </summary>
        public HtmlConverter()
        {
            InitializeControl();
            using (WebBrowser browser = new WebBrowser())
            {
                if (browser.Version.Major >= 10)
                    ClipScrollBars = true;
                else
                    ClipScrollBars = false;

                if (browser != null)
                    browser.Dispose();
            }
        }

        /// <summary>
        /// Disposes component.
        /// </summary>
        /// <param name="disposing">Indicates whether dispose managed resources or not.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }

            if (m_webBrowser != null && !m_webBrowser.IsDisposed)
            {
                DisposeBrowser();
            }

            base.Dispose(disposing);

            Application.DoEvents();
        }
        #endregion

        #region Class Public Methods

        private void InitializeControl()
        {
            m_webBrowser = new AxWebBrowser();



            m_webBrowser.BeginInit();
            SuspendLayout();

            m_webBrowser.Enabled = true;
            m_webBrowser.Location = new System.Drawing.Point(0, 0);
            Size initSize = m_webBrowser.Size;
            //m_webBrowser.Size = new Size( 100, 100 );
            m_webBrowser.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentComplete);
            m_webBrowser.NewWindow2 += new DWebBrowserEvents2_NewWindow2EventHandler(NewWindow);
            m_webBrowser.NavigateError += new DWebBrowserEvents2_NavigateErrorEventHandler(NavigateError);
            Controls.Add(m_webBrowser);



            Size = m_webBrowser.Size;
            VScroll = true;
            HScroll = true;

            m_webBrowser.EndInit();
            m_webBrowser.Size = initSize;
            m_initialBrowserSize = initSize;
            ResumeLayout(false);

            // Register host.
            SetUIHandler();

            // Disables the binary behavior
            EnableBinaryBehaviors = false;
            ClipScrollBars = false;
            m_controlSize = Size.Empty;
            m_isImagePath = false;
        }

        public HtmlConverter(IHtmlRenderer renderer)
            : this()
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            m_otherHtmlRenderer = renderer;
        }

        private void DisposeBrowser()
        {
            try
            {
                if (m_webBrowser.Busy)
                {
                    m_webBrowser.Stop();
                }
                m_webBrowser.DocumentComplete -= new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                m_webBrowser.NewWindow2 -= new DWebBrowserEvents2_NewWindow2EventHandler(NewWindow);
                m_webBrowser.NavigateError -= new DWebBrowserEvents2_NavigateErrorEventHandler(NavigateError);

                // Release interface.
                IntPtr iUnknown = Marshal.GetIUnknownForObject(m_webBrowser);
                if (iUnknown != IntPtr.Zero)
                {
                    Marshal.Release(iUnknown);
                }
                m_webBrowser.Dispose();
                m_webBrowser = null;
                m_DocComplete = true;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>Image height can be greater than <see cref="height"/> value.</remarks>
        public Image ConvertToImage(string url, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_aspectRatio = aspectRatio;

            return Convert(url, type, width, height, aspectRatio).RenderedImage;
        }

        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>Image height can be greater than <see cref="height"/> value.</remarks>

        public Image ConvertToImage(string url, ImageType type, int width, int height, AspectRatio aspectRatio, string username, string password)
        {
            m_aspectRatio = aspectRatio;
            m_username = username;
            m_password = password;

            return ConvertToImage(url, type, width, height);
        }


        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>Image height can be greater than <see cref="height"/> value.</remarks>
        public Image ConvertToImage(string url, ImageType type, int width, int height)
        {
            SetHeight(height);

            return ConvertToImage(url, type, width);
        }
        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>Image height can be greater than <see cref="height"/> value.</remarks>
        public Image ConvertToImage(string url, ImageType type, int width, int height, string username, string password)
        {
            SetHeight(height);
            m_username = username;
            m_password = password;
            return ConvertToImage(url, type, width);
        }

        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image ConvertToImage(string url, ImageType type, int width)
        {
            SetWidth(width);

            return ConvertToImage(url, type);
        }
        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image ConvertToImage(string url, ImageType type, int width, string username, string password)
        {
            SetWidth(width);
            m_username = username;
            m_password = password;
            return ConvertToImage(url, type);
        }
        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image ConvertToImage(string url, ImageType type)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (url.Length == 0)
                throw new ArgumentException("url - string can not be empty");


            m_url = url;
            m_type = type;
            m_DocComplete = false;
            m_bCancelNavigate = false;
            m_html = null;

            return ToImage();
        }

        /// <summary>
        /// Calculates the offsets.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private List<int[]> CalculateOffsets(Size size)
        {
            int maxHeight = m_forceSize ? m_size.Height : Int16.MaxValue;
            int pageHeight = size.Height;
            int pos = 0;
            List<int[]> offsets = new List<int[]>();

            if (size.Height < maxHeight)
            {
                offsets.Add(new int[] { 0, size.Height });
            }
            else
            {
                offsets.Add(new int[] { 0, maxHeight });
                do
                {
                    pageHeight -= maxHeight;
                    pos += maxHeight;
                    offsets.Add(new int[] { pos, (pageHeight > maxHeight) ? maxHeight :pageHeight});
                } while (pageHeight > maxHeight);
            }
            return offsets;
        }

        /// <summary>
        /// Gets the images from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal Image[] GetImagesFromUrl(string url, ImageType type)
        {
            try
            {
                Image img = Image.FromFile(url);
                m_isImagePath = true;
                img.Dispose();
            }
            catch (Exception)
            {

            }
            if (m_otherHtmlRenderer != null)
            {
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                double widthInInches = convertor.ConvertUnits(m_size.Width, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Inch);
                double heightInInches = convertor.ConvertUnits(m_size.Height, PdfGraphicsUnit.Pixel, PdfGraphicsUnit.Inch);

                if (!string.IsNullOrEmpty(m_username) || !string.IsNullOrEmpty(m_password))
                {
                    m_docStream = m_otherHtmlRenderer.GetDocumentImageStream(url, widthInInches, heightInInches, m_username, m_password);
                }
                else
                {
                    m_docStream = m_otherHtmlRenderer.GetDocumentImageStream(url, widthInInches, heightInInches);
                }
                return null;
            }
            
            if (url == null)
                throw new ArgumentNullException("url");
            if (url.Length == 0)
                throw new ArgumentException("url - string can not be empty");

            List<Image> images = new List<Image>();

            m_url = url;
            m_type = type;
            m_DocComplete = false;
            m_bCancelNavigate = false;
            m_html = null;

            InitializeBrowser();
            m_controlSize = ResizeBrowser3();
            if (!m_isImagePath)
                CheckHeight(ref m_controlSize);

            List<int[]> offSet = CalculateOffsets(m_controlSize);

            m_htmlHeight = m_controlSize.Height;

            // Generates one metafile at a time and sends to PDF. Based on the layouting results resumes with next metafile.
            if (m_forceSize)
            {
                int[] region = offSet[0];
                int scrollto = 0;

                if (m_startFrom > 0.0f) // can be first metafile or layouting is over for the last generated metafile.
                {
                    PdfUnitConvertor convertor = new PdfUnitConvertor();
                    scrollto = (int)convertor.ConvertToPixels(m_startFrom, PdfGraphicsUnit.Point);

                    scrollto += currentregions[0]; // Scrolls to the correct position.
                }
                else
                {
                    if (currentregions != null)
                    {
                        scrollto = currentregions[0] + currentregions[1];
                    }
                }
                region[1] = (scrollto + offSet[0][1]) > m_controlSize.Height ? (m_controlSize.Height - scrollto) : offSet[0][1];

                region[0] = scrollto;

                m_webBrowser.Height = region[1];

                (m_webBrowser.Document as IHTMLDocument2).parentWindow.scrollTo(0, region[0]);

                IHTMLElement2 element = GetHtmlBody();
                if (element.scrollTop != region[0])
                {
                    (m_webBrowser.Document as IHTMLDocument2).parentWindow.scrollTo(0, region[0]);
                }

                Image mf = ToImage(new Size(m_controlSize.Width, region[1]));
                images.Add(mf);

                currentregions = region;
            }
            // Go to old conversion of generating multiple images.
            else
            {
                foreach (int[] region in offSet)
                {
                    m_webBrowser.Height = region[1];

                    (m_webBrowser.Document as IHTMLDocument2).parentWindow.scrollTo(0, region[0]);

                    IHTMLElement2 element = GetHtmlBody();
                    if (element != null && element.scrollTop != region[0])
                    {
                        (m_webBrowser.Document as IHTMLDocument2).parentWindow.scrollTo(0, region[0]);
                    }

                    Image mf = ToImage(new Size(m_controlSize.Width, region[1]));
                    images.Add(mf);

                    if (offSet.IndexOf(region) != offSet.Count - 1)
                        InitializeBrowser();
                }
            }

            return images.ToArray();
        }

        private void CheckHeight(ref Size sz)
        {
            // Add dummy hyperlink to validate height.
            IHTMLDocument2 document = GetDocument();
            if (document != null)
            {
                string oriString = document.body.innerHTML;
                string checkhref = Guid.NewGuid().ToString();
                string check = string.Format("<p><a href=\"{0}\"></a></p>", checkhref);

                if (document.body.innerHTML != null)
                {
                    int len = document.body.innerHTML.Length;
                    document.body.innerHTML = document.body.innerHTML.Insert(len, check);
                }
                else
                    return;

                IHTMLElementCollection elements = document.links;
                for (int i = elements.length; i > -1; i--)
                {
                    try
                    {
                        RectangleF linkBounds = new RectangleF();
                        object obj = elements.item(i, i);

                        if (obj is IHTMLAnchorElement)
                        {
                            IHTMLAnchorElement element = obj as IHTMLAnchorElement;
                            IHTMLElement anchorElement = obj as IHTMLElement;
                            string href = Path.GetFileNameWithoutExtension(element.href);
                            if (checkhref == href)
                            {
                                linkBounds = GetBounds(anchorElement, false);
                                if (sz.Height < linkBounds.Y)
                                    sz.Height = (int)linkBounds.Y;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Converts Html by the url to the image.
        /// </summary>
        /// <param name="url">Path to the Html resource.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image ConvertToImage(string url, ImageType type, string username, string password)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (url.Length == 0)
                throw new ArgumentException("url - string can not be empty");

            m_url = url;
            m_type = type;
            m_DocComplete = false;
            m_bCancelNavigate = false;
            m_html = null;

            m_username = username;
            m_password = password;

            return ToImage();
        }
        /// <summary>
        /// Converts Html data contained in the stream to the image.
        /// </summary>
        /// <param name="stream">Stream containing Html data.</param>
        /// <param name="encoding">Encoding used for reading data from the stream.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>If the stream can seek, the position will be set to 0 before data reading.</remarks>
        public Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_aspectRatio = aspectRatio;

            return ConvertToImage(stream, encoding, type, width, height);
        }
        /// <summary>
        /// Converts Html data contained in the stream to the image.
        /// </summary>
        /// <param name="stream">Stream containing Html data.</param>
        /// <param name="encoding">Encoding used for reading data from the stream.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <param name="height">Preffered height of the image in pixels.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>If the stream can seek, the position will be set to 0 before data reading.</remarks>
        public Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width, int height)
        {
            SetHeight(height);

            return ConvertToImage(stream, encoding, type, width);
        }
        /// <summary>
        /// Converts Html data contained in the stream to the image.
        /// </summary>
        /// <param name="stream">Stream containing Html data.</param>
        /// <param name="encoding">Encoding used for reading data from the stream.</param>
        /// <param name="type">Type of the output image.</param>
        /// <param name="width">Preffered width of the image in pixels.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>If the stream can seek, the position will be set to 0 before data reading.</remarks>
        public Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width)
        {
            SetWidth(width);

            return ConvertToImage(stream, encoding, type);
        }
        /// <summary>
        /// Converts Html data contained in the stream to the image.
        /// </summary>
        /// <param name="stream">Stream containing Html data.</param>
        /// <param name="encoding">Encoding used for reading data from the stream.</param>
        /// <param name="type">Type of the output image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        /// <remarks>If the stream can seek, the position will be set to 0 before data reading.</remarks>
        public Image ConvertToImage(Stream stream, Encoding encoding, ImageType type)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            long pos = stream.Position;
            Image img = null;

            try
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                long length = stream.Length - stream.Position;
                byte[] buff = new byte[length];
                stream.Read(buff, 0, buff.Length);
                string html = encoding.GetString(buff);
                img = FromString(html, type);

            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = pos;
                }
            }

            return img;
        }

        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="baseUrl">Used to retrieve images, scripts and stylesheets.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <param name="height">Preffered height of the image.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, string baseUrl, ImageType type, int width, int height, AspectRatio aspectRatio, string username, string password)
        {
            m_aspectRatio = aspectRatio;
            m_username = username;
            m_password = password;

            return FromString(html, baseUrl, type, width, height);
        }

        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="baseUrl">Used to retrieve images, scripts and stylesheets.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <param name="height">Preffered height of the image.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, string baseUrl, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_aspectRatio = aspectRatio;

            return FromString(html, baseUrl, type, width, height);
        }

        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="baseUrl">Used to retrieve images, scripts and stylesheets.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <param name="height">Preffered height of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, string baseUrl, ImageType type, int width, int height)
        {
            SetHeight(height);

            return FromString(html, baseUrl, type, width);
        }
        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="baseUrl">Used to retrieve images, scripts and stylesheets.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, string baseUrl, ImageType type, int width)
        {
            SetWidth(width);

            return FromString(html, baseUrl, type);
        }
        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="baseUrl">Used to retrieve images, scripts and stylesheets.</param>
        /// <param name="type">Type of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, string baseUrl, ImageType type)
        {
            if ((m_webBrowser == null) || (html.Length == 0))
                return null;

            if (!Regex.IsMatch(baseUrl, DEF_REGEX_URL_PATTERN))
                baseUrl = "about:blank";


            Match match = Regex.Match(html, DEF_REGEX_HEADTAG_PATTERN, RegexOptions.IgnoreCase);
            if (match != null && !string.IsNullOrEmpty(baseUrl.Trim()))
            {
                Group group = match.Groups["HEAD_TAG_GROUP"];
                html = html.Insert(group.Index + group.Length, string.Format("<BASE HREF=\"{0}\">", baseUrl));
            }


            m_url = DEF_BLANK;
            m_type = type;
            m_DocComplete = false;
            m_bCancelNavigate = false;
            m_html = html;

            if (!Created)
            {
                CreateControl();
            }


            // Make sure the ie control is ready, and if not wait for it to be ready
            while (!m_webBrowser.Created)
            {
                m_webBrowser.CreateControl();
                Application.DoEvents();
            }

            // Dsable user interaction such a message boxes, etc.
            if (!m_webBrowser.Silent)
            {
                m_webBrowser.Silent = true;
            }

            object obj = m_webBrowser.GetOcx();
            IOleObject oc = obj as IOleObject;
            oc.SetClientSite(this);

            object flags = BrowserNavConstants.navNoHistory |
              BrowserNavConstants.navNoReadFromCache |
              BrowserNavConstants.navNoWriteToCache;
            object obj2 = null;
            object obj3 = null;
            object obj4 = null;

            m_webBrowser.Navigate(m_url, ref flags, ref obj2, ref obj3, ref obj4);

            // Wait for the document to load
            while (!m_DocComplete && !m_bCancelNavigate)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

            // If someone disposed this object during document loading or error occured - exit.
            if (IsDisposed || m_bCancelNavigate)
            {
                return null;
            }

            // Wait for the DOM to be accessible
            while (GetHtmlBody() == null)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

            // Load html from the string.
            if (m_url == DEF_BLANK && m_html != null)
            {
                IHTMLDocument2 document = GetDocument();
                document.write(m_html);
            }

            // Stop any other loading.
            if (m_webBrowser.Busy)
            {
                m_webBrowser.Stop();
            }


            Image result = null;
            string tempFileName = Path.GetTempFileName();
            string destFileName = tempFileName + ".html";

            //Temp Fix : Investigate further and implement custom moniker.
            try
            {
                File.Move(tempFileName, destFileName);
            }
            catch (Exception exception)
            {
                File.Delete(tempFileName);
                throw new Exception("Conversion Failed.");
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(destFileName, false, Encoding.UTF8))
                {
                    writer.Write(html);
                }
                result = ConvertToImage(destFileName, m_type, m_size.Width, m_size.Height, m_aspectRatio);

            }
            catch (Exception exception2)
            {
                throw new Exception("Conversion Failed.");
            }
            finally
            {
                File.Delete(destFileName);
            }


            //IPersistMoniker pPM = m_webBrowser.Document as IPersistMoniker;

            //if (pPM == null)
            //    return null;
            //System.Runtime.InteropServices.ComTypes.IBindCtx bindctx = null;
            //WinApis.CreateBindCtx((uint)0, out bindctx);
            //if (bindctx == null)
            //    return null;
            //LoadHTMLMoniker loader = new LoadHTMLMoniker();
            //if (string.IsNullOrEmpty(baseUrl))
            //    baseUrl = m_webBrowser.LocationURL;
            //loader.InitLoader(html, baseUrl);
            //pPM.Load(1, loader, bindctx, WinApis.STGM_READ);



            return result;
        }

        private HtmlToPdfResult GetImagesFromString(string html, string baseUrl, ImageType type, bool flag)
        {
            if ((m_webBrowser == null) || (html.Length == 0))
                return null;

            if (!Regex.IsMatch(baseUrl, DEF_REGEX_URL_PATTERN))
                baseUrl = "about:blank";


            Match match = Regex.Match(html, DEF_REGEX_HEADTAG_PATTERN, RegexOptions.IgnoreCase);
            if (match != null && !string.IsNullOrEmpty(baseUrl.Trim()))
            {
                Group group = match.Groups["HEAD_TAG_GROUP"];
                html = html.Insert(group.Index + group.Length, string.Format("<BASE HREF=\"{0}\">", baseUrl));
            }

            m_url = DEF_BLANK;
            m_type = type;
            m_html = html;

            if (!m_enableJavaScript)
            {
                m_DocComplete = false;
                m_bCancelNavigate = false;

                if (!Created)
                {
                    CreateControl();
                }


                // Make sure the ie control is ready, and if not wait for it to be ready
                while (!m_webBrowser.Created)
                {
                    m_webBrowser.CreateControl();
                    Application.DoEvents();
                }

                // Dsable user interaction such a message boxes, etc.
                if (!m_webBrowser.Silent)
                {
                    m_webBrowser.Silent = true;
                }

                object obj = m_webBrowser.GetOcx();
                IOleObject oc = obj as IOleObject;
                oc.SetClientSite(this);

                object flags = BrowserNavConstants.navNoHistory |
                  BrowserNavConstants.navNoReadFromCache |
                  BrowserNavConstants.navNoWriteToCache;
                object obj2 = null;
                object obj3 = null;
                object obj4 = null;

                m_webBrowser.Navigate(m_url, ref flags, ref obj2, ref obj3, ref obj4);

                // Wait for the document to load
                while (!m_DocComplete && !m_bCancelNavigate)
                {
                    Thread.Sleep(DEF_TIMEOUT);
                    Application.DoEvents();
                }

                // If someone disposed this object during document loading or error occured - exit.
                if (IsDisposed || m_bCancelNavigate)
                {
                    return null;
                }

                // Wait for the DOM to be accessible
                while (GetHtmlBody() == null)
                {
                    Thread.Sleep(DEF_TIMEOUT);
                    Application.DoEvents();
                }

                // Load html from the string.
                if (m_url == DEF_BLANK && m_html != null)
                {
                    IHTMLDocument2 document = GetDocument();
                    document.write(m_html);
                }

                // Stop any other loading.
                if (m_webBrowser.Busy)
                {
                    m_webBrowser.Stop();
                }
            }

            string tempFileName = Path.GetTempFileName();
            string destFileName = tempFileName + ".html";

            //Temp Fix : Investigate further and implement custom moniker.
            try
            {
                File.Move(tempFileName, destFileName);
            }
            catch (Exception exception)
            {
                File.Delete(tempFileName);
                throw new Exception("Conversion Failed.");
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(destFileName, false, Encoding.UTF8))
                {
                    writer.Write(html);
                }
                 return Convert(destFileName, m_type, m_size.Width, m_size.Height, m_aspectRatio);

            }
            catch (Exception exception2)
            {
                throw new Exception("Conversion Failed.");
            }
            finally
            {
                File.Delete(destFileName);
            }


            //IPersistMoniker pPM = m_webBrowser.Document as IPersistMoniker;

            //if (pPM == null)
            //    return null;
            //System.Runtime.InteropServices.ComTypes.IBindCtx bindctx = null;
            //WinApis.CreateBindCtx((uint)0, out bindctx);
            //if (bindctx == null)
            //    return null;
            //LoadHTMLMoniker loader = new LoadHTMLMoniker();
            //if (string.IsNullOrEmpty(baseUrl))
            //    baseUrl = m_webBrowser.LocationURL;
            //loader.InitLoader(html, baseUrl);
            //pPM.Load(1, loader, bindctx, WinApis.STGM_READ);



            return null;
        }

        public Image[] GetImagesFromString(string html, string baseUrl, ImageType type)
        {
            return GetImagesFromString(html, baseUrl, type, false).Images;
        }

        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <param name="height">Preffered height of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, ImageType type, int width, int height)
        {
            SetHeight(height);

            return FromString(html, type, width);
        }
        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <param name="height">Preffered height of the image.</param>
        /// <param name="aspectRatio">Aspect ratio of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_aspectRatio = aspectRatio;

            return FromString(html, type, width, height);
        }
        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <param name="width">Preffered width of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        public Image FromString(string html, ImageType type, int width)
        {
            SetWidth(width);

            return FromString(html, type);
        }

        /// <summary>
        /// Renders html from the string to the image.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="type">Type of the image.</param>
        /// <returns>Image, containing data from the Html.</returns>
        private Image FromString(string html, ImageType type)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            m_url = DEF_BLANK;
            m_type = type;
            m_DocComplete = false;
            m_bCancelNavigate = false;
            m_html = html;

            Image[] imgs = GetImagesFromString(html, "", type);

            return (imgs.Length > 0) ? imgs[0] : null;
        }



        #endregion

        #region Class utility methods
        /// <summary>
        /// Converts the url as Tagged PDF.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public PdfLayoutResult ConvertToTaggedPDF(Syncfusion.Pdf.PdfDocument document, string url, string userName, string password)
        {
            PdfLayoutResult layoutResult = null;

            if (document != null)
            {
                document.FileStructure.TaggedPdf = true;
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                Syncfusion.Pdf.PdfPage page = document.Pages.Add();
                int width = (int)convertor.ConvertToPixels(page.GetClientSize().Width, PdfGraphicsUnit.Point);
                int height = (int)convertor.ConvertToPixels(page.GetClientSize().Height, PdfGraphicsUnit.Point);

                // Layout format for Metafile.
                PdfMetafileLayoutFormat metafileFormat = new PdfMetafileLayoutFormat();
                metafileFormat.Break = PdfLayoutBreakType.FitPage;
                metafileFormat.Layout = PdfLayoutType.Paginate;

                EnableHyperlinks = false; // Hyperlinks are not supported in Tagged PDF.
                ForceSize = true;

                HtmlToPdfResult result = null;
                
                float pageHeight = 0.0f;

                if (userName == string.Empty && password == string.Empty)
                    result = Convert(url, ImageType.Metafile, width, height, AspectRatio.KeepWidth);
                else
                    result = Convert(url, ImageType.Metafile, width, height, AspectRatio.KeepWidth, userName, password);

                while (true)
                {
                    result.Render(page, metafileFormat);

                    if (result.Completed)
                        break;

                    page = document.Pages.Add();
                    m_startFrom = result.Height;
                    pageHeight += m_startFrom;

                    if (userName == string.Empty && password == string.Empty)
                        result = Convert(url, ImageType.Metafile, width, height, AspectRatio.KeepWidth);
                    else
                        result = Convert(url, ImageType.Metafile, width, height, AspectRatio.KeepWidth, userName, password);
                }

                if (m_htmlHeight < height)
                    pageHeight = convertor.ConvertFromPixels(m_htmlHeight, PdfGraphicsUnit.Point);
                else
                    pageHeight = convertor.ConvertFromPixels(m_htmlHeight, PdfGraphicsUnit.Point) - pageHeight;
                
                RectangleF tempRectangle = new RectangleF(PointF.Empty, new SizeF(result.LayoutResult.Bounds.Width, pageHeight));
                layoutResult = new PdfLayoutResult(result.LayoutResult.Page, tempRectangle);
            }

            return layoutResult;
        }

        /// <summary>
        /// Converts the url as Tagged PDF.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="url"></param>
        public PdfLayoutResult ConvertToTaggedPDF(Syncfusion.Pdf.PdfDocument document, string url)
        {
            return ConvertToTaggedPDF(document, url, string.Empty, string.Empty);
        }

        /// <summary>
        /// Converts the html string as Tagged PDF.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="html"></param>
        /// <param name="baseURL"></param>
        public PdfLayoutResult ConvertToTaggedPDF(Syncfusion.Pdf.PdfDocument document, string html, string baseURL)
        {
            PdfLayoutResult layoutResult = null;

            if (document != null)
            {
                document.FileStructure.TaggedPdf = true;
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                Syncfusion.Pdf.PdfPage page = document.Pages.Add();
                int width = (int)convertor.ConvertToPixels(page.GetClientSize().Width, PdfGraphicsUnit.Point);
                int height = (int)convertor.ConvertToPixels(page.GetClientSize().Height, PdfGraphicsUnit.Point);

                // Layout format for Metafile.
                PdfMetafileLayoutFormat metafileFormat = new PdfMetafileLayoutFormat();
                metafileFormat.Break = PdfLayoutBreakType.FitPage;
                metafileFormat.Layout = PdfLayoutType.Paginate;

                EnableHyperlinks = false; //Hyperlinks are not supported in Tagged PDF
                ForceSize = true;

                float pageHeight = 0.0f;

                HtmlToPdfResult result = Convert(html, string.Empty, ImageType.Metafile, width, height, AspectRatio.KeepWidth);

                while (true)
                {
                    result.Render(page, metafileFormat);
                    if (result.Completed)
                        break;

                    page = document.Pages.Add();
                    m_startFrom = result.Height;
                    pageHeight += m_startFrom;

                    result = Convert(html, string.Empty, ImageType.Metafile, width, height, AspectRatio.KeepWidth);
                }

                if (m_htmlHeight < height)
                    pageHeight = convertor.ConvertFromPixels(m_htmlHeight, PdfGraphicsUnit.Point);
                else
                    pageHeight = convertor.ConvertFromPixels(m_htmlHeight, PdfGraphicsUnit.Point) - pageHeight;

                RectangleF tempRectangle = new RectangleF(PointF.Empty, new SizeF(result.LayoutResult.Bounds.Width, pageHeight));
                layoutResult = new PdfLayoutResult(result.LayoutResult.Page, tempRectangle);
            }

            return layoutResult;
        }

        /// <summary>
        /// Converts html to image.
        /// </summary>
        /// <returns>Converted image.</returns>
        private Image ConvertToImage()
        {
            Image result = null;

            while (m_webBrowser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

            if (m_type == ImageType.Bitmap)
            {
                result = ConvertToBitmap2();
            }
            else
            {
                result = ConvertToMetafile2();
            }

            return result;
        }

        /// <summary>
        /// Converts html to HtmlToPdfElement result.
        /// </summary>
        /// <returns>Converted image.</returns>
        public Syncfusion.Pdf.HtmlToPdf.HtmlToPdfResult Convert(string url, ImageType type, int width, int height, AspectRatio aspectRatio, string username, string password)
        {
            m_url = url;
            m_size = new Size(width, height);
            if (height != -1)
                isCustomHeight = true;
            m_username = username;
            m_password = password;
            m_aspectRatio = aspectRatio;
            m_webBrowser.Size = m_initialBrowserSize;
            SetHeight(height);
            SetWidth(width);

            HtmlToPdfResult convertedResult = null;
            Image[] result = null;

            result = GetImagesFromUrl(url, type);
            convertedResult = Convert(result);
            convertedResult.IsImagePath = m_isImagePath;
            return convertedResult;
        }

        /// <summary>
        /// Converts html to HtmlToPdfElement result.
        /// </summary>
        /// <returns>Converted image.</returns>
        public Syncfusion.Pdf.HtmlToPdf.HtmlToPdfResult Convert(string url, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_url = url;
            m_size = new Size(width, height);
            if (height != -1)
                isCustomHeight = true;
            m_aspectRatio = aspectRatio;
            m_webBrowser.Size = m_initialBrowserSize;
            SetHeight(height);
            SetWidth(width);
            HtmlToPdfResult convertedResult = null;
            Image[] result = null;

            result = GetImagesFromUrl(url, type);
            if (result != null)
            {
                convertedResult = Convert(result);
                convertedResult.IsImagePath = m_isImagePath;
            }

            if (result == null && m_docStream != null)
            {
                HtmlToPdfResult pdfResult = new HtmlToPdfResult(m_docStream);
                pdfResult.IsImagePath = m_isImagePath;
                return pdfResult;
            }
            else
                return convertedResult;
        }

        /// <summary>
        /// Converts the given html string to HtmlToPdfResult.
        /// </summary>
        /// <returns>Converted image.</returns>
        public Syncfusion.Pdf.HtmlToPdf.HtmlToPdfResult Convert(string html, string baseurl, ImageType type, int width, int height, AspectRatio aspectRatio)
        {
            m_size = new Size(width, height);
            if (height != -1)
                isCustomHeight = true;
            m_aspectRatio = aspectRatio;
            m_webBrowser.Size = m_initialBrowserSize;
            SetHeight(height);
            SetWidth(width);
            Image[] result = GetImagesFromString(html, baseurl, type);
            return Convert(result);
        }

        /// <summary>
        /// Converts html to HtmlToPdf Element result.
        /// </summary>
        /// <returns>Converted image.</returns>
        private Syncfusion.Pdf.HtmlToPdf.HtmlToPdfResult Convert(Image[] img)
        {
            ArrayList pageBreak = null;
            ArrayList hyperLinks = null;
            if (img == null && m_docStream != null)
            {
                HtmlToPdfResult tempResult = new HtmlToPdfResult(m_docStream);
                tempResult.IsImagePath = m_isImagePath;
                return tempResult;
            }
            
            while (m_webBrowser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

             pageBreak = GetPageBreak();
             hyperLinks = (EnableHyperlinks) ? GetHyperlinks() : new ArrayList();
            ArrayList documentLinks = (EnableHyperlinks) ? GetDocumentLinks() : new ArrayList();

            // if ForceSize, then keep remaining height in memory.
            float currentHeight = 0.0f;
            if (currentregions != null && m_forceSize)
            {
                float temp = currentregions[0] + currentregions[1];
                if (temp < m_htmlHeight)
                    currentHeight = m_htmlHeight - temp;
            }

            HtmlToPdfResult htmlToPdfResult = new HtmlToPdfResult(img, pageBreak, hyperLinks, documentLinks, currentHeight);
            if (m_clipScrollBars)
            {
                PdfUnitConvertor unitConv = new PdfUnitConvertor();
                float w = unitConv.ConvertFromPixels(SystemInformation.VerticalScrollBarWidth, PdfGraphicsUnit.Point);
                float h = unitConv.ConvertFromPixels(SystemInformation.HorizontalScrollBarHeight, PdfGraphicsUnit.Point);
                if (m_controlSize.Height + h > m_size.Height)
                    htmlToPdfResult.ScrollBarWidth = w;
                if (m_controlSize.Width + w > m_size.Width)
                    htmlToPdfResult.ScrollBarHeight = h;
            }
            htmlToPdfResult.IsImagePath = m_isImagePath;
            return htmlToPdfResult;
        }


        private Image ToImage(Size size)
        {
            Image image;
            MemoryStream ms = new MemoryStream();
            try
            {
                IntPtr hDCMem = Native.CreateCompatibleDC((IntPtr)null);
                Rectangle bounds = new Rectangle(Point.Empty, CalculateImageSize(size));
                RECT rect = new RECT(bounds.X, bounds.Y, bounds.Width, bounds.Height);

                if (m_type == ImageType.Metafile)
                {

                        image = new Metafile(ms, hDCMem, bounds,
                      MetafileFrameUnit.Pixel, EmfType.EmfOnly);
                }
                else
                {
                    image = new Bitmap(bounds.Width, bounds.Height);
                }
                Graphics graphics = Graphics.FromImage(image);
                IntPtr memdc = graphics.GetHdc();
                IViewObject viewObject = (IViewObject)m_webBrowser.GetOcx();
                int hr = viewObject.Draw((uint)System.Runtime.InteropServices.ComTypes.DVASPECT.DVASPECT_CONTENT,
                -1,
                 IntPtr.Zero,
                 IntPtr.Zero,
                 IntPtr.Zero,
                 memdc,
                 ref rect,
                 ref rect,
                 IntPtr.Zero,
                 (uint)0);
                viewObject = null;
                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                Native.DeleteDC(hDCMem);

                Reset();
                //DumpMetafile(ms);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                ms.Close();
            }
            return image;
        }



        /// <summary>
        /// Converts Html to metafile.
        /// </summary>
        /// <returns>Metafile image.</returns>
        private Metafile ConvertToMetafile2()
        {

            Metafile mf;
            MemoryStream ms = new MemoryStream();

            try
            {
                Size size = ResizeBrowser2();
                IntPtr hDCMem = Native.CreateCompatibleDC((IntPtr)null);
                Rectangle bounds = new Rectangle(Point.Empty, CalculateImageSize(size));


                mf = new Metafile(ms, hDCMem, bounds,
                  MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                Graphics graphics = Graphics.FromImage(mf);
                IntPtr memdc = graphics.GetHdc();

                Native.OleDraw(m_webBrowser.GetOcx(), 1, memdc, ref bounds);

                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                Native.DeleteDC(hDCMem);

                //DumpMetafile(ms);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                ms.Close();
            }

            return mf;
        }
        /// <summary>
        /// Converts Html to metafile.
        /// </summary>
        /// <returns>Metafile image.</returns>
        private Metafile ConvertToMetafile()
        {
            Metafile mf;
            MemoryStream ms = new MemoryStream();

            try
            {
                Size size = ResizeBrowser();
                Bitmap bmp = new Bitmap(1, 1);
                Graphics rtfGraphics = Graphics.FromImage(bmp);
                IntPtr refHdc = rtfGraphics.GetHdc();

                RectangleF bounds = new RectangleF(0, 0, size.Width, size.Height);

                mf = new Metafile(ms, refHdc, bounds,
                  MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                Graphics graphics = Graphics.FromImage(mf);
                IntPtr memdc = graphics.GetHdc();

                IHTMLElement2 bodyElement = GetHtmlBody();

                Print(memdc, false);

                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                rtfGraphics.ReleaseHdc(refHdc);
                rtfGraphics.Dispose();
                bmp.Dispose();

                //DumpMetafile( ms );
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                ms.Close();
            }

            return mf;
        }
        /// <summary>
        /// Converts Html to bitmap.
        /// </summary>
        /// <returns>Bitmap image.</returns>
        private Bitmap ConvertToBitmap2()
        {
            Bitmap result;

            try
            {
                InitializeBrowser();
                Size size = ResizeBrowser2();
                CheckHeight(ref size);
                Rectangle bounds = new Rectangle(Point.Empty, CalculateImageSize(size));
                Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                IntPtr memdc = graphics.GetHdc();
                IntPtr hbitmap = bitmap.GetHbitmap();
                Native.SelectObject(memdc, hbitmap);

                Native.OleDraw(m_webBrowser.GetOcx(), 1, memdc, ref bounds);
                result = Bitmap.FromHbitmap(hbitmap);

                Native.DeleteObject(hbitmap);
                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                bitmap.Dispose();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }

            return result;
        }
        /// <summary>
        /// Converts Html to bitmap.
        /// </summary>
        /// <returns>Bitmap image.</returns>
        private Bitmap ConvertToBitmap()
        {
            Bitmap result;

            try
            {
                Size size = ResizeBrowser();

                Bitmap bitmap = new Bitmap(size.Width, size.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                IntPtr memdc = graphics.GetHdc();
                IntPtr hbitmap = bitmap.GetHbitmap();
                Native.SelectObject(memdc, hbitmap);

                Print(memdc, true);
                result = Bitmap.FromHbitmap(hbitmap);

                Native.DeleteObject(hbitmap);
                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                bitmap.Dispose();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }

            return result;
        }


        public int Authenticate(ref IntPtr phwnd, ref IntPtr pszUsername,
    ref IntPtr pszPassword)
        {

            IntPtr sUser = Marshal.StringToCoTaskMemAuto(m_username);
            IntPtr sPassword = Marshal.StringToCoTaskMemAuto(m_password);

            pszUsername = sUser;
            pszPassword = sPassword;
            return S_OK;
        }

        #region IServiceProvider Members

        public int QueryService(ref Guid guidService, ref Guid riid, out
IntPtr ppvObject)
        {
            int nRet = guidService.CompareTo(IID_IAuthenticate);
            // Zero returned if the compared objects are equal
            if (nRet == 0)
            {
                nRet = riid.CompareTo(IID_IAuthenticate);
                // Zero returned if the compared objects are equal
                if (nRet == 0)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this,
typeof(IAuthenticate));
                    return S_OK;
                }
            }
            ppvObject = new IntPtr();
            return INET_E_DEFAULT_ACTION;
        }

        #endregion

        /// <summary>
        /// Start exporting Html to Image.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        private Image ToImage()
        {
            Image result = ConvertToImage();
            Reset();

            return result;
        }


        private void InitializeBrowser()
        {
            if (!Created)
            {
                CreateControl();
            }

            // Make sure the ie control is ready, and if not wait for it to be ready
            while (!m_webBrowser.Created)
            {
                m_webBrowser.CreateControl();
                Application.DoEvents();
            }

            // Dsable user interaction such a message boxes, etc.
            if (!m_webBrowser.Silent)
            {
                m_webBrowser.Silent = true;
            }

            // Stop any other loading.
            if (m_webBrowser.Busy)
            {
                m_webBrowser.Stop();
            }

            if (m_username != null||m_username!=string.Empty)
            {
                if (ClearInternetCache)
                {
                    try
                    {
                        ClearFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache))); ClearFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)));
                    }
                    catch
                    {
                    }
                }

                object missing = System.Reflection.Missing.Value;
                missing = null;
                m_webBrowser.Navigate("about:blank", ref missing, ref missing, ref missing, ref missing);

                object obj = m_webBrowser.GetOcx();
                IOleObject oc = obj as IOleObject;
                oc.SetClientSite(this);
            }

            object flags = BrowserNavConstants.navNoHistory |
              BrowserNavConstants.navNoReadFromCache |
              BrowserNavConstants.navNoWriteToCache;
            object obj2 = null;
            object obj3 = null;
            object obj4 = null;

            m_webBrowser.Navigate(m_url, ref flags, ref obj2, ref obj3, ref obj4);

            // Wait for the document to load
            while (!m_DocComplete && !m_bCancelNavigate)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

            // If someone disposed this object during document loading or error occured - exit.
            if (IsDisposed)// || m_bCancelNavigate)
            {
                return;
            }

            // Wait for the DOM to be accessible
            while (GetHtmlBody() == null)
            {
                Thread.Sleep(DEF_TIMEOUT);
                Application.DoEvents();
            }

            // Load html from the string.
            if (m_url == DEF_BLANK && m_html != null)
            {
                IHTMLDocument2 document = GetDocument();

                document.write(m_html);
            }
			
			if (m_enableActiveXContents)
            {
                HTMLDocumentClass htmlDom = (m_webBrowser.Document as HTMLDocumentClass);

                if (htmlDom != null)
                {
                    IHTMLElementCollection elements = (htmlDom.body.all as IHTMLElementCollection);
                    IHTMLElementCollection objElements = (elements.tags("object") as IHTMLElementCollection);

                    foreach (IHTMLObjectElement element in objElements)
                    {
                        // Fix for object tag throws exception without Silverlight control.
                        try
                        {
                            IXcpControl2 ctrl = (element.@object as IXcpControl2);
                            if (ctrl != null)
                            {
                                while (ctrl.IsLoaded == false)
                                {
                                    Thread.Sleep(DEF_TIMEOUT);
                                    Application.DoEvents();
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }                
            }
			if (m_additionalDelay > 0)
            {
                int count = m_additionalDelay;
                while (count > 0)
                {
                    count -= DEF_TIMEOUT;
                    Thread.Sleep(DEF_TIMEOUT);
                    Application.DoEvents();
                }
            }
        }

        private void ClearFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo fiCurrFile in directoryInfo.GetFiles())
            {
                try
                {
                    fiCurrFile.Delete();
                }
                catch (Exception ex)
                {
                }
            }
            foreach (DirectoryInfo diSubFolder in directoryInfo.GetDirectories())
            {
                ClearFolder(diSubFolder); // Call recursively for all subfolders
            }
        }

        /// <summary>
        /// Returns BODY element of the document.
        /// </summary>
        /// <returns>BODY element of the document.</returns>
        private IHTMLElement2 GetHtmlBody()
        {
            IHTMLDocument2 htmlDocument2 = GetDocument();

            return (IHTMLElement2)((htmlDocument2 != null) ?
              htmlDocument2.body : null);
        }
        /// <summary>
        /// Returns the current document.
        /// </summary>
        /// <returns>The current document.</returns>
        private IHTMLDocument2 GetDocument()
        {
            IWebBrowser2 wb2 = (IWebBrowser2)m_webBrowser.GetOcx();
            IHTMLDocument2 htmlDocument2 =m_webBrowser.Document as IHTMLDocument2;

            return htmlDocument2;
        }

        /// <summary>
        /// Resizes browser to the document.
        /// </summary>
        /// <returns></returns>
        private Size ResizeBrowser3()
        {
            IHTMLElement2 htmlBody2 = GetHtmlBody();
            Size size = new Size(m_webBrowser.Width, m_webBrowser.Height);

            if (htmlBody2 != null)
            {
                int width = htmlBody2.scrollWidth;
                //if (m_clipScrollBars)
                //    width += m_webBrowser.Margin.Vertical;
                int height = Math.Max(htmlBody2.scrollHeight, m_webBrowser.Height);

                if (m_isImagePath)
                {
                    if (!isCustomHeight || (m_size.Height < height))
                    {
                        if (m_clipScrollBars)
                        {
                            width = SystemInformation.VerticalScrollBarWidth + width;
                            height = SystemInformation.HorizontalScrollBarHeight + height;
                        }
                    }
                }
                size = new Size(width, height);
                m_webBrowser.Size = size;
            }

            return size;
        }

        /// <summary>
        /// Resizes browser to the document.
        /// </summary>
        /// <returns>Size of the browser.</returns>
        private Size ResizeBrowser2()
        {
            // Figure out width and height of full page plus scroll bar
            IHTMLElement2 htmlBody2 = GetHtmlBody();
            Size size = new Size(m_webBrowser.Width, m_webBrowser.Height);

            if (htmlBody2 != null)
            {
                int width = htmlBody2.scrollWidth;
                int height = Math.Max(htmlBody2.scrollHeight, m_webBrowser.Height);

                Width = width;
                Height = height;



                m_webBrowser.Width = width;
                m_webBrowser.Height = height;
                size = new Size(width, height);

                if (EnableHyperlinks && size.Height < Int16.MaxValue)
                {
                    PdfUnitConvertor converter = new PdfUnitConvertor();

                    Size outSize = converter.ConvertToPixels((SizeF)m_size, PdfGraphicsUnit.Point).ToSize();

                    if (outSize.Width > Width)
                    {
                        outSize.Width = Width;
                        m_size = converter.ConvertFromPixels((SizeF)outSize, PdfGraphicsUnit.Point).ToSize();
                    }

                    m_webBrowser.Width = (outSize.Width <= 0) ? width : outSize.Width;
                    m_webBrowser.Height = (outSize.Height <= 0) ? height : outSize.Height;
                    size = new Size(m_webBrowser.Width, m_webBrowser.Height);
                }

            }

            return size;
        }
        /// <summary>
        /// Resizes browser to the document.
        /// </summary>
        /// <returns>Size of the browser.</returns>
        private Size ResizeBrowser()
        {
            // Figure out width and height of full page plus scroll bar
            IHTMLElement2 htmlBody2 = GetHtmlBody();
            int width = htmlBody2.scrollWidth;
            int height = m_forceSize ? htmlBody2.scrollHeight : Math.Max(htmlBody2.scrollHeight, m_webBrowser.Height);

            // Make our embedded IE control that size
            int vWidth = SystemInformation.VerticalScrollBarWidth + width + 2;
            int hHeight = SystemInformation.HorizontalScrollBarHeight + height + 2;

            if (m_webBrowser.Height != hHeight)
            {
                m_webBrowser.Height = hHeight;
            }

            if (m_webBrowser.Width != vWidth)
            {
                m_webBrowser.Width = vWidth;
            }
            return new Size(width, height);
        }
        /// <summary>
        /// Prints Html to the device context.
        /// </summary>
        /// <param name="ptr">Device context.</param>
        private void Print(IntPtr ptr, bool isBitmap)
        {
            uint lParam = Native.PRF_CLIENT |
              Native.PRF_OWNED | Native.PRF_ERASEBKGND;

            if (isBitmap)
            {
                lParam |= Native.PRF_CHILDREN;
            }

            int err = Native.SendMessage(m_webBrowser.Handle,
              Native.WM_PRINT, (uint)ptr, lParam);
        }
        /// <summary>
        /// Saves metafile to the file. It's debug method.
        /// </summary>
        /// <param name="ms">data containing metafile.</param>
        internal void DumpMetafile(MemoryStream ms)
        {
            if (ms == null)
                throw new ArgumentNullException("ms");

            using (FileStream fs = File.OpenWrite("C:\\Temp\\image.emf"))
            {
                ms.Position = 0;
                byte[] buff = new byte[ms.Length];
                ms.Read(buff, 0, buff.Length);
                fs.Write(buff, 0, buff.Length);
            }
        }
        /// <summary>
        /// disables scrollbars.
        /// </summary>
        private void DisableScrollBars()
        {
            IHTMLElement2 bodyElm = GetHtmlBody();

            if (bodyElm != null)
            {
                IHTMLElement body = (IHTMLElement)bodyElm;
                IHTMLBodyElement bodyElement = (IHTMLBodyElement)body;
                body.style.borderStyle = "none";
                bodyElement.scroll = "no";
            }
        }
        /// <summary>
        /// Sets width of the web browser.
        /// </summary>
        /// <param name="width">Width of the web browser.</param>
        private void SetWidth(int width)
        {
            if (width > 0)
            {
                Size clientSize = m_webBrowser.ClientSize;
                clientSize.Width = width;
                m_webBrowser.ClientSize = clientSize;
                m_size.Width = width;
            }
        }
        /// <summary>
        /// Sets height of the web browser.
        /// </summary>
        /// <param name="height">Height of the web browser.</param>
        private void SetHeight(int height)
        {
            if (height > 0)
            {
                Size clientSize = m_webBrowser.ClientSize;
                clientSize.Height = height;
                m_webBrowser.ClientSize = clientSize;
                m_size.Height = height;
            }
        }
        /// <summary>
        /// Resets the settings.
        /// </summary>
        private void Reset()
        {
            m_DocComplete = false;
            m_bCancelNavigate = false;
            //m_size = Size.Empty;
            //m_aspectRatio = AspectRatio.None;
        }
        /// <summary>
        /// Calculates size of the resulting image.
        /// </summary>
        /// <param name="documentSize">Size of the document.</param>
        /// <returns>Size of the image.</returns>
        private Size CalculateImageSize(Size documentSize)
        {
            SizeF imageSize = documentSize;
            dx = (float)m_size.Width / (float)documentSize.Width;
            dy = (float)m_size.Height / (float)documentSize.Height;
            if (dx < 0)
                dx = 1;
            if (dy < 0)
                dy = 1;

            if (m_forceSize)
            {
                imageSize.Width *= dx;
                if (dy < 1)
                    imageSize.Height *= dy;
                return Size.Ceiling(imageSize);
            }

            switch (m_aspectRatio)
            {
                case AspectRatio.KeepWidth:
                    if (imageSize.Width > m_size.Width)
                    {
                        imageSize.Width *= dx;
                        imageSize.Height *= dx;
                    }
                    else
                        dx = 1;
                    break;
                case AspectRatio.KeepHeight:
                    imageSize.Width *= dy;
                    imageSize.Height *= dy;
                    break;
                //default:
                //    if (EnableHyperlinks)
                //    {
                //        imageSize.Width *= dx;
                //        imageSize.Height *= dx;
                //    }
                //    break;
            }

            return Size.Ceiling(imageSize);
        }

        /// <summary>
        /// Calculates the bounds of the Html Element.
        /// </summary>
        /// <param name="element">The Html Element.</param>
        /// <returns>The Bounds.</returns>
        internal RectangleF GetBounds(IHTMLElement element, bool ConvertToPoint)
        {
            float offsetLeft;
            float offsetWidth;
            float offsetHeight;
            float offsetTop;
            if (isLinkWrapped == false)
            {
                offsetLeft = element.offsetLeft;
                offsetWidth = element.offsetWidth;
                offsetHeight = element.offsetHeight;
                offsetTop = element.offsetTop;
            }
            else
            {
                offsetLeft = 10;
                offsetWidth = element.offsetWidth;
                offsetTop = element.offsetTop + element.offsetHeight;
                offsetHeight = element.offsetHeight;
            }
            PdfUnitConvertor convertor = new PdfUnitConvertor();

            for (IHTMLElement i = element.offsetParent; i != null; i = i.offsetParent)
            {
                offsetLeft += i.offsetLeft;
                offsetTop += i.offsetTop;
            }
            if (ConvertToPoint)
            {
                RectangleF bounds = new RectangleF(offsetLeft, offsetTop, offsetWidth, offsetHeight);
                return convertor.ConvertFromPixels(bounds, PdfGraphicsUnit.Point);
            }
            else
                return new RectangleF(offsetLeft, offsetTop, offsetWidth, offsetHeight);

        }

        /// <summary>
        /// Gets page break.
        /// </summary>
        private ArrayList GetHyperlinks()
        {
            ArrayList hyperLinkCollection = new ArrayList();
            IHTMLDocument2 document = m_webBrowser.Document as IHTMLDocument2;
            if (document != null)
            {
                IHTMLElementCollection elements = document.links;
                for (int i = 0; i < elements.length; i++)
                {
                    try
                    {
                        isLinkWrapped = false;
                        RectangleF linkBounds = new RectangleF();
                        RectangleF linkPosition = new RectangleF();
                        float size;
                        object obj = elements.item(i, i);
                        bool ishash = false;

                        if (obj is IHTMLAnchorElement)
                        {
                            IHTMLAnchorElement element = obj as IHTMLAnchorElement;
                            if (element.protocol == "file:")
                                ishash = true;
                            IHTMLElement anchorElement = obj as IHTMLElement;
                            string tempText = anchorElement.innerText;
                            string href = element.href;
                            linkBounds = GetBounds(anchorElement, false);

                            if (m_size.Width < linkBounds.X + linkBounds.Width)
                            {
                                string link;
                                string[] innerText = anchorElement.innerText.Split(' ');
                                anchorElement.innerText = "";
                                for (int loop = 0; loop < innerText.Length; loop++)
                                {
                                    linkPosition = linkBounds;
                                    link = anchorElement.innerText;
                                    anchorElement.innerText += innerText[loop] + " ";
                                    linkBounds = GetBounds(anchorElement, false);
                                    size = linkBounds.X + linkBounds.Width;
                                    if ((int)size > m_size.Width && isLinkWrapped == false)
                                    {
                                        if (m_aspectRatio == AspectRatio.KeepWidth)
                                        {
                                            linkPosition.X = linkPosition.X * dx;
                                            linkPosition.Y = linkPosition.Y * dx;
                                            linkPosition.Width = linkPosition.Width * dx;
                                            linkPosition.Height = linkPosition.Height * dx;
                                        }
                                        else if (m_aspectRatio == AspectRatio.KeepHeight)
                                        {
                                            linkPosition.X = linkPosition.X * dy;
                                            linkPosition.Y = linkPosition.Y * dy;
                                            linkPosition.Width = linkPosition.Width * dy;
                                            linkPosition.Height = linkPosition.Height * dy;
                                        }
                                        HtmlHyperLink temp = new HtmlHyperLink(linkPosition, href);
                                        if (ishash)
                                            temp.Hash = element.hash.Remove(0, 1);
                                        hyperLinkCollection.Add(temp);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (m_aspectRatio == AspectRatio.KeepWidth)
                                {
                                    linkBounds.X = linkBounds.X * dx;
                                    linkBounds.Y = linkBounds.Y * dx;
                                    linkBounds.Width = linkBounds.Width * dx;
                                    linkBounds.Height = linkBounds.Height * dx;
                                }
                                else if (m_aspectRatio == AspectRatio.KeepHeight)
                                {
                                    linkBounds.X = linkBounds.X * dy;
                                    linkBounds.Y = linkBounds.Y * dy;
                                    linkBounds.Width = linkBounds.Width * dy;
                                    linkBounds.Height = linkBounds.Height * dy;
                                }
                                HtmlHyperLink temp = new HtmlHyperLink(linkBounds, href);
                                if (ishash)
                                    temp.Hash = element.hash.Remove(0, 1);
                                hyperLinkCollection.Add(temp);
                            }
                            if (anchorElement.innerText != tempText)
                                anchorElement.innerText = tempText;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return hyperLinkCollection;
        }

        /// <summary>
        /// Returns all document links present in the HTML.
        /// </summary>
        /// <returns></returns>
        private ArrayList GetDocumentLinks()
        {
            ArrayList documentLink = new ArrayList();

            IHTMLDocument2 document = m_webBrowser.Document as IHTMLDocument2;
            if (document != null)
            {
                IHTMLElementCollection collection = document.anchors;
                float size;
                RectangleF linkBounds = RectangleF.Empty;
                RectangleF linkPosition = RectangleF.Empty;

                for (int i = 0; i < collection.length; i++)
                {
                    Object obj = collection.item(i, i);
                    if (obj is IHTMLAnchorElement)
                    {
                        IHTMLElement anchorElement = obj as IHTMLElement;
                        string tempText = anchorElement.innerText;
                        string href = anchorElement.innerText;

                        if ((obj as IHTMLAnchorElement).name == null)
                            continue;

                        linkBounds = GetBounds(anchorElement, false);

                        if (href != null && m_size.Width < linkBounds.X + linkBounds.Width)
                        {
                            string link;
                            string[] innerText = anchorElement.innerText.Split(' ');
                            anchorElement.innerText = "";
                            for (int loop = 0; loop < innerText.Length; loop++)
                            {
                                linkPosition = linkBounds;
                                link = anchorElement.innerText;
                                anchorElement.innerText += innerText[loop] + " ";
                                linkBounds = GetBounds(anchorElement, false);
                                size = linkBounds.X + linkBounds.Width;
                                if ((int)size > m_size.Width && isLinkWrapped == false)
                                {
                                    if (m_aspectRatio == AspectRatio.KeepWidth)
                                    {
                                        linkPosition.X = linkPosition.X * dx;
                                        linkPosition.Y = linkPosition.Y * dx;
                                        linkPosition.Width = linkPosition.Width * dx;
                                        linkPosition.Height = linkPosition.Height * dx;
                                    }
                                    else if (m_aspectRatio == AspectRatio.KeepHeight)
                                    {
                                        linkPosition.X = linkPosition.X * dy;
                                        linkPosition.Y = linkPosition.Y * dy;
                                        linkPosition.Width = linkPosition.Width * dy;
                                        linkPosition.Height = linkPosition.Height * dy;
                                    }
                                    HtmlHyperLink temp = new HtmlHyperLink(linkPosition, href);
                                    temp.Name = (obj as IHTMLAnchorElement).name;
                                    documentLink.Add(temp);

                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (m_aspectRatio == AspectRatio.KeepWidth)
                            {
                                linkBounds.X = linkBounds.X * dx;
                                linkBounds.Y = linkBounds.Y * dx;
                                linkBounds.Width = linkBounds.Width * dx;
                                linkBounds.Height = linkBounds.Height * dx;
                            }
                            else if (m_aspectRatio == AspectRatio.KeepHeight)
                            {
                                linkBounds.X = linkBounds.X * dy;
                                linkBounds.Y = linkBounds.Y * dy;
                                linkBounds.Width = linkBounds.Width * dy;
                                linkBounds.Height = linkBounds.Height * dy;
                            }
                            HtmlHyperLink temp = new HtmlHyperLink(linkBounds, href);
                            temp.Name = (obj as IHTMLAnchorElement).name;
                            documentLink.Add(temp);
                        }
                        if (anchorElement.innerText != tempText)
                            anchorElement.innerText = tempText;
                    }
                }
            }
            return documentLink;
        }

        /// <summary>
        /// Gets page break.
        /// </summary>
        private ArrayList GetPageBreak()
        {
            ArrayList pageBreaks = new ArrayList();
            float num = 0;
            float offset = 0;
            float pos = 0;

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            if (!AutoDetectPageBreak)
            {
                pos = convertor.ConvertFromPixels(m_webBrowser.Height, Syncfusion.Pdf.Graphics.PdfGraphicsUnit.Point);
                pageBreaks.Add(pos);
                return pageBreaks;
            }
            IHTMLDocument2 document = m_webBrowser.Document as IHTMLDocument2;
            if (document != null)
            {
                for (int i = 0; i < document.all.length; i++)
                {
                    try
                    {
                        IHTMLElement element = (IHTMLElement)document.all.item(i, i);
                        if (element is IHTMLElement2)
                        {
                            IHTMLElement2 element2 = element as IHTMLElement2;
                            string pageBreakBefore = element2.currentStyle.pageBreakBefore;
                            if ((pageBreakBefore != null) && (pageBreakBefore.Trim().ToLower() == "always"))
                            {
                                RectangleF bounds = GetBounds(element, false);
                                pos = convertor.ConvertFromPixels(bounds.Top, Syncfusion.Pdf.Graphics.PdfGraphicsUnit.Point) - offset;
                                offset += pos;
                                if (m_aspectRatio == AspectRatio.KeepWidth)
                                {
                                    pos *= dx;
                                }
                                else if (m_aspectRatio == AspectRatio.KeepHeight)
                                {
                                    pos *= dy;
                                }
                                 
                                pageBreaks.Add(pos);
                                num += pos;
                            }
                            string pageBreakAfter = element2.currentStyle.pageBreakAfter;
                            if ((pageBreakAfter != null) && (pageBreakAfter.Trim().ToLower() == "always"))
                            {
                                RectangleF bounds = GetBounds(element, false);
                                pos = convertor.ConvertFromPixels(bounds.Bottom, Syncfusion.Pdf.Graphics.PdfGraphicsUnit.Point) - offset;
                                offset += pos;
                                if (m_aspectRatio == AspectRatio.KeepWidth)
                                {
                                    pos *= dx;
                                }
                                else if (m_aspectRatio == AspectRatio.KeepHeight)
                                {
                                    pos *= dy;
                                }
                                pageBreaks.Add(pos);
                                num += pos;
                            }
                        }
                    }
                    catch
                    { }
                }
                if (pageBreaks.Count == 0)
                {
                    pos = convertor.ConvertFromPixels(m_webBrowser.Height, Syncfusion.Pdf.Graphics.PdfGraphicsUnit.Point);
                    pageBreaks.Add(pos);
                    return pageBreaks;
                }
                pos = convertor.ConvertFromPixels(m_webBrowser.Height, Syncfusion.Pdf.Graphics.PdfGraphicsUnit.Point) - num;
                if (pos > 0)
                {
                    pageBreaks.Add(0.0f);
                }
                else
                {
                    pageBreaks.Add(pos);
                }
            }
            return pageBreaks;
        }

        private bool IsVerScrollBarPresent()
        {
            if (m_clipScrollBars)
            {
                PdfUnitConvertor unitConv = new PdfUnitConvertor();
                float w = unitConv.ConvertFromPixels(SystemInformation.VerticalScrollBarWidth, PdfGraphicsUnit.Point);
                float h = unitConv.ConvertFromPixels(SystemInformation.HorizontalScrollBarHeight, PdfGraphicsUnit.Point);
                if (m_controlSize.Height + h > m_size.Height)
                    return false;
            }
            return false;
        }
        private bool IsHorScrollBarPresent()
        {
            if (m_clipScrollBars)
            {
                PdfUnitConvertor unitConv = new PdfUnitConvertor();
                float w = unitConv.ConvertFromPixels(SystemInformation.VerticalScrollBarWidth, PdfGraphicsUnit.Point);
                float h = unitConv.ConvertFromPixels(SystemInformation.HorizontalScrollBarHeight, PdfGraphicsUnit.Point);
                if (m_controlSize.Width + w > m_size.Width)
                    return false;
            }
            return false;
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Loading of the document completed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDocumentComplete(object sender, DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            string url = e.uRL.ToString();
            bool urlValid = ((url != DEF_BLANK || url == m_url) && url.Length > 0);
            bool finalFrame = (e.pDisp == m_webBrowser.GetOcx());

            if (urlValid && finalFrame)
            {
                m_DocComplete = true;
                if (!m_clipScrollBars)
                    DisableScrollBars();
            }

        }
        /// <summary>
        /// Blocks popups.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NewWindow(object sender, DWebBrowserEvents2_NewWindow2Event e)
        {
            e.cancel = true;
        }
        /// <summary>
        /// Handles Navigation Error.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NavigateError(object sender, DWebBrowserEvents2_NavigateErrorEvent e)
        {
            bool finalFrame = (e.pDisp == m_webBrowser.GetOcx());

            if (finalFrame)
            {
                // Cancel the final navigation to the error page.
                e.cancel = true;
                m_bCancelNavigate = true;
            }
        }
        #endregion

        #region Customization Members
        /// <summary>
        /// Register this window as host of the web browser.
        /// </summary>
        private void SetUIHandler()
        {
            try
            {
                object obj = m_webBrowser.GetOcx();
                IOleObject oc = obj as IOleObject;
                oc.SetClientSite(this);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        /// <summary>
        /// Applies download and executing properties.
        /// </summary>
        /// <returns>Settings flag.</returns>
        /// <remarks>Don't call this method. It's used for internal purpose only. </remarks>
        [DispIdAttribute(DISPID_AMBIENT_DLCONTROL)]
        public int CustomizeDownload()
        {
            INVOKE_PARAMS Exec_Params = INVOKE_PARAMS.DLCTL_SILENT |
                    INVOKE_PARAMS.DLCTL_NO_JAVA |
                    INVOKE_PARAMS.DLCTL_VIDEOS |
                    INVOKE_PARAMS.DLCTL_DLIMAGES | INVOKE_PARAMS.DLCTL_NO_RUNACTIVEXCTLS |
                    INVOKE_PARAMS.DLCTL_NO_BEHAVIORS;

            if (!m_enableJavaScript)
                Exec_Params |= INVOKE_PARAMS.DLCTL_NO_SCRIPTS;

            if (!m_enableActiveXContents)
                Exec_Params |= INVOKE_PARAMS.DLCTL_NO_DLACTIVEXCTLS;// | INVOKE_PARAMS.DLCTL_NO_RUNACTIVEXCTLS;

            return (int)Exec_Params;

        }

        int IDocHostShowUI.ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, ref int lpResult)
        {
            return Native.S_OK;
        }

        int IDocHostShowUI.ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, out object pDispatchObjectHit)
        {
            pDispatchObjectHit = null;
            return Native.S_OK;
        }

        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.ShowContextMenu(uint dwID, ref tagPOINT ppt, object pcmdtReserved, object pdispReserved)
        {
            
        }

        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.GetHostInfo(ref DOCHOSTUIINFO pInfo)
        {
            if (m_clipScrollBars)
                DEF_DOCHOSTUI_FLAG &= ~DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO;
            pInfo.dwFlags |= (uint)DEF_DOCHOSTUI_FLAG;
            pInfo.dwDoubleClick = (uint)DEF_DOCHOSTUIDBLCLKFLAG;
            pInfo.cbSize = (uint)Marshal.SizeOf(pInfo);
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.ShowUI(uint dwID, IntPtr pActiveObject, IntPtr pCommandTarget, IntPtr pFrame, IntPtr pDoc)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.HideUI()
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.UpdateUI()
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.EnableModeless(int fEnable)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.OnDocWindowActivate(int fActivate)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.OnFrameWindowActivate(int fActivate)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.ResizeBorder(ref tagRECT prcBorder, IntPtr pUIWindow, int fRameWindow)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.TranslateAccelerator(ref tagMSG lpmsg, ref Guid pguidCmdGroup, uint nCmdID)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.GetOptionKeyPath(out string pchKey, uint dw)
        {
            pchKey = null;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.GetDropTarget(Syncfusion.HtmlConverter.Natives.IDropTarget pDropTarget, out Syncfusion.HtmlConverter.Natives.IDropTarget ppDropTarget)
        {
            ppDropTarget = null;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.GetExternal(out object ppDispatch)
        {
            ppDispatch = new ExternalScriptWrapper(this);
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.TranslateUrl(uint dwTranslate, ref ushort pchURLIn, IntPtr ppchURLOut)
        {
            
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        void IDocHostUIHandler.FilterDataObject(System.Runtime.InteropServices.ComTypes.IDataObject pDO, out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet)
        {
            ppDORet = null;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.SaveObject()
        {
            return Native.S_OK;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk)
        {
            return Native.S_OK;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.GetContainer(object ppContainer)
        {
            ppContainer = this;

            return Native.S_OK;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.ShowObject()
        {
            return Native.S_OK;
        }
        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.OnShowWindow(bool fShow)
        {
            return Native.S_OK;
        }

        /// <summary>
        /// Customization method.
        /// </summary>
        int IOleClientSite.RequestNewObjectLayout()
        {
            return Native.S_OK;
        }
        #endregion

        #region IInternetSecurityManager Members
        int IInternetSecurityManager.SetSecuritySite(IntPtr pSite)
        {
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.GetSecuritySite(out IntPtr pSite)
        {
            pSite = IntPtr.Zero;
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.MapUrlToZone(string pwszUrl, out uint pdwZone, uint dwFlags)
        {
            pdwZone = 0;
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.GetSecurityId(string pwszUrl, IntPtr pbSecurityId, ref uint pcbSecurityId, ref uint dwReserved)
        {
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.ProcessUrlAction(string pwszUrl, uint dwAction, IntPtr pPolicy, uint cbPolicy, IntPtr pContext, uint cbContext, uint dwFlags, uint dwReserved)
        {
            if (ProcessUrlAction == null)
                return unchecked((int)0x800C0011);

            try
            {
                URLACTION action = (URLACTION)dwAction;
                ProcessUrlActionFlags flags = (ProcessUrlActionFlags)dwFlags;
                bool hasUrlPolicy = (cbPolicy >= unchecked((uint)Marshal.SizeOf(typeof(int))));
                URLPOLICY urlPolicy = (hasUrlPolicy) ? urlPolicy = (URLPOLICY)Marshal.ReadInt32(pPolicy) : URLPOLICY.ALLOW;
                bool hasContext = (cbContext >= unchecked((uint)Marshal.SizeOf(typeof(Guid))));
                Guid context = (hasContext) ? (Guid)Marshal.PtrToStructure(pContext, typeof(Guid)) : Guid.Empty;

                ProcessUrlActionEvent.SetParameters(pwszUrl, action, urlPolicy, context, flags, hasContext);
                ProcessUrlAction(this, ProcessUrlActionEvent);

                if (ProcessUrlActionEvent.handled && hasUrlPolicy)
                {
                    Marshal.WriteInt32(pPolicy, (int)ProcessUrlActionEvent.urlPolicy);
                    return (ProcessUrlActionEvent.Cancel) ? Hresults.S_FALSE : Hresults.S_OK;
                }

            }
            finally
            {
                ProcessUrlActionEvent.ResetParameters();
            }

            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.QueryCustomPolicy(string pwszUrl, ref Guid guidKey, out IntPtr ppPolicy, out uint pcbPolicy, IntPtr pContext, uint cbContext, uint dwReserved)
        {
            ppPolicy = IntPtr.Zero;
            pcbPolicy = 0;
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.SetZoneMapping(uint dwZone, string lpszPattern, uint dwFlags)
        {
            return unchecked((int)0x800C0011);
        }

        int IInternetSecurityManager.GetZoneMappings(uint dwZone, out System.Runtime.InteropServices.ComTypes.IEnumString ppenumString, uint dwFlags)
        {
            ppenumString = null;
            return unchecked((int)0x800C0011);
        }

        #endregion
    }

    /// <summary>
    /// Class to execute javascript calls for GetExternal.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ExternalScriptWrapper
    {
        HtmlConverter myOwner;

        /// <summary>
        /// Requires a handle to the owning form
        /// </summary>
        /// <param name="ownerForm"></param>
        public ExternalScriptWrapper(HtmlConverter ownerForm)
        {
            myOwner = ownerForm;
        }

        ~ExternalScriptWrapper()
        {

        }
    }

    #region Enums
    /// <summary>
    /// Type of the image.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// Image is bitmap.
        /// </summary>
        Bitmap,
        /// <summary>
        /// Image is metafile.
        /// </summary>
        Metafile
    }

    /// <summary>
    /// The type defines the primary dimension of the output image.
    /// </summary>
    public enum AspectRatio
    {
        /// <summary>
        /// Don't apply keeping of aspect ratio.
        /// </summary>
        None,
        /// <summary>
        /// Keep width fixed and proportionally change height of the image.
        /// </summary>
        KeepWidth,
        /// <summary>
        /// Keep height fixed and proportionally change width of the image.
        /// </summary>
        KeepHeight,
        /// <summary>
        /// keep the width and height.
        /// </summary>
        FitPageSize
    }
    #endregion
}
