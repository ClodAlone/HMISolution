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
    /// Represents Pdf form's submit action.
    /// </summary>
    /// <remarks>This type of action allows a user to go to a resource on the Internet, tipically a hypertext link. </remarks>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Apply";
    /// //Create a new PdfSubmitAction
    /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
    /// submitAction.DataFormat = SubmitDataFormat.Html;
    /// submitButton.Actions.GotFocus = submitAction;
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("SubmitAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Text = "Apply"
    /// 'Create a new PdfSubmitAction
    /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
    /// submitAction.DataFormat = SubmitDataFormat.Html;
    /// submitButton.Actions.GotFocus = submitAction
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("SubmitAction.pdf")
    /// </code>
    /// </example>
    public class PdfSubmitAction : PdfFormAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file name.
        /// </summary>
        private string m_fileName = String.Empty;

        /// <summary>
        /// Internal variable to store submit flags.
        /// </summary>
        private PdfSubmitFormFlags m_flags = 0;

        /// <summary>
        /// Internal variable to store Http method.
        /// </summary>
        private HttpMethod m_httpMethod = HttpMethod.Post;

        /// <summary>
        /// Internal variable to store value whether to submit dates in canonical format.
        /// </summary>
        private bool m_canonicalDateTimeFormat = false;

        /// <summary>
        /// Internal variable to store value whether to submit mouse pointer coordinates.
        /// </summary>
        private bool m_submitCoordinates = false;

        /// <summary>
        /// Internal variable to store value whether to submit fields without values.
        /// </summary>
        private bool m_includeNoValueFields = false;

        /// <summary>
        /// Internal variable to store value whether to submit incremental updates.
        /// </summary>
        private bool m_includeIncrementalUpdates = false;

        /// <summary>
        /// Internal variable to store value indicating whether to submit annotations.
        /// </summary>
        private bool m_includeAnnotations = false;

        /// <summary>
        /// Internal variable to store value indicating whether to exclude non-user annotations.
        /// </summary>
        private bool m_excludeNonUserAnnotations = false;

        /// <summary>
        /// Internal variable to store value indicating whether to embed form.
        /// </summary>
        private bool m_embedForm = false;

        /// <summary>
        /// Internal variable to store submit data format.
        /// </summary>
        private SubmitDataFormat m_dataFormat = SubmitDataFormat.Fdf;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSubmitAction"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// submitAction.DataFormat = SubmitDataFormat.Html;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// submitAction.DataFormat = SubmitDataFormat.Html;
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public PdfSubmitAction(string url)
            : base()
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (url.Length <= 0)
            {
                throw new ArgumentException("The URL can't be an empty string.", "url");
            }

            m_fileName = url;
            Dictionary.SetProperty(DictionaryProperties.F, new PdfString(m_fileName));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets an Url address where the data should be transferred.
        /// </summary>          
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// //Gets the url from the submit action
        /// string url=submitAction.Url;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        // 'Gets the url form the submit action
        // Dim url As String=submitAction.Url
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        /// <value>An string value specifying the full URI for the internet resource. </value>
        public string Url
        {
            get
            {
                return m_fileName;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>The HTTP method.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the HttpMethod to submit action
        /// submitAction.HttpMethod=HttpMethod.Post;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the HttpMethod to submit action
        /// submitAction.HttpMethod=HttpMethod.Post
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public HttpMethod HttpMethod
        {
            get
            {
                return m_httpMethod;
            }
            
            set
            {
                if (m_httpMethod != value)
                {
                    m_httpMethod = value;

                    if (m_httpMethod == HttpMethod.Get)
                    {
                        m_flags |= PdfSubmitFormFlags.GetMethod;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.GetMethod;
                    }
                }
            }
        }

        /// <summary>
        /// If set, any submitted field values representing dates are converted to the 
        /// standard format. The interpretation of a form field as a date is not specified 
        /// explicitly in the field itself but only in the JavaScript code that processes it.
        /// </summary>
        /// <value>
        /// <c>true</c> if use canonical date time format when submit data; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the CanonicalDateTimeFormat to submit action
        /// submitAction.CanonicalDateTimeFormat=true;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the CanonicalDateTimeFormat to submit action
        /// submitAction.CanonicalDateTimeFormat=True
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool CanonicalDateTimeFormat
        {
            get
            {
                return m_canonicalDateTimeFormat;
            }
           
            set
            {
                if (m_canonicalDateTimeFormat != value)
                {
                    m_canonicalDateTimeFormat = value;

                    if (m_canonicalDateTimeFormat)
                    {
                        m_flags |= PdfSubmitFormFlags.CanonicalFormat;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.CanonicalFormat;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to submit mouse pointer coordinates. If set, 
        /// the coordinates of the mouse click that caused the submit-form action are transmitted 
        /// as part of the form data. The coordinate values are relative to the upper-left corner 
        /// of the field�s widget annotation rectangle.
        /// </summary>
        /// <value><c>true</c> if submit coordinates; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the SubmitCoordinates to submit action
        /// submitAction.SubmitCoordinates=true;
        /// submitButton.Actions.GotFocus = submit action;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the SubmitCoordinates to submit action
        /// submitAction.SubmitCoordinates=True
        /// submitButton.Actions.GotFocus = submit action;
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool SubmitCoordinates
        {
            get
            {
                return m_submitCoordinates;
            }
           
            set
            {
                if (m_submitCoordinates != value)
                {
                    m_submitCoordinates = value;

                    if (m_submitCoordinates)
                    {
                        m_flags |= PdfSubmitFormFlags.SubmitCoordinates;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.SubmitCoordinates;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to submit fields without value.
        /// If set, all fields designated by the Fields collection and the <see cref="Include"/>
        /// flag are submitted, regardless of whether they have a value. For fields without a 
        /// value, only the field name is transmitted.
        /// </summary>
        /// <value>
        /// <c>true</c> if submit fields without value or the empty ones; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the IncludeNoValueFields to submit action
        /// submitAction.IncludeNoValueFields=true;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the IncludeNoValueFields to submit action
        /// submitAction.IncludeNoValueFields=true
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool IncludeNoValueFields
        {
            get
            {
                return m_includeNoValueFields;
            }
            
            set
            {
                if (m_includeNoValueFields != value)
                {
                    m_includeNoValueFields = value;

                    if (m_includeNoValueFields)
                    {
                        m_flags |= PdfSubmitFormFlags.IncludeNoValueFields;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.IncludeNoValueFields;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to submit form's incremental updates.
        /// Meaningful only when the form is being submitted in Forms Data Format.
        /// If set, the submitted FDF file includes the contents of all incremental 
        /// updates to the underlying PDF document. If clear, the incremental updates are 
        /// not included.
        /// </summary>
        /// <value>
        /// <c>true</c> if incremental updates should be submitted; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the IncludeIncrementalUpdates to submit action
        /// submitAction.IncludeIncrementalUpdates=true;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the IncludeIncrementalUpdates to submit action
        /// submitAction.IncludeIncrementalUpdates=True
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool IncludeIncrementalUpdates
        {
            get
            {
                return m_includeIncrementalUpdates;
            }
            
            set
            {
                if (m_includeIncrementalUpdates != value)
                {
                    m_includeIncrementalUpdates = value;

                    if (m_includeIncrementalUpdates)
                    {
                        m_flags |= PdfSubmitFormFlags.IncludeAppendSaves;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.IncludeAppendSaves;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to submit annotations.
        /// Meaningful only when the form is being submitted in Forms Data Format.
        /// If set, the submitted FDF file includes all markup annotations in the 
        /// underlying PDF document. If clear, markup annotations are not included.
        /// </summary>
        /// <value><c>true</c> if annotations should be submitted; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the IncludeAnnotations to submit action
        /// submitAction.IncludeAnnotations=true;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the IncludeAnnotations to submit action
        /// submitAction.IncludeAnnotations=True
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool IncludeAnnotations
        {
            get
            {
                return m_includeAnnotations;
            }
           
            set
            {
                if (m_includeAnnotations != value)
                {
                    m_includeAnnotations = value;

                    if (m_includeAnnotations)
                    {
                        m_flags |= PdfSubmitFormFlags.IncludeAnnotations;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.IncludeAnnotations;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude non user annotations form submit 
        /// data stream. Meaningful only when the form is being submitted in Forms Data Format 
        /// and the <see cref="IncludeAnnotations"/> property is set to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if non user annotations should be excluded; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// 'Sets the ExcludeNonUserAnnotations to submit action
        /// submitAction.ExcludeNonUserAnnotations=true;
        /// submitButton.Actions.GotFocus = submitAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the ExcludeNonUserAnnotations to submit action
        /// submitAction.ExcludeNonUserAnnotations=True
        /// submitButton.Actions.GotFocus = submitAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool ExcludeNonUserAnnotations
        {
            get
            {
                return m_excludeNonUserAnnotations;
            }
           
            set
            {
                if (m_excludeNonUserAnnotations != value)
                {
                    m_excludeNonUserAnnotations = value;

                    if (m_excludeNonUserAnnotations)
                    {
                        m_flags |= PdfSubmitFormFlags.ExclNonUserAnnots;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.ExclNonUserAnnots;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include form to submit data stream.
        /// Meaningful only when the form is being submitted in Forms Data Format.
        /// If set, the <see cref="Url"/> property is a file name containing an embedded file 
        /// stream representing the PDF file from which the FDF is being submitted.
        /// </summary>
        /// <value><c>true</c> if form should be embedded to submit stream; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        // //Sets the EmbedForm option to submit action
        //  submitAction.EmbedForm=true;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// //Sets the EmbedForm option to submit action
        ///  submitAction.EmbedForm=True
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public bool EmbedForm
        {
            get
            {
                return m_embedForm;
            }
           
            set
            {
                if (m_embedForm != value)
                {
                    m_embedForm = value;

                    if (m_embedForm)
                    {
                        m_flags |= PdfSubmitFormFlags.EmbedForm;
                    }
                    else
                    {
                        m_flags &= ~PdfSubmitFormFlags.EmbedForm;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the submit data format.
        /// </summary>
        /// <value>The submit data format.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        // //Sets the SubmitDataFormat option to submit action
        //  submitAction.DataFormat=SubmitDataFormat.Html;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the SubmitDataFormat option to submit action
        /// submitAction.DataFormat=SubmitDataFormat.Html
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public SubmitDataFormat DataFormat
        {
            get
            {
                return m_dataFormat;
            }
            
            set
            {
                if (m_dataFormat != value)
                {
                    m_dataFormat = value;

                    switch (m_dataFormat)
                    {
                        case SubmitDataFormat.Pdf:
                            m_flags |= PdfSubmitFormFlags.SubmitPdf;
                            m_flags &= ~(PdfSubmitFormFlags.Xfdf & PdfSubmitFormFlags.ExportFormat);
                            break;

                        case SubmitDataFormat.Xfdf:
                            m_flags |= PdfSubmitFormFlags.Xfdf;
                            m_flags &= ~(PdfSubmitFormFlags.SubmitPdf & PdfSubmitFormFlags.ExportFormat);
                            break;

                        case SubmitDataFormat.Html:
                            m_flags |= PdfSubmitFormFlags.ExportFormat;
                            m_flags &= ~(PdfSubmitFormFlags.SubmitPdf & PdfSubmitFormFlags.Xfdf);
                            break;

                        case SubmitDataFormat.Fdf:
                            m_flags &= ~(PdfSubmitFormFlags.SubmitPdf & PdfSubmitFormFlags.Xfdf &
                                PdfSubmitFormFlags.ExportFormat);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fields contained in Fields
        /// collection will be included for submitting.
        /// </summary>
        /// <value><c>true</c> if include; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If Include property is true, only the fields in this collection will be submitted.
        /// If Include property is false, the fields in this collection are not submitted
        /// and only the remaining form fields are submitted.
        /// If the collection is null or empty, then all the form fields are reset
        /// and the Include property is ignored.
        /// If the field has Export property set to false it will be not included for 
        /// submitting in any case.
        /// </remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfSubmitAction
        /// PdfSubmitAction submitAction = new PdfSubmitAction("http://stevex.net/dump.php");
        /// //Sets the Include option to submit action
        /// submitAction.Include=true;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SubmitAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create a new PdfSubmitAction
        /// Dim submitAction As PdfSubmitAction = new PdfSubmitAction("http://stevex.net/dump.php")
        /// 'Sets the Include option to submit action
        /// submitAction.Include=True
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SubmitAction.pdf")
        /// </code>
        /// </example>
        public override bool Include
        {
            get
            {
                return base.Include;
            }
           
            set
            {
                if (base.Include != value)
                {
                    base.Include = value;

                    if (base.Include)
                    {
                        m_flags &= ~PdfSubmitFormFlags.IncludeExclude;
                    }
                    else
                    {
                        m_flags |= PdfSubmitFormFlags.IncludeExclude;
                    }
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

            Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.SubmitForm));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Dictionary.SetProperty(DictionaryProperties.Flags, new PdfNumber((int)m_flags));
        }
        #endregion
    }
}
