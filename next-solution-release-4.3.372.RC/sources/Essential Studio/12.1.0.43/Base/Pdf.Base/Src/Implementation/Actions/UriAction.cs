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
    /// Represents an action which resolves unique resource identifier.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Apply";
    /// //Create a new PdfUriAction
    /// PdfUriAction uriAction = new PdfUriAction("http://www.google.com");
    /// submitButton.Actions.GotFocus = uriAction;
    /// document.Form.Fields.Add(submitButton);
    /// //Save the document to disk.
    /// document.Save("UriAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Text = "Apply"
    /// 'Create a new PdfUriAction
    /// Dim uriAction As PdfUriAction  = New PdfUriAction("http://www.google.com");
    /// submitButton.Actions.GotFocus = javaAction
    /// document.Form.Fields.Add(submitButton)
    /// 'Save the document to disk.
    /// document.Save("UriAction.pdf")
    /// </code>
    /// </example>
    public class PdfUriAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store unique resource identifier.
        /// </summary>
        private string m_uri = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUriAction"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfUriAction
        /// PdfUriAction uriAction = new PdfUriAction();
        /// submitButton.Actions.GotFocus = uriAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save the document to disk.
        /// document.Save("UriAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfUriAction
        /// Dim uriAction As PdfUriAction  = New PdfUriAction()
        /// document.Form.Fields.Add(submitButton)
        /// 'Save the document to disk.
        /// document.Save("UriAction.pdf")
        /// </code>
        /// </example>
        public PdfUriAction()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfUriAction"/> class.
        /// </summary>
        /// <param name="uri">The unique resource identifier.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfUriAction
        /// PdfUriAction uriAction = new PdfUriAction("http://www.google.com");
        /// submitButton.Actions.GotFocus = uriAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save the document to disk.
        /// document.Save("UriAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfUriAction
        /// Dim uriAction As PdfUriAction  = New PdfUriAction("http://www.google.com")
        /// document.Form.Fields.Add(submitButton)
        /// 'Save the document to disk.
        /// document.Save("UriAction.pdf")
        /// </code>
        /// </example>
        public PdfUriAction(string uri)
            : base()
        {
            Uri = uri;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique resource identifier.
        /// </summary>
        /// <value>The unique resource identifier.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfUriAction
        /// PdfUriAction uriAction = new PdfUriAction();
        /// uriAction.Uri="http://www.google.com";
        /// submitButton.Actions.GotFocus = uriAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save the document to disk.
        /// document.Save("UriAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfUriAction
        /// Dim uriAction As PdfUriAction  = New PdfUriAction()
        /// uriAction.Uri="http://www.google.com";
        /// submitButton.Actions.GotFocus = javaAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save the document to disk.
        /// document.Save("UriAction.pdf")
        /// </code>
        /// </example>
        public string Uri
        {
            get
            {
                return m_uri;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("uri");
                }

                if (m_uri != value)
                {
                    m_uri = value;
                    Dictionary.SetString(DictionaryProperties.URI, m_uri);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.URI));
        }
        #endregion
    }
}
