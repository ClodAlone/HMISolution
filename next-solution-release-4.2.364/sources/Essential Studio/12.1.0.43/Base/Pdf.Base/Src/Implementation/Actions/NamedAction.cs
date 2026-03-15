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
    /// Represents an action which perfoms the named action.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add();
    /// //Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "First Page";
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Create a new PdfNamedAction
    /// PdfNamedAction namedAction = new PdfNamedAction(PdfActionDestination.FirstPage);
    /// //Set the named action to submit button
    /// submitButton.Actions.GotFocus = namedAction;
    /// //Add the submitButton to the new document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("NamedAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add()
    /// 'Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "First Page"
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Create a new PdfNamedAction
    /// Dim namedAction As PdfNamedAction  = new PdfNamedAction(PdfActionDestination.FirstPage)
    /// 'Set the named action to submit button
    /// submitButton.Actions.GotFocus = namedAction
    /// 'Add the gotoAction to the document
    /// document.Form.Fields.Add(gotoAction)
    /// 'Save document to disk.
    /// document.Save("NamedAction.pdf")
    /// </code>
    /// </example>
    public class PdfNamedAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store destination.
        /// </summary>
        private PdfActionDestination m_destination = PdfActionDestination.NextPage;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The <see cref="PdfActionDestination"/> object representing destination of an action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "First Page";
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Create a new PdfNamedAction.
        /// PdfNamedAction namedAction = new PdfNamedAction(PdfActionDestination.FirstPage);
        /// //Set the PdfActionDestination
        /// namedAction.Destination=PdfActionDestination.PrevPage;
        /// //Set the named action to submit button
        /// submitButton.Actions.GotFocus = namedAction;
        /// //Add the submit button to a new document
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("NamedAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "First Page"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Crea a new PdfNamedAction
        /// Dim namedAction As PdfNamedAction  = new PdfNamedAction(PdfActionDestination.FirstPage)
        /// 'Set the PdfActionDestination
        /// namedAction.Destination=PdfActionDestination.PrevPage
        /// 'Set the named action to submit button
        /// submitButton.Actions.GotFocus = namedAction
        /// Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("NamedAction.pdf")
        /// </code>
        /// </example>
        public PdfActionDestination Destination
        {
            get
            {
                return m_destination;
            }
            
            set
            {
                if (m_destination != value)
                {
                    m_destination = value;
                    Dictionary.SetName(DictionaryProperties.N, m_destination.ToString());
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNamedAction"/> class.
        /// </summary>
        /// <param name="destination">The <see cref="PdfActionDestination"/> object representing destination of an action.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "First Page";
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Create a new PdfNamedAction.
        /// PdfNamedAction namedAction = new PdfNamedAction(PdfActionDestination.FirstPage);
        /// //Set the PdfActionDestination
        /// namedAction.Destination=PdfActionDestination.PrevPage;
        /// //Set the named action to submit button
        /// submitButton.Actions.GotFocus = namedAction;
        /// //Add the submit button to a new document
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("NamedAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "First Page"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Crea a new PdfNamedAction
        /// Dim namedAction As PdfNamedAction  = new PdfNamedAction(PdfActionDestination.FirstPage)
        /// 'Set the PdfActionDestination
        /// namedAction.Destination=PdfActionDestination.PrevPage
        /// 'Set the named action to submit button
        /// submitButton.Actions.GotFocus = namedAction
        /// Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("NamedAction.pdf")
        /// </code>
        /// </example>
        public PdfNamedAction(PdfActionDestination destination)
            : base()
        {
            Destination = destination;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.Named));
            Dictionary.SetProperty(DictionaryProperties.N, new PdfName(m_destination.ToString()));
        }
        #endregion
    }
}
