#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which is between the control and the HTMLParser.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class InputHTML
      : IInputHTML, IDisposable, ICloneable
    {
        #region Class constants
        /// <summary>
        /// Default size of the document.
        /// </summary>
        private readonly Size DEF_SIZE = new Size(400, 280);

        /// <summary>
        /// Default indent by X coordinate from the sides of the control to the document.
        /// </summary>
        private const int DEF_XINDENT = 10;
        #endregion

        #region  Class members
        /// <summary>
        /// Holds all elements by its unique ID.
        /// </summary>
        private Hashtable m_hashElementByUniqueID;

        /// <summary>
        /// Holds an array of elements by their name.
        /// </summary>
        private Hashtable m_hashElementsByName;

        /// <summary>
        /// Holds elements by their id attributes.
        /// </summary>
        private Hashtable m_hashElementByUserId;

        /// <summary>
        /// Holds an array of formats by Unique ID (formats belong to element with such unique ID).
        /// </summary>
        private Hashtable m_hashFormatsByUniqueID;

        /// <summary>
        /// Instance to the Format Manager.
        /// </summary>
        private FormatManager m_formatManager;

        /// <summary>
        /// XML document of our HTML document.
        /// </summary>
        private XmlDocument m_document;

        /// <summary>
        /// Root element of our HTML element tree.
        /// </summary>
        private IHTMLElement m_root;

        /// <summary>
        /// Instance on body element in the elements tree.
        /// </summary>
        private IHTMLElement m_body;

        /// <summary>
        /// Indicates that we were or were not disposed once.
        /// </summary>
        private bool m_bDisposed;

        /// <summary>
        /// Contains all WinForms controls in the document.
        /// </summary>
        private ArrayList m_userControls;

        /// <summary>
        /// Holds the virtual size of the document.
        /// </summary>
        private Size m_autoScrollMinSize;

        /// <summary>
        /// Scroll position offset.
        /// </summary>
        private Point m_autoScrollPosition;

        /// <summary>
        /// Searches and gets the element in which the defined point is located.
        /// </summary>
        private RectSearcher m_searcher;

        /// <summary>
        /// Client rectangle for this document.
        /// </summary>
        private Rectangle m_clientRect;

        /// <summary>
        /// Visible part of the document in virtual coordinates.
        /// </summary>
        private Rectangle m_visibleVirtualRect;

        /// <summary>
        /// Cache of images in the document.
        /// </summary>
        private BitmapCache m_imgCache;

        /// <summary>
        /// Collection of script compile errors.
        /// </summary>
        private ArrayList m_compileErrors;

        /// <summary>
        /// Indicates the status of quiet mode of the document.
        /// Used for interactivity events. If quiet mode is enabled, all changes are
        /// without any reaction.
        /// </summary>
        private bool m_bQuietMode;

        /// <summary>
        /// Type of reaction after disabling QuietMode.
        /// </summary>
        private ReactLevel m_reaction;

        /// <summary>
        /// Indicates whether we must recreate user controls.
        /// </summary>
        private bool m_bDestroyControls;

        /// <summary>
        /// Exception which occurs while loading document.
        /// </summary>
        private Exception m_loadException;

        /// <summary>
        /// Controls the element's focus changing in the document.
        /// </summary>
        private FocusManager m_tabManager;

        /// <summary>
        /// Instance on Tag element on which the mouse Event occurs.
        /// </summary>
        private IHTMLElement m_prvElement;

         /// <summary>
        /// Original text of the document.
        /// </summary>
        private string m_originalText;

        /// <summary>
        /// Holds the element from which drawing starts.
        /// </summary>
        private IHTMLElement m_startDrawElement;

        /// <summary>
        /// Indicates whether element was drawn after recalculating.
        /// </summary>
        private bool m_bWasDrawn;

        /// <summary>
        /// Indicates whether document is printing now.
        /// </summary>
        private bool m_bIsPrinting;

        /// <summary>
        /// Margins for the document.
        /// </summary>
        private Margins m_margins;

        /// <summary>
        /// Object containing data about the source of the document.
        /// </summary>
        private DataSource m_dataSource;

        /// <summary>
        /// Controls primitives flow during printing.
        /// </summary>
        private TextRegionManager m_textRegionManager;
        #endregion

        #region  Class properties
        /// <summary>
        /// Gets a value indicating whether the input HTML document is loaded from file.
        /// </summary>
        public bool IsFileName
        {
            get
            {
                return this.DataSource.IsFileName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the input HTML document is loaded by Uri.
        /// </summary>
        public bool IsUri
        {
            get
            {
                return this.DataSource.IsUri;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the input HTML document is loaded from stream.
        /// </summary>
        public bool IsStream
        {
            get
            {
                return this.DataSource.IsStream;
            }
        }

        /// <summary>
        /// Gets the path to the HTML document.
        /// </summary>
        [Category("Data"), Browsable(true), ReadOnly(true), ImmutableObject(true), DefaultValue(null), Description("Get source file name if document based on file content.")]
        public string FileName
        {
            get
            {
                return this.DataSource.FileName;
            }
        }

        /// <summary>
        /// Gets the URI to the HTML document.
        /// </summary>
        [Category("Data"), Browsable(true), ReadOnly(true), ImmutableObject(true), DefaultValue(null), Description("Get Unique Resource Identifier (URI) if document based on it.")]
        public Uri Uri
        {
            get
            {
                return this.DataSource.Uri;
            }
        }

        /// <summary>
        /// Gets the HTML document stream.
        /// </summary>
        [Category("Data"), Browsable(false), ReadOnly(true), ImmutableObject(true), DefaultValue(null), Description("Get reference on stream if document based on it.")]
        public Stream Stream
        {
            get
            {
                return this.DataSource.Stream;
            }
        }

        /// <summary>
        /// Gets the HTML document after transformation.
        /// </summary>
        [Browsable(false)]
        public XmlDocument Document
        {
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets the root element of the HTML elements tree.
        /// </summary>
        [Browsable(true), Description("Gets root element of HTML elements tree.")]
        //// [ScriptBrowsable( true ) ]
        public IHTMLElement Root
        {
            get
            {
                if (m_root == null)
                {
                    SearchRoot();
                }

                return m_root;
            }
        }

        /// <summary>
        /// Gets an instance of the body tag element.
        /// </summary>
        /// <remarks>This element is the root for rendering.</remarks>
        [Browsable(true), Description("Gets instance to body tag element.")]
        public IHTMLElement RenderRoot
        {
            get
            {
                if (m_body == null)
                {
                    SearchRoot();
                }

                return m_body;
            }
        }

        /// <summary>
        /// Gets an hash of elements by their unique ID.
        /// </summary>
        [Browsable(false)]
        public Hashtable ElementsByUniqueID
        {
            get
            {
                return m_hashElementByUniqueID;
            }
        }

        /// <summary>
        /// Gets an hash of elements by their User ID.
        /// </summary>
        [Browsable(false)]
        public Hashtable ElementsByUserID
        {
            get
            {
                return m_hashElementByUserId;
            }
        }

        /// <summary>
        /// Gets an hash of elements by their Tag name.
        /// </summary>
        [Browsable(false)]
        public Hashtable ElementsByTagName
        {
            get
            {
                return m_hashElementsByName;
            }
        }

        /// <summary>
        /// Gets an hash of formats by their unique ID.
        /// </summary>
        [Browsable(false)]
        public Hashtable ElementFormatsByUniqueID
        {
            get
            {
                return m_hashFormatsByUniqueID;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an instance is already disposed.
        /// </summary>
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return m_bDisposed;
            }
        }

        /// <summary>
        /// Gets or sets the client size of the document.
        /// </summary>
        [Browsable(false)]
        public Size ClientSize
        {
            get
            {
                return m_clientRect.Size;
            }
            set
            {
                if (m_clientRect.Size != value)
                {
                    if (value.Width <= 0 || value.Height <= 0)
                        value = DEF_SIZE;

                    m_clientRect.Size = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the virtual size of the document.
        /// </summary>
        [Browsable(false)]
        public Size AutoScrollMinSize
        {
            get
            {
                return m_autoScrollMinSize;
            }
            set
            {
                if (m_autoScrollMinSize != value)
                {
                    if (value.Width <= 0 || value.Height <= 0)
                        value = DEF_SIZE;

                    m_autoScrollMinSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the scroll offset position.
        /// </summary>
        [Browsable(false)]
        public Point AutoScrollPosition
        {
            get
            {
                return m_autoScrollPosition;
            }
            set
            {
                if (m_autoScrollPosition != value)
                {
                    m_autoScrollPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current directory for this document.
        /// </summary>
        [Browsable(false)]
        public string CurrentDirectory
        {
            get
            {
                return this.DataSource.RootDirectory;
            }
            set
            {
                this.DataSource.RootDirectory = value;
            }
        }

        /// <summary>
        /// Gets an array of script compile errors.
        /// </summary>
        [Category("Data"), Browsable(false), ReadOnly(true), ImmutableObject(false), DefaultValue(null), Description("Gets array of script compile errors.")]
        public string[] CompileErrors
        {
            get
            {
                if (m_compileErrors == null)
                {
                    m_compileErrors = new ArrayList();
                }

                return (string[])m_compileErrors.ToArray(typeof(string));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the status of quiet mode of document.
        /// If True, all interactivity changes are done without any reaction.
        /// Turn on this mode before multiple changes and turn off after all.
        /// </summary>
        [Browsable(false)]
        public bool QuietMode
        {
            get
            {
                return m_bQuietMode;
            }
            set
            {
                if (m_bQuietMode != value)
                {
                    m_bQuietMode = value;
                    OnQuietModeChanged();
                }
            }
        }

        /// <summary>
        /// Gets the exception object which occured while document parsing and rendering.
        /// </summary>
        [Browsable(false)]
        public Exception RenderException
        {
            get
            {
                return m_loadException;
            }
        }

        /// <summary>
        /// Gets the format manager object.
        /// </summary>
        [Browsable(false)]
        public FormatManager Formats
        {
            get
            {
                return m_formatManager;
            }
        }

        /// <summary>
        /// Gets or sets the start point to calculate position of elements.
        /// </summary>
        [Browsable(false), Obsolete("Don't use this property. Use Margins property instead.")]
        public Point StartPoint
        {
            get
            {
                return new Point(this.Margins.Left, this.Margins.Top);
            }
            set
            {
                // NOTE: this property is obsolete.  
            }
        }

        /// <summary>
        /// Gets the margins for the document being displayed.
        /// </summary>
        /// <remarks>This property exposes leftmargin, topmargin, rightmargin and bottommargin
        /// of the BODY tag.</remarks>
        [Browsable(false)]
        public Margins Margins
        {
            get
            {
                if (m_margins == null)
                {
                    m_margins = new Margins(this.RenderRoot as BODYElementImpl);
                }

                return m_margins;
            }
        }

        /// <summary>
        /// Gets the list of user control wrappers in the document.
        /// </summary>
        protected internal ArrayList UserControls
        {
            get
            {
                if (m_userControls == null)
                {
                    m_userControls = new ArrayList();
                }

                return m_userControls;
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate for the client position of the document.
        /// </summary>
        protected internal int X
        {
            get
            {
                return m_clientRect.X;
            }
            set
            {
                if (m_clientRect.X != value)
                {
                    m_clientRect.X = value;
                    CheckLocation();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate for the client position of the document.
        /// </summary>
        protected internal int Y
        {
            get
            {
                return m_clientRect.Y;
            }
            set
            {
                if (m_clientRect.Y != value)
                {
                    m_clientRect.Y = value;
                    CheckLocation();
                }
            }
        }

        /// <summary>
        /// Gets or sets the client width of the document.
        /// </summary>
        protected internal int ClientWidth
        {
            get
            {
                return m_clientRect.Width;
            }
            set
            {
                if (m_clientRect.Width != value)
                {
                    m_clientRect.Width = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the client height of the document.
        /// </summary>
        protected internal int ClientHeight
        {
            get
            {
                return m_clientRect.Height;
            }
            set
            {
                if (m_clientRect.Height != value)
                {
                    m_clientRect.Height = value;
                }
            }
        }

        /// <summary>
        /// Gets the searcher which searches element that contains the defined point.
        /// </summary>
        internal RectSearcher Searcher
        {
            get
            {
                if (m_searcher == null)
                {
                    m_searcher = new RectSearcher(this);
                }

                return m_searcher;
            }
        }

        /// <summary>
        /// Gets or sets the client rectangle for the document.
        /// </summary>
        protected internal Rectangle ClientRectangle
        {
            get
            {
                return m_clientRect;
            }
            set
            {
                if (!m_clientRect.Equals(value))
                {
                    m_clientRect = value;
                }
            }
        }

        /// <summary>
        /// Gets the visible part of the document in virtual coordinates.
        /// </summary>
        protected internal Rectangle VisibleRectangle
        {
            get
            {
                return m_visibleVirtualRect;
            }
        }

        /// <summary>
        /// Gets the cache of all images in the document.
        /// </summary>
        internal BitmapCache ImageCache
        {
            get
            {
                return m_imgCache;
            }
        }

        /// <summary>
        /// Gets or sets the type of document's reaction after disabling quiet mode.
        /// </summary>
        internal ReactLevel Reaction
        {
            get
            {
                return m_reaction;
            }
            set
            {
                if (m_reaction != value)
                {
                    m_reaction = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user controls in the document must be recreated.
        /// </summary>
        protected internal bool DestroyControls
        {
            get
            {
                return m_bDestroyControls;
            }
            set
            {
                if (m_bDestroyControls != value)
                {
                    m_bDestroyControls = value;
                }
            }
        }

        /// <summary>
        /// Gets the object that controls the element's focus being changed in the document.
        /// </summary>
        internal FocusManager FocusManager
        {
            get
            {
                if (m_tabManager == null)
                {
                    m_tabManager = new FocusManager();
                }

                return m_tabManager;
            }
        }

        /// <summary>
        /// Gets or sets the instance of the Tag element on which the mouse Event occurs.
        /// </summary>
        protected internal IHTMLElement PrevElement
        {
            get
            {
                return m_prvElement;
            }
            set
            {
                if (m_prvElement != value)
                {
                    m_prvElement = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the original input text of the document.
        /// </summary>
        internal string OriginalText
        {
            get
            {
                return m_originalText;
            }
            set
            {
                if (m_originalText != value)
                {
                    m_originalText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether element was drawn after position recalculation.
        /// </summary>
        protected internal bool WasPainted
        {
            get
            {
                return m_bWasDrawn;
            }
            set
            {
                if (m_bWasDrawn != value)
                {
                    m_bWasDrawn = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the document is printing.
        /// </summary>
        protected internal bool IsPrinting
        {
            get
            {
                return m_bIsPrinting;
            }
            set
            {
                if (m_bIsPrinting != value)
                {
                    m_bIsPrinting = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets object managing the objects flow during printing.
        /// </summary>
        internal TextRegionManager TextRegionManager
        {
            get
            {
                return m_textRegionManager;
            }
            set
            {
                m_textRegionManager = value;
            }
        }

        /// <summary>
        /// Gets the object containing data about the source of the document.
        /// </summary>
        internal DataSource DataSource
        {
            get
            {
                return m_dataSource;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Delegate. Raised when quiet mode property is changed.
        /// </summary>
        public event EventHandler QuietModeChanged;
        #endregion

        #region  Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the InputHTML class
        /// </summary>
        /// <param name="manager">FormatManager instance</param>
        protected InputHTML(FormatManager manager)
        {
            m_hashElementByUniqueID = new Hashtable();
            m_hashElementByUserId = new Hashtable();
            m_hashElementsByName = CollectionsUtil.CreateCaseInsensitiveHashtable();
            m_hashFormatsByUniqueID = new Hashtable();
            m_root = null;
            m_formatManager = manager;

            m_autoScrollMinSize = DEF_SIZE;
            m_autoScrollPosition = Point.Empty;
            m_clientRect = new Rectangle(DEF_XINDENT, 0, DEF_SIZE.Width, DEF_SIZE.Height);
            m_bDisposed = false;
            m_imgCache = new BitmapCache(this);
            m_reaction = ReactLevel.None;
            m_bDestroyControls = true;
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class
        /// </summary>
        /// <param name="filename">string filename</param>
        /// <param name="manager">FormatManager instance</param>
        public InputHTML(string filename, FormatManager manager)
            : this(filename, manager, new XmlDocument())
        {
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class
        /// </summary>
        /// <param name="filename">Path to the source of the HTML data.</param>
        /// <param name="manager">Format manager object.</param>
        /// <param name="output">XML document which represents the HTML data.</param>
        public InputHTML(string filename, FormatManager manager, XmlDocument output)
            : this(manager)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (filename.Length == 0)
                throw new ArgumentException("filename - string can not be empty");

            if (manager == null)
                throw new ArgumentNullException("manager");

            if (output == null)
                throw new ArgumentNullException("output");

            m_dataSource = new DataSource(filename);
            m_document = output;
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class
        /// </summary>
        /// <param name="uri">Unique Resource identifier (URI) of the HTML document.</param>
        /// <param name="manager">Format manager object.</param>
        public InputHTML(Uri uri, FormatManager manager)
            : this(uri, manager, new XmlDocument())
        {
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class. Requires URI to file.
        /// </summary>
        /// <param name="uri">Unique Resource identifier (URI) of the HTML document.</param>
        /// <param name="manager">Format manager object.</param>
        /// <param name="output">XML storage of the HTML data.</param>
        public InputHTML(Uri uri, FormatManager manager, XmlDocument output)
            : this(manager)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (manager == null)
                throw new ArgumentNullException("manager");

            if (output == null)
                throw new ArgumentNullException("output");

            m_dataSource = new DataSource(uri);
            m_document = output;
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class. Requires the stream of the document.
        /// </summary>
        /// <param name="stream">Source stream of the data.</param>
        /// <param name="manager">Format manager object.</param>
        public InputHTML(Stream stream, FormatManager manager)
            : this(stream, manager, new XmlDocument())
        {
        }

        /// <summary>
        /// Initializes a new instance of the InputHTML class. Requires the stream of the document.
        /// </summary>
        /// <param name="stream">Source stream of the data.</param>
        /// <param name="manager">Format manager object.</param>
        /// <param name="output">XML document of the HTML document.</param>
        public InputHTML(Stream stream, FormatManager manager, XmlDocument output)
            : this(manager)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (manager == null)
                throw new ArgumentNullException("manager");

            if (output == null)
                throw new ArgumentNullException("output");

            m_dataSource = new DataSource(stream);
            m_document = output;
        }

        /// <summary>
        /// Creates document from string.
        /// </summary>
        /// <param name="html">Html data.</param>
        /// <param name="manager">Format manager object.</param>
        /// <returns>New document instance.</returns>
        public static InputHTML FromString(string html, FormatManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (html == null || html.Length == 0)
            {
                html = "<html/>";
            }

            TokenStream ts = TokenStream.FromString(html);
            InputHTML document = new InputHTML(ts, manager);

            return document;
        }

        /// <summary>
        /// Clears all resources.
        /// </summary>
        public void Dispose()
        {
            if (m_bDisposed) return;

            lock (this)
            {
                this.QuietMode = true;

                // Check again if we wait in LOCK then someone can
                // dispose us earlier.
                if (m_bDisposed) return;

                DisposeControls();

                if (this.Root != null)
                {
                    ((IDisposable)this.Root).Dispose();
                }

                if (m_dataSource != null)
                {
                    m_dataSource.Dispose();
                    m_dataSource = null;
                }

                DisposeCollection(ref m_hashElementByUniqueID);
                DisposeCollection(ref m_hashElementsByName);
                DisposeCollection(ref m_hashElementByUserId);
                DisposeCollection(ref m_hashFormatsByUniqueID);

                if (m_document != null)
                {
                    m_document.RemoveAll();
                    m_document = null;
                }

                if (m_formatManager != null)
                {
                    m_formatManager.Dispose();
                    m_formatManager = null;
                }

                if (m_searcher != null)
                {
                    ClearRectSearcher();
                    m_searcher = null;
                }

                if (m_imgCache != null)
                {
                    m_imgCache.Dispose();
                    m_imgCache = null;
                }

                m_root = null;
                m_body = null;
                m_originalText = null;

                // Set flag that we were disposed.
                m_bDisposed = true;

                GC.Collect();
            }
        }

        /// <summary>
        /// Overloaded. Clears and destroys the ArrayList collection.
        /// </summary>
        /// <param name="coll">Collection to dispose.</param>
        public void DisposeCollection(ref ArrayList coll)
        {
            if (coll != null)
            {
                coll.Clear();
                coll = null;
            }
        }

        /// <summary>
        /// Clears and destroys an Hashtable collection.
        /// </summary>
        /// <param name="coll">Collection to dispose.</param>
        public void DisposeCollection(ref Hashtable coll)
        {
            if (coll != null)
            {
                coll.Clear();
                coll = null;
            }
        }

        /// <summary>
        /// Disposes control resources.
        /// </summary>
        private void DisposeControls()
        {
            if (m_userControls != null)
            {
                foreach (object ctrl in m_userControls)
                {
                    ICustomControlBase ctrlBase = ctrl as ICustomControlBase;
                    IControlImpl ctrlEx = ctrl as IControlImpl;

                    //// If element is HTHML standard control,
                    //// we must dispose it.
                    if (ctrlEx != null)
                    {
                        ctrlEx.DisposeControl();
                    }
                     else if ((ctrlBase != null) && (ctrlBase.Parent is CUSTOMElementImpl))
                    {
                        //// Element is not standard HTML control, but it was created from CUSTOM 
                        //// tag and as result we must dispose it too.
                        ctrlBase.DisposeControl();
                    }
                     else if (ctrlBase != null)
                    {
                        //// Detach events attached when control was in the document.
                        ctrlBase.DetachEvents();
                    }
                }

                DisposeCollection(ref m_userControls);
            }
        }
        #endregion

        #region  Class Public methods
        /// <summary>
        /// Returns the hash of elements by unique ID.
        /// </summary>
        /// <returns>Dictionary of elements with keys - UniqueID of elements.</returns>
        public Hashtable GetElementsByUniqueIdHash()
        {
            return m_hashElementByUniqueID;
        }

        /// <summary>
        /// Returns the hash of elements by name.
        /// </summary>
        /// <returns>Dictionary of elements with keys - names of elements.</returns>
        public Hashtable GetElementsByNameHash()
        {
            return m_hashElementsByName;
        }

        /// <summary>
        /// Returns the hash of elements by ID attribute.
        /// </summary>
        /// <returns>Dictionary of elements with keys - user ids.</returns>
        public Hashtable GetElementsByUserIdHash()
        {
            return m_hashElementByUserId;
        }

        /// <summary>
        /// Overloaded. Holds an array of formats by Unique ID of element.
        /// </summary>
        /// <returns>Dictionary of formats of element by uniqueID.</returns>
        public Hashtable GetCSSFormatsToElementHash()
        {
            if (m_formatManager != null)
            {
                if (m_hashFormatsByUniqueID.Count == 0 && m_root != null)
                {
                    m_formatManager.InFillFormatsHash(m_hashFormatsByUniqueID, m_root, true);
                }
            }

            return m_hashFormatsByUniqueID;
        }

        /// <summary>
        /// Holds an array of formats by unique ID of element.
        /// </summary>
        /// <param name="element">Element from which format calculation starts.</param>
        /// <returns>Dictionary of formats for element.</returns>
        public Hashtable GetCSSFormatsToElementHash(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (m_formatManager != null)
            {
                m_formatManager.InFillFormatsHash(m_hashFormatsByUniqueID, element, true);
            }

            return m_hashFormatsByUniqueID;
        }

        /// <summary>
        /// Returns the HTML element by its unique ID, if such exists; NULL otherwise.
        /// </summary>
        /// <param name="uniqueID">UniqueID of element.</param>
        /// <returns>Element object by its UniqueID.</returns>
        public IHTMLElement GetElementByUniqueId(string uniqueID)
        {
            if (uniqueID == null)
                throw new ArgumentNullException("uniqueID");

            if (uniqueID.Length == 0)
                throw new ArgumentException("uniqueID - string can not be empty");

            object elm = GetElementsByUniqueIdHash()[uniqueID];

            return (elm != null) ? (elm as IHTMLElement) : null;
        }

        /// <summary>
        /// Returns the HTML element by its user ID, defined in the HTML document if such exists;
        /// NULL - otherwise.
        /// </summary>
        /// <param name="userID">ID defined in HTML document.</param>
        /// <returns>Element object by its user ID.</returns>
        public IHTMLElement GetElementByUserId(string userID)
        {
            if (userID == null)
                throw new ArgumentNullException("userID");

            if (userID.Length == 0)
                throw new ArgumentException("userID - string can not be empty");

            object elm = GetElementsByUserIdHash()[userID];

            return (elm != null) ? (elm as IHTMLElement) : null;
        }

        /// <summary>
        /// Returns an array of elements with the specified tag name.
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        /// <returns>Array of elements with the specified name; NULL otherwise.</returns>
        public IHTMLElement[] GetElementsByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            Hashtable allElements = GetElementsByNameHash();

            ArrayList neededElements = allElements[name] as ArrayList;
            IHTMLElement[] result = null;

            if (neededElements != null)
            {
                result = (IHTMLElement[])neededElements.ToArray(typeof(IHTMLElement));
            }

            return result;
        }

        /// <summary>
        /// Returns the custom control by its parent tag element.
        /// </summary>
        /// <param name="parent">Parent element containing custom control.</param>
        /// <returns>Control contained by the parent element if it exists; NULL otherwise.</returns>
        public Control GetControlByElement(IHTMLElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            ICustomControlBase container = null;
            Control result = null;

            for (int i = 0, len = this.UserControls.Count; i < len; i++)
            {
                container = this.UserControls[i] as ICustomControlBase;

                if (container != null && (container.Parent == parent))
                {
                    result = container.CustomControl;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the point from client coordinates to global coordinates.
        /// </summary>
        /// <param name="point">Point in client coordinates.</param>
        /// <returns>Point in global coordinates.</returns>
        public Point ClientToGlobal(Point point)
        {
            point.X -= this.AutoScrollPosition.X;
            point.Y -= this.AutoScrollPosition.Y;

            return point;
        }

        /// <summary>
        /// Converts point from global coordinates to client coordinates.
        /// </summary>
        /// <param name="point">Point in global coordinates.</param>
        /// <returns>Point in client coordinates.</returns>
        public Point GlobalToClient(Point point)
        {
            point.X += this.AutoScrollPosition.X;
            point.Y += this.AutoScrollPosition.Y;

            return point;
        }

        /// <summary>
        /// Draws document to defined region.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="location">Start location for the drawing.</param>
        public void Draw(PaintEventArgs e, Point location)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            this.AutoScrollPosition = location;

            UpdateClientRectangle();

            m_visibleVirtualRect.Location = ClientToGlobal(m_clientRect.Location);
            ////m_visibleVirtualRect.Location = ClientToGlobal( loc );

            ProcessDraw(e);

            this.WasPainted = true;
        }

        /// <summary>
        /// Recalculates document corresponding to the defined properties.
        /// </summary>
        public void Recalculate()
        {
            if (m_body == null)
                throw new ArgumentNullException("m_body");

            // Resize client size of the document according to start location.
            this.X = this.Margins.Left;
            this.Y = this.Margins.Top;
            this.ClientWidth -= this.Margins.Left + this.Margins.Right;
            this.ClientHeight -= this.Margins.Top + this.Margins.Bottom;

            this.AutoScrollMinSize = this.ClientSize;
            m_visibleVirtualRect = this.ClientRectangle;
            this.WasPainted = false;

            if (this.ClientWidth > this.AutoScrollMinSize.Width - SystemInformation.VerticalScrollBarWidth)
                this.ClientWidth -= SystemInformation.VerticalScrollBarWidth;

            m_body.CalculateSize();
            m_body.CalculatePosition();

            Size virtualSize = this.AutoScrollMinSize;
            if (this.AutoScrollMinSize.Width > (this.ClientWidth + DEF_XINDENT))
            {
                this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Width + DEF_XINDENT, this.AutoScrollMinSize.Height);
            }

            UpdateClientRectangle();
        }

        /// <summary>
        /// Disables momentary reaction of document to some attributes changing.
        /// </summary>
        public void BeginUpdate()
        {
            this.QuietMode = true;
        }

        /// <summary>
        /// Enables momentary reaction of document to some attributes changing.
        /// </summary>
        public void EndUpdate()
        {
            this.QuietMode = false;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Raised when quiet mode property is changed.
        /// </summary>
        protected virtual void OnQuietModeChanged()
        {
            PerformChanges();
            RaiseQuietModeChangedEvent();
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raised event when quiet mode changes.
        /// </summary>
        protected void RaiseQuietModeChangedEvent()
        {
            if (QuietModeChanged != null)
            {
                QuietModeChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Recreates format for this element and recalculates the document.
        /// </summary>
        /// <param name="element">Element for recalculating.</param>
        protected internal void RecreateFormatElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            GetCSSFormatsToElementHash(element);
        }

        /// <summary>
        ///  Sets an array of compile errors.
        /// </summary>
        /// <param name="errors">Array of compile errors.</param>
        protected internal void SetCompileErrors(ArrayList errors)
        {
            m_compileErrors = errors;
        }

        /// <summary>
        /// Adds compile error message to errors storage.
        /// </summary>
        /// <param name="message">Error message to be stored.</param>
        protected internal void AddCompileError(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (m_compileErrors == null)
            {
                m_compileErrors = new ArrayList();
            }

            m_compileErrors.Add(message);
        }

        /// <summary>
        /// Removes the specified element from all collections.
        /// </summary>
        /// <param name="element">Element to be removed from the document.</param>
        protected internal void RemoveElement(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            // Remove from ElementByUniqueID hash.
            if (ElementsByUniqueID != null)
            {
                ElementsByUniqueID.Remove(element.UniqueID);
            }

            // Remove from ElementByUserID hash.
            if (ElementsByUserID != null && element.ID != null && element.ID.Length > 0)
            {
                ElementsByUserID.Remove(element.ID);
            }

            // Remove from ElementsByTagName hash.
            if (ElementsByTagName != null && ElementsByTagName.ContainsKey(element.Name))
            {
                ((ArrayList)ElementsByTagName[element.Name]).Remove(element);
            }

            // Remove from hash of formats.
            if (ElementFormatsByUniqueID != null)
            {
                ElementFormatsByUniqueID.Remove(element.UniqueID);
            }
        }

        /// <summary>
        /// Sets the loading exception.
        /// </summary>
        /// <param name="exception">Rendering exception object.</param>
        protected internal void SetError(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            m_loadException = exception;
        }

        /// <summary>
        /// Sets the root element for the document.
        /// </summary>
        /// <param name="root">IHTMLElement instance</param>
        protected internal void SetRootElement(IHTMLElement root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            m_root = root;

            if (m_root.Children.Count > 1)
            {
                m_body = m_root.Children[1];
                m_margins = new Margins(m_body as BODYElementImpl);
            }
        }

        /// <summary>
        /// Clears the Rect searcher storage.
        /// </summary>
        protected internal void ClearRectSearcher()
        {
            if (m_searcher != null)
            {
                m_searcher.Clear();
            }

            m_prvElement = null;
        }

        /// <summary>
        /// Sets the element from which drawing must be started.
        /// </summary>
        /// <param name="startElement">Element from which drawing must be started.</param>
        protected internal void SetStartDrawElement(IHTMLElement startElement)
        {
            if (startElement == null)
                throw new ArgumentNullException("startElement");

            m_startDrawElement = startElement;
        }

        /// <summary>
        /// Clears the cache of pending reactions stored as a result of some changes.
        /// </summary>
        protected internal void ClearPendingReactions()
        {
            m_reaction = ReactLevel.None;
        }

        /// <summary>
        /// Converts the original text of the document using right encoding.
        /// </summary>
        /// <param name="encoding">Encoding object</param>
        private void ConvertOriginalText(Encoding encoding)
        {
            // Convert original text of the document using right Encoding.
            if (this.OriginalText != null)
            {
                string prevText = this.OriginalText;

                byte[] buff = Encoding.Default.GetBytes(prevText);

                // Reconvert string using right encoding.
                this.OriginalText = encoding.GetString(buff);

                buff = null;
            }
        }

        /// <summary>
        /// Reacts to changes, based on changes.
        /// </summary>
        protected internal void PerformChanges()
        {
            if (this.QuietMode || m_reaction.Equals(ReactType.None)) return;

            if ((m_reaction & ReactLevel.ReFormatsCreate) > 0)
            {
                ReFormatsCreate();
            }
            else if ((m_reaction & ReactLevel.ReMergeFormats) > 0)
            {
                ReFormatsMerge();
            }
            else if ((m_reaction & ReactLevel.ReCalculateDoc) > 0)
            {
                ReCalculateDoc();
            }
            else if ((m_reaction & ReactLevel.RePaintdoc) > 0)
            {
                RePaintDoc();
            }

            ClearPendingReactions();
        }

        /// <summary>
        /// Creates all formats to elements and recalculates the document.
        /// </summary>
        private void ReFormatsCreate()
        {
            if ((m_reaction & ReactLevel.ReFormatsCreate) <= 0) return;

            GetCSSFormatsToElementHash(m_root);
            ReCalculateDoc();
        }

        /// <summary>
        /// Merges formats for all elements and recalculates the document.
        /// </summary>
        private void ReFormatsMerge()
        {
            if ((m_reaction & ReactLevel.ReMergeFormats) <= 0) return;

            if (m_body != null)
            {
                Queue queue = new Queue();
                queue.Enqueue(m_body);
                IHTMLElement tagElement = null;
                IHTMLElement child = null;

                while (queue.Count > 0)
                {
                    tagElement = queue.Dequeue() as IHTMLElement;

                    // All children of element must calculate format.
                    tagElement.CalculateFormat();

                    if (tagElement.Children.Count > 0)
                    {
                        for (int i = 0, len = tagElement.Children.Count; i < len; i++)
                        {
                            child = tagElement.Children[i];
                            queue.Enqueue(child);
                        }
                    }
                }

                ReCalculateDoc();
            }
        }

        /// <summary>
        /// Recalculates the document.
        /// </summary>
        private void ReCalculateDoc()
        {
            if ((m_reaction & ReactLevel.ReCalculateDoc) <= 0) return;

            if (m_body != null)
            {
                m_body.Control.RecalculateDocument();
            }

            RePaintDoc();
        }

        /// <summary>
        /// Repaints the control.
        /// </summary>
        private void RePaintDoc()
        {
            if ((m_reaction & ReactLevel.RePaintdoc) <= 0) return;

            if (m_body != null)
            {
                m_body.Control.Invalidate();
                Debug.WriteLine("Paint invoked");
            }
        }

        /// <summary>
        /// Starts the drawing of the document from the start element.
        /// </summary>
        /// <param name="e">Paint event data.</param>
        private void ProcessDraw(PaintEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (m_startDrawElement == null)
            {
                this.RenderRoot.DrawElement(e);
            }
            else
            {
                m_startDrawElement.DrawElement(e);

                // Reset start element.
                m_startDrawElement = null;
            }
        }

        /// <summary>
        /// Checks the location of the client rectangle.
        /// </summary>
        private void CheckLocation()
        {
            if (m_clientRect.X < 0)
            {
                m_clientRect.X = 0;
            }

            if (m_clientRect.Y < 0)
            {
                m_clientRect.Y = 0;
            }
        }

        /// <summary>
        /// Updates the client rectangle according to the document size and scroll position.
        /// </summary>
        private void UpdateClientRectangle()
        {
            Size virtualSize = this.AutoScrollMinSize;
            Point scrollPos = this.AutoScrollPosition;
            this.ClientWidth = Math.Min(this.ClientWidth, virtualSize.Width);
            this.ClientHeight = Math.Min(this.ClientHeight, virtualSize.Height);

            //// scrolling is on the left/top corner.
            if (scrollPos.IsEmpty)
            {
                this.X = this.Margins.Left;
                this.Y = this.Margins.Top;
            }
             else
            {
                //// document is scrolled.
                if (scrollPos.Y < 0)
                {
                    this.Y = Math.Max(0, this.Margins.Top + scrollPos.Y);
                }

                if (scrollPos.X < 0)
                {
                    this.X = Math.Max(0, this.Margins.Left + scrollPos.X);
                }
            }

            m_visibleVirtualRect.Height = this.ClientHeight +
              (this.Margins.Top - this.Y + this.Margins.Bottom);
            m_visibleVirtualRect.Width = this.ClientWidth +
              this.Margins.Left + this.Margins.Right;
        }

        /// <summary>
        /// Searches for root element.
        /// </summary>
        private void SearchRoot()
        {
            IHTMLElement[] elements = GetElementsByName(TagName.Html);

            if (elements != null && elements.Length > 0)
            {
                IHTMLElement elm = elements[0];
                SetRootElement(elm);
            }
        }

        /// <summary>
        /// Parses the document from html to xhtml using the specified parser.
        /// </summary>
        /// <param name="parser">Parser object.</param>
        internal void Parse(HTMLUIParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            bool bClose;
            TokenStream ts = this.DataSource.GetInputStream(out bClose);
            this.OriginalText = this.DataSource.GetDocumentContent();

            try
            {
                // Import data to document of the document.
                this.Document.RemoveAll();
                this.Document.PreserveWhitespace = true;

                XmlDocument converted = parser.Parse(ts);
                this.Document.LoadXml(converted.OuterXml);

                converted.RemoveAll();
                converted = null;
            }
            catch (XmlException xe)
            {
                Debug.WriteLine("Can't load HTML Document! Details: " +
                  xe.Message + Environment.NewLine +
                  xe.StackTrace);
                throw;
            }
            finally
            {
                if (bClose)
                {
                    ts.Close();
                }
            }

            ConvertOriginalText(parser.Encoding);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        object ICloneable.Clone()
        {
            if (this.Formats == null)
                throw new ArgumentNullException("this.Formats");

            if (this.IsDisposed)
                throw new ArgumentException("Can not create clone from disposed object.");

            FormatManager manager = new FormatManager(this.Formats.Control);

            // Copy default format from previous format manager.
            manager.SetDefaultFormat(this.Formats.DefaultFormat);
            InputHTML output = new InputHTML(manager);

            output.m_document = new XmlDocument();
            output.m_document.PreserveWhitespace = true;

            output.m_dataSource = this.DataSource;

            output.m_searcher = this.m_searcher;
            output.CurrentDirectory = this.CurrentDirectory;

            return output;
        }

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public InputHTML Clone()
        {
            return ((ICloneable)this).Clone() as InputHTML;
        }
        #endregion
    }
}
