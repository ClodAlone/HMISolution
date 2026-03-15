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
    /// Represents an action which performs java script action in pdf document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonFieldd
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "Apply";
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Create a new PdfJavaScriptAction
    /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
    /// //Set the javaAction to submit button
    /// submitButton.Actions.GotFocus = javaAction;
    /// //Add the  submit button to the new document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("JavaScriptAction.pdf");
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
    /// submitButton.Text = "Apply"
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Create a new PdfJavaScriptAction.
    /// Dim javaAction As PdfJavaScriptAction  = New PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")")
    /// 'Set the javaAction to  submit button
    /// submitButton.Actions.GotFocus = javaAction
    /// 'Add the submit button to the new document.
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("JavaScriptAction.pdf")
    /// </code>
    /// </example>
    public class PdfJavaScriptAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store java script code.
        /// </summary>
        private string m_javaScript = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfJavaScriptAction"/> class.
        /// </summary>
        /// <param name="javaScript">The java script code.</param>
        /// <value>A string value representing valid javascript code to be executed.</value>
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
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Create a new PdfJavaScriptAction
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// set the java action to submit button
        /// submitButton.Actions.GotFocus = javaAction;
        /// //Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("JavaScriptAction.pdf");
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
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Create a new PdfJavaScriptAction
        /// Dim javaAction As PdfJavaScriptAction  = New PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")")
        /// 'Set the javaAction to submitButton.
        /// submitButton.Actions.GotFocus = javaAction
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction(string javaScript)
            : base()
        {
            if (javaScript == null)
            {
                throw new ArgumentNullException("javaScript");
            }

            JavaScript = javaScript;
        }
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the javascript code to be executed when this action is executed. 
        /// </summary>
        /// <value>A string value representing valid javascript code to be executed. </value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        /// submitButton.BackColor = new PdfColor(181, 191, 203);
        /// //Create a new PdfJavaScriptAction
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// //Set the javaAction to submitButton
        /// submitButton.Actions.GotFocus = javaAction;
        /// ''Get the JavaScript form javaAction
        /// string javaScript=javaAction.JavaScript;
        /// //Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("JavaScriptAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// 'Create a new PdfJavaScriptAction
        /// Dim javaAction As PdfJavaScriptAction  = New PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")")
        /// 'Set the javaAction to submit button
        /// submitButton.Actions.GotFocus = javaAction
        /// 'Get the JavaScript from java action
        /// Dim javaScript As string = javaAction.JavaScript;
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public string JavaScript
        {
            get
            {
                return m_javaScript;
            }

            set
            {
                if (m_javaScript != value)
                {
                    m_javaScript = value;
                    Dictionary.SetString(DictionaryProperties.JS, m_javaScript);
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
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.JavaScript));
            Dictionary.SetProperty(DictionaryProperties.JS, new PdfString(m_javaScript));
        }
        #endregion
    }
}
