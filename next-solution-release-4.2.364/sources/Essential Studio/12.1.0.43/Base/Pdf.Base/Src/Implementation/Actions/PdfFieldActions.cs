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
    /// Represents actions to be performed as response to field events. 
    /// </summary>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Launch";
    /// //Create a new PdfLaunchAction
    /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
    /// //Set the launch action to submit button
    /// submitButton.Actions.MouseEnter = launchAction;
    /// Add the submit button to a new document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("FieldActions.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new  PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Text = "Launch"
    /// 'Create a new PdfLaunchAction
    /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
    /// 'Set the launch action to submit button
    /// submitButton.Actions.MouseEnter = launchAction
    /// 'Add the submit button to a new document.
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("FieldActions.pdf")
    /// </code>
    /// </example>
    public class PdfFieldActions : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Interanal variable to store annotation's actions.
        /// </summary>
        private PdfAnnotationActions m_annotationActions = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store key pressed action.
        /// </summary>
        private PdfJavaScriptAction m_keyPressed = null;

        /// <summary>
        /// Internal variable to store format action.
        /// </summary>
        private PdfJavaScriptAction m_format = null;

        /// <summary>
        /// Internal variable to store validate action.
        /// </summary>
        private PdfJavaScriptAction m_validate = null;

        /// <summary>
        /// Internal variable to store calculate action.
        /// </summary>
        private PdfJavaScriptAction m_calculate = null;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFieldActions"/> class.
        /// </summary>
        /// <param name="annotationActrions">The annotation actions.</param>
        public PdfFieldActions(PdfAnnotationActions annotationActions)
        {
            if (annotationActions == null)
            {
                throw new ArgumentNullException("annotationActrions");
            }

            m_annotationActions = annotationActions;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the JavaScript action to be performed when the user types a keystroke 
        /// into a text field or combo box or modifies the selection in a scrollable list box. 
        /// This action can check the keystroke for validity and reject or modify it.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed when the user types a keystroke.</value>
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
        /// submitButton.Actions.KeyPressed = javaAction;
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
        /// submitButton.Actions.KeyPressed = javaAction
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction KeyPressed
        {
            get
            {
                return m_keyPressed;
            }
           
            set
            {
                if (m_keyPressed != value)
                {
                    m_keyPressed = value;
                    m_dictionary.SetProperty(DictionaryProperties.K, m_keyPressed);
                }
            }
        }

        /// <summary>
        /// Gets or sets the JavaScript action to be performed before the field is formatted 
        /// to display its current value. This action can modify the field�s value before formatting.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed for formating the field value.</value>
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
        /// submitButton.Actions.Format = javaAction;
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
        /// submitButton.Actions.Format = javaAction
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction Format
        {
            get
            {
                return m_format;
            }
          
            set
            {
                if (m_format != value)
                {
                    m_format = value;
                    m_dictionary.SetProperty(DictionaryProperties.F, m_format);
                }
            }
        }

        /// <summary>
        /// Gets or sets the JavaScript action to be performed when the field�s value is changed. 
        /// This action can check the new value for validity.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed for validating the field value.</value>
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
        /// submitButton.Actions.Validate = javaAction;
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
        /// submitButton.Actions.Validate = javaAction
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction Validate
        {
            get
            {
                return m_validate;
            }
           
            set
            {
                if (m_validate != value)
                {
                    m_validate = value;
                    m_dictionary.SetProperty(DictionaryProperties.V, m_validate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the JavaScript action to be performed to recalculate the value 
        /// of this field when that of another field changes.
        /// </summary>
        /// <value>A <see cref="PdfJavaScriptAction"/> object specifying the action to be executed for calculating the field value.</value>
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
        /// submitButton.Actions.Calculate = javaAction;
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
        /// submitButton.Actions.Calculate = javaAction
        /// 'Add the  submit button to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("JavaScriptAction.pdf")
        /// </code>
        /// </example>
        public PdfJavaScriptAction Calculate
        {
            get
            {
                return m_calculate;
            }
           
            set
            {
                if (m_calculate != value)
                {
                    m_calculate = value;
                    m_dictionary.SetProperty(DictionaryProperties.C, m_calculate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the cursor enters the fields�s 
        /// area.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the mouse enters the field's area.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseEnter = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseEnter = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseEnter
        {
            get
            {
                return m_annotationActions.MouseEnter;
            }
           
            set
            {
                m_annotationActions.MouseEnter = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the cursor exits the fields�s 
        /// area.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the mouse leaves the field's area.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseLeave = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseLeave = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseLeave
        {
            get
            {
                return m_annotationActions.MouseLeave;
            }
           
            set
            {
                m_annotationActions.MouseLeave = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is released 
        /// inside the field�s area.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the mouse button is released inside the field's area.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseUp = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseUp = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseUp
        {
            get
            {
                return m_annotationActions.MouseUp;
            }
         
            set
            {
                m_annotationActions.MouseUp = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is pressed inside the 
        /// field�s area.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the mouse button is pressed inside the field's area.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseDown = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseDown = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>       
        public PdfAction MouseDown
        {
            get
            {
                return m_annotationActions.MouseDown;
            }
           
            set
            {
                m_annotationActions.MouseDown = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the field receives the 
        /// input focus.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the field receives the input focus.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.GotFocus = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction GotFocus
        {
            get
            {
                return m_annotationActions.GotFocus;
            }
           
            set
            {
                m_annotationActions.GotFocus = value;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the field loses the 
        /// input focus.
        /// </summary>
        /// <value>A <see cref="PdfAction"/> descendant specifying the action to be executed when the field losts the input focus.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.LostFocus = launchAction;
        /// Add the submit button to a new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.LostFocus = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction LostFocus
        {
            get
            {
                return m_annotationActions.LostFocus;
            }
           
            set
            {
                m_annotationActions.LostFocus = value;
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
