#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;


/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Specifies the file path type.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new PdfButtonField.
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// //Set the bounds to submitButton.
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// //Set the submit button text.
    /// submitButton.Text = "Launch";
    /// //Create a new PdfLaunchAction and set the PdfFilePathType.
    /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute);
    /// //Set the actions to submit button.
    /// submitButton.Actions.GotFocus = launchAction;
    /// //Save document to disk.
    /// document.Save("ActionDestination.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new PdfButtonField.
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// 'Set the bounds to submit button.
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// 'Set the submit button text.
    /// submitButton.Text = "Launch"
    /// 'Create a new PdfLaunchAction and set the PdfFilePathType.
    /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute)
    /// 'Set the actions to submit button.
    /// submitButton.Actions.GotFocus = launchAction
    /// 'Save document to disk.
    /// document.Save("ActionDestination.pdf")
    /// </code>
    /// </example>
    public enum PdfFilePathType
    {
        /// <summary>
        /// Specifies the file location with out including the domain name.
        /// </summary>
        Relative,       

        /// <summary>
        /// Specifies the location, including the domain name.
        /// </summary>
        Absolute
    }

    /// <summary>
    /// Specifies the available named actions supported by the viewer. 
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
    /// //Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add();
    /// //Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonField.
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// //Set the bounds to submitButton.
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// //Set the font to submitButton.
    /// submitButton.Font = font;
    /// //Sets the submit button text.
    /// submitButton.Text = "First Page";
    /// //Set the back color to submit button.
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Create a new PdfNamedAction.
    /// PdfNamedAction namedAction = new PdfNamedAction(PdfActionDestination.FirstPage);
    /// //Set the named action destination.
    /// namedAction.Destination=PdfActionDestination.PrevPage;
    /// //Set the Actions  to namedAction.
    /// submitButton.Actions.GotFocus = namedAction;
    /// //Add the submitButton to the new document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("ActionDestination.pdf");
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
    /// 'Creates a new page and adds it as the last page of the document
    /// page = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// 'Set the bounds to submit button.
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// 'Set the font to submitButton.
    /// submitButton.Font = font
    /// 'Sets the submit button text.
    /// submitButton.Text = "First Page"
    /// 'Set the back color to submit button.
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Create a new PdfNamedAction
    /// Dim namedAction As PdfNamedAction  = new PdfNamedAction(PdfActionDestination.FirstPage)
    /// 'Set the named action.
    /// namedAction.Destination=PdfActionDestination.PrevPage
    /// 'Set the Actions  to namedAction.
    /// submitButton.Actions.GotFocus = namedAction
    /// 'Add the submitButton to the new document.
    /// document.Form.Fields.Add(gotoAction)
    /// 'Save document to disk.
    /// document.Save("ActionDestination.pdf")
    /// </code>
    /// </example>
    public enum PdfActionDestination
    {
        /// <summary>
        /// Navigate to first page.
        /// </summary>
        FirstPage,

        /// <summary>
        /// Navigate to last page.
        /// </summary>
        LastPage,

        /// <summary>
        /// Navigate to next page.
        /// </summary>
        NextPage,

        /// <summary>
        /// Navigate to previous page.
        /// </summary>
        PrevPage
    }

    /// <summary>
    /// Specifies the available data formats for submitting the form data.
    /// </summary>
    [Flags]
    public enum PdfSubmitFormFlags
    {
        /// <summary>
        /// If clear, the Fields array specifies which fields to
        /// include in the submission. (All descendants of the specified fields in
        /// the field hierarchy are submitted as well.)
        /// If set, the Fields array tells which fields to exclude. All fields in the
        /// document�s interactive form are submitted except those listed in the
        /// Fields array and those whose NoExport flag.
        /// </summary>
        IncludeExclude = 1,

        /// <summary>
        /// If set, all fields designated by the Fields array and the Include/
        /// Exclude flag are submitted, regardless of whether they have a value. 
        /// For fields without a value, only the
        /// field name is transmitted.
        /// </summary>
        IncludeNoValueFields = 2,

        /// <summary>
        /// Meaningful only if the SubmitPDF and XFDF flags are clear. If set,
        /// field names and values are submitted in HTML Form format. If
        /// clear, they are submitted in Forms Data Format
        /// </summary>
        ExportFormat = 4,

        /// <summary>
        /// If set, field names and values are submitted using an HTTP GET
        /// request. If clear, they are submitted using a POST request. This flag
        /// is meaningful only when the ExportFormat flag is set; if ExportFormat
        /// is clear, this flag must also be clear.
        /// </summary>
        GetMethod = 8,

        /// <summary>
        /// If set, the coordinates of the mouse click that caused the submitform
        /// action are transmitted as part of the form data. The coordinate
        /// values are relative to the upper-left corner of the field�s widget annotation
        /// rectangle.
        /// </summary>
        SubmitCoordinates = 16,

        /// <summary>
        /// Meaningful only if the SubmitPDF flags are clear. If set,
        /// field names and values are submitted as XML Forms Data Format .
        /// </summary>
        Xfdf = 32,

        /// <summary>
        /// Meaningful only when the form is being submitted in
        /// Forms Data Format (that is, when both the XFDF and ExportFormat
        /// flags are clear). If set, the submitted FDF file includes the contents
        /// of all incremental updates to the underlying PDF document,
        /// as contained in the Differences entry in the FDF dictionary.
        /// If clear, the incremental updates are not included.
        /// </summary>
        IncludeAppendSaves = 64,

        /// <summary>
        /// Meaningful only when the form is being submitted in
        /// Forms Data Format (that is, when both the XFDF and ExportFormat
        /// flags are clear). If set, the submitted FDF file includes all markup
        /// annotations in the underlying PDF document.
        /// If clear, markup annotations are not included.
        /// </summary>
        IncludeAnnotations = 128,

        /// <summary>
        /// If set, the document is submitted as PDF, using the
        /// MIME content type application/pdf (described in Internet RFC
        /// 2045, Multipurpose Internet Mail Extensions (MIME), Part One:
        /// Format of Internet Message Bodies; see the Bibliography). If set, all
        /// other flags are ignored except GetMethod.
        /// </summary>
        SubmitPdf = 256,

        /// <summary>
        /// If set, any submitted field values representing dates are
        /// converted to the standard format described.
        /// </summary>
        CanonicalFormat = 512,

        /// <summary>
        /// Meaningful only when the form is being submitted in
        /// Forms Data Format (that is, when both the XFDF and
        /// ExportFormat flags are clear) and the IncludeAnnotations flag is
        /// set. If set, it includes only those markup annotations whose T entry
        /// matches the name of the current user, as determined
        /// by the remote server to which the form is being submitted.
        /// </summary>
        ExclNonUserAnnots = 1024,

        /// <summary>
        /// Meaningful only when the form is being submitted in
        /// Forms Data Format (that is, when both the XFDF and ExportFormat
        /// flags are clear). If set, the submitted FDF excludes the F entry.
        /// </summary>
        ExclFKey = 2048,

        /// <summary>
        /// Meaningful only when the form is being submitted in
        /// Forms Data Format (that is, when both the XFDF and ExportFormat
        /// flags are clear). If set, the F entry of the submitted FDF is a file
        /// specification containing an embedded file stream representing the
        /// PDF file from which the FDF is being submitted.
        /// </summary>
        EmbedForm = 4096
    }
}