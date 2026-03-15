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
    /// Represents Pdf form's reset action.
    /// </summary>
    /// <remarks>This action allows a user to reset the form fields to their default values. </remarks>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a PdfButtonField 
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Apply";
    /// //Create a new PdfResetAction
    /// PdfResetAction resetAction = new PdfResetAction();
    /// //Set the resetAction to submitButton
    /// submitButton.Actions.GotFocus = resetAction;
    /// //Add the submit button to a document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("ResetAction.pdf");
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
    /// 'Create a new PdfResetAction
    /// PdfResetAction resetAction = new PdfResetAction()
    /// 'Set the resetAction to submitButton
    /// submitButton.Actions.GotFocus = resetAction
    /// 'Add the submitButton to document
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("ResetAction.pdf")
    /// </code>
    /// </example>
    public class PdfResetAction : PdfFormAction
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfResetAction"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a PdfButtonField 
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// //Create a new PdfResetAction
        /// PdfResetAction resetAction = new PdfResetAction();
        /// //Set the resetAction to submitButton
        /// submitButton.Actions.GotFocus = resetAction;
        /// //Add the submit button to a document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("ResetAction.pdf");
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
        /// 'Create a new PdfResetAction
        /// PdfResetAction resetAction = new PdfResetAction()
        /// 'Set the resetAction to submitButton
        /// submitButton.Actions.GotFocus = resetAction
        /// 'Add the submitButton to document
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("ResetAction.pdf")
        /// </code>
        /// </example>
        public PdfResetAction()
            : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether fields contained in Fields
        /// collection will be included for resetting.
        /// </summary>
        /// <value><c>true</c> if include; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If Include property is true, only the fields in this collection will be reset.
        /// If Include property is false, the fields in this collection are not reset
        /// and only the remaining form fields are reset.
        /// If the collection is null or empty, then all the form fields are reset
        /// and the Include property is ignored.
        /// </remarks>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Reset";
        /// //Create a new resetAction
        /// PdfResetAction resetAction = new PdfResetAction();
        /// resetAction.Include=true;
        /// submitButton.Actions.GotFocus = resetAction;
        /// //Add the submit button to a document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("ResetAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Reset"
        /// 'Create a new resetAction
        /// PdfResetAction resetAction = new PdfResetAction()
        /// resetAction.Include=True
        /// submitButton.Actions.GotFocus = resetAction
        /// 'Add the submitButton to document
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("ResetAction.pdf")
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
                    Dictionary.SetNumber(DictionaryProperties.Flags, base.Include ? 0 : 1);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.ResetForm));
        }
        #endregion
    }
}
