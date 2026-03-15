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
    /// Represents an action which goes to a destination in the current document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "Goto";
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage pdfPages=document.Pages.Add();
    /// //Create a new PdfGoToAction
    /// PdfGoToAction gotoAction = new PdfGoToAction(pdfPages);
    /// //Add the gotoAction
    /// gotoAction.Actions.GotFocus = gotoAction;
    /// //Add the submit button to a new document.
    /// document.Form.Fields.Add(gotoAction);
    /// //Save document to disk.
    /// document.Save("GoToAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "Goto"
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As pdfpage = document.Pages.Add()
    /// 'Create a new PdfGoToAction
    /// PdfGoToAction gotoAction = new PdfGoToAction(pdfpage)
    /// 'Add the gotoAction to submitButton
    /// gotoAction.Actions.GotFocus = gotoAction
    /// 'Add the submit button to the new document
    /// document.Form.Fields.Add(gotoAction)
    /// 'Save document to disk.
    /// document.Save("GoToAction.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfAction"/> Class
    public class PdfGoToAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store action's destination.
        /// </summary>
        private PdfDestination m_destination = null;
        #endregion
      
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGoToAction"/> class.
        /// </summary>
        /// <param name="destination">The destination to jump to.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Font = font;
        /// submitButton.Text = "Goto";
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage pdfPages=document.Pages.Add();
        /// //Create a new PdfDestination
        /// PdfDestination destination = new PdfDestination(pdfPages);
        /// //Set the PdfDestinationMode
        /// destination.Mode = PdfDestinationMode.FitToPage;
        /// //Create a new PdfGoToAction
        /// PdfGoToAction gotoAction = new PdfGoToAction(destination);
        /// //Set the gotoAction to submitButton
        /// submitButton.Actions.GotFocus = gotoAction;
        /// //Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("GoToAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Create a new PdfButtonField.
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Goto"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As pdfpage = document.Pages.Add()
        /// 'Create a new PdfDestination.
        /// Dim destination As PdfDestination  = New PdfDestination(pdfpage)
        /// 'Set the PdfDestinationMode.
        /// destination.Mode = PdfDestinationMode.FitToPage
        /// 'Create a new PdfGoToAction
        /// PdfGoToAction gotoAction = new PdfGoToAction(destination)
        /// 'Set the action  to submit button
        /// submitButton.Actions.GotFocus = resetAction
        /// 'Add the submitButton to a new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("GoToAction.pdf")
        /// </code>
        /// </example>
        public PdfGoToAction(PdfDestination destination)
            : base()
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            m_destination = destination;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGoToAction"/> class.
        /// </summary>
        /// <param name="page">The page to jump to.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Font = font;
        /// submitButton.Text = "Goto";
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// PdfGoToAction gotoAction = new PdfGoToAction(page);
        /// Set the gotoAction to submitButton.
        /// submitButton.Actions.GotFocus = gotoAction;
        /// //Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("GoToAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Create a new PdfButtonField.
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Goto"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As pdfpage = document.Pages.Add()
        /// 'Create a new PdfGoToAction
        /// PdfGoToAction gotoAction = new PdfGoToAction(pdfpage)
        /// 'Set the action to submit button
        /// submitButton.Actions.GotFocus = resetAction
        /// 'Add the submit button to the new document
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("GoToAction.pdf")
        /// </code>
        /// </example>
        public PdfGoToAction(PdfPage page)
            : base()
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            m_destination = new PdfDestination(page);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage pdfPages=document.Pages.Add();
        /// //Create a new PdfDestination
        /// PdfDestination destination = new PdfDestination(pdfPages);
        /// //Set the PdfDestinationMode
        /// destination.Mode = PdfDestinationMode.FitToPage;
        /// //Create a new PdfGoToAction
        /// PdfGoToAction gotoAction = new PdfGoToAction(pdfPages);
        /// //Set the Destination to gotoAction
        /// gotoAction.Destination=destination;
        /// //Set the action to the goto action
        /// submitButton.Actions.GotFocus = gotoAction;
        /// //Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As pdfpage = document.Pages.Add()
        /// 'Create a new PdfDestination.
        /// Dim destination As PdfDestination  = New PdfDestination(pdfpage)
        /// 'Set the PdfDestinationMode
        /// destination.Mode = PdfDestinationMode.FitToPage
        /// 'Creat a new PdfGoToAction
        /// PdfGoToAction gotoAction = new PdfGoToAction(pdfpage)
        /// gotoAction.Destination=destination
        /// 'Set the action to the goto action
        /// submitButton.Actions.GotFocus = resetAction
        /// 'Add the submit button to the new document
        /// document.Form.Fields.Add(submitButton)
        /// </code>
        /// </example>
        public PdfDestination Destination
        {
            get
            {
                return m_destination;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Destination");
                }

                if (value != m_destination)
                {
                    m_destination = value;
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
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.GoTo));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Dictionary.SetProperty(DictionaryProperties.D, m_destination);
        }
        #endregion
    }
}
