#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the form action base class.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonField.
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// //Sets the bounds
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// //Set the submitButton Font.
    /// submitButton.Font = font;
    /// //Set the submit button text
    /// submitButton.Text = "Apply";
    /// //Set the submit button backcolor
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Create a new PdfFormAction.
    /// PdfFormAction formAction = new PdfFormAction();
    /// form.Include = true;
    /// //Set the submit button action.
    /// submitButton.Actions.GotFocus = formAction;
    /// //Add the submit button to the new documnet.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("FormAction.pdf");
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
    /// 'Sets the bounds
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// 'Set the submitButton Font.
    /// submitButton.Font = font
    /// 'Set the submit button text
    /// submitButton.Text = "Apply"
    /// 'Set the submit button backcolor
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Create a new PdfFormAction.
    /// Dim formAction As PdfFormAction  = New PdfFormAction()
    /// 'Set the Include option in formAction
    /// form.Include = true;
    /// 'Set the submit button action.
    /// submitButton.Actions.GotFocus = formAction
    /// 'Add the submit button to the new document.
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("FormAction.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfAction"/>
    public class PdfFormAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store value indicating whether to include
        /// or exclude fields for resetting process.
        /// </summary>
        private bool m_include = false;

        /// <summary>
        /// Internal variable to store fields affected by the action.
        /// </summary>
        private PdfFieldCollection m_fields = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFormAction"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Create a new PdfButtonField.
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// //Sets the bounds
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// //Set the submitButton Font.
        /// submitButton.Font = font;
        /// //Set the submit button text
        /// submitButton.Text = "Apply";
        /// //Set the submit button backcolor
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Create a new PdfFormAction.
        /// PdfFormAction formAction = new PdfFormAction();
        /// form.Include = true;
        /// //Set the submit button action.
        /// submitButton.Actions.GotFocus = formAction;
        /// //Add the submit button to the new documnet.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("FormAction.pdf");
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
        /// 'Sets the bounds
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// 'Set the submitButton Font.
        /// submitButton.Font = font
        /// 'Set the submit button text
        /// submitButton.Text = "Apply"
        /// 'Set the submit button backcolor
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Create a new PdfFormAction.
        /// Dim formAction As PdfFormAction  = New PdfFormAction()
        /// 'Set the Include option in formAction
        /// form.Include = true;
        /// 'Set the submit button action.
        /// submitButton.Actions.GotFocus = formAction
        /// 'Add the submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("FormAction.pdf")
        /// </code>
        /// </example>
        public PdfFormAction()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether fields contained in <see cref="Fields"/> 
        /// collection will be included for resetting or submitting.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField.
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// //Set the submitButton Bounds.
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// //Create a new PdfFormAction
        /// PdfFormAction formAction = new PdfFormAction();
        /// form.Include = true;
        /// //Set the actiont to the submitButton
        /// submitButton.Actions.GotFocus = formAction;
        /// //Add the submit button to the new documnet.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("FormAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField.
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// 'Set the submitButton bounds.
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// 'Create a new PdfFormAction
        /// Dim formAction As PdfFormAction  = New PdfFormAction()
        /// form.Include = true;
        /// 'Set the submit button action.
        /// submitButton.Actions.GotFocus = formAction
        /// 'Add the submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("FormAction.pdf")
        /// </code>
        /// </example>
        /// <remarks>
        /// If Include property is true, only the fields in this collection will be reset or submitted.
        /// If Include property is false, the fields in this collection are not reset or submitted 
        /// and only the remaining form fields are reset or submitted.
        /// If the collection is null or empty, then all the form fields are reset 
        /// and the Include property is ignored.
        /// </remarks>
        /// <value><c>true</c> if include; otherwise, <c>false</c>.</value>
        public virtual bool Include
        {
            get
            {
                return m_include;
            }
           
            set
            {
                m_include = value;
            }
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField.
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// //Set the submitButton Bounds.
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// //Create a new PdfFormAction.
        /// PdfFormAction formAction = new PdfFormAction();
        /// //Gets the FieldCollection from formAction
        /// PdfFieldCollection fields=formAction.Fileds;
        /// //Set actions to submit button 
        /// submitButton.Actions.GotFocus = formAction;
        /// //Add the submit button to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Add the submit button to the new documnet.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("FormAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// 'Set the boudns to submitButton
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// 'Create the new PdfFormAction
        /// Dim formAction As PdfFormAction  = New PdfFormAction()
        /// 'Gets the FieldCollection from formAction
        /// Dim fields As PdfFieldCollection = formAction.Fileds;
        /// 'Set actions to submit button 
        /// submitButton.Actions.GotFocus = formAction
        /// 'Add the submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("FormAction.pdf")
        /// </code>
        /// </example>
        public PdfFieldCollection Fields
        {
            get
            {
                if (m_fields == null)
                {
                    m_fields = new PdfFieldCollection();
                    Dictionary.SetProperty(DictionaryProperties.Fields, m_fields);
                }

                return m_fields;
            }
        }
        #endregion
    }
}
