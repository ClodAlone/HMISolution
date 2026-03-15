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
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an action for the document.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  //Create a new PDF document
    ///  PdfDocument document = new PdfDocument();
    ///  //Create and add new launch Action to the document
    ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
    ///  document.Actions.AfterOpen = action;
    ///  //Save document to disk
    ///  document.Save("DocumentAction.pdf");
    /// </code>
    /// <code lang="VB">
    ///  'Create a new PDF document.
    ///  Dim document As PdfDocument = New PdfDocument()
    ///  'Create and add new launch Action to the document.
    ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
    ///  document.Actions.AfterOpen = action
    ///  'Save document to disk
    ///  document.Save("DocumentAction.pdf")
    /// </code>
    /// </example>
    public class PdfDocumentActions : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictionary wrapper.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store action to be performed after the document is opened.
        /// </summary>
        private PdfAction m_afterOpen = null;

        /// <summary>
        /// Internal variable to store action to be perforemed before document closes.
        /// </summary>
        private PdfJavaScriptAction m_beforeClose = null;

        /// <summary>
        /// Internal variable to store action to be performed before the document saves.
        /// </summary>
        private PdfJavaScriptAction m_beforeSave = null;

        /// <summary>
        /// Internal variable to store action to be performed after the document saves.
        /// </summary>
        private PdfJavaScriptAction m_afterSave = null;

        /// <summary>
        /// Internal variable to store action to be performed before the document prints.
        /// </summary>
        private PdfJavaScriptAction m_beforePrint = null;

        /// <summary>
        /// Internal variable to store action to be performed after the document prints.
        /// </summary>
        private PdfJavaScriptAction m_afterPrint = null;

        /// <summary>
        /// Internal variable to store catalog.
        /// </summary>
        private PdfCatalog m_catalog = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentActions"/> class.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        internal PdfDocumentActions(PdfCatalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
            {
                throw new PdfConformanceException("Usage of Javascript are not allowed by the PDF/A1-B standard");
            }
#endif

            m_catalog = catalog;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the action to execute when the document is opened. 
        /// </summary>
        /// <value>A <see cref="PdfAction"/> specifying the action to be executed when documents opens in the viewer. </value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.AfterOpen = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.AfterOpen = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfAction AfterOpen
        {
            get
            {
                return m_afterOpen;
            }
           
            set
            {
                if (value != m_afterOpen)
                {
                    m_afterOpen = value;
                    PdfDictionary.SetProperty(m_catalog, DictionaryProperties.OpenAction, m_afterOpen);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed before the document is closed.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> object specifying the action to be executed before the document is closed. </value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.BeforeClose = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.BeforeClose = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction BeforeClose
        {
            get
            {
                return m_beforeClose;
            }
           
            set
            {
                if (value != m_beforeClose)
                {
                    m_beforeClose = value;
                    m_dictionary.SetProperty(DictionaryProperties.WC, m_beforeClose);
                }
            }
        }

        /// <summary>
        /// Gets or sets the java script action to be performed before the document is saved.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed before the document is saved. </value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.BeforeSave = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.BeforeSave = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction BeforeSave
        {
            get
            {
                return m_beforeSave;
            }
          
            set
            {
                if (value != m_beforeSave)
                {
                    m_beforeSave = value;
                    m_dictionary.SetProperty(DictionaryProperties.WS, m_beforeSave);
                }
            }
        }

        /// <summary>
        /// Gets or sets the jave script action to be performed after the document is saved.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed after the document is saved.</value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.AfterSave = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.AfterSave = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction AfterSave
        {
            get
            {
                return m_afterSave;
            }
           
            set
            {
                if (value != m_afterSave)
                {
                    m_afterSave = value;
                    m_dictionary.SetProperty(DictionaryProperties.DS, m_afterSave);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed before the document is printed.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed before the document is printed. </value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.BeforePrint = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.BeforePrint = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction BeforePrint
        {
            get
            {
                return m_beforePrint;
            }
            
            set
            {
                if (value != m_beforePrint)
                {
                    m_beforePrint = value;
                    m_dictionary.SetProperty(DictionaryProperties.WP, m_beforePrint);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed after the document is printed.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed after the document is printed. .</value>
        /// <example>
        /// <code lang="C#">
        ///  //Create a new PDF document
        ///  PdfDocument document = new PdfDocument();
        ///  //Create and add new launch Action to the document
        ///  PdfLaunchAction action = new PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte);
        ///  document.Actions.AfterPrint = action;
        ///  //Save document to disk
        ///  document.Save("DocumentAction.pdf");
        /// </code>
        /// <code lang="VB">
        ///  'Create a new PDF document.
        ///  Dim document As PdfDocument = New PdfDocument()
        ///  'Create and add new launch Action to the document.
        ///  Dim action As PdfLaunchAction = New PdfLaunchAction("myAction.txt", PdfFilePathType.Absoulte)
        ///  document.Actions.AfterPrint = action
        ///  'Save document to disk
        ///  document.Save("DocumentAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction AfterPrint
        {
            get
            {
                return m_afterPrint;
            }
           
            set
            {
                if (value != m_afterPrint)
                {
                    m_afterPrint = value;
                    m_dictionary.SetProperty(DictionaryProperties.DP, m_afterPrint);
                }
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
