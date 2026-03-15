#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// HTMLUIControl renders an HTML document.
    /// </summary>
    /// <remarks>
    /// The HTMLUIControl loads and renders an HTML document with the appropriate styles. The control also
    /// exposes the HTML elements as objects for programmatic manipulation.
    /// </remarks>
    [ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"), DefaultEvent("LoadFinished"), ToolboxBitmap(typeof(HTMLUIControl), "ToolBoxIcons.htmlui.bmp"), Designer(typeof(HTMLUIControlDesigner), typeof(IDesigner))]
    public class HTMLUIControl
      : ScrollableControl, ISupportInitialize
    {
        #region Class constants

        /// <summary>
        /// Default title of newly created control.
        /// </summary>
        private const string DEF_TITLE = "<Empty Document>";

        /// <summary>
        /// Name of the thread which loads the document.
        /// </summary>
        private const string DEF_THREAD_NAME = "HTML Load Thread";

        /// <summary>
        /// Name of the Title tag element.
        /// </summary>
        private const string TITLE = "title";

        /// <summary>
        /// Prefix for setting unique id to an element.
        /// </summary>
        private const string DEF_UNIQUE_ID_PREFFIX = "uniqueID";

        #endregion

        #region Class Static members
        /// <summary>
        /// Parser that parses and converts HTML document to XML document.
        /// </summary>
        private static HTMLUIParser _htmlParser = new HTMLUIParser();

        /// <summary>
        /// Parser for parsing CSS data into XML.
        /// </summary>
        private static HTMLUICSSParser _cssParser = new HTMLUICSSParser();

        /// <summary>
        /// This is not a real constant and it's value must be changed on changing
        /// Desktop settings. It is used for drawing title rectangle
        /// of document.
        /// </summary>
        private int dEF_TITLE_HEIGHT = SystemInformation.MenuHeight;

        #endregion

        #region Class members
        /// <summary>
        /// Helper class which helps to draw some GDI premitives.
        /// </summary>
        private GDIUtils m_gdi = new GDIUtils();

        /// <summary>
        /// Default Title string format.
        /// </summary>
        private StringFormat m_titleFormat = (StringFormat)GDIUtils.OneLineFormat.Clone();

        /// <summary>
        /// Builds Formats collection.
        /// </summary>
        private FormatManager m_formats;

        /// <summary>
        /// Holds all hashes of elements and formats.
        /// </summary>
        private InputHTML m_htmlDocument;

        /// <summary>
        /// Unique ID for HTML Elements.
        /// </summary>
        private long m_uniqueID;

        /// <summary>
        /// Default title of the document.
        /// </summary>
        private string m_strTitle = DEF_TITLE;

        /// <summary>
        /// True shows default margin of the document.
        /// </summary>                   
        private bool m_bNeedDefaultMargin = true;

        /// <summary>
        /// True shows title of the document.
        /// </summary>                   
        private bool m_bShowTitle = true;

        /// <summary>
        /// Variable which helps to control redrawing of control. If the value is not
        /// equal to zero, then control does not redraw itself till user
        /// forces action or EndUpdate method call.
        /// </summary>
        private int m_iCanInvalidate;

        /// <summary>
        /// Start point to draw element.
        /// </summary>
        private Point m_startPoint;

        /// <summary>
        /// Indicates if we must recalculate size and position.
        /// </summary>
        private bool m_bRecalculate;

        /// <summary>
        /// Thread for loading new document.
        /// </summary>
        private Thread m_loadThread;

        /// <summary>
        /// New Input element which works inside new thread.
        /// </summary>
        private InputHTML m_threadData;

        /// <summary>
        /// ToolTip control.
        /// </summary>
        private ToolTip m_tooltip;

        /// <summary>
        /// True if document is loading at the current moment.
        /// </summary>
        private bool m_bLoading;

        /// <summary>
        /// History of loaded document's paths.
        /// </summary>
        private History m_history;

        /// <summary>
        /// Delegate for invoking Event OnLoadFinished.
        /// </summary>
        private Delegate m_loadDelegate;

        /// <summary>
        /// Delegate for invoking Event OnLoadStarted.
        /// </summary>
        private Delegate m_loadStarted;

        /// <summary>
        /// Delegate for invoking Event LoadError.
        /// </summary>
        private Delegate m_loadError;

        /// <summary>
        /// Raises PreRenderDocument event.
        /// </summary>
        private Delegate m_prerenderEvent;

        /// <summary>
        /// Path to startup folder for current document in the control.
        /// </summary>
        private string m_startupFolder;

        /// <summary>
        /// Path to current document, loaded in the control.
        /// </summary>
        private string m_startupDocument;

        /// <summary>
        /// Clicked mouse button.
        /// </summary>
        private MouseButtons m_clickedButton;

        /// <summary>
        /// Indicates where mouse up event occurs.
        /// </summary>
        private Point m_clickedLocation;

        /// <summary>
        /// Collection of script collections.
        /// </summary>
        private ScriptManagerExCollection m_scripts;

        /// <summary>
        /// Indicates whether initialization of control has been done.
        /// </summary>
        private bool m_binitDone;

        /// <summary>
        /// Indicates whether to run script just after loading document.
        /// </summary>
        private bool m_bAutoRunScripts;

        /// <summary>
        /// Indicates whether a separate thread is used by the control for document loading.
        /// </summary>
        private bool m_bEnableMultithreading;

        /// <summary>
        /// Controls all selection in the control.
        /// </summary>
        private SelectionManager m_selectionManager;

        /// <summary>
        /// Indicates whether the original text of the document will be saved.
        /// </summary>
        private bool m_bEnableTextCache = true;

        /// <summary>
        /// Indicates whether the HTMLUI image size should be set according to the width of the image
        /// </summary>
        private bool m_bSizetoFit = true;
        #endregion

        #region Class Static properties
        /// <summary>
        ///  Gets an instance of the HTMLUIParser.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static HTMLUIParser HTMLParser
        {
            get
            {
                return _htmlParser;
            }
        }

        /// <summary>
        ///  Gets an instance of the LiteCSSParser.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static HTMLUICSSParser CSSParser
        {
            get
            {
                return _cssParser;
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets an instance of the Format manager.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        public FormatManager FormatManager
        {
            get
            {
                // Must wait while variables would be reassigned (when thread is working).
                lock (m_loadDelegate)
                {
                    return m_formats;
                }
            }
        }

        /// <summary>
        /// Gets an access to default format used as base for document rendering.
        /// </summary>
        [Category("Appearance"), Browsable(true), DefaultValue(null), TypeConverter(typeof(ExpandableObjectConverter)), Description("Get access to default format used as base for document rendering."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Syncfusion.Documentation.DocumentationExclude()]
        public IHTMLFormat DefaultFormat
        {
            get
            {
                return this.FormatManager.DefaultFormat;
            }
        }

        /// <summary>
        /// Gets the parsed and displayed document.
        /// </summary>
        /// <remarks>
        /// The <see cref="IInputHTML"/> document defines the various elements of the HTML document
        /// loaded into the <see cref="HTMLUIControl"/>. The document provides access to all the elements in the
        /// HTML document.
        /// </remarks>
        [Category("Data"), RefreshProperties(RefreshProperties.All), Browsable(true), ReadOnly(true), DefaultValue(null), Description("Get information about parsed and shown document.")]
        public IInputHTML Document
        {
            get
            {
                // Wait while new document would be loaded and variables would be reassigned.
                lock (m_loadDelegate)
                {
                    return m_htmlDocument;
                }
            }
        }

        /// <summary>
        /// Gets or sets the HTML rendered by the <see cref="HTMLUIControl"/>.
        /// </summary>
        /// <remarks>
        /// Any text set to this property will be parsed by the control and rendered. 
        /// If <see cref="IsOriginalTextCached"/> property is set to true and document 
        /// is loaded from this property or from local file using 
        /// method, this property will return original data         /// entered for loading; Otherwise, this property will 
        /// return <see cref="DisplayText"/> property content.
        /// </remarks>
        [Category("Appearance"), RefreshProperties(RefreshProperties.All), Browsable(true), Bindable(BindableSupport.Yes), Localizable(true), Description("Gets or sets the HTML rendered.")]
        public override string Text
        {
            get
            {
                string text;

                if (this.DocumentEx != null)
                {
                    string originalText = this.DocumentEx.OriginalText;
                    bool showOriginal = this.IsOriginalTextCached && originalText != null && originalText.Length > 0;
                    text = showOriginal ? originalText : this.DisplayText;
                }
                else
                {
                    text = string.Empty;
                }

                return text;
            }
            set
            {
                bool bParse = true;

                // If we try to set property to the same value skip parsing
                // and simply Refresh document view.
                if (this.Document != null)
                {
                    if (value != null && value.Length != 0)
                    {
                        bParse = this.Document.Document.InnerXml.GetHashCode() != value.GetHashCode();
                    }
                }

                if (bParse)
                {
                    this.StartupDocument = string.Empty;

                    // Load and render data.
                    LoadFromString(value);

                    // Raise event to user TextChanged.
                    OnTextChanged(EventArgs.Empty);
                }
                else
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets the internal appearance of the document's HTML data.
        /// </summary>
        [Browsable(false)]
        public string DisplayText
        {
            get
            {
                if (this.Document != null)
                {
                    return this.Document.Document.InnerXml;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the Title of the HTMLUIControl / the document loaded into the control.
        /// </summary>
        /// <remarks>
        /// The Title value can be set explicitly or this is auto extracted from the Title tag of the HTML document.
        /// </remarks>
        [Category("Appearance"), RefreshProperties(RefreshProperties.All), Browsable(true), DefaultValue(DEF_TITLE), Bindable(BindableSupport.Yes), Localizable(true), Description("Gets or sets document title.")]
        public virtual string Title
        {
            get
            {
                if (this.Document != null)
                {
                    ArrayList list = (ArrayList)this.DocumentEx.ElementsByTagName[TITLE];
                    if (list != null && list.Count > 0)
                    {
                        m_strTitle = ((BaseElement)list[0]).Storage.InnerText;
                    }
                }

                return m_strTitle;
            }
            set
            {
                if (value != m_strTitle)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_strTitle, value);
                    m_strTitle = value;
                    OnTitleChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default margin is needed.
        /// </summary>
        [Category("Appearance"),
         DefaultValue(true),
        Description("Indicates whether the default margin is needed.")]
        public bool NeedDefaultMargin
        {
            get
            {
                return m_bNeedDefaultMargin;
            }
            set
            {
                if (value != m_bNeedDefaultMargin)
                {
                    m_bNeedDefaultMargin = value;
                    if (this.DocumentEx != null)
                    {
                        RefreshStartPoint(this.DocumentEx);
                    }

                    RecalculateDocument();
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="HTMLUIControl.Text"/> will be displayed.
        /// </summary>
        [Category("Appearance"), RefreshProperties(RefreshProperties.Repaint), Browsable(true), DefaultValue(true), Localizable(true), Description("Show or hide document title")]
        public virtual bool ShowTitle
        {
            get
            {
                return m_bShowTitle;
            }
            set
            {
                if (value != m_bShowTitle)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_bShowTitle, value);
                    m_bShowTitle = value;

                    m_startPoint.Y = value ? dEF_TITLE_HEIGHT : 0;

                    if (this.DocumentEx != null)
                    {
                        RefreshStartPoint(this.DocumentEx);
                    }

                    OnShowTitleChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets the history of the documents loaded by the control in this session.
        /// </summary>
        [Browsable(false), ReadOnly(true), Description("History of the loaded by control documents.")]
        public History History
        {
            get
            {
                return m_history;
            }
        }

        /// <summary>
        /// Gets or sets the path to the start up document for the control.
        /// </summary>
        /// <remarks>The startup document is the document that will be loaded by the HTMLUIControl initially.</remarks>
        [Category("Data"), RefreshProperties(RefreshProperties.All), Browsable(true), DefaultValue(typeof(string), ""), Localizable(true), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Description("Gets or sets path to HTML document which would be loaded just after control loading.")]
        public string StartupDocument
        {
            get
            {
                return m_startupDocument;   
            }
            set
            {
                if (m_startupDocument != value)
                {
                    if (value != null && value.Length > 0 && File.Exists(value))
                    {
                        m_startupDocument = value;
                        this.StartupFolder = Path.GetDirectoryName(Path.GetFullPath(value));

                        if ( /*this.DesignMode ||*/ m_binitDone)
                        {
                            LoadHTML(value);
                        }
                    }
                    else
                    {
                        m_startupDocument = string.Empty;
                        this.StartupFolder = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the startup folder for the start up document of the control.
        /// </summary>
        [Category("Data"), RefreshProperties(RefreshProperties.All), Browsable(true), DefaultValue(typeof(string), ""), Localizable(true), Description("Gets or sets start up path for control.")]
        public string StartupFolder
        {
            get
            {
                return m_startupFolder;
            }
            set
            {
                if (value != m_startupFolder)
                {
                    m_startupFolder = value;
                }
            }
        }

        /// <summary>
        /// Gets the point where the mouse was clicked.
        /// </summary>
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public Point ClickedPoint
        {
            get
            {
                return m_clickedLocation;
            }
        }

        /// <summary>
        /// Gets the collection of scripts in the document.
        /// </summary>
        [Category("Data"), Browsable(true), DefaultValue(null), Editor(typeof(ScriptObjectCollectionEditor), typeof(UITypeEditor)), Description("Gets collection of the scripts in the document.")]
        public ScriptManagerExCollection Scripts
        {
            get
            {
                return m_scripts;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scripts in the body of the loaded HTML document must be executed.
        /// </summary>
        [Category("Data"), Browsable(true), DefaultValue(false), Description("Specifies if scripts in BODY tag must be executed just after document loading.")]
        public bool AutoRunScripts
        {
            get
            {
                return m_bAutoRunScripts;
            }
            set
            {
                if (m_bAutoRunScripts != value)
                {
                    m_bAutoRunScripts = value;
                }
            }
        }

        /// <summary>
        /// Specifies that the size of the image on a HTMLUI that automatically gets adjust to fit on a HTMLUI.
        /// </summary>
        [Category("Appearance"), Browsable(true), DefaultValue(true), Description("Specifies that the size of the image on a HTMLUI that automatically gets adjust to fit on a HTMLUI.")]
        public bool SizeToFit
        {
            get
            {
                return m_bSizetoFit;
            }
            set
            {
                if (m_bSizetoFit != value)
                {
                    m_bSizetoFit = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether a separate thread is used for document loading.
        /// </summary>
        [Browsable(false), DefaultValue(false)]
        public bool EnableMultithreading
        {
            get
            {
                return m_bEnableMultithreading;
            }
            set
            {
                if (m_bEnableMultithreading != value)
                {
                    m_bEnableMultithreading = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the original text of the document will be stored.
        /// </summary>
        [Browsable(false), DefaultValue(true)]
        public bool IsOriginalTextCached
        {
            get
            {
                return m_bEnableTextCache;
            }
            set
            {
                if (m_bEnableTextCache != value)
                {
                    m_bEnableTextCache = value;

                    if (!m_bEnableTextCache && this.ThreadDocument != null)
                    {
                        this.ThreadDocument.OriginalText = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected text displayed in the control.
        /// </summary>
        [Browsable(false)]
        public string SelectedText
        {
            get
            {
                return this.SelectionManager.SelectedText;
            }
        }

        /// <summary>
        /// Gets an array of selected elements in the document.
        /// </summary>
        [Browsable(false)]
        public IHTMLElement[] SelectedElements
        {
            get
            {
                ArrayList selectedElements = this.SelectionManager.SelectedElements;
                return (IHTMLElement[])selectedElements.ToArray(typeof(IHTMLElement));
            }
        }

        /// <summary>
        /// Gets of data while data is loading.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Category("Data"), ReadOnly(true), Browsable(false), DefaultValue(null), Description("Reference on document which we started to load but not finished yet yet.")]
        protected internal InputHTML ThreadDocument
        {
            get
            {
                return m_threadData;
            }
        }

        /// <summary>
        /// Gets the ToolTip control.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal ToolTip ToolTip
        {
            get
            {
                if (m_tooltip == null)
                {
                    m_tooltip = new ToolTip();
                }

                return m_tooltip;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document is loading.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool IsLoading
        {
            get
            {
                return m_bLoading;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the size and position must be recalculated.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool Recalculate
        {
            get
            {
                return m_bRecalculate;
            }
            set
            {
                if (m_bRecalculate != value)
                {
                    m_bRecalculate = value;
                }
            }
        }

        /// <summary>
        /// Gets the increase in height needed to accommodate the title.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal int Increase
        {
            get
            {
                return this.ShowTitle ? dEF_TITLE_HEIGHT : 0;
            }
        }

        /// <summary>
        /// Gets the clicked mouse button.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal MouseButtons ClickedButton
        {
            get
            {
                return m_clickedButton;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal bool InDesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }

        /// <summary>
        /// Gets the document property typed to InputHTML type.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal InputHTML DocumentEx
        {
            get
            {
                return this.Document as InputHTML;
            }
        }

        /// <summary>
        /// Gets the Selection manager that controls selection in the document.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal SelectionManager SelectionManager
        {
            get
            {
                return m_selectionManager;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Event that is to be raised after the <see cref="HTMLUIControl.Title"/> property is changed.
        /// </summary>
        [Description(" Event raised after the Title property is changed"), Category("Property Changed")]
        public event ValueChangedEventHandler TitleChanged;

        /// <summary>
        /// Event that is to be raised after the <see cref="HTMLUIControl.ShowTitle"/> property is changed.
        /// </summary>
        [Description(" Event raised after the ShowTitle property is changed"), Category("Property Changed")]
        public event ValueChangedEventHandler ShowTitleChanged;

        /// <summary>
        /// Event that is to be raised after the HTML document is loaded.
        /// </summary>
        [Category("Data"), Description("Event raised after the HTML document is loaded.")]
        public event EventHandler LoadFinished;

        /// <summary>
        /// Event that is to be raised when loading of a new HTML document has started.
        /// </summary>
        [Category("Data"), Description("Event raised when loading of a new HTML document has started.")]
        public event EventHandler LoadStarted;

        /// <summary>
        /// Event that is to be raised when an error occurs during HTML document loading / rendering.
        /// </summary>
        [Category("Data"), Description("Event raised when an error occurs during HTML document loading / rendering.")]
        public event LoadErrorEventHandler LoadError;

        /// <summary>
        /// Event that is to be raised when a tree of elements has been created, but their size
        /// and location have not been calculated yet.
        /// </summary>
        [Category("Data"), Description("Event raised when a tree of elements has been created, but their size and location have not been calculated yet.")]
        public event PreRenderDocumentEventHandler PreRenderDocument;

        /// <summary>
        /// Event that is to be raised after the hyperlink was clicked and before the hyperlink tries
        /// to load a new resource.
        /// </summary>
        [Category("Data"), Description("Event raised after the hyperlink was clicked and before the hyperlink tries to load a new resource.")]
        public event LinkForwardEventHandler LinkClicked;
        #endregion

        #region Class initialize/finalize methods
        /// <overload>
        /// Overloaded. Initializes a new <see cref="HTMLUIControl"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLUIControl"/> class.
        /// </summary>
        public HTMLUIControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(HTMLUIControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // Create Format Manager.
            m_formats = new FormatManager(this);

            // Title alignment (for future change according to Accessories settings).
            m_titleFormat.LineAlignment = StringAlignment.Near;
            m_titleFormat.Alignment = StringAlignment.Near;

            // Attach event to system changes.
            this.SystemColorsChanged += new EventHandler(OnSystemColorsChanged);

            // Set control styles.
            ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
              ControlStyles.DoubleBuffer |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserMouse |
              ControlStyles.ContainerControl |
              ControlStyles.Selectable |
              ControlStyles.UserPaint;

            ControlStyles styleFalse = ControlStyles.CacheText |
              ControlStyles.FixedHeight |
              ControlStyles.FixedWidth |
              ControlStyles.Opaque;

            SetStyle(styleTrue, true);
            SetStyle(styleFalse, false);

            ////SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.TabStop = true;

            m_startPoint = new Point(0, 0);
            m_bRecalculate = true;

            m_tooltip = new ToolTip();
            m_tooltip.AutoPopDelay = 5000;
            m_tooltip.InitialDelay = 700;
            m_tooltip.ReshowDelay = 500;

            m_loadDelegate = (Delegate)new EventHandler(OnLoadFinished);
            m_loadStarted = (Delegate)new EventHandler(OnLoadStarted);
            m_loadError = (Delegate)new LoadErrorEventHandler(OnLoadError);
            m_prerenderEvent = (Delegate)new PreRenderDocumentEventHandler(OnPreRenderDocument);

            m_history = new History();

            m_clickedButton = Control.MouseButtons;
            m_clickedLocation = Point.Empty;

            m_scripts = new ScriptManagerExCollection(this);
            m_selectionManager = new SelectionManager(this);

#if DEBUG
            if (Thread.CurrentThread.Name == null ||
              Thread.CurrentThread.Name.Length == 0)
            {
                Thread.CurrentThread.Name = "Main Thread";
            }
#endif
        }

        /// <summary>
        /// Override Dispose
        /// </summary>    
        /// <param name="disposing">bool variable</param>
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (m_loadThread != null)
                {
                    AbortLoadThread();
                    m_loadThread = null;
                }

                if (m_threadData != null)
                {
                    Dispose(ref m_threadData);
                }

                if (m_htmlDocument != null)
                {
                    Dispose(ref m_htmlDocument);
                }

                if (m_history != null)
                {
                    m_history.Dispose();
                    m_history = null;
                }

                if (m_scripts != null)
                {
                    m_scripts.Dispose();
                    m_scripts = null;
                }

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Disposes all previous <see cref="HTMLUIControl.Document"/> after a new document has been loaded.
        /// </summary>
        /// <param name="document">Document being disposed.</param>
        private void Dispose(ref InputHTML document)
        {
            if (document != null)
            {
                document.Dispose();
                document = null;
            }
        }
        #endregion

        #region Class Static Methods
        /// <summary>
        /// Converts the EventArgs object to BubblingEventArgs type if possible.
        /// </summary>
        /// <param name="args">EventArgs object.</param>
        /// <returns>BubblingEventArgs object if converting was successful,
        /// Null otherwise.</returns>
        public static BubblingEventArgs GetBublingEventArgs(EventArgs args)
        {
            return args as BubblingEventArgs;
        }
        #endregion

        #region Public methods
        /// <overload>
        /// Overloaded. Loads the specified HTML file into the  <see cref="HTMLUIControl"/> and renders it.
        /// </overload>
        /// <summary>
        /// Loads the specified HTML file into the  <see cref="HTMLUIControl"/> and renders it.
        /// </summary>
        /// <param name="fileName">Source file name.</param>
        public virtual void LoadHTML(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.Length == 0)
                throw new ArgumentException("fileName - string can not be empty");

            if (!File.Exists(Utilities.RemoveBookmark(fileName)))
                throw new IOException("File not found. File name is: " + fileName);

            InputHTML document = new InputHTML(fileName, this.FormatManager);

            ParseDocument(document);
        }

        /// <overload>
        /// Loads the specified HTML document from a <see cref="System.IO.Stream"/> into the  <see cref="HTMLUIControl"/> and renders it.
        /// </overload>
        /// <summary>
        /// Loads the specified HTML document from a and renders it.
        /// </summary>
        /// <param name="stream">Input stream reference.</param>
        /// <remarks>A stream passing to this method must support Seek operation.</remarks>
        public virtual void LoadHTML(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            ParseDocument(new InputHTML(stream, this.FormatManager));
        }

        /// <overload>
        /// Loads the specified HTML document from a <see cref="System.Uri"/> into the  <see cref="HTMLUIControl"/> and renders it.
        /// </overload>
        /// <summary>
        /// Loads the specified HTML document from a <see cref="System.Uri"/> into the  <see cref="HTMLUIControl"/> and renders it.
        /// </summary>
        /// <param name="uri">Source file name by URI.</param>
        public virtual void LoadHTML(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            InputHTML document = new InputHTML(uri, this.FormatManager);

            ParseDocument(document);
        }

        /// <overload>
        /// Overloaded. Loads the styles from the specified CSS file and refreshes current document using the styles.
        /// </overload>
        /// <summary>
        /// Loads styles from the specified CSS file and refreshes current document using the styles.
        /// </summary>
        /// <param name="fileName">Input file with CSS styles.</param>
        public virtual void LoadCSS(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.Length == 0)
                throw new ArgumentException("fileName - string can not be empty");

            this.FormatManager.LoadCss(fileName);

            // Update control (recalculate formats and document).
            if (this.Document.RenderRoot != null)
            {
                (this.Document.RenderRoot as BaseElement).ReFormatCrtDocAndReCalcDoc(null);
            }
        }

        /// <summary>
        /// Updates the specified CSS style in the form of string to the HTML document.
        /// </summary>
        /// <param name="style">The CSS style as string.</param>
        public virtual void LoadCSSFromString(string style)
        {
            if (style == null)
                throw new ArgumentNullException("string");

            if (style.Length == 0)
                throw new ArgumentException("style - string can not be empty");

            Stream str = Utilities.StreamFromString(style);
            LoadCSS(str);
            str.Close();
        }

        /// <overload>
        /// Loads styles from the specified CSS document from a <see cref="System.IO.Stream"/> and refreshes current document using the styles.
        /// </overload>
        /// <summary>
        /// Loads styles from the specified CSS document from a <see cref="System.IO.Stream"/> and refreshes current document using the styles.
        /// </summary>
        /// <param name="stream">Input stream with CSS styles.</param>
        /// <remarks>A stream passing to this method must support Seek operation.</remarks>
        public virtual void LoadCSS(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            this.FormatManager.LoadCss(stream);

            // Update control (recalculate formats and document).
            if (this.Document.RenderRoot != null)
            {
                (this.Document.RenderRoot as BaseElement).ReFormatCrtDocAndReCalcDoc(null);
            }
        }

        /// <summary>
        /// Loads HTML code from string.
        /// </summary>
        /// <param name="html">Html file</param>
        public virtual void LoadFromString(string html)
        {
            InputHTML document = InputHTML.FromString(html, this.FormatManager);

            // Set original text to the document.
            SetOriginalText(document, html);

            // Parse document.
            ParseDocument(document);
        }

        /// <summary>
        /// Select all the text in the control.
        /// </summary>
        public virtual void SelectAll()
        {
            //// Select all text in the control
            this.SelectionManager.SelectAllText();
        }

        /// <summary>
        /// Stops redrawing the control until the call of EndUpdate method.
        /// </summary>
        public virtual void BeginUpdate()
        {
            m_iCanInvalidate++;

            //// NOTE: Prevent redrawing of control.
            //// It is required for children controls in the control.
			if(this.AutoScroll)
				NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, 0, 0);

            if (this.ThreadDocument != null)
            {
                this.ThreadDocument.BeginUpdate();
            }
        }

        /// <summary>
        /// Continues redrawing the control after the previous BeginUpdate method call.
        /// </summary>
        public virtual void EndUpdate()
        {
            if (this.ThreadDocument != null)
            {
                this.ThreadDocument.EndUpdate();
            }

            m_iCanInvalidate--;

            if (m_iCanInvalidate <= 0)
            {
                m_iCanInvalidate = 0;

                //// NOTE: Unlock redrawing of control.
                //// Invalidate control with invalidating children controls.
				if(this.AutoScroll)
					NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                OnLayout(new LayoutEventArgs(null, null));
                Invalidate(true);
            }
        }

        /// <summary>
        /// Overloaded. Returns the tag element which contains the specified point.
        /// </summary>
        /// <param name="point">Coordinates are expressed relative to the upper-left corner
        /// of the control's client area. </param>
        /// <returns>Element which contains the point.</returns>
        public IHTMLElement GetElementAtClientPoint(Point point)
        {
            return GetElementAtClientPoint(point.X, point.Y);
        }

        /// <summary>
        /// Returns the tag element which contains the specified point.
        /// </summary>
        /// <param name="x">X coordinate of the point expressed relative to left corner of the control client area.</param>
        /// <param name="y">Y coordinate of the point expressed relative to left corner of the control client area.</param>
        /// <returns>Element which contains the point.</returns>
        public IHTMLElement GetElementAtClientPoint(int x, int y)
        {
            IHTMLElement element = null;

            if (this.Document != null && !this.DocumentEx.Searcher.IsEmpty())
            {
                Size baseSize = base.ClientSize;
                bool ptInRgn =
                  (x > 0) &&
                  (y > 0) &&
                  (x < baseSize.Width) &&
                  (y < baseSize.Height);

                if (!ptInRgn) return null;

                Point point = new Point(x - this.DocumentEx.AutoScrollPosition.X, y - this.DocumentEx.AutoScrollPosition.Y);

                // If point lies in title.
                if (PointInTitle(point))
                {
                    return GetTitleElement();
                }

                element = this.DocumentEx.Searcher.GetElement(point);
            }
            return element;
        }

        /// <summary>
        /// Returns the tag element which contains the specified point.
        /// </summary>
        /// <param name="point">Coordinates are expressed relative to the upper-left corner
        /// of the control's virtual rectangle. </param>
        /// <returns>Element which contains the point.</returns>
        public IHTMLElement GetElementAtVirtualPoint(Point point)
        {
            return GetElementAtVirtualPoint(point.X, point.Y);
        }

        /// <summary>
        /// Returns the tag element which contains the point with defined coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the point expressed relative to the left corner of the control virtual rectangle.</param>
        /// <param name="y">Y coordinate of the point expressed relative to the left corner of the control virtual rectangle.</param>
        /// <returns>Element by coordinates.</returns>
        public IHTMLElement GetElementAtVirtualPoint(int x, int y)
        {
            IHTMLElement element = null;

            if (this.Document != null && !this.DocumentEx.Searcher.IsEmpty())
            {
                bool ptInRgn =
                  (x > 0) &&
                  (y > 0) &&
                  (x < this.DocumentEx.VisibleRectangle.Right) &&
                  (y < this.DocumentEx.VisibleRectangle.Bottom);

                if (!ptInRgn) return null;

                Point point = new Point(x, y);

                // If point lies in title.
                if (PointInTitle(point))
                {
                    return GetTitleElement();
                }

                element = this.DocumentEx.Searcher.GetElement(point);
            }

            return element;
        }

        /// <summary>
        /// Converts client coordinates to virtual coordinates of control.
        /// </summary>
        /// <param name="point">Point coordinates in client coordinates.</param>
        /// <returns>Coordinates in global coordinates.</returns>
        public Point ClientToVirtual(Point point)
        {
            point.X -= this.AutoScrollPosition.X;
            point.Y -= this.AutoScrollPosition.Y;

            return point;
        }

        /// <summary>
        /// Converts virtual coordinates to client coordinates of control.
        /// </summary>
        /// <param name="point">Coordinates in global coordinates.</param>
        /// <returns>Coordinates in client coordinates.</returns>
        public Point VirtualToClient(Point point)
        {
            point.X += this.AutoScrollPosition.X;
            point.Y += this.AutoScrollPosition.Y;

            return point;
        }

        /// <overloads>
        /// Overloaded. Loads the previous document from history.
        /// </overloads>
        /// <summary>
        /// Loads the previous document from history.
        /// </summary>
        public void Back()
        {
            Back(1);
        }

        /// <overloads>
        /// Loads the document from history according to the index.
        /// </overloads>
        /// <summary>
        /// Loads the document from history according to the index.
        /// </summary>
        /// <param name="index">Index of moving in the history. Must be less then zero.</param>
        public void Back(int index)
        {
            if (index > 0)
                throw new ArgumentOutOfRangeException("index", "index is nonnegative number");

            LoadFromHistory(index);
        }

        /// <overloads>
        /// Overloaded. Loads next document from history.
        /// </overloads>
        /// <summary>
        /// Loads next document from history.
        /// </summary>
        public void Forward()
        {
            Forward(1);
        }

        /// <overloads>
        /// Loads the document from history according to the index.
        /// </overloads>
        /// <summary>
        /// Loads the document from history according to the index.
        /// </summary>
        /// <param name="index">Index of moving in the history. Must be greater then zero.</param>
        public void Forward(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "index is negative number");

            LoadFromHistory(index);
        }

        /// <summary>
        /// Prepares document object. Parses and renders it.
        /// It may be used for non-visual rendering. It does not assign controls to any
        /// properties such as a document.
        /// </summary>
        /// <param name="document">Object containing HTML data.</param>
        /// <remarks>This method is suitable for rendering documents if
        /// HTMLUI control is used without graphic interface.</remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void PrepareDocument(IInputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            PrepareDocument(document, true);
        }

        /// <summary>
        /// Prepares document object.
        /// It may be used for non-visual rendering. It does not assign controls to any
        /// properties such as a document.
        /// </summary>
        /// <param name="document">Object containing HTML data.</param>
        /// <param name="layout">If true - layouts elements, otherwise - just parses and creates elements.</param>
        /// <remarks>This method is suitable for rendering documents if
        /// HTMLUI control is used without graphic interface.</remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void PrepareDocument(IInputHTML document, bool layout)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // NOTE: this method is not thread safe. Needs to be refactored for making 
            // thread safe it.
            InputHTML docEx = document as InputHTML;

            if (docEx != null)
            {
                try
                {
                    m_threadData = docEx;

                    // Parse document.
                    m_threadData.Parse(HTMLParser);

                    // Reset original text if no need to save data.
                    if (!this.IsOriginalTextCached)
                    {
                        m_threadData.OriginalText = null;
                    }

                    docEx.SetRootElement(ConvertDocument(m_threadData.Document.DocumentElement, null, m_threadData));
                    m_threadData.GetCSSFormatsToElementHash();

                    // Calculate size and location of the elements inside of the document.
                    if (layout)
                    {
                        m_threadData.Recalculate();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    // Raise event notification about render error.
                    m_threadData.SetError(ex);
                    LoadErrorEventArgs args = new LoadErrorEventArgs(m_threadData);

                    object[] arguments = new object[] { this, args };

                    if (!this.IsHandleCreated)
                    {
                        this.CreateControl();
                    }

                    this.Invoke(m_loadError, arguments);
                }
                finally
                {
                    m_threadData = null;
                }
            }
        }

        /// <summary>
        /// Displays the find form for searching the text content of the HTMLUI control's current document.
        /// </summary>
        public void DisplayFindForm()
        {
            this.SelectionManager.ProcessTextSearch();
        }

        /// <summary>
        /// Scrolls control in such way that the specified element is visible.
        /// </summary>
        /// <param name="element">Tag Element.</param>
        public void ScrollToElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            BaseElement elementEx = element as BaseElement;

            if (this.AutoScroll &&
              elementEx.Document == this.ThreadDocument &&
              element.IsVisible && elementEx.Blocks.Count > 0)
            {
                Rectangle elmRect = GetElementScrollRectangle(elementEx);
                InputHTML document = elementEx.Document;
                ScrollToRectangle(elmRect, document);
            }
        }

        /// <summary>
        /// Scrolls control to the specified rectangle. Tries to show the whole rectangle in the
        /// client area.
        /// </summary>
        /// <param name="rect">Rectangle structure.</param>
        /// <param name="document">Document in which this rectangle exists.</param>
        public void ScrollToRectangle(Rectangle rect, IInputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            InputHTML documentEx = document as InputHTML;

            Point startPoint = new Point(DocumentEx.Margins.Left, DocumentEx.Margins.Top);
            Rectangle virtualRect = new Rectangle(startPoint, documentEx.AutoScrollMinSize);

            if (this.AutoScroll && virtualRect.Contains(rect))
            {
                Point curScrollPoint = this.AutoScrollPosition;
                curScrollPoint.X = Math.Abs(curScrollPoint.X);
                curScrollPoint.Y = Math.Abs(curScrollPoint.Y);
                Point newScrollPoint = curScrollPoint;

                // Element is not visible in the control.
                if (!documentEx.VisibleRectangle.Contains(rect))
                {
                    // Search new Y position.
                    if (rect.Y < curScrollPoint.Y ||
                      rect.Height > documentEx.ClientRectangle.Height)
                    {
                        newScrollPoint.Y = rect.Y;
                    }
                    else
                    {
                        // element is under visible area and can be contained by it.
                        int newYCoord = rect.Bottom - documentEx.ClientRectangle.Height;
                        newScrollPoint.Y = newYCoord;
                    }

                    // Search new X position.
                    if (rect.X < curScrollPoint.X ||
                      rect.Width > documentEx.ClientRectangle.Width)
                    {
                        newScrollPoint.X = rect.X;
                    }
                    else
                    {
                        int newXCoord = rect.Right - documentEx.ClientRectangle.Width;
                        newScrollPoint.X = newXCoord;
                    }
                }

                // Scroll window.
                if (newScrollPoint != curScrollPoint)
                {
                    SetAutoScrollPosition(newScrollPoint, DocumentEx);
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Parses input HTML document and converts it to an XML document
        /// with additional HTML elements objects tree.
        /// </summary>
        /// <param name="document">Document to parse.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual void ParseDocument(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // Prepare thread for new document loading.
            DetachUserControls();

            if (this.EnableMultithreading)
            {
                AbortLoadThread();

                if (!this.IsHandleCreated)
                {
                    this.CreateHandle();
                }

                m_loadThread = new Thread(new ThreadStart(this.LoadThread));
                m_loadThread.Priority = ThreadPriority.AboveNormal;
                m_loadThread.IsBackground = true;
            }

            // Set flag that loading started - this will tell OnPaint method to skip
            // drawing of old document.
            m_bLoading = true;
            m_uniqueID = 0;

            m_threadData = document.Clone();

            SetBeforeCalculating(m_threadData);

            // Start document loading.
            if (this.EnableMultithreading)
            {
                // If we are in the designer, then do not start thread?
                m_loadThread.Start();
            }
            else
            {
                LoadThread();
            }
        }

        /// <summary>
        /// Raises the <see cref="HTMLUIControl.TitleChanged"/> event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnTitleChanged(ValueChangedEventArgs args)
        {
            RaiseTitleChanged(args);

            if (this.Document != null)
            {
                ArrayList list = (ArrayList)this.DocumentEx.ElementsByTagName["title"];

                if (list != null && list.Count > 0)
                {
                    ((BaseElement)list[0]).InnerHTML = (string)args.NewValue;
                }
                else
                {
                    this.Text = "<title>" + (string)args.NewValue + "</title>" + this.Text;
                }
            }

            Refresh();
        }

        /// <summary>
        /// Raises the <see cref="HTMLUIControl.ShowTitleChanged"/> event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnShowTitleChanged(ValueChangedEventArgs args)
        {
            RaiseShowTitleChanged(args);

            RecalculateDocument();
            Refresh();
        }

        /// <summary>
        /// Triggers when System colors are changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">EventArgs instance</param>
        /// <copyfrom cref="Control.OnSystemColorsChanged"/>
        protected virtual void OnSystemColorsChanged(object sender, EventArgs e)
        {
            dEF_TITLE_HEIGHT = SystemInformation.MenuHeight;

            this.Invalidate();
        }

        /// <summary>
        /// Called after a document has been loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        protected virtual void OnLoadFinished(object sender, EventArgs e)
        {
            string bookmark = null;

            // Reassign variables which hold documents.
            lock (m_loadDelegate)
            {
                bool bDiferent = m_htmlDocument != m_threadData;

                if (bDiferent)
                {
                    Dispose(ref m_htmlDocument);
                    m_htmlDocument = m_threadData;
                    InfillScriptCollection(m_threadData);
                    AttachUserControls();

                    SetAfterCalculating(m_threadData);

                    if (m_threadData != null)
                    {
                        if (m_formats != null)
                        {
                            m_formats.Dispose();
                            m_formats = null;
                        }

                        m_formats = m_threadData.Formats;
                    }

                    // Store in history URL/File path of loaded document.
                    if (m_htmlDocument != null && m_htmlDocument.IsFileName)
                    {
                        m_history.Push(m_htmlDocument.FileName, PathType.File);

                        // Search if scrolling to some fragment is needed inside document.
                        bookmark = Utilities.GetBookmark(m_htmlDocument.FileName);
                    }
                    else if (m_htmlDocument != null && m_htmlDocument.IsUri)
                    {
                        m_history.Push(m_htmlDocument.Uri.ToString(), PathType.Uri);

                        // Search if scrolling to some fragment is needed inside document.
                        bookmark = m_htmlDocument.Uri.Fragment;
                    }

                    RaiseLoadFinished(e);
                    RunAutoScripts();
                }
            }

            m_bLoading = false;
            EndUpdate();

            //// Scroll to bookmark.
            if (bookmark != null && bookmark.Length > 0)
            {
                JumpToFragment(bookmark, m_htmlDocument);
            }
        }

        /// <summary>
        /// Called after a document loading has started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        protected virtual void OnLoadStarted(object sender, EventArgs e)
        {
            m_bLoading = true;
            BeginUpdate();

            RaiseLoadStarted(e);
        }

        /// <summary>
        /// Raises the <see cref="HTMLUIControl.PreRenderDocument"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        protected virtual void OnPreRenderDocument(object sender, PreRenderDocumentArgs args)
        {
            RaisePreRenderDocument(args);
        }

        /// <summary>
        /// Raises the <see cref="HTMLUIControl.LoadError"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        protected virtual void OnLoadError(object sender, LoadErrorEventArgs args)
        {
            AttachUserControls();
            this.EndUpdate();

            RaiseLoadError(args);
        }

        /// <summary>
        /// Raises the <see cref="HTMLUIControl.LinkClicked"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">An <see cref="LinkForwardEventArgs"/> with event data.</param>
        protected internal virtual void OnLinkClicked(object sender, LinkForwardEventArgs args)
        {
            RaiseLinkClicked(sender, args);
        }

        /// <summary>
        /// Aborts thread which loads a new document.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void AbortLoadThread()
        {
            if (m_loadThread != null)
            {
                if (m_loadThread.IsAlive)
                {
                    Debug.WriteLine("Aborting Load Tread...");
                    m_loadThread.Abort();
                    m_loadThread.Join();
                }

                m_loadThread = null;
            }
        }

        /// <summary>
        /// Loads document in a new thread.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void LoadThread()
        {
            lock (m_loadDelegate)
            {
                if (this.EnableMultithreading) Thread.CurrentThread.Name = DEF_THREAD_NAME;

                bool bAbort = true;

                try
                {
                    if (m_threadData == null) return;

                    // Invoke load of document started.
                    object[] invokeArgs = new object[] { this, EventArgs.Empty };
                    InvokeDelegate(m_loadStarted, invokeArgs);

#if PERFORMANCE
                    DateTime now = DateTime.Now;
#endif
                    // Clear all selection data.
                    this.SelectionManager.Reset();

                    m_threadData.Parse(HTMLParser);

                    // Reset original text if no need to save data.
                    if (!this.IsOriginalTextCached)
                    {
                        m_threadData.OriginalText = null;
                    }

#if PERFORMANCE
                    Debug.Indent();
                    Debug.WriteLine(DateTime.Now.Subtract(now), "Load / Parsing Takes");
                    Debug.Unindent();
                    now = DateTime.Now;
#endif
                    IHTMLElement rootElm = ConvertDocument(m_threadData.Document.DocumentElement, null, m_threadData);
                    m_threadData.SetRootElement(rootElm);

#if PERFORMANCE
                    Debug.Indent();
                    Debug.WriteLine(DateTime.Now.Subtract(now), "Convert Document Takes");
                    Debug.Unindent();
                    now = DateTime.Now;
#endif
                    // Raise event before document rendering.
                    PreRenderDocumentArgs args = new PreRenderDocumentArgs(m_threadData);
                    object[] prerenderArgs = new object[] { this, args };
                    InvokeDelegate(m_prerenderEvent, prerenderArgs);

                    m_threadData.GetCSSFormatsToElementHash();

                    // Update bounds for the document.
                    if (this.ShowTitle)
                    {
                        RefreshStartPoint(m_threadData);
                    }

#if PERFORMANCE
                    Debug.Indent();
                    Debug.WriteLine(DateTime.Now.Subtract(now), "CSSFormats Takes");
                    Debug.Unindent();
                    now = DateTime.Now;
#endif
                    m_threadData.Recalculate();

#if PERFORMANCE
                    Debug.Indent();
                    Debug.WriteLine(DateTime.Now.Subtract(now), "Recalculate Takes");
                    Debug.Unindent();
                    now = DateTime.Now;
#endif
                    // Notify user that we finish loading of document and start calculations.
                    InvokeDelegate(m_loadDelegate, invokeArgs);

                    bAbort = false;
                }
                catch (Exception ex)
                {
                    //// NOTE: If in code any exception happens, then set abort flag to False
                    //// state and rethrow exception to control.
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");
                    bAbort = false;

                    //// Raise event notification about render error.
                    m_threadData.SetError(ex);
                    LoadErrorEventArgs args = new LoadErrorEventArgs(m_threadData);

                    object[] arguments = new object[] { this, args };
                    m_bLoading = false;

                    InvokeDelegate(m_loadError, arguments);
                }
                finally
                {
                    //// Stop loading on abort.
                    if (bAbort) Thread.ResetAbort();
                }
            }
        }

        #endregion

        #region Paint Overrides
        /// <summary>
        /// Overrides paint event
        /// </summary>
        /// <param name="e">PaintEventArgs instance</param>
        /// <override/>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //// Skip drawing if BeginUpdate called at least once or document
            //// not loaded yet.
            if (m_iCanInvalidate > 0) return;
            if (this.IsLoading) return;
            if (this.Document == null) return;
            if (this.Document.Root == null) return;
            if (this.Document.RenderRoot == null) return;

            this.Document.Draw(e, this.AutoScrollPosition);

            //// Draw focused element.
            this.DocumentEx.FocusManager.DrawFocusRect(e.Graphics);
        }

        /// <summary>
        /// Overrides PaintBackGround event
        /// </summary>
        /// <param name="pevent">PaintEventArgs instance</param>
        /// <override/>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            // Skip drawing if BeginUpdate called at least once.
            if (m_iCanInvalidate > 0) return;

            Graphics g = pevent.Graphics;
            Rectangle rc = pevent.ClipRectangle;

            int y = this.AutoScrollPosition.Y;
            int x = this.AutoScrollPosition.X;

            // Draw background.
            if (this.Document != null && this.Document.RenderRoot != null)
            {
                Color bodyColor = this.Document.RenderRoot.Format.BackgroundColor;
                Color bgColor = (bodyColor != Color.Empty) ? bodyColor : this.BackColor;

                using (Brush brush = new SolidBrush(bgColor))
                {
                    Rectangle backRect = this.DisplayRectangle;
                    backRect.X += m_startPoint.X;
                    backRect.Y += m_startPoint.Y;

                    g.FillRectangle(brush, backRect);
                }
            }

            //// Draw title bar.
            if ((Math.Abs(y) < dEF_TITLE_HEIGHT) && m_bShowTitle &&
              this.DocumentEx != null)
            {
                Rectangle rcOut = GDIUtils.FixRectangleHeightWidth(x, y, Math.Max(Math.Max(this.AutoScrollMinSize.Width, rc.Width), this.DocumentEx.AutoScrollMinSize.Width), dEF_TITLE_HEIGHT);

                g.DrawString(this.Title, SystemInformation.MenuFont, SystemBrushes.WindowText, RectangleF.Inflate((RectangleF)rcOut, -3, -2), m_titleFormat);

                m_gdi.Draw3DBox(g, rcOut, Canvas3DStyle.Title);
            }
        }

        /// <summary>
        /// Overrides Resize event
        /// </summary>
        /// <param name="e">Event data</param>
        /// <override/>
        protected override void OnResize(EventArgs e)
        {
            if (!this.Recalculate || this.ClientSize.Width <= 0 ||
              this.ClientSize.Height <= 0 || this.IsLoading) return;

            base.OnResize(e);

            if (this.Document != null)
            {
                if (m_htmlDocument != null)
                {
                    RecalculateDocument();
                }
            }
        }

        /// <summary>
        /// Suspends the layout of the control.
        /// </summary>
        private void InternalBeginUpdate()
        {
            this.SuspendLayout();
        }

        /// <summary>
        /// Resumes the layout of the control.
        /// </summary>
        private void InternalEndUpdate()
        {
            this.Recalculate = false;
            this.ResumeLayout();
            this.Recalculate = true;
        }
        #endregion

        #region Event Overrides
        /// <summary> 
        /// Overrides MouseDown event
        /// </summary>
        /// <param name="e">Event Data</param>
        /// <override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.DocumentEx != null)
            {
                Point pt = this.DocumentEx.ClientToGlobal(new Point(e.X, e.Y));
                IHTMLElement elm = GetElementAtVirtualPoint(e.X, e.Y);

                if (elm != null)
                {
                    RaiseEventOnElement(elm, EventName.MouseDown, EventArgs.Empty);
                }
            }

            //// Define mouse button which was clicked.
            m_clickedButton = e.Button;

            if (this.DocumentEx != null && m_clickedButton == MouseButtons.Left)
            {
                Point p = new Point(e.X, e.Y);
                p = this.DocumentEx.ClientToGlobal(p);

                this.SelectionManager.StartSelection(p);
            }
        }

        /// <summary>
        /// Overrides MouseUp event
        /// </summary>
        /// <param name="e">MouseEventArgs instance</param>
        /// <override/>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            m_clickedLocation.X = e.X;
            m_clickedLocation.Y = e.Y;

            if (this.DocumentEx != null)
            {
                this.SelectionManager.EndSelection();
            }
        }

        /// <summary> 
        /// Overrides Click event
        /// </summary>
        /// <param name="e">Event Data</param>
        /// <override/>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.Document != null && !this.DocumentEx.Searcher.IsEmpty())
            {
                Point clickPoint = this.DocumentEx.ClientToGlobal(PointToClient(Control.MousePosition));
                BaseElement clickedElement = GetElementAtVirtualPoint(clickPoint) as BaseElement;

                // Raise Leave event on focused element.
                BaseElement focusedElement = this.DocumentEx.FocusManager.FocusedElement;

                if (focusedElement != null)
                {
                    this.DocumentEx.FocusManager.Reset();
                }

                // Raise Focus event on clicked element.
                if (clickedElement != null)
                {
                    clickedElement.Focus();

                    // Raise click event on clicked element.
                    RaiseEventOnElement(clickedElement, EventName.Click, e);
                }
            }

            m_clickedButton = Control.MouseButtons;
        }

        /// <summary> 
        /// Overrides DoubleClick event
        /// </summary>
        /// <param name="e">Event Data</param>
        /// <override/>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (this.Document != null && !this.DocumentEx.Searcher.IsEmpty())
            {
                Point clickPoint = this.DocumentEx.ClientToGlobal(PointToClient(Control.MousePosition));
                BaseElement clickedElement = GetElementAtVirtualPoint(clickPoint) as BaseElement;

                // Raise Leave event on focused element.
                BaseElement focusedElement = this.DocumentEx.FocusManager.FocusedElement;

                if (focusedElement != null)
                {
                    this.DocumentEx.FocusManager.Reset();
                }

                // Raise Focus event on clicked element.
                if (clickedElement != null)
                {
                    clickedElement.Focus();

                    // Raise DoubleClick event on clicked element.
                    RaiseEventOnElement(clickedElement, EventName.DoubleClick, e);
                }
            }

            m_clickedButton = Control.MouseButtons;
        }

        /// <summary> 
        /// Overrides MouseMove event
        /// </summary>
        /// <param name="e">Event Data</param>
        /// <override/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.Document != null && !this.IsLoading &&
              !this.DocumentEx.Searcher.IsEmpty())
            {
                Point clickPoint = this.DocumentEx.ClientToGlobal(PointToClient(Control.MousePosition));
                BaseElement element = GetElementAtVirtualPoint(clickPoint) as BaseElement;

                // Raise OnMouseEnter and OnMouseLeave events.
                if (element != null)
                {
                    //// Set ToolTip:
                    //// ShowDebugToolTip( element, clickPoint );
                    this.SelectionManager.ProcessSelection(clickPoint);

                    RaiseEventOnElement(element, EventName.MouseMove, e);

                    if (this.DocumentEx.PrevElement == null)
                    {
                        this.DocumentEx.PrevElement = element;
                    }

                    if (this.DocumentEx.PrevElement != element)
                    {
                        if (this.DocumentEx.PrevElement != null)
                        {
                            RaiseEventOnElement(this.DocumentEx.PrevElement, EventName.MouseLeave, e);
                        }

                        if (element != null)
                        {
                            RaiseEventOnElement(element, EventName.MouseEnter, e);
                            this.DocumentEx.PrevElement = element;
                        }
                    }
                }
                else
                {
                    this.SelectionManager.CheckScrolling();
                }
            }
        }

        /// <summary>
        /// Overloaded. Raised when control has got focus.
        /// </summary>
        /// <param name="e">An EventArgs which contains event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (this.DocumentEx != null && !this.DocumentEx.FocusManager.IsFocusing)
            {
                this.DocumentEx.FocusManager.Reset();
            }

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Overloaded. Raised when control has lost focus.
        /// </summary>
        /// <param name="e">An EventArgs which contains event data.</param>
        protected override void OnLeave(EventArgs e)
        {
            if (this.DocumentEx != null && !this.DocumentEx.FocusManager.IsFocusing)
            {
                this.DocumentEx.FocusManager.BeginFocus();
                this.DocumentEx.FocusManager.Reset();
                this.DocumentEx.FocusManager.EndFocus();
            }

            base.OnLeave(e);
        }

        /// <summary>
        /// Overloaded. Processes a dialog key for handling Tab command key.
        /// </summary>
        /// <param name="keyData">One of the Key values that represents the key to process.</param>
        /// <returns>True if the key was processed by the control; false otherwise.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool result = false;

            if (this.DocumentEx != null)
            {
                if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
                {
                    this.DocumentEx.FocusManager.BeginFocus();
                    Keys key = keyData & Keys.KeyCode;

                    //// Tab key was down.
                    if (key == Keys.Tab)
                    {
                        //// bool bForward = false;

                        // Only Tab key was down.
                        if ((keyData & Keys.Shift) == Keys.None)
                        {
                            result = this.DocumentEx.FocusManager.SetFocus(1);
                            //// bForward = true;
                        }
                        else
                        {
                            //// Shift Tab was down.
                            result = this.DocumentEx.FocusManager.SetFocus(-1);
                        }

                        //// Default processing.
                        if (!result)
                        {
                            result = base.ProcessDialogKey(keyData);
                        }
                        ////result = SetFocusControl( keyData, result, bForward );
                    }
                    else if (key == Keys.Return)
                    {
                        //// Enter key was pressed, try click link element if focused.
                        BaseElement focused = this.DocumentEx.FocusManager.FocusedElement;

                        if (focused != null && focused is AElementImpl)
                        {
                            m_clickedButton = MouseButtons.Left;
                            RaiseEventOnElement(focused, EventName.Click, EventArgs.Empty);
                            result = true;
                        }
                    }
                    else
                    {
                        //// We don't process other keys and invoke default functionality.
                        this.DocumentEx.FocusManager.EndFocus();
                        result = base.ProcessDialogKey(keyData);
                    }

                    this.DocumentEx.FocusManager.EndFocus();
                }
                else
                {
                    this.DocumentEx.FocusManager.EndFocus();
                    result = base.ProcessDialogKey(keyData);
                }
            }
            else
            {
                //// Default processing.
                result = base.ProcessDialogKey(keyData);
            }

            return result;
        }

        /// <summary>
        /// Overloaded. Raised when control has lost focus.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            this.SelectionManager.EndSelection();

            base.OnLostFocus(e);
        }

        /// <summary>
        /// Overloaded. Used for shortcuts invoked within the control.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && !e.Alt && !e.Shift)
            {
                Keys key = e.KeyData & e.KeyCode;

                //// Start text searching.
                if (key == Keys.F)
                {
                    this.SelectionManager.ProcessTextSearch();
                }
                else if (key == Keys.A)
                {
                    //// Select all text.
                    this.SelectionManager.SelectAllText();
                }
                else if (key == Keys.C && this.SelectionManager.SelectedText.Length > 0)
                {
                    ////Copies the SelectedText in the Control to ClipBoard.
                    Clipboard.SetDataObject(this.SelectionManager.SelectedText, true);
                }
            }
        }

        /// <summary>
        /// Overloaded. Raises the Layout event. Also it prevents invoking of the base method 
        /// while code running is between BeginUpdate / EndUpdate method calls.
        /// </summary>
        /// <param name="levent">Event data.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (m_iCanInvalidate <= 0)
            {
                base.OnLayout(levent);
            }
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises the TitleChanged event.
        /// </summary>
        /// <param name="args">Event data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseTitleChanged(ValueChangedEventArgs args)
        {
            if (TitleChanged != null)
            {
                TitleChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the ShowTitleChanged event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseShowTitleChanged(ValueChangedEventArgs args)
        {
            if (ShowTitleChanged != null)
            {
                ShowTitleChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the LoadFinished event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseLoadFinished(EventArgs args)
        {
            if (LoadFinished != null)
            {
                LoadFinished(this, args);
            }
        }

        /// <summary>
        /// Raises the LoadStarted event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseLoadStarted(EventArgs args)
        {
            if (LoadStarted != null)
            {
                LoadStarted(this, args);
            }
        }

        /// <summary>
        /// Raises the PreRenderDocument event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaisePreRenderDocument(PreRenderDocumentArgs args)
        {
            if (PreRenderDocument != null)
            {
                PreRenderDocument(this, args);
            }
        }

        /// <summary>
        /// Raises the LoadError event.
        /// </summary>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseLoadError(LoadErrorEventArgs args)
        {
            if (LoadError != null)
            {
                LoadError(this, args);
            }
        }

        /// <summary>
        /// Raises the LinkClicked event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event Data</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseLinkClicked(object sender, LinkForwardEventArgs args)
        {
            if (LinkClicked != null)
            {
                LinkClicked(sender, args);
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculates all formats for the specified element.
        /// </summary>
        /// <param name="element">Element object.</param>
        private void CalculateFormats(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.CalculateFormat();

            if (element.HasChildren)
            {
                BaseElement childElement = null;

                for (int i = 0, len = element.Children.Count; i < len; i++)
                {
                    childElement = element.Children[i] as BaseElement;
                    CalculateFormats(childElement);
                }
            }
        }

        /// <summary>
        /// Converts XML document to HTML Elements tree.
        /// </summary>
        /// <param name="xmlCurrent">Current data of HTML tag.</param>
        /// <param name="elementParent">Parent element of this tag.</param>
        /// <param name="document">Parent document of this element.</param>
        /// <returns>Returns BaseElement instanse</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal BaseElement ConvertDocument(XmlElement xmlCurrent, BaseElement elementParent, InputHTML document)
        {
            if (xmlCurrent == null)
                throw new ArgumentNullException("xmlCurrent");

            InputHTML doc = (document == null) ? m_threadData : document;

            //// Change UniqueID to new value.
            m_uniqueID++;

            BaseElement tagElement = ElementsFactory.ConvertTo(this, xmlCurrent);
            tagElement.Parent = elementParent;
            tagElement.Document = doc;

            ////NOTE: It is important for UniqueID to be string value of integer.
            ////Do not change it.
            tagElement.UniqueID = m_uniqueID.ToString();

            ////Add converted element into parent collection.
            if (elementParent != null) elementParent.Children.Add(tagElement);

            if (tagElement is IElementHasCss)
            {
                TokenStream stream = ((IElementHasCss)tagElement).GetCssStream();

                ////If element can resolve link to CSS file then parse it.
                if (stream != null)
                {
                    XmlDocument css = CSSParser.Parse(stream);
                    ElementHasCssEventArgs args = new ElementHasCssEventArgs(css, tagElement);
                    doc.Formats.Element_HasCss(this, args);
                    stream.Close();
                }
            }

            if (xmlCurrent.HasChildNodes)
            {
                XmlNode node1 = xmlCurrent.FirstChild;

                while (node1 != null)
                {
                    XmlNode childNode = node1;
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        ConvertDocument(childNode as XmlElement, tagElement, document);
                    }

                    node1 = node1.NextSibling;
                }
            }

            InfillInputHashes(tagElement, doc);

            //// Finish initializaion.
            tagElement.InitializeElement();

            return tagElement;
        }

        /// <summary>
        /// Infills all hashes which contain elements and formats needed for work.
        /// </summary>
        /// <param name="tagElement">Element object.</param>
        /// <param name="document">Parent document object for the element.</param>
        private void InfillInputHashes(BaseElement tagElement, InputHTML document)
        {
            if (document != null)
            {
                //// infill ElementByUniqueID hash
                document.ElementsByUniqueID.Add(tagElement.UniqueID, tagElement);

                //// infill ElementByUserID hash
                if (tagElement.ID != null && tagElement.ID.Length > 0)
                {
                    document.ElementsByUserID[tagElement.ID] = tagElement;
                }

                //// infill ElementsByTagName hash
                if (!document.ElementsByTagName.ContainsKey(tagElement.Name))
                {
                    document.ElementsByTagName[tagElement.Name] = new ArrayList();
                }

                ((ArrayList)document.ElementsByTagName[tagElement.Name]).Add(tagElement);
            }
        }

        /// <summary>
        /// Indicates whether the point is inside the title.
        /// </summary>
        /// <param name="point">Point value</param>
        /// <returns>True if the point is in the title area.</returns>
        private bool PointInTitle(Point point)
        {
            if (!this.ShowTitle) return false;

            if (point.Y > dEF_TITLE_HEIGHT) return false;

            int visTitleHeight = dEF_TITLE_HEIGHT + this.AutoScrollPosition.Y;
            if (visTitleHeight <= point.Y) return false;

            return true;
        }

        /// <summary>
        /// Returns the TITLE element if exists; NULL otherwise.
        /// </summary>
        /// <returns>Title element object.</returns>
        private IHTMLElement GetTitleElement()
        {
            if (this.Document != null)
            {
                ArrayList list = (ArrayList)this.DocumentEx.ElementsByTagName["title"];
                if (list != null && list.Count > 0)
                {
                    return (IHTMLElement)list[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Raises event on element on which Event occurs.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        private void RaiseEventOnElement(string eventName, EventArgs args)
        {
            if (eventName == null)
                throw new ArgumentNullException("eventName");

            if (eventName.Length == 0)
                throw new ArgumentException("eventName - string can not be empty");

            if (args == null)
                throw new ArgumentNullException("args");

            if (this.Document != null && !this.DocumentEx.Searcher.IsEmpty())
            {
                Point clickPoint = this.DocumentEx.ClientToGlobal(PointToClient(Control.MousePosition));
                BaseElement element = GetElementAtVirtualPoint(clickPoint) as BaseElement;

                if (element != null)
                {
                    if (element.Events.Contains(eventName))
                    {
                        ((HashElementEvents)element.Events[eventName]).RaiseEvent(args);
                    }
                }
            }
        }

        /// <summary>
        /// Raises event on Tag element.
        /// </summary>
        /// <param name="element">Tag element.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="args">The Arguments.</param>
        private void RaiseEventOnElement(IHTMLElement element, string eventName, EventArgs args)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (eventName == null)
                throw new ArgumentNullException("eventName");

            if (eventName.Length == 0)
                throw new ArgumentException("eventName - string can not be empty");

            if (args == null)
                throw new ArgumentNullException("args");

            BaseElement baseElement = (BaseElement)element;
            if (baseElement.Events.Contains(eventName))
            {
                ((HashElementEvents)baseElement.Events[eventName]).RaiseEvent(args);
            }
        }

        /// <summary>
        /// Detaches all winforms controls in a document from the control.
        /// </summary>
        private void DetachUserControls()
        {
            if (this.Controls.Count > 0 && !this.DesignMode)
            {
                this.Recalculate = false;
                this.Controls.Clear();
                this.Recalculate = true;
            }
        }

        /// <summary>
        /// Attaches all winforms controls in a document to the control.
        /// </summary>
        private void AttachUserControls()
        {
            if (this.Document == null) return;

            if (this.DocumentEx.UserControls.Count > 0)
            {
                this.SuspendLayout();
                this.Recalculate = false;
                ICustomControlBase control = null;
                UserControlImpl intControl = null;

                for (int i = 0; i < this.DocumentEx.UserControls.Count; i++)
                {
                    control = (ICustomControlBase)this.DocumentEx.UserControls[i];

                    //// NOTE: We attach controls at first, then we must destroy
                    //// them and recreate on this thread.
                    intControl = control as UserControlImpl;
                    if (this.DocumentEx.DestroyControls && intControl != null
                      && intControl.NeedDispose)
                    {
                        //// Save size of the control.
                        Size controlSize = intControl.CustomControl.Size;

                        intControl.DisposeControl();
                        intControl.InitializeControl();

                        //// Restore size of the control.
                        intControl.CustomControl.Size = controlSize;
                    }

                    control.SetLocation();
                    this.Controls.Add(control.CustomControl);
                }

                this.Recalculate = true;
                this.DocumentEx.DestroyControls = false;
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Sets the location for all user controls according to their elements.
        /// </summary>
        /// <param name="document">Document object.</param>
        private void SetControlsLocation(InputHTML document)
        {
            if (document == null) return;

            if (document.UserControls.Count > 0)
            {
                this.SuspendLayout();
                this.Recalculate = false;
                ICustomControlBase control = null;

                for (int i = 0; i < document.UserControls.Count; i++)
                {
                    control = (ICustomControlBase)document.UserControls[i];
                    control.SetLocation();

                    //// Add control if it is not still in control's collection.
                    if (!this.Controls.Contains(control.CustomControl))
                    {
                        this.Controls.Add(control.CustomControl);
                    }
                }

                this.Recalculate = true;
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Loads the document from the history according to the index.
        /// </summary>
        /// <param name="index">Index of moving in the history.</param>
        private void LoadFromHistory(int index)
        {
            HistoryPair pair = m_history.Pop(index);

            if (pair.Key == null || pair.Key.Length == 0) return;

            switch (pair.Tag)
            {
                case PathType.File:
                    LoadHTML(pair.Key);
                    return;

                case PathType.Uri:
                    Uri uri = new Uri(pair.Key);
                    LoadHTML(uri);
                    return;
            }
        }

        /// <summary>
        /// Sets the sizes in document before calculating its real size.
        /// </summary>
        /// <param name="document">Document object.</param>
        private void SetBeforeCalculating(InputHTML document)
        {
            if (document != null)
            {
                if (document.Y < this.Increase)
                {
                    document.Y = this.Increase;
                }

                document.ClientSize = this.ClientSize;
                document.AutoScrollMinSize = this.ClientSize;

                //// NOTE: Here scrolling is not available and we must reduce width of
                //// the document's client area.
                if (this.AutoScroll)
                {
                    if (!this.VScroll)
                    {
                        document.ClientWidth -= SystemInformation.VerticalScrollBarWidth;
                    }
                    this.AutoScrollMinSize = new Size(document.ClientWidth, document.ClientHeight);
                }

                this.SelectionManager.ResetCalculation();
            }
        }

        /// <summary>
        /// Sets the sizes in control after calculating corresponding document real size.
        /// </summary>
        /// <param name="document">Document object.</param>
        private void SetAfterCalculating(InputHTML document)
        {
            if (document != null)
            {
                Size size = document.AutoScrollMinSize;

                size.Width += document.Margins.Left;
                size.Height += document.Margins.Top;

                this.Recalculate = false;

                if (this.AutoScroll)
                {
                    if (this.VScroll)
                    {
                        this.AutoScrollMinSize = new Size(size.Width - SystemInformation.VerticalScrollBarWidth, size.Height);
                    }
                    else
                        this.AutoScrollMinSize = size;
                }

                this.Recalculate = true;

                SetControlsLocation(document);
            }
        }

        /// <summary>
        /// Recalculates current document and changes the view of control.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void RecalculateDocument()
        {
            RecalculateDocument(this.DocumentEx);
        }

        /// <summary>
        /// Recalculates current document and changes the view of control.
        /// </summary>
        /// <param name="document">InputHtml document</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void RecalculateDocument(InputHTML document)
        {
            if (document != null)
            {
                lock (m_loadDelegate)
                {
                    try
                    {
                        InternalBeginUpdate();

                        m_bLoading = true;
                        SetBeforeCalculating(document);
                        document.Recalculate();
                        SetAfterCalculating(document);
                        m_bLoading = false;
                    }
                    finally
                    {
                        InternalEndUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Scrolls control to the specified text object.
        /// </summary>
        /// <param name="textObj">Text object.</param>
        protected internal void ScrollToText(Text textObj)
        {
            if (textObj == null)
                throw new ArgumentNullException("textObj");

            if (this.AutoScroll &&
              textObj.Parent.Owner.Document == this.ThreadDocument &&
              textObj.Parent.Owner.IsVisible)
            {
                Rectangle boundRect = textObj.Parent[textObj];
                InputHTML document = textObj.Parent.Owner.Document;
                ScrollToRectangle(boundRect, document);
            }
        }

        /// <summary>
        /// Infills collection of user scripts from the document.
        /// </summary>
        /// <param name="document">Parent document of script elements.</param>
        private void InfillScriptCollection(InputHTML document)
        {
            if (document == null) return;

            m_scripts.Clear();

            ArrayList embScripts = document.ElementsByTagName[TagName.Script] as ArrayList;

            if (embScripts == null || embScripts.Count == 0) return;

            SCRIPTElementImpl element = null;

            for (int i = 0, len = embScripts.Count; i < len; i++)
            {
                element = embScripts[i] as SCRIPTElementImpl;

                if (element == null) continue;

                HTMLScriptSite scrSite = new HTMLScriptSite(this);
                HTMLScript script = new HTMLScript(element);

                ScriptManagerEx obj = new ScriptManagerEx(scrSite, script);
                script.Language = GetScriptLanguage(element);
                obj.IsEmbeded = false;
                obj.QuietMode = true;

                string path = element.Path;

                if (path != null)
                {
                    obj.File = path;
                }

                //// File doesn't exist.
                if (obj.File.Length == 0)
                {
                    script.SourceText = element.Storage.InnerText;
                    obj.IsEmbeded = true;
                }

                obj.QuietMode = false;

                obj.AutoRun = true;

                if (element.Parent != null && element.Parent is HEADElementImpl)
                {
                    obj.AutoRun = false;
                }

                this.Scripts.Add(obj);
            }
        }

        /// <summary>
        /// Returns the script language to the script tag element.
        /// </summary>
        /// <param name="element">Element containing script code.</param>
        /// <returns>Type of script language.</returns>
        private ScriptLanguages GetScriptLanguage(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            IHTMLAttribute attr = element.Attributes[AttributeName.Language];

            if (attr == null) return ScriptLanguages.JScript;

            switch (attr.Value.ToLower(CultureInfo.CurrentCulture))
            {
                case AttributeValue.Vbs:
                case AttributeValue.VBScript: return ScriptLanguages.VisualBasic;
                case AttributeValue.Csh:
                case AttributeValue.Csharp: return ScriptLanguages.CSharp;
                case AttributeValue.JavaScript:
                case AttributeValue.JScript:
                default: return ScriptLanguages.JScript;
            }
        }

        /// <summary>
        /// Compiles the script after document loading.
        /// </summary>
        private void RunAutoScripts()
        {
            if (!this.AutoRunScripts || m_htmlDocument == null) return;

            m_htmlDocument.SetCompileErrors(null);
            this.Scripts.Compile();
        }

        /// <summary>
        /// Invokes delegate synchronously or asynchronously depending on the status of the
        /// EnableMultithreading property.
        /// </summary>
        /// <param name="del">Delegate to invoke.</param>
        /// <param name="parameters">Array of input parameters for delegate.</param>
        private void InvokeDelegate(Delegate del, object[] parameters)
        {
            if (del == null)
                throw new ArgumentNullException("del");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (this.EnableMultithreading)
            {
                this.BeginInvoke(del, parameters);
            }
            else
            {
                del.DynamicInvoke(parameters);
            }
        }

        /// <summary>
        /// Sets the next focused control.
        /// </summary>
        /// <param name="keyData">Key data value</param>
        /// <param name="elmFocused">Indicates whether any element was focused.</param>
        /// <param name="isForward">Indicates whether the tab direction is forward.</param>
        /// <returns>True if focusing was processed; False otherwise.</returns>
        private bool SetFocusControl(Keys keyData, bool elmFocused, bool isForward)
        {
            if (this.Parent != null && this.Parent.Controls.Count > 0 &&
              isForward && !elmFocused)
            {
                int curIndex = this.Parent.Controls.GetChildIndex(this, false);
                Control nextControl;
                int childsCount = this.Parent.Controls.Count;

                if (curIndex >= 0 && curIndex < this.Parent.Controls.Count)
                {
                    int nextIndex = ++curIndex;
                    nextIndex = (nextIndex >= childsCount) ? 0 : nextIndex;

                    nextControl = this.Parent.Controls[nextIndex];

                    if (nextControl != null)
                    {
                        elmFocused = this.Parent.SelectNextControl(nextControl, isForward, true, true, true);
                    }
                }
            }
            else if (!elmFocused)
            {
                elmFocused = base.ProcessDialogKey(keyData);
            }

            return elmFocused;
        }

        /// <summary>
        /// Shows the Debug Tooltip on the element.
        /// </summary>
        /// <param name="element">Element where the tooltip is to be shown.</param>
        /// <param name="clickPoint">Mouse position.</param>
        private void ShowDebugToolTip(BaseElement element, Point clickPoint)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            string text = "Element: " + element.Name + " at: " + clickPoint.ToString();

            m_tooltip.SetToolTip(this, text);
            this.ToolTip.Active = true;
        }

        /// <summary>
        /// Stores the original text to the document.
        /// </summary>
        /// <param name="document">Document for storing original Text.</param>
        /// <param name="data">Original data.</param>
        private void SetOriginalText(InputHTML document, string data)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (data == null)
                throw new ArgumentNullException("data");

            if (this.IsOriginalTextCached)
            {
                document.OriginalText = data;
            }
        }

        /// <summary>
        /// Recalculates the start point of the document.
        /// </summary>
        /// <param name="document">Document object.</param>
        private void RefreshStartPoint(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            int top = document.Margins.Top;

            if (this.ShowTitle)
            {
                m_startPoint.Y = dEF_TITLE_HEIGHT;
                top += dEF_TITLE_HEIGHT;
            }
            else
            {
                m_startPoint.Y = 0;
                top -= dEF_TITLE_HEIGHT;
                if (!this.NeedDefaultMargin)
                    top = 0;
            }
            document.Margins.Top = top;
        }

        /// <summary>
        /// Scrolls the document to the specified fragment, defined in the uri object.
        /// </summary>
        /// <param name="fragment">Fragment of the uri.</param>
        /// <param name="document">Document displaying in the control.</param>
        internal void JumpToFragment(string fragment, InputHTML document)
        {
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            // Document must be scrolled.
            if (document != null && fragment.Length > 1 &&
              fragment[0] == Utilities.DEF_FRAGMENT_PREFIX)
            {
                string elmName = fragment.Substring(1);

                IHTMLElement element = document.GetElementByUserId(elmName);
                BaseElement elementEx = element as BaseElement;

                if (element != null)
                {
                    if (this.AutoScroll &&
                      document == this.ThreadDocument &&
                      document == elementEx.Document &&
                      element.IsVisible && elementEx.Blocks.Count > 0)
                    {
                        Rectangle elmRect = GetElementScrollRectangle(elementEx);

                        //// Here we have to place the anchor at the top of the control.
                        int x = Math.Min(elmRect.X, Math.Abs(this.AutoScrollPosition.X));
                        int y = elmRect.Y;
                        Point pt = new Point(x, y);
                        SetAutoScrollPosition(pt, document);
                    }
                }
            }
        }

        /// <summary>
        /// Description of GetElementScrollRectangle method
        /// </summary>
        /// <param name="element">The element to scroll.</param>
        /// <returns>Returns rectangle</returns>
        private Rectangle GetElementScrollRectangle(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Rectangle elmRect = Rectangle.Empty;

            if (element.IsBlock)
            {
                elmRect = element.MainBlock.Rectangle;
            }
            else
            {
                Block block = element.Blocks[element.Blocks.Count - 1];
                elmRect = block.Rectangle;
            }

            return elmRect;
        }

        /// <summary>
        /// Sets autoscroll position of the control.
        /// </summary>
        /// <param name="point">A new scroll position.</param>
        /// <param name="document">The current document.</param>
        internal void SetAutoScrollPosition(Point point, InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            this.AutoScrollPosition = point;
            document.AutoScrollPosition = this.AutoScrollPosition;
        }
        #endregion

        #region ISupportInitialize Members

        /// <summary>
        /// Invoked when initialization begins.
        /// </summary>
        public void BeginInit()
        {
            if (this.DesignMode)
            {
                if (StartupDocument != null && StartupDocument.Length > 0)
                {
                    LoadHTML(this.StartupDocument);
                }
            }
        }

        /// <summary>
        /// Invoked when initialization ends.
        /// </summary>
        public void EndInit()
        {
            //// if( !this.DesignMode )
            {
                if (StartupDocument != null && StartupDocument.Length > 0)
                {
                    LoadHTML(this.StartupDocument);
                }
            }

            m_binitDone = true;
        }
        #endregion

        #region Class Serialize methods

        /// <summary>
        /// Indicates whether Text property is serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeText()
        {
            return this.StartupDocument == null || this.StartupDocument.Length == 0;
        }

        /// <summary>
        /// Indicates whether DefaultFormat property should be serialized.
        /// </summary>
        /// <returns>bool value</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeDefaultFormat()
        {
            return true;
        }

        /// <summary>
        /// Indicates whether DisplayText property is persisted.
        /// </summary>
        /// <returns>bool value</returns>
        [Browsable(false)]
        protected virtual bool ShouldSerializeDisplayText()
        {
            return false;
        }
        #endregion
    }
}
