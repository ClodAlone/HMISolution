#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Defines the way the document is to be presented on the screen or in print.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Create a new document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Set the document`s viewer preference
    /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments;
    /// doc.ViewerPreferences.PageScaling = PageScalingMode.None;
    /// doc.ViewerPreferences.FitWindow = true;
    /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage;
    /// // Save the document
    /// doc.Save("ViewerPreferences.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Set the document`s viewer preference
    /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments
    /// doc.ViewerPreferences.PageScaling = PageScalingMode.None
    /// doc.ViewerPreferences.FitWindow = True
    /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage
    /// ' Save the document
    /// doc.Save("ViewerPreferences.pdf")
    /// </code>
    /// </example>
    public class PdfViewerPreferences : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store value whether to center document's window.
        /// </summary>
        private bool m_centerWindow = false;

        /// <summary>
        /// Internal variable to store value whether to display window�s title bar.
        /// </summary>
        private bool m_displayDocTitle = false;

        /// <summary>
        /// Internal variable to store value whether to resize the document�s window 
        /// to fit the size of the first displayed page.
        /// </summary>
        private bool m_fitWindow = false;

        /// <summary>
        /// Internal variable to store value whether to hide the viewer application�s 
        /// menu bar when the document is active.
        /// </summary>
        private bool m_hideMenubar = false;

        /// <summary>
        /// Internal variable to store value whether to hide the viewer application�s 
        /// tool bars when the document is active.
        /// </summary>
        private bool m_hideToolbar = false;

        /// <summary>
        /// Internal vaiable to store value whether to hide user interface elements 
        /// in the document�s window.
        /// </summary>
        private bool m_hideWindowUI = false;

        /// <summary>
        /// Internal variable to store value how the document should be displayed when opened
        /// </summary>
        private PdfPageMode m_pageMode = PdfPageMode.UseNone;

        /// <summary>
        /// Internal variable to store value specifying the page layout to be used when the 
        /// document is opened.
        /// </summary>
        private PdfPageLayout m_pageLayout = PdfPageLayout.SinglePage;

        /// <summary>
        /// Internal variable to store value specifying document's catalog.
        /// </summary>
        private PdfCatalog m_catalog = null;

        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store value specifying the page scaling mode used while printing.
        /// </summary>
        private PageScalingMode m_pageScaling;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfViewerPreferences"/> class.
        /// </summary>
        internal PdfViewerPreferences()
            : base()
        {
        }

        /// <summary>
        /// Initializes new <see cref="PdfViewerPreferences"/> instance.
        /// </summary>
        /// <param name="catalog">Catalog of the document.</param>
        internal PdfViewerPreferences(PdfCatalog catalog)
            : base()
        {
            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

            m_catalog = catalog;
        }
        #endregion

        #region Properties
        /// <summary>
        /// A flag specifying whether to position the document�s window in the center of the screen.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.CenterWindow = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.CenterWindow = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool CenterWindow
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfDictionary tempDictionary = (m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder).Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.CenterWindow))
                        {
                            m_centerWindow = bool.Parse((tempDictionary[DictionaryProperties.CenterWindow] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.CenterWindow))
                        {
                            m_centerWindow = bool.Parse((tempDictionary[DictionaryProperties.CenterWindow] as PdfBoolean).Value.ToString());
                        }
                    }
                }
                return m_centerWindow;
            }

            set
            {
                m_centerWindow = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfDictionary tempDictionary = (m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder).Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.CenterWindow, m_centerWindow);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.CenterWindow, m_centerWindow);
                }
            }
        }

        /// <summary>
        /// A flag specifying whether the window�s title bar should display the document title taken 
        /// from the Title entry of the document information dictionary. If false, the title bar 
        /// should instead display the name of the Pdf file containing the document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.DisplayTitle = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.DisplayTitle = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool DisplayTitle
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfDictionary tempDictionary = (m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder).Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.DisplayDocTitle))
                        {
                            m_displayDocTitle = bool.Parse((tempDictionary[DictionaryProperties.DisplayDocTitle] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.DisplayDocTitle))
                        {
                            m_displayDocTitle = bool.Parse((tempDictionary[DictionaryProperties.DisplayDocTitle] as PdfBoolean).Value.ToString());
                        }
                    }
                }
                return m_displayDocTitle;
            }

            set
            {

                m_displayDocTitle = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfDictionary tempDictionary = (m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder).Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.DisplayDocTitle, m_displayDocTitle);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.DisplayDocTitle, m_displayDocTitle);
                }
            }
        }

        /// <summary>
        /// A flag specifying whether to resize the document�s window to fit the size of the first 
        /// displayed page.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.FitWindow = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.FitWindow = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool FitWindow
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.FitWindow))
                        {
                            m_fitWindow = bool.Parse((tempDictionary[DictionaryProperties.FitWindow] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.FitWindow))
                        {
                            m_fitWindow = bool.Parse((tempDictionary[DictionaryProperties.FitWindow] as PdfBoolean).Value.ToString());
                        }
                    }
                }
                return m_fitWindow;
            }

            set
            {

                m_fitWindow = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                    PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.FitWindow, m_fitWindow);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.FitWindow, m_fitWindow);
                }
            }
        }

        /// <summary>
        /// A flag specifying whether to hide the viewer application�s menu bar when the 
        /// document is active.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.HideMenubar = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.HideMenubar = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool HideMenubar
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.HideMenubar))
                        {
                            m_hideMenubar = bool.Parse((tempDictionary[DictionaryProperties.HideMenubar] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if(tempDictionary.ContainsKey(DictionaryProperties.HideMenubar))
                        {
                        m_hideMenubar = bool.Parse((tempDictionary[DictionaryProperties.HideMenubar] as PdfBoolean).Value.ToString());
                        }
                    }
                }
                return m_hideMenubar;
            }

            set
            {
                m_hideMenubar = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                    PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideMenubar, m_hideMenubar);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideMenubar, m_hideMenubar);
                }
            }
        }

        /// <summary>
        /// A flag specifying whether to hide the viewer application�s tool bars when the document is active.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.HideToolbar = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.HideToolbar = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool HideToolbar
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.HideToolbar))
                        {
                            m_hideToolbar = bool.Parse((tempDictionary[DictionaryProperties.HideToolbar] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                       if(tempDictionary.ContainsKey(DictionaryProperties.HideToolbar))
                        m_hideToolbar = bool.Parse((tempDictionary[DictionaryProperties.HideToolbar] as PdfBoolean).Value.ToString());
                    }
                }
                return m_hideToolbar;
            }

            set
            {
                m_hideToolbar = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                    PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideToolbar, m_hideToolbar);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideToolbar, m_hideToolbar);
                }
            }
        }

        /// <summary>
        /// A flag specifying whether to hide user interface elements in the document�s window 
        /// (such as scroll bars and navigation controls), leaving only the document�s contents displayed.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.HideWindowUI = true;           
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");            
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.HideWindowUI = True
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public bool HideWindowUI
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.HideWindowUI))
                        {
                            m_hideWindowUI = bool.Parse((tempDictionary[DictionaryProperties.HideWindowUI] as PdfBoolean).Value.ToString());
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if(tempDictionary.ContainsKey(DictionaryProperties.HideWindowUI))
                        m_hideWindowUI = bool.Parse((tempDictionary[DictionaryProperties.HideWindowUI] as PdfBoolean).Value.ToString());
                    }
                }
                return m_hideWindowUI;
            }

            set
            {
                m_hideWindowUI = value;
                m_dictionary = m_catalog as PdfDictionary;

                if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                {
                    PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                    PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideWindowUI, m_hideWindowUI);
                }
                else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                {
                    PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                    tempDictionary.SetBoolean(DictionaryProperties.HideWindowUI, m_hideWindowUI);
                }
            }
        }

        /// <summary>
        /// A name object specifying how the document should be displayed when opened.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference
        /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments;      
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.PageMode = PdfPageMode.UseAttachments      
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public PdfPageMode PageMode
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.PageMode] != null)
                    {
                        PdfName name = m_dictionary[DictionaryProperties.PageMode] as PdfName;
                        m_pageMode = (PdfPageMode)Enum.Parse(typeof(PdfPageMode), name.Value, true);
                    }
                }
                return m_pageMode;
            }

            set
            {
                m_pageMode = value;
                PdfDictionary.SetName(m_catalog, DictionaryProperties.PageMode, m_pageMode.ToString());
            }
        }

        /// <summary>
        /// A name object specifying the page layout to be used when the document is opened.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference      
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage;
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference      
        /// doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public PdfPageLayout PageLayout
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;

                    if (m_dictionary.ContainsKey(DictionaryProperties.PageLayout))
                    {
                        m_pageLayout = (PdfPageLayout)Enum.Parse(typeof(PdfPageLayout), (m_dictionary[DictionaryProperties.PageLayout] as PdfName).Value.ToString(), true);
                    }
                }
                return m_pageLayout;
            }

            set
            {
                m_pageLayout = value;
                PdfDictionary.SetName(m_catalog, DictionaryProperties.PageLayout, m_pageLayout.ToString());
            }
        }

        /// <summary>
        /// Gets or Set the page scaling option to be selected 
        /// when a print dialog is displayed for this document.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Create a new document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Set the document`s viewer preference      
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.None;
        /// // Save the document
        /// doc.Save("ViewerPreferences.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// 'Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Set the document`s viewer preference
        /// doc.ViewerPreferences.PageScaling = PageScalingMode.None
        /// ' Save the document
        /// doc.Save("ViewerPreferences.pdf")
        /// </code>
        /// </example>
        public PageScalingMode PageScaling
        {
            get
            {
                if (m_catalog.LoadedDocument != null)
                {
                    m_dictionary = m_catalog as PdfDictionary;
                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.PrintScaling))
                        {
                            m_pageScaling = (PageScalingMode)Enum.Parse(typeof(PageScalingMode), (tempDictionary[DictionaryProperties.PrintScaling] as PdfName).Value.ToString(), true);
                        }
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        if (tempDictionary.ContainsKey(DictionaryProperties.PrintScaling))
                        {
                            m_pageScaling = (PageScalingMode)Enum.Parse(typeof(PageScalingMode), (tempDictionary[DictionaryProperties.PrintScaling] as PdfName).Value.ToString(), true);

                        }
                    }
                }
                return m_pageScaling;
            }

            set
            {
                if (value != null)
                {
                    m_pageScaling = value;
                    m_dictionary = m_catalog as PdfDictionary;

                    if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfReferenceHolder)
                    {
                        PdfReferenceHolder refHolder = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfReferenceHolder;
                        PdfDictionary tempDictionary = refHolder.Object as PdfDictionary;
                        tempDictionary.SetName(DictionaryProperties.PrintScaling, m_pageScaling.ToString());
                    }
                    else if (m_dictionary[DictionaryProperties.ViewerPreferences] is PdfDictionary)
                    {
                        PdfDictionary tempDictionary = m_dictionary[DictionaryProperties.ViewerPreferences] as PdfDictionary;
                        tempDictionary.SetName(DictionaryProperties.PrintScaling, m_pageScaling.ToString());
                    }
                }
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
